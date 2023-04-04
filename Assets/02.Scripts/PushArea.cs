using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PushArea : MonoBehaviour
{
    private Renderer cubeRenderer;
    private Color originalColor;
    public UnityEvent onSelectPushArea;
    public bool ispushed = false;
    
    
    void Start()
    {
        // 큐브의 Renderer 컴포넌트 가져오기
        cubeRenderer = GetComponent<Renderer>();
        originalColor = GetComponent<Renderer>().material.color;
    }

    public void OnPush()
    {
        onSelectPushArea.Invoke(); // Invoke : 발동하다. 즉, 이벤트 호출
        ispushed = true;
    }
    
    // 충돌 중일 때 호출되는 함수
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
            cubeRenderer.material.color = originalColor * 1.5f;
    }

    // 충돌이 끝났을 때 호출되는 함수
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")) 
            cubeRenderer.material.color = originalColor;
    }
    
}
