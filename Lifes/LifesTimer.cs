// таймер жизней

using System;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;

public class LifesTimer : MonoBehaviour
{
    // время до следующей жизни
    public long nextLifeTime = 0L;
    
    // инициализовано ли
    [SerializeField] private bool isInitialized;
    
    // время для одной жизни
    [SerializeField] private long oneLifeTime = 600000L;
    
    // сервис жизней
    [SerializeField] private LifesService lifesService;
    
    // текст таймера
    [SerializeField] private TMP_Text timerText;
    
    // запуск таймера
    public void StartTimer(int lifes)
    {
        // если жизней уже 5 то ничего не делаем,
        // в ином же случае начинаем отсчет 
        if (lifes < 5)
        {
            // если нет сохранений
            if (!PlayerPrefs.HasKey("lifes_timer"))
            {
                nextLifeTime = GameTime.CurrentTimeMillis() + oneLifeTime;
                StorageService.GetInstance().Save<long>("lifes_timer", nextLifeTime);
            }
            else
            {
                nextLifeTime = (long) StorageService.GetInstance().Get<long>("lifes_timer");
            }
        }
        else
        {
            nextLifeTime = GameTime.CurrentTimeMillis() + oneLifeTime;
            StorageService.GetInstance().Save<long>("lifes_timer", nextLifeTime);
        }
        isInitialized = true;
    }
    
    // таймер
    public void Update()
    {
        // если не инициализовано
        if (!isInitialized)
        {
            return;
        }
        /*
        GameDebugger.Log("Current MS: " + GameTime.CurrentTimeMillis());
        GameDebugger.Log("Next Life Time: " + nextLifeTime);
        int tempLifes = (int)((GameTime.CurrentTimeMillis() - nextLifeTime) / oneLifeTime) + 1;
        GameDebugger.Log("Lifes: " + tempLifes);
        */
        // проверяем время
        if (nextLifeTime <= GameTime.CurrentTimeMillis())
        {
            // жизней для выдачи
            long timeDifference = GameTime.CurrentTimeMillis() - (nextLifeTime-oneLifeTime);
            int lifes = (int) (timeDifference/oneLifeTime) + 1;
            // выдаем жизни
            lifesService.GiveLifes(lifes);
            // обновляем время
            nextLifeTime = GameTime.CurrentTimeMillis() + oneLifeTime;
            StorageService.GetInstance().Save<long>("lifes_timer", nextLifeTime);
        }
        // текст таймера
        if (!lifesService.IsInfiniteLifes() && lifesService.GetLifes() < 5)
        {
            if (!timerText.gameObject.activeSelf)
            {
                timerText.gameObject.SetActive(true);
            }
            timerText.text = FormatTimeMills(nextLifeTime-GameTime.CurrentTimeMillis());
        }
        else
        {
            timerText.gameObject.SetActive(false);
        }
    }

    // форматирование миллисекунд в дн. час. мин. сек
    private string FormatTimeMills(long currentTimeMillis)
    {
        // форматируем время
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(currentTimeMillis);
        return $"{timeSpan.Hours.ToString()}:{timeSpan.Minutes.ToString()}:{timeSpan.Seconds.ToString()}";
    }    
}