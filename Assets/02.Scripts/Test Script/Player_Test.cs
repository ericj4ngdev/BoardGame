using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Test : MonoBehaviour
{
    public bool canMove = false;

    public void func1()
    {
        Debug.Log("func1");
        canMove = true;
    }

    public bool isCanMove()
    {
        if (canMove) return true;
        else return false;
    }
    
    private void OnMouseEnter()
    {
        func1();
    }
}
