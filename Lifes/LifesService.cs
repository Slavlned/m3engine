// жизни

using System;
using UnityEngine;

public class LifesService : MonoBehaviour, Service
{
    // инстанс
    public static LifesService Instance;
    public static LifesService GetInstance() => Instance;
    
    // размеры шрифтов
    [SerializeField] private float lifesAmountFontSize = 50f;
    [SerializeField] private float infiniteLifesFontSize = 26f;

    // таймер
    [SerializeField] private LifesTimer timer;
    
    // инициализации
    public void Initialize(Service from)
    {
        // инстанс
        Instance = this;
        // ивенты
        ScenesService.GetInstance().onSceneChanging += Save;
        // загружаем сохранения
        Load();  
        // инциализируем таймер
        timer.StartTimer(lifes);
    }
    
    // жизни
    [SerializeField]
    private long infiniteLifes;
    [SerializeField]
    private int lifes;
    
    // при выходе из приложения
    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnDisable()
    {
        ScenesService.GetInstance().onSceneChanging -= Save;
    }
    
    // сохранение
    private void Save()
    {
        // сохранения
        PlayerPrefs.SetInt("lifes", lifes);
        PlayerPrefs.SetString("infinite_lifes", infiniteLifes.ToString());
    }
    
    // загрузка
    public void Load()
    {
        // жизни
        if (PlayerPrefs.HasKey("lifes"))
        {
            // если есть жизни в сохранках загружаем
            lifes = (int) StorageService.GetInstance().Get<int>("lifes");
        }
        else
        {
            // в ином случае создаем сохраненку с 5-ю жизнями и загружаем
            StorageService.GetInstance().Save<int>("lifes", 5);
            lifes = (int) StorageService.GetInstance().Get<int>("lifes");
        }
        // бексконечные жизни
        infiniteLifes = (long) StorageService.GetInstance().Get<long>("infinite_lifes");
    }

    // получение размера шрифта по формату жизней
    public float GetFontSize()
    {
        // если время жизней истекло
        if (infiniteLifes <= GameTime.CurrentTimeMillis())
        {
            return lifesAmountFontSize;
        }
        else
        {
            return infiniteLifesFontSize;
        }
    }
    
    // выдача жизней
    public void GiveLifes(int _lifes)
    {
        // лог
        GameDebugger.Log("Lifes: giving lifes (" + _lifes.ToString() + ")");
        // если жизней больше 5, то выдаем
        if (_lifes + lifes > 5)
        {
            StorageService.GetInstance().Save("lifes", 5);
            this.lifes = 5;
        }
        else
        {
            StorageService.GetInstance().Save("lifes", this.lifes + _lifes);
            this.lifes += _lifes;
        }
    }
    
    // можно ли играть в уровень ( проверяет на наличие жизней )
    public bool CanPlayLevel()
    {
        // если бесконечные жизни, то играть можно
        if (IsInfiniteLifes())
        {
            return true;
        }
        // если жизней больше 0, то играть можно
        if (lifes > 0)
        {
            return true;
        }
        
        // в иных случаях играть нельзя
        return false;
    }

    // получение отформатированных жизней
    public string GetLifesFormat()
    {
        // если время жизней истекло
        if (infiniteLifes <= GameTime.CurrentTimeMillis())
        {
            return $"{lifes.ToString()}";
        }
        else
        {
            return $"{FormatTimeMills(infiniteLifes-GameTime.CurrentTimeMillis())}";
        }
    }    

    // форматирование миллисекунд в дн. час. мин. сек
    private string FormatTimeMills(long currentTimeMillis)
    {
        // форматируем время
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(currentTimeMillis);
        return $"{timeSpan.Hours.ToString()}:{timeSpan.Minutes.ToString()}:{timeSpan.Seconds.ToString()}";
    }
    
    // бесконечные ли жизни
    public bool IsInfiniteLifes() => infiniteLifes > GameTime.CurrentTimeMillis();

    // получение жизней
    public int GetLifes()
    {
        return lifes;
    }
}