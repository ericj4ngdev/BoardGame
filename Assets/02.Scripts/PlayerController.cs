using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 10f;
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            print("충돌");
            rb.velocity = Vector3.zero;
            
        }
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
            
        Vector3 position = transform.position;

        position.x += horizontalInput * moveSpeed * Time.deltaTime;
        position.z += verticalInput * moveSpeed * Time.deltaTime;
        
        transform.position = position;
    }
}