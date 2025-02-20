using System;
using System.Collections.Generic;
using UnityEngine;

// бомба
public class M3Bomb : MonoBehaviour {
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
        // эффект
        m_Object.GetBoard().GetFx().SpawnFx(m_Object.GetTile().Pos(), "FX_BOMB");
        // получаем доску и дамажим
        m_Object.GetBoard().DamageWave(m_Object.GetTile().Pos(), 1);
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
            // эффект
            m_Object.GetBoard().GetFx().SpawnFx(m_Object.GetTile().Pos(), "FX_BOMB");
            // получаем доску и дамажим
            m_Object.GetBoard().DamageWave(m_Object.GetTile().Pos(), 1);
        }
    }

    // когда свап окончен относительно нас
    private void OnSwapDoneFrom(M3Object m3Object1, M3Object m3Object2)
    {
        // если мы в состоянии смерти то не активируем
        if (m_Object.GetState() == M3State.DIE) { return; }          
        // если обе бомбы
        if (m3Object1.GetM3Type() == m3Object2.GetM3Type() && m3Object1.GetM3Type() == M3ObjectType.BOMB)
        {
            // уничтожаем 2й бустер
            m3Object2.Damage(M3DestroyType.NOCALL);  
            m_Object.Damage(M3DestroyType.NOCALL);              
            // эффект
            m_Object.GetBoard().GetFx().SpawnFx(m3Object2.GetTile().Pos(), "FX_BOMB");
            // получаем доску и дамажим
            m_Object.GetBoard().DamageWave(m3Object2.GetTile().Pos(), 2);
        }
        // если мы бомба, а второй объект - стрелка горизонтальная
        else if (m3Object2.GetM3Type() == M3ObjectType.ARROW_HORIZONTAL)
        {
            // уничтожаем 2й бустер
            m3Object2.Damage(M3DestroyType.NOCALL);  
            m_Object.Damage(M3DestroyType.NOCALL);              
            // эффект
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos(), true);
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos().Add(0, 1), true);
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos().Add(0, -1), true);
            // получаем доску и дамажим
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos(), m3Object2.GetM3Type());
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos().Add(0, 1), m3Object2.GetM3Type());
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos().Add(0, -1), m3Object2.GetM3Type());
        }
        // если мы бомба, а второй объект - стрелка вертикальная
        else if (m3Object2.GetM3Type() == M3ObjectType.ARROW_VERTICAL)
        {
            // уничтожаем 2й бустер
            m3Object2.Damage(M3DestroyType.NOCALL);  
            m_Object.Damage(M3DestroyType.NOCALL);  
            // эффект
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos(), false);
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos().Add(1, 0), false);
            m_Object.GetBoard().GetFx().SpawnArrowFx(m3Object2.GetBoard(), m3Object2.GetTile().Pos().Add(-1, 0), false);
            // получаем доску и дамажим
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos(), m3Object2.GetM3Type());
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos().Add(1, 0), m3Object2.GetM3Type());
            m_Object.GetBoard().DamageArrow(m3Object2.GetTile().Pos().Add(-1, 0), m3Object2.GetM3Type());
        }
        // если мы бомба, а вторая цветная бомба
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
            m_Object.GetBoard().GetFx().ActivateColorBombFillWithFx(m_Object.GetBoard(), m_Object.GetTile().Pos(), m3Object2.GetTile().Pos(), M3ObjectType.BOMB, positions);
        }
    }
}