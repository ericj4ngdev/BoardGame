using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    private List<GameObject> collidedObjects;

    void Start()
    {
        collidedObjects = new List<GameObject>();
    }

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(
            gameObject.transform.position).z;
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            Debug.Log("OnTriggerEnter 올려놓음 감지");
            collidedObjects.Add(other.gameObject);
        }
    }

    // 나가면 충돌 리스트 제거
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            Debug.Log("OnTriggerExit");
            collidedObjects.Remove(other.gameObject);
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
            Debug.Log("OnMouseUp");
            // collidedObject.GetComponent<Renderer>().material.color = Color.red;
            collidedObject.GetComponent<PushArea>().OnPush();
            // 드래그 앤 드롭 후 함수 호출하고 싶으면 여기서 하면 될듯. 
            // Debug.Log("Color changed");
        }
        collidedObjects.Clear();
    }
}