using UnityEngine;

public class M3Tests : MonoBehaviour
{
    // тесты
    [SerializeField]
    private M3Board board;

    // запуск тестов
    public void OnEnable()
    {
        board.onIdle += OnIdle;
    }

    public void OnDisable()
    {
        board.onIdle -= OnIdle; 
    }

    public void OnIdle()
    {
        // Debug.Log("Idle event sub test succesfuly passed!");
    }
}