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
        // if (isDFS(board.DFSList) && !isClicked)
            rend.material.color = moveAvailableColor;
    }
    
    private void OnMouseExit()
    {
        rend.material.color = originalColor;
    }
    
    private void OnMouseDown()
    {
        // 한번 클릭하면 다신 클릭 못하게 함.
        // 다시 클릭하려면 isClicke =false로 바꿔야 함. 
        if (isClicked) return;
        // 클릭 가능 여부는 dfs가 있는 곳만 가능
        if (isDFS(board.DFSList))
        {
            rend.material.color = clickedColor;
            isClicked = true;
            GetComponent<Renderer>().material.color = clickedColor;
            // 플레이어 이동, startpos,Targetpos 수정 함수 호출
            tileposition = transform.position;
            board.SetTargetpos(tileposition);
        }
        else
        {
            print("그곳엔 이동할 수 없습니다.");
            rend.material.color = originalColor;
        }
        print(transform.position);
    }

    
    
    private void OnMouseUp()
    {
        // isClicked = false;
        GetComponent<Renderer>().material.color = originalColor;  // 기존 색상으로 변경
    }
    
    
    private bool isDFS(List<Node_> DFSList)
    {
        Vector3Int TilePosition = new Vector3Int((int)transform.position.x, 0, (int)transform.position.z);
        for(int i  =0 ; i < DFSList.Count ; i++)
        {
            
            if (Mathf.Abs(TilePosition.x - DFSList[i].x) < 0.1 && Mathf.Abs(TilePosition.z - DFSList[i].z) <0.1 )
            {
                print("true");
                Debug.Log($"(DFSList[{i}]좌표 : {DFSList[i].x}, {DFSList[i].z})");
                Debug.Log(TilePosition);    
                return true;
            }
            else
            {
                print("false");
                Debug.Log(TilePosition);
            }
        }
        return false;
    }
    

    public void OnMoveto(Vector3 end)
    {
        StartCoroutine("MoveTo",end);
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
}
