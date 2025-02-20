// попап <<Нет жизней>>
using UnityEngine;

public class NoLifesPopup : MonoBehaviour
{
    // попап
    [SerializeField] private Popup popup;
    
    // показ и скрытие попапа
    public void Show() => popup.Show();
    public void Hide() => popup.Hide();
    
    // цена
    [SerializeField]
    private int price;

    // сервис жизней
    [SerializeField]
    private LifesService lifes;
    
    // поап уровня
    [SerializeField]
    private LevelPopup levelPopup;
    
    // сервис меню
    [SerializeField]
    private MenuService menuService;
    
    // покупка жизней
    public void BuyLifes()
    {
        int userCoins = (int)StorageService.GetInstance().Get<int>("coins");
        if (userCoins >= price)
        {
            // выдаем жизни
            lifes.GiveLifes(5);
            // забираем койны
            StorageService.GetInstance().Save<int>("coins", userCoins - price);
            // прячем текущий попап
            popup.Hide();
            // обновление текста монет
            menuService.UpdateCurrencies();
            // показываем попап уровня
            levelPopup.ShowPopup();
        }
    }
}