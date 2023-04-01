using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    // 타일 이동
    [Header("Tiles")]
    public List<GameObject> AllTileList;
    public List<Node> TileBoard = new List<Node>();
    public List<Node> FixedTile = new List<Node>();
    public GameObject rotatingObject;
    public GameObject board;

    [Header("Players")] 
    // public GameObject[] player;
    // Vector3[] playerInitialPosition;
    public GameObject player1_Prefab;
    public GameObject player2_Prefab;
    Vector3 player1_InitialPosition;
    Vector3 player2_InitialPosition;
    
    [Header("UI")]
    public GameObject EndTurnButton;
    public Text turnText;
    public Text StatusText;
    private bool player1Turn;
    private bool endTurnClicked;
    
    [Header("WayPoints")]
    public List<GameObject> waypoint;

    [Header("Item")]
    public List<GameObject> AllItems;
    public List<GameObject> player1_Items = new List<GameObject>();
    public List<GameObject> player2_Items = new List<GameObject>();
    private List<GameObject> AllItems_shuffled;
    public List<Transform> AllItemsPos;
    public List<Transform> AllItemsPos_shuffled;
    private List<Transform> player1_Itemspos = new List<Transform>();
    private List<Transform> player2_Itemspos = new List<Transform>();
    
    
    // private Vector2Int BoardSize = new Vector2Int(5,5);
    private Vector3 eulerRotation;
    private float time;
    private Node node;

    private void Awake()
    {
        eulerRotation = transform.rotation.eulerAngles;
        AllItemsPos = new List<Transform>(FixedTile.Count);
        for (int i = 0; i < FixedTile.Count; i++)
        {
            AllItemsPos.Add(FixedTile[i].transform);
        }
        SpawnTiles();
        
    }
    private void Start()
    {
        // GameObject player1 = Instantiate(player1_Prefab, player1_InitialPosition, Quaternion.identity);
        // GameObject player2 = Instantiate(player2_Prefab, player2_InitialPosition, Quaternion.identity);
        // player의 초기 위치 저장, 나중에 돌아올때 사용
        // player1_InitialPosition = player1.transform.position;
        // player2_InitialPosition = player2.transform.position;
        
        player1Turn = true;
        EndTurnButton.SetActive(false);
        SpawnItem();

        StartCoroutine(GameLoop());
    }

    private List<T> Shuffle<T>(List<T> list)
    {
        int random1,  random2;
        T temp;

        for (int i = 0; i < list.Count; ++i)
        {
            random1 = Random.Range(0, list.Count);
            random2 = Random.Range(0, list.Count);

            temp = list[random1];
            list[random1] = list[random2];
            list[random2] = temp;
        }

        return list;
    }
    private void SpawnItem()
    {
        int index1, index2;
        Shuffle(AllItems);
        Shuffle(AllItemsPos);
        Queue<GameObject> itemqueue = new Queue<GameObject>(AllItems);
        Queue<Transform> itemPosqueue = new Queue<Transform>(AllItemsPos);
        
        for (int i = 0; i < 4; i++)
        {
            if (itemqueue.Count > 0)
            {
                GameObject item = itemqueue.Dequeue();
                Transform pos = itemPosqueue.Dequeue();
                player1_Items.Add(item);
                player1_Itemspos.Add(pos);
                Instantiate(item, pos);
            }
            else
            {
                break;
            }
        }
    
        // 플레이어 2에게 아직 할당되지 않은 아이템 중 플레이어 1과 겹치지 않는 아이템 할당
        itemqueue = new Queue<GameObject>(AllItems.Except(player1_Items));
        itemPosqueue = new Queue<Transform>(AllItemsPos.Except(player1_Itemspos));
        for (int i = 0; i < 4; i++)
        {
            index2 = Random.Range(0, FixedTile.Count);
            if (itemqueue.Count > 0)
            {
                GameObject item = itemqueue.Dequeue();
                Transform pos = itemPosqueue.Dequeue();
                if (!player1_Items.Contains(item) && !player2_Items.Contains(item) && !player1_Itemspos.Contains(pos) && !player2_Itemspos.Contains(pos))
                {
                    player2_Items.Add(item);
                    player2_Itemspos.Add(pos);
                    Instantiate(item, pos);
                }
                else
                {
                    itemqueue.Enqueue(item);
                    itemPosqueue.Enqueue(pos);
                }
            }
            else
            {
                break;
            }
        }
    }
    
    private IEnumerator GameLoop()
    {
        while (true)
        {
            // 현재 턴 정보 업데이트
            UpdateTurnUI();
            OnMovePlayerFinished();
            if (player1Turn)
            {
                yield return StartCoroutine(PlayerTurn(player1_Prefab));
                isPlayer1Itmem(player1_Prefab);
            }
            else
            {
                yield return StartCoroutine(PlayerTurn(player2_Prefab));
                isPlayer2Itmem(player2_Prefab);
            }
            player1Turn = !player1Turn;
            if (IsGameOver())
            {
                break;
            }
        }
    }

    private bool IsGameOver()
    {
        bool gameOver = false;
    
        // 승리 또는 패배 조건 판단 로직 작성
        if (player1_Items.Count == 0)
        {
            gameOver = true;
            Debug.Log("Player 1 Wins!");
        }
        else if (player2_Items.Count == 0)
        {
            gameOver = true;
            Debug.Log("Player 2 Wins!");
        }

        return gameOver;
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
            rotatingObject.GetComponent<Node>().isSelected = true;
            // 타일을 놓았을 때 타일 영역에 들어가면 타일 이동 종료. 다음 spawningObejct
            if (rotatingObject.GetComponent<Node>().isPushed)
            {
                foreach (var VARIABLE in TileBoard)
                    VARIABLE.GetComponent<Node>().isPushed = true;
                foreach (var VARIABLE in FixedTile)
                    VARIABLE.GetComponent<Node>().isPushed = true;
                rotatingObject.GetComponent<Node>().isPushed = false;
                rotatingObject.GetComponent<Node>().isSelected = false;
                // 여기 있는 건 다음 spawn물건임.
                // Debug.Log(rotatingObject.GetComponent<Node>().isPushed);
                break;
            }
            UpdateCoroutineStatus("DragTile");
            yield return null;
        }
        
    }
    private IEnumerator RestDFS()
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
        OnMovePlayerMove();
        
        while (true)
        {
            UpdateCoroutineStatus("MovePlayer 중");
            for (int i = 0; i < TileBoard.Count; i++)
            {
                // TileBoard[i].GetComponent<Node>().reachableTileColorChange();
                // 모든 타일 중 하나라도 클릭한 적이 있다면 다음 턴
                if (TileBoard[i].GetComponent<Node>().isClicked)
                {
                    board.GetComponent<Board>().FollowFinalNodeList_player(TileBoard[i].gameObject, player);
                    TileBoard[i].GetComponent<Node>().isClicked = false;
                    yield break;
                }
            }
            for (int i = 0; i < FixedTile.Count; i++)
            {
                // FixedTile[i].GetComponent<Node>().reachableTileColorChange();
                // 모든 타일 중 하나라도 클릭한 적이 있다면 다음 턴
                if (FixedTile[i].GetComponent<Node>().isClicked)
                {
                    board.GetComponent<Board>().FollowFinalNodeList_player(FixedTile[i].gameObject, player);
                    FixedTile[i].GetComponent<Node>().isClicked = false;
                    yield break;
                }
            }
            // 마우스로 타일 클릭시 플레이어 말을 해당 타일 위치로 이동시키는 기능 활성화.
            yield return null;
        }
    }

    private void OnMovePlayerMove()
    {
        for (int i = 0; i < TileBoard.Count; i++)
        {
            TileBoard[i].GetComponent<Node>().reachableTileColorChange();
        }

        for (int i = 0; i < FixedTile.Count; i++)
        {
            FixedTile[i].GetComponent<Node>().reachableTileColorChange();
        }
    }
    
    private void OnMovePlayerFinished()
    {
        for (int i = 0; i < TileBoard.Count; i++)
        {
            TileBoard[i].GetComponent<Node>().ResetTileColor();
        }
        for (int i = 0; i < FixedTile.Count; i++)
        {
            FixedTile[i].GetComponent<Node>().ResetTileColor();
        }
    }
    

    // ==========================================
    public void isPlayer1Itmem(GameObject player)
    {
        if (player1_Items.Contains(player.GetComponent<Player>().test))
        {
            // 닿은 물체인 test를 비활성화. 
            Debug.Log($"{player.GetComponent<Player>().test.name}");
            player.GetComponent<Player>().test.SetActive(false);
            player1_Items.Remove(player.GetComponent<Player>().test);
        }
    }
    
    public void isPlayer2Itmem(GameObject player)
    {
        if (player2_Items.Contains(player2_Prefab.GetComponent<Player>().test))
        {
            player2_Prefab.SetActive(false);
            player2_Items.Remove(player2_Prefab.GetComponent<Player>().test);
        }
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
