using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongList : MonoBehaviour
{
    public List<GameObject> targetList;
    public float timeToReachTarget;

    private int currentIndex = 0;
    private bool isMoving = false;

    private void Start()
    {
        StartCoroutine(MoveToNextTarget());
    }

    private IEnumerator MoveToNextTarget()
    {
        isMoving = true;

        while (currentIndex < targetList.Count)
        {
            Vector3 targetPosition = targetList[currentIndex].transform.position;
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            float speed = distanceToTarget / timeToReachTarget;

            float elapsedTime = 0f;
            while (elapsedTime < timeToReachTarget)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime * speed / distanceToTarget);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            currentIndex++;
        }

        isMoving = false;
        yield return null;
    }
}
