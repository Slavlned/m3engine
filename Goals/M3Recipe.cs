using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class M3Goal
{
    // цель
    // тип
    public M3ObjectType type;
    // колличество
    public int amount;

    // выполнен ли
    public bool IsDone()
    {
        return amount == 0;
    }
}

[System.Serializable]
public class M3Recipe
{
    // рецепт
    public List<M3Goal> goals = new List<M3Goal>();

    // готов ли
    public bool IsDone()
    {
        // перебераем цели
        foreach (var goal in goals)
        {
            if (!goal.IsDone()) return false;
        }

        // рецепт готов
        return true;
    }

    // копирывание рецепта
    public M3Recipe Copy()
    {
        // рецепт
        M3Recipe recipe = new M3Recipe();
        // голы
        foreach (M3Goal goal in goals)
        {
            M3Goal _goal = new M3Goal();
            _goal.type = goal.type;
            _goal.amount = goal.amount;
            recipe.goals.Add(_goal);
        }
        // возвращаем
        return recipe;
    }
}