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
        collidedObjects.Clear();
    }

    private void OnMouseUp()
    {
        foreach (GameObject collidedObject in collidedObjects)
        {
            if (collidedObject.CompareTag("Cube"))
            {
                collidedObject.GetComponent<Renderer>().material.color = Color.red;
                print("aaaaaaaaaaaa");
            }
        }
    }

}