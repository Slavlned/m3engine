using UnityEngine;
using System;

[SerializeField]
public class LevelPostEvents : MonoBehaviour, Service
{
    // хэндлит события для ивентов
    // [SerializeField] private Event ...;
    public Action<M3LevelFinishResult> onLevelPlayed;

    public void Initialize(Service from)
    {
        // если игрок завершил уровень
        if (PlayerPrefs.HasKey("level_finish")) {
            // вызываем ивент
            onLevelPlayed?.Invoke(StorageService.GetInstance().Get<M3LevelFinishResult>("level_finish") as M3LevelFinishResult);
            // удаляем результат прохождения уровня
            PlayerPrefs.DeleteKey("level_finish");
        }
    }
}