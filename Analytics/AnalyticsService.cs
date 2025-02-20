using UnityEngine;
using UnityEngine.Networking;

// аналитика игры
public class AnalyticsService : MonoBehaviour, Service
{
    // api key
    [SerializeField] public string apiKey = "WARBalYeku8FmHd";
        
    // синглтон
    public static AnalyticsService Instance;
    public static AnalyticsService GetInstance() => Instance;
    
    // инициализация
    public void Intialize(Service from)
    {
        // чекаем инстанс на нулл
        if (Instance == null)
        {
            // запрещаем уничтожения при загрузке сцены
            DontDestroyOnLoad(this.gameObject);
            // синглтон
            Instance = this;
        }
        // эксцепшен
        else
        {
            // уничтожаем
            Destroy(this.gameObject);
        }
    }
    
    // отправка аналитики
    public void Send(AnalyticType type, string key)
    {
        // закидываем аналитику на сервер
        if (!ServerService.GetInstance().IsDevBuild())
        {
            ServerService.GetInstance().PostAnalytics(apiKey, type, key);
        }
    }
}