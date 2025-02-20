using System;
using UnityEngine;

// бублик
public class M3Donut : MonoBehaviour
{
    [SerializeField]
    // объект
    private M3Object m_Object;

    // ивенты
    private void OnEnable()
    {
        m_Object.onCall += OnCall;
        m_Object.onDamageFailure += OnDamageFailure;
    }

    // ивенты
    private void OnDisable()
    {
        m_Object.onCall -= OnCall;
        m_Object.onDamageFailure -= OnDamageFailure;
    }

    // когда вызван
    private void OnCall(M3Object from)
    {
        // эффект
        m_Object.GetBoard().GetFx().SpawnFx(m_Object.GetTile().Pos(), "M3_OBJECT_DONUT");         
        // уничтожаем объект
        m_Object.GetBoard().DestroyObject(m_Object, M3DestroyType.NOCALL);
        m_Object.GetBoard().GetResult().UpdateGoal(M3ObjectType.DONUT, 1);
    }

    // когда дестрой окончен
    private void OnDamageFailure()
    {
        // эффект
        m_Object.GetBoard().GetFx().SpawnFx(m_Object.GetTile().Pos(), "M3_OBJECT_DONUT");        
        // уничтожаем объект
        m_Object.GetBoard().DestroyObject(m_Object, M3DestroyType.NOCALL);
        m_Object.GetBoard().GetResult().UpdateGoal(M3ObjectType.DONUT, 1);
    }
}