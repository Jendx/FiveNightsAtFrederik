using FiveNightsAtFrederik.CsScripts.Enums;
using System;
using System.Collections.Generic;

namespace FiveNightsAtFrederik.CsScripts.Controllers;

public static class OrderController
{
    private static readonly Random random = new();
    private static readonly List<PizzaType> handledOrders = new();

    public static PizzaType CurrentOrder { get; private set; }

    public static Action OnOrderCreated { get; internal set; }

    public static void CreateOrder()
    {
        CurrentOrder = (PizzaType)random.Next(0, Enum.GetValues<PizzaType>().Length);
        OnOrderCreated();
    }
}
