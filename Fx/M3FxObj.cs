using System.Collections;
using UnityEngine;

public class M3FxObj : MonoBehaviour
{
    // fx объект
    // уничтожает объект после проигрывания эффекта
    [SerializeField]
    private float duration;
    // ожидение
    [SerializeField]
    private WaitForSeconds waitForSeconds;

    // awake
    private void Awake()
    {
        // ожидание
        waitForSeconds = new WaitForSeconds(duration);
        // если эффект не бесконечный, запускаем короутину
        if (duration != -1) { StartCoroutine(DurationCoroutine()); }
    }

    // короутина выжидания
    private IEnumerator DurationCoroutine()
    {
        // ждем
        yield return waitForSeconds;
        // удаляем эффект
        Destroy(this.gameObject);
    }
}