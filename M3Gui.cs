using DG.Tweening;
using TMPro;
using UnityEngine;

public class M3Gui : MonoBehaviour
{
    // текст для поп-апа
    public TMP_Text m_popupText;
    public Color black;
    public Color white;
    public float popupDuration;

    // интснас
    public static M3Gui Instance;
    public static M3Gui GetInstance() => Instance;

    // авэйк
    private void Awake()
    {
        Instance = this;
    }

    // гуай в M3
    public static void ShowPopup(string text)
    {
        // активируем
        GetInstance().m_popupText.gameObject.SetActive(true);

        // текст
        GetInstance().m_popupText.text = text;

        // твин
        DOTween.Sequence()
            .Append(
                GetInstance().m_popupText.DOColor(
                    GetInstance().white,
                    GetInstance().popupDuration
                )
            )
            .Append(
                GetInstance().m_popupText.DOColor(
                    GetInstance().black,
                    GetInstance().popupDuration
                )
            )
            .OnComplete(() => {
                // деактивируем
                GetInstance().m_popupText.gameObject.SetActive(false);
            });
    }
}