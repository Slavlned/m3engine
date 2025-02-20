using System.Collections.Generic;
using UnityEngine;

/*
Бонусы на старте игры
*/
public class M3Bonuses : MonoBehaviour
{
    // доска
    [SerializeField] private M3Board board;
    
    // fx
    [SerializeField] private M3Fx fx;
    
    // спавн бонусов
    public void SpawnBonuses(List<M3ObjectType> types)
    {
        // если ничего нет
        if (types == null || types.Count == 0)
        {
            return;
        }
        // перебераем типы
        foreach (M3ObjectType t in types)
        {
            SpawnBonus(t);
        }
    } 
    
    // спавн бонуса
    public void SpawnBonus(M3ObjectType t)
    {
        // выбор рандомного лёгкого тайла
        // там где будет фишка
        M3Tile tile = board.ChooseRandomChipTile();
        if (tile == null)
        {
            GameDebugger.LogE("No tile found for bonus: " + t);
        }
        tile.ReplaceTo(t);
        // эффект
        fx.SpawnFx(tile.Pos(), "BONUS_SPAWN_FX");
    }
    
    // загрузка
    public static List<M3ObjectType> Load()
    {
        // возвращаем бустеры
        if (PlayerPrefs.HasKey("additional_bonuses"))
        {
            return JsonUtility.FromJson<List<M3ObjectType>>(
                (string)StorageService.GetInstance().Get<string>("additional_bonuses"));
        }
        else
        {
            return new List<M3ObjectType>();
        }
    }
    
    // установка
    public static void Set(List<M3ObjectType> types)
    {
        StorageService.GetInstance().Save<string>("additional_bonuses", JsonUtility.ToJson(types));
    }    
}