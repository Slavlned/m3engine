using DG.Tweening;
using TMPro;
using UnityEngine;

namespace M3
{
    // попап входа на уровень
    public class M3EnterPopup : MonoBehaviour
    {
        // текст
        [SerializeField] private TMP_Text text;
        // ui группа
        [SerializeField] private CanvasGroup canvasGroup;
        // длительность попапа
        [SerializeField] private float moveDuration = 0.5f;
        [SerializeField] private float holdDuration = 0.7f;
        // позиции
        [SerializeField] private Vector3 topTextPosition;
        [SerializeField] private Vector3 middleTextPosition;
        [SerializeField] private Vector3 bottomTextPosition;
        // рут объект
        [SerializeField] private GameObject root;
        
        // показываем
        public void Show(string lvl)
        {
            // включаем объект
            root.SetActive(true);
            // текст
            text.text = "УРОВЕНЬ: " + lvl;
            // твин цвета
            DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, moveDuration))
                .AppendInterval(holdDuration)
                .Append(canvasGroup.DOFade(0f, moveDuration));
            // твин текста
            DOTween.Sequence()
                .Append(text.transform.DOLocalMove(middleTextPosition, moveDuration))
                .AppendInterval(holdDuration)
                .Append(text.transform.DOLocalMove(bottomTextPosition, moveDuration))
                .OnComplete(() =>
                {
                    root.SetActive(false);
                });
        }
    }
}