#include <iostream>
#include <string>
#include <vector>
#include <cstdlib>
#include <conio.h>
// #include "BinaryInfo.h"
// using namespace std;

enum TileType 
{
    STRAIGHT,
    CORNER,
    HALFCROSS
};
struct Tile 
{
    std::vector<std::vector<int>> shape;
    int rotation;
    TileType type;
    int x; int y;
    bool isvisited = false;
    bool isPlayer1;
    bool isPlayer2;
    bool isPlayer1Item;
    bool isPlayer2Item;
};
struct Node
{
    int num;
    bool isVisited = false;
    // 좌표
    int x;
    int y;
};

// 

struct Player
{
    int num;
    int x;
    int y;
    int target;
    int targetCount;

};

// void DFS(Node& player, std::vector<Node>& dfsList)
// {
//     DFSListAdd(player, dfsList);
//     // for (int i = 0; i < dfsList.size(); i++)
//     // {
//     //     std::cout << dfsList[i].num << " ";
//     // }
// }


// 배열화, 메모리 안정화.
// C#화 해보고 느리면 롤백

std::vector<Node> DFSList;
std::vector<std::vector<Node>> boardList;


void printstdVector2(std::vector<std::vector<Node>>& v){
    for (int i = 0; i < v.size(); ++i) 
    {
        for (int j = 0; j < v[i].size(); ++j)
        {
            switch (v[i][j].num)
            {
            case 0 : 
                std::cout << "#" << " ";
                break;

            case 1 : 
                std::cout << " " << " ";
                break;

            case 2 : 
                std::cout << "A" << " ";
                break;

            case 3 : 
                std::cout << "a" << " ";
                break;

            case 4 : 
                std::cout << "B" << " ";
                break;

            case 5 : 
                std::cout << "b" << " ";
                break;

            default:
                break;
            }            
        }
        std::cout << std::endl;
    }
}
void inputSize(int& n, int& m)
{
    std::cout << "Enter the number of rows: ";
    std::cin >> n;
    std::cout << "Enter the number of columns: ";
    std::cin >> m;
}
void printTileInfo(Tile &tile) 
{
    for (int i = 0; i < tile.shape.size(); ++i) {
        for (int j = 0; j < tile.shape[i].size(); ++j)
            std::cout << tile.shape[i][j] << " ";
        std::cout << std::endl;
    }
    std::cout << "tile.rotation : " << tile.rotation << std::endl;
    std::cout << "tile.type : " << tile.type << std::endl;
    std::cout << std::endl;
}
void rotateTileCW(Tile &tile) 
{
    std::vector<std::vector<int>> r = tile.shape;
    int n = tile.shape.size();
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < n; ++j) {
            tile.shape[j][n-1-i] = r[i][j];
        }
    }
    if (tile.rotation >= 3) tile.rotation = 0;
    else tile.rotation++;
}
void rotateTileRand(Tile &tile) 
{
    for (int i = 0; i < rand()%4; i++) rotateTileCW(tile);    
}
std::vector<std::vector<Node>> printBoard(std::vector<std::vector<Tile>> &v) 
{
    int k = 0, l = 0;
    std::vector<std::vector<Node>> boardList2;
    std::vector<Node> row;
    Node node;
    // l에 대한 for문을 나중에 돌려서 벡터 크기를 따로 저장
    int x = v[k][l].shape.size();
    for (k = 0; k < v.size(); ++k) 
    {
        for (int i = 0; i < x; ++i)
        {   
            for (l = 0; l < v[k].size(); ++l)
            {                
                // 가로 모두 출력
                for (int j = 0; j < v[k][l].shape[i].size(); ++j)
                {
                    std::cout << v[k][l].shape[i][j] << " ";
                    node.num = v[k][l].shape[i][j];
                    node.x = 3 * l + j;
                    node.y = 3 * k + i;
                    row.push_back(node);
                }
                std::cout << "\t";
            }
            boardList2.push_back(row);
            row.clear();
            std::cout << std::endl;
        }
        std::cout << std::endl;
    }
    std::cout << std::endl;
    return boardList2;
}
void printNextBoard(Tile tile, std::vector<std::vector<Tile>> board, const std::string& location)
{
    Tile temp;
    char ch = location[0];
    int num = (location[1] - '0')*2 + 1;    
    std::cout << num << std::endl;
    // 문자 '0'에서 해당 문자의 아스키 코드값을 빼면 숫자 값을 얻을 수 있습니다.
    switch (ch)
    {
        case 'L':
            temp = board[num][board.size()-1];
            for (int i = board.size()-1; i >= 1; i--) 
            {
                board[num][i] = board[num][i-1];
            }
            board[num][0] = tile;
            tile = temp;
            break;
        case 'R':
            temp = board[num][0];
            for (int i = 0; i < board.size()-1; i++) 
            {
                board[num][i] = board[num][i+1];
            }
            board[num][board.size()-1] = tile;
            tile = temp;
            break;
        case 'B':
            temp = board[0][num];
            for (int i = 0; i < board.size()-1; i++) 
            {
                board[i][num] = board[i+1][num];
            }
            board[board.size()-1][num] = tile;
            tile = temp;
            break;
        case 'T':
            temp = board[board.size()-1][num];
            for (int i = board.size()-1; i >= 1; i--) 
            {
                board[i][num] = board[i-1][num];
            }
            board[0][num] = tile;
            tile = temp;
            break;
        default:
            break;
    }
    printBoard(board);
}

void generateBoard(std::vector<Tile>& list,std::vector<std::vector<Tile>>& board)
{
    // 7*7 자동생성
    for (int i = 0; i < board.size(); i++)
    {
        for (int j = 0; j < board[i].size(); ++j)
        {
            board[i][j] = list[rand()%3]; 
            rotateTileRand(board[i][j]);            
        }
    }
}
void pushTile(Tile& tile, std::vector<std::vector<Tile>>& board, const std::string& location) 
{
    Tile temp;
    char ch = location[0];
    int num = (location[1] - '0')*2 + 1;    
    std::cout << num << std::endl;
    // 문자 '0'에서 해당 문자의 아스키 코드값을 빼면 숫자 값을 얻을 수 있습니다.
    switch (ch)
    {
        case 'L':
            temp = board[num][board.size()-1];
            for (int i = board.size()-1; i >= 1; i--) 
            {
                board[num][i] = board[num][i-1];
            }
            board[num][0] = tile;
            tile = temp;
            break;
        case 'R':
            temp = board[num][0];
            for (int i = 0; i < board.size()-1; i++) 
            {
                board[num][i] = board[num][i+1];
            }
            board[num][board.size()-1] = tile;
            tile = temp;
            break;
        case 'B':
            temp = board[0][num];
            for (int i = 0; i < board.size()-1; i++) 
            {
                board[i][num] = board[i+1][num];
            }
            board[board.size()-1][num] = tile;
            tile = temp;
            break;
        case 'T':
            temp = board[board.size()-1][num];
            for (int i = board.size()-1; i >= 1; i--) 
            {
                board[i][num] = board[i-1][num];
            }
            board[0][num] = tile;
            tile = temp;
            break;
        default:
            break;
    }
}

void spawnPlayer(std::vector<std::vector<Tile>>& board)
{
    board[6][0].isPlayer1 = true;
    board[6][0].shape[1][1] = 2;
    board[0][6].isPlayer2 = true;
    board[0][6].shape[1][1] = 4;
}

Node getPlayer1Pos(std::vector<std::vector<Node>>& nodeList)
{
    Node player1;
    for (int i = 0; i < nodeList.size(); i++)
    {
        for (int j = 0; j < nodeList[i].size(); j++)
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
    return Node();
}

Node getPlayer2Pos(std::vector<std::vector<Node>>& nodeList)
{
    Node player2;
    for (int i = 0; i < nodeList.size(); i++)
    {
        for (int j = 0; j < nodeList[i].size(); j++)
        {
            if(nodeList[i][j].num == 4) {
                player2.num = 4;
                player2.x = j;
                player2.y = i;
                return player2;
            }
        }        
    }
    return Node();
}


void spawnItem(std::vector<std::vector<Tile>>& board)
{
    for (int i = 0; i < 4; i++)
    {
        int r = rand()%5 + 1;
        int l = rand()%5 + 1;
        // 위치 중복안되게 배치
        while(board[r][l].isPlayer1Item)
        {
            r = rand()%5 + 1;
            l = rand()%5 + 1;           
        }
        board[r][l].isPlayer1Item = true;
        board[r][l].shape[1][1] = 3;
    }
    for (int i = 0; i < 4; i++)
    {
        int r = rand()%5 + 1;
        int l = rand()%5 + 1;
        // 아이템이 없을때까지 랜덤수 재출력
        while(board[r][l].isPlayer1Item || board[r][l].isPlayer2Item)
        {
            r = rand()%5 + 1;
            l = rand()%5 + 1;
        }
        board[r][l].isPlayer2Item = true;
        board[r][l].shape[1][1] = 5;
    }
}

void DFSListAdd(Node currentNode)
{
    // printstd::Vector2(boardList);
    int cnt = 0;
    Node NeighborNode;
    while (cnt != 9)
    {
        int i = cnt % 3 - 1;
        int j = static_cast<int>(cnt/3) - 1;
        // Skip the currentNode itself
        if (i == 0 && j == 0) 
        {
            cnt++;
            continue;
        }
        int nexty = currentNode.y + i;
        int nextx = currentNode.x + j;
        
        // Check if nexty and nextx are within the bounds of the boardList
        if (nexty >= 0 && nexty < boardList.size() && nextx >= 0 && nextx < boardList[0].size())
        {   
            if (boardList[nexty][nextx].num >= 1 && !boardList[nexty][nextx].isVisited)
            {
                boardList[nexty][nextx].isVisited = true;
                NeighborNode = boardList[nexty][nextx];
                
                DFSList.push_back(NeighborNode);
                DFSListAdd(NeighborNode);
            }
        }
        cnt++;
    }
}

// 플레이어 정보,  DFSList를 받아서 
// 해당 플레이어가 도달할 수 있는 아이템의 개수를 출력한다. 
void checkReachableItem()
{
    int count = 0;
    std::cout << "DFSList" << std::endl;
    for (int i = 0; i < DFSList.size(); i++)
    {
        if(DFSList[i].num == 5) count++;
        std::cout << DFSList[i].num << " ";
    }
    
    std::cout << std::endl << "player2 reachable item : " << count << std::endl;
    DFSList.clear();
}

int main()
{
    Tile straight = {
        {
            {false, false, false},
            {true, true, true},
            {false, false, false}
        },
        0,
        STRAIGHT
    };

    Tile corner = {
        {
            {false, false, false},
            {false, true, true},
            {false, true, false}
        },
        0,
        CORNER
    };

    Tile halfcross = {
        {
            {false, true, false},
            {true, true, false},
            {false, true, false}
        },
        0,
        HALFCROSS
    };

    Tile pTile = {
        {
            {false, false, false},
            {false, false, false},
            {false, false, false}
        },
        0,
    };
    
    std::vector<Tile> Tilelist = {straight, corner, halfcross};
    srand(static_cast<unsigned>(time(0)));

    int r = rand()%3;
    int n = 7, m = 7;
    int rotate = 0;

    std::string location;
    
    // 맵 생성
    // inputSize(n,m);
    std::vector<std::vector<Tile>> board(n, std::vector<Tile>(m));
    // std::vector<std::vector<Node>> boardList;
    generateBoard(Tilelist,board);
    spawnPlayer(board);
    spawnItem(board);
    // 이차 배열로 받음
    boardList = printBoard(board);
    printstdVector2(boardList);
    std::cout << std::endl;
    Node player1 = getPlayer1Pos(boardList);
    Node player2 = getPlayer2Pos(boardList);
    std::cout << player2.y << " " << player2.x << std::endl;


    // 밀어넣을 타일 랜덤으로 정하기
    pTile = Tilelist[r];
    printTileInfo(pTile);
    
    // DFS(player2, DFSList);
    DFSListAdd(player2);
    checkReachableItem();

    while (true)
    {
        std::cout << "Choose Rotate : ";
        std::cin >> rotate;   
        for (int i = 0; i < rotate; i++) rotateTileCW(pTile);
        char x = 'y';
        // while(x == 'x')
        // {            
        //     std::cout << "To Know Next Board by Loaction : ";
        //     std::cin >> location;
        //     printNextBoard(pTile, board, location);
        //     std::cout << "To Decide, input x" << std::endl;
        //     std::cin >> x;
        // }
        std::cout << "Choose Loaction again: ";
        std::cin >> location;
        // system("cls");     
        pushTile(pTile, board, location);
        boardList = printBoard(board);
        printstdVector2(boardList);
        printTileInfo(pTile);
        DFSListAdd(player2);
        checkReachableItem();
        // movePlayer(player1, board);
    }
    
    
    return 0;
}