using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BoardInfo : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject SpawnObject;
    public GameObject SpawnPoint;
    public GameObject[] waypoints;
    public GameObject[] cornerPoint;
    public Vector3[] center_x = new Vector3[5];
    public Vector3[] center_z = new Vector3[5];
    public Vector3 size_x;
    public Vector3 size_z;
    public LayerMask layerMask;
    WaitForSeconds delay1 = new WaitForSeconds(2f);
    private Renderer[] pushArea_Render;
    public Material originalColor;
    
    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        SpawnObject = gameManager.rotatingObject;
        SpawnObject.GetComponent<Node>().isSelected = true;
        pushArea_Render = new Renderer[waypoints.Length];
        for (int i = 0; i < waypoints.Length; i++)
        {
            pushArea_Render[i] = waypoints[i].GetComponent<Renderer>();
        }
    }
    void OnDrawGizmos()
    {
        // 기즈모 색상 지정
        Gizmos.color = Color.blue;
        Gizmos.matrix = transform.localToWorldMatrix;
        // 기즈모로 범위 그리기
        for (int i = 0; i < center_x.Length; i++)
            Gizmos.DrawWireCube(center_x[i]/3, size_x);
        
        for (int i = 0; i < center_z.Length; i++)
            Gizmos.DrawWireCube(center_z[i]/3, size_z);
        
    }

    public void DragTile(GameObject rotating, string info)
    {
        char location = info[0];
        int num = int.Parse(info.Substring(1));

        switch (location)
        {
            case 'L':
                rotating.GetComponent<Node>().OnMoveto(waypoints[3*1+num].transform.position);
                break;
            case 'R':
                rotating.GetComponent<Node>().OnMoveto(waypoints[3*2+num].transform.position);
                break;
            case 'T':
                rotating.GetComponent<Node>().OnMoveto(waypoints[num].transform.position);
                break;
            case 'B':
                rotating.GetComponent<Node>().OnMoveto(waypoints[3*3+num].transform.position);
                break;
        }

        
        
    }
    
    
    public void PushNode(string info)
    {
        // 매개변수 받기
        char location = info[0];
        int num = int.Parse(info.Substring(1));
        List<Transform> lastOnePath = new List<Transform>();
        
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
        /*for (int i = 0; i < sortedColliders_x.Length; i++)
        {
            for (int j = 0; j < sortedColliders_x[i].Length; j++)
            {
                Debug.Log($"x[{i}][{j}] {sortedColliders_x[i][j].transform.position}");
            }
        }
        for (int i = 0; i < sortedColliders_z.Length; i++)
        {
            for (int j = 0; j < sortedColliders_z[i].Length; j++)
            {
                Debug.Log($"z[{i}][{j}] {sortedColliders_z[i][j].transform.position}");
                Debug.Log($"z[{i}][{j}] {sortedColliders_z[i][j].GetComponent<Node>()}");
            }
        }*/

        // 이동
        switch (location)
        {
            case 'L':
                // 1칸씩 이동
                for (int i = 0; i < sortedColliders_x[num].Length - 1; i++)
                    sortedColliders_x[num][i].gameObject.GetComponent<Node>().OnMoveto(sortedColliders_x[num][i+1].transform.position);
                
                // 마지막거 이동
                lastOnePath.Add(waypoints[3*2+num].transform); // 오른쪽 지점
                lastOnePath.Add(SpawnPoint.transform);
                sortedColliders_x[num][sortedColliders_x[num].Length - 1].gameObject.GetComponent<Node>().OnMoveto_(lastOnePath);
                
                // 넣을 타일 이동
                SpawnObject.GetComponent<Node>().OnMoveto(sortedColliders_x[num][0].transform.position);
                // 넣은 놈의 isSelected는 해제
                SpawnObject.gameObject.GetComponent<Node>().isSelected = false;
                // sortedColliders_x[num][sortedColliders_x[num].Length - 1].gameObject.GetComponent<Node>().OnMoveto(SpawnObject.transform.position);
                SpawnObject = sortedColliders_x[num][sortedColliders_x[num].Length - 1].gameObject;
                gameManager.rotatingObject = sortedColliders_x[num][sortedColliders_x[num].Length - 1].gameObject;
                resetBtn();
                waypoints[3*2+num].SetActive(false);
                break;
            case 'R':
                // 1칸씩 이동
                for (int i = sortedColliders_x[num].Length - 1; i > 0; i--)
                    sortedColliders_x[num][i].gameObject.GetComponent<Node>().OnMoveto(sortedColliders_x[num][i-1].transform.position);
                
                // 마지막거 이동
                lastOnePath.Add(waypoints[3*1+num].transform); // 왼쪽 지점
                lastOnePath.Add(cornerPoint[2].transform);        // 왼쪽 아래 코너지점
                lastOnePath.Add(cornerPoint[0].transform);        // 오른쪽 아래 코너지점
                lastOnePath.Add(SpawnPoint.transform);
                sortedColliders_x[num][0].gameObject.GetComponent<Node>().OnMoveto_(lastOnePath);
                
                // SpawnObject 이동
                SpawnObject.GetComponent<Node>().OnMoveto(sortedColliders_x[num][sortedColliders_x[num].Length - 1].transform.position);
                // 넣은 놈의 isSelected는 해제
                SpawnObject.gameObject.GetComponent<Node>().isSelected = false;
                // sortedColliders_x[num][0].gameObject.GetComponent<Node>().OnMoveto(SpawnObject.transform.position);
                SpawnObject = sortedColliders_x[num][0].gameObject;
                gameManager.rotatingObject = sortedColliders_x[num][0].gameObject;
                resetBtn();
                waypoints[3*1+num].SetActive(false);
                break;
            case 'T':
                // 1칸씩 이동
                for (int i = sortedColliders_z[2-num].Length - 1; i > 0 ; i--)
                    sortedColliders_z[2-num][i].gameObject.GetComponent<Node>().OnMoveto(sortedColliders_z[2-num][i-1].transform.position);
                
                // 마지막거 이동
                lastOnePath.Add(waypoints[3*3+num].transform); // 아래쪽 지점
                lastOnePath.Add(cornerPoint[0].transform); // 오른쪽 아래 코너지점
                lastOnePath.Add(SpawnPoint.transform);
                sortedColliders_z[2-num][0].gameObject.GetComponent<Node>().OnMoveto_(lastOnePath);
                
                // SpawnObject 이동
                SpawnObject.GetComponent<Node>().OnMoveto(sortedColliders_z[2-num][sortedColliders_z[2-num].Length - 1].transform.position);
                // 넣은 놈의 isSelected는 해제
                SpawnObject.gameObject.GetComponent<Node>().isSelected = false;
                
                SpawnObject = sortedColliders_z[2-num][0].gameObject;
                gameManager.rotatingObject = sortedColliders_z[2-num][0].gameObject;
                resetBtn();
                waypoints[3*3+num].SetActive(false);
                break;
            case 'B':
                // 1칸씩 이동
                for (int i = 0; i < sortedColliders_z[2-num].Length - 1; i++)
                    sortedColliders_z[2-num][i].gameObject.GetComponent<Node>().OnMoveto(sortedColliders_z[2-num][i+1].transform.position);
                
                // 마지막거 이동
                lastOnePath.Add(waypoints[3*0+num].transform); // 위쪽 지점
                lastOnePath.Add(cornerPoint[1].transform); // 오른쪽 위 코너지점
                lastOnePath.Add(SpawnPoint.transform);
                sortedColliders_z[2-num][sortedColliders_z[2-num].Length - 1].gameObject.GetComponent<Node>().OnMoveto_(lastOnePath);

                // SpawnObject 이동
                SpawnObject.gameObject.GetComponent<Node>().OnMoveto(sortedColliders_z[2-num][0].transform.position);
                // 넣은 놈의 isSelected는 해제
                SpawnObject.gameObject.GetComponent<Node>().isSelected = false;
                SpawnObject = sortedColliders_z[2-num][sortedColliders_z[2-num].Length - 1].gameObject;
                gameManager.rotatingObject = sortedColliders_z[2-num][sortedColliders_z[2-num].Length - 1].gameObject;
                resetBtn();
                waypoints[3*0+num].SetActive(false);
                break;
        }
        // 클릭 여부 점검을 위해 추가
        SpawnObject.GetComponent<Node>().isSelected = true;
        SpawnObject.GetComponent<Node>().isPushed = true;       // <- 이거 추가하니까 해결됨..
        // 솔직히 이해는 안됨.. rotatingObject는 놓는 순간 바뀌는데 어캐 이게 적용되노..
        // 플레이어 위치 갱신하고 DFS 호출하는 함수 호출
        // board.SetStartpos();
    }

    private void resetBtn()
    {
        foreach (var VARIABLE in waypoints)
        {
            VARIABLE.SetActive(true);
        }
    }
    
    public void DisableBtn(GameObject PushArea)
    {
        foreach (var VARIABLE in pushArea_Render)
        {
            VARIABLE.material = originalColor;
        }
        StartCoroutine("DisablePushArea", PushArea);
        // SetActive(false);
    }
    public IEnumerator DisablePushArea(GameObject PushArea)
    {
        PushArea.gameObject.SetActive(false);
        yield return delay1;
        PushArea.gameObject.SetActive(true);
    }
    // 민 쪽의 반대쪽은 불가
}

