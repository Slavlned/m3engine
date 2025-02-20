// попап проигрыша

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class M3LosePopup : MonoBehaviour
{
    // поап
    [SerializeField]
    private Popup popup;

    // спрайты
    [SerializeField] private Sprite infiniteLifes;
    [SerializeField] private Sprite normalLifes;
    
    // image
    [SerializeField]
    private Image lifesImage;
    
    // тайтл
    [SerializeField] private TMP_Text title;
    
    // установка тайтла
    public void SetTitle(string text)
    {
        this.title.text = text;
    }
    
    // установка и показ поапа
    public void Show()
    {
        // спрайт жизней
        lifesImage.sprite = GetLifesImage();
        // показ поапа
        popup.Show();
    }
    
    // прячем поап
    public void Hide()
    {
        // скрытие поапа
        popup.Hide();
    }
    
    // получаем изображение жизней
    private Sprite GetLifesImage()
    {
        var infinite_lifes = (long) StorageService.GetInstance().Get<long>("infinite_lifes");
        if (infinite_lifes <= GameTime.CurrentTimeMillis())
        {
            return normalLifes;
        }
        else
        {
            return infiniteLifes;
        }
    }
}