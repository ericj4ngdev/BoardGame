#include <iostream>
#include <vector>
#include <cstdlib>
// #include "BinaryInfo.h"
using namespace std;

enum TileType 
{
    STRAIGHT,
    CORNER,
    HALFCROSS
};
struct Tile 
{
    vector<vector<int>> shape;
    int rotation;
    TileType type;
    bool isPlayer1;
    bool isPlayer2;
    bool isPlayer1Item;
    bool isPlayer2Item;
};

void inputSize(int& n, int& m)
{
    cout << "Enter the number of rows: ";
    cin >> n;
    cout << "Enter the number of columns: ";
    cin >> m;
}
void printTileInfo(Tile &tile) 
{
    for (int i = 0; i < tile.shape.size(); ++i) {
        for (int j = 0; j < tile.shape[i].size(); ++j)
            cout << tile.shape[i][j] << " ";
        cout << endl;
    }
    cout << "tile.rotation : " << tile.rotation << endl;
    cout << "tile.type : " << tile.type << endl;
    cout << endl;
}
void rotateTileCW(Tile &tile) 
{
    vector<vector<int>> r = tile.shape;
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
void printBoard(vector<vector<Tile>> &v) 
{
    int k = 0, l=0;
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
                    cout << v[k][l].shape[i][j] << " ";
                }
                cout << "\t";
            }
            cout << endl;
        }
        cout << endl;
    }
    cout << endl;
}
void printNextBoard(Tile tile, vector<vector<Tile>> board, const string& location)
{
    Tile temp;
    char ch = location[0];
    int num = (location[1] - '0')*2 + 1;    
    cout << num << endl;
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
void getNextBoard(Tile tile, vector<vector<Tile>> board, const string& location)
{
}
void generateBoard(vector<Tile>& list,vector<vector<Tile>>& v)
{
    // 7*7 자동생성
    for (int i = 0; i < v.size(); i++)
    {
        for (int j = 0; j < v[i].size(); ++j)
        {
            v[i][j] = list[rand()%3]; 
            rotateTileRand(v[i][j]);            
        }
    }
}
void pushTile(Tile& tile, vector<vector<Tile>>& board, const string& location) 
{
    Tile temp;
    char ch = location[0];
    int num = (location[1] - '0')*2 + 1;    
    cout << num << endl;
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
void spawnPlayer(vector<vector<Tile>>& board)
{
    board[6][0].isPlayer1 = true;
    board[6][0].shape[1][1] = 2;
    board[0][6].isPlayer2 = true;
    board[0][6].shape[1][1] = 4;
}

void spawnItem(vector<vector<Tile>>& board)
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
    vector<Tile> Tilelist = {straight, corner, halfcross};
    srand(static_cast<unsigned>(time(0)));

    int r = rand()%3;
    int n = 7, m = 7;
    int rotate = 0;
    string location;
    
    // 맵 생성
    // inputSize(n,m);
    vector<vector<Tile>> board(n, vector<Tile>(m));
    generateBoard(Tilelist,board);
    spawnPlayer(board);
    spawnItem(board);
    printBoard(board);

    // 밀어넣을 타일 랜덤으로 정하기
    pTile = Tilelist[r];
    printTileInfo(pTile);

    while (cin)
    {
        cout << "Choose Rotate : ";
        cin >> rotate;   
        for (int i = 0; i < rotate; i++) rotateTileCW(pTile);
        
        cout << "Choose Loaction : ";
        cin >> location;
        printNextBoard(pTile, board, location);
        cout << "Choose Loaction again: ";
        cin >> location;
        // system("cls");     
        pushTile(pTile, board, location);
        printBoard(board);
        printTileInfo(pTile);
    }
    
    
    return 0;
}
