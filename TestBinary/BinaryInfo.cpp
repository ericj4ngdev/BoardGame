#include <iostream>
#include <vector>
using namespace std;

void print(vector<vector<bool>> v) {
    for (int i = 0; i < v.size(); ++i) {
        for (int j = 0; j < v[i].size(); ++j)
            cout << v[i][j] << " ";
        cout << endl;
    }
    cout << endl;
}

void printBoard(vector<vector<vector<vector<bool>>>> v) {
    int k = 0, l=0;
    // l에 대한 for문을 나중에 돌려서 벡터 크기를 따로 저장
    int x = v[k][l].size();
    for (k = 0; k < v.size(); ++k) 
    {
        for (int i = 0; i < x; ++i)
        {   
            for (l = 0; l < v[k].size(); ++l)   // 2까지 증가
            {                
                // 가로 모두 출력
                for (int j = 0; j < v[k][l][i].size(); ++j)
                {
                    cout << v[k][l][i][j] << " ";
                }
                cout << "\t";
            }
            cout << endl;
        }
        cout << endl;
    }
    cout << endl;
}


typedef vector<vector<bool>> Tile;
int main() {
    // Straight
    vector<vector<bool>> straight = {
        {false, false, false},
        {true, true, true},
        {false, false, false}
    };
    // Corner
    vector<vector<bool>> corner = {
        {false, false, false},
        {false, true, true},
        {false, true, false}
    };
    // Halfcross
    vector<vector<bool>> halfcross = {
        {false, true, false},
        {true, true, false},
        {false, true, false}
    };
    
    // Board
    vector<vector<Tile>> board = {
        {straight, corner},
        {halfcross, corner}
    };
    
    // Print Tile
    cout << "straight" << endl;
    print(straight);
    cout << endl;
    cout << "corner" << endl;
    print(corner);
    cout << endl;
    cout << "halfcross" << endl;
    print(halfcross);
    cout << endl;

    printBoard(board);
    // Print board

    cout << endl;
    
    return 0;
}
