using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed; // 플레이어 이동 속도
    private IEnumerator<Node_> nodeEnumerator;
    private Vector3 targetPosition;
    private GameObject groundObj;
    private Transform tr;
    
    public float waitTime; // 이동 대기 시간
    public int moveTime = 10;
    public bool isMoving = false;
    public float num;
    public GameObject test;
    public Vector3 playerPosition = Vector3.zero;
    public bool isStopped = false;
    
    private void Awake()
    {
        tr = GetComponent<Transform>();
    }

    private void Update()
    {
        playerPosition = tr.position;
    }

    private void OnTriggerStay(Collider other)
    {
        // 플레이어의 것인지 other.name == player1_Items.Contain
        if (other.tag == "Item_1" || other.tag == "Item_2")
        {
            test = other.gameObject;
            // Debug.Log(test.name);
        }
    }

    public void MoveController()
    {
        if (isMoving) return;
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector3.right, 1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector3.left, 1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Move(Vector3.forward, 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector3.back, 1);
        }
    }
    
    private void Move(Vector3 direction, float distance)
    {
        Vector3 newPosition = transform.position + direction * distance;
        if (!IsWall(newPosition))
        {
            isMoving = true;
            StartCoroutine( MoveTo(newPosition));
        }
    }
    
    bool IsWall(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.4f); // 위치 주위의 콜라이더 검색
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Wall")) // 태그가 "wall"인 콜라이더를 찾았으면
            {
                return true; // 벽이므로 true 반환
            }
        }
        return false; // 벽이 아니므로 false 반환
    }
    
    private IEnumerator MoveTo(Vector3 end)
    {
        float	current  = 0;
        float	percent  = 0;
        float	moveTime = 0.1f;
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
        isMoving = false;
    }

    
    
    public void FollowPath(List<Node_> path)
    {
        moveSpeed = (path.Count) / moveTime;
        nodeEnumerator = path.GetEnumerator();
        if (nodeEnumerator.MoveNext())
        {
            targetPosition = new Vector3(nodeEnumerator.Current.x, transform.position.y, nodeEnumerator.Current.z);
            StartCoroutine("MoveToNextNode", moveSpeed);
        }
    }

    private IEnumerator MoveToNextNode(float moveSpeed)
    {
        isStopped = false;
        // 플레이어와 목표타일 사이 거리가 가까워질 때까지 
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            if (isStopped) yield break;     // 멈추면 빠져나오기
            transform.position = Vector3.Lerp(transform.position, targetPosition, num);
            yield return null;
        }
        yield return new WaitForSeconds(waitTime);
        if (nodeEnumerator.MoveNext())
        {
            targetPosition = new Vector3(nodeEnumerator.Current.x, transform.position.y, nodeEnumerator.Current.z);
            StartCoroutine("MoveToNextNode", moveSpeed);
        }
        else StopMoving();      // 멈추면 isStopped 도 true
    }

    public void StopMoving()
    {
        isStopped = true;
    }

}
