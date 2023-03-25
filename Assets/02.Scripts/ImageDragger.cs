using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDragger : MonoBehaviour
{
    private Vector3 eulerRotation;
    private Quaternion newRotation;
    
    private void Start()
    {
        eulerRotation = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            eulerRotation.z -= 90f;
            transform.rotation = Quaternion.Euler(eulerRotation);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            eulerRotation.z += 90f;
            transform.rotation = Quaternion.Euler(eulerRotation);
        }
    }
}
