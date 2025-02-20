using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class M3GoalUI : MonoBehaviour
{
    // цель
    private M3Goal goal = null;

    // колличество
    [SerializeField]
    private TMP_Text amount;

    // изображение
    [SerializeField]
    private Image image;

    // ui результата
    public M3ResultUI resultUI;


    // инициализация
    public void InitUI(M3Goal _goal)
    {
        // устанавливаем цель
        this.goal = _goal;

        // запускаем ui
        if (goal != null)
        {
            gameObject.SetActive(true);

            image.sprite = resultUI.GetImage(goal.type);
            amount.text = goal.amount.ToString();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // обновление
    public void UpdateUI()
    {
        if (goal != null)
        {
            amount.text = goal.amount.ToString();
        }
    }
}