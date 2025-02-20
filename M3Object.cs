using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
// ReSharper disable Unity.PerformanceCriticalCodeNullComparison
// ReSharper disable RedundantJumpStatement
// ReSharper disable HeuristicUnreachableCode

public class M3Object : MonoBehaviour
{
    /*
     * Переменные
     */
    [SerializeField]
    // доска
    private M3Board m3Board;
    [SerializeField]
    // тип
    private M3ObjectType type;
    [SerializeField]
    // настройки
    private M3ObjectSettings settings;
    public M3Tile GetTile() => tile;
    // метод установки тайла
    public void SetTile(M3Tile _tile) => tile = _tile;
    // получение стэйта
    public M3State GetState() => state;
    public void SetState(M3State _state) => state = _state;
    // время твина
    public float tweenTime = 0.2f;
    // параметры
    [SerializeField]
    private List<M3Param> parameters = new List<M3Param>();
    // максимальная скорость
    [SerializeField] public float maxSpeed = 5f;
    
    // двигаемся ли мы в настоящем времени с клетки на клетку и тд
    [SerializeField]
    private bool moving;

    public bool Moving { get => moving; set => moving = value; }
    
    /*
     * Геттеры и сеттеры
     */
    // геттер & сеттер типа
    public M3ObjectType GetM3Type() => type;
    public M3ObjectType SetM3Type(M3ObjectType _type) => type = _type;
    // геттер параметров
    public List<M3Param> GetParameters() => parameters;
    public void SetParameters(List<M3Param> _parameters) {
        // устанавлиаем параметры
        parameters = _parameters;
        // вызываем ивент
        onParametersSet?.Invoke();
    }
    // геттер настроек
    public M3ObjectSettings GetSettings() => settings;
    // установка гравитации
    public void SetGravity(bool gravity)
    {
        // ставим настройку
        settings.IsGravityEnabled = gravity;
    }
    // геттер & сеттер для доски
    public void SetBoard(M3Board _m3Board)
    {
        m3Board = _m3Board;
    }
    public M3Board GetBoard()
    {
        return m3Board;
    }
    
    
    /*
     * События
     */
    public event Action<M3Object, M3Object> onSwapDone;
    public event Action<M3Object, M3Object> onSwapDoneFrom;
    public event Action onDamageSuccess;
    public event Action onDamageFailure;
    public event Action<M3Object> onCall;
    public event Action onFallEnded;
    public event Action onParametersSet;
    
    
    /*
     * Временные переменные
     */
    // твин пружины
    public Tween springAnimationTween;
    // текущая скорость
    private float currentSpeed = 2f;
    [SerializeField]
    // тайл
    private M3Tile tile;
    [SerializeField]
    // стэйт
    private M3State state;
    
    
    /*
     * Позиции
     */
    private M3Tile Under => tile.UnderTile;
    private M3Tile Left => tile.LeftTile;
    private M3Tile Right => tile.RightTile;
    private M3Tile UnderLeft => tile.UnderLeftTile;
    private M3Tile UnderRight => tile.UnderRightTile;
    private M3Pos LeftPos => tile.LeftTilePos;
    private M3Pos RightPos => tile.RightTilePos;


    /*
     * Гравитация
     */
    // тик гравитации
    public void TickGravity()
    {
        // если состояние не покой - выходим.
        if (Moving || state == M3State.DIE) { return; }
        if (!Fall()) {
            // если состояние не покой, заканчиваем падение.
            if (state != M3State.IDLE) EndFall();
        }
    }    
    // проверка возможности падения вниз
    private bool FallAvailableDown(M3Tile under)
    {
        return under != null && under.IsEmpty() && !Moving;
    }
    // проверка возможности падения по диагонали
    private bool FallAvailableDiag(M3Tile underTile, M3Tile _tile, M3Pos pos)
    {
        return underTile != null && underTile.IsEmpty() && !Moving &&
            (_tile == null || _tile.IsEmpty() || !_tile.GetObject().GetSettings().IsGravityEnabled || _tile.GetGravity() != tile.GetGravity())
            && NothingInFlowCanFall(pos, tile.GetGravity());
    }
    // находимся ли мы в потоке падения (флоу)
    public bool InFallFlow()
    {
        // если тайл нулл значит нет флоу
        if (tile == null)  { return false; }
        
        // кординаты & гравитация
        int curX = tile.Pos().X;
        int curY = tile.Pos().Y;
        M3GravityDir dir = tile.GetGravity();
        
        // смотрим
        switch (dir)
        {
            case M3GravityDir.DOWN:
                for (int y = curY; y > 0; y--)
                {
                    M3Tile ftile = m3Board.TileAt(curX, y);
                    if (ftile != null)
                    {
                        if (ftile.IsEmpty()) { continue; }
                        if (ftile.GetGravity() != dir) { continue; }
                        if (ftile.GetObject().CanFall()) { return true; }
                    }
                    else { continue; }
                }

                return false;
            case M3GravityDir.UP:
                for (int y = curY; y < m3Board.GetInfo().height; y++)
                {
                    M3Tile ftile = m3Board.TileAt(curX, y);
                    if (ftile != null)
                    {
                        if (ftile.IsEmpty()) { continue; }
                        if (ftile.GetGravity() != dir) { continue; }
                        if (ftile.GetObject().CanFall()) { return true; }
                    }
                    else { continue; }
                }

                return false;
            case M3GravityDir.LEFT:
                for (int x = curX; x > 0; x--)
                {
                    M3Tile ftile = m3Board.TileAt(x, curY);
                    if (ftile != null)
                    {
                        if (ftile.IsEmpty()) { continue; }
                        if (ftile.GetGravity() != dir) { continue; }
                        if (ftile.GetObject().CanFall()) { return true; }
                    }
                    else { continue; }
                }

                return false;
            case M3GravityDir.RIGHT:
                for (int x = curX; x < m3Board.GetInfo().width; x++)
                {
                    M3Tile ftile = m3Board.TileAt(x, curY);
                    if (ftile != null)
                    {
                        if (ftile.IsEmpty()) { continue; }
                        if (ftile.GetGravity() != dir) { continue; }
                        if (ftile.GetObject().CanFall()) { return true; }
                    }
                    else { continue; }
                }

                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    // может ли упасть объект
    public bool CanFall()
    {
        // если гравитация выключена - то нельзя падать
        if (!settings.IsGravityEnabled) { return false; }

        // гравитация
        M3Tile under = Under;
        M3Tile underLeft = UnderLeft;
        M3Tile underRight = UnderRight;
        M3Tile left = Left;
        M3Tile right = Right;
        M3Pos lPos = LeftPos;
        M3Pos rPos = RightPos;

        // низ
        if (FallAvailableDown(under)) { return true; }

        // левая диагональ
        if (FallAvailableDiag(underLeft, left, lPos)) { return true; }

        // правая диагональ
        if (FallAvailableDiag(underRight, right, rPos)) { return true; }

        // в ином случае
        return false;
    }
    // падение в клеточку
    public void StartFallTo(M3Tile _tile)
    {
        // если есть какие-то анимки - то убиваем их твины
        KillSpringTween();
        // если гравитация выключена - то нельзя падать
        if (!settings.IsGravityEnabled) { return; }
        // ставим состояние на движение
        Moving = true;
        SetState(M3State.MOVE);
        // ставим клеточку
        if (tile != null) { tile.Set(null); }
        _tile.Set(this);
        tile = _tile;
        // твиним падение
        DOTween.Sequence()
            .Append(transform.DOMove(_tile.transform.position, tweenTime / currentSpeed))
            .OnKill(HandleFallEnd);            
        // событие
        m3Board?.onFallStarted?.Invoke(this);
    }
    // тикаем падение
    private bool Fall()
    {
        // если гравитация выключена - то нельзя падать
        if (!settings.IsGravityEnabled) { return false; }
        // Ищем позиции
        M3Tile under = Under;
        M3Tile underLeft = UnderLeft;
        M3Tile underRight = UnderRight;
        M3Tile left = Left;
        M3Tile right = Right;
        M3Pos lPos = LeftPos;
        M3Pos rPos = RightPos;
        // смотрим можно ли упасть в какую-либо клеточку
        // низ
        if (FallAvailableDown(under)) { StartFallTo(under); return true; }
        // левая диагональ
        if (FallAvailableDiag(underLeft, left, lPos)) { StartFallTo(underLeft); return true; }
        // правая диагональ
        if (FallAvailableDiag(underRight, right, rPos)) { StartFallTo(underRight); return true; }
        // устанавливаем состояние покоя
        return false;
    }
    // обновление падения
    private void HandleFallEnd()
    {
        // заканчиваем падение
        Moving = false;
    }
    // может ли что-то верхнее(из потока) упасть
    private bool NothingInFlowCanFall(M3Pos pos, M3GravityDir dir)
    {
        if (dir == M3GravityDir.DOWN)
        {
            int x = pos.X;
            for (int y = pos.Y; y < m3Board.GetInfo().height; y++)
            {
                if (m3Board.TileAt(x, y) == null)
                {
                    return true;
                }
                else
                {
                    if (m3Board.TileAt(x, y).GetGravity() != dir) { return true; }
                    if (!m3Board.TileAt(x, y).IsEmpty() && !m3Board.TileAt(x, y).GetObject().GetSettings().IsGravityEnabled) { return true; }
                    if (!m3Board.TileAt(x, y).IsEmpty() && m3Board.TileAt(x, y).GetObject().GetSettings().IsGravityEnabled)
                    {
                        // if (m3Board.TileAt(x, y).GetObject().CanFall())
                        // {
                        return false;
                        // }
                    }
                }
            }
        }
        else if (dir == M3GravityDir.UP)
        {
            int x = pos.X;
            for (int y = pos.Y; y > 0; y--)
            {
                if (m3Board.TileAt(x, y) == null)
                {
                    return true;
                }
                else
                {
                    if (m3Board.TileAt(x, y).GetGravity() != dir) { return true; }
                    if (!m3Board.TileAt(x, y).IsEmpty() && !m3Board.TileAt(x, y).GetObject().GetSettings().IsGravityEnabled) { return true; }
                    if (!m3Board.TileAt(x, y).IsEmpty() && m3Board.TileAt(x, y).GetObject().GetSettings().IsGravityEnabled)
                    {
                        // if (m3Board.TileAt(x, y).GetObject().CanFall())
                        // {
                        return false;
                        // }
                    }
                }
            }
        }
        else if (dir == M3GravityDir.RIGHT)
        {
            int y = pos.Y;
            for (int dx = pos.X; dx > 0; dx--)
            {
                if (m3Board.TileAt(dx, y) == null)
                {
                    return true;
                }
                else
                {
                    if (m3Board.TileAt(dx, y).GetGravity() != dir) { return true; }
                    if (!m3Board.TileAt(dx, y).IsEmpty() && !m3Board.TileAt(dx, y).GetObject().GetSettings().IsGravityEnabled) { return true; }
                    if (!m3Board.TileAt(dx, y).IsEmpty() && m3Board.TileAt(dx, y).GetObject().GetSettings().IsGravityEnabled)
                    {
                        // if (m3Board.TileAt(x, y).GetObject().CanFall())
                        // {
                        return false;
                        // }
                    }
                }
            }
        }
        else if (dir == M3GravityDir.LEFT)
        {
            int y = pos.Y;
            for (int dx = pos.X; dx < m3Board.GetInfo().width; dx++)
            {
                if (m3Board.TileAt(dx, y) == null)
                {
                    return true;
                }
                else
                {
                    if (m3Board.TileAt(dx, y).GetGravity() != dir) { return true; }
                    if (!m3Board.TileAt(dx, y).IsEmpty() && !m3Board.TileAt(dx, y).GetObject().GetSettings().IsGravityEnabled) { return true; }
                    if (!m3Board.TileAt(dx, y).IsEmpty() && m3Board.TileAt(dx, y).GetObject().GetSettings().IsGravityEnabled)
                    {
                        // if (m3Board.TileAt(x, y).GetObject().CanFall())
                        // {
                        return false;
                        // }
                    }
                }
            }
        }

        return true;
    }
    // остановка падения
    private void EndFall()
    {
        // состояние - покой.
        SetState(M3State.IDLE);
        // добавляем матч очередь.
        m3Board.EnqueueMatch(FindMatch(false));
        // ивент.
        onFallEnded?.Invoke();
        // ивенты
        m3Board?.onFallEnded?.Invoke(this);
    }
    // убийство анимации пружины
    public void KillSpring()
    {
        // убиваем, если надо
        if (springAnimationTween != null && springAnimationTween.IsActive())
        {
            springAnimationTween?.Kill();
        }
    }
    // падает ли что-то в потоке (поток падения)
    private bool SomethingFallingInFlow() 
    {
        // если тайл нулл значит нет флоу
        if (tile == null)  { return false; }
        
        // кординаты & гравитация
        int curX = tile.Pos().X;
        int curY = tile.Pos().Y;
        M3GravityDir dir = tile.GetGravity();
        
        // смотрим
        switch (dir)
        {
            case M3GravityDir.DOWN:
                for (int y = m3Board.GetInfo().height-1; y > 0; y--)
                {
                    M3Tile tempTile = m3Board.TileAt(curX, y);
                    if (tempTile != null)
                    {
                        if (tempTile.IsEmpty() ||
                            tile.GetGravity() != dir) { continue; }
                        if (tempTile.GetObject().Moving || tempTile.GetObject().GetState() != M3State.IDLE ||
                            tempTile.GetObject().CanFall()) { return true; }
                    }
                    else { continue; }
                }

                return false;
            case M3GravityDir.UP:
                for (int y = 0; y < m3Board.GetInfo().height; y++)
                {
                    M3Tile tempTile = m3Board.TileAt(curX, y);
                    // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
                    if (tempTile != null)
                    {
                        if (tempTile.IsEmpty() ||
                            tempTile.GetGravity() != dir) { continue; }
                        if (tempTile.GetObject().Moving || tempTile.GetObject().GetState() != M3State.IDLE ||
                            tempTile.GetObject().CanFall()) { return true; }
                    }
                    else { continue; }
                }

                return false;
            case M3GravityDir.LEFT:
                for (int x = m3Board.GetInfo().width-1; x > 0; x--)
                {
                    M3Tile tempTile = m3Board.TileAt(x, curY);
                    if (tempTile != null)
                    {
                        if (tempTile.IsEmpty() ||
                            tempTile.GetGravity() != dir) { continue; }
                        if (tempTile.GetObject().Moving || tempTile.GetObject().GetState() != M3State.IDLE ||
                            tempTile.GetObject().CanFall()) { return true; }
                    }
                    else { continue; }
                }

                return false;
            case M3GravityDir.RIGHT:
                for (int x = 0; x < m3Board.GetInfo().width; x++)
                {
                    M3Tile tempTile = m3Board.TileAt(x, curY);
                    if (tempTile != null)
                    {
                        if (tempTile.IsEmpty() ||
                            tempTile.GetGravity() != dir) { continue; }
                        if (tempTile.GetObject().Moving || tempTile.GetObject().GetState() != M3State.IDLE ||
                            tempTile.GetObject().CanFall()) { return true; }
                    }
                    else { continue; }
                }

                return false;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        // возвращаем что флоу нет (лишний код ниже)
        // return false;
    }
    
    
    /*
     * Мэтчи
     */
    private List<M3Tile> FindMatchablesHor(bool isRight)
    {
        List<M3Tile> m3TilesHorizontal = new List<M3Tile>();
        
        for (int x = 1; x < m3Board.GetInfo().width; x++)
        {
            // относительно нашей точки
            M3Tile _tile;
            if (isRight)
            {
                // справа
                _tile = m3Board.TileAt(tile.Pos().X + x, tile.Pos().Y);
            }
            else
            {
                // слева
                _tile = m3Board.TileAt(tile.Pos().X - x, tile.Pos().Y);
            }

            // брикаем цикл
            if (_tile == null) { break; }
            if (_tile.IsEmpty()) { break; }
            if (_tile.GetObject().GetM3Type() != GetM3Type()) { break; }
            if (_tile.GetObject().state == M3State.DIE) { break; }

            // в список добовляем
            m3TilesHorizontal.Add(_tile);
        }

        return m3TilesHorizontal;
    }
    private List<M3Tile> FindMatchablesVer(bool isTop)
    {
        List<M3Tile> m3TilesHorizontal = new List<M3Tile>();
        
        for (int y = 1; y < m3Board.GetInfo().height; y++)
        {
            // относительно нашей точки
            M3Tile _tile;
            if (isTop)
            {
                // верх
                _tile = m3Board.TileAt(tile.Pos().X, tile.Pos().Y + y);
            }
            else
            {
                // низ
                _tile = m3Board.TileAt(tile.Pos().X, tile.Pos().Y - y);
            }

            // брикаем цикл
            if (_tile == null) { break; }
            if (_tile.IsEmpty()) { break; }
            if (_tile.GetObject().GetM3Type() != GetM3Type()) { break; }
            if (_tile.GetObject().state == M3State.DIE) { break; }

            // в список добовляем
            m3TilesHorizontal.Add(_tile);
        }

        return m3TilesHorizontal;
    }
    // проверка и нахождение мэтча
    public M3Match FindMatch(bool isMatchInitiatedByPlayer)
    {
        // TO:DO Изменить размер нахождения
        // по аксису с 5 до размера поля по нужному ✅
        // аксису

        // m3 тайлы по горизонтали
        List<M3Tile> m3TilesHorizontal = new List<M3Tile>();
        // m3 тайлы по вертикали
        List<M3Tile> m3TilesVertical = new List<M3Tile>();

        // проверяем нас на готовность
        if (Moving) { return null; }
        if (state != M3State.IDLE) { return null; }
        if (!M3Register.chips.Contains(GetM3Type())) { return null; }

        // ищем по горизонтали, слева и справа
        m3TilesHorizontal.AddRange(FindMatchablesHor(isRight: false));
        m3TilesHorizontal.AddRange(FindMatchablesHor(isRight: true));
        // ищем по вертикали, снизу и сверху
        m3TilesVertical.AddRange(FindMatchablesVer(isTop: false));
        m3TilesVertical.AddRange(FindMatchablesVer(isTop: true));

        // матч 5 по гонизонтали
        if (m3TilesHorizontal.Count >= 4)
        {
            // возвращаем
            return new M3Match(this.GetTile(), m3TilesHorizontal, M3ObjectType.COLOR_BOMB, isMatchInitiatedByPlayer);
        }
        // матч 5 по вертикали
        else if (m3TilesVertical.Count >= 4)
        {
            // возвращаем
            return new M3Match(this.GetTile(), m3TilesVertical, M3ObjectType.COLOR_BOMB, isMatchInitiatedByPlayer);
        }
        // матч 3 буквой Г и Т
        else if (m3TilesHorizontal.Count >= 2 && m3TilesVertical.Count >= 2)
        {
            // комбинем лист
            m3TilesHorizontal.AddRange(m3TilesVertical);
            // возвращаем
            return new M3Match(this.GetTile(), m3TilesHorizontal, M3ObjectType.BOMB, isMatchInitiatedByPlayer);
        }
        // матч 4 по гонизонтали
        else if (m3TilesHorizontal.Count == 3 && m3TilesVertical.Count < 2)
        {
            // возвращаем
            return new M3Match(this.GetTile(), m3TilesHorizontal, M3ObjectType.ARROW_VERTICAL, isMatchInitiatedByPlayer);
        }
        // матч 4 по вертикали
        else if (m3TilesVertical.Count == 3 && m3TilesHorizontal.Count < 2)
        {
            // возвращаем
            return new M3Match(this.GetTile(), m3TilesVertical, M3ObjectType.ARROW_HORIZONTAL, isMatchInitiatedByPlayer);
        }
        // матч 3 по горизонтали
        else if (m3TilesHorizontal.Count >= 2 && m3TilesHorizontal.Count < 5)
        {
            return new M3Match(this.GetTile(), m3TilesHorizontal, isMatchInitiatedByPlayer);
        }
        // матч 3 по вертикали
        else if (m3TilesVertical.Count >= 2 && m3TilesVertical.Count < 5)
        {
            return new M3Match(this.GetTile(), m3TilesVertical, isMatchInitiatedByPlayer);
        }

        // нулл
        // Debug.LogWarning("Warning! Match not found!");
        return null;
    }
    
    
    /*
     * Состояние
     */
    // установка состояния на idle
    private void SetIdle(M3Object first, M3Object second)
    {
        // idle
        Moving = false;
        SetState(M3State.IDLE);
        // добавляем матч очередь
        m3Board.EnqueueMatch(FindMatch(true));
        // ивенты
        onSwapDone?.Invoke(first, second);
    }
    // можно ли интерактить
    public bool CanInteract()
    {
        return state == M3State.IDLE && !Moving;
    }
    
    
    /*
     * Уничтожение объекта
     */
    public void DestroyObject(M3DestroyType type)
    {
        // удаляем из списка
        m3Board.objects.Remove(this);
        // вызываем колл объектов вокруг
        // если тип дестроя - колл
        if (type == M3DestroyType.CALL)
        {
            CallAround();
        }
        // ставим клетку на пустоту
        GetTile().Set(null);
        // ставим состояние объекта на мертвое
        SetState(M3State.DIE);
        // уничтожаем с эффектом
        m3Board.GetFx().DestroyFx(gameObject);
    }
    
    
    /*
     * Визуалы
     */
    private void KillSpringTween()
    {
        if (springAnimationTween != null)
        {
            if (springAnimationTween.IsActive())
            {
                springAnimationTween.Kill();
            }

            springAnimationTween = null;
        }        
    }
    // установка альфа канала чилдам на 0
    // ReSharper disable Unity.PerformanceAnalysis
    public void SetZeroAlphaForChildren()
    {
        if (transform.childCount > 0)
        {
            TMP_Text text = transform.GetChild(0).GetComponent<TMP_Text>();            
            SpriteRenderer spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();

            if (text != null)
            {
                text.color = new Color(1, 1, 1, 0);
            }
            else if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(1, 1, 1, 0);
            }
        }
    }  
    
    
    /*
     * Свапы, дамаг, вызовы
     */
    public void CallAround()
    {
        // проверяем наш тайл на нулл
        if (tile == null) { return;  }
        // тайлы
        M3Tile up = m3Board.TileAt(tile.Pos().Add(0, 1));
        M3Tile down = m3Board.TileAt(tile.Pos().Add(0, -1));
        M3Tile left = m3Board.TileAt(tile.Pos().Add(-1, 0));
        M3Tile right = m3Board.TileAt(tile.Pos().Add(1, 0));
        // проверяем и вызваем
        if (up != null && !up.IsEmpty()) { up.GetObject().onCall?.Invoke(this); }
        if (down != null && !down.IsEmpty()) { down.GetObject().onCall?.Invoke(this); }
        if (left != null && !left.IsEmpty()) { left.GetObject().onCall?.Invoke(this); }
        if (right != null && !right.IsEmpty()) { right.GetObject().onCall?.Invoke(this); }
    }
    // свап с объектом
    public void Swap(M3Object other)
    {
        // если есть какие-то анимки - то убиваем их твины
        KillSpringTween();
        
        // позиции
        Vector3 otherPosition = other.transform.position;
        Vector3 selfPosition = transform.position;

        // ставим состояние MOVE
        other.SetState(M3State.MOVE);
        other.Moving = true;
        SetState(M3State.MOVE);
        Moving = true;

        // двигаемся к другому объекту
        DOTween.Sequence()
            .Join(transform.DOMove(otherPosition, tweenTime))
            .Join(other.transform.DOMove(selfPosition, tweenTime))
            .OnComplete(() =>
            {
                // По приоритету устанавливаем состояние IDLE
                // Сначало обычному, потом бустеру. Чтобы обычный предмет
                // с которым свапился бустер первее перешел в IDLE. Чтобы
                // бустер его задел.
                // P.S Если оба бустеры или обычные предметы, ничего не поменяется. 
                
                // объект с высоким приоритетом.
                M3Object highPriority;
                // объект с низким.
                M3Object lowPriority;
                // ищем по приоритету
                if (M3Register.boosters.Contains(GetM3Type()))
                {
                    highPriority = other;
                    lowPriority = this;
                }
                else
                {
                    highPriority = this;
                    lowPriority = other;
                }
                
                // устанавливаем состояние покоя
                highPriority.SetIdle(first: this, second: other);
                // устанавливаем состояние покоя
                lowPriority.SetIdle(first: other, second: this);
                
                // ивент
                onSwapDoneFrom?.Invoke(this, other);
            });

        // меняем клетки
        other.GetTile().Set(this);
        this.GetTile().Set(other);

        // тайлы
        M3Tile otherTile = other.GetTile();
        other.SetTile(this.GetTile());
        SetTile(otherTile);
    }
    // фэйковый свап
    public void FakeSwap(M3Object other)
    {
        // если есть какие-то анимки - то убиваем их твины
        KillSpringTween();

        // позиции
        Vector3 otherPosition = other.transform.position;
        Vector3 selfPosition = transform.position;

        // ставим состояние MOVE
        other.SetState(M3State.MOVE);
        other.Moving = true;
        SetState(M3State.MOVE);
        Moving = true;

        // двигаемся к другому объекту
        DOTween.Sequence()
            .Append(transform.DOMove(otherPosition, tweenTime))
            .Append(transform.DOMove(selfPosition, tweenTime))
            .OnComplete(() =>
            {
                SetState(M3State.IDLE);
                Moving = false;
            });

        // двигаем тот объект на наше место
        DOTween.Sequence()
            .Append(other.transform.DOMove(selfPosition, tweenTime))
            .Append(other.transform.DOMove(otherPosition, tweenTime))
            .OnComplete(() =>
            {
                other.SetState(M3State.IDLE);
                other.Moving = false;
            });

    }
    // дамаг
    public void Damage(M3DestroyType destroyType)
    {
        // если мы в состоянии смерти то не дамажим
        if (GetState() == M3State.DIE) { return; }
        // наносим урон и вызываем событие
        if (settings.IsDamageable)
        {
            // ивент
            onDamageSuccess?.Invoke();
            // цели
            m3Board.GetResult().UpdateGoal(type, 1);
            // уничтожаем
            m3Board.DestroyObject(this, destroyType);
        }
        // в ином случае вызываем другое событие
        else
        {
            // ивент
            onDamageFailure?.Invoke();
        }
    }


    /*
     * Заказ тика
     */
    public void TickOrder()
    {
     
    }
}