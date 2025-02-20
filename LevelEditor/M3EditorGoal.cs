using TMPro;
using UnityEngine;

public class M3EditorGoal : MonoBehaviour {
    // цель в эдиторе
    [SerializeField] private TMP_InputField value;
    [SerializeField] private M3ObjectType type;

    // геттеры
    public string GetValue() => value.text;
    public M3ObjectType GetM3Type() => type;
    public void SetValue(string amount) => value.text = amount;
}