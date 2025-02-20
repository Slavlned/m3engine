using TMPro;
using UnityEngine;

public class M3BoosterButton : MonoBehaviour
{
    // кнопка бустера
    [SerializeField]
    private M3BoosterType type;
    
    // бустеры
    [SerializeField] private M3Boosters boosters;
    
    // анимация
    [SerializeField] private Animator boosterAnimator;

    // получение аниматора
    public Animator GetAnimator() => boosterAnimator;
    
    // активация
    public void Activate()
    {
        // если поле в состоянии покоя
        if (boosters.m3Board.FullIdle() && !boosters.m3Board.GetLock() && boosters.m3Board.GetGravity())
        {
            // включаем анимацию
            boosterAnimator.Play("ButtonBlink");

            // выбираем бустер
            boosters.Select((int)type, this);            
        }
    }
}