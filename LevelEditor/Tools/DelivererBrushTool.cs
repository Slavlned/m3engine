// кисть
public class DelivererBrushTool: M3EditorTool
{
    // эдитор
    private M3Editor editor;

    // констуктор
    public DelivererBrushTool(M3Editor editor)
    {
        this.editor = editor;
    }

    // тул
    public override object Use(int x, int y, M3ObjectType type)
    {
        // устанавливаем объект
        editor.SetDelivererAt(x, y);

        // возвращаем ничего
        return null;
    }
}