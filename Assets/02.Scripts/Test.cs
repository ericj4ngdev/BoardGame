using System;
using UnityEngine;
using System.IO;

public class Test : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 newPosition;
    public Vector3 pos;

    private void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPosition();
    }
    

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void Update()
    {
        pos = GetMouseWorldPosition();
        if (isDragging)
        {
            newPosition = GetMouseWorldPosition() + offset;
            newPosition.y = transform.position.y; // 유지하고자 하는 y값
            transform.position = newPosition;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y - transform.position.y; // 카메라와 거리 계산
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
