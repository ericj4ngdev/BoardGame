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
        player.GetComponent<Player>().canMove = false;
        Debug.Log($"{player} 차례");
        
        // 타일 옮기기
        bool tileMoved = false;
        while (!tileMoved)
        {
            // 타일 옮기기 
            yield return StartCoroutine(DragTile(player));
            // 타일을 옮겼는지 확인하는 함수.
            // 함수통해서 GameManager가 변수 저장
            tileMoved = true;
        }
        Debug.Log($"{player} 타일 움직이기 끝");
        player.GetComponent<Player>().canMove = true;
        Debug.Log($"{player} canMove = true");
        yield return StartCoroutine(MovePlayer(player));

        // 다음 턴을 위해 타일을 초기 위치로 돌려놓음
        // ResetTilePosition();

        // 타일 옮기기가 끝나면 플레이어의 말을 이동할 수 있음

        // 플레이어 턴이 끝날 때까지 기다림
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

    }

    private IEnumerator DragTile(GameObject player)
    {
        Debug.Log($"{player} 타일 움직이기");
        // 마우스로 타일 드래그 드롭
        while (true)
        {
            // 타일을 놓았을 때 타일 영역에 들어가면 타일 이동 종료
            if (pushArea.OnPush())
            {    
                // 타일 비활성화, 타일 프리펨 가져와서 Setactive() = false;  
                break;
            }
            yield return null;
        }
    }

    private IEnumerator MovePlayer(GameObject player)
    {
        Debug.Log($"{player} 말 움직이기");
        // 플레이어 말 이동
        while (true)
        {
            // 마우스로 타일 클릭시 플레이어 말을 해당 타일 위치로 이동시키는 기능 활성화.
            node.isPushed = true;
            
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null && hit.collider.CompareTag("Tile"))
                {
                    // player.GetComponent<Player_Test>().MoveToTile(hit.collider);
                    break;
                }
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
