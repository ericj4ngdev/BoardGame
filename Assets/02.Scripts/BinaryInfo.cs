using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Random = UnityEngine.Random;

enum TileType  
{
    STRAIGHT,
    CORNER,
    HALFCROSS
};

struct Tile 
{
    public Tile(Tile tile)
    {
        Shape = new List<List<int>>();
        for (int i = 0; i < tile.Shape.Count; i++)
        {
            Shape.Add(new List<int>());
            for (int j = 0; j < tile.Shape[i].Count; j++)
            {
                Shape[i].Add(tile.Shape[i][j]);
            }
        }
        rotation = tile.rotation;
        type = tile.type;
        x = tile.x;
        y = tile.y;
        isvisited = tile.isvisited;
        isPlayer1 = tile.isPlayer1;
        isPlayer2 = tile.isPlayer2;
        IsPlayer1Item = tile.IsPlayer1Item;
        IsPlayer2Item = tile.IsPlayer2Item;
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
};

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
    public int precent;
    // public int Player1_DeltaReachableItem;
    // public int Player2_DeltaReachableItem;
    // public int Player1_opt;
}

struct Player
{
    public int num;
    public int x;
    public int y;
    public int target;
    public int targetCount;
}

public class BinaryInfo : MonoBehaviour
{
    List<List<NodeTest>> boardList = new List<List<NodeTest>>();
    List<List<NodeTest>> CopiedBoardList = new List<List<NodeTest>>();
    
    private List<Score> ScoreList = new List<Score>();
    
    
    Tile straight = new Tile {
        Shape = new List<List<int>> {
            new List<int> {0, 0, 0},
            new List<int> {1, 1, 1},
            new List<int> {0, 0, 0}
        },
        rotation = 0,
        type = TileType.STRAIGHT
    };
    Tile corner = new Tile {
        Shape = new List<List<int>> {
            new List<int> {0, 0, 0},
            new List<int> {0, 1, 1},
            new List<int> {0, 1, 0}
        },
        rotation = 0,
        type = TileType.CORNER
    };
    Tile halfcross = new Tile {
        Shape = new List<List<int>> {
            new List<int> {0, 1, 0},
            new List<int> {1, 1, 0},
            new List<int> {0, 1, 0}
        },
        rotation = 0,
        type = TileType.HALFCROSS
    };

    private Tile pTile = new Tile();
    private List<List<Tile>> board = new List<List<Tile>>(0);
    private List<NodeTest> DFSList1 = new List<NodeTest>();
    private List<NodeTest> DFSList2 = new List<NodeTest>();

    List<string> locations = new List<string>
    {   "L0", "L1","L2",
        "R0","R1","R2",
        "T0","T1","T2",
        "B0","B1","B2"
    };
    
    
    private int n = 7;
    private int m = 7;
    private int rotate;
    private string location;
    private int deltaItem = 0;

    private int ReachableItem_1;
    private int ReachableItem_2;
    private int NextReachableItem_1;
    private int NextReachableItem_2;
    private int count = 0;
    
    private void Start()
    {
        // 메모장 출력용 코드
        FileStream test = new FileStream("Assets/Resources/test.txt", FileMode.Create);
        StreamWriter testStreamWriter = new StreamWriter(test);
        string str = "";
        
        
        int rand = Random.Range(0, 3);
        
        List<Tile> TileList = new List<Tile> {
            straight,
            corner,
            halfcross
        };

        // 보드 초기화
        for (int i = 0; i < n; i++)
        {
            List<Tile> row = new List<Tile>();
            for (int j = 0; j < m; j++)
                row.Add(new Tile());
            board.Add(row);
        }
        
        GenerateBoard(TileList,ref board, ref str);
        
        SpawnPlayer(board);
        SpawnItem(board);
        
        str += "board 원본" + "\n";
        boardList = PrintBoard(board, ref str);
        printList2(boardList, ref str);

        // 플레이어 위치정보 NodeTest타입으로 저장
        NodeTest player1 = GetPlayer1Pos(boardList);
        NodeTest player2 = GetPlayer2Pos(boardList);
        
        // 밀어넣을 타일 랜덤으로 정하기
        str += "\n";
        str += "밀어넣을 타일" + "\n";
        pTile = TileList[rand];
        RotateTileRand(ref pTile);
        PrintTileInfo(pTile, ref str);
        str += "\n" ;
        
        // 첫번째 판에서 dfs 출력
        DFSListAdd(DFSList1, boardList, player1);
        DFSListAdd(DFSList2, boardList, player2);
        
        // 도달 가능 아이템 수 변화를 계산하기 위해 다른 변수에 저장
        ReachableItem_1 = CheckReachableItem_1(DFSList1, ref str);
        str += "player1 reachable item : " + ReachableItem_1 + "\n";
        ReachableItem_2 = CheckReachableItem_2(DFSList2, ref str);
        str += "player2 reachable item : " + ReachableItem_2 + "\n";
        
        int sum = 0;

        // 48가지 경우의 수 
        for (int i = 0; i < 4; i++)
        {
            Tile copyTile = new Tile(pTile);
            
            // 회전 4가지. i에 의해 회전 가지수 결정
            for (int j = 0; j < i; j++)
                RotateTileCW(ref copyTile);

            // 각 회전에 따른 12가지 위치에 따른 다음 경우의 수 출력
            for (int j = 0; j < locations.Count; j++)
            {
                // 변화량 변수
                int deltaReachableItem_1 = 0;
                int deltaReachableItem_2 = 0;
                List<List<Tile>> copiedBoard = new List<List<Tile>>(0);
                Score score = new Score();
                
                // 원본 board를 copiedBoard에 복제
                foreach (List<Tile> row in board)
                {
                    List<Tile> copiedRow = new List<Tile>(row);
                    copiedBoard.Add(copiedRow);
                }
                str += i + "번 회전, " + locations[j] + "에 push했을 때 경우" + "\n";
                PrintNextBoard(copyTile,copiedBoard,locations[j], ref str);
                
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
                // score.Player1_DeltaReachableItem = deltaReachableItem_1;
                // score.Player2_DeltaReachableItem = deltaReachableItem_2;
                // score.Player1_opt = deltaReachableItem_1 - deltaReachableItem_2;
                ScoreList.Add(score);
                sum += NextReachableItem_1;
                // str += "Player1이 먹을 수 있는 a의 수 변화 : " + deltaReachableItem_1 + "\n";
                // str += "Player2가 먹을 수 있는 b의 수 변화 : " + deltaReachableItem_2 + "\n";
            }
        }

        /*// 각 경우의 수 정보 출력
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
        }*/

        int p = 0;
        List<Score> arr = new List<Score>();
        for(int i=0; i<100; i++){
            Score score = new Score();
            score.Player1ReachableItem = -1;
            arr.Add(score);
        }

        count = 0;
        if (sum != 0)
        {
            while (true)
            {
                p = Random.Range(0, 100);
                // 빈칸인가?
                if (arr[p].Player1ReachableItem == -1)
                {
                    arr[p].rotation = ScoreList[count].rotation; // 빈칸이면 채워넣기
                    arr[p].location = ScoreList[count].location; // 빈칸이면 채워넣기
                    arr[p].Player1ReachableItem = ScoreList[count].Player1ReachableItem; // 빈칸이면 채워넣기
                    arr[p].Player2ReachableItem = ScoreList[count].Player2ReachableItem; // 빈칸이면 채워넣기
                    arr[p].precent = (ScoreList[count].Player1ReachableItem / sum) * 100;  //  확률
                    count++;
                }
                if (count == (ScoreList[count].Player1ReachableItem / sum * 100)) break;
            }
        }
        str += arr.Count + "100개 배열 " + "\n";
        for (int i = 0; i < arr.Count; i++)
        {
            str += arr[i].rotation + arr[i].location + " 확률 : \t" +
                   arr[i].precent + "\n";
        }
        
        p = Random.Range(0, 100);
        if(arr[p].Player1ReachableItem != -1)
        {
            // 제일 점수가 높은 첫번째 Score의 정보로 Tile push
            rotate = arr[p].rotation;
            location = arr[p].location;
            for (int i = 0; i < rotate; i++)
                RotateTileCW(ref pTile);
            str += rotate + "회전 후 " + location + "에 push 한 후" + "\n";
            PushTile(ref pTile, board, location);
        }
        
        
        /*// 제일 점수가 높은 첫번째 Score의 정보로 Tile push
        rotate = ScoreList[0].rotation;
        location = ScoreList[0].location;
        for (int i = 0; i < rotate; i++)
            RotateTileCW(ref pTile);
        str += rotate + "회전 후 " + location + "에 push 한 후" + "\n";
        PushTile(ref pTile, board, location);*/
        // locations - location
        // 밀고 난 후 보드 출력
        boardList = PrintBoard(board, ref str);
        printList2(boardList, ref str);
        
        str += "\n";
        str += "밀어넣을 타일" + "\n";
        PrintTileInfo(pTile, ref str);
        str += "\n";

        str += "Player1 "; DFSListAdd(DFSList1,boardList, player1);
        ReachableItem_1 = CheckReachableItem_1(DFSList1, ref str);
        str += "player1 reachable item : " + ReachableItem_1 + "\n";
        
        str += "Player2 "; DFSListAdd(DFSList2,boardList, player2);
        ReachableItem_2 = CheckReachableItem_2(DFSList2, ref str);
        str += "player2 reachable item : " + ReachableItem_2 + "\n";

        testStreamWriter.Write(str);
        testStreamWriter.Close();
    }


    /// <summary>
    /// 보드 생성 함수
    /// </summary>
    /// <param name="list">타일 3가지 종류</param>
    /// <param name="board">원본 보드</param>
    /// <param name="str">출력용 매개변수</param>
    void GenerateBoard(List<Tile> list, ref List<List<Tile>> board, ref string str)
    {
        // 7*7 자동생성
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].Count; ++j)
            {
                Tile tile = new Tile(list[Random.Range(0,3)]);
                RotateTileRand(ref tile);
                board[i][j] = tile;
            }
        }

        // 정사각형 보드라 가정하고 모서리 좌표를 board.Count로 통일
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                Tile CornerTile = new Tile(corner);
                board[0][0] = CornerTile;
            }
            else if (i == 1)
            {
                Tile CornerTile = new Tile(corner);
                for (int j = 0; j < i; j++)
                {
                    RotateTileCW(ref CornerTile);
                }
                board[0][board.Count-1] = CornerTile;
            }
            else if (i == 2)
            {
                Tile CornerTile = new Tile(corner);
                for (int j = 0; j < i; j++)
                {
                    RotateTileCW(ref CornerTile);
                }
                board[board.Count-1][board.Count-1] = CornerTile;
            }
            else if (i == 3)
            {
                Tile CornerTile = new Tile(corner);
                for (int j = 0; j < i; j++)
                {
                    RotateTileCW(ref CornerTile);
                }
                board[board.Count-1][0] = CornerTile;
            }
        }
    }
    
    void RotateTileCW(ref Tile tile)
    {
        List<List<int>> r = new List<List<int>>();
        foreach (var row in tile.Shape) {
            var newRow = new List<int>(row);
            r.Add(newRow);
        }
        int n = tile.Shape.Count;
        for (int i = 0; i < n; ++i)
        {
            for (int j = 0; j < n; ++j)
            {
                tile.Shape[j][n - 1 - i] = r[i][j];
            }
        }
        if (tile.rotation >= 3) tile.rotation = 0;
        else tile.rotation++;
    }
    void RotateTileRand(ref Tile tile) 
    {
        int n = Random.Range(0, 4);
        for (int i = 0; i < n; i++) RotateTileCW(ref tile); 
    }

    List<List<NodeTest>> PrintBoard(List<List<Tile>> v, ref string str)
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
    
    
    void PrintTileInfo(Tile tile, ref string str)
    {
        for (int i = 0; i < tile.Shape.Count; i++)
        {
            for (int j = 0; j < tile.Shape[i].Count; j++)
                str += tile.Shape[i][j] + " ";
            str += "\n";
        }
        str += "tile.rotation : " + tile.rotation + "\n";
        str += "tile.type : " + tile.type + "\n";
    }
    
    void PrintNextBoard(Tile tile, List<List<Tile>> board, string location, ref string str)
    {
        Tile temp;
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
                board[num][0] = tile;
                tile = temp;
                break;
            case 'R':
                temp = board[num][0];
                for (int i = 0; i < board.Count-1; i++)
                {
                    board[num][i] = board[num][i+1];
                }
                board[num][board.Count-1] = tile;
                tile = temp;
                break;
            case 'B':
                temp = board[0][num];
                for (int i = 0; i < board.Count-1; i++)
                {
                    board[i][num] = board[i+1][num];
                }
                board[board.Count-1][num] = tile;
                tile = temp;
                break;
            case 'T':
                temp = board[board.Count-1][num];
                for (int i = board.Count-1; i >= 1; i--)
                {
                    board[i][num] = board[i-1][num];
                }
                board[0][num] = tile;
                tile = temp;
                break;
            default:
                break;
        }
        CopiedBoardList = PrintBoard(board, ref str);
        PrintTileInfo(tile, ref str);
    }
    
    void PushTile(ref Tile tile, List<List<Tile>> board, string location)
    {
        Tile temp;
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
                board[num][0] = tile;
                tile = temp;
                // locations[3 + num].Remove();
                break;
            case 'R':
                temp = board[num][0];
                for (int i = 0; i < board.Count - 1; i++)
                {
                    board[num][i] = board[num][i + 1];
                }
                board[num][board.Count - 1] = tile;
                tile = temp;
                break;
            case 'B':
                temp = board[0][num];
                for (int i = 0; i < board.Count - 1; i++)
                {
                    board[i][num] = board[i + 1][num];
                }
                board[board.Count - 1][num] = tile;
                tile = temp;
                break;
            case 'T':
                temp = board[board.Count - 1][num];
                for (int i = board.Count - 1; i >= 1; i--)
                {
                    board[i][num] = board[i - 1][num];
                }
                board[0][num] = tile;
                tile = temp;
                break;
            default:
                break;
        }
    }

    void SpawnPlayer(List<List<Tile>> board)
    {
        Tile tile = board[6][0];
        tile.isPlayer1 = true;
        tile.Shape[1][1] = 2;
        board[6][0] = tile;
    
        tile = board[0][6];
        tile.isPlayer2 = true;
        tile.Shape[1][1] = 4;
        board[0][6] = tile;
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
    
    void SpawnItem(List<List<Tile>> board)
    {
        for (int i = 0; i < 4; i++)
        {
            int r = Random.Range(1,6);
            int l = Random.Range(1,6);
            // 위치 중복안되게 배치
            while (board[r][l].IsPlayer1Item)
            {
                r = Random.Range(1,6);
                l = Random.Range(1,6);
            }
            Tile tile = board[r][l];
            tile.IsPlayer1Item = true;
            tile.Shape[1][1] = 3;
            board[r][l] = tile;
        }
        for (int i = 0; i < 4; i++)
        {
            int r = Random.Range(1,6);
            int l = Random.Range(1,6);
            // 아이템이 없을때까지 랜덤수 재출력
            while (board[r][l].IsPlayer1Item || board[r][l].IsPlayer2Item)
            {
                r = Random.Range(1,6);
                l = Random.Range(1,6);
            }
            Tile tile = board[r][l];
            tile.IsPlayer2Item = true;
            tile.Shape[1][1] = 5;
            board[r][l] = tile;
        }
    }
    
    
    
    void DFSListAdd(List<NodeTest> DFSList, List<List<NodeTest>> NodeList, NodeTest currentNode)
    {
        NodeTest NeighborNode;
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
