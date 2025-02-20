// попап <<Нет жизней>>
using UnityEngine;

public class LevelNoLifesPopup : MonoBehaviour
{
    // попап
    [SerializeField] private Popup popup;
    
    // показ и скрытие попапа
    public void Show() => popup.Show();
    public void Hide() => popup.Hide();
    
    // цена
    [SerializeField]
    private int price;
    
    // поап уровня
    [SerializeField]
    private Popup restartPopup;

    // покупка жизней
    public void BuyLifes()
    {
        int userCoins = (int)StorageService.GetInstance().Get<int>("coins");
        if (userCoins >= price)
        {
            // выдаем жизни
            StorageService.GetInstance().Save<int>("lifes", 5);
            // забираем койны
            StorageService.GetInstance().Save<int>("coins", userCoins - price);
            // прячем текущий попап
            popup.Hide();
            // показываем попап уровня
            restartPopup.Show();
        }
    }
    
    // выход из попапа
    public void Exit()
    {
        // прячем текущий попап
        popup.Hide();        
        // показываем попап уровня
        restartPopup.Show();
    }
}