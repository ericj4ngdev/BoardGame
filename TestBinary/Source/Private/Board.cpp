#include "Board.h"


void Tile::Shape()
{
    Height = Nodes.size();
    Width = Nodes[0].size();
}

void Tile::Shape(int &Y, int &X)
{
    Height = Nodes.size();
    Width = Nodes[0].size();
    Y = Height;
    X = Width;
}

void Tile::rotate()
{
    vector<vector<Node>> r = Nodes;
    for (int i = 0; i < Height; ++i) 
    {
        for (int j = 0; j < Width; ++j)
        {
            Nodes[j][Height-1-i].num = r[i][j].num;
        }
    }
    if (rotation >= 3) rotation = 0;
    else rotation++;
}

void Tile::printInfo() 
{
    for (int i = 0; i < Height; ++i) 
    {
        for (int j = 0; j < Width; ++j)
        {
            cout << Nodes[i][j].num << " ";
        }
        cout << endl;
    }
    cout << "rotation : " << rotation << endl;
    cout << "type : " << type << endl;
    cout << endl;
}
