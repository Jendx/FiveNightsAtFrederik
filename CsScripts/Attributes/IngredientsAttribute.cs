using FiveNightsAtFrederik.CsScripts.Enums;
using System;

namespace FiveNightsAtFrederik.CsScripts.Attributes;

public class IngredientsAttribute : Attribute
{
    private IngredientTypes[] ingredients;

    public IngredientsAttribute(params IngredientTypes[] ingredients)
    {
        this.ingredients = ingredients;
    }
}
