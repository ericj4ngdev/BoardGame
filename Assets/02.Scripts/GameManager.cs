using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class GameManager : MonoBehaviour
{
    // 타일 이동
    public GameObject rotatingObject;
    public GameObject board;
    public Transform SpawnSpot;
    private Vector3 eulerRotation;
    private Vector2Int BoardSize = new Vector2Int(5,5);
    private List<Node> TileBoard = new List<Node>();
    
    // private List<List<Node>> TileBoard = new List<List<Node>>();        // 소환한 타일 정보
    
    public List<GameObject> waypoint;
    public List<GameObject> AllTileList;

    
    private void Awake()
    {
        eulerRotation = transform.rotation.eulerAngles;
        SpawnTiles();
    }
    
    private void SpawnTiles()
    {
        GameObject tilePrefab;
        
        for (int i = 0; i < waypoint.Count; i++)
        {
            tilePrefab = AllTileList[Random.Range(0, AllTileList.Count)];
            GameObject clone = Instantiate(tilePrefab, waypoint[i].transform.position, waypoint[i].transform.rotation,board.transform);
            Node node = clone.GetComponent<Node>();
            TileBoard.Add(node);
        }
        tilePrefab = AllTileList[Random.Range(0, AllTileList.Count)];
        rotatingObject = Instantiate(tilePrefab, SpawnSpot);
    }

    public void RotateLeft()
    {
        eulerRotation.y -= 90f;
        Vector3 end = eulerRotation;
        StartCoroutine("RotateTo",end);
    }
    public void RotateRight()
    {
        eulerRotation.y += 90f;
        Vector3 end = eulerRotation;
        StartCoroutine("RotateTo",end);
    }
    
    private IEnumerator RotateTo(Vector3 end)
    {
        float	current  = 0;
        float	percent  = 0;
        float	moveTime = 1.0f;
        
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
    }
    
    
    
}
