using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float moveSpeed; // 플레이어 이동 속도
    public float waitTime; // 이동 대기 시간
    public int moveTime = 1;
    public bool canMove;
    
    public float num;
    private IEnumerator<Node_> nodeEnumerator;
    private Vector3 targetPosition;
    private GameObject groundObj;
    public GameObject test;
    private Transform tr;

    public Vector3 playerPosition = Vector3.zero;
    
    private void Awake()
    {
        tr = GetComponent<Transform>();
    }

    private void Update()
    {
        playerPosition = tr.position;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            transform.SetParent(collision.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            transform.SetParent(null);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 플레이어의 것인지 other.name == player1_Items.Contain
        if (other.tag == "Item")
        {
            test = other.gameObject;
        }
    }

    public void FollowPath(List<Node_> path)
    {
        moveSpeed = (path.Count) * 10 / moveTime;
        nodeEnumerator = path.GetEnumerator();
        if (nodeEnumerator.MoveNext())
        {
            targetPosition = new Vector3(nodeEnumerator.Current.x, transform.position.y, nodeEnumerator.Current.z);
            StartCoroutine("MoveToNextNode", moveSpeed);
        }
    }

    private IEnumerator MoveToNextNode(float moveSpeed)
    {
        // 차례차례 MoveToWards 수행
        // 여기서 moveSpeed 를 올리면 빨리 갈 듯 
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, num);
            yield return null;
        }
        // 여기가 시간 조절하는 곳
        yield return new WaitForSeconds(waitTime);

        if (nodeEnumerator.MoveNext())
        {
            targetPosition = new Vector3(nodeEnumerator.Current.x, transform.position.y, nodeEnumerator.Current.z);
            StartCoroutine("MoveToNextNode",moveSpeed);
        }
    }
}
