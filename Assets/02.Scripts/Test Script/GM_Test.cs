using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GM_Test : MonoBehaviour
{
    public bool isRotating = false;
    private Vector3 eulerRotation;
    public GameObject rotatingObject;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            RotateLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            RotateRight();
        }
    }

    public void RotateLeft()
    {
        if (isRotating) return;
        isRotating = true;
        eulerRotation.y -= 90f;
        Vector3 end = eulerRotation;
        StartCoroutine("RotateTo",end);
    }
    public void RotateRight()
    {
        if (isRotating) return;
        isRotating = true;
        eulerRotation.y += 90f;
        Vector3 end = eulerRotation;
        StartCoroutine("RotateTo",end);
    }
    
    private IEnumerator RotateTo(Vector3 end)
    {
        float	current  = 0;
        float	percent  = 0;
        float	moveTime = 0.1f;
        
        Quaternion startRotation = rotatingObject.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(end);

        while ( percent < 1 )
        {
            current += Time.deltaTime;
            percent = current / moveTime;
            // 회전하는 코드
            rotatingObject.transform.rotation = Quaternion.Lerp(startRotation, endRotation, percent);

            yield return null;
        }
        isRotating = false;
    }
}
