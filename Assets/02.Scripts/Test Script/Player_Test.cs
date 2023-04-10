using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Player_Test : MonoBehaviourPunCallbacks
{
    private Transform tr;
    public PhotonView PV;
    public bool isMoving = false;
    
    private void Awake()
    {
        tr = GetComponent<Transform>();
    }

    private void Update()
    {
        // Control 
        if (PV.IsMine) MoveController();
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
    
}
