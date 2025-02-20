using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class M3Match
{
    // источник мэтча
    public M3Tile source;
    // тайл
    public List<M3Tile> tiles;
    // текущий
    private M3ObjectType _out;
    // инициализирован ли игроком
    private bool _initiatedByPlayer;

    // конструктор
    public M3Match(M3Tile source, List<M3Tile> tiles, bool initiatedByPlayer)
    {
        this.source = source;
        this.tiles = tiles;
        this._initiatedByPlayer = initiatedByPlayer;
    }

    public M3Match(M3Tile source, List<M3Tile> tiles, M3ObjectType _out, bool initiatedByPlayer)
    {
        this.source = source;
        this.tiles = tiles;
        this._out = _out;
        this._initiatedByPlayer = initiatedByPlayer;
    }

    // как строка
    public string AsString()
    {
        string str = "(";

        foreach (M3Tile tile in this.tiles)
        {
            if (tile.IsEmpty()) { continue;  }
            str += "(" + tile.Pos().X + ", " + tile.Pos().Y + ", " + tile.GetObject().GetM3Type() + ")";
        }

        if (!source.IsEmpty())
        {
            str += ", (source: " + source.Pos().X + ", " + source.Pos().Y + ", " + source.GetObject().GetM3Type() + ")";
        }

        return str;
    }

    // подтверждения матча
    public void Confirm()
    {
        // лог
        // Debug.Log("Confirming match with tiles amount: " + (tiles.Count + 1).ToString());

        // если нет объекта который нужно создать после комбинации
        if (!M3Register.boosters.Contains(_out))
        {
            // удаляем
            foreach (M3Tile tile in tiles)
            {
                tile.GetBoard().GetResult().UpdateGoal(tile.GetObject().GetM3Type(), 1);
                tile.GetBoard().DestroyObject(tile.GetObject(), M3DestroyType.CALL);
            }
            
            source.GetBoard().GetResult().UpdateGoal(source.GetObject().GetM3Type(), 1);
            source.GetBoard().DestroyObject(source.GetObject(), M3DestroyType.CALL);
        } else
        {
            // мерджим
            source.GetBoard().Merge(this);
        }
    }

    // экспандирование матча
    public M3Match Expand()
    {
        // список
        List<M3Match> matches = new List<M3Match>();

        // перебеираем клетки
        foreach (M3Tile tile in tiles)
        {
            if (!tile.IsEmpty())
            {
                matches.Add(tile.GetObject().FindMatch(_initiatedByPlayer));
            }
        }

        // источник
        if (!source.IsEmpty()) {
            matches.Add(source.GetObject().FindMatch(_initiatedByPlayer));
        }

        // возвращаем
        M3Match m = this;
        foreach (M3Match match in matches) {
            if (match == null) continue;
            if (match.tiles.Count + 1 > m.tiles.Count + 1)
            {
                if (match.Ready())
                {
                    m = match;
                }
            }
        }

        return m;
    }

    // сравнение матчей
    public bool SimilarByTiles(M3Match match)
    {
        // проверяем тайлы
        foreach (M3Tile tile in tiles)
        {
            foreach(M3Tile _tile in match.tiles)
            {
                if (tile == _tile || tile == match.source || _tile == source)
                {
                    return true;
                }
            }
        }
        // проверяем тайлы наоборот
        foreach (M3Tile tile in match.tiles)
        {
            foreach (M3Tile _tile in tiles)
            {
                if (tile == _tile || tile == source || _tile == match.source)
                {
                    return true;
                }
            }
        }
        // проверяем сорс
        if (match.source == source) { return true; }

        // сорс нам не важен
        return false;
    }

    // расширяем, (находим мэтчи вокруг)
    public List<M3Match> Extend()
    {
        // экстенд мэтчи
        List<M3Match> extended = new List<M3Match>();

        // тайлы
        foreach (M3Tile tile in tiles)
        {
            if (!tile.IsEmpty())
            {
                extended.Add(tile.GetObject().FindMatch(_initiatedByPlayer));
            }
        }

        // источник
        if (!source.IsEmpty())
        {
            extended.Add(source.GetObject().FindMatch(_initiatedByPlayer));
        }

        // возвращаем
        return extended;
    }

    // готов ли к очистке клеточек
    public bool Ready()
    {
        // Debug.Log(AsString());

        // проверяем тайлы на доступность
        foreach (M3Tile tile in tiles)
        {
            if (tile.GetObject() == null) { return false; }
            if (tile.GetObject().Moving) { return false; }
            if (!tile.GetObject().CanInteract()) { return false; }
            if (tile.GetObject().InFallFlow()) { return false; }
            // if (tile.GetObject().CanFall()) { return false; }
            // Debug.Log("+");
        }

        // проверяем соурс
        if (source.GetObject() == null) { return false; }
        if (source.GetObject().Moving) { return false; }
        if (!source.GetObject().CanInteract()) { return false; }
        if (source.GetObject().InFallFlow()) { return false; }
        // if (source.GetObject().CanFall()) { return false; }

        // готов
        return true;
    }

    // возможен ли этот мэтч
    public bool IsCorrupted()
    {
        // проверяем соурс
        if (source.GetObject() == null) { return true; }
        if (source.GetObject().Moving) { return true; }

        // проверяем тайлы на доступность
        foreach (M3Tile tile in tiles)
        {
            if (tile.GetObject() == null) { return true; }
            if (tile.GetObject().Moving) { return true; }
            if (tile.GetObject().GetM3Type() != source.GetObject().GetM3Type()) { return true; }
        }

        // готов
        return false;
    }

    // объект который нужно создать
    public M3ObjectType GetOut()
    {
        return _out;
    }

    // инициирован ли игроком
    public bool InitiatedByPlayer()
    {
        return _initiatedByPlayer;
    }
}