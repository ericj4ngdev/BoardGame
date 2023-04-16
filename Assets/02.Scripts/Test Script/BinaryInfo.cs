using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BinaryInfo : MonoBehaviour
{
    private bool[,] straight = new bool[3, 3];
    private bool[,] corner = new bool[3, 3];
    private bool[,] halfcross = new bool[3, 3];
    private bool[,][,] board = new bool[3, 3][,];
    private void Awake()
    {
        straight = new bool[3,3] {
            { false, false, false },
            { true, true, true },
            { false, false, false }
        };

        corner = new bool[3,3] {
            { false, false, false },
            { false, true, true },
            { false, true, false }
        };

        halfcross = new bool[3,3] {
            { false, true, false },
            { true, true, false },
            { false, true, false }
        };

        board = new bool[3,3][,] {
            {straight, corner, halfcross},
            {straight, corner, halfcross},
            {straight, corner, halfcross}
        };
    }

    private void Start()
    {
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                Debug.Log("Tile " + i + "," + j);
                for (int k = 0; k < board[i, j].GetLength(0); k++) {
                    string row = "";
                    for (int l = 0; l < board[i, j].GetLength(1); l++) {
                        row += board[i, j][k, l] ? "1" : "0";
                    }
                    Debug.Log(row);
                }
            }
        }
    }
    
    // 타일을 밀어넣는 함수
    void PushTile(bool[,] tile, int index, bool[,][,] board)
    {
        int row = board.GetLength(0);
        int col = board.GetLength(1);

        switch (index)
        {
            case 0:         // L0
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        board[i, 0][j, 0] = board[i, 0][j, 1];
                        board[i, 0][j, 1] = board[i, 0][j, 2];
                        board[i, 0][j, 2] = tile[j, i];
                    }
                }
                break;
            
            case 1:         // L1:
                for (int i = 0; i < row; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        board[i, 0][j, 0] = board[i, 0][j, 1];
                        board[i, 0][j, 1] = board[i, 0][j, 2];
                        board[i, 0][j, 2] = tile[j - 1, i];
                    }
                }
                break;
            
            case 2:         // R0:
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        board[i, col - 1][j, 2] = board[i, col - 1][j, 1];
                        board[i, col - 1][j, 1] = board[i, col - 1][j, 0];
                        board[i, col - 1][j, 0] = tile[2 - j, i];
                    }
                }
                break;
            
            case 3:         //R1:
                for (int i = 0; i < row; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        board[i, col - 1][j, 2] = board[i, col - 1][j, 1];
                        board[i, col - 1][j, 1] = board[i, col - 1][j, 0];
                        board[i, col - 1][j, 0] = tile[2 - j + 1, i];
                    }
                }
                break;
            
            case 4:         //R1:
                for (int i = 0; i < row; i++)
                {
                    for (int j = 1; j < 3; j++)
                    {
                        board[i, col - 1][j, 2] = board[i, col - 1][j, 1];
                        board[i, col - 1][j, 1] = board[i, col - 1][j, 0];
                        board[i, col - 1][j, 0] = tile[2 - j + 1, i];
                    }
                }
                break;
            
        }
    }
}
