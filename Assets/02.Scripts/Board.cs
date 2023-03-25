using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Node_
{
    public Node_(bool _isWall, int _x, int _z) { isWall = _isWall; x = _x; z = _z; }

    public bool isWall;
    public Node_ ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, z, G, H;
    public int F { get { return G + H; } }
}

public class Board : MonoBehaviour
{
    
    public Vector3Int bottomLeft, topRight; 
    public Vector3Int startPos, targetPos;
    public List<Node_> FinalNodeList;
    public bool allowDiagonal, dontCrossCorner;

    int sizeX, sizeZ;
    Node_[,] NodeArray;
    Node_ StartNode, TargetNode, CurNode;
    List<Node_> OpenList, ClosedList;
    Collider[] collision;
    private Transform tr;

    public Vector3 center;
    public Vector3 size;
    public LayerMask layerMask;
    
    // 감지해서 출력
    private void Start()
    {
        // Test();
        // GetWallInfo();
    }

    
    public void Test()
    {
        
        collision = Physics.OverlapBox(center, size, Quaternion.identity, layerMask);       // 검은 큐브 
        for (int i = 0; i < collision.Length; i++)
        {
            Debug.Log("collision 물체: " + collision[i].transform.name);
            Debug.Log("collision 좌표: " + collision[i].transform.position);
        }
        print(collision.Length);
    }
    public void GetWallInfo()
    {
        sizeX = topRight.x - bottomLeft.x + 1;      // 27
        sizeZ = topRight.z - bottomLeft.z + 1;      // 27
        
        NodeArray = new Node_[sizeX, sizeZ];
        
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                bool isWall = false;
                // collision = Physics.OverlapBox(new Vector3(i + bottomLeft.x, 0f, j + bottomLeft.z), Vector3.one * 0.5f, Quaternion.identity, layerMask);
                foreach (var collider in Physics.OverlapBox(new Vector3(i + bottomLeft.x, 0f, j + bottomLeft.z), Vector3.one * 0.4f, Quaternion.identity, layerMask))
                {
                    if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;
                    // Debug.Log(collider.gameObject.name);
                }
                NodeArray[i, j] = new Node_(isWall, i + bottomLeft.x, j + bottomLeft.z);
            }
        }
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                Node_ node = NodeArray[i, j];
                Debug.Log($"Node [{i},{j}]: isWall={node.isWall}, x={node.x}, z={node.z}");
            }
        }

    }
    
    void OnDrawGizmos()
    {
        // Gizmos.color = Color.black;
        // Gizmos.DrawWireCube(center, size * 2);
        
        if(FinalNodeList.Count != 0) for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector3(FinalNodeList[i].x,1f ,FinalNodeList[i].z), new Vector3(FinalNodeList[i + 1].x,1f, FinalNodeList[i + 1].z));
    }
    
    public void PathFinding()
    {
        // NodeArray의 크기 정해주고, isWall, x, y 대입
        sizeX = topRight.x - bottomLeft.x + 1;      // 27
        sizeZ = topRight.z - bottomLeft.z + 1;      // 27
        
        NodeArray = new Node_[sizeX, sizeZ];
        
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                bool isWall = false;
                // collision = Physics.OverlapBox(new Vector3(i + bottomLeft.x, 0f, j + bottomLeft.z), Vector3.one * 0.5f, Quaternion.identity, layerMask);
                foreach (var collider in Physics.OverlapBox(new Vector3(i + bottomLeft.x, 0f, j + bottomLeft.z), Vector3.one * 0.4f, Quaternion.identity, layerMask))
                {
                    if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;
                    // Debug.Log(collider.gameObject.name);
                }
                NodeArray[i, j] = new Node_(isWall, i + bottomLeft.x, j + bottomLeft.z);
            }
        }

        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.z - bottomLeft.z];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.z - bottomLeft.z];

        OpenList = new List<Node_>() { StartNode };
        ClosedList = new List<Node_>();
        FinalNodeList = new List<Node_>();

        
        while (OpenList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // 마지막
            if (CurNode == TargetNode)
            {
                Node_ TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();

                for (int i = 0; i < FinalNodeList.Count; i++)
                {
                    print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].z);
                }
                return;
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.z + 1);
            OpenListAdd(CurNode.x + 1, CurNode.z);
            OpenListAdd(CurNode.x, CurNode.z - 1);
            OpenListAdd(CurNode.x - 1, CurNode.z);
        }
    }

    void OpenListAdd(int checkX, int checkZ)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkZ >= bottomLeft.z && checkZ < topRight.z + 1 && !NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z].isWall && !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z]))
        {
            // 코너를 가로질러 가지 않을시, 이동 중에 수직수평 장애물이 있으면 안됨
            if (dontCrossCorner) if (NodeArray[CurNode.x - bottomLeft.x, checkZ - bottomLeft.z].isWall || NodeArray[checkX - bottomLeft.x, CurNode.z - bottomLeft.z].isWall) return;

            
            // 이웃노드에 넣고, 직선은 10, 대각선은 14비용
            Node_ NeighborNode = NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z];
            int MoveCost = CurNode.G + (CurNode.x - checkX == 0 || CurNode.z - checkZ == 0 ? 10 : 14);


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.z - TargetNode.z)) * 10;
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }

    public void DFS()
    {
        
    }
    
}
