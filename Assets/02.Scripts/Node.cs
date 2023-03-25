using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    public Color buildAvailableColor;
    public Color buildNotAvailableColor;
    private Transform tr;
    
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        tr = GetComponent<Transform>();
        originalColor = rend.material.color;
    }
    private void OnMouseEnter()
    {
        rend.material.color = buildAvailableColor;
    }

    private void OnMouseDown()
    {
        // 모서리 쪽에 있는 타일인가? -> 이동할 방향 정하기, UI띄우기 
        // 함수 호출, 이벤트 
        // TileUI.instance.SetUp(transform.position, this);
        // 플레이어가 노드의 정중앙으로 이동. 그런데 타일의 최단경로를 따라 이동한다. 
        
    }

    public void OnMoveto(Vector3 end)
    {
        StartCoroutine("MoveTo",end);
    }
    
    private void OnMouseExit()
    {
        rend.material.color = originalColor;
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
