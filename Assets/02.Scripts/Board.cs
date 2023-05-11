using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class Node_
{
    public Node_(bool _isWall, bool _isVisited, int _x, int _z) { isWall = _isWall;
        isVisited = _isVisited; x = _x; z = _z; }

    public bool isWall;
    public bool isVisited;
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
    public List<Node_> DFSList;
    public bool dontCrossCorner;
    [FormerlySerializedAs("player")] public PlayerController playerController;
    
    int sizeX, sizeZ;
    Node_[,] NodeArray;
    Node_ StartNode, TargetNode, CurNode, UnreachableNode;
    List<Node_> OpenList, ClosedList;
    Collider[] collision;
    private Transform tr;
    private Node node;
    
    public Vector3 center;
    public Vector3 size;
    public LayerMask layerMask;
    
    private void Awake()
    {
        node = GetComponentInChildren<Node>();
        tr = playerController.GetComponent<Transform>();
    }
    
    private void Start()
    {
        // DFS();
    }
    
    /*public void FollowFinalNodeList(GameObject targetTile)
    {
        // for(int i = 0; i < transform.childCount; i++)
        // {
        //     Transform child = transform.GetChild(i);
        //     Vector3Int TilePosition = new Vector3Int(Mathf.RoundToInt(child.position.x), 0, Mathf.RoundToInt(child.position.z));
        //     if(TilePosition == targetTile)
        // }
        Vector3Int TilePosition = new Vector3Int(Mathf.RoundToInt(targetTile.transform.position.x), 0,
                                                Mathf.RoundToInt(targetTile.transform.position.z));
        targetPos = TilePosition;
        // 갱신한 targetPos로 길찾기
        PathFinding();
        // PathFinding_player();
        // 플레이어 이동
        player.FollowPath(FinalNodeList);
        // 시작점을 최종 노드로 설정
        startPos = new Vector3Int(FinalNodeList[FinalNodeList.Count - 1].x,0,FinalNodeList[FinalNodeList.Count - 1].z);
        // 타일 클릭 초기화
        SetAllChildrenIsClickedFalse();
        // 갈수 있는 타일 검색
        DFS();  // GM으로 옮기고 DFS_player();매개변수 받아서 호출해보자. 
    }*/
    
    public void FollowFinalNodeList_player(GameObject targetTile, GameObject player)
    {
        Vector3Int TilePosition = new Vector3Int(Mathf.RoundToInt(targetTile.transform.position.x), 0,
            Mathf.RoundToInt(targetTile.transform.position.z));
        targetPos = TilePosition;
        // 갱신한 targetPos로 길찾기
        PathFinding_player(player);
        // 플레이어 이동
        player.GetComponent<PlayerController>().FollowPath(FinalNodeList);
        // 시작점을 최종 노드로 설정
        startPos = new Vector3Int(FinalNodeList[FinalNodeList.Count - 1].x,0,FinalNodeList[FinalNodeList.Count - 1].z);
        // 타일 클릭 초기화
        SetAllChildrenIsClickedFalse();
        // 갈수 있는 타일 검색
        DFS_player(player);  // GM으로 옮기고 DFS_player();매개변수 받아서 호출해보자. 
    }
    

    /*public void SetTargetpos(Vector3 pos)
    {
        if (targetPos == startPos) return;
        if (targetPos != startPos)
        {
            SetStartpos(targetPos);     // 출발점이 목표점이 된다. (도착하면 도착한 곳이 다음 시작점이기 때문)
        }
        targetPos = new Vector3Int(Mathf.RoundToInt(pos.x), 0, Mathf.RoundToInt(pos.z));  // 도착점 = 클릭한 타일 위치
    }
    
    private void SetStartpos(Vector3Int pos)
    {
        startPos = pos;
    }*/
    
    // 감지해서 출력
    
    public void SetAllChildrenIsClickedFalse()
    {
        Node[] childNodes = GetComponentsInChildren<Node>();
        foreach (Node childNode in childNodes)
            childNode.isPushed = false;
    }

    public void Test()
    {
        collision = Physics.OverlapBox(center, size, Quaternion.identity, layerMask); // 검은 큐브 
        for (int i = 0; i < collision.Length; i++)
        {
            Debug.Log("collision 물체: " + collision[i].transform.name);
            Debug.Log("collision 좌표: " + collision[i].transform.position);
        }

        print(collision.Length);
    }

    public void GetWallInfo()
    {
        sizeX = topRight.x - bottomLeft.x + 1; // 21
        sizeZ = topRight.z - bottomLeft.z + 1; // 21

        NodeArray = new Node_[sizeX, sizeZ];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                bool isWall = false;
                bool isVisited = false;
                // collision = Physics.OverlapBox(new Vector3(i + bottomLeft.x, 0f, j + bottomLeft.z), Vector3.one * 0.5f, Quaternion.identity, layerMask);
                foreach (var collider in Physics.OverlapBox(new Vector3(i + bottomLeft.x, 0f, j + bottomLeft.z),
                             Vector3.one * 0.4f, Quaternion.identity, layerMask))
                {
                    if (collider.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;
                    // Debug.Log(collider.gameObject.name);
                }

                NodeArray[i, j] = new Node_(isWall, isVisited,i + bottomLeft.x, j + bottomLeft.z);
            }
        }

        /*for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                Node_ node = NodeArray[i, j];
                Debug.Log($"Node [{i},{j}]: isWall={node.isWall}, x={node.x}, z={node.z}");
            }
        }*/

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(center, size * 2);

        if (FinalNodeList.Count != 0)
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
                Gizmos.DrawLine(new Vector3(FinalNodeList[i].x, 1f, FinalNodeList[i].z),
                    new Vector3(FinalNodeList[i + 1].x, 1f, FinalNodeList[i + 1].z));
        
        // if (FinalNodeList.Count != 0)
        //     for (int i = 0; i < FinalNodeList.Count; i++)
        //         Gizmos.DrawCube(new Vector3(FinalNodeList[i].x, 2f, FinalNodeList[i].z), Vector3.one);
        
        Gizmos.color = Color.yellow;
        if (DFSList.Count != 0)
            for (int i = 0; i < DFSList.Count; i++)
                Gizmos.DrawCube(new Vector3(DFSList[i].x, 10f, DFSList[i].z), Vector3.one);
    }
    public void setDFS(GameObject player)
    {
        startPos = new Vector3Int((int)player.transform.position.x, 0,(int)player.transform.position.z);
        // 타일 클릭 초기화
        SetAllChildrenIsClickedFalse();
        // 갈수 있는 타일 검색
        DFS_player(player);
    }
    
    public void PathFinding()
    {
        GetWallInfo();

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
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                    CurNode = OpenList[i];

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

                // for (int i = 0; i < FinalNodeList.Count; i++)
                // {
                //     print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].z);
                // }

                return;
            }

            // ↑ → ↓ ←
            OpenListAdd(CurNode.x, CurNode.z + 1);
            OpenListAdd(CurNode.x + 1, CurNode.z);
            OpenListAdd(CurNode.x, CurNode.z - 1);
            OpenListAdd(CurNode.x - 1, CurNode.z);
        }
    }
    
    public void PathFinding_player(GameObject player)
    {
        GetWallInfo();

        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[(int)player.transform.position.x - bottomLeft.x, 
                            (int)player.transform.position.z - bottomLeft.z];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.z - bottomLeft.z];
        
        OpenList = new List<Node_>() { StartNode };
        ClosedList = new List<Node_>();
        FinalNodeList = new List<Node_>();

        while (OpenList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F <= CurNode.F && OpenList[i].H < CurNode.H)
                    CurNode = OpenList[i];

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

                // for (int i = 0; i < FinalNodeList.Count; i++)
                // {
                //     print(i + "번째는 " + FinalNodeList[i].x + ", " + FinalNodeList[i].z);
                // }

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
        if (checkX >= bottomLeft.x && checkX < topRight.x + 1 && checkZ >= bottomLeft.z && checkZ < topRight.z + 1 &&
            !NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z].isWall &&
            !ClosedList.Contains(NodeArray[checkX - bottomLeft.x, checkZ - bottomLeft.z]))
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
        GetWallInfo();
        // 시작노드의 문제인가... start pos의 조정이니 여기에 player정보를 가져오는 것도 방법인듯 하다. 
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.z - bottomLeft.z];
        StartNode.isVisited = true;
        DFSList = new List<Node_>() { StartNode };
        CurNode = DFSList[0];
        // 호출
        DFSListAdd(CurNode);
    }
    
    public void DFS_player(GameObject player)
    {
        GetWallInfo();
        // 시작노드의 문제인가... start pos의 조정이니 여기에 player정보를 가져오는 것도 방법인듯 하다. 
        StartNode = NodeArray[(int)player.transform.position.x - bottomLeft.x, (int)player.transform.position.z - bottomLeft.z];
        StartNode.isVisited = true;
        DFSList = new List<Node_>() { StartNode };
        CurNode = DFSList[0];
        // 호출
        DFSListAdd(CurNode);
    }
    
    void DFSListAdd(Node_ currentNode)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아닌지 확인
        // 다음 노드가 방문했던 노드인지? OR 다음 노드가 벽인지? 확인
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // 다음 노드의 좌표
                int nextX = currentNode.x + i;
                int nextZ = currentNode.z + j;
                // 다음 노드가 NodeArray를 벗어나는지 확인 && 다음 노드가 방문했던 노드인지? && 다음 노드가 벽인지? 확인
                if (nextX >= bottomLeft.x && nextX <= topRight.x && nextZ >= bottomLeft.z && nextZ <= topRight.z &&
                    !NodeArray[nextX - bottomLeft.x, nextZ - bottomLeft.z].isVisited && !NodeArray[nextX - bottomLeft.x, nextZ - bottomLeft.z].isWall)
                {
                    NodeArray[nextX - bottomLeft.x, nextZ - bottomLeft.z].isVisited = true;
                    Node_ NeighborNode = NodeArray[nextX - bottomLeft.x, nextZ - bottomLeft.z];
                    DFSList.Add(NeighborNode);
                    DFSListAdd(NeighborNode);
                }
            }
        }
    }

    
}
