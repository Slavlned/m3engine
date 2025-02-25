using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Редактор уровней
public class M3Editor : MonoBehaviour
{
    // уровень
    public M3Level editingLevel;

    // тул
    private M3EditorTool tool;

    // инпут
    [SerializeField]
    private M3EditorInputs inputs;

    // список тулов
    private List<M3EditorTool> tools = new List<M3EditorTool>();

    // список клеток
    [SerializeField]
    private List<M3EditorTile> tiles = new List<M3EditorTile>();

    // имя файла
    private string fileName;

    // текущий объект для кисти
    private M3ObjectType type;
    
    // параметры объекта
    private List<M3Param> parameters;

    // получаем тип
    public M3ObjectType GetM3Type() => type;
    
    // дефолтный оффсет клеток
    [SerializeField]
    private Vector2 defaultTileOffset = new Vector2(-0.2f, -0.2f);
    
    // задний тайлмап
    [SerializeField]
    private M3EditorBackgroundTilemap backgroundTilemap;

    // добавляем инструменты
    private void Awake()
    {
        // лог
        GameDebugger.Log("Editor: started.");        
        // инструменты
        tools.Add(new BrushTool(this));
        tools.Add(new EraserTool(this));
        tools.Add(new GenBrushTool(this));
        tools.Add(new GravityTool(this));
        tools.Add(new DelivererBrushTool(this));
        tools.Add(new CoverBrushTool(this));
        // загружаем в дропдаун
        List<string> files = new M3EditorJson().GetFileNames();
        inputs.SetDropDown(files);
    }

    // использование
    public void SetType(int t)
    {
        // устанавливаем тип
        type = (M3ObjectType) t;
    }
    
    // установка параметров
    // e.g dir:down;capacity:5
    public void SetParams(string _params)
    {
        parameters = new List<M3Param>();
        if (_params == "")
        {
            return;
        }
        else
        {
            var settingsPairs = _params.Split(";").ToList();
            settingsPairs.ForEach(elem =>
            {
                var keyValues = elem.Split(":").ToList();
                var parameter = new M3Param();
                parameter.Name = keyValues[0];
                parameter.Value = keyValues[1];
                parameters.Add(parameter);
            });
        }
    }

    // смена инструмента
    public void SetTool(int newTool)
    {
        // устанавилваем инструмент
        this.tool = tools[newTool];
        // лог
        GameDebugger.Log("Tool is changed to: " + this.tool);
        // обнуляем параметры
        this.parameters = new List<M3Param>();
        // проверяем инструмент на гравитационный
        if (this.tool is GravityTool)
        {
            tiles.ForEach(t => t.gravityLayerObj.SetActive(true));
        }
        else
        {
            tiles.ForEach((t) => {
                if (t != null) {
                    t.gravityLayerObj.SetActive(false);
                }
            });
        }
    }

    // получение инструмента
    public M3EditorTool GetTool() => this.tool;

    // устанавливаем кавер на позиции

    public void SetCoverAt(int x, int y, M3ObjectType typeForSet)
    {
        // клеточка
        M3EditorTile tile = tiles.Find(cell => cell.X == x && cell.Y == y);

        // устанавливаем
        if (tile.bottomCover != null) { Destroy(tile.bottomCover.gameObject);  }
        tile.bottomCover = Instantiate(
            M3EditorPrefabs.ByKey(typeForSet), ToPoint(x, y), 
            Quaternion.identity
        ).GetComponent<M3EditorCover>();
    }    
    
    // устанавливаем на позиции
    public void SetAt(int x, int y, M3ObjectType typeForSet)
    {
        // клеточка
        M3EditorTile tile = tiles.Find(cell => cell.X == x && cell.Y == y);

        // устанавливаем
        if (tile.obj != null) { Destroy(tile.obj.gameObject);  }
        tile.obj = Instantiate(
            M3EditorPrefabs.ByKey(typeForSet), ToPoint(x, y), 
            Quaternion.identity
        ).GetComponent<M3EditorObj>();
        tile.obj.parameters = parameters;
    }

    // устанавливаем на позиции пустоту
    public void EmptyAt(int x, int y)
    {
        // клеточка
        M3EditorTile tile = tiles.Find(cell => cell.X == x && cell.Y == y);

        // если клетка существует
        if (tile != null)
        {
            // уничтожаем
            tiles.Remove(tile);
            Destroy(tile.gameObject);
            // задний фон
            backgroundTilemap.RemoveAt(new M3Pos(x, y));      
        }
    }

    // устанавливаем на позиции делеверер
    public void SetDelivererAt(int x, int y)
    {
        // клеточка
        M3EditorTile tile = tiles.Find(cell => cell.X == x && cell.Y == y);

        // устанавливаем
        if (tile.upLayerObj != null) { Destroy(tile.upLayerObj); }
        tile.IsGenerator = false;
        tile.IsDeliverer = true;
        tile.upLayerObj = Instantiate(
            M3EditorPrefabs.GetInstance().delivererPrefab, ToPoint(x, y),
            Quaternion.identity
        );
    }

    // устанавливаем на позиции генератор
    public void SetGenAt(int x, int y)
    {
        // клеточка
        M3EditorTile tile = tiles.Find(cell => cell.X == x && cell.Y == y);

        // устанавливаем
        if (tile.upLayerObj != null) { Destroy(tile.upLayerObj); }
        tile.IsGenerator = true;
        tile.IsDeliverer = false;
        tile.upLayerObj = Instantiate(
            M3EditorPrefabs.GetInstance().generatorPrefab, ToPoint(x, y),
            Quaternion.identity
        );
    }

    // устанавливаем на позиции пустоту
    public void ResetAt(int x, int y)
    {
        // клеточка
        M3EditorTile tile = tiles.Find(cell => cell.X == x && cell.Y == y);

        // уничтожаем
        if (tile.obj != null)
        {
            // удаляем
            Destroy(tile.obj.gameObject);
        }
        else if (tile.IsGenerator || tile.IsDeliverer)
        {
            // ставим генератор и деливерер
            // на false
            tile.IsGenerator = false; 
            tile.IsDeliverer = false;
            // уничтожаем верхней объект
            Destroy(tile.upLayerObj);
        }
        else
        {
            // удаляем клетку
            tile.IsEmpty = true;
            // уничтожаем
            Destroy(tile.gravityLayerObj);
            if (tile.upLayerObj != null) { Destroy(tile.upLayerObj); }
            Destroy(tile.gameObject);
            // удаляем из списка
            tiles.Remove(tile);
            // удаляем задний фон
            backgroundTilemap.RemoveAt(new M3Pos(x, y));            
        }
    }

    // меняем гравитацию на позиции
    public void Gravity(int x, int y)
    {
        // клеточка
        M3EditorTile tile = tiles.Find(cell => cell.X == x && cell.Y == y);

        // меняем гравитацию
        if (tile.gravityDir == M3GravityDir.DOWN)
        {
            tile.gravityDir = M3GravityDir.LEFT;
        }
        else if (tile.gravityDir == M3GravityDir.LEFT)
        {
            tile.gravityDir = M3GravityDir.UP;
        }
        else if (tile.gravityDir == M3GravityDir.UP)
        {
            tile.gravityDir = M3GravityDir.RIGHT;
        }
        else if (tile.gravityDir == M3GravityDir.RIGHT)
        {
            tile.gravityDir = M3GravityDir.DOWN;
        }

        // ротейтим стрелку
        tile.gravityLayerObj.transform.Rotate(0, 0, -90);
    }

    // в поинт
    public Vector3 ToPoint(int x, int y)
    {
        return new Vector2(
            x + (editingLevel.cellOffset.x * x),
            y + (editingLevel.cellOffset.y * y)
        ) + new Vector2(
            editingLevel.boardOffset.x,
            editingLevel.boardOffset.y
        );
    }

    // обновляем инпут
    public void UpdateInput()
    {
        // устанавливаем значения
        editingLevel.width = inputs.Width();
        editingLevel.height = inputs.Height();
        editingLevel.boardOffset.x = inputs.XOffset();
        editingLevel.boardOffset.y = inputs.YOffset();
        editingLevel.moves = inputs.Moves();
        fileName = inputs.FileName();
    }

    // создаём поле
    public void Create()
    {
        // лог
        GameDebugger.Log("Editor: recreating board...");
        
        // очищаем
        tiles.ForEach((tile)=>
        {
            if (tile != null)
            {
                Destroy(tile.gameObject);
                if (tile.upLayerObj != null)
                {
                    Destroy(tile.upLayerObj);
                }
                if (tile.gravityLayerObj != null)
                {
                    Destroy(tile.gravityLayerObj);
                }                
                if (tile.obj != null)
                {
                    Destroy(tile.obj.gameObject);
                }
                if (tile.bottomCover != null)
                {
                    Destroy(tile.bottomCover.gameObject);
                }                  
            }
        });
        tiles.Clear();

        // обновляем инпут
        UpdateInput();
        
        // обновляем оффсет
        editingLevel.cellOffset = defaultTileOffset;

        // создаём 
        for (int x = 0; x < inputs.Width(); x++)
        {
            for (int y = 0; y < inputs.Height(); y++)
            {
                // создаём
                GameObject o = Instantiate(M3EditorPrefabs.Instance.cellPrefab, ToPoint(x, y), Quaternion.identity);
                GameObject g = Instantiate(M3EditorPrefabs.Instance.gravityArrow, ToPoint(x, y), Quaternion.identity);
                g.SetActive(false);
                M3EditorTile tile = o.GetComponent<M3EditorTile>();
                tile.gravityLayerObj = g;
                tile.gravityDir = M3GravityDir.DOWN;
                tile.X = x;
                tile.Y = y;
                tiles.Add(tile);
            }
        }
        
        // задний фон
        backgroundTilemap.PrepareTilemap(ToPoint(0, 0), tiles.ConvertAll(a => new M3Pos(a.X, a.Y)));        
    }

    // конвертация в инфо о поле
    public void ConvertToLevel()
    {
        // лог
        GameDebugger.Log("Editor: loading...");
        
        // очищаем
        editingLevel.tiles.Clear();
        editingLevel.recipe.goals.Clear();
        editingLevel.cellOffset = defaultTileOffset;

        // заполняем
        foreach (var tile in tiles)
        {
            // если пуст пропускаем
            if (tile.IsEmpty) { continue;  }

            // получаем информацию
            M3TileInfo tileInfo = new M3TileInfo();
            tileInfo.Pos = new M3Pos(tile.X, tile.Y);
            tileInfo.IsGenerator = tile.IsGenerator;
            tileInfo.IsDeliverer = tile.IsDeliverer;
            tileInfo.Info = new M3ObjectInfo();
            tileInfo.BottomCover = new M3CoverInfo();
            // объект тайла
            if (tile.obj != null) {
                tileInfo.Info.type = tile.obj.type;
                tileInfo.Info.IsRandom = false;
                tileInfo.Info.parameters = tile.obj.parameters;
            }
            else {
                tileInfo.Info.IsRandom = true;
                tileInfo.Info.parameters = new List<M3Param>();
            }
            // кавер тайла (нижний)
            if (tile.bottomCover != null) {
                tileInfo.BottomCover.coverType = tile.bottomCover.type;
                tileInfo.BottomCover.HasCover = true;
            }
            else
            {
                tileInfo.BottomCover.HasCover = false;
            }   
            // направление гравитации
            tileInfo.GravityDir = tile.gravityDir;

            // добавляем
            editingLevel.tiles.Add(
                tileInfo
            );
        }

        // голы
        editingLevel.recipe.goals = inputs.Goals();
        editingLevel.moves = inputs.Moves();

        // конвертим в json и сохраняем
        new M3EditorJson().Save(editingLevel, inputs.FileName());
    }

    // редактируем уровень
    public void Edit()
    {
        // лог
        GameDebugger.Log("Editor: editing(" + inputs.EditFileName() + ")");        
        // очищаем
        tiles.ForEach((tile) =>
        {
            Destroy(tile.gameObject);
            if (tile.upLayerObj != null)
            {
                Destroy(tile.upLayerObj);
            }
            if (tile.gravityLayerObj != null)
            {
                Destroy(tile.gravityLayerObj);
            }            
            if (tile.obj != null)
            {
                Destroy(tile.obj.gameObject);
            }
            if (tile.bottomCover != null)
            {
                Destroy(tile.bottomCover.gameObject);
            }            
        });
        tiles.Clear();
        
        // очищаем инпуты
        inputs.ResetGoals();

        // загружаем уровень
        M3Level _level = new M3EditorJson().Load(inputs.EditFileName());

        // ставим редактируемый уровень на загруженный
        this.editingLevel = _level;
        this.editingLevel.cellOffset = defaultTileOffset;

        // загружаем тайлы
        foreach (M3TileInfo m3TileInfo in editingLevel.tiles)
        {
            // клеточка
            GameObject o = Instantiate(M3EditorPrefabs.Instance.cellPrefab, 
                                        ToPoint(m3TileInfo.Pos.X, m3TileInfo.Pos.Y), 
                                        Quaternion.identity);
            M3EditorTile tile = o.GetComponent<M3EditorTile>();
            tile.X = m3TileInfo.Pos.X;
            tile.Y = m3TileInfo.Pos.Y;
            tile.gravityDir = m3TileInfo.GravityDir;
            tile.gravityLayerObj = Instantiate(M3EditorPrefabs.Instance.gravityArrow, ToPoint(tile.X, tile.Y), Quaternion.identity);
            tile.gravityLayerObj.SetActive(false);           
            tiles.Add(tile);

            // если объект генератор
            if (m3TileInfo.IsGenerator)
            {
                SetGenAt(m3TileInfo.Pos.X, m3TileInfo.Pos.Y);
            }

            // если объект деливерер
            if (m3TileInfo.IsDeliverer)
            {
                SetDelivererAt(m3TileInfo.Pos.X, m3TileInfo.Pos.Y);
            }

            // объект в клеточке
            if (!m3TileInfo.Info.IsRandom)
            {
                SetAt(m3TileInfo.Pos.X, m3TileInfo.Pos.Y, m3TileInfo.Info.type);
                tile.obj.parameters = m3TileInfo.Info.parameters;
            }
            
            // кавер в клеточке (нижний)
            if (m3TileInfo.BottomCover.HasCover)
            {
                SetCoverAt(m3TileInfo.Pos.X, m3TileInfo.Pos.Y, m3TileInfo.BottomCover.coverType);
            }
            
            // гравитация в клеточке
            SetGravityAt(m3TileInfo.Pos.X, m3TileInfo.Pos.Y, m3TileInfo.GravityDir);
        }

        // загружаем в инпуты
        inputs.SetInputs(this, inputs.EditFileName());

        // задний фон
        backgroundTilemap.PrepareTilemap(ToPoint(0, 0), tiles.ConvertAll(a => new M3Pos(a.X, a.Y)));                
    }

    private void SetGravityAt(int x, int y, M3GravityDir gravity)
    {
        // тайл
        M3EditorTile tile = tiles.Find(o => o.X == x  && o.Y == y);
        // гравитация
        tile.gravityDir = gravity;

        // поворот
        if (tile.gravityDir == M3GravityDir.LEFT)
        {
            tile.gravityDir = M3GravityDir.LEFT;
            tile.gravityLayerObj.transform.Rotate(0, 0, -90);
        }
        else if (tile.gravityDir == M3GravityDir.UP)
        {
            tile.gravityDir = M3GravityDir.UP;
            tile.gravityLayerObj.transform.Rotate(0, 0, -180);
        }
        else if (tile.gravityDir == M3GravityDir.RIGHT)
        {
            tile.gravityDir = M3GravityDir.RIGHT;
            tile.gravityLayerObj.transform.Rotate(0, 0, -270);
        }
    }

    // играем в уровень
    public void Play()
    {
        // конвертим в уровень
        ConvertToLevel();

        // устанавливаем сохранения
        StorageService.GetInstance().Save<bool>("is_editor_level", true);
        StorageService.GetInstance().Save<string>("editor_level", inputs.FileName());

        // запускаем сцену
        ScenesService.GetInstance().Load("M3Scene");
    }
    
    // возвращаемся в меню
    public void Menu()
    {
        // запускаем сцену
        ScenesService.GetInstance().Load("MenuScene");
    }
}