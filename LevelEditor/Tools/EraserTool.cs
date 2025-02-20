// кисть
public class EraserTool : M3EditorTool
{
    // эдитор
    private M3Editor editor;

    // констуктор
    public EraserTool(M3Editor editor)
    {
        this.editor = editor;
    }

    // тул
    public override object Use(int x, int y, M3ObjectType type)
    {
        // устанавливаем объект
        editor.ResetAt(x, y);

        // возвращаем ничего
        return null;
    }
}