using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public LayerMask groundLayer;
    private Transform tr;
    private Transform localTransform;

    private Rigidbody rb;
    private Collider col;
    public bool isGrounded;
    private GameObject groundObj;

    private void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        // 콜리션 정보를 가져와서 fixedupdate에서 위치를 갱신한다. 
        // tr = groundObj.gameObject.transform;
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


    /*private void OnCollisionStay(Collision collision)
    {
        print("감지중 1");
        isGrounded = true;
        groundObj = collision.gameObject;
        tr = collision.gameObject.transform;
        /*if (collision.gameObject.layer == groundLayer)
        {
            print("감지중 2");
            isGrounded = true;
            groundObj = collision.gameObject;
            // localTransform = tr - collision.gameObject.transform;
            tr = collision.gameObject.transform;
        }#1#
    }*/

}