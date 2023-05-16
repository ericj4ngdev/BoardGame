using System;
using System.Collections.Generic;
using UnityEngine;

class Person
{
    public string ta = "";
    public int tb = 0;
}

public class Test : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddList();
        }
    }

    void AddList()
    {
        List<Person> arr = new List<Person>();

        for (int i = 0; i < 5; i++)
        {
            Person person = new Person();
            person.ta = "i";
            person.tb = i;
            arr.Add(person);
            Debug.Log(arr[i].ta);
            Debug.Log(arr[i].tb);
        }

        Debug.Log("디버그 찍기");
        
        for (int i = 0; i < arr.Count; i++)
        {
            Debug.Log($"{i}번째 ta : {arr[i].ta} + {i}번째 tb : {arr[i].tb}");
        }
        
        
    }

    void PrintList(List<Person> arr)
    {
        
    }
}

