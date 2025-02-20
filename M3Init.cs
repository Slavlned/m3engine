using System;
using System.Collections.Generic;
using M3;
using UnityEngine;

public class M3Init : MonoBehaviour
{
    [SerializeField]
    // m3 fx
    public M3Fx m3Fx;

    [SerializeField]
    // m3 рандом
    public M3Random m3Random;

    [SerializeField]
    // m3 префабы
    public M3Prefabs m3Prefabs;

    [SerializeField]
    // m3 доска
    public M3Board m3Board;

    [SerializeField]
    // m3 результат
    public M3Result m3Result;
    
    [SerializeField]
    // gui при входе[
    private M3EnterPopup m3EnterPopup;

    [SerializeField]
    // уровень
    private M3Level m3Level;

    // когда сцена загружена
    private void OnEnable()
    {
        ScenesService.GetInstance().onSceneChanged += Initialize;
    }
    
    private void OnDisable()
    {
        ScenesService.GetInstance().onSceneChanged -= Initialize;
    }

    private void Initialize()
    {
        // загружаем уровень
        LoadLevel();
        // показываем попап
        if (m3Board.editorMode)
        {
            m3EnterPopup.Show("ЭДИТОР");
        }
        else
        {
            m3EnterPopup.Show((m3Level.level).ToString());            
        }
        // инициализация эффекты
        m3Fx.Init();
        // инициализация рандома
        m3Random.Init();
        // инициализация префабов
        m3Prefabs.Init();
        // инициализация доски
        List<M3ObjectType> bonuses = M3Bonuses.Load(); 
        m3Board.Init(m3Level, bonuses);
        // инициализация системы результата
        m3Result.Init(m3Level);
        // тайм скейл
        // Time.timeScale = 0.05f;
    }

    private void LoadLevel()
    {
        // уровень
        bool fromEditor = false;
        if (StorageService.GetInstance() != null)
        {
            fromEditor = (bool)StorageService.GetInstance().Get<bool>("is_editor_level");
        }

        // если из редактора -> загружаем
        if (fromEditor)
        {
            // говорим что уровень
            // нужно загрузить в режиме редактора
            m3Board.editorMode = true;
            // загружаем
            string level = (string)StorageService.GetInstance().Get<string>("editor_level");
            m3Level = new M3EditorJson().Load(level);
            // далее будет
            // не эдиторский
            // уровень
            StorageService.GetInstance().Save<bool>("is_editor_level", false);
            // лог
            GameDebugger.Log("Level: editor");
        }
        // если не из редактора -> загружаем
        // текущий уровень
        else
        {
            // говорим что уровень
            // нужно загрузить в обычном режиме
            m3Board.editorMode = false;            
            // если хранилище существует
            if (StorageService.GetInstance() != null)
            {
                // загружаем
                int level = (int)StorageService.GetInstance().Get<int>("level");
                m3Level = ServerService.GetInstance().GetLevels()[level];
                // лог
                GameDebugger.Log("Level: " + (level+1));
            }
        }
    }
}