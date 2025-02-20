using System.Collections.Generic;
using UnityEngine;

public class M3Prefabs : MonoBehaviour
{
    // префабы
    public List<M3Prefab> prefabs;
    public GameObject tilePrefab;
    // инстанс
    public static M3Prefabs Instance;
    public static M3Prefabs GetInstance() => Instance;

    // авэйк
    public void Init()
    {
        // ставим инстанс
        Instance = this;
    }

    // рандомный чип
    public M3Prefab RandomChip()
    {
        return prefabs[M3Random.GetInstance().GetRandomChip()];
    }

    // по ключу
    public GameObject ByKey(M3ObjectType key)
    {
        return prefabs.Find(prefab => prefab.Key == key).Value;
    }
}