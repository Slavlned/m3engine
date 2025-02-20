using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class M3Level
{
    // m3 уровень

    public int level;
    public List<M3TileInfo> tiles;

    // m3 оффсеты
    public Vector2 cellOffset;
    public Vector2 boardOffset;

    // m3 размер
    public int width, height;

    // m3 ходы
    public int moves;

    // m3 голы
    public M3Recipe recipe;
}