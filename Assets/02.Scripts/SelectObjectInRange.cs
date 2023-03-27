using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SelectObjectInRange : MonoBehaviour
{
    private GameManager gameManager;
    private Board board;

    public GameObject SpawnObject;
    public GameObject SpawnPoint;
    public GameObject[] waypoints;
    public GameObject[] cornerPoint;
    public Vector3[] center_x = new Vector3[5];
    public Vector3[] center_z = new Vector3[5];
    public Vector3 size_x;
    public Vector3 size_z;
    public LayerMask layerMask;
    public LayerMask layerMask_2;
    WaitForSeconds delay1 = new WaitForSeconds(1f);
    
    void OnDrawGizmos()
    {
        // 기즈모 색상 지정
        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        // 기즈모로 범위 그리기
        for (int i = 0; i < center_x.Length; i++)
        {
            Gizmos.DrawWireCube(center_x[i]/3, size_x);
        }
        for (int i = 0; i < center_z.Length; i++)
        {
            Gizmos.DrawWireCube(center_z[i]/3, size_z);
        }
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        board = FindObjectOfType<Board>();
        SpawnObject = gameManager.rotatingObject;
    }

    private void Start()
    {
    }


    
    public void PushNode(string info)
    {
        // 매개변수 받기
        string[] Info = info.Split(' ');
        string location = Info[0];
        int num = int.Parse(Info[1]);
        
        Transform previousTransform;
        Transform spawnTransform;

        Collider[][] hitColliders_x = new Collider[center_x.Length][];
        Collider[][] hitColliders_z = new Collider[center_z.Length][];
        // 히트 정보 저장 및 정렬(x 좌표를 기준으로 오름차순 정렬)
        for (int i = 0; i < center_x.Length; i++)
        {
            hitColliders_x[i] = Physics.OverlapBox(transform.position + center_x[i], size_x, Quaternion.identity, layerMask);
            Array.Sort(hitColliders_x[i], (c1, c2) => c1.bounds.center.x.CompareTo(c2.bounds.center.x));
            
            hitColliders_z[i] = Physics.OverlapBox(transform.position + center_z[i], size_z, Quaternion.identity,layerMask);
            Array.Sort(hitColliders_z[i], (c1, c2) => c1.bounds.center.z.CompareTo(c2.bounds.center.z));
        }
        Collider[][] sortedColliders_x = hitColliders_x;
        Collider[][] sortedColliders_z = hitColliders_z;
        
        // 디버깅
        // for (int i = 0; i < sortedColliders_x.Length; i++)
        // {
        //     for (int j = 0; j < sortedColliders_x[i].Length; j++)
        //     {
        //         Debug.Log($"x[{i}][{j}] {sortedColliders_x[i][j].transform.position}");
        //     }
        // }
        // for (int i = 0; i < sortedColliders_z.Length; i++)
        // {
        //     for (int j = 0; j < sortedColliders_z[i].Length; j++)
        //     {
        //         Debug.Log($"z[{i}][{j}] {sortedColliders_z[i][j].transform.position}");
        //     }
        // }

        // 이동
        switch (location)
        {
            case "Left":
                for (int i = 0; i < sortedColliders_x[num].Length - 1; i++)
                    sortedColliders_x[num][i].gameObject.GetComponent<Node>().OnMoveto(sortedColliders_x[num][i+1].transform.position);
                
                sortedColliders_x[num][sortedColliders_x[num].Length - 1].gameObject.GetComponent<Node>().OnMoveto(SpawnObject.transform.position);
                SpawnObject.GetComponent<Node>().OnMoveto(sortedColliders_x[num][0].transform.position);
                SpawnObject = sortedColliders_x[num][sortedColliders_x[num].Length - 1].gameObject;
                gameManager.rotatingObject = sortedColliders_x[num][sortedColliders_x[num].Length - 1].gameObject;
                break;
            case "Right":
                for (int i = sortedColliders_x[num].Length - 1; i > 0; i--)
                    sortedColliders_x[num][i].gameObject.GetComponent<Node>().OnMoveto(sortedColliders_x[num][i-1].transform.position);
                
                sortedColliders_x[num][0].gameObject.GetComponent<Node>().OnMoveto(SpawnObject.transform.position);
                SpawnObject.GetComponent<Node>().OnMoveto(sortedColliders_x[num][sortedColliders_x[num].Length - 1].transform.position);
                SpawnObject = sortedColliders_x[num][0].gameObject;
                gameManager.rotatingObject = sortedColliders_x[num][0].gameObject;
                break;
            case "Top":
                for (int i = sortedColliders_z[num].Length - 1; i > 0 ; i--)
                    sortedColliders_z[num][i].gameObject.GetComponent<Node>().OnMoveto(sortedColliders_z[num][i-1].transform.position);
            
                sortedColliders_z[num][0].gameObject.GetComponent<Node>().OnMoveto(SpawnObject.transform.position);
                SpawnObject.GetComponent<Node>().OnMoveto(sortedColliders_z[num][sortedColliders_z[num].Length - 1].transform.position);
                SpawnObject = sortedColliders_z[num][0].gameObject;
                gameManager.rotatingObject = sortedColliders_z[num][0].gameObject;
                break;
            case "Bottom":
                // previousTransform = sortedColliders_z[num][1].gameObject.transform;
                // spawnTransform = SpawnObject.transform;
                // List<GameObject> selectedPath = new List<GameObject>();
                List<GameObject> lastOnePath = new List<GameObject>();

                // selectedPath.Add(cornerPoint[0]); // 오른쪽 아래 코너지점
                // selectedPath.Add(sortedColliders_z[num][0].gameObject); // 아래지점
                // selectedPath.Add(waypoints[num+8]);     // 콜라이더가 움직여서 바뀜
                
                lastOnePath.Add(sortedColliders_z[num][sortedColliders_z[num].Length - 1].gameObject); // 위쪽 지점
                lastOnePath.Add(cornerPoint[1]); // 오른쪽 위 코너지점
                lastOnePath.Add(SpawnPoint);
                
                // SpawnObject.GetComponent<Node>().OnMoveto_(selectedPath);
                sortedColliders_z[num][sortedColliders_z[num].Length - 2].gameObject.GetComponent<Node>().OnMoveto_(lastOnePath);
                
                for (int i = 1; i < sortedColliders_z[num].Length - 2; i++)
                {
                    sortedColliders_z[num][i].gameObject.GetComponent<Node>().OnMoveto(sortedColliders_z[num][i+1].transform.position);
                }
                
                // spawn object 갱신
                SpawnObject = sortedColliders_z[num][sortedColliders_z[num].Length - 2].gameObject;
                gameManager.rotatingObject = sortedColliders_z[num][sortedColliders_z[num].Length - 2].gameObject;
                
                break;
        }
        // 플레이어 위치 갱신하고 DFS 호출하는 함수 호출
        // board.SetStartpos();
    }


    public void DisableUI(GameObject UIPanel)
    {
        StartCoroutine("DisableButtonForFiveSeconds", UIPanel);
    }
    public IEnumerator DisableButtonForFiveSeconds(GameObject UIPanel)
    {
        UIPanel.gameObject.SetActive(false);
        yield return delay1;
        UIPanel.gameObject.SetActive(true);
        
    }
}

