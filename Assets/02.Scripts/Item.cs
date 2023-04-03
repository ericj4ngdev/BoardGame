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
    }
    private void Update()
    {
        // tr.Rotate(Vector3.down * rotateSpeed * Time.deltaTime);
        // light.color = originalColor;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            transform.SetParent(collision.transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            transform.SetParent(null);
        }
    }
}
