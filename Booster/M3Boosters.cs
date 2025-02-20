using System;
using DG.Tweening;
using UnityEngine;

public class M3Boosters : MonoBehaviour
{
    // спец. бустеры для match3
    // доска
    [SerializeField] public M3Board m3Board;
    // тип бустера
    [SerializeField] private M3BoosterType selected;
    // инпут
    [SerializeField] private M3Input m3Input;
    // текущая кнопка
    [SerializeField] private M3BoosterButton button;
    // префабы
    [SerializeField] private GameObject needlePrefab;
    [SerializeField] private GameObject snailPrefab;
    [SerializeField] private GameObject rakePrefab;
    // длина анимации
    [SerializeField] private float animationDuration;

    // активация
    public void ActivateBoosterAt(M3Pos pos)
    {
        switch (selected)
        {
            // грабли
            case M3BoosterType.RAKE:
                // дамаг с анимацией
                GameObject rake = Instantiate(rakePrefab, m3Board.ToPoint(pos.X, m3Board.GetInfo().height-1), Quaternion.identity);
                DOTween.Sequence()
                    .Append(rake.transform.DOMove(m3Board.ToPoint(pos.X, 0), animationDuration))
                    .OnComplete(() =>
                    {
                        Destroy(rake);
                    });
                m3Board.DamageArrow(pos, M3ObjectType.ARROW_VERTICAL);
                break;
            // улитка
            case M3BoosterType.SNAIL:
                // дамаг
                GameObject snail = Instantiate(snailPrefab, m3Board.ToPoint(0, pos.Y), Quaternion.identity);
                DOTween.Sequence()
                    .Append(snail.transform.DOMove(m3Board.ToPoint(m3Board.GetInfo().width-1, pos.Y), animationDuration))
                    .OnComplete(() =>
                    {
                        Destroy(snail);
                    });
                m3Board.DamageArrow(pos, M3ObjectType.ARROW_HORIZONTAL);
                break;
            // игла
            case M3BoosterType.NEEDLE:
                // дамаг
                GameObject needle = Instantiate(needlePrefab, new Vector3(pos.X+2, pos.Y+2), Quaternion.identity);
                DOTween.Sequence()
                    .Append(needle.transform.DOMove(m3Board.ToPoint(pos.X, pos.Y), animationDuration))
                    .OnComplete(() =>
                    {
                        Destroy(needle);
                    });
                m3Board.DamageOrActivate(m3Board.TileAt(pos));
                break;            
            default:
                throw new Exception("Invalid booster type");
        }
        // анимация кнопки
        button.GetAnimator().Play("ButtonIdle");
        // устанавливаем тип инпута на бустер
        m3Input.SetType(M3InputType.DEFAULT);        
    }

    // селектим
    public void Select(int index, M3BoosterButton btn)
    {
        // если поле idle
        if (m3Board.FullIdle() && !m3Board.GetLock() && m3Board.GetGravity())
        {
            // селектим
            selected = (M3BoosterType)index;
            // текущая кнопка
            button = btn;
            // устанавливаем тип инпута на бустер
            m3Input.SetType(M3InputType.BOOSTER);
        }
        else
        {
            // анимация кнопки
            button.GetAnimator().Play("ButtonIdle");
            // устанавливаем тип инпута на бустер
            m3Input.SetType(M3InputType.DEFAULT);              
        }
    }

    // инпут
    private void OnEnable()
    {
        m3Input.onBoosterInput += ActivateBoosterAt;
    }

    private void OnDisable()
    {
        m3Input.onBoosterInput -= ActivateBoosterAt;
    }    
}