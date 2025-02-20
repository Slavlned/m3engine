using System.Collections;
using UnityEngine;

public class GravityTool : M3EditorTool
{
    // эдитор
    private M3Editor editor;

    // констуктор
    public GravityTool(M3Editor editor)
    {
        this.editor = editor;
    }

    // тул
    public override object Use(int x, int y, M3ObjectType type)
    {
        // устанавливаем объект
        editor.Gravity(x, y);

        // возвращаем ничего
        return null;
    }
}