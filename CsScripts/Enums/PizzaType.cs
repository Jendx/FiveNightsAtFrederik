using FiveNightsAtFrederik.CsScripts.Attributes;
using System.ComponentModel;

namespace FiveNightsAtFrederik.CsScripts.Enums;

public enum PizzaType
{
    [Description("Medvědí Pizza")]
    [Ingredients(IngredientTypes.Bacon, IngredientTypes.Meatballs, IngredientTypes.Ham, IngredientTypes.Tomatoes)]
    BearsPizza = 0,

    [Description("Kačeří výběr")]
    [Ingredients(IngredientTypes.Broccoli, IngredientTypes.Anchovies, IngredientTypes.Salami, IngredientTypes.Tomatoes)]
    DucksChoice = 1,
}