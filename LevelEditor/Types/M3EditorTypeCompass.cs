using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// компасс в редакторе
public class M3EditorTypeCompass : MonoBehaviour
{
    // объект
    [SerializeField]
    private M3EditorObj obj;
    // длительность
    [SerializeField] private float duration = 0.1f;
    // спрайты направлений
    [SerializeField] private List<Sprite> dirs;
    
    public void Awake()
    {
        StartCoroutine(SetRotation());
    }

    private IEnumerator SetRotation()
    {
        // выжидаем
        yield return new WaitForSeconds(duration);

        // если есть направление
        if (obj.parameters.Find(elem => elem.Name == "dir") != null)
        {
            // устанавливаем
            int dir = int.Parse(obj.parameters.Find(elem => elem.Name == "dir").Value);
            obj.GetComponent<SpriteRenderer>().sprite = dirs[dir];
        }
    }
}