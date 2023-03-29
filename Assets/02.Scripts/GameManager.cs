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

    // =================================================
    
    public GameObject[] tiles;
    public GameObject player1;
    public GameObject player2;

    private PushArea pushArea;
    private Node node;
    
    private bool player1Turn;

    private void Start()
    {
        player1Turn = true;
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            if (player1Turn)
            {
                yield return StartCoroutine(PlayerTurn(player1));
            }
            else
            {
                yield return StartCoroutine(PlayerTurn(player2));
            }
            player1Turn = !player1Turn;
        }
    }

    private IEnumerator PlayerTurn(GameObject player)
    {
        // 타일 옮기기 전에는 플레이어의 말을 이동할 수 없음
        Debug.Log($"{player.name} 차례");
        // 타일 옮기기 
        yield return StartCoroutine(DragTile(player));
        Debug.Log($"{player.name} DragTile 끝");
        
        // 말 옮기기 
        yield return StartCoroutine(MovePlayer(player));
        Debug.Log($"{player.name} MovePlayer 끝");

        // 플레이어 턴이 끝날 때까지 기다림
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

    }

    private IEnumerator DragTile(GameObject player)
    {
        Debug.Log($"{player.name} DragTile 시작");
        // 마우스로 타일 드래그 드롭
        while (true)
        {
            // 타일을 놓았을 때 타일 영역에 들어가면 타일 이동 종료. 다음 spawningObejct
            if (rotatingObject.GetComponent<Node>().IsPushed()) break;
            yield return null;
        }
        
    }

    private IEnumerator MovePlayer(GameObject player)
    {
        Debug.Log($"{player.name} MovePlayer 시작");
        // 플레이어 말 이동
        while (true)
        {
            // 마우스로 타일 클릭시 플레이어 말을 해당 타일 위치로 이동시키는 기능 활성화.
            if (node.IsClicked())
            {
                break;
            }
            yield return null;
        }
    }
    
    // ==========================================
    private void Awake()
    {
        eulerRotation = transform.rotation.eulerAngles;
        SpawnTiles();
    }
    
    private void SpawnTiles()
    {
        GameObject tilePrefab;
        Quaternion randomRotation;
        for (int i = 0; i < waypoint.Count; i++)
        {
            tilePrefab = AllTileList[Random.Range(0, AllTileList.Count)];
            randomRotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);

            GameObject clone = Instantiate(tilePrefab, waypoint[i].transform.position, randomRotation,board.transform);
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
