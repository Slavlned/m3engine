// кнопки m3
using UnityEngine;

public class M3Buttons : MonoBehaviour
{
    // попап нет жизней
    [SerializeField] private LevelNoLifesPopup noLifesPopup;
    // доска
    [SerializeField] private M3Board board;
    
    // в меню
    public void Menu()
    {
        ScenesService.GetInstance().Load("MenuScene");
    }

    // новый уровень
    public void NextLevel()
    {
        int level = (int)StorageService.GetInstance().Get<int>("level");
        if (level < ServerService.GetInstance().GetLevels().Count)
        {
            ScenesService.GetInstance().Load("M3Scene");
        }
        else
        {
            ScenesService.GetInstance().Load("M3Finish");
        }
    }

    // перезапуск
    public void RestartLevel()
    {
        int lifes = (int)StorageService.GetInstance().Get<int>("lifes");
        if (lifes > 0)
        {
            if (board.editorMode)
            {
                StorageService.GetInstance().Save<bool>("is_editor_level", true);
            }
            ScenesService.GetInstance().Load("M3Scene");
        }
        else
        {
            noLifesPopup.Show();
        }
    }    
}