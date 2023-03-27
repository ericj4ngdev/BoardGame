using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDragger : MonoBehaviour
{
    private Vector3 offset;
    private float zCoord;

    void OnMouseDown()
    {
        // 마우스 클릭 시 오브젝트와 마우스 포인터의 거리를 계산하여 offset 변수에 저장
        zCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        offset = gameObject.transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        // 마우스 드래그 시 오브젝트를 새로운 위치로 이동
        Vector3 newPosition = GetMouseWorldPos() + offset;
        transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
    }

    private Vector3 GetMouseWorldPos()
    {
        // 마우스 포인터 위치를 3D 좌표로 변환하여 반환
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

}
