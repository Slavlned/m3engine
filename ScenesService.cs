using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesService : MonoBehaviour, Service
{
    // Сервис смены сцен

    // Инстанс
    public static ScenesService Instance;
    public static ScenesService GetInstance() => Instance;

    [SerializeField]
    // партиклы
    private List<ParticleSystem> particles = new List<ParticleSystem>();

    [SerializeField]
    // время перехода
    private float transitionTime;

    [SerializeField]
    // имя сцены загрузки
    private string loadingScene = "LoadingScene";

    // ивент загрузки сцены
    public event Action onSceneChanging;

    // ивент конца загрузки сцену
    public event Action onSceneChanged;
    // загружается ли уже какая-либо сцена
    private bool isLoading;
    
    // Ивенты
    public void OnEnable()
    {
        // ивент
        SceneManager.sceneLoaded += OnSceneLoad;
        // инициализация
        Initialize(null);
    }

    public void OnDisable()
    {
        // ивент
        SceneManager.sceneLoaded -= OnSceneLoad; 
    }

    // Канвас
    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    // Загрузка сцены
    public void Load(string name)
    {
        // если уже загружается, повторно не грузим
        if (isLoading)
        {
            return;
        }
        // вызываем событие
        onSceneChanging?.Invoke();
        // да, загружаем
        isLoading = true;
        // партиклы
        foreach(ParticleSystem particle in particles)
        {
            particle.Play();
        }
        // короутина
        StartCoroutine(LoadCoroutine(name));
    }

    // короутина загрузки
    private IEnumerator LoadCoroutine(string name)
    {
        // ждём время
        yield return new WaitForSeconds(transitionTime);
        // проверяем на тех обслуживание
        if (!ServerService.GetInstance().IsDevBuild()) {
            yield return ServerService.GetInstance().StartCoroutine(
                ServerService.GetInstance().CheckMaintenance()
            );
        }
        // если тех обслуживание ставим сцену
        // на тех обслуживание
        if (!ServerService.GetInstance().IsDevBuild() && ServerService.GetInstance().Maintenance)
        {
            name = loadingScene;
        }        
        // загружаем сцену
        SceneManager.LoadSceneAsync(name);
        // ждем время
        yield return new WaitForSeconds(transitionTime/1.2f);
        // вызываем событие
        onSceneChanged?.Invoke();
        // обновляем состояние
        isLoading = false;
    }

    // Инициализатор
    public void Initialize(Service from)
    {
        // Синглтон
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
    }
}