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
    List<NodeTest> DFSList = new List<NodeTest>();
    List<List<NodeTest>> boardList = new List<List<NodeTest>>();
    

    private void Start()
    {
        FileStream test = new FileStream("Assets/Resources/test.txt", FileMode.Create);
        StreamWriter testStreamWriter = new StreamWriter(test);
        string str = "";
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
        
        Tile pTile = new Tile {
            Shape = new List<List<int>> {
                new List<int> {0, 0, 0},
                new List<int> {1, 1, 1},
                new List<int> {0, 0, 0}
            },
            rotation = 0
        };

        List<Tile> TileList = new List<Tile> {
            straight,
            corner,
            halfcross
        };

        // int r = Random.Range(0, 3);
        int n = 7;
        int m = 7;
        int rotate = 0;
        string location = "L0";

        PrintTileInfo(corner, ref str);
        PrintTileInfo(halfcross, ref str);
        
        List<List<Tile>> board = new List<List<Tile>>();
        for (int i = 0; i < n; i++)
        {
            List<Tile> row = new List<Tile>();
            for (int j = 0; j < m; j++)
            {
                row.Add(new Tile());
            }
            board.Add(row);
        }
        GenerateBoard(TileList,board, ref str);
        
        //SpawnPlayer(board);
        //SpawnItem(board);
        boardList = PrintBoard(board, ref str);
        // printList2(boardList, ref str);
        testStreamWriter.Write(str);
        
        testStreamWriter.Close();
        
        /*NodeTest player1 = GetPlayer1Pos(boardList);
        NodeTest player2 = GetPlayer2Pos(boardList);
        testStreamWriter.Write(player2.y);
        testStreamWriter.Write(player2.x);
        
        // 밀어넣을 타일 랜덤으로 정하기
        pTile = TileList[r];
        printTileInfo(pTile);
    
        // DFS(player2, DFSList);
        DFSListAdd(player2);
        checkReachableItem();

        DFSListAdd(player2);
        checkReachableItem();*/
    }
    
    void GenerateBoard(List<Tile> list, List<List<Tile>> board, ref string str)
    {
        // 7*7 자동생성
        for (int i = 0; i < board.Count; i++)
        {
            for (int j = 0; j < board[i].Count; ++j)
            {
                board[i][j] = list[Random.Range(0,3)]; 
                // RotateTileRand(board[i][j]);
                str += board[i][j].type + "\n";
                PrintTileInfo(board[i][j], ref str);
                // str += board[i][j].rotation + "\t";
            }
            str += "\n";
        }
        str += "\n";
    }
    
    List<List<NodeTest>> PrintBoard(List<List<Tile>> v, ref string str)
    {
        int k = 0, l = 0;
        List<List<NodeTest>> boardList2 = new List<List<NodeTest>>();
        List<NodeTest> row = new List<NodeTest>();
        NodeTest node = new NodeTest();
        // l에 대한 for문을 나중에 돌려서 리스트 크기를 따로 저장
        int x = v[k][l].Shape.Count;
        for (k = 0; k < v.Count; ++k)
        {
            for (int i = 0; i < x; ++i)
            {
                for (l = 0; l < v[k].Count; ++l)
                {
                    str += v[k][l].type + " ";
                    // 가로 모두 출력
                    for (int j = 0; j < v[k][l].Shape[i].Count; ++j)
                    {
                        node.x = 3 * l + j;
                        node.y = 3 * k + i;
                        node.num = v[k][l].Shape[i][j];
                        row.Add(node);
                        str += node.num + " ";
                    }
                    str += "\t"; // str에 "\t" 추가
                }
                boardList2.Add(row);
                row.Clear();
                str += "\n"; // str에 "\n" 추가
            }
            str += "\n"; // str에 "\n" 추가
        }
        str += "\n"; // str에 "\n" 추가
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
    
    void inputSize(int n, int m)
    {
        Console.Write("Enter the number of rows: ");
        n = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter the number of columns: ");
        m = Convert.ToInt32(Console.ReadLine());
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
    void RotateTileCW(Tile tile)
    {
        List<List<int>> r = tile.Shape;
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
    void RotateTileRand(Tile tile) 
    {
        for (int i = 0; i < Random.Range(0,3); i++) RotateTileCW(tile);    
    }
    
    
    
    void printNextBoard(Tile tile, List<List<Tile>> board, string location)
    {
        Tile temp;
        char ch = location[0];
        int num = (location[1] - '0') * 2 + 1;
        Console.WriteLine(num);
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
        // PrintBoard(board,);
    }
    


    void PushTile(Tile tile, List<List<Tile>> board, string location)
    {
        Tile temp;
        char ch = location[0];
        int num = (location[1] - '0') * 2 + 1;
        Console.WriteLine(num);
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


    
    void DFSListAdd(NodeTest currentNode)
    {
        int cnt = 0;
        NodeTest NeighborNode;
        while (cnt != 9)
        {
            int i = cnt % 3 - 1;
            int j = (int)(cnt / 3) - 1;
            // Skip the currentNode itself
            if (i == 0 && j == 0)
            {
                cnt++;
                continue;
            }
            int nexty = currentNode.y + i;
            int nextx = currentNode.x + j;

            // Check if nexty and nextx are within the bounds of the boardList
            if (nexty >= 0 && nexty < boardList.Count && nextx >= 0 && nextx < boardList[0].Count)
            {
                if (boardList[nexty][nextx].num >= 1 && !boardList[nexty][nextx].isVisited)
                {
                    boardList[nexty][nextx].isVisited = true;
                    NeighborNode = boardList[nexty][nextx];

                    DFSList.Add(NeighborNode);
                    DFSListAdd(NeighborNode);
                }
            }
            cnt++;
        }
    }
    
    void checkReachableItem()
    {
        int count = 0;
        Console.WriteLine("DFSList");
        for (int i = 0; i < DFSList.Count; i++)
        {
            if(DFSList[i].num == 5) count++;
            Console.Write(DFSList[i].num + " ");
        }
    
        Console.WriteLine("\nplayer2 reachable item : " + count);
        DFSList.Clear();
    }


}
