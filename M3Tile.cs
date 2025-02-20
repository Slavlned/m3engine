// тайл с объектом внутри
using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class M3Tile : MonoBehaviour
{
    // позиция
    [SerializeField]
    private M3Pos pos;

    // объект
    [SerializeField]
    private M3Object _object;
    
    // нижнее покрытие
    [SerializeField]
    private M3Cover _bottomCover;

    // доска
    [SerializeField]
    private M3Board m3Board;

    // генератор
    [SerializeField]
    private M3Generator m3Generator;

    // доставщик ли (место
    // для спуска некоторых ингридиентов)
    [SerializeField]
    public bool IsDeliverer;

    // гравитация
    [SerializeField]
    private M3GravityDir m3GravityDir;

    // спрайт рендерер
    private SpriteRenderer spriteRenderer;
    
    // дефолтный цвет
    [SerializeField] public Color defaultColor;

    private void Awake()
    {
        // получение спрайт-рендера
        spriteRenderer = GetComponent<SpriteRenderer>();
        // ставим альфа-канал на 0
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }
    
    // получение генератора
    public M3Generator GetGenerator() => m3Generator;

    // получение доски
    public M3Board GetBoard() => m3Board;

    // получение гравитации
    public M3GravityDir GetGravity() => m3GravityDir;
    
    // получение кавера (нижнего)
    public M3Cover GetBottomCover() => _bottomCover;

    // установка генератора
    public void SetGenerator(M3Generator generator) => m3Generator = generator;

    // методы
    public bool IsEmpty()
    {
        return _object == null;
    }
    
    // позиции
    private M3Tile GetUnder()
    {
        if (GetGravity() == M3GravityDir.DOWN)
        {
            // нижняя гравитация
            // 🟦🟦🟦
            // 🟦⬇️🟦
            // 🟦🟥🟦
            return m3Board.TileAt(Pos().X, Pos().Y - 1);
        }
        else if (GetGravity() == M3GravityDir.UP)
        {
            // нижняя гравитация
            // 🟦🟥🟦
            // 🟦⬆️🟦
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X, Pos().Y + 1);
        }
        else if (GetGravity() == M3GravityDir.LEFT)
        {
            // левая гравитация
            // 🟦🟦🟦
            // 🟥⬅️🟦
            // 🟦🟦🟦           
            return m3Board.TileAt(Pos().X - 1, Pos().Y);
        }
        else
        {
            // правая гравитация
            // 🟦🟦🟦
            // 🟦➡️🟥
            // 🟦🟦🟦          
            return m3Board.TileAt(Pos().X + 1, Pos().Y);
        }
    }
    private M3Tile GetLeft()
    {
        if (GetGravity() == M3GravityDir.DOWN)
        {
            // нижняя гравитация
            // 🟦🟦🟦
            // 🟥⬇️🟦
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X - 1, Pos().Y);
        }
        else if (GetGravity() == M3GravityDir.UP)
        {
            // верхняя гравитация
            // 🟦🟦🟦
            // 🟥⬆️🟦
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X - 1, Pos().Y);
        }
        else if (GetGravity() == M3GravityDir.LEFT)
        {
            // левая гравитация
            // 🟦🟦🟦
            // 🟦⬅️🟦
            // 🟦🟥🟦
            return m3Board.TileAt(Pos().X, Pos().Y - 1);
        }
        else
        {
            // правая гравитация
            // 🟦🟦🟦
            // 🟦➡️🟦
            // 🟦🟥🟦
            return m3Board.TileAt(Pos().X, Pos().Y - 1);
        }
    }
    private M3Tile GetRight()
    {
        if (GetGravity() == M3GravityDir.DOWN)
        {
            // нижняя гравитация
            // 🟦🟦🟦
            // 🟦⬇️🟥
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X + 1, Pos().Y);
        }
        else if (GetGravity() == M3GravityDir.UP)
        {
            // верхняя гравитация
            // 🟦🟦🟦
            // 🟦⬆️🟥
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X + 1, Pos().Y);
        }
        else if (GetGravity() == M3GravityDir.LEFT)
        {
            // левая гравитация
            // 🟦🟥🟦
            // 🟦⬅️🟦
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X, Pos().Y + 1);
        }
        else
        {
            // правая гравитация
            // 🟦🟥🟦
            // 🟦➡️🟦
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X, Pos().Y + 1);
        }
    }
    private M3Tile GetUnderLeft()
    {
        if (GetGravity() == M3GravityDir.DOWN)
        {
            // нижняя гравитация
            // 🟦🟦🟦
            // 🟧⬇️🟦
            // 🟥🟦🟦
            return m3Board.TileAt(Pos().X - 1, Pos().Y - 1);
        }
        else if (GetGravity() == M3GravityDir.UP)
        {
            // верхняя гравитация
            // 🟥🟦🟦
            // 🟧⬆️🟦
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X - 1, Pos().Y + 1);
        }
        else if (GetGravity() == M3GravityDir.LEFT)
        {
            // левая гравитация
            // 🟦🟦🟦
            // 🟦⬅️🟦
            // 🟥🟧🟦
            return m3Board.TileAt(Pos().X - 1, Pos().Y - 1);
        }
        else
        {
            // правая гравитация
            // 🟦🟦🟦
            // 🟦➡️🟦
            // 🟦🟧🟥
            return m3Board.TileAt(Pos().X + 1, Pos().Y - 1);
        }
    }
    private M3Tile GetUnderRight()
    {
        if (GetGravity() == M3GravityDir.DOWN)
        {
            // нижняя гравитация
            // 🟦🟦🟦
            // 🟦⬇️🟧
            // 🟦🟦🟥
            return m3Board.TileAt(Pos().X + 1, Pos().Y - 1);
        }
        else if (GetGravity() == M3GravityDir.UP)
        {
            // верхняя гравитация
            // 🟦🟦🟥
            // 🟦⬆️🟧
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X + 1, Pos().Y + 1);
        }
        else if (GetGravity() == M3GravityDir.LEFT)
        {
            // левая гравитация
            // 🟥🟧🟦
            // 🟦⬅️🟦
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X - 1, Pos().Y + 1);
        }
        else
        {
            // правая гравитация
            // 🟦🟧🟥
            // 🟦➡️🟦
            // 🟦🟦🟦
            return m3Board.TileAt(Pos().X + 1, Pos().Y + 1);
        }
    }
    private M3Pos GetLeftPos()
    {
        if (GetGravity() == M3GravityDir.DOWN)
        {
            // нижняя гравитация
            // 🟦🟦🟦
            // 🟥⬇️🟦
            // 🟦🟦🟦
            return new M3Pos(Pos().X - 1, Pos().Y);
        }
        else if (GetGravity() == M3GravityDir.UP)
        {
            // верхняя гравитация
            // 🟦🟦🟦
            // 🟥⬆️🟦
            // 🟦🟦🟦
            return new M3Pos(Pos().X - 1, Pos().Y);
        }
        else if (GetGravity() == M3GravityDir.LEFT)
        {
            // левая гравитация
            // 🟦🟦🟦
            // 🟦⬅️🟦
            // 🟦🟥🟦
            return new M3Pos(Pos().X, Pos().Y - 1);
        }
        else
        {
            // правая гравитация
            // 🟦🟦🟦
            // 🟦➡️🟦
            // 🟦🟥🟦
            return new M3Pos(Pos().X, Pos().Y - 1);
        }
    }
    private M3Pos GetRightPos()
    {
        if (GetGravity() == M3GravityDir.DOWN)
        {
            // нижняя гравитация
            // 🟦🟦🟦
            // 🟦⬇️🟥
            // 🟦🟦🟦
            return new M3Pos(Pos().X + 1, Pos().Y);
        }
        else if (GetGravity() == M3GravityDir.UP)
        {
            // верхняя гравитация
            // 🟦🟦🟦
            // 🟦⬆️🟥
            // 🟦🟦🟦
            return new M3Pos(Pos().X + 1, Pos().Y);
        }
        else if (GetGravity() == M3GravityDir.LEFT)
        {
            // левая гравитация
            // 🟦🟥🟦
            // 🟦⬅️🟦
            // 🟦🟦🟦
            return new M3Pos(Pos().X, Pos().Y + 1);
        }
        else
        {
            // правая гравитация
            // 🟦🟥🟦
            // 🟦➡️🟦
            // 🟦🟦🟦
            return new M3Pos(Pos().X, Pos().Y + 1);
        }
    }

    // тайлы вокруг, снизу
    public M3Tile UnderTile;
    public M3Tile UnderLeftTile;
    public M3Tile UnderRightTile;
    public M3Tile LeftTile;
    public M3Tile RightTile;
    // позиции справа и слева
    public M3Pos RightTilePos;
    public M3Pos LeftTilePos;
    
    // установка объекта
    public void Set(M3Object m3Object)
    {
        // Debug.Log("Empty at: " + Pos().X + ", " + Pos().Y); 
        _object = m3Object;
    }
    
    // установка нижнего кавера
    public void SetBottomCover(M3Cover m3Cover)
    {
        // Debug.Log("Empty at: " + Pos().X + ", " + Pos().Y); 
        _bottomCover = m3Cover;
    }
    
    // позиция
    public M3Pos Pos() => pos;

    // получение объекта
    public M3Object GetObject()
    {
        return _object;
    }

    // установка позиции
    public void SetPos(int x, int y)
    {
        pos = new M3Pos(x, y);
    }
    
    // найти клетки вокруг
    public void FindTilesAround()
    {
        UnderTile = GetUnder();
        UnderLeftTile = GetUnderLeft();
        UnderRightTile = GetUnderRight();
        LeftTile = GetLeft();
        RightTile = GetRight();
        LeftTilePos = GetLeftPos();
        RightTilePos = GetRightPos();             
    }

    // установка гравитации
    public void SetGravity(M3GravityDir gravity)
    {
        // устанавливаем
        m3GravityDir = gravity;
    }

    // ставим доску
    public void SetBoard(M3Board _board) => m3Board = _board;

    // заполнение рандомным объектом
    public void FillRandom()
    {
        // тайл объект
        GameObject _tileObject = Instantiate(
            M3Prefabs.GetInstance().RandomChip().Value,
            m3Board.ToPoint(Pos().X, Pos().Y+1),
            Quaternion.identity
        );
        // получаем объект
        M3Object m3Object = _tileObject.GetComponent<M3Object>();

        // добавляем
        m3Board.AddObject(m3Object);

        // ставим тайлу объект на новосозданный
        m3Object.StartFallTo(this);
        m3Object.SetBoard(m3Board);

        // ивент
        m3Board.onGenerate?.Invoke(m3Object);
    }

    // замена
    public void ReplaceTo(M3ObjectType fillTo)
    {
        // проверяем объект
        if (_object != null)
        {
            // уничтожаем
            m3Board.DestroyObject(_object, M3DestroyType.NOCALL);
        }
        
        // тайл объект
        GameObject _tileObject = Instantiate(
            M3Prefabs.GetInstance().ByKey(fillTo),
            m3Board.ToPoint(Pos().X, Pos().Y),
            Quaternion.identity
        );
        // получаем объект
        M3Object m3Object = _tileObject.GetComponent<M3Object>();

        // добавляем
        m3Board.AddObject(m3Object);

        // ставим тайлу объект на новосозданный
        m3Object.SetTile(this);
        m3Object.SetBoard(m3Board);
        Set(m3Object);
    }

    // заполнение спец. объектом
    public void FillWith(M3ObjectType m3ObjectType)
    {
        // тайл объект
        GameObject _tileObject = Instantiate(
            M3Prefabs.GetInstance().ByKey(m3ObjectType),
            m3Board.ToPoint(Pos().X, Pos().Y + 1),
            Quaternion.identity
        );
        // получаем объект
        M3Object m3Object = _tileObject.GetComponent<M3Object>();

        // добавляем
        m3Board.AddObject(m3Object);

        // ставим тайлу объект на новосозданный
        m3Object.StartFallTo(this);
        m3Object.SetBoard(m3Board);

        // ивент
        m3Board.onGenerate?.Invoke(m3Object);
    }

    // анимирует моргание
    public void AnimateBlink()
    {
        // твин цвета
        DOTween.Sequence().Append(
            spriteRenderer.DOColor(defaultColor, 0.5f)
        );

        // твиним кавер если он есть
        if (_bottomCover != null)
        {
            // твин цвета внутреннего объекта
            var tempRenderer = _bottomCover.GetComponent<SpriteRenderer>();
            DOTween.Sequence().Append(
                tempRenderer.DOColor(new Color(1, 1, 1, 1), 0.5f)
            );
            if (_bottomCover.transform.childCount > 0)
            {
                SpriteRenderer renderer = _bottomCover.transform.GetChild(0).GetComponent<SpriteRenderer>();
            }
        }        
        // твиним объект если он есть
        if (_object != null)
        {
            // твин цвета внутреннего объекта
            var tempRenderer = _object.GetComponent<SpriteRenderer>();
            DOTween.Sequence().Append(
                tempRenderer.DOColor(new Color(1, 1, 1, 1), 0.5f)
            );
            if (_object.transform.childCount > 0)
            {
                TMP_Text text = _object.transform.GetChild(0).GetComponent<TMP_Text>();
                SpriteRenderer renderer = _object.transform.GetChild(0).GetComponent<SpriteRenderer>();

                if (text != null)
                {
                    text.DOColor(new Color(1, 1, 1, 1), 0.5f);
                }
                else if (renderer != null)
                {
                    renderer.DOColor(new Color(1, 1, 1, 1), 0.5f);
                }
            }
        }
    }
}