using UnityEngine;

[System.Serializable]
public class M3LevelFinishResult
{
    // результат окончания уровня
    public M3GameState result;
    
    // конструктор
    public M3LevelFinishResult(M3GameState result)
    {
        this.result = result;
    }
}