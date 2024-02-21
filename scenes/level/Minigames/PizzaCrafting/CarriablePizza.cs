using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.Scenes.Player;
using System.Collections.Generic;

namespace FiveNightsAtFrederik.scenes.level.Minigames.PizzaCrafting;

public partial class CarriablePizza : BaseCarriableItem
{
    public float Value { get; internal set; }

    /// <summary>
    /// Ingredients in placed order
    /// </summary>
    public Queue<Ingredient> Ingredients { get; private set; } = new();

    public void AddIngredients(Queue<Ingredient> placedIngredients)
    {
        Ingredients = new(placedIngredients);
        foreach (var placedIngredient in Ingredients)
        {
            // We need to create new instances. When original item expires, it will expire here as well
            // Ingredient type is hidden in "EditorDescription"
            placedIngredient.Reparent(this);
        }
    }
}
