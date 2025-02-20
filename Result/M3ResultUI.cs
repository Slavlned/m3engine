using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M3ResultUI : MonoBehaviour
{
    // ui результата

    // текст для ходов
    [SerializeField]
    private TMP_Text movesText;
    // изображение цели
    [SerializeField]
    private List<M3GoalImage> goalImages;
    // ui цели
    [SerializeField]
    private List<M3GoalUI> goalUiList;
    // система результата
    [SerializeField]
    private M3Result result;
    // панель для выигрыша
    [SerializeField]
    private M3WinPopup winPopup;
    // панель для проигрыша
    [SerializeField]
    private M3LosePopup losePopup;

    // получение изображения
    public Sprite GetImage(M3ObjectType key) => goalImages.Find(goal => goal.Key == key).Value;

    // инициализация UI
    public void InitUI()
    {
        // обновляем ходы
        movesText.text = result.GetMoves().ToString();
        // обновляем рецепт
        for (int i = 0; i < result.GetRecipe().goals.Count; i++)
        {
            goalUiList[i].InitUI(result.GetRecipe().goals[i]);
        }
    }

    // обновление UI
    public void UpdateUI()
    {
        // обновляем ходы
        movesText.text = result.GetMoves().ToString();
        // обновляем рецепт
        goalUiList.ForEach(goalUi => goalUi.UpdateUI());
    }

    // показываем панель проигрыша
    public void ShowLosePanel()
    {
        // показываем
        losePopup.SetTitle(result.GetBoard().GetLevelName());
        losePopup.Show();
    }

    // показываем панель выигрыша
    public void ShowWinPanel()
    {
        // показываем
        winPopup.SetTitle(result.GetBoard().GetLevelName());
        winPopup.Show(result.GetMoves());
    }
}