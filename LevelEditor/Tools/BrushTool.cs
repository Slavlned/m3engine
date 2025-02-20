// кисть
using UnityEngine;

public class BrushTool : M3EditorTool
{
    // эдитор
    private M3Editor editor;

    // констуктор
    public BrushTool(M3Editor editor)
    {
        this.editor = editor;
    }

    // тул
    public override object Use(int x, int y, M3ObjectType type)
    {
        // устанавливаем объект
        editor.SetAt(x, y, type);

        // возвращаем ничего
        return null;
    }
}