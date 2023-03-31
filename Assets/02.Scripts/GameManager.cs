using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    // 타일 이동
    public GameObject rotatingObject;
    public GameObject board;
    // public Transform SpawnSpot;
    // new Vector3(26,0.05,-7);
    private Vector3 eulerRotation;
    private Vector2Int BoardSize = new Vector2Int(5,5);
    public List<Node> TileBoard = new List<Node>();
    public List<Node> MapTile = new List<Node>();
    private float time;
    // private List<List<Node>> TileBoard = new List<List<Node>>();        // 소환한 타일 정보
    
    public List<GameObject> waypoint;
    public List<GameObject> AllTileList;

    // =================================================
    
    public GameObject[] tiles;
    public GameObject player1;
    public GameObject player2;

    private Node node;
    private bool player1Turn;
    private bool endTurnClicked;
    public GameObject EndTurnButton;
    public Text turnText;
    public Text StatusText;
    
    private void Awake()
    {
        eulerRotation = transform.rotation.eulerAngles;
        
        SpawnTiles();
    }
    private void Start()
    {
        player1Turn = true;
        EndTurnButton.SetActive(false);
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            // 현재 턴 정보 업데이트
            UpdateTurnUI();
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
        // 스폰 오브젝트는 드래그 가능
        // 타일 옮기기 전에는 플레이어의 말을 이동할 수 없음
        rotatingObject.GetComponent<Node>().isSelected = true;
        
        // 타일 옮기기 
        yield return StartCoroutine(DragTile(player));
        yield return StartCoroutine(RestDFS());
        
        // 플레이어 기준으로 DFS 하기, 그런데 움직인 타일의 위치가 반영이 안되어 있다. 
        // 모든 node의 isPushed = true로 해야 클릭 가능.
        board.GetComponent<Board>().GetWallInfo();
        board.GetComponent<Board>().DFS_player(player);
        
        // 말 옮기기 
        yield return StartCoroutine(MovePlayer(player));
        foreach (var VARIABLE in TileBoard)
            VARIABLE.GetComponent<Node>().isPushed = false;
        Debug.Log($"{player.name} MovePlayer 끝");
        
        // 여기서 아무것도 못하게 함. 돌리기는 물론 spawnObject 클릭하지 못하게 하기
        rotatingObject.GetComponent<Node>().isSelected = false; // 드래그 불가
        
        UpdateCoroutineStatus("턴 넘기기");
        // EndTurnButton을 활성화해서 플레이어가 클릭하도록 함
        EndTurnButton.SetActive(true);
        
        // 플레이어가 EndTurnButton을 클릭할 때까지 대기
        yield return new WaitUntil(() => endTurnClicked);

        // EndTurnButton을 다시 비활성화해서 다음 플레이어의 차례를 기다리도록 함
        EndTurnButton.SetActive(false);

        // endTurnClicked 변수를 초기화
        endTurnClicked = false;
    }
    
    private void UpdateCoroutineStatus(string coroutineName)
    {
        // UI 업데이트
        StatusText.text = coroutineName;
    }
    
    
    public void OnEndTurnClicked()
    {
        // EndTurnButton을 클릭하면 endTurnClicked 변수를 true로 설정
        endTurnClicked = true;
    }

    private void UpdateTurnUI()
    {
        // "Player 1's Turn" 또는 "Player 2's Turn" 등의 텍스트를 UI 요소에 업데이트
        turnText.text = player1Turn ? "Player 1's Turn" : "Player 2's Turn";
    }

    private IEnumerator DragTile(GameObject player)
    {
        // Debug.Log($"{player.name} DragTile 시작");
        // 마우스로 타일 드래그 드롭
        while (true)
        {
            
            // 타일을 놓았을 때 타일 영역에 들어가면 타일 이동 종료. 다음 spawningObejct
            if (rotatingObject.GetComponent<Node>().isPushed)
            {
                foreach (var VARIABLE in TileBoard)
                    VARIABLE.GetComponent<Node>().isPushed = true;
                foreach (var VARIABLE in MapTile)
                    VARIABLE.GetComponent<Node>().isPushed = true;
                rotatingObject.GetComponent<Node>().isPushed = false;
                // 여기 있는 건 다음 spawn물건임.
                // Debug.Log(rotatingObject.GetComponent<Node>().isPushed);
                break;
            }
            UpdateCoroutineStatus("DragTile");
            yield return null;
        }
        
    }
    IEnumerator RestDFS()
    {
        time = 0;
        while (true)
        {
            
            time += Time.deltaTime;
            if(time>=5f)
                break; 
            UpdateCoroutineStatus("RestDFS 중");
            yield return null;
        }
    }
    private IEnumerator MovePlayer(GameObject player)
    {
        // Debug.Log($"{player.name} MovePlayer 시작");
        // 플레이어 말 이동
        while (true)
        {
            UpdateCoroutineStatus("MovePlayer 중");
            for (int i = 0; i < TileBoard.Count; i++)
            {
                TileBoard[i].GetComponent<Node>().reachableTileColorChange();
                // 모든 타일 중 하나라도 클릭한 적이 있다면 다음 턴
                if (TileBoard[i].GetComponent<Node>().isClicked)
                {
                    board.GetComponent<Board>().FollowFinalNodeList_player(TileBoard[i].gameObject, player);
                    TileBoard[i].GetComponent<Node>().isClicked = false;
                    yield break;
                }
            }
            for (int i = 0; i < MapTile.Count; i++)
            {
                MapTile[i].GetComponent<Node>().reachableTileColorChange();
                // 모든 타일 중 하나라도 클릭한 적이 있다면 다음 턴
                if (MapTile[i].GetComponent<Node>().isClicked)
                {
                    board.GetComponent<Board>().FollowFinalNodeList_player(MapTile[i].gameObject, player);
                    MapTile[i].GetComponent<Node>().isClicked = false;
                    yield break;
                }
            }
            // 마우스로 타일 클릭시 플레이어 말을 해당 타일 위치로 이동시키는 기능 활성화.
            yield return null;
        }
    }

    // ==========================================
   
    
    private void SpawnTiles()
    {
        GameObject tilePrefab;
        Quaternion randomRotation;
        for (int i = 0; i < waypoint.Count; i++)
        {
            tilePrefab = AllTileList[Random.Range(0, AllTileList.Count)];
            randomRotation = Quaternion.Euler(0, Random.Range(0, 4) * 90, 0);
            
            GameObject clone = Instantiate(tilePrefab, waypoint[i].transform.position, randomRotation,board.transform);
            if (i == waypoint.Count - 1)
            {
                rotatingObject = clone;
                Node node_ = rotatingObject.GetComponent<Node>();
                TileBoard.Add(node_);
                break;
                // return;
            }
            Node node = clone.GetComponent<Node>();
            TileBoard.Add(node);
        }
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
