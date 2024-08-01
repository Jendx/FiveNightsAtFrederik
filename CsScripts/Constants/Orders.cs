using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Models;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System.Collections.Generic;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.Constants;

public static class Orders
{
    public static readonly List<Order> AllOrders = new()
    {
        new Order
        {
            OrderAudioStream = GD.Load<AudioStream>("res://scenes/level/Minigames/TakeOrder/Sounds/Order_BearPizza.mp3"),
            Pizzas = new List<PizzaType>() { PizzaType.BearsPizza },
            Drinks = new List<CarriableItem>()
        },
        new Order
        {
            OrderAudioStream = GD.Load<AudioStream>("res://scenes/level/Minigames/TakeOrder/Sounds/Order_DucksChoice.mp3"),
            Pizzas = new List<PizzaType>() { PizzaType.DucksChoice },
            Drinks = new List<CarriableItem>()
        }
    };
}
