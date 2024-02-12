using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;
using Godot;
using System;

namespace FiveNightsAtFrederik.scenes.level;

public partial class SodaMinigame : BaseMinigame, IPlayerUsable
{
    public bool IsInteractionUIDisplayed { get; set; } = true;

    private MeshInstance3D mug;
    private MeshInstance3D drink;
    private MeshInstance3D foam;
    private Area3D drinkTopLimitArea;
    private Area3D drinkWinLimitArea;
    private Timer foamRaiseTimer;
    private Timer foamShrinkingTimer;
    private PickupSpawner drinkSpawner;

    /// <summary>
    /// Determines how long will the foam rise. Value is derived from how long does player keep pouring. (Max 4)
    /// </summary>
    private float foamRaisingTimerWaitTime = 0f;
    private bool isFoamShrinking;
    private bool isInWinArea;

    /// <summary>
    /// How fast does the drink's volume raise 
    /// </summary>
    private const float drinkFillRate = 1f;
    private const float foamFillRate = 0.001f;

    /// <summary>
    /// How long must player keep filling the mug, before the foam will raise for some time
    /// </summary>
    private const float MinimumFoamRaiseThreshold = 0.1f;
    private const float MinimumShrinkFoamScale = 1.025f;
    private const int MaximumFoamRaiseThreshold = 2;

    public override void _Ready()
    {
        mug = this.TryGetNode<MeshInstance3D>(NodeNames.Mug, nameof(mug));
        drink = this.TryGetNode<MeshInstance3D>(NodeNames.Drink, nameof(drink));
        drinkTopLimitArea = this.TryGetNode<Area3D>(NodeNames.DrinkTopLimitArea, nameof(drinkTopLimitArea));
        drinkWinLimitArea = this.TryGetNode<Area3D>(NodeNames.DrinkWinLimitArea, nameof(drinkWinLimitArea));
        foam = this.TryGetNode<MeshInstance3D>(NodeNames.Foam, nameof(foam));
        foamRaiseTimer = this.TryGetNode<Timer>(NodeNames.FoamRaiseTimer, nameof(foamRaiseTimer));
        foamShrinkingTimer = this.TryGetNode<Timer>(NodeNames.FoamShrinkingTimer, nameof(foamShrinkingTimer));
        drinkSpawner = this.TryGetNode<PickupSpawner>(NodeNames.DrinkSpawner, nameof(drinkSpawner));

        foamRaiseTimer.Timeout += () =>
        {
            isFoamShrinking = true;
            foamRaisingTimerWaitTime = 0;
        };

        foamShrinkingTimer.Timeout += () => isFoamShrinking = false;
        drinkTopLimitArea.BodyEntered += DrinkTopLimitCollision_BodyEntered;
        drinkWinLimitArea.BodyEntered += (_) => isInWinArea = true;
        drinkSpawner.OnObjectLeftSpawnArea += () =>
        {
            mug.Visible = true;
            interactionCollision.Disabled = false;
        };

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
    /// There is a bug with static Bodies, where they are not detected by Area3D
    /// https://github.com/godotengine/godot/issues/74300
    /// </summary>
    /// <param name="delta"></param>
    public override void _PhysicsProcess(double delta)
    {
        drinkTopLimitArea.Monitorable = false;
        drinkTopLimitArea.Monitorable = true;

        drinkWinLimitArea.Monitorable = false;
        drinkWinLimitArea.Monitorable = true;
    }

    /// <summary>
    /// Checks if all conditions for win is met & creates new object for player to deliver
    /// </summary>
    protected override void TryWin()
    {
        // If the foamRisingTimerWaitTime is zero & foam is not shrinking then the drink's volume is not changing && is in Win Area => Can be "submitted" 
        var isDrinkLevelChanging = foamRaisingTimerWaitTime > 0 || isFoamShrinking;
        if (!isDrinkLevelChanging && isInWinArea)
        {
            mug.Visible = false;
            interactionCollision.Disabled = true;

            drinkSpawner.TrySpawnItem();
            ResetMinigame();
            LeaveMinigame();
        }
    }

    /// <summary>
    /// Hides drink's win/lose areas
    /// </summary>
    protected override void LeaveMinigame()
    {
        drinkWinLimitArea.Visible = false;
        drinkTopLimitArea.Visible = false;

        base.LeaveMinigame();
    }

    public override void _Process(double delta)
    {
        // Pours the drink and causes foam and drink to rise
        if (Input.IsActionPressed(ActionNames.Use) && isActive)
        {
            HandleFilling(drink, drinkFillRate);
            HandleFilling(foam, foamFillRate);

            drink.Visible = true;
            drinkWinLimitArea.Visible = true;
            drinkTopLimitArea.Visible = true;

            isFoamShrinking = false;
            foamRaisingTimerWaitTime = Mathf.Min(foamRaisingTimerWaitTime + (float)delta, MaximumFoamRaiseThreshold);
            return;
        }

        // When the player stops pouring the drink, we will make the foam raise faster for foamRaisingTimerWaitTime
        if (Input.IsActionJustReleased(ActionNames.Use) && isActive)
        {
            foamRaiseTimer.Start(foamRaisingTimerWaitTime);

            return;
        }

        // Makes the foam raise fast after player stops pouring the drink
        if (!isFoamShrinking && foamRaiseTimer.TimeLeft != 0)
        {
            HandleFilling(foam, foamFillRate, 1.1f);
            return;
        }

        // Starts shrinking the foam after it has been rising for some time and also raising the drink
        if (isFoamShrinking)
        {
            isFoamShrinking = foam.Scale.Y > MinimumShrinkFoamScale;
            HandleFilling(foam, foamFillRate, -1);
            HandleFilling(drink, drinkFillRate, 0.4f);
        }

        TryWin();
    }

    /// <summary>
    /// Handles raising and shrinking
    /// </summary>
    /// <param name="multiplicationConstant">if negative, the object will start to go down. If positive it will rise</param>
    private void HandleFilling(MeshInstance3D mesh, float fillRate, float multiplicationConstant = 1)
    {
        mesh.Scale = new Vector3()
        {
            X = mesh.Scale.X,
            Y = mesh.Scale.Y + fillRate * multiplicationConstant,
            Z = mesh.Scale.Z
        };
    }

    private void DrinkTopLimitCollision_BodyEntered(Node3D body)
    {
        ResetMinigame();

        GD.Print("You Failed Minigame!");
    }


    /// <summary>
    /// Resets the minigame into its original state
    /// </summary>
    protected override void ResetMinigame()
    {
        isInWinArea = false;
        drink.Visible = false;
        foam.Visible = false;
        foamRaisingTimerWaitTime = 0;
        foamRaiseTimer.Stop();

        drink.Scale = new Vector3()
        {
            X = drink.Scale.X,
            Y = 1,
            Z = drink.Scale.Z
        };

        foam.Scale = new Vector3()
        {
            X = foam.Scale.X,
            Y = 1,
            Z = foam.Scale.Z
        };
    }
}
