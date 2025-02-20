// кисть кавера
using UnityEngine;

public class CoverBrushTool : M3EditorTool
{
    // эдитор
    private M3Editor editor;

    // констуктор
    public CoverBrushTool(M3Editor editor)
    {
        this.editor = editor;
    }

    // тул
    public override object Use(int x, int y, M3ObjectType type)
    {
        // устанавливаем кавер 
        editor.SetCoverAt(x, y, type);
        
        // возвращаем ничего
        return null;
    }
}