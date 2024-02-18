using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.Scenes.Player;

[GlobalClass]
public partial class Ingredient : BaseCarriableItem, IStashable
{
    [Export]
    public IngredientTypes Type { get; set; }

    public int Count { get; set; } = 1;
}
