using FiveNightsAtFrederik.CsScripts.Enums;
using System;
using System.Collections.Generic;

namespace FiveNightsAtFrederik.CsScripts.Attributes;

public class IngredientsAttribute : Attribute
{
    public List<IngredientTypes> ingredients = new();

    public IngredientsAttribute(params IngredientTypes[] ingredients)
    {
        this.ingredients = new(ingredients);
    }
}
