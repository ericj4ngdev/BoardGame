using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushArea : MonoBehaviour
{
    private Renderer cubeRenderer;
    private bool isColliding = false;
    private Color originalColor;
    public UnityEvent onSelectPushArea;
    
    
    void Start()
    {
        // 큐브의 Renderer 컴포넌트 가져오기
        cubeRenderer = GetComponent<Renderer>();
        originalColor = GetComponent<Renderer>().material.color;
    }

    void Update()
    {
        // 오브젝트가 충돌 중인 경우 색상 변경
        if (isColliding)
        {
            cubeRenderer.material.color = originalColor * 1.5f;
        }
        // 오브젝트가 충돌 중이 아닌 경우 색상 초기화
        else
        {
            cubeRenderer.material.color = originalColor;
        }
    }

    public void OnPush()
    {
        print(" OnPush");
        onSelectPushArea.Invoke(); // Invoke : 발동하다. 즉, 이벤트 호출
    }
    
    // 충돌 중일 때 호출되는 함수
    void OnTriggerStay(Collider other)
    {
        // 충돌한 오브젝트가 구인 경우
        if (other.gameObject.CompareTag("Ground"))
        {
            // isColliding 변수를 true로 설정하여 색상 변경
            isColliding = true;
        }
    }

    // 충돌이 끝났을 때 호출되는 함수
    void OnTriggerExit(Collider other)
    {
        // 충돌한 오브젝트가 구인 경우
        if (other.gameObject.CompareTag("Ground"))
        {
            // isColliding 변수를 false로 설정하여 색상 초기화
            isColliding = false;
        }
    }
    
    
}
