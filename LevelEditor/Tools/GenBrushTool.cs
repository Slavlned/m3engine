// кисть
public class GenBrushTool : M3EditorTool
{
    // эдитор
    private M3Editor editor;

    // констуктор
    public GenBrushTool(M3Editor editor)
    {
        this.editor = editor;
    }

    // тул
    public override object Use(int x, int y, M3ObjectType type)
    {
        // устанавливаем объект
        editor.SetGenAt(x, y);

        // возвращаем ничего
        return null;
    }
}