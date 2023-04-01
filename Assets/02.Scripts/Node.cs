using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Node : MonoBehaviour
{
    private Renderer rend;
    private Color originalColor;
    public Color reachableTileColor;
    public Color ClickableTileColor = Color.blue;
    private Color clickedColor;
    private Transform tr;
    private Board board;
    public bool isPushed = false;
    public bool isClicked = false;
    private Vector3 tileposition;
    float timeToReachTarget = 1f;
    private GameManager gameManager;
    
    [Header("Drag and Drop")]
    private Vector3 mOffset;
    private float mZCoord;
    private List<GameObject> collidedObjects;
    public bool isSelected = false;             // 
    
    private void Awake()
    {
        rend = GetComponent<Renderer>();
        tr = GetComponent<Transform>();
        originalColor = transform.GetChild(0).GetComponent<Renderer>().material.color;
        // originalColor = GetComponent<Renderer>().material.color;
        clickedColor = originalColor * 0.8f;
        board = GetComponentInParent<Board>();
        // print(isDFS(board.DFSList));
        gameManager = FindObjectOfType<GameManager>();
    }
    
    void Start()
    {
        collidedObjects = new List<GameObject>();
    }

    public void reachableTileColorChange()
    {
        if(isDFS(board.DFSList))
            transform.GetChild(0).GetComponent<Renderer>().material.color = reachableTileColor;
    }
    
    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseEnter()
    {
        if (!isPushed) return;
        // 모든 노드의 isPushed = true로 만든다. 
        if (isDFS(board.DFSList))
            transform.GetChild(0).GetComponent<Renderer>().material.color = ClickableTileColor;
    }
    
    private void OnMouseExit()
    {
        transform.GetChild(0).GetComponent<Renderer>().material.color = originalColor;
    }
    
    private void OnMouseDrag()
    {
        if (!isSelected) return;
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }

    private void OnMouseDown()
    {
        // 클릭
        // if (!isSelected) return;
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        // 타일을 밀었다면 클릭가능
        if (isPushed)
        {
            // dfs 구간이면 클릭 가능 및 이동
            if (isDFS(board.DFSList))
            {
                isClicked = true;
                transform.GetChild(0).GetComponent<Renderer>().material.color = clickedColor;
                // 플레이어 이동
                // board.FollowFinalNodeList(gameObject);
            }
            else
            { 
                print("그곳엔 이동할 수 없습니다.");
                transform.GetChild(0).GetComponent<Renderer>().material.color = originalColor;
            }
        }
    }

    private void OnMouseUp()
    {
        if (collidedObjects.Count == 0)
        {
            print(" 제자리로 ");
            return;
        }
        
        foreach (GameObject collidedObject in collidedObjects)
        {
            // Debug.Log("플레이어 말 움직이기로 넘어가기");
            collidedObject.GetComponent<PushArea>().OnPush();
        }
        collidedObjects.Clear();
        
        transform.GetChild(0).GetComponent<Renderer>().material.color = originalColor;  // 기존 색상으로 변경
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
            collidedObjects.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
            collidedObjects.Remove(other.gameObject);
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
    public void OnMoveto_(List<Transform> waypointList)
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
    private IEnumerator MoveToNextTarget(List<Transform> waypointList)
    {
        int currentIndex = 0;
        
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
        yield return null;
    }
}