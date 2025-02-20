using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M3Fx : MonoBehaviour
{
    // эффекты

    // доска
    [SerializeField]
    private M3Board m3Board;

    // стретчи
    [SerializeField]
    private Vector2 smallObjectScale;
    [SerializeField]
    private Vector2 defaultObjectScale;
    [SerializeField]
    private Vector2 stretchObjectScale;
    [SerializeField]
    private float stretchTime;

    // эффекты
    [SerializeField]
    private List<M3FxPair> particles;
    [SerializeField]
    private M3ArrowFx arrowFx;
    [SerializeField]
    private M3ColorBombFx colorBombFx;

    // время мерджа
    [SerializeField]
    private float mergeTime;


    // ивенты
    public void Init()
    {
        m3Board.onFallEnded += OnFallEnded;
        m3Board.onFallStarted += OnFallStarted;
        m3Board.onGenerate += GenerationEffect;
        m3Board.onMerge += MergeEffect;
    }

    public void OnDisable()
    {
        m3Board.onFallEnded -= OnFallEnded;
        m3Board.onFallStarted -= OnFallStarted;
        m3Board.onGenerate -= GenerationEffect;
        m3Board.onMerge -= MergeEffect;
    }

    public void SpawnFx(M3Pos pos, string key)
    {
        // берем тайл
        M3Tile _tile = m3Board.TileAt(pos.X, pos.Y);

        // если тайл нулл
        if (_tile == null) { return;  }

        // инстантиэйтим
        Instantiate(particles.Find(p => p.key == key).pefab, _tile.transform.position, Quaternion.identity);
    }

    public void SpawnArrowFx(M3Board board, M3Pos pos, bool isHorizontal)
    {
        // создаём эффект
        arrowFx.Create(board, pos, isHorizontal);
    }

    public void DestroyFx(GameObject o)
    {
        // уничтожаем
        DOTween.Sequence().
            Append(o.transform.DOScale(
                o.transform.localScale / 2,
                stretchTime / 3
            ))
            .OnComplete(() =>
            {
                Destroy(o.gameObject);
            });
    }

    private void MergeEffect(M3Match m3Match)
    {
        // выключаем гравитацию сорсу
        m3Match.source.GetObject().SetGravity(false);
        // ставим объекту стэйт на умирание
        m3Match.source.GetObject().SetState(M3State.DIE);
        // убиваем анимацию пружины, если она есть
        m3Match.source.GetObject().KillSpring();

        // мерджим
        foreach (M3Tile tile in m3Match.tiles)
        {
            // выключаем гравитацию объекту тайла
            tile.GetObject().SetGravity(false);
            // ставим объекту стэйт на умирание
            tile.GetObject().SetState(M3State.DIE);
            // убиваем анимацию пружины, если она есть
            tile.GetObject().KillSpring();
            // двигаем
            DOTween.Sequence()
                .Append(
                    tile.GetObject().transform.DOMove(
                        m3Match.source.transform.position,
                        mergeTime
                    )
                );
        }
        // короутина
        StartCoroutine(MergeEnd(m3Match));
    }

    private IEnumerator MergeEnd(M3Match m3Match)
    {
        // выждать время
        yield return new WaitForSeconds(mergeTime);

        // удаляем
        foreach (M3Tile tile in m3Match.tiles)
        {
            tile.GetBoard().GetResult().UpdateGoal(tile.GetObject().GetM3Type(), 1);
            tile.GetBoard().DestroyObject(tile.GetObject(), M3DestroyType.CALL);
        }

        m3Match.source.GetBoard().GetResult().UpdateGoal(m3Match.source.GetObject().GetM3Type(), 1);
        m3Match.source.GetBoard().DestroyObject(m3Match.source.GetObject(), M3DestroyType.CALL);

        // ставим объект
        m3Match.source.FillWith(m3Match.GetOut());
    }

    private void OnFallStarted(M3Object m3Object)
    {
    }

    private void OnFallEnded(M3Object m3Object)
    {
        // твин
        bool tweenPlaying = m3Object.springAnimationTween.IsActive() && m3Object.springAnimationTween.IsPlaying();
        if (m3Object.springAnimationTween == null || !tweenPlaying)
        {
            // если нет, запускаем твин
            // ПРОБЛЕМА ЗДЕСЬ ⬇️⬇️⬇️
            m3Object.springAnimationTween = DOTween.Sequence()
                .Append(m3Object.transform.DOLocalMove(GetOffsetPosition(m3Object), stretchTime))
                .Append(m3Object.transform.DOLocalMove(m3Object.transform.localPosition, stretchTime))
                .SetLink(m3Object.gameObject)                
                .SetEase(Ease.InOutQuad);
        }
    }

    private Vector3 GetOffsetPosition(M3Object m3Object)
    {
        switch (m3Object.GetTile().GetGravity())
        {
            case M3GravityDir.UP:
                return m3Object.transform.localPosition + new Vector3(0, 0.05f, 0);
            case M3GravityDir.DOWN:
                return m3Object.transform.localPosition - new Vector3(0, 0.05f, 0);
            case M3GravityDir.LEFT:
                return m3Object.transform.localPosition - new Vector3(0.05f, 0, 0);
            case M3GravityDir.RIGHT:
                return m3Object.transform.localPosition + new Vector3(0.05f, 0, 0);
        }

        return Vector3.zero;
    }

    private void GenerationEffect(M3Object m3Object)
    {
        // ставим уменьшеный размер
        m3Object.transform.localScale = smallObjectScale;
        // твин
        DOTween.Sequence()
            .Append(m3Object.transform.DOScale(defaultObjectScale, stretchTime))
            .SetEase(Ease.InOutQuad);

    }

    public void ActivateColorBombWithFx(M3Board m3Board, M3Pos m3Pos, List<M3Pos> positions)
    {
        // активируем fx
        colorBombFx.CreateAndDamage(m3Board, m3Pos, positions);
    }

    public void ActivateColorBombFillWithFx(M3Board m3Board, M3Pos m3Pos, M3Pos sM3Pos, M3ObjectType fillTo, List<M3Pos> positions)
    {
        // активируем fx
        colorBombFx.CreateAndFill(m3Board, m3Pos, sM3Pos, fillTo, positions);
    }
}