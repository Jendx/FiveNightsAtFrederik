using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System.Collections.Generic;

namespace FiveNightsAtFrederik.CsScripts.Models;

public sealed class Order
{
    public AudioStream OrderAudioStream { get; set; }
    public List<PizzaType> Pizzas { get; set; } = new();
    public List<CarriableItem> Drinks { get; set; } = new();
    public float TotalValue { get; set; }
}
