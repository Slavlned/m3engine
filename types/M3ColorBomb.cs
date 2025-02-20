using System;
using System.Collections.Generic;
using UnityEngine;

// бомба
public class M3ColorBomb : MonoBehaviour
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

    // когда свап окончен
    private void OnSwapDone(M3Object m3Object1, M3Object m3Object2)
    {
        // если мы в состоянии смерти то не активируем
        if (m_Object.GetState() == M3State.DIE) { return; }          
        // если одна из клеток не бустер
        if (!M3Register.boosters.Contains(m3Object1.GetM3Type()) ||
            !M3Register.boosters.Contains(m3Object2.GetM3Type()))
        {
            // тип объекта
            M3ObjectType objectType = 
                M3Register.boosters.Contains(m3Object1.GetM3Type()) ? 
                    m3Object2.GetM3Type() : 
                    m3Object1.GetM3Type();

            // если это не фишка
            if (!M3Register.chips.Contains(objectType))
            {
                return;
            }

            // список для позиций
            List<M3Pos> positions = new List<M3Pos>();
            // ищем объекты
            foreach (M3Tile tile in m_Object.GetBoard().GetTiles())
            {
                if (!tile.IsEmpty() && tile.GetObject().GetM3Type() == objectType)
                {
                    positions.Add(tile.Pos());
                }
            }

            // эффект и дамаг
            m_Object.GetBoard().GetFx().ActivateColorBombWithFx(m_Object.GetBoard(), m_Object.GetTile().Pos(), positions);
        }
    }


    // когда свап окончен относительно нас
    private void OnSwapDoneFrom(M3Object m3Object1, M3Object m3Object2)
    {
        // если мы в состоянии смерти то не активируем
        if (m_Object.GetState() == M3State.DIE) { return; }          
        // если обе из клеток бустеры
        if (M3Register.boosters.Contains(m3Object1.GetM3Type()) &&
            M3Register.boosters.Contains(m3Object2.GetM3Type()))
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
            m_Object.GetBoard().GetFx().ActivateColorBombFillWithFx(m_Object.GetBoard(), m_Object.GetTile().Pos(), m3Object2.GetTile().Pos(), m3Object2.GetM3Type(), positions);
        }
    }
}