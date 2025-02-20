using System.Collections.Generic;
using UnityEngine;

public class M3EditorPrefabs : MonoBehaviour
{
    // префабы редактора
    public static M3EditorPrefabs Instance;
    public static M3EditorPrefabs GetInstance() => Instance;

    [SerializeField]
    // префабы
    private List<M3Prefab> prefabs = new List<M3Prefab>();

    [SerializeField]
    // префаб клетки
    public GameObject cellPrefab;
    [SerializeField]
    // префаб генератора
    public GameObject generatorPrefab;
    [SerializeField]
    // префаб гравитационной стрелки
    public GameObject gravityArrow;
    [SerializeField]
    // префаб делверера
    public GameObject delivererPrefab;

    // получаем префаб по ключу
    public static GameObject ByKey(M3ObjectType type) => Instance.prefabs.Find(o => o.Key == type).Value;

    // инстанс
    private void Awake()
    {
        Instance = this;
    }
}