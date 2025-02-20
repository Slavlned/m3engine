using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// компасс в редакторе
public class M3EditorTypeGlobe : MonoBehaviour
{
    // текст
    [SerializeField] private TMP_Text globeText;
    // объект
    [SerializeField]
    private M3EditorObj obj;
    // длительность
    [SerializeField] private float duration = 0.1f;

    public void Awake()
    {
        StartCoroutine(SetRotation());
    }

    private IEnumerator SetRotation()
    {
        // выжидаем
        yield return new WaitForSeconds(duration);

        // если есть направление
        if (obj.parameters.Find(elem => elem.Name == "stage") != null)
        {
            // устанавливаем
            int stage = int.Parse(obj.parameters.Find(elem => elem.Name == "stage").Value);
            globeText.text = stage.ToString();
        }
    }
}