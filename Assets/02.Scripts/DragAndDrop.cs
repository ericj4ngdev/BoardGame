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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            collidedObjects.Remove(other.gameObject);
        }
    }

    private void OnMouseUp()
    {
        foreach (GameObject collidedObject in collidedObjects)
        {
            // collidedObject.GetComponent<Renderer>().material.color = Color.red;
            // 드래그 앤 드롭 후 함수 호출하고 싶으면 여기서 하면 될듯. 
            Debug.Log("Color changed");
        }
    }
}