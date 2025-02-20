// кисть
public class CellBrushTool : M3EditorTool
{
    // эдитор
    private M3Editor editor;

    // констуктор
    public CellBrushTool(M3Editor editor)
    {
        this.editor = editor;
    }

    // тул
    public override object Use(int x, int y, M3ObjectType type)
    {
        // устанавливаем объект
        editor.EmptyAt(x, y);

        // возвращаем ничего
        return null;
    }
}