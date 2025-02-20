using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// компасс в редакторе
public class M3EditorTypeFocusHat : MonoBehaviour
{
    // объект
    [SerializeField]
    private M3EditorObj obj;
    // длительность
    [SerializeField] private float duration = 0.1f;
    // спрайты направлений
    [SerializeField] private Sprite withBunnySprite;
    
    public void Awake()
    {
        StartCoroutine(SetRotation());
    }

    private IEnumerator SetRotation()
    {
        // выжидаем
        yield return new WaitForSeconds(duration);

        // если есть направление
        if (obj.parameters.Find(elem => elem.Name == "has_bunny") != null)
        {
            // устанавливаем
            bool withbunny = bool.Parse(obj.parameters.Find(elem => elem.Name == "has_bunny").Value);
            // если с кроликом то ставим спрайт
            if (withbunny) {
                obj.GetComponent<SpriteRenderer>().sprite = withBunnySprite;
            }
        }
    }
}