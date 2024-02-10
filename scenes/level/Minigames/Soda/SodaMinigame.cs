using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using Godot;
using System;

namespace FiveNightsAtFrederik.scenes.level;

public partial class SodaMinigame : BaseMinigame, IPlayerUsable
{
    public bool IsInteractionUIDisplayed { get; set; } = true;

    private MeshInstance3D drink;
    private MeshInstance3D foam;
    private MeshInstance3D mug;
    private MeshInstance3D drinkTopLimitCollision;
    private Timer foamRaiseTimer;
    private Timer foamShrinkingTimer;

    /// <summary>
    /// Determines how long will the foam rise. Value is derived from how long does player keep pouring. (Max 4)
    /// </summary>
    private float foamRaisinggTimerWaitTime = 1f;
    private bool isFoamShrinking;

    /// <summary>
    /// How fast does the drink's volume raise 
    /// </summary>
    private const float drinkFillRate = 0.001f;
    private const float foamFillRate = 0.001f;
    
    /// <summary>
    /// If added, Counters the scale effect of moving item up ( 1/2 scaleRate)
    /// </summary>
    private const float drinkAntiScaleRaise = drinkFillRate / 2;
    private const float foamAntiScaleRaise = foamFillRate / 2;

    /// <summary>
    /// How long must player keep filling the mug, before the foam will raise for some time
    /// </summary>
    private const float MinimumFoamRaiseThreshold = 0.3f;
    private const float MinimumShrinkFoamScale = 1.1f;
    private const int MaxiumumFoamRaiseThreshold = 2;

    public override void _Ready()
    {
        mug = this.TryGetNode<MeshInstance3D>(NodeNames.Mug, nameof(mug));
        drink = this.TryGetNode<MeshInstance3D>(NodeNames.Drink, nameof(drink));
        foam = this.TryGetNode<MeshInstance3D>(NodeNames.Foam, nameof(foam));
        drinkTopLimitCollision = this.TryGetNode<MeshInstance3D>(NodeNames.DrinkTopLimitCollision, nameof(foam));
        foamRaiseTimer = this.TryGetNode<Timer>(NodeNames.FoamRaiseTimer, nameof(foamRaiseTimer));
        foamShrinkingTimer = this.TryGetNode<Timer>(NodeNames.FoamShrinkingTimer, nameof(foamShrinkingTimer));
        foamRaiseTimer.Timeout += () => isFoamShrinking = true;
        foamShrinkingTimer.Timeout += () => isFoamShrinking = false;
        base._Ready();
    }

    private void DrinkTopLimitCollision_BodyShapeEntered(Rid bodyRid, Node3D body, long bodyShapeIndex, long localShapeIndex)
    {
        GD.Print("Hit");
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed(ActionNames.DEBUG_TOGGLEMOUSE) && isActive)
        {
            LeaveMinigame();
        }
    }

    public override void _Process(double delta)
    {


        // Pours the drink and causes foam and drink to rise
        if (Input.IsActionPressed(ActionNames.Use) && isActive)
        {
            FillMug();
            HandleFoamLevel();

            isFoamShrinking = false;
            foamRaisinggTimerWaitTime = Mathf.Max(foamRaisinggTimerWaitTime + (float)delta, MaxiumumFoamRaiseThreshold);

            return;
        }

        // When the player stops pouring the drink, we will make the foam raise faster for foamRaisingTimerWaitTime
        if (Input.IsActionJustReleased(ActionNames.Use) && foamRaisinggTimerWaitTime > MinimumFoamRaiseThreshold && isActive)
        {
            foamRaiseTimer.Start(foamRaisinggTimerWaitTime);
            foamRaisinggTimerWaitTime = 0;

            return;
        }

        // Makes the foam raise fast after player stops pouring the drink
        if (!isFoamShrinking && foamRaiseTimer.TimeLeft != 0)
        {
            HandleFoamLevel(3);
            return;
        }

        // Starts shrinking the foam after it has been rising for some time
        if (isFoamShrinking)
        {
            isFoamShrinking = foam.Scale.Y > MinimumShrinkFoamScale;
            HandleFoamLevel(-1);
            FillMug(0.05f);
        }
    }

    /// <summary>
    /// Handles filling the mug with the drink
    /// </summary>
    /// <param name="multiplier"></param>
    private void FillMug(float multiplier = 1)
    {
        drink.Visible = true;
        drink.Position = new Vector3()
        {
            X = drink.Position.X,
            Y = drink.Position.Y + drinkAntiScaleRaise * multiplier,
            Z = drink.Position.Z
        };

        drink.Scale = new Vector3()
        {
            X = drink.Scale.X,
            Y = drink.Scale.Y - drinkFillRate * multiplier,
            Z = drink.Scale.Z
        };

    }

    /// <summary>
    /// Handles raising and shrinking of foam
    /// </summary>
    /// <param name="multiplicationConstant">if negative, the foam level will start to go down. If positive it will rise</param>
    private void HandleFoamLevel(float multiplicationConstant = 1)
    {
        foam.Position = new Vector3()
        {
            X = foam.Position.X,
            Y = foam.Position.Y - foamAntiScaleRaise * multiplicationConstant,
            Z = foam.Position.Z
        };

        foam.Scale = new Vector3()
        {
            X = foam.Scale.X,
            Y = foam.Scale.Y + foamFillRate * multiplicationConstant,
            Z = foam.Scale.Z
        };
    }
}
