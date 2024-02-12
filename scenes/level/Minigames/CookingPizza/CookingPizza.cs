using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;
using Godot;

namespace FiveNightsAtFrederik.scenes.level;

public partial class CookingPizza : BaseMinigame, IPlayerUsable
{
    public bool IsInteractionUIDisplayed { get; set; } = true;

    private MeshInstance3D pizza;
    private PickupSpawner pizzaSpawner;

    /// <summary>
    /// Determines how long will the foam rise. Value is derived from how long does player keep pouring. (Max 4)
    /// </summary>
    private float foamRaisingTimerWaitTime;
    private bool isFoamShrinking;
    private bool isInWinArea;

    public override void _Ready()
    {
        pizza = this.TryGetNode<MeshInstance3D>(NodeNames.Pizza, nameof(pizza));
        pizzaSpawner = this.TryGetNode<PickupSpawner>(NodeNames.PizzaSpawner, nameof(pizzaSpawner));

        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed(ActionNames.DEBUG_TOGGLEMOUSE) && isActive)
        {
            LeaveMinigame();
        }
    }

    protected override void TryWin()
    {

    }

    protected override void LeaveMinigame()
    {

    }

    public override void _Process(double delta)
    {
        
    }

    /// <summary>
    /// Resets the minigame into its original state
    /// </summary>
    protected override void ResetMinigame()
    {
        
    }
}
