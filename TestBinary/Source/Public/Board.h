// #pragma once

// #include <iostream>
// #include <vector>
// #include <cstdlib>
// #include <conio.h>

// using std::vector, std::cout, std::endl, std::cin, std::string;

// enum TileType 
// {
//     STRAIGHT,
//     CORNER,
//     HALFCROSS
// };


// struct Node
// {
//     Node()
//     {
//         num = 0;
//         isVisited = false;
//     };

//     int num;
//     bool isVisited;
//     // 좌표
//     int x;
//     int y;
// };


// class Tile 
// {
//     Tile()
//     {
//         Shape();
//     }
// public:
    
//     int rotation;
//     int Height, Width;
//     TileType type;
// private:
//     vector<vector<Node>> Nodes;
//     int x, y;
//     bool isvisited = false;

//     /***/
//     void Shape();
//     void Shape(int &Y, int &X);
    
//     /***/
//     void printInfo();

//     /** Rotate tile in CW */
//     void rotate();
// };


// class Board
// {
//     vector<vector<Tile>> Tiles;   
// };
