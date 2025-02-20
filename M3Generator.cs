using System.Numerics;
using UnityEngine;

public class M3Generator
{
    // доска
    private M3Board board;
    // тайл
    private M3Tile tile;

    // установка
    public M3Generator(M3Board _board, M3Tile _tile)
    {
        this.board = _board;
        this.tile = _tile;
    }

    // тик
    public void Tick()
    {
        // если тайл не пуст пропускаем тик
        if (tile.GetObject() != null) { return; }

        // в ином случае заполняем рандомной клеточкой
        tile.FillRandom();
    }
}