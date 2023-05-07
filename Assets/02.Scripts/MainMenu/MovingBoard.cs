using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBoard : MonoBehaviour
{
    private Transform tr;
    private int Speed = 3;
    // private int count = 1;
    // public GameObject Board;
    
    // Start is called before awthe first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        tr.position += Time.deltaTime * Speed * new Vector3(0, 0, -1);
    }

    private void OnTriggerEnter(Collider other)
    {
        // count++;
        tr.Translate(new Vector3(0, 0, 63*3));
    }
    
}
