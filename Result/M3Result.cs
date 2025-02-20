using System;
using UnityEngine;

public class M3Result : MonoBehaviour {
    // менеджер результата

    // доска
    [SerializeField]
    private M3Board m3Board;
    // ходы
    [SerializeField]
    private int movesLeft;
    // рецепт
    [SerializeField]
    private M3Recipe m3Recipe;
    // ui
    [SerializeField]
    private M3ResultUI ui;
    // состояние
    [SerializeField]
    private M3GameState state;

    // геттер & сеттер состояний
    public M3GameState GetState() => state;
    public void SetState(M3GameState _state) => state = _state;

    // рецепт
    public M3Recipe GetRecipe() => m3Recipe;

    // получение ходов
    public int GetMoves() => movesLeft;
    
    // получение доски
    public M3Board GetBoard() => m3Board;

    // инициализатор
    public void Init(M3Level _level)
    {
        // устанавливаем значение
        movesLeft = _level.moves;
        m3Recipe = _level.recipe.Copy();
        // инициализируем ui
        ui.InitUI();
    }

    // обновление ходов
    public void SubMove()
    {
        // отнимаем ход
        movesLeft--;
        // обновляем UI
        ui.UpdateUI();
    }

    // тикинг результата
    public void TickResult()
    {
        // если состояние не PLAY не
        // тикаем результат
        if (state != M3GameState.PLAY) return;
        
        // если поле не в состоянии покоя
        if (!m3Board.Idle())
        {
            throw new Exception("Board Is Not Idle While Result Tick");
        }

        // если рецепт готов
        if (m3Recipe.IsDone())
        {
            // устанавливаем состояние победы
            SetState(M3GameState.WIN);
            // говорим что игрок прошел первый уровень
            StorageService.GetInstance().Save<bool>("first_level_played", true);
            // выдаём награды
            GiveWinRewards();
            // устанавливаем в сохранениях то,
            // что игрок прошел новый уровень с результатом "Win".
            // чтобы потом это хэндлить, например в ивентах
            StorageService.GetInstance().Save<M3LevelFinishResult>(
                "level_finish", new M3LevelFinishResult(M3GameState.WIN));
            // переходим на след. уровень
            if (StorageService.GetInstance() is not null && !m3Board.editorMode)
            {
                int level = (int)StorageService.GetInstance().Get<int>("level");
                StorageService.GetInstance().Save<int>("level", level + 1);
            }
            // отправляем аналитику
            string _level = ((int)StorageService.GetInstance().Get<int>("level")-1).ToString();
            AnalyticsService.GetInstance().Send(AnalyticType.LEVEL_WIN, _level);
            // показываем попап
            ui.ShowWinPanel();
        }
        // если ходы на нуле
        else if (movesLeft == 0)
        {
            // устанавливаем состояние проигрыша
            SetState(M3GameState.LOSE);
            // устанавливаем в сохранениях то,
            // что игрок прошел новый уровень с результатом "Win".
            // чтобы потом это хэндлить, например в ивентах
            StorageService.GetInstance().Save<M3LevelFinishResult>(
                "level_finish", new M3LevelFinishResult(M3GameState.LOSE));      
            // отправляем аналитику
            string _level = ((int)StorageService.GetInstance().Get<int>("level")-1).ToString();
            AnalyticsService.GetInstance().Send(AnalyticType.LEVEL_LOSE, _level);
            // отнимаем жизнь если уровень не в режиме редактора и нет бесконечных жизней
            bool infiniteLifes = (long)StorageService.GetInstance().Get<long>("infinite_lifes") >
                                 GameTime.CurrentTimeMillis();
            if (!m3Board.editorMode && !infiniteLifes)
            {
                // забираем жизнь
                int savedLifes = (int)StorageService.GetInstance().Get<int>("lifes");
                StorageService.GetInstance().Save("lifes", savedLifes - 1);
            }
            // показываем попап
            ui.ShowLosePanel();
        }
    }

    // выдача наград за победу
    private void GiveWinRewards()
    {
        // монеты
        QuantityService.Give(new Reward(RewardType.COINS, (movesLeft+10) * 2));
        // звёзды
        QuantityService.Give(new Reward(RewardType.STARS, 1));
        // тикеты
        QuantityService.Give(new Reward(RewardType.TICKET, 1));
    }

    // получение предварительного результата
    public M3GameState GetResult()
    {
        // если рецепт окончен
        if (m3Recipe.IsDone())
        {
            return M3GameState.WIN;
        }
        // если ходы на нуле
        else if (movesLeft == 0)
        {
            return M3GameState.LOSE;
        }

        // возвращаем состояние игры
        return M3GameState.PLAY;
    }

    // обновление цели
    public void UpdateGoal(M3ObjectType t, int amount)
    {
        // обновляем цель
        M3Goal goal = m3Recipe.goals.Find(goal => goal.type == t);
        if (goal != null)
        {
            if (goal.amount - amount > 0) { goal.amount -= amount; }
            else { goal.amount = 0; }
            // обновляем UI
            ui.UpdateUI();
        }
    }
    
    // готов ли рецепт
    public bool IsDone(M3ObjectType type)
    {
        return m3Recipe.goals.Find(goal => goal.type == type).amount <= 0;
    }

    // добавление ходов
    public void AddMoves(int amount)
    {
        // добавляем ходы
        movesLeft += amount;
        // обновляем UI
        ui.UpdateUI();        
    }
}