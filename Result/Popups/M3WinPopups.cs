// попап проигрыша

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class M3WinPopup : MonoBehaviour
{
    // поап
    [SerializeField]
    private Popup popup;
    
    // текст монет
    [SerializeField]
    private TMP_Text coinsText;
    
    // тайтл
    [SerializeField] private TMP_Text title;
    
    // установка тайтла
    public void SetTitle(string text)
    {
        this.title.text = text;
    }
    
    // установка и показ попапа
    public void Show(int moves)
    {
        // устанавливаем монеты
        coinsText.text = ((moves+10) * 2).ToString();
        // показываем попап
        popup.Show();
    }
    
    // прячем поап
    public void Hide()
    {
        // скрытие поапа
        popup.Hide();
    }    
}