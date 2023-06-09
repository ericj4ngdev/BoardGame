using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GameManager : MonoBehaviour
{
    // 타일 이동
    [Header("Tiles")]
    public List<GameObject> AllTileList;
    public List<Node> TileBoard = new List<Node>();
    public List<Node> FixedTile = new List<Node>();
    public GameObject rotatingObject;
    public GameObject board;
    public BoardInfo boardInfo;
    public List<int> tileCounts = new List<int>();
    
    [Header("Players")] 
    public GameObject player1_Prefab;
    public GameObject player2_Prefab;
    Vector3 player1_InitialPosition;
    Vector3 player2_InitialPosition;
    
    [Header("UI")]
    public GameObject EndTurnButton;
    public Image Winner;
    public Text turnText;
    public Text StatusText;
    public Text WinText;
    public GameObject pausePanel;
    private bool endTurnClicked;
    private bool isPlayer1Turn;
    private bool isPaused = false;
    
    [Header("WayPoints")]
    public List<GameObject> waypoint;

    [Header("Item")]
    public List<GameObject> AllItems;
    public List<GameObject> player1_Items = new List<GameObject>();
    public List<GameObject> player1_ItemsforExcept = new List<GameObject>();
    public List<GameObject> player2_Items = new List<GameObject>();
    private List<GameObject> AllItems_shuffled;
    public List<Transform> AllItemsPos;
    private List<Transform> player1_Itemspos = new List<Transform>();
    private List<Transform> player2_Itemspos = new List<Transform>();

    public List<GameObject> BlueItem = new List<GameObject>();
    public List<GameObject> RedItem = new List<GameObject>();
    
    private Vector3 eulerRotation;
    private float time;
    private Node node;
    private Coroutine gameLoopCoroutine;
    public bool isRotating = false;

    private void Awake()
    {
        // 회전각
        eulerRotation = transform.rotation.eulerAngles;
        AllItemsPos = new List<Transform>(FixedTile.Count);
        for (int i = 0; i < waypoint.Count; i++)
        {
            AllItemsPos.Add(waypoint[i].transform);
        }
        SpawnTiles();
    }
    private void Start()
    {
        isPlayer1Turn = true;
        EndTurnButton.SetActive(false);
        SpawnItem();

        StartCoroutine(GameLoop());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                Time.timeScale = 0f; // 게임 일시정지
                pausePanel.SetActive(true); // 일시정지 패널 활성화
            }
            else
            {
                Time.timeScale = 1f; // 게임 재개
                pausePanel.SetActive(false); // 일시정지 패널 비활성화
            }
        }
    }

    public void OnResumeButtonClicked()
    {
        Time.timeScale = 1f; // 게임 재개
        pausePanel.SetActive(false); // 일시정지 패널 비활성화
        isPaused = !isPaused;
    }
    
    private IEnumerator GameLoop()
    {
        while (true)
        {
            // 현재 턴 정보 업데이트
            UpdateTurnUI();
            OnMovePlayerFinished();
            // 플레이어 턴인가?
            if (isPlayer1Turn) yield return StartCoroutine(PlayerTurn(player1_Prefab));
            else yield return StartCoroutine(AITurn(player2_Prefab));
            
            // 턴 전환
            isPlayer1Turn = !isPlayer1Turn;
            if (IsGameOver()) break;
        }
        // 코루틴 종료 후 gameLoopCoroutine을 null로 초기화
        gameLoopCoroutine = null;
    }

    private bool IsGameOver()
    {
        bool gameOver = false;
        // 승리 또는 패배 조건 판단 로직 작성
        if (player1_Items.Count == 0)
        {
            gameOver = true;
            Debug.Log("Player 1 Wins!");
            WinText.text = "Player 1 Wins!";
            StartCoroutine(FadeInImage(Winner, WinText,1f));
        }
        else if (player2_Items.Count == 0)
        {
            gameOver = true;
            Debug.Log("Player 2 Wins!");
            WinText.text = "Player 2 Wins!";
            StartCoroutine(FadeInImage(Winner, WinText,1f));
        }
        return gameOver;
    }
    
    public void RestartGame()
    {
        if (gameLoopCoroutine != null)
        {
            // 현재 실행 중인 GameLoop 코루틴 중지
            StopCoroutine(gameLoopCoroutine);
        }
        SceneManager.LoadScene("ProtoType3D");
    }

    IEnumerator FadeInImage(Image image, Text text, float fadeTime)
    {
        Color originalImageColor = image.color;
        Color originalTextColor = text.color;
        float time = 0f;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float imagealpha = Mathf.Lerp(0f, 0.4f, time / fadeTime);
            float textalpha = Mathf.Lerp(0f, 1, time / fadeTime);
            image.color = new Color(originalImageColor.r, originalImageColor.g, originalImageColor.b, imagealpha);
            text.color = new Color(originalTextColor.r, originalTextColor.g, originalTextColor.b, textalpha);
            yield return null;
        }
    }

    private IEnumerator AITurn(GameObject player)
    {
        // 스캔
        this.GetComponent<BinaryInfo>().ScanBoard();
        
        // AI턴에서는 아무것도 못하게 함.
        rotatingObject.GetComponent<Node>().isSelected = false; // 드래그 불가
        
        // 타일 옮기기 
        yield return StartCoroutine(AIDragTile(player));
        yield return StartCoroutine(RestDFS());
        
        // 플레이어 기준으로 DFS 하기, 그런데 움직인 타일의 위치가 반영이 안되어 있다. 
        // 모든 node의 isPushed = true로 해야 클릭 가능.
        board.GetComponent<Board>().GetWallInfo();
        board.GetComponent<Board>().DFS_player(player);
        
        // 말 옮기기 
        yield return StartCoroutine(AIMovePlayer(player));
        foreach (var VARIABLE in TileBoard)
            VARIABLE.GetComponent<Node>().isPushed = false;
        Debug.Log($"{player.name} MovePlayer 끝");

        UpdateCoroutineStatus("턴 넘기기");
        // EndTurnButton을 활성화해서 플레이어가 클릭하도록 함
        EndTurnButton.SetActive(true);
        
        endTurnClicked = true;
        // 플레이어가 EndTurnButton을 클릭할 때까지 대기
        yield return new WaitUntil(() => endTurnClicked);

        // EndTurnButton을 다시 비활성화해서 다음 플레이어의 차례를 기다리도록 함
        EndTurnButton.SetActive(false);
        
        // endTurnClicked 변수를 초기화
        endTurnClicked = false;
    }
    
    private IEnumerator PlayerTurn(GameObject player)
    {
        // 스폰 오브젝트는 드래그 가능
        // 타일 옮기기 전에는 플레이어의 말을 이동할 수 없음
        rotatingObject.GetComponent<Node>().isSelected = true;
        
        // 타일 옮기기 
        yield return StartCoroutine(DragTile(player));
        // yield return StartCoroutine(RestDFS());
        
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
        turnText.text = isPlayer1Turn ? "Player 1 Turn" : "AI Turn";
    }

    private IEnumerator DragTile(GameObject player)
    {
        // 밀어넣을 타일 미리 저장.
        GameObject previousObject = rotatingObject;
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
                int childCount = rotatingObject.transform.childCount;
                List<GameObject> players = new List<GameObject>();
                // 만약 나온 타일에 자식 오브젝트중 player가 있다면 
                for (int i = 0; i < childCount; i++)
                {
                    // Debug.Log($"{i}번쨰 점검 : {rotatingObject.transform.GetChild(i).name}");
                    if (rotatingObject.transform.GetChild(i).CompareTag("player"))
                    {
                        // 여러개 일수 있다. 이미 하나 빼버리면 인덱스 오류나버린다.
                        players.Add(rotatingObject.transform.GetChild(i).gameObject);
                    }
                }

                foreach (var VARIABLE in players)
                {
                    VARIABLE.transform.SetParent(previousObject.transform);
                    VARIABLE.transform.position = previousObject.transform.position + new Vector3(0,1,0);
                }

                // 여기 있는 건 다음 spawn물건임.
                // Debug.Log(rotatingObject.GetComponent<Node>().isPushed);
                break;
            }
            UpdateCoroutineStatus("DragTile");
            yield return null;
        }
        
    }
    
    private IEnumerator AIDragTile(GameObject player)
    {
        UpdateCoroutineStatus("DragTile");
        // 밀어넣을 타일 미리 저장.
        GameObject previousObject = rotatingObject;
        previousObject.transform.position += new Vector3(0,5,0);
        this.GetComponent<BinaryInfo>().AIPushTile_2();     // AI판단
        string info = GetComponent<BinaryInfo>().location;
        
        int rotate = GetComponent<BinaryInfo>().rotate;
        for (int i = 0; i < rotate; i++)
        {
            yield return StartCoroutine(Value01Co());
            RotateRight();
            value01 = 0;
        }
        yield return StartCoroutine(Value01Co());
        
        boardInfo.DragTile(rotatingObject, info);
        value01 = 0;
        yield return StartCoroutine(Value01Co());
        boardInfo.PushNode(info);           // 행동
        
        value01 = 0;
        // 마우스로 타일 드래그 드롭
        while (true)
        {
            // UpdateCoroutineStatus("DragTile");
            rotatingObject.GetComponent<Node>().isSelected = true;
            // 타일을 놓았을 때 타일 영역에 들어가면 타일 이동 종료. 다음 spawningObejct
            
            // AI는 rotatingObject.GetComponent<Node>().isPushed 를 true로 만들고
            // BoardInfo에 있는 이벤트에 위치, 회전값 string을 인자로 전달
            // 위치를 어떻게 전달하지... 
                 // 출력이 안됨... 
            // 
            
            if (rotatingObject.GetComponent<Node>().isPushed)
            {
                foreach (var VARIABLE in TileBoard)
                    VARIABLE.GetComponent<Node>().isPushed = true;
                foreach (var VARIABLE in FixedTile)
                    VARIABLE.GetComponent<Node>().isPushed = true;
                rotatingObject.GetComponent<Node>().isPushed = false;
                rotatingObject.GetComponent<Node>().isSelected = false;
                int childCount = rotatingObject.transform.childCount;
                List<GameObject> players = new List<GameObject>();
                // 만약 나온 타일에 자식 오브젝트중 player가 있다면 
                for (int i = 0; i < childCount; i++)
                {
                    // Debug.Log($"{i}번쨰 점검 : {rotatingObject.transform.GetChild(i).name}");
                    if (rotatingObject.transform.GetChild(i).CompareTag("player"))
                    {
                        // 여러개 일수 있다. 이미 하나 빼버리면 인덱스 오류나버린다.
                        players.Add(rotatingObject.transform.GetChild(i).gameObject);
                    }
                }

                foreach (var VARIABLE in players)
                {
                    VARIABLE.transform.SetParent(previousObject.transform);
                    VARIABLE.transform.position = previousObject.transform.position + new Vector3(0,1,0);
                }
                // Debug.Log(rotatingObject.GetComponent<Node>().isPushed);
                break;
            }
            
            yield return null;
        }
        
    }
    
    /// <summary>
    /// Gizmo상 변한 미로에 대한 플레이어 DFS갱신하는데 걸리는 시간 딜레이 
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestDFS()
    {
        time = 0;
        while (true)
        {
            time += Time.deltaTime;
            if(time>=2f)
                break; 
            UpdateCoroutineStatus("타일 옮기는 중");
            yield return null;
        }
    }
    private IEnumerator MovePlayer(GameObject player)
    {
        while (true)
        {
            UpdateCoroutineStatus("MovePlayer 중");
            player.GetComponent<PlayerController>().MoveController();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 먹는거 시전
                isPlayerItem(player);
                break;
            }
            player.GetComponent<Collider>().isTrigger = false;
            player.GetComponent<Rigidbody>().isKinematic = false;
            // 마우스로 타일 클릭시 플레이어 말을 해당 타일 위치로 이동시키는 기능 활성화.
            yield return null;
        }
    }
    [SerializeField]
    private GameObject testTile;
    private IEnumerator AIMovePlayer(GameObject player)
    {
        Debug.Log($"{player.name} AIMovePlayer 시작");
        // 플레이어 말 이동
        testTile = this.GetComponent<BinaryInfo>().AIMove();     // AI판단
        // FollowFinalNodeList_player 함수가 Coroutine이므로 yield return을 사용하여 대기합니다.
        board.GetComponent<Board>().FollowFinalNodeList_player(testTile,player);
        // bool 타입 변수를 만들어서 코루틴이 끝나기 전까지는 bool이 false이고 그 뒤론 true가 되도록 하는 거다. 
        
        while (true)
        {
            UpdateCoroutineStatus("MovePlayer 중");
            // 움직이는 리스트를 받는다. 숫자정보. 예를 들어 1이면 x+1, 이런 느낌 
            // 멈추면 먹는거 시전
            // Debug.Log(player.GetComponent<PlayerController>().isStopped);
            if (player.GetComponent<PlayerController>().isStopped)
            {
                Debug.Log("아이템 확인중");
                isPlayerItem(player);
                break;
            }
            
            player.GetComponent<Collider>().isTrigger = false;
            player.GetComponent<Rigidbody>().isKinematic = false;
            
            yield return null;
        }
    }


    [Range(0f, 1f)] [SerializeField] private float value01;
    IEnumerator Value01Co()
    {
        while (true)
        {
            value01 += Time.deltaTime;
            if (value01 >= 1f)
            {
                value01 = 1f;
                break; 
            }
            yield return null;
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
            // 여기 오류 있다고 함
            FixedTile[i].GetComponent<Node>().ResetTileColor();
        }
    }
    

    // ==========================================
    public void isPlayerItem(GameObject player)
    {
        Debug.Log(player);
        if (player == player1_Prefab)
        {
            if (player1_Items.Contains(player.GetComponent<PlayerController>().test))
            {
                // 닿은 물체인 test를 비활성화. 
                Debug.Log($"{player.GetComponent<PlayerController>().test.name} 획득!!");
                player.GetComponent<PlayerController>().test.SetActive(false);
                player1_Items.Remove(player.GetComponent<PlayerController>().test);
                Destroy(player.GetComponent<PlayerController>().test); 
            }
        }
        if (player == player2_Prefab)
        {
            if (player2_Items.Contains(player.GetComponent<PlayerController>().test))
            {
                Debug.Log($"{player.GetComponent<PlayerController>().test.name} 획득!!");
                player.GetComponent<PlayerController>().test.SetActive(false);
                player2_Items.Remove(player.GetComponent<PlayerController>().test);
                Destroy(player.GetComponent<PlayerController>().test); 
            }
        }
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
        // 각 색에 맞게 분류했고 위치만 노중복으로 배치할 것.
        GameObject itemPrefab;
        itemPrefab = BlueItem[Random.Range(0, BlueItem.Count)];
        GameObject clone;
        Shuffle(AllItemsPos);
        Queue<Transform> itemPosqueue = new Queue<Transform>(AllItemsPos);
        
        for (int i = 0; i < 4; i++)
        {
            Transform pos = itemPosqueue.Dequeue();
            clone = Instantiate(itemPrefab, pos);
            player1_Items.Add(clone);
            player1_Itemspos.Add(pos);
        }
        itemPrefab = RedItem[Random.Range(0, RedItem.Count)];
        itemPosqueue = new Queue<Transform>(AllItemsPos.Except(player1_Itemspos));
        for (int i = 0; i < 4; i++)
        {
            if (itemPosqueue.Count > 0)
            {
                Transform pos = itemPosqueue.Dequeue();
                if (!player1_Items.Contains(itemPrefab) && !player2_Items.Contains(itemPrefab) && !player1_Itemspos.Contains(pos) && !player2_Itemspos.Contains(pos))
                {
                    clone = Instantiate(itemPrefab, pos);
                    player2_Items.Add(clone);
                    player2_Itemspos.Add(pos);
                }
                else
                {
                    itemPosqueue.Enqueue(pos);
                }
            }
            else
            {
                break;
            }
        }
    }
    
    private void SpawnTiles()
    {
        GameObject tilePrefab = null; // 변수 초기화
        Quaternion randomRotation;
        for (int i = 0; i < waypoint.Count; i++)
        {
            int index = Random.Range(0, AllTileList.Count);
            // 선택된 GameObject 개수 확인
            while (tileCounts[index] <= 0) {
                index = Random.Range(0, AllTileList.Count);
            }
            tileCounts[index]--; // 선택된 GameObject 개수 -1

            tilePrefab = AllTileList[index];

            randomRotation = Quaternion.Euler(0, Random.Range(0, 3) * 90, 0);
        
            GameObject clone = Instantiate(tilePrefab, waypoint[i].transform.position, randomRotation,board.transform);
            if (i == waypoint.Count - 1)
            {
                rotatingObject = clone;
                Node node_ = rotatingObject.GetComponent<Node>();
                TileBoard.Add(node_);
                break;
            }
            Node node = clone.GetComponent<Node>();
            TileBoard.Add(node);
        }
    }
    
    public void RotateLeft()
    {
        if (isRotating) return;
        isRotating = true;
        eulerRotation.y -= 90f;
        Vector3 axis = Vector3.up; // 회전할 축
        StartCoroutine("RotateTo", Quaternion.AngleAxis(-90f, axis) * rotatingObject.transform.rotation);
    }

    public void RotateRight()
    {
        if (isRotating) return;
        isRotating = true;
        eulerRotation.y += 90f;
        Vector3 axis = Vector3.up; // 회전할 축
        StartCoroutine("RotateTo", Quaternion.AngleAxis(90f, axis) * rotatingObject.transform.rotation);
    }

    private IEnumerator RotateTo(Quaternion endRotation)
    {
        float	current  = 0;
        float	percent  = 0;
        float	moveTime = 0.1f;
    
        Quaternion startRotation = rotatingObject.transform.rotation;

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
