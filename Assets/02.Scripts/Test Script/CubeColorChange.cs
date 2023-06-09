using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeColorChange : MonoBehaviour
{
    private Renderer cubeRenderer;
    private bool isColliding = false;

    void Start()
    {
        // 큐브의 Renderer 컴포넌트 가져오기
        cubeRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // 오브젝트가 충돌 중인 경우 색상 변경
        if (isColliding)
        {
            cubeRenderer.material.color = Color.red;
        }
        // 오브젝트가 충돌 중이 아닌 경우 색상 초기화
        else
        {
            cubeRenderer.material.color = Color.white;
        }
    }

    // 충돌 중일 때 호출되는 함수
    void OnTriggerStay(Collider other)
    {
        // 충돌한 오브젝트가 구인 경우
        if (other.gameObject.CompareTag("Wall"))
        {
            // isColliding 변수를 true로 설정하여 색상 변경
            isColliding = true;
        }
    }

    // 충돌이 끝났을 때 호출되는 함수
    void OnTriggerExit(Collider other)
    {
        // 충돌한 오브젝트가 구인 경우
        if (other.gameObject.CompareTag("Wall"))
        {
            // isColliding 변수를 false로 설정하여 색상 초기화
            isColliding = false;
        }
    }
}
