using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class M3SimObject
{
    // объект симуляции
    public int X;
    public int Y;
    public M3ObjectType type;
    public bool swappable;

    public M3SimObject(int x, int y, M3ObjectType type, bool swappable)
    {
        this.X = x;
        this.Y = y;
        this.type = type;
        this.swappable = swappable;
    }
}

public static class M3Predications
{
    // класс предиктов по m3

    // public M3Hint GetHint();
    public static bool IsMakesM3(M3Board board, M3Swap swap)
    {
        // если что-то нулл выходим
        if (swap.from == null || swap.to == null)
        {
            return false; 
        }
        // копируем данные из доски
        List<M3SimObject> simObjects = new List<M3SimObject>();
        // перебераем тайлы
        foreach (M3Tile obj in board.GetTiles())
        {
            if (!obj.IsEmpty()) {
                if (!obj.GetObject().GetSettings().IsSwapEnabled) { continue; }
                simObjects.Add(new M3SimObject(obj.Pos().X, obj.Pos().Y, obj.GetObject().GetM3Type(), true));
            }
        }
        // возвращаем
        bool match = HasMatch(simObjects, swap.from.GetTile().Pos().X, swap.from.GetTile().Pos().Y, swap.to.GetTile().Pos().X, swap.to.GetTile().Pos().Y, swap.from.GetM3Type());
        // если нет матча ищем его относительно 2 клетки
        if (!match)
        {
            // список
            simObjects = new List<M3SimObject>();
            // перебераем тайлы
            foreach (M3Tile obj in board.GetTiles())
            {
                if (!obj.IsEmpty())
                {
                    if (!obj.GetObject().GetSettings().IsSwapEnabled) { continue; }
                    simObjects.Add(new M3SimObject(obj.Pos().X, obj.Pos().Y, obj.GetObject().GetM3Type(), true));
                }
            }
            // возвращаем
            match = HasMatch(simObjects, swap.to.GetTile().Pos().X, swap.to.GetTile().Pos().Y, swap.from.GetTile().Pos().X, swap.from.GetTile().Pos().Y, swap.to.GetM3Type());
        }

        // если есть мэтч
        if (match) { 
            // если найден мэтч
            return match;  
        }
        else { 
            // проверка на бустер
            if (M3Register.boosters.Contains(swap.from.GetM3Type())) {
                if (swap.from.GetM3Type() == M3ObjectType.COLOR_BOMB &&
                    (M3Register.chips.Contains(swap.to.GetM3Type()) || 
                     M3Register.boosters.Contains(swap.to.GetM3Type()))
                    )
                {
                    return true;
                }
                else if (swap.from.GetM3Type() != M3ObjectType.COLOR_BOMB)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (M3Register.boosters.Contains(swap.to.GetM3Type())) {
                if (swap.to.GetM3Type() == M3ObjectType.COLOR_BOMB &&
                    (M3Register.chips.Contains(swap.from.GetM3Type()) || 
                     M3Register.boosters.Contains(swap.from.GetM3Type()))
                    )
                {
                    return true;
                }
                else if (swap.to.GetM3Type() != M3ObjectType.COLOR_BOMB)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }            
            else
            {
                return false;
            }
        }
    }

    // темповые сим объекты
    private static List<M3SimObject> GetTempSimObjects(M3Board board)
    {
        List<M3SimObject> simObjects = new List<M3SimObject>();

        foreach (M3Tile obj in board.GetTiles())
        {
            if (!obj.IsEmpty())
            {
                if (!obj.GetObject().GetSettings().IsSwapEnabled) { continue; }
                simObjects.Add(new M3SimObject(obj.Pos().X, obj.Pos().Y, obj.GetObject().GetM3Type(), true));
            }
        }

        return simObjects;
    }

    // проверяем на shuffle
    public static bool Shufflable(M3Board board)
    {
        // копируем данные из доски
        List<M3SimObject> simObjects = new List<M3SimObject>();

        // перебераем тайлы
        foreach (M3Tile obj in board.GetTiles())
        {
            if (!obj.IsEmpty())
            {
                // если объект бустер и его можно сдвинуть - сразу говорим что смысла перемешки нет
                if (M3Register.boosters.Contains(obj.GetObject().GetM3Type()))
                {
                    List<M3SimObject> tempSimObjects = GetTempSimObjects(board);
                    if (obj.GetObject().GetM3Type() != M3ObjectType.COLOR_BOMB)
                    {
                        if (tempSimObjects.Any(a => a.X == obj.Pos().X && a.Y == obj.Pos().Y + 1) ||
                            tempSimObjects.Any(a => a.X == obj.Pos().X && a.Y == obj.Pos().Y - 1) ||
                            tempSimObjects.Any(a => a.X == obj.Pos().X + 1 && a.Y == obj.Pos().Y) ||
                            tempSimObjects.Any(a => a.X == obj.Pos().X - 1 && a.Y == obj.Pos().Y))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        M3SimObject first = tempSimObjects.Find(a => a.X == obj.Pos().X && a.Y == obj.Pos().Y + 1);
                        M3SimObject second = tempSimObjects.Find(a => a.X == obj.Pos().X && a.Y == obj.Pos().Y - 1);
                        M3SimObject third = tempSimObjects.Find(a => a.X == obj.Pos().X + 1 && a.Y == obj.Pos().Y);
                        M3SimObject fourth = tempSimObjects.Find(a => a.X == obj.Pos().X - 1 && a.Y == obj.Pos().Y);

                        if ((first != null && M3Register.chips.Contains(first.type)) ||
                            (second != null && M3Register.chips.Contains(second.type)) ||
                            (third != null && M3Register.chips.Contains(third.type)) ||
                            (fourth != null && M3Register.chips.Contains(fourth.type))
                            )
                        {
                            return false;
                        }                        
                    }
                }
                // добавляем объект в список
                if (!obj.GetObject().GetSettings().IsSwapEnabled) { continue; }
                simObjects.Add(new M3SimObject(obj.Pos().X, obj.Pos().Y, obj.GetObject().GetM3Type(), true));
            }
        }

        // проверяем все возможные ходы
        foreach (M3SimObject simObj in simObjects) {

            if (HasMatch(GetTempSimObjects(board), simObj.X, simObj.Y, simObj.X + 1, simObj.Y, simObj.type) ||
                HasMatch(GetTempSimObjects(board), simObj.X, simObj.Y, simObj.X - 1, simObj.Y, simObj.type) ||
                HasMatch(GetTempSimObjects(board), simObj.X, simObj.Y, simObj.X, simObj.Y + 1, simObj.type) ||
                HasMatch(GetTempSimObjects(board), simObj.X, simObj.Y, simObj.X, simObj.Y - 1, simObj.type))
            {
                // Debug.Log("Found: " + simObj.X + ", " + simObj.Y);
                return false;
            }
        }

        return true;
    }

    // на позиции
    public static M3SimObject At(List<M3SimObject> list, int x, int y)
    {
        return list.Find(o => o.X == x && o.Y == y);
    }


    // нахождение мэтча
    public static bool HasMatch(List<M3SimObject> list, int fromX, int fromY, int originX, int originY, M3ObjectType type)
    {
        // если оба объекта не null
        if (At(list, fromX, fromY) == null) {  return false; }
        if (At(list, originX, originY) == null) { return false; }

        // удаляем наш объект
        list.RemoveAll(o => o.X == fromX && o.Y == fromY);

        // m3 тайлы по горизонтали
        List<M3SimObject> m3TilesHorizontal = new List<M3SimObject>();
        // m3 тайлы по вертикали
        List<M3SimObject> m3TilesVertical = new List<M3SimObject>();

        // проверяем нас на готовность
        if (!M3Register.chips.Contains(type)) { return false; }

        // ищем по горизонтали, с лева
        for (int x = 1; x < 5; x++)
        {
            // относительно нашей точки
            M3SimObject _tile = At(list, originX + x, originY);

            // брикаем цикл
            if (_tile == null) { break; }
            if (_tile.type != type) { break; }

            // в список добовляем
            m3TilesHorizontal.Add(_tile);
        }

        // ищем по горизонтали, с права
        for (int x = 1; x < 5; x++)
        {
            // относительно нашей точки
            M3SimObject _tile = At(list, originX - x, originY);

            // брикаем цикл
            if (_tile == null) { break; }
            if (_tile.type != type) { break; }

            // в список добовляем
            m3TilesHorizontal.Add(_tile);
        }

        // ищем по вертикали, с верху
        for (int y = 1; y < 5; y++)
        {
            // относительно нашей точки
            M3SimObject _tile = At(list, originX, originY + y);

            // брикаем цикл
            if (_tile == null) { break; }
            if (_tile.type != type) { break; }

            // в список добовляем
            m3TilesVertical.Add(_tile);
        }

        // ищем по вертикали, с низу
        for (int y = 1; y < 5; y++)
        {
            // относительно нашей точки
            M3SimObject _tile = At(list, originX, originY - y);

            // брикаем цикл
            if (_tile == null) { break; }
            if (_tile.type != type) { break; }

            // в список добовляем
            m3TilesVertical.Add(_tile);
        }

        // матч 5 по гонизонтали
        if (m3TilesHorizontal.Count >= 4)
        {
            // возвращаем
            return true;
        }
        // матч 5 по вертикали
        else if (m3TilesVertical.Count >= 4)
        {
            // возвращаем
            return true;
        }
        // матч 3 буквой Г
        else if (m3TilesHorizontal.Count >= 2 && m3TilesVertical.Count >= 2)
        {
            // комбинем лист
            m3TilesHorizontal.AddRange(m3TilesVertical);
            // возвращаем
            return true;
        }
        // матч 4 по гонизонтали
        else if (m3TilesHorizontal.Count == 3 && m3TilesVertical.Count < 2)
        {
            // возвращаем
            return true;
        }
        // матч 4 по вертикали
        else if (m3TilesVertical.Count == 3 && m3TilesHorizontal.Count < 2)
        {
            // возвращаем
            return true;
        }
        // матч 3 по горизонтали
        else if (m3TilesHorizontal.Count >= 2 && m3TilesHorizontal.Count < 5)
        {
            return true;
        }
        // матч 3 по вертикали
        else if (m3TilesVertical.Count >= 2 && m3TilesVertical.Count < 5)
        {
            return true;
        }

        // нулл
        // Debug.LogWarning("Warning! Match not found!");
        return false;
    }
}