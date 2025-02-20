using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using NonSeedRandom = System.Random;

public class M3Board : MonoBehaviour
{
    [SerializeField]
    // тайлы на сцене
    private List<M3Tile> tiles = new List<M3Tile>();

    [SerializeField]
    // объекты на сцене
    public List<M3Object> objects = new List<M3Object>();

    [SerializeField]
    // кулдаун между тиками
    private float tickCooldown;

    [SerializeField]
    // инфо об уровне
    private M3Level _info;

    // бонусы при старте
    [SerializeField] 
    private M3Bonuses bonuses;
    
    // список бонусов на старте
    [SerializeField]
    private List<M3ObjectType> _startBonuses = new List<M3ObjectType>();
    
    // результат
    [SerializeField]
    private M3Result result;
    
    // задний тайлмап доски
    [SerializeField]
    private M3BackgroundTilemap backgroundTilemap;

    // мэтчи
    [SerializeField]
    private List<M3Match> matches = new List<M3Match>();

    // ивенты
    // public Action<M3Object, M3Object> onSwap;
    public Action<M3Object> onFallStarted;
    public Action<M3Object> onFallEnded;
    public Action<M3Object> onGenerate;
    public Action<M3Match> onMerge;
    public Action onTickBegin;
    public Action onTick;
    public Action onIdle;

    // состояние поля на прошлый тик
    private bool lastBoardState;

    // доступна ли гравитация
    [SerializeField]
    private bool gravityEnabled = true;
    [SerializeField]
    private bool isLocked = false;

    // переменные для полного состояния покоя
    // показывает последнее значение idle
    private bool idleLastValue = false;
    // показывает запущена ли короутина отсчета для idle
    private bool idleCoroutineRunned;

    // последний инпут
    private M3Swap lastInput = null;

    // из редактора ли?
    public bool editorMode = false;

    // fx (эффекты)
    [SerializeField]
    private M3Fx fx;
    
    // запущен ли цикл тиков
    private bool loopStarted = false;

    // fx геттер
    public M3Fx GetFx()
    {
        return fx;
    }

    // сеттер для лока
    public void SetLock(bool locked)
    {
        // устанавливаем значение
        this.isLocked = locked;
    }

    // геттер для лока
    public bool GetLock()
    {
        // возвращаем значение
        return this.isLocked;
    }

    // геттер для результата
    public M3Result GetResult()
    {
        // возвращаем значение
        return this.result;
    }

    // получени объектов
    public void AddObject(M3Object m3Object)
    {
        if (!objects.Contains(m3Object))
        {
            objects.Add(m3Object);
        }
    }

    // уничтожение объекта
    public void DestroyObject(M3Object m3Object, M3DestroyType type) {
        // ReSharper disable once Unity.PerformanceCriticalCodeNullComparison
        if (m3Object != null)
        {
            // удаляем
            m3Object.DestroyObject(type);
        }
    }    
    
    // уничтожение объектов по типу
    public void DestroyObjectOfType(M3ObjectType type, M3DestroyType dType)
    {
        List<M3Object> forDestroy = new List<M3Object>();
        forDestroy.AddRange(objects);
        foreach (M3Object o in forDestroy)
        {
            if (o.GetM3Type() == type &&
                o.GetState() == M3State.IDLE)
            {
                DestroyObject(o, dType);
            }
        }
    }
    
    // инициализация
    public void Init(M3Level levelInfo, List<M3ObjectType> bonusesOnStart)
    {
        // логгинг
        DOTween.debugMode = true;
        DOTween.debugStoreTargetId = true;
        DOTween.logBehaviour = LogBehaviour.Verbose;
        // ставим инфо
        _info = levelInfo;
        // ставим бонусы
        _startBonuses = bonusesOnStart;
        // загружаем уровень
        M3Load(_info);
    }

    // загрузка тайлов
    private void M3Load(M3Level level)
    {
        // лог
        GameDebugger.Log("Board: loading board...");   
        // создаём задний фон
        backgroundTilemap.PrepareTilemap(ToPoint(0, 0), level.tiles.ConvertAll(a => a.Pos));
        // загружаем тайлы
        foreach (M3TileInfo tile in level.tiles)
        {
            // спавн тайла
            var _tile = SpawnTileInfo(tile);
            // добавляем в список
            tiles.Add(_tile);
            // спавним объект
            var _object = SpawnTileObjectInfo(tile, _tile);
            // добавляем в список
            objects.Add(_object);
            // спавним покрытие
            if (tile.BottomCover.HasCover)
            {
                _tile.SetBottomCover(SpawnTileCoverInfo(tile, _tile, tile.BottomCover));
            }
        }
        foreach (M3Tile _tile in tiles)
        {
            _tile.FindTilesAround();
        }
        // анимируем тайлы
        StartCoroutine(AnimateTiles());  
    }

    // спавн тайла
    private M3Tile SpawnTileInfo(M3TileInfo tile) {
        // создаём тайл
        GameObject _tile = Instantiate(
            M3Prefabs.GetInstance().tilePrefab, 
            ToPoint(tile.Pos.X, tile.Pos.Y),
            Quaternion.identity
        );
        M3Tile _tileComponent = _tile.GetComponent<M3Tile>();
        // ставим позицию
        _tileComponent.SetPos(tile.Pos.X, tile.Pos.Y);
        // ставим доску
        _tileComponent.SetBoard(this);
        // ставим гравитацию
        _tileComponent.SetGravity(tile.GravityDir);
        // ставим альфа канал на 0
        _tile.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);            
        // генератор
        if (tile.IsGenerator)
        {
            _tileComponent.SetGenerator(new M3Generator(this, _tile.GetComponent<M3Tile>()));
        }
        // деливерер
        if (tile.IsDeliverer)
        {
            _tileComponent.IsDeliverer = true;
        }
        // возвращаем тайл
        return _tileComponent;
    }
    
    // спавн объекта в тайле
    private M3Object SpawnTileObjectInfo(M3TileInfo tile, M3Tile _tile) {
        // если рандомный
        if (tile.Info.IsRandom)
        {
            // тайл объект
            GameObject _tileObject = Instantiate(
                M3Prefabs.GetInstance().RandomChip().Value, 
                ToPoint(tile.Pos.X, tile.Pos.Y),
                Quaternion.identity
            );
            M3Object _tileObjectComponent = _tileObject.GetComponent<M3Object>(); 

            // ставим тайлу объект на новосозданный
            _tile.Set(_tileObjectComponent);
            _tileObjectComponent.SetTile(_tile);
            _tileObjectComponent.SetBoard(this);
            _tileObjectComponent.SetParameters(tile.Info.parameters);
            // ставим альфа канал на 0
            _tileObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);  
            // ставим альфа канал чилдам
            _tileObjectComponent.SetZeroAlphaForChildren();    
            // возвращаем m3объект
            return _tileObjectComponent;    
        }
        else
        {
            // тайл объект
            GameObject _tileObject = Instantiate(
                M3Prefabs.GetInstance().ByKey(tile.Info.type),
                ToPoint(tile.Pos.X, tile.Pos.Y),
                Quaternion.identity
            );
            M3Object _tileObjectComponent = _tileObject.GetComponent<M3Object>(); 
            
            // ставим тайлу объект на новосозданный
            _tile.Set(_tileObjectComponent);
            _tileObjectComponent.SetTile(_tile);
            _tileObjectComponent.SetBoard(this);
            _tileObjectComponent.SetParameters(tile.Info.parameters);
            // ставим альфа канал на 0
            _tileObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0); 
            // ставим альфа канал чилдам
            _tileObjectComponent.SetZeroAlphaForChildren();              
            // возвращаем m3объект
            return _tileObjectComponent;                             
        }
    }

    // спавн кавера в тайле
    private M3Cover SpawnTileCoverInfo(M3TileInfo tile, M3Tile _tile, M3CoverInfo cover) {
        // тайл объект
        GameObject _tileCover = Instantiate(
            M3Prefabs.GetInstance().ByKey(cover.coverType),
            ToPoint(tile.Pos.X, tile.Pos.Y),
            Quaternion.identity
        );
        // даём информацию о тайле и поле каверу
        M3Cover _tileCoverComponent = _tileCover.GetComponent<M3Cover>();
        _tileCoverComponent.SetTile(_tile);
        _tileCoverComponent.SetBoard(this);
        // ставим альфа канал на 0
        _tileCover.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);  
        // возвращаем m3объект
        return _tileCoverComponent;    
    }
    
    // анимация тайлов
    private readonly WaitForSeconds tileAnimationWait = new WaitForSeconds(0.05f);
    private IEnumerator AnimateTiles()
    {
        // перебор тайлов
        foreach (M3Tile tile in tiles)
        {
            tile.AnimateBlink();
            yield return tileAnimationWait;
        }
        // запускаем тикинг с выжиданием анимаций
        StartTicking(0.015f * tiles.Count);   
        // устанавливаем полную видимость
        SetMaxVisibilityForObjects();
    }
    
    // полная видимость для объектов 
    private void SetMaxVisibilityForObjects()
    {
        foreach (M3Object o in objects)
        {
            o.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
        }        
    }

    // в поинт
    public Vector3 ToPoint(int x, int y)
    {
        return new Vector2(
            x + (_info.cellOffset.x * x),
            y + (_info.cellOffset.y * y)
        ) + new Vector2(
            _info.boardOffset.x,
            _info.boardOffset.y
        );
    }

    // запуск тикинга
    private void StartTicking(float delay)
    {
        // лог
        GameDebugger.Log("Board: ticking started."); 
        // начинаем тикать
        StartCoroutine(StartTickLoop(delay));
    }

    // тикаем результат
    private void TickResult()
    {
        // тикаем результат
        result.TickResult();
    }

    // тикаем падение
    public void TickGravity()
    {
        // если гравитация выключена ничего не делаем
        if (gravityEnabled == false) { return; }
        // объекты
        foreach (M3Tile t in tiles.ToList())
        {
            if (t.IsEmpty()) { continue; }
            // тикаем гравитацию
            t.GetObject().TickGravity();
        }
    }

    // тикаем генераторы
    public void TickGenerators()
    {
        // если гравитация выключена ничего не делаем
        if (gravityEnabled == false) { return; }
        // тайлы
        foreach (M3Tile t in tiles)
        {
            // тикаем генератор если тайл им является
            if (t.GetGenerator() != null)
            {
                t.GetGenerator().Tick();
            }
        }
    }

    // помещение мэтча
    public void EnqueueMatch(M3Match match)
    {
        // если мэтич нулл выходим
        if (match == null) { return; }

        // добавляем мэтч
        matches.Add(match);
    }
    
    // список пендинга матчей
    private readonly List<M3Match> pending = new List<M3Match>();
    // заказы
    // private List<M3Object> orders = new List<M3Object>();

    // тикаем мэтчи
    public void TickMatches()
    {
        // очищаем список пендинга
        pending.Clear();

        foreach (M3Match _match in matches.ToList())
        {
            // если создан игроком
            if (_match.InitiatedByPlayer() && !_match.IsCorrupted())
            {
                // подтверждаем ход сразу - если это ход игрока, для избежания ситуаций
                // когда игрок создал две комбинации одним ходом, а 1 из них не удалилась.
                _match.Confirm();
                continue;
            }
            // если не создан игроком
            // то экспандим мэтч, мало-ли
            // после гравитации, ему пологается
            // больше фишек(чипов) внутри
            M3Match match = _match.Expand();
            
            if (match.Ready() && !match.IsCorrupted())
            {
                // проигрываем звук
                fx.SpawnFx(match.source.Pos(), "SFX_COMBO");
                // подтверждаем мэтчи
                match.Confirm();
            } else {
                if (match.IsCorrupted())
                {
                    foreach (M3Match extended in match.Extend())
                    {
                        if (extended != null && !extended.IsCorrupted())
                        {
                            pending.Add(extended);
                        }
                    }
                }
                else
                {
                    pending.Add(match);
                }
            }
        }

        // очищаем
        matches.Clear();

        // энкуеуриум матчи
        for (int x = 0; x < pending.Count; x++)
        {
            EnqueueMatch(pending[x]);
        }
    }

    // очищает мусор из списка
    public void TickGarbageCollector()
    {
        objects.RemoveAll(m => !m);
    }

    // тикаем нахождение матчей, например
    // для тех ситуаций, когда нашелся матч
    // и 2 матч, который отбросился, первый матч
    // не возможно было выполнить, а 2 матча нет,
    // благодаря этому методу мы его найдём
    public void TickFindMatches()
    {
        foreach (M3Object o in objects)
        {
            // если объект есть
            if (o != null)
            {
                // если можно интерактнуть
                if (o.CanInteract())
                {
                    // добавляем мэтч
                    M3Match match = o.FindMatch(false);
                    EnqueueMatch(match);
                }
            }
        }
    }

    // тикаем инпут
    private void TickInput()
    {
        // свапаем если есть инпут
        if (lastInput != null && result.GetMoves() > 0 && !result.GetRecipe().IsDone())
        {
            // если создаётся комбо делаем свап
            if (M3Predications.IsMakesM3(this, lastInput))
            {
                // если свап доступен
                if (lastInput.from.GetSettings().IsSwapEnabled &&
                    lastInput.to.GetSettings().IsSwapEnabled)
                {
                    // свапаем
                    Swap(lastInput.from, lastInput.to);
                    // отнимаем ход
                    result.SubMove();
                }
            }
            // в ином случае показываем фейковый
            else
            {
                // если свап доступен
                if (lastInput.from.GetSettings().IsSwapEnabled &&
                    lastInput.to.GetSettings().IsSwapEnabled)
                {
                    FakeSwap(lastInput.from, lastInput.to);
                }
            }
            lastInput = null;
        }
    }

    // устанавливаем инпут
    public void SwapInput(M3Swap input)
    {
        lastInput = input;
    }

    // тикаем idle
    public void TickIdle()
    {
        // если короутина запущена ничего не делаем
        if (idleCoroutineRunned) { return;  }

        // получаем состояние
        bool idle = FullIdle();
        // если состояние не равное последнему найденому
        if (idle != idleLastValue)
        {
            // устанавливаем
            idleLastValue = idle;

            // если поле в состоянии покоя
            if (idleLastValue)
            {
                // ивент
                StartCoroutine(TickIdleCoroutine());
            }
        }
    }

    // короутина для idle
    private IEnumerator TickIdleCoroutine()
    {
        // короутина запущена
        idleCoroutineRunned = true;

        // ждём пять тиков
        yield return idleCoroutineCooldown;
        // вызываем ивент если мы всё ещё в состоянии покоя
        if (FullIdle())
        {
            // тикаем результат
            TickResult();
            // тикаем shuffle
            TickShuffle();
            // вызываем ивент
            if (FullIdle())
            {
                onIdle?.Invoke();
            }
        }
        // проверяем результат
        if (result.GetResult() != M3GameState.PLAY)
        {
            StartCoroutine(TickIdleCoroutine());
        }
        // проверяем шаффлинг
        if (M3Predications.Shufflable(this))
        {
            StartCoroutine(TickIdleCoroutine());
        }

        // короутина не запущена
        idleCoroutineRunned = false;
    }

    // тикаем шаффлинг (перемешку)
    public void TickShuffle()
    {
        // если можно шаффлнуть => шаффлим
        if (M3Predications.Shufflable(this)) {
            StartCoroutine(Shuffle());
        }
    }

    // шаффлинг (перемешивание)
    public IEnumerator Shuffle()
    {
        // лог
        GameDebugger.Log("Board: shuffling board..."); 
        
        // проверяем на idle
        if (!Idle()) { throw new Exception("Not Idle In Shuffle"); }

        // выключаем гравитацию и лочим поле
        SetLock(true);
        SetGravity(false);

        // удаялем объекты
        List<M3Object> forDestroy = new List<M3Object>();
        foreach (M3Object obj in objects)
        {
            forDestroy.Add(obj);
        }
        forDestroy.ForEach((M3Object obj) =>
        {
            // уничтожаем если объект чип
            if (M3Register.chips.Contains(obj.GetM3Type()))
            {
                DestroyObject(obj, M3DestroyType.NOCALL);
            }
        });

        // ждём 1 кадр
        yield return null;

        // заполняем
        foreach (M3Tile tile in tiles)
        {
            // заполняем
            if (tile.IsEmpty())
            {
                tile.ReplaceTo(
                    M3Register.chips[M3Random.GetInstance().GetRandomChip()]
                );
            }
        }

        // показываем поп-ап
        M3Gui.ShowPopup("Перемешка!");

        // ищем мэтчи
        TickFindMatches();

        // включаем гравитацию и разрешаем ввод игроку
        SetLock(false);
        SetGravity(true);

        // если можно сделать шаффл ещё раз -> делаем
        TickShuffle();
    }

    // кулдаун для idle-короутины
    private static WaitForSeconds idleCoroutineCooldown;

    // вызовы ивентов
    private void TickEventBegin()
    {
        onTickBegin?.Invoke();
    }

    private void TickEvent()
    {
        onTick?.Invoke();
    }
    
    // запуск тиков
    private IEnumerator StartTickLoop(float delay)
    {
        /*
         * Выжидаем задержку для начала тикинга
         */
        if (delay > 0) yield return new WaitForSeconds(delay);
        /*
         * Необходимые в скрипте позже кулдауны
         */
        var cooldownWaiter = new WaitForSeconds(tickCooldown);
        idleCoroutineCooldown = new WaitForSeconds(tickCooldown * 5);
        /*
         * тики перед запуском цикла тиков
         */
        // запуск появления бонусов на поле
        TickSpawnBonuses();
        // тикаем мэтчи
        TickFindMatches();
        // тикаем shuffle
        TickShuffle();        
        // луп
        loopStarted = true;
    }
    
    // запуск появления бонусов на поле
    private void TickSpawnBonuses()
    {
        // спавним
        bonuses.SpawnBonuses(_startBonuses);
    }
    
    // обновление
    private void Update()
    {
        if (loopStarted && !isLocked)
        {
            // тикаем гэрбедж коллектор
            TickGarbageCollector();
            // тикаем инпут
            TickInput();
            // вызываем ивент начала тика
            TickEventBegin();                    
            // тикаем мэтчи (комбинации)
            // обязательно до гравитации,
            // для избежаания того
            // что фишка из мэтча упадет
            TickMatches();
            // тикаем поиск мэтчей
            // тех которые остались
            // TO:DO удалить этот комментарий
            // TickFindMatches();
            // тикаем гэрбедж коллектор
            TickGarbageCollector();            
            // тикаем генераторы
            TickGenerators();
            // тикаем гравитацию
            TickGravity();
            // ещё раз тикаем генераторы
            TickGenerators();
            // тикаем заказы
            // TickOrders();
            // вызываем ивент тика
            TickEvent();
            // тикаем айдл ивент
            TickIdle();
        }        
    }


    // тикаем заказы тиков
    /*
    public void TickOrders()
    {
        foreach (M3Object o in orders)
        {
            if (o != null)
            {
                o.TickOrder();
            }
        }
        orders.Clear();
    }
    */

    // тайл
    public M3Tile TileAt(int x, int y)
    {
        foreach (M3Tile tile in tiles)
        {
            if (tile.Pos().X == x && tile.Pos().Y == y)
            {
                return tile;
            }
        }

        return null;
    }

    // тайл
    public M3Tile TileAt(M3Pos pos)
    {
        foreach (M3Tile tile in tiles)
        {
            if (tile.Pos().X == pos.X && tile.Pos().Y == pos.Y)
            {
                return tile;
            }
        }

        return null;
    }

    // свап объектов
    // ReSharper disable Unity.PerformanceAnalysis
    public void Swap(M3Object m3Object, M3Object m3Object2)
    {
        // свапаем если объекты могут свапаться
        if (m3Object.GetSettings().IsSwapEnabled &&
            m3Object2.GetSettings().IsSwapEnabled
        )
        {
            // свапаем
            m3Object.Swap(m3Object2);
        }
    }

    // фэйк свап объектов ( не правильно )
    public void FakeSwap(M3Object m3Object, M3Object m3Object2)
    {
        // проверка на состояние
        if (m3Object.GetState() != M3State.IDLE 
            || m3Object2.GetState() != M3State.IDLE)
        {
            return;
        }
        // звук
        fx.SpawnFx(m3Object2.GetTile().Pos(), "SFX_SWAP_FAIL");
        // фэйк свап
        m3Object.FakeSwap(m3Object2);
    }

    // проверка на то, находится ли поле в айдл состоянии (покой)
    // * проверяет стейты объектов.
    public bool Idle()
    {
        // Если поле локнуто или гравитация выключена. Возвращаем что поле не idle
        if (isLocked || !gravityEnabled) { return false; }

        foreach (M3Object m3 in objects)
        {
            if (m3.GetState() != M3State.IDLE || m3.Moving)
            {
                return false;
            }
        }

        return true;
    }

    // проверка на то, находится ли поле в полном айдл состоянии (покой)
    // * проверяет стейты объектов.
    // * проверяет могут ли фишки упасть.
    public bool FullIdle()
    {
        // Если поле локнуто или гравитация выключена. Возвращаем что поле не idle
        if (isLocked || !gravityEnabled) { return false; }

        foreach (M3Object m3 in objects)
        {
            if (m3.GetState() != M3State.IDLE ||
                m3.Moving || m3.CanFall())
            {
                return false;
            }
        }

        return true;
    }

    // получение инфо об уровне
    public M3Level GetInfo()
    {
        // возвращаем
        return _info;
    }

    // взрывная волна
    public void DamageWave(M3Pos source, int radius)
    {
        // уничтожение источника
        if (!TileAt(source).IsEmpty())
        {
            TileAt(source).GetObject().Damage(M3DestroyType.NOCALL);
        }
        // гравитация
        SetGravity(false);
        // перебор по x
        for (int xR = -radius; xR <= radius; xR++)
        {
            // перебор по y
            for (int xY = -radius; xY <= radius; xY++)
            {
                // уничтожаем
                M3Tile _tile = TileAt(source.X+xY, source.Y+xR);
                if (_tile != null && !_tile.IsEmpty())
                {
                    DamageOrActivate(_tile);
                }
            }
        }
        // гравитация
        SetGravity(true);
    }

    // fx мерджа
    public void Merge(M3Match m3Match)
    {
        // вызываем событие, которое ловит M3Fx
        onMerge?.Invoke(m3Match);
    }

    // переключатель гравитации
    public void SetGravity(bool gravity)
    {
        // меняем гравитацию
        gravityEnabled = gravity;
    }

    // получение гравитации
    public bool GetGravity() => gravityEnabled;
    
    // дамаг или активация
    public void DamageOrActivate(M3Tile _tile)
    {
        // проверяем что объект доступен
        if (!_tile.GetObject().CanInteract())
        {
            return;
        }

        // елси это бустер - активируем
        if (M3Register.boosters.Contains(
                _tile.GetObject().GetM3Type()
            ))
        {
            // компоненты
            var bomb_component = _tile.GetObject().GetComponent<M3Bomb>();
            var arrow_component = _tile.GetObject().GetComponent<M3Arrow>();
            // бомба
            if (bomb_component != null)
            {
                bomb_component.Activate();
            }
            // стрелка
            else if (arrow_component != null)
            {
                arrow_component.Activate();
            }
            // в ином случае ничего
            else
            {
                // ...
                return;
            }
        }
        // в ином случае - уничтожаем
        else
        {
            _tile.GetObject().Damage(M3DestroyType.NOCALL);
        }
    }

    // дамаг арроу
    public void DamageArrow(M3Pos source, M3ObjectType m3ObjectType)
    {
        // уничтожение источника
        if (!TileAt(source).IsEmpty())
        {
            TileAt(source).GetObject().Damage(M3DestroyType.NOCALL);
        }   
        // гравитация
        SetGravity(false);
        // перебор по вертикали если вертикальная линия
        if (m3ObjectType == M3ObjectType.ARROW_VERTICAL)
        {
            for (int y = 0; y < _info.height; y++)
            {
                // уничтожаем
                M3Tile _tile = TileAt(source.X, y);
                if (_tile != null && !_tile.IsEmpty() && _tile.Pos() != source)
                {
                    // проверяем позицию на источник
                    DamageOrActivate(_tile);
                }
            }
        }
        // в ином случае перебор по горизонтали
        else
        {
            for (int x = 0; x < _info.width; x++)
            {
                // уничтожаем
                M3Tile _tile = TileAt(x, source.Y);
                if (_tile != null && !_tile.IsEmpty() && _tile.Pos() != source)
                {
                    // проверяем позицию на источник
                    DamageOrActivate(_tile);
                }
            }
        }
        // гравитация
        SetGravity(true);
    }

    // получение списка тайлов
    public List<M3Tile> GetTiles()
    {
        return tiles;
    }

    // бронирование тика
    /*
    public void OrderTick(M3Object m3Object)
    {
        orders.Add(m3Object);
    }
    */
    
    // получение имени уровня
    public string GetLevelName()
    {
        if (editorMode)
        {
            return "ЭДИТОР";
        }
        
        return "УРОВЕНЬ: "+ _info.level.ToString();
    }

    // выбор рандомной клеточки с чипом внутри
    // (с фишкой внутри)
    public M3Tile ChooseRandomChipTile()
    {
        // рандом без сида для выбора клетки
        NonSeedRandom rnd = new NonSeedRandom();
        // перебор тайлов
        List<M3Tile> chipTiles = new List<M3Tile>();
        foreach (M3Tile t in tiles)
        {
            if (!t.IsEmpty() && M3Register.chips.Contains(t.GetObject().GetM3Type()))
            {
                chipTiles.Add(t);
            }
        }
        // возвращаем ничего
        return chipTiles[rnd.Next(0, chipTiles.Count)];
    }
}