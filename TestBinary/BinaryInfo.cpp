#include <iostream>
#include <vector>
using namespace std;

// typedef vector<vector<bool>> Tile;

enum TileType {
    STRAIGHT,
    CORNER,
    HALFCROSS
};

struct Tile {
    vector<vector<bool>> shape;
    int rotation;
    TileType type;
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
    vector<vector<bool>> r = tile.shape;
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

int main() {

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
    Tile pushTile = {
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
    int n, m;
    inputSize(n,m);
    vector<vector<Tile>> board(n, vector<Tile>(m));
    generateBoard(Tilelist,board);
    printBoard(board);
    
    // 밀어넣을 타일 랜덤으로 정하기
    pushTile = Tilelist[r];
    printTileInfo(pushTile);

    return 0;
}
