using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public LayerMask groundLayer;
    private Transform localTransform;

    private Transform tr;
    private Rigidbody rb;
    private Collider col;
    public bool isGrounded;
    

    private void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

}