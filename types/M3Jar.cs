using System;
using System.Collections;
using UnityEngine;

// варенье
public class M3Jar : MonoBehaviour
{
    [SerializeField]
    // объект
    private M3Object m_Object;

    // ивенты
    private void OnEnable()
    {
        m_Object.onFallEnded += OnFallEnded;
        m_Object.onSwapDone += OnSwapDone;
    }

    // ивенты
    private void OnDisable()
    {
        m_Object.onFallEnded -= OnFallEnded;
        m_Object.onSwapDone -= OnSwapDone;
    }

    // когда упал
    private void OnFallEnded()
    {
        // если объект деливерер
        if (m_Object.GetTile().IsDeliverer)
        {
            // эффект
            m_Object.GetBoard().GetFx().SpawnFx(m_Object.GetTile().Pos(), "M3_OBJECT_JAR");             
            // уничтожаем объект
            m_Object.GetBoard().DestroyObject(m_Object, M3DestroyType.NOCALL);
            m_Object.GetBoard().GetResult().UpdateGoal(M3ObjectType.JAR, 1);
        }
    }

    // когда свапнулся
    private void OnSwapDone(M3Object first, M3Object second)
    {
        // короутина
        StartCoroutine(Del());
    }

    // короутина удаление
    private IEnumerator Del()
    {
        // выжидаем тик
        yield return new WaitForSeconds(0.1f);
        // если объект деливерер
        if (m_Object.GetTile().IsDeliverer)
        {
            // эффект
            m_Object.GetBoard().GetFx().SpawnFx(m_Object.GetTile().Pos(), "M3_OBJECT_JAR");            
            // уничтожаем объект
            m_Object.GetBoard().DestroyObject(m_Object, M3DestroyType.NOCALL);
            m_Object.GetBoard().GetResult().UpdateGoal(M3ObjectType.JAR, 1);
        }
    }
}