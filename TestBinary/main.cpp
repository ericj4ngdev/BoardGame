
#include <iostream>
#include <vector>

#include "BinaryInfo.h"

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
    vector<vector<Node>> boardList;
    generateBoard(Tilelist,board);
    spawnPlayer(board);
    spawnItem(board);
    // 이차 배열로 받음
    boardList = printBoard(board);
    printVector2(boardList);
    cout << endl;
    Node player1 = getPlayer1Pos(boardList);
    Node player2 = getPlayer2Pos(boardList);
    cout << player2.y << " " << player2.x << endl;


    // 밀어넣을 타일 랜덤으로 정하기
    pTile = Tilelist[r];
    printTileInfo(pTile);
    
    // DFS(player2, DFSList);
    DFSListAdd(player2);

    cout << "DFSList" << endl;
    for (int i = 0; i < DFSList.size(); i++)
    {
        cout << DFSList[i].num << " ";
    }

    // while (true)
    // {
    //     cout << "Choose Rotate : ";
    //     cin >> rotate;   
    //     for (int i = 0; i < rotate; i++) rotateTileCW(pTile);
        
    //     // cout << "To Know Next Board by Loaction : ";
    //     // cin >> location;
    //     // printNextBoard(pTile, board, location);
    //     cout << "Choose Loaction again: ";
    //     cin >> location;
    //     // system("cls");     
    //     pushTile(pTile, board, location);
    //     boardList = printBoard(board);
    //     for (int i = 0; i < boardList.size(); ++i) 
    //     {
    //         for (int j = 0; j < boardList[i].size(); ++j)
    //             cout << boardList[i][j] << " ";
    //         cout << endl;
    //     }
    //     printTileInfo(pTile);
    //     // movePlayer(player1, board);
        
    // }
    
    
    return 0;
}
