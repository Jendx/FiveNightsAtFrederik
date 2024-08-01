using FiveNightsAtFrederik.CsScripts.Attributes;
using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Models;
using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using FiveNightsAtFrederik.scenes.level.Controls.WinControl;
using FiveNightsAtFrederik.scenes.level.Minigames.PizzaCrafting;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System;
using System.Linq;

namespace FiveNightsAtFrederik.scenes.level.Minigames.PizzaExpedition;

public partial class PizzaExpedition : Node3D, IIndirectlyUsable<BaseUsableParameters>
{
    private const int ordersToWin = 1;

    private int servedOrdersCount; 
    private Area3D submitArea;
    private Marker3D reparentMarker;
    private Order? orderToSubmit;

    public override void _Ready()
    {
        reparentMarker = this.TryGetNode<Marker3D>(NodeNames.ReparentPositionMarker, nameof(reparentMarker));
        submitArea = this.TryGetNode<Area3D>(NodeNames.SubmitArea, nameof(submitArea));
        submitArea.BodyEntered += SubmitArea_BodyEntered;
    }

    private void SubmitArea_BodyEntered(Node3D body)
    {
        if (body is not CarriablePizza && !body.Name.ToString().Contains("Soda"))
        {
            return;
        }

        AttachToExpeditionBox(body);

        orderToSubmit ??= new Order();
        if (body is CarriablePizza pizza)
        {
            foreach (var pizzaType in Enum.GetValues<PizzaType>())
            {
                var ingredientTypes = pizzaType.GetAttributeOfType<IngredientsAttribute>().ingredients;
                var usedIngredientsTypes = pizza.Ingredients.Select(i => i.Type).ToList();
                if (usedIngredientsTypes?.Count > 0 && ingredientTypes.SequenceEqual(ingredientTypes))
                {
                    orderToSubmit.Pizzas.Add(pizzaType);
                    break;
                }
            }

            return;
        }

        orderToSubmit.Drinks.Add((CarriableItem)body);
    }

    /// <summary>
    /// Attaches Physic body to delivery box
    /// </summary>
    /// <param name="body"></param>
    private void AttachToExpeditionBox(Node3D body)
    {
        var orderItem = (BaseCarriableItem)body;

        orderItem.Freeze = true;
        orderItem.Reparent(reparentMarker);
        orderItem.Position = new Vector3()
        {
            X = reparentMarker.Position.X,
            Y = reparentMarker.Position.Y,
            Z = reparentMarker.Position.Z
        };
    }

    public void OnBeginUse(BaseUsableParameters? parameters)
    {
        if (orderToSubmit is null)
        {
            return;
        }

        OrderController.FinishOrder(orderToSubmit);
        // TODO: Add animation of order being moved away

        servedOrdersCount++;
        if (servedOrdersCount >= ordersToWin)
        {
            var winScreen = GetTree().GetFirstNodeInGroup(GroupNames.WinScreen) as WinScreen;
            winScreen.FinishGame();
            return;
        }
        
        orderToSubmit = null;
    }

    public void OnEndUse(BaseUsableParameters? parameters) {}
}
