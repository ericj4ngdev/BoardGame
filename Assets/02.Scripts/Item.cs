using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Transform tr;
    float rotateSpeed = 50f;
    private Color originalColor;
    private Light light;

    private void Awake()
    {
        tr = GetComponent<Transform>();
        light = GetComponent<Light>();
        originalColor = GetComponent<Renderer>().material.color;
        transform.SetParent(null);
    }

    private void Update()
    {
        light.color = originalColor;
    }
}
