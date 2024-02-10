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
    private float foamRaisingTimerWaitTime = 0f;
    private bool isFoamShrinking;

    /// <summary>
    /// How fast does the drink's volume raise 
    /// </summary>
    private const float drinkFillRate = 1f;
    private const float foamFillRate = 0.001f;

    /// <summary>
    /// How long must player keep filling the mug, before the foam will raise for some time
    /// </summary>
    private const float MinimumFoamRaiseThreshold = 0.6f;
    private const float MinimumShrinkFoamScale = 1.025f;
    private const int MaxiumumFoamRaiseThreshold = 2;

    /// <summary>
    /// Defines maximum amount of drink poured
    /// </summary>
    private const int MaxiumumDrinkLevelThreshold = 1060;
    private const float FailureHeightThreshold = MaxiumumDrinkLevelThreshold + MinimumShrinkFoamScale;

    public override void _Ready()
    {
        mug = this.TryGetNode<MeshInstance3D>(NodeNames.Mug, nameof(mug));
        drink = this.TryGetNode<MeshInstance3D>(NodeNames.Drink, nameof(drink));
        foam = this.TryGetNode<MeshInstance3D>(NodeNames.Foam, nameof(foam));
        foamRaiseTimer = this.TryGetNode<Timer>(NodeNames.FoamRaiseTimer, nameof(foamRaiseTimer));
        foamShrinkingTimer = this.TryGetNode<Timer>(NodeNames.FoamShrinkingTimer, nameof(foamShrinkingTimer));
        foamRaiseTimer.Timeout += () => isFoamShrinking = true;
        foamShrinkingTimer.Timeout += () => isFoamShrinking = false;

        base._Ready();
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
        GD.Print($"{drink.Scale.Y} {foam.Scale.Y}, {drink.Scale.Y + foam.Scale.Y}");
        if (drink.Scale.Y + foam.Scale.Y > FailureHeightThreshold)
        {
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

            GD.Print("You Failed Minigame!");
        }


        // Pours the drink and causes foam and drink to rise
        if (Input.IsActionPressed(ActionNames.Use) && isActive)
        {
            HandleFilling(drink, drinkFillRate);
            HandleFilling(foam, foamFillRate);

            drink.Visible = true;

            isFoamShrinking = false;
            foamRaisingTimerWaitTime = Mathf.Max(foamRaisingTimerWaitTime + (float)delta * 0.5f, MaxiumumFoamRaiseThreshold);

            return;
        }

        // When the player stops pouring the drink, we will make the foam raise faster for foamRaisingTimerWaitTime
        if (Input.IsActionJustReleased(ActionNames.Use) && foamRaisingTimerWaitTime > MinimumFoamRaiseThreshold && isActive)
        {
            foamRaiseTimer.Start(foamRaisingTimerWaitTime);
            foamRaisingTimerWaitTime = 0;

            return;
        }

        // Makes the foam raise fast after player stops pouring the drink
        if (!isFoamShrinking && foamRaiseTimer.TimeLeft != 0)
        {
            GD.Print("Timer");
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
}
