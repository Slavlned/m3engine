using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M3EditorInputs : MonoBehaviour {
    // инпуты редактора
    [SerializeField]
    private TMP_InputField width;
    [SerializeField]
    private TMP_InputField height;
    [SerializeField]
    private TMP_InputField fileName;
    [SerializeField]
    private TMP_InputField moves;
    [SerializeField]
    private TMP_InputField xOffset;
    [SerializeField]
    private TMP_InputField yOffset;
    [SerializeField]
    private TMP_Dropdown editFileName;
    [SerializeField]
    private List<M3EditorGoal> goals;

    // геттеры
    public int Width() => Convert.ToInt32(width.text);
    public int Height() => Convert.ToInt32(height.text);
    public float XOffset() => float.Parse(xOffset.text);
    public float YOffset() => float.Parse(yOffset.text);
    public string FileName() => fileName.text;
    public string EditFileName() => editFileName.options[editFileName.value].text;
    public int Moves() => Convert.ToInt32(moves.text);
    public List<M3Goal> Goals()
    {
        // цели
        List<M3Goal> _goals = new List<M3Goal>();

        // перебераем
        foreach (var goalText in goals)
        {
            M3Goal goal = new M3Goal();
            String value = goalText.GetValue();
            if (value != "")
            {
                goal.amount = Convert.ToInt32(value);
                goal.type = goalText.GetM3Type();
                _goals.Add(goal);
            }
        }

        // возвращаем
        return _goals;
    }
    public void SetDropDown(List<string> files)
    {
        // опции
        List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
        // файлы
        foreach (var file in files)
        {
            optionDatas.Add(new TMP_Dropdown.OptionData(file));
        }
        // добавляем
        editFileName.AddOptions(optionDatas);
    }

    public void SetInputs(M3Editor editor, String filename)
    {
        // параметры
        this.width.text = editor.editingLevel.width.ToString();
        this.height.text = editor.editingLevel.height.ToString();
        this.moves.text = editor.editingLevel.moves.ToString();
        this.fileName.text = filename;
        this.xOffset.text = editor.editingLevel.boardOffset.x.ToString();
        this.yOffset.text = editor.editingLevel.boardOffset.y.ToString();

        // цели
        foreach (M3Goal goal in editor.editingLevel.recipe.goals)
        {
            goals.Find(_goal => _goal.GetM3Type() == goal.type).SetValue(goal.amount.ToString());
        }
    }

    public void ResetGoals()
    {
        // перебираем и обнуляем
        goals.ForEach(goal => goal.SetValue(""));
    }
}