using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    public Color moveAvailableColor;
    private Color clickedColor;
    private Transform tr;
    private Board board;
    public bool isClicked = false;
    private Vector3 tileposition;
    
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        tr = GetComponent<Transform>();
        // originalColor = rend.material.color;
        originalColor = GetComponent<Renderer>().material.color;
        clickedColor = originalColor * 0.8f;
        board = GetComponentInParent<Board>();
        // print(isDFS(board.DFSList));
    }

    private void Update()
    {
        // isDFS(board.DFSList);
    }

    private void OnMouseEnter()
    {
        if (isDFS(board.DFSList) && !isClicked)
            rend.material.color = moveAvailableColor;
    }
    
    private void OnMouseExit()
    {
        rend.material.color = originalColor;
    }
    
    private void OnMouseDown()
    {
        // 한번 클릭하면 다신 클릭 못하게 함.
        // dfs상 도달 가능하면 클릭 가능
        if (isDFS(board.DFSList))
        {
            if (isClicked) return;
            rend.material.color = clickedColor;
            isClicked = true;
            GetComponent<Renderer>().material.color = clickedColor;
            // 플레이어 이동, startpos,Targetpos 수정 함수 호출
            tileposition = transform.position;
            board.FollowFinalNodeList(gameObject);
        }
        else
        {
            print("그곳엔 이동할 수 없습니다.");
            rend.material.color = originalColor;
        }
    }

    
    
    private void OnMouseUp()
    {
        // isClicked = false;
        GetComponent<Renderer>().material.color = originalColor;  // 기존 색상으로 변경
    }
    
    
    private bool isDFS(List<Node_> DFSList)
    {
        Vector3Int TilePosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), 0, Mathf.RoundToInt(transform.position.z));
        // 현재 타일의 위치를 가져왔는데 -5나 특이한 곳에 있는 타일이 있다는 것이다... 
        // 한번 찍어보자
        for(int i  =0 ; i < DFSList.Count ; i++)
            if (Mathf.Abs(TilePosition.x - DFSList[i].x) < 0.1 && Mathf.Abs(TilePosition.z - DFSList[i].z) <0.1 )
                return true;
        return false;
    }
    

    public void OnMoveto(Vector3 end)
    {
        StartCoroutine("MoveTo",end);
        // 중간에 다른 이동 경로가 있는 경우.
    }

    public void OnMoveto_(List<GameObject> waypointList)
    {
        StartCoroutine(MoveToNextTarget(waypointList));
    }
    
    
    private IEnumerator MoveTo(Vector3 end)
    {
        float	current  = 0;
        float	percent  = 0;
        float	moveTime = 1.0f;
        Vector3	start	 = tr.position; // 본인 위치

        while ( percent < 1 )
        {
            current += Time.deltaTime;
            percent = current / moveTime;
            // 움직이는 코드
            tr.position = Vector3.Lerp(start, end , percent);
            
            yield return null;
        }
        tr.position = end;
    }
    
    float timeToReachTarget = 1f;

    private int currentIndex = 0;
    private bool isMoving = false;
    
    private IEnumerator MoveToNextTarget(List<GameObject> waypointList)
    {
        isMoving = true;

        while (currentIndex < waypointList.Count)
        {
            Vector3 targetPosition = waypointList[currentIndex].transform.position;
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            float speed = distanceToTarget / timeToReachTarget;

            float elapsedTime = 0f;
            while (elapsedTime < timeToReachTarget)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime * speed / distanceToTarget);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            currentIndex++;
        }

        isMoving = false;
        yield return null;
    }
}
