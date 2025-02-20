using UnityEngine;

/*
 * Кавер мёда
 */
public class M3CoverHoney : MonoBehaviour
{
    [SerializeField]
    private M3Cover cover;

    private void OnEnable()
    {
        cover.onCoverDamage += OnCoverDamage;
    }
    
    private void OnDisable()
    {
        cover.onCoverDamage -= OnCoverDamage;
    }
    
    // когда прошелся дамаг по каверу
    private void OnCoverDamage()
    {
        // цели
        cover.GetBoard().GetResult().UpdateGoal(M3ObjectType.COVER_HONEY, 1);
        // уничтожаем
        Destroy(cover.gameObject);
    }
}