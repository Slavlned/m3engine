using System;
using System.Collections.Generic;
using UnityEngine;

// бомба
public class M3Arrow : MonoBehaviour
{
    [SerializeField]
    // объект
    private M3Object m_Object;

    // ивенты
    private void OnEnable()
    {
        m_Object.onSwapDone += OnSwapDone;
        m_Object.onSwapDoneFrom += OnSwapDoneFrom;
    }

    // ивенты
    private void OnDisable()
    {
        m_Object.onSwapDone -= OnSwapDone;
        m_Object.onSwapDoneFrom -= OnSwapDoneFrom;
    }

    // простая активация
    public void Activate()
    {
        // тайл
        M3Tile _tile = m_Object.GetTile();
        // эффект
        bool isHorizontal = _tile.GetObject().GetM3Type() == M3ObjectType.ARROW_HORIZONTAL ? true : false;
        m_Object.GetBoard().GetFx().SpawnArrowFx(_tile.GetBoard(), _tile.Pos(), isHorizontal);
        // получаем доску и дамажим
        m_Object.GetBoard().DamageArrow(_tile.Pos(), m_Object.GetM3Type());
    }

    // когда свап окончен
    private void OnSwapDone(M3Object m3Object1, M3Object m3Object2)
    {
        // если мы в состоянии смерти то не активируем
        if (m_Object.GetState() == M3State.DIE) { return; }        
        // если одна из клеток не бустер
        if (!M3Register.boosters.Contains(m3Object1.GetM3Type()) ||
            !M3Register.boosters.Contains(m3Object2.GetM3Type()))
        {
            // тайл
            M3Tile _tile = m_Object.GetTile();
            // эффект
            bool isHorizontal = _tile.GetObject().GetM3Type() == M3ObjectType.ARROW_HORIZONTAL ? true : false;
            m_Object.GetBoard().GetFx().SpawnArrowFx(_tile.GetBoard(), _tile.Pos(), isHorizontal);
            // получаем доску и дамажим
            m_Object.GetBoard().DamageArrow(_tile.Pos(), m_Object.GetM3Type());
        }
    }

    // когда свап окончен относительно нас
    private void OnSwapDoneFrom(M3Object m3Object1, M3Object m3Object2)
    {
        // если мы в состоянии смерти то не активируем
        if (m_Object.GetState() == M3State.DIE) { return; }          
        // если мы стрелка горизонтальная, а второй объект - бомба
        if (m3Object2.GetM3Type() == M3ObjectType.BOMB && m_Object.GetM3Type() == M3ObjectType.ARROW_HORIZONTAL)
        {
            // уничтожаем 2й бустер
            m3Object2.Damage(M3DestroyType.NOCALL);  
            m_Object.Damage(M3DestroyType.NOCALL);  
            // эффект
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos(), true);
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos().Add(0, -1), true);
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos().Add(0, 1), true);
            // получаем доску и дамажим
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos(), M3ObjectType.ARROW_HORIZONTAL);
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos().Add(0, -1), M3ObjectType.ARROW_HORIZONTAL);
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos().Add(0, 1), M3ObjectType.ARROW_HORIZONTAL);
        }
        // если мы стрелка вертикальная, а второй объект - бомба
        else if (m3Object2.GetM3Type() == M3ObjectType.BOMB && m_Object.GetM3Type() == M3ObjectType.ARROW_VERTICAL)
        {
            // уничтожаем 2й бустер
            m3Object2.Damage(M3DestroyType.NOCALL);  
            m_Object.Damage(M3DestroyType.NOCALL);           
            // эффект
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos(), false);
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos().Add(-1, 0), false);
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos().Add(1, 0), false);
            // получаем доску и дамажим
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos(), M3ObjectType.ARROW_VERTICAL);
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos().Add(-1, 0), M3ObjectType.ARROW_VERTICAL);
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos().Add(1, 1), M3ObjectType.ARROW_VERTICAL);
        }
        // если мы стрелка, и второй объект - стрелка
        else if (m3Object2.GetM3Type() == M3ObjectType.ARROW_HORIZONTAL || m3Object2.GetM3Type() == M3ObjectType.ARROW_VERTICAL)
        {
            // уничтожаем 2й бустер
            m3Object2.Damage(M3DestroyType.NOCALL);  
            m_Object.Damage(M3DestroyType.NOCALL);            
            // эффект
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos(), true);
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos(), false);
            // получаем доску и дамажим
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos(), M3ObjectType.ARROW_HORIZONTAL);
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos(), M3ObjectType.ARROW_VERTICAL);
        }
        // если мы стрелка, и второй объект - колор бомба
        else if (m3Object2.GetM3Type() == M3ObjectType.COLOR_BOMB)
        {
            // тип объекта
            M3ObjectType rndType = M3Register.chips[M3Random.GetInstance().GetRandom(0, M3Register.chips.Count)];
            // список для позиций
            List<M3Pos> positions = new List<M3Pos>();
            // ищем объекты
            foreach (M3Tile tile in m_Object.GetBoard().GetTiles())
            {
                if (!tile.IsEmpty() &&
                    tile.GetObject().GetM3Type() == rndType)
                {
                    positions.Add(tile.Pos());
                }
            }
            // эффект и дамаг
            m_Object.GetBoard().GetFx().ActivateColorBombFillWithFx(m_Object.GetBoard(), m_Object.GetTile().Pos(), m3Object2.GetTile().Pos(), m_Object.GetM3Type(), positions);
        }
    }
}