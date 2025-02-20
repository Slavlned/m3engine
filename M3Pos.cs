// позиция на доске
using System;

[System.Serializable]
public class M3Pos
{
    public int X;
    public int Y;

    public M3Pos(int x, int y)
    {
        X = x;
        Y = y;
    }

    public M3Pos Add(int x, int y)
    {
        return new M3Pos(X+x, Y+y);
    }
} 