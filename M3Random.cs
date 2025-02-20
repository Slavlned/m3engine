using TMPro;
using UnityEngine;

public class M3Random : MonoBehaviour
{
    // min, max
    public int min = 0, max = 4;
    // seed
    // по дефолту: 123456
    public int seed = -1331403679;

    // инстанс
    public static M3Random Instance;
    public static M3Random GetInstance() => Instance;
    
    // текст отображения сида
    [SerializeField] private TMP_Text seedText;

    // инициализация рандомизатора
    public void Init()
    {
        // получаем сид из времени
        seed = (int) GameTime.CurrentTimeMillis();
        // инициализация рандома
        Random.InitState(seed);
        // синглтон
        Instance = this;
        // устанавливаем текст
        seedText.text = "seed: " + seed.ToString();
    }

    // рандомное число
    public int GetRandom(int _min, int _max)
    {
        return Random.Range(_min, _max);
    }

    // рандомное число для чипа
    public int GetRandomChip()
    {
        return Random.Range(min, max);
    }
}