using DG.Tweening;
using UnityEngine;

public class M3ArrowFx : MonoBehaviour
{
    // префабы для FX
    [SerializeField]
    private GameObject up_Prefab;
    [SerializeField]
    private GameObject down_Prefab;
    [SerializeField]
    private GameObject left_Prefab;
    [SerializeField]
    private GameObject right_Prefab;
    [SerializeField] 
    private GameObject sfx_Prefab;

    // настройки
    [SerializeField]
    private float minValue = -10;
    [SerializeField]
    private float maxValue = 10;
    [SerializeField]
    private float fxTime = 2;

    // создание
    public void Create(M3Board board, M3Pos pos, bool isHorizontal)
    {
        // если горизонтальный
        if (isHorizontal)
        {
            // левый префаб
            Transform left = Instantiate(
                left_Prefab, 
                board.ToPoint(pos.X, pos.Y), 
                Quaternion.identity
            ).transform;
            // правый префаб
            Transform right = Instantiate(
                right_Prefab, 
                board.ToPoint(pos.X, pos.Y), 
                Quaternion.identity
            ).transform;
            // звук
            Instantiate(sfx_Prefab, board.ToPoint(pos.X, pos.Y), Quaternion.identity);
            // двигаем твином левую часть
            Vector2 targetLeft = left.position + new Vector3(minValue, 0, 0);
            DOTween.Sequence()
                .Append(
                    left.transform.DOMove(targetLeft, fxTime)
                )
                .SetLink(left.gameObject);
            // двигаем твином правую часть
            Vector2 targetRight = right.position + new Vector3(maxValue, 0, 0);
            DOTween.Sequence()
                .Append(
                    right.transform.DOMove(targetRight, fxTime)
                )
                .SetLink(right.gameObject);
        }
        else {
            // левый префаб
            Transform up = Instantiate(
                up_Prefab,
                board.ToPoint(pos.X, pos.Y),
                Quaternion.identity
            ).transform;
            // правый префаб
            Transform down = Instantiate(
                down_Prefab,
                board.ToPoint(pos.X, pos.Y),
                Quaternion.identity
            ).transform;
            // звук
            Instantiate(sfx_Prefab, board.ToPoint(pos.X, pos.Y), Quaternion.identity);            
            // двигаем твином верхнию часть
            Vector2 targetDown = up.position + new Vector3(0, minValue, 0);
            DOTween.Sequence()
                .Append(
                    down.transform.DOMove(targetDown, fxTime)
                )
                .SetLink(down.gameObject);
            // двигаем твином нижнию часть
            Vector2 targetUp = down.position + new Vector3(0, maxValue, 0);
            DOTween.Sequence()
                .Append(
                    up.transform.DOMove(targetUp, fxTime)
                )
                .SetLink(up.gameObject);
        }
    }
}