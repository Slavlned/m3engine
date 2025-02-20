using System;
using TMPro;
using UnityEngine;

// интерфейс жизней
public class LifesUI : MonoBehaviour, Service
{
    // текст
    [SerializeField]
    private TMP_Text text;
    
    // сервис жизней
    [SerializeField]
    private LifesService lifesService;
    
    // awake
    public void Initialize(Service from)
    {
        lifesService = LifesService.GetInstance();
    }

    // обновление
    private void Update()
    {
        // текст
        text.text = lifesService.GetLifesFormat();
        text.fontSize = lifesService.GetFontSize();
    }
}