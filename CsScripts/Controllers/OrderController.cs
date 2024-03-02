using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiveNightsAtFrederik.CsScripts.Controllers;

public static class OrderController
{
    private static readonly Random random = new();
    private static readonly List<Order> handledOrders = new();

    public static Order CurrentOrder { get; private set; }

    public static Action OnOrderCreated { get; internal set; }

    public static void CreateOrder()
    {
        var pizzaType = (PizzaType)random.Next(0, Enum.GetValues<PizzaType>().Length);
        CurrentOrder = Orders.AllOrders[random.Next(0, Orders.AllOrders.Count)];
        
        OnOrderCreated();
    }

    /// <summary>
    /// Adds current order into a handledOrders & handles evaluation of order
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public static void FinishOrder(Order finishedOrder)
    {
        // TODO: Tweak values for each failed condition
        if (finishedOrder.Drinks.Count != CurrentOrder.Drinks.Count)
        {
            finishedOrder.TotalValue -= 20;
        }

        if (finishedOrder.Pizzas.Count != CurrentOrder.Pizzas.Count)
        {
            finishedOrder.TotalValue -= 50;
        }

        foreach (var pizza in finishedOrder.Pizzas)
        {
            if (!CurrentOrder.Pizzas.Any(p => p == pizza))
            {
                finishedOrder.TotalValue -= 10;
            }
        }

        handledOrders.Add(finishedOrder);
    }

    public static float GetTotalValue() => handledOrders.Sum(p => p.TotalValue);
}
