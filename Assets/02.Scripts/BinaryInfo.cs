using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

class BinTile 
{
    public BinTile()
    {
        Shape = new List<List<int>> {
            new List<int>{ 0, 0, 0 },
            new List<int>{ 0, 0, 0 },
            new List<int>{ 0, 0, 0 }
        };
        rotation = 0;
        type = TileType.STRAIGHT;
        x = 0;
        y = 0;
        isvisited = false;
        isPlayer1 = false;
        isPlayer2 = false;
        IsPlayer1Item = false;
        IsPlayer2Item = false;
    }
    public BinTile(BinTile other)
    {
        this.Shape = new List<List<int>>();
        for (int i = 0; i < other.Shape.Count; i++) {
            this.Shape.Add(new List<int>(other.Shape[i]));
        }
        this.rotation = other.rotation;
        this.type = other.type;
        this.x = other.x;
        this.y = other.y;
        this.isvisited = other.isvisited;
        this.isPlayer1 = other.isPlayer1;
        this.isPlayer2 = other.isPlayer2;
        this.IsPlayer1Item = other.IsPlayer1Item;
        this.IsPlayer2Item = other.IsPlayer2Item;
    }
    
    public List<List<int>> Shape;
    public int rotation;
    public TileType type;
    public int x;
    public int y;
    public bool isvisited;
    public bool isPlayer1;
    public bool isPlayer2;
    public bool IsPlayer1Item;
    public bool IsPlayer2Item;
}

class NodeTest
{
    public int x;
    public int y;
    public int num;
    public bool isVisited;
}

class Score
{
    public string location;
    public int rotation;
    public int Player1ReachableItem;
    public int Player2ReachableItem;
    public int Player1_DeltaReachableItem;
    public int Player2_DeltaReachableItem;
    public float Percent_1 = 0;
    public float Percent_2 = 0;
    public float per = 0;
    
    public Score() // 생성자 추가
    {
        location = "";
        rotation = 0;
        Player1ReachableItem = 0;
        Player2ReachableItem = 0;
        Player1_DeltaReachableItem = 0;
        Player2_DeltaReachableItem = 0;
        Percent_1 = 0;
        Percent_2 = 0;
        per = 0;
    }
    
    public Score(Score other)
    {
        location = other.location;
        rotation = other.rotation;
        Player1ReachableItem = other.Player1ReachableItem;
        Player2ReachableItem = other.Player2ReachableItem;
        Player1_DeltaReachableItem = other.Player1_DeltaReachableItem;
        Player2_DeltaReachableItem = other.Player2_DeltaReachableItem;
        Percent_1 = other.Percent_1;
        Percent_2 = other.Percent_2;
        per = other.per;
    }
}



public class BinaryInfo : MonoBehaviour
{
    List<List<NodeTest>> boardList = new List<List<NodeTest>>();
    List<List<NodeTest>> CopiedBoardList = new List<List<NodeTest>>();
    // List<Score> ScoreList = new List<Score>();
    List<string> locations = new List<string> {   "L0","L1","L2",
        "R0","R1","R2",
        "T0","T1","T2",
        "B0","B1","B2"
    };
    List<List<BinTile>> board = new List<List<BinTile>>();
    List<NodeTest> DFSList1 = new List<NodeTest>();
    List<NodeTest> DFSList2 = new List<NodeTest>();
    List<BinTile> NodeTestInfoList = new List<BinTile>();
    
    
    BinTile straight = new BinTile {
        Shape = new List<List<int>> {
            new List<int> {0, 1, 0},
            new List<int> {0, 1, 0},
            new List<int> {0, 1, 0}
        },
        rotation = 0,
        type = TileType.STRAIGHT
    };
    BinTile corner = new BinTile {
        Shape = new List<List<int>> {
            new List<int> {0, 0, 0},
            new List<int> {0, 1, 1},
            new List<int> {0, 1, 0}
        },
        rotation = 0,
        type = TileType.CORNER
    };
    BinTile halfcross = new BinTile {
        Shape = new List<List<int>> {
            new List<int> {0, 1, 0},
            new List<int> {1, 1, 0},
            new List<int> {0, 1, 0}
        },
        rotation = 0,
        type = TileType.HALFCROSS
    };

    public float ratio;
    
    private BinTile pBinTile = new BinTile();
   
    GameObject Tile;
    
    private int n = 7;
    private int m = 7;
    private int deltaItem = 0;
    private int ReachableItem_1;
    private int ReachableItem_2;
    private int NextReachableItem_1;
    private int NextReachableItem_2;
    private int count = 0;
    private NodeTest player1;
    private NodeTest player2;

    public string location;
    public int rotate;
    public int sum_1;
    public int sum_2;
    public string info;
    public Vector3 target;
    public GameObject Board;

    private FileStream test;
    private StreamWriter testStreamWriter;
    private string str;
    
    private void Awake()
    {
        test = new FileStream("Assets/Resources/test.txt", FileMode.OpenOrCreate);
        testStreamWriter = new StreamWriter(test);
        str = "";
        ratio = 0f;
    }

    private void OnApplicationQuit()
    {
        testStreamWriter.Write(str);
        testStreamWriter.Close();
    }
    
    public void ScanBoard()
    {
        NodeTestInfoList.Clear();
        DFSList1.Clear();
        DFSList2.Clear();
        // 태그가 Ground인것만 리스트에 담기
        for (int i = 0; i < Board.transform.childCount; i++)
        {
            Tile = Board.transform.GetChild(i).gameObject;
            BinTile Node = new BinTile();
            // 타일이면 
            if (Tile.CompareTag("Ground"))
            {
                // 타일 종류
                switch (Tile.GetComponent<Node>().tileType)
                {
                    case TileType.HALFCROSS: 
                        Node.type = TileType.HALFCROSS;
                        Node.Shape = new List<List<int>>
                        {
                            new List<int> { 0, 1, 0 },
                            new List<int> { 1, 1, 0 },
                            new List<int> { 0, 1, 0 }
                        };
                        break;
                    case TileType.CORNER: 
                        Node.type = TileType.CORNER;
                        Node.Shape = new List<List<int>>
                        {
                            new List<int> { 0, 0, 0 },
                            new List<int> { 0, 1, 1 },
                            new List<int> { 0, 1, 0 }
                        };
                        break;
                    case TileType.STRAIGHT: 
                        Node.type = TileType.STRAIGHT;
                        Node.Shape = new List<List<int>>
                        {
                            new List<int> { 0, 1, 0 },
                            new List<int> { 0, 1, 0 },
                            new List<int> { 0, 1, 0 }
                        };
                        break;
                }
                //타일 회전 정보
                Node.rotation = (int)Tile.transform.rotation.eulerAngles.y / 90;
                for (int j = 0; j < Node.rotation; j++)
                    Node.Shape = RotateShapeCW(Node.Shape);

                // 타일 위치
                Node.x = Mathf.RoundToInt((-1f / 3f) * Tile.transform.position.z + 3f);
                Node.y = Mathf.RoundToInt((1f / 3f) * Tile.transform.position.x + 3f);

                // 플레이어 정보. 타일의 자식 오브젝트 탐색
                for (int j = Tile.transform.childCount - 1; j > 0; j--)
                {
                    // Debug.Log(Tile.transform.GetChild(j).transform.name);
                    // 타일의 자식중 tag가 player이고 이름이 player1이면 isplayer1 = true;
                    if (Tile.transform.GetChild(j).transform.name == "Player1")
                    {
                        Node.isPlayer1 = true;
                        Node.Shape[1][1] = 2;
                    }
                    else if (Tile.transform.GetChild(j).transform.name == "Player2")
                    {
                        Node.isPlayer2 = true;
                        Node.Shape[1][1] = 4;
                    }
                    else if (Tile.transform.GetChild(j).transform.CompareTag("Item_1"))
                    {
                        Node.IsPlayer1Item = true;
                        Node.Shape[1][1] = 3;
                    }
                    else if (Tile.transform.GetChild(j).transform.CompareTag("Item_2"))
                    {
                        Node.IsPlayer2Item = true;
                        Node.Shape[1][1] = 5;
                    }
                }
                // pBinTile
                if (Node.y > 6)
                {
                    pBinTile = new BinTile(Node);
                    continue;       // add안하고 넘어가기
                }
                
                // 담기는 건 i랑 무관. 중간에 하나는 pTile이라 인덱스상 1차이날 수 있다.
                /*str += i + "번째 노드 정보 \n" + "<Shape>" + "\n"; 
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        str += Node.Shape[j][k] +" ";
                    }
                    str += "\n";
                }
                str += "Type : " + Node.type + "\n" +
                       "Rotation " + Node.rotation + "\n" + 
                       "x : " + Tile.transform.position.x + "     z :  " + Tile.transform.position.z + "\n" +
                       "x2 : " + Node.x + "   y2 :  " + Node.y + "\n" + 
                       "isPlayer1 : " + Node.isPlayer1 + "   isPlayer2 :  " + Node.isPlayer2 + "\n"+
                       "IsPlayer1Item : " + Node.IsPlayer1Item + "   IsPlayer2Item :  " + Node.IsPlayer2Item + "\n\n";*/
            }
            NodeTestInfoList.Add(Node);
        }
        
        // NodeTestInfoList에 잘 담겼는지 확인. 이미 여기서 오류
        /*for (int i = 0; i < Board.transform.childCount - 1; i++)
        {
            PrintTileInfo(NodeTestInfoList[i], ref str);
            str += "\n";
        }*/
        // 보드 초기화
        board.Clear();
        boardList.Clear();
        
        for (int i = 0; i < n; i++)
        {
            List<BinTile> row = new List<BinTile>();
            for (int j = 0; j < m; j++)
                row.Add(new BinTile());
            board.Add(row);
        }
        // 7*7 보드에 저장
        for (int i = 0; i < Board.transform.childCount - 1; i++)
        {
            BinTile binTile = new BinTile(NodeTestInfoList[i]);
            board[NodeTestInfoList[i].x][NodeTestInfoList[i].y] = binTile;
        }

        str += "board 원본" + "\n";
        boardList = PrintBoard(board, ref str);
        printList2(boardList, ref str);

        // 밀어넣을 타일 랜덤으로 정하기
        str += "\n";
        str += "밀어넣을 타일" + "\n";
        PrintTileInfo(pBinTile, ref str);
        str += "\n" ;
        
        // 플레이어 위치정보 NodeTest타입으로 저장
        player1 = GetPlayer1Pos(boardList);
        player2 = GetPlayer2Pos(boardList);
        
        // 첫번째 판에서 dfs 출력
        DFSListAdd(DFSList1, boardList, player1);
        DFSListAdd(DFSList2, boardList, player2);
        
        // 도달 가능 아이템 수 변화를 계산하기 위해 다른 변수에 저장
        ReachableItem_1 = CheckReachableItem_1(DFSList1, ref str);
        str += "player1 reachable item : " + ReachableItem_1 + "\n";
        ReachableItem_2 = CheckReachableItem_2(DFSList2, ref str);
        str += "player2 reachable item : " + ReachableItem_2 + "\n";
        
        testStreamWriter.Write(str);
        testStreamWriter.Flush();
    }
    
    /*public void AIPushTile_1()
    {
        #region 48가지 경우의 수 
        for (int i = 0; i < 4; i++)
        {
            BinTile copyBinTile = new BinTile(pBinTile);
            
            // 회전 4가지. i에 의해 회전 가지수 결정
            for (int j = 0; j < i; j++)
                RotateTileCW(ref copyBinTile);

            // 각 회전에 따른 12가지 위치에 따른 다음 경우의 수 출력
            for (int j = 0; j < locations.Count; j++)
            {
                // 변화량 변수
                int deltaReachableItem_1 = 0;
                int deltaReachableItem_2 = 0;
                List<List<BinTile>> copiedBoard = new List<List<BinTile>>(0);
                Score score = new Score();
                
                // 원본 board를 copiedBoard에 복제
                foreach (List<BinTile> row in board)
                {
                    List<BinTile> copiedRow = new List<BinTile>(row);
                    copiedBoard.Add(copiedRow);
                }
                str += i + "번 회전, " + locations[j] + "에 push했을 때 경우" + "\n";
                PrintNextBoard(copyBinTile,copiedBoard,locations[j], ref str);
                
                // CopiedBoardList 가지고 DFS 계산
                DFSListAdd(DFSList1,CopiedBoardList, player1);
                DFSListAdd(DFSList2, CopiedBoardList, player2);        
                
                // △player1 reachable item = next - past
                NextReachableItem_1 = CheckReachableItem_1(DFSList1, ref str);
                // deltaReachableItem_1 = NextReachableItem_1 - ReachableItem_1;
                str += "player1 reachable item : " + NextReachableItem_1 + "\n";
                
                // △player2 reachable item = next - past
                NextReachableItem_2 = CheckReachableItem_2(DFSList2, ref str);
                deltaReachableItem_2 = NextReachableItem_2 - ReachableItem_2;
                str += "player2 reachable item : " + NextReachableItem_2 + "\n";
                
                // score클래스에 각 경우의 위치, 회전, 아이템 변화량 정보 저장후 리스트에 저장
                score.rotation = i;
                score.location = locations[j];
                score.Player1ReachableItem = NextReachableItem_1;
                score.Player2ReachableItem = NextReachableItem_2;
                // score.Player1_DeltaReachableItem = deltaReachableItem_1;
                score.Player2_DeltaReachableItem = deltaReachableItem_2;
                // score.Player1_opt = deltaReachableItem_1 - deltaReachableItem_2;
                sum_1 += NextReachableItem_1;
                // sum_2 += 4 - NextReachableItem_2;
                ScoreList.Add(score);
                // str += "Player1이 먹을 수 있는 a의 수 변화 : " + deltaReachableItem_1 + "\n";
                // str += "Player2가 먹을 수 있는 b의 수 변화 : " + deltaReachableItem_2 + "\n";
            }
        }
        

        #endregion 
        
        // max 구하기
        int maxDelta = 0;
        foreach (Score item in ScoreList)
        {
            if (item.Player2_DeltaReachableItem > maxDelta)
            {
                maxDelta = item.Player2_DeltaReachableItem;
            }
        }

        foreach (Score item in ScoreList)
        {
            if (maxDelta - item.Player2ReachableItem < 0) continue;
            sum_2 += maxDelta - item.Player2ReachableItem + 1;
        }
        // 각 경우의 수 정보 출력
        for (int i = 0; i < ScoreList.Count; i++)
        {
            str += ScoreList[i].rotation + ScoreList[i].location + "\t" +
                   ScoreList[i].Player1ReachableItem + "\n";
        }

        // 최선의 선택을 위해 Player1_opt 내림차순 정렬
        ScoreList.Sort((s1, s2) => s2.Player1ReachableItem.CompareTo(s1.Player1ReachableItem));
        str += "\n" + "정렬 후 " + "\n";
        
        // 정렬 후 각 경우의 수 정보 출력
        for (int i = 0; i < ScoreList.Count; i++)
        {
            str += ScoreList[i].rotation + ScoreList[i].location + "\t" +
                   ScoreList[i].Player1ReachableItem + "\n";
        }

        str += sum_1 + " " + sum_2 + " " + maxDelta + "\n";
        List<Score> arr_1 = new List<Score>();
        List<Score> arr_2 = new List<Score>();
        Score p1 = new Score();
        Score p2 = new Score();
        int p = 0;

        #region 내가 먹는 경우 확률
        if (sum_1 != 0)
        {
            for (int i = 0; i < ScoreList.Count; i++)
            {
                if (ScoreList[i].Player1ReachableItem > 0)
                {
                    for (int j = 0; j < ScoreList[i].Player1ReachableItem; j++)
                    {
                        Score score = new Score();
                        score.location = ScoreList[i].location;
                        score.rotation = ScoreList[i].rotation;
                        score.Player1ReachableItem = ScoreList[i].Player1ReachableItem;
                        score.Player2ReachableItem = ScoreList[i].Player2ReachableItem;
                        arr_1.Add(score);
                    }
                    float per = (float)ScoreList[i].Player1ReachableItem / sum_1 * 100;
                    ScoreList[i].Percent_1 = (float)Math.Round(per,2);
                }
            }
            p = Random.Range(0, arr_1.Count);
            p1.location = arr_1[p].location;
            p1.rotation = arr_1[p].rotation;
            p1.Player1ReachableItem = arr_1[p].Player1ReachableItem;
            p1.Player2ReachableItem = arr_1[p].Player2ReachableItem;
        }
        else
        {
            p = Random.Range(0, 48);
            p1.location = ScoreList[p].location;
            p1.rotation = ScoreList[p].rotation;
            p1.Player1ReachableItem = ScoreList[p].Player1ReachableItem;
            p1.Player2ReachableItem = ScoreList[p].Player2ReachableItem;
        }
        

        #endregion

        #region 상대가 못먹을 확률
        if (sum_2 != 0)
        {
            for (int i = 0; i < ScoreList.Count; i++)
            {
                if (ScoreList[i].Player2ReachableItem > 0)
                {
                    for (int j = 0; j < ScoreList[i].Player2ReachableItem; j++)
                    {
                        Score score = new Score();
                        score.location = ScoreList[i].location;
                        score.rotation = ScoreList[i].rotation;
                        score.Player1ReachableItem = ScoreList[i].Player1ReachableItem;
                        score.Player2ReachableItem = ScoreList[i].Player2ReachableItem;
                        arr_2.Add(score);
                    }
                    float per = (float)ScoreList[i].Player2ReachableItem / sum_2 * 100;
                    ScoreList[i].Percent_2 = (float)Math.Round(per,2);
                }
            }
            p = Random.Range(0, arr_2.Count);
            p2.location = arr_2[p].location;
            p2.rotation = arr_2[p].rotation;
            p2.Player1ReachableItem = arr_2[p].Player1ReachableItem;
            p2.Player2ReachableItem = arr_2[p].Player2ReachableItem;
        }
        else
        {
            p = Random.Range(0, 48);
            p2.location = ScoreList[p].location;
            p2.rotation = ScoreList[p].rotation;
            p2.Player1ReachableItem = ScoreList[p].Player1ReachableItem;
            p2.Player2ReachableItem = ScoreList[p].Player2ReachableItem;
        }
        #endregion

        // 확률 출력
        str += "======================== 확률 표======================= \n";
        for (int i = 0; i < ScoreList.Count; i++)
        {
            str += $"{ScoreList[i].rotation}{ScoreList[i].location} 확률 : " +
                   $"{ScoreList[i].Percent_1,5:0.00} %\t\t{ScoreList[i].Percent_2,5:0.00000} %\n";
        }
        str += "====================== p1 선택지 ====================== \n";
        for (int i = 0; i < arr_1.Count; i++)
        {
            str += arr_1[i].rotation + arr_1[i].location + "\n";
        }
        str += "====================== p2 선택지 ====================== \n";
        for (int i = 0; i < arr_2.Count; i++)
        {
            str += arr_2[i].rotation + arr_2[i].location + "\n";
        }
        str += "====================================================== \n";
        str += "p1 선택 : " + p1.rotation + p1.location + "\n";;
        str += "p2 선택 : " + p2.rotation + p2.location + "\n";;
        
        str += "====================== 최종 선택 ====================== \n";
        
        // 랜덤 뽑기
        p = Random.Range(0, 100);       // (0~99)
        // ratio = 내가 쓴 숫자가 p1의 비율
        
        str += rotate + location + "\n";;
        for (int i = 0; i < rotate; i++)
            RotateTileCW(ref pBinTile);
        
        // 이제 이 rotation과 location 정보를 이용해 밀어넣으면 된다. 
        // =======================================================================
        // 미는 동작은?
        // 밀고 난 후 보드 출력
        str += rotate + "회전 후 " + location + "에 push 한 후" + "\n";
        PushTile(ref pBinTile, board, location);
        boardList = PrintBoard(board, ref str);
        printList2(boardList, ref str);
        
        str += "\n";
        str += "밀어넣을 타일" + "\n";
        PrintTileInfo(pBinTile, ref str);
        str += "\n";

        str += "Player1 "; DFSListAdd(DFSList1,boardList, player1);
        ReachableItem_1 = CheckReachableItem_1(DFSList1, ref str);
        str += "player1 reachable item : " + ReachableItem_1 + "\n";
        
        str += "Player2 "; DFSListAdd(DFSList2,boardList, player2);
        ReachableItem_2 = CheckReachableItem_2(DFSList2, ref str);
        str += "player2 reachable item : " + ReachableItem_2 + "\n";
        
        testStreamWriter.Write(str);
        testStreamWriter.Flush();
    }*/
    
    public void AIPushTile_2()
    {
        str += "AIPushTile_2 호출" + '\n';
        List<Score> ScoreList = new List<Score>();
        List<Score> arr_1 = new List<Score>();
        List<Score> arr_2 = new List<Score>();
        // arr_1.Clear();
        // arr_2.Clear();
        
        #region 48가지 경우의 수 
        for (int i = 0; i < 4; i++)
        {
            BinTile copyBinTile = new BinTile(pBinTile);
            
            // 회전 4가지. i에 의해 회전 가지수 결정
            for (int j = 0; j < i; j++)
                RotateTileCW(ref copyBinTile);

            // 각 회전에 따른 12가지 위치에 따른 다음 경우의 수 출력
            for (int j = 0; j < locations.Count; j++)
            {
                // 변화량 변수
                int deltaReachableItem_1 = 0;
                int deltaReachableItem_2 = 0;
                List<List<BinTile>> copiedBoard = new List<List<BinTile>>(0);
                Score score = new Score();
                
                // 원본 board를 copiedBoard에 복제
                foreach (List<BinTile> row in board)
                {
                    List<BinTile> copiedRow = new List<BinTile>(row);
                    copiedBoard.Add(copiedRow);
                }
                str += i + "번 회전, " + locations[j] + "에 push했을 때 경우" + "\n";
                PrintNextBoard(copyBinTile,copiedBoard,locations[j], ref str);
                
                // CopiedBoardList 가지고 DFS 계산
                DFSListAdd(DFSList1,CopiedBoardList, player1);
                DFSListAdd(DFSList2, CopiedBoardList, player2);        
                
                // △player1 reachable item = next - past
                NextReachableItem_1 = CheckReachableItem_1(DFSList1, ref str);
                // deltaReachableItem_1 = NextReachableItem_1 - ReachableItem_1;
                str += "player1 reachable item : " + NextReachableItem_1 + "\n";
                
                // △player2 reachable item = next - past
                NextReachableItem_2 = CheckReachableItem_2(DFSList2, ref str);
                // deltaReachableItem_2 = NextReachableItem_2 - ReachableItem_2;
                str += "player2 reachable item : " + NextReachableItem_2 + "\n";
                
                // score클래스에 각 경우의 위치, 회전, 아이템 변화량 정보 저장후 리스트에 저장
                score.rotation = i;
                score.location = locations[j];
                score.Player1ReachableItem = NextReachableItem_1;
                score.Player2ReachableItem = NextReachableItem_2;
                score.Player1_DeltaReachableItem = deltaReachableItem_1;
                score.Player2_DeltaReachableItem = deltaReachableItem_2;
                // score.Player1_opt = deltaReachableItem_1 - deltaReachableItem_2;
                sum_1 += NextReachableItem_1;
                sum_2 += NextReachableItem_2;
                ScoreList.Add(score);
                // str += "Player1이 먹을 수 있는 a의 수 변화 : " + deltaReachableItem_1 + "\n";
                // str += "Player2가 먹을 수 있는 b의 수 변화 : " + deltaReachableItem_2 + "\n";
            }
        }
        #endregion 
        
        /*// 각 경우의 수 정보 출력
        for (int i = 0; i < ScoreList.Count; i++)
        {
            str += ScoreList[i].rotation + ScoreList[i].location + "\t" +
                   ScoreList[i].Player2ReachableItem + "\n";
        }

        // 최선의 선택을 위해 Player1_opt 내림차순 정렬
        ScoreList.Sort((s1, s2) => s2.Player2ReachableItem.CompareTo(s1.Player2ReachableItem));
        str += "\n" + "정렬 후 " + "\n";
        
        // 정렬 후 각 경우의 수 정보 출력
        for (int i = 0; i < ScoreList.Count; i++)
        {
            str += ScoreList[i].rotation + ScoreList[i].location + "\t" +
                   ScoreList[i].Player2ReachableItem + "\n";
        }*/

        str += sum_1 + " " + sum_2 + " " + "\n";
        
        // Score p1 = new Score();
        // Score p2 = new Score();
        int p = 0;

        #region player2가 먹는 경우 확률
        if (sum_2 != 0)
        {
            for (int i = 0; i < ScoreList.Count; i++)
            {
                // 해당 경우에서 먹을 수 있는 아이템이 있는가?
                if (ScoreList[i].Player2ReachableItem > 0)
                {
                    for (int j = 0; j < ScoreList[i].Player2ReachableItem; j++)
                    {
                        Score score = new Score();
                        score.location = ScoreList[i].location;
                        score.rotation = ScoreList[i].rotation;
                        score.Player1ReachableItem = ScoreList[i].Player1ReachableItem;
                        score.Player2ReachableItem = ScoreList[i].Player2ReachableItem;
                        arr_2.Add(score);
                    }
                    float per = (float)ScoreList[i].Player2ReachableItem / sum_2 * 100;
                    ScoreList[i].Percent_2 = (float)Math.Round(per,2);
                }
            }
            /*
             p = Random.Range(0, arr_2.Count);
            p2.location = arr_2[p].location;
            p2.rotation = arr_2[p].rotation;
            p2.Player1ReachableItem = arr_2[p].Player1ReachableItem;
            p2.Player2ReachableItem = arr_2[p].Player2ReachableItem;
            */
        }
        else
        {
            for (int i = 0; i < ScoreList.Count; i++)
            {
                for (int j = 0; j < ScoreList[i].Player2ReachableItem; j++)
                {
                    Score score = new Score();
                    score.location = ScoreList[i].location;
                    score.rotation = ScoreList[i].rotation;
                    score.Player1ReachableItem = ScoreList[i].Player1ReachableItem;
                    score.Player2ReachableItem = ScoreList[i].Player2ReachableItem;
                    arr_2.Add(score);
                }
                // float per = (float)ScoreList[i].Player2ReachableItem / sum_2 * 100;
                ScoreList[i].Percent_2 = (float)Math.Round(1/48f,2);
            }
            /*p = Random.Range(0, 48);
            p2.location = ScoreList[p].location;
            p2.rotation = ScoreList[p].rotation;
            p2.Player1ReachableItem = ScoreList[p].Player1ReachableItem;
            p2.Player2ReachableItem = ScoreList[p].Player2ReachableItem;*/
        }
        

        #endregion

        #region player1이 먹는 경우 확률
        if (sum_1 != 0)
        {
            for (int i = 0; i < ScoreList.Count; i++)
            {
                if (ScoreList[i].Player1ReachableItem > 0)
                {
                    for (int j = 0; j < ScoreList[i].Player1ReachableItem; j++)
                    {
                        Score score = new Score();
                        score.location = ScoreList[i].location;
                        score.rotation = ScoreList[i].rotation;
                        score.Player1ReachableItem = ScoreList[i].Player1ReachableItem;
                        score.Player2ReachableItem = ScoreList[i].Player2ReachableItem;
                        arr_1.Add(score);
                    }
                    float per = (float)ScoreList[i].Player1ReachableItem / sum_1 * 100;
                    ScoreList[i].Percent_1 = (float)Math.Round(per,2);
                }
            }
            /*p = Random.Range(0, arr_1.Count);
            p1.location = arr_1[p].location;
            p1.rotation = arr_1[p].rotation;
            p1.Player1ReachableItem = arr_1[p].Player1ReachableItem;
            p1.Player2ReachableItem = arr_1[p].Player2ReachableItem;*/
        }
        else
        {
            for (int i = 0; i < ScoreList.Count; i++)
            {
                 for (int j = 0; j < ScoreList[i].Player1ReachableItem; j++)
                 {
                     Score score = new Score();
                     score.location = ScoreList[i].location;
                     score.rotation = ScoreList[i].rotation;
                     score.Player1ReachableItem = ScoreList[i].Player1ReachableItem;
                     score.Player2ReachableItem = ScoreList[i].Player2ReachableItem;
                     arr_1.Add(score);
                 }
                 // float per = (float)ScoreList[i].Player1ReachableItem / sum_1 * 100;
                 ScoreList[i].Percent_1 = (float)Math.Round(1/48f,2);
            }
            /*p = Random.Range(0, 48);
            p1.location = ScoreList[p].location;
            p1.rotation = ScoreList[p].rotation;
            p1.Player1ReachableItem = ScoreList[p].Player1ReachableItem;
            p1.Player2ReachableItem = ScoreList[p].Player2ReachableItem;*/
        }


        #endregion

        // 0 -> p2만 고려
        // 1 -> p1만 고려
        
        // 확률 출력
        str += "======================== 확률 표======================= \n";
        for (int i = 0; i < ScoreList.Count; i++)
        {
            ScoreList[i].per = (ScoreList[i].Percent_1 * ratio) + (ScoreList[i].Percent_2 * (1 - ratio));
            str += $"{ScoreList[i].rotation}{ScoreList[i].location} 확률 : " +
                   $"{ScoreList[i].Percent_1,5:0.0000} %\t\t{ScoreList[i].Percent_2,5:0.0000} %\t\t{ScoreList[i].per,5:0.0000} %\n";
        }
        
        // per를 기준으로 백분율화?

        str += "====================== p1 선택지 ====================== \n";
        for (int i = 0; i < arr_1.Count; i++)
        {
            str += arr_1[i].rotation + arr_1[i].location + "\n";
        }
        str += "====================== p2 선택지 ====================== \n";
        for (int i = 0; i < arr_2.Count; i++)
        {
            str += arr_2[i].rotation + arr_2[i].location + "\n";
        }
        str += "====================== 최종 선택 ====================== \n";

        // 가장 높은 확률인거 고르기
        int maxPerIndex = 0;
        float maxPer = 0f;
        for (int i = 0; i < ScoreList.Count; i++)
        {
            if (ScoreList[i].per > maxPer)
            {
                maxPer = ScoreList[i].per;
                maxPerIndex = i;
            }
        }
        location = ScoreList[maxPerIndex].location;
        rotate = ScoreList[maxPerIndex].rotation;
        
        // 랜덤 뽑기
        
        str += rotate + location + "\n";;
        for (int i = 0; i < rotate; i++)
            RotateTileCW(ref pBinTile);
        info += location;       // 꼴이 R0 이런 식.
        // 이제 이 rotation과 location 정보를 이용해 밀어넣으면 된다. 
        // =======================================================================
        // 미는 동작은?
        // 밀고 난 후 보드 출력
        str += rotate + "회전 후 " + location + "에 push 한 후" + "\n";
        PushTile(ref pBinTile, board, location);
        boardList = PrintBoard(board, ref str);
        printList2(boardList, ref str);
        
        str += "\n";
        str += "밀어넣을 타일" + "\n";
        PrintTileInfo(pBinTile, ref str);
        str += "\n";

        str += "Player1 "; DFSListAdd(DFSList1,boardList, player1);
        ReachableItem_1 = CheckReachableItem_3(DFSList1, ref str);
        str += "player1 reachable item : " + ReachableItem_1 + "\n";
        
        str += "Player2 "; DFSListAdd(DFSList2,boardList, player2);
        // str += "DFSList2.Count : " + DFSList2.Count + "\n";
        ReachableItem_2 = CheckReachableItem_4(DFSList2, ref str);
        str += "player2 reachable item : " + ReachableItem_2 + "\n";
        
        testStreamWriter.Write(str);
        testStreamWriter.Flush();
    }

    public GameObject AIMove()
    {
        // ScanBoard();
        str += "AIMove 호출" + "\n";
        str += "AIMove() - DFSList2.Count : " + DFSList2.Count + "\n";
        str += "AIMove() - ReachableItem_2 : " + ReachableItem_2 + "\n";
        if (DFSList2.Count > 3)
        {
            Debug.Log(DFSList2.Count);
            // 아이템이 있는 지점
            if (ReachableItem_2 > 0)
            {
                Debug.Log(ReachableItem_2);
                // move
                // 도착지점 = DFSList에서 5 인 곳
                // 정해주는 순간 PathFinding해서 이동.
                for (int i = 0; i < DFSList2.Count; i++)
                {
                    if (DFSList2[i].num == 5)
                    {
                        Debug.Log((i+1) + "번째");
                        Debug.Log(DFSList2[i].x + ", " + DFSList2[i].y);
                        int tx = 3 * (DFSList2[i].x/3 - 3);
                        int tz = (-3) * (DFSList2[i].y/3 - 3);
                        target = new Vector3(tx, 0, tz);
                        Debug.Log(tx + ", " + tz);
                        for (int j = 0; j < Board.transform.childCount; j++)
                        {
                            Tile = Board.transform.GetChild(j).gameObject;

                            if (Vector3.Distance(Tile.transform.position, target) <= 0.1f)
                            {
                                Debug.Log(j);
                                return Tile;
                            }
                        }
                    }
                }
            }
            else       // 도착지점 = 아무 곳?
            {
                int r = 0;
                do
                {
                    r = Random.Range(0, DFSList2.Count);
                    if (DFSList2[r].num == 1)
                    {
                        Debug.Log((r+1) + "번째\n");
                        Debug.Log(DFSList2[r].x + ", " + DFSList2[r].y);
                        int tx = 3 * (DFSList2[r].x/3 - 3);
                        int tz = (-3) * (DFSList2[r].y/3 - 3);
                        target = new Vector3(tx, 0, tz);
                        for (int j = 0; j < Board.transform.childCount; j++)
                        {
                            Tile = Board.transform.GetChild(j).gameObject;
                            if (Vector3.Distance(Tile.transform.position, target) <= 0.1f) return Tile;
                        }
                    }
                } while (DFSList2[r].num != 1);
            }
        }
        else // 도착지점 = 제자리
        {
            for (int i = 0; i < DFSList2.Count; i++)
            {
                if (DFSList2[i].num == 4)
                {
                    Debug.Log((i+1) + "번째\n");
                    Debug.Log(DFSList2[i].x + ", " + DFSList2[i].y);
                    int tx = 3 * (DFSList2[i].x/3 - 3);
                    int tz = (-3) * (DFSList2[i].y/3 - 3);
                    target = new Vector3(tx, 0, tz);
                    Debug.Log(tx + ", " + tz);
                    for (int j = 0; j < Board.transform.childCount; j++)
                    {
                        Tile = Board.transform.GetChild(j).gameObject;
                        if (Vector3.Distance(Tile.transform.position, target) <= 0.1f) return Tile;
                    }
                }
            }
        }
        
        str += "AIMove 끝" + "\n";
        testStreamWriter.Write(str);
        testStreamWriter.Flush();
        
        return Tile;
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            // StartCoroutine(AIChoosing());
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            // StartCoroutine(Scanning());
        }
    }
    
    IEnumerator Scanning()
    {
        yield return StartCoroutine(Value01Co());
        ScanBoard();
        print("로딩 완료");
        value01 = 0;
    }
    
    IEnumerator AIChoosing()
    {
        yield return StartCoroutine(Value01Co());
        // AIPushTile_1();
        AIPushTile_2();
        print("48가지 경우의 수 계산");
        value01 = 0;
    }

    List<List<int>> RotateShapeCW(List<List<int>> shape)
    {
        int n = shape.Count;
        List<List<int>> rotatedShape = new List<List<int>>();
        for (int i = 0; i < n; i++)
        {
            rotatedShape.Add(new List<int>());
            for (int j = 0; j < n; j++)
            {
                rotatedShape[i].Add(shape[n - j - 1][i]);
            }
        }
        return rotatedShape;
    }

    void RotateTileCW(ref BinTile binTile)
    {
        List<List<int>> r = new List<List<int>>();
        foreach (var row in binTile.Shape) {
            var newRow = new List<int>(row);
            r.Add(newRow);
        }
        int n = binTile.Shape.Count;
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                binTile.Shape[j][n - 1 - i] = r[i][j];
            }
        }
        if (binTile.rotation >= 3) binTile.rotation = 0;
        else binTile.rotation++;
    }
    void RotateTileRand(ref BinTile binTile) 
    {
        int n = Random.Range(0, 4);
        for (int i = 0; i < n; i++) RotateTileCW(ref binTile); 
    }

    List<List<NodeTest>> PrintBoard(List<List<BinTile>> v, ref string str)
    {
        int k = 0, l = 0;
        List<List<NodeTest>> boardList2 = new List<List<NodeTest>>();
        NodeTest node = new NodeTest();
        // l에 대한 for문을 나중에 돌려서 리스트 크기를 따로 저장
        int x = v[k][l].Shape.Count;
        for (k = 0; k < v.Count; ++k)
        {
            for (int i = 0; i < x; ++i)
            {
                List<NodeTest> row = new List<NodeTest>();
                for (l = 0; l < v[k].Count; ++l)
                {
                    // 가로 모두 출력
                    for (int j = 0; j < v[k][l].Shape[i].Count; ++j)
                    {
                        node = new NodeTest(); // 새로운 NodeTest 객체 생성
                        node.x = 3 * l + j;
                        node.y = 3 * k + i;
                        node.num = v[k][l].Shape[i][j];
                        row.Add(node);
                        str += node.num + " ";
                    }
                    str += "\t";
                }
                boardList2.Add(row);
                str += "\n";
            }
            str += "\n";
        }
        return boardList2;
    }
    
    void printList2(List<List<NodeTest>> v, ref string str)
    {
        for (int i = 0; i < v.Count; i++)
        {
            for (int j = 0; j < v[i].Count; j++)
            {
                switch (v[i][j].num)
                {
                    case 0:
                        str += "# ";
                        break;
                    case 1:
                        str += "  ";
                        break;
                    case 2:
                        str += "A ";
                        break;
                    case 3:
                        str += "a ";
                        break;
                    case 4:
                        str += "B ";
                        break;
                    case 5:
                        str += "b ";
                        break;
                    default:
                        break;
                }
            }
            str += "\n";
        }
    }

    void PrintTileInfo(BinTile binTile, ref string str)
    {
        for (int i = 0; i < binTile.Shape.Count; i++)
        {
            for (int j = 0; j < binTile.Shape[i].Count; j++)
                str += binTile.Shape[i][j] + " ";
            str += "\n";
        }
        str += "tile.rotation : " + binTile.rotation + "\n";
        str += "tile.type : " + binTile.type + "\n";
    }
    
    void PrintNextBoard(BinTile binTile, List<List<BinTile>> board, string location, ref string str)
    {
        BinTile temp;
        char ch = location[0];
        int num = (location[1] - '0') * 2 + 1;
        switch (ch)
        {
            case 'L':
                temp = board[num][board.Count-1];
                for (int i = board.Count-1; i >= 1; i--)
                {
                    board[num][i] = board[num][i-1];
                }
                board[num][0] = binTile;
                binTile = temp;
                break;
            case 'R':
                temp = board[num][0];
                for (int i = 0; i < board.Count-1; i++)
                {
                    board[num][i] = board[num][i+1];
                }
                board[num][board.Count-1] = binTile;
                binTile = temp;
                break;
            case 'B':
                temp = board[0][num];
                for (int i = 0; i < board.Count-1; i++)
                {
                    board[i][num] = board[i+1][num];
                }
                board[board.Count-1][num] = binTile;
                binTile = temp;
                break;
            case 'T':
                temp = board[board.Count-1][num];
                for (int i = board.Count-1; i >= 1; i--)
                {
                    board[i][num] = board[i-1][num];
                }
                board[0][num] = binTile;
                binTile = temp;
                break;
            default:
                break;
        }
        CopiedBoardList = PrintBoard(board, ref str);
        PrintTileInfo(binTile, ref str);
    }
    
    void PushTile(ref BinTile binTile, List<List<BinTile>> board, string location)
    {
        BinTile temp;
        char ch = location[0];
        int num = (location[1] - '0') * 2 + 1;
        switch (ch)
        {
            case 'L':
                temp = board[num][board.Count - 1];
                for (int i = board.Count - 1; i >= 1; i--)
                {
                    board[num][i] = board[num][i - 1];
                }
                board[num][0] = binTile;
                binTile = temp;
                // locations[3 + num].Remove();
                break;
            case 'R':
                temp = board[num][0];
                for (int i = 0; i < board.Count - 1; i++)
                {
                    board[num][i] = board[num][i + 1];
                }
                board[num][board.Count - 1] = binTile;
                binTile = temp;
                break;
            case 'B':
                temp = board[0][num];
                for (int i = 0; i < board.Count - 1; i++)
                {
                    board[i][num] = board[i + 1][num];
                }
                board[board.Count - 1][num] = binTile;
                binTile = temp;
                break;
            case 'T':
                temp = board[board.Count - 1][num];
                for (int i = board.Count - 1; i >= 1; i--)
                {
                    board[i][num] = board[i - 1][num];
                }
                board[0][num] = binTile;
                binTile = temp;
                break;
            default:
                break;
        }
    }
    
    NodeTest GetPlayer1Pos(List<List<NodeTest>> nodeList)
    {
        NodeTest player1 = new NodeTest();
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = 0; j < nodeList[i].Count; j++)
            {
                if(nodeList[i][j].num == 2) 
                {
                    player1.num = 2;
                    player1.x = j;
                    player1.y = i;
                    return player1;
                }
            }        
        }
        return new NodeTest();
    }
    NodeTest GetPlayer2Pos(List<List<NodeTest>> nodeList)
    {
        NodeTest player2 = new NodeTest();
        for (int i = 0; i < nodeList.Count; i++)
        {
            for (int j = 0; j < nodeList[i].Count; j++)
            {
                if(nodeList[i][j].num == 4) 
                {
                    player2.num = 4;
                    player2.x = j;
                    player2.y = i;
                    return player2;
                }
            }        
        }
        return new NodeTest();
    }
    
    void DFSListAdd(List<NodeTest> DFSList, List<List<NodeTest>> NodeList, NodeTest currentNode)
    {
        NodeTest NeighborNode = new NodeTest();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Skip the currentNode itself
                if (i == 0 && j == 0) continue;
                
                int nexty = currentNode.y + i;
                int nextx = currentNode.x + j;

                // Check if nexty and nextx are within the bounds of the boardList
                if (nexty >= 0 && nexty < NodeList.Count && nextx >= 0 && nextx < NodeList[0].Count)
                {
                    if (NodeList[nexty][nextx].num >= 1 && !NodeList[nexty][nextx].isVisited)
                    {
                        NodeList[nexty][nextx].isVisited = true;
                        NeighborNode = NodeList[nexty][nextx];

                        DFSList.Add(NeighborNode);
                        DFSListAdd(DFSList, NodeList, NeighborNode);
                    }
                }
            }
        }
    }
    
    int CheckReachableItem_1(List<NodeTest> DFSList, ref string str)
    {
        int count = 0;
        str += "DFSList : ";
        for (int i = 0; i < DFSList.Count; i++)
        {
            if(DFSList[i].num == 3) count++;
            str += DFSList[i].num + " ";
        }
        str += "\n";
        DFSList.Clear();
        return count;
    }
    int CheckReachableItem_3(List<NodeTest> DFSList, ref string str)
    {
        int count = 0;
        str += "DFSList : ";
        for (int i = 0; i < DFSList.Count; i++)
        {
            if(DFSList[i].num == 3) count++;
            str += DFSList[i].num + " ";
        }
        str += "\n";
        return count;
    }
    int CheckReachableItem_4(List<NodeTest> DFSList, ref string str)
    {
        int count = 0;
        str += "DFSList : ";
        for (int i = 0; i < DFSList.Count; i++)
        {
            if(DFSList[i].num == 5) count++;
            str += DFSList[i].num + " ";
        }
        str += "\n";
        return count;
    }
    
    int CheckReachableItem_2(List<NodeTest> DFSList, ref string str)
    {
        int count = 0;
        str += "DFSList : ";
        for (int i = 0; i < DFSList.Count; i++)
        {
            if(DFSList[i].num == 5) count++;
            str += DFSList[i].num + " ";
        }
        str += "\n";
        DFSList.Clear();
        return count;
    }
}
