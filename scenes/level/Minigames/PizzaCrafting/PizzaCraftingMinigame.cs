using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;
using Godot;

namespace FiveNightsAtFrederik.scenes.level;

public partial class PizzaCraftingMinigame : BaseMinigame, IPlayerUsable
{
    public bool IsInteractionUIDisplayed { get; set; } = true;

    private MeshInstance3D? dough;
    private PickupSpawner? pizzaSpawner;

    /// <summary>
    /// Determines how long will the foam rise. Value is derived from how long does player keep pouring. (Max 4)
    /// </summary>
    private float foamRaisingTimerWaitTime;
    private bool isFoamShrinking;
    private bool isInWinArea;

    public override void _Ready()
    {
        dough = this.TryGetNode<MeshInstance3D>(NodeNames.Dough, nameof(dough));
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

    /// <summary>
    /// Show Mouse, when player enters
    /// </summary>
    public override void OnBeginUse()
    {
        Input.MouseMode = Input.MouseModeEnum.Confined;
        base.OnBeginUse();
    }

    /// <summary>
    /// Hides mouse, when player is leaving minigame
    /// </summary>
    protected override void LeaveMinigame()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        base.LeaveMinigame();
    }

    protected override void TryWin()
    {

    }

    public override void _Process(double delta)
    {
        if (!isActive)
        {
            return;
        }

        AddChild(new MeshInstance3D()
        {
            Mesh = new CylinderMesh() { }
        });
    }

    /// <summary>
    /// Resets the minigame into its original state
    /// </summary>
    protected override void ResetMinigame()
    {
        
    }
}
