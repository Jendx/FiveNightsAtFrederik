using FiveNightsAtFrederik.CsScripts.BaseNodes;
using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Enums;
using FiveNightsAtFrederik.CsScripts.Extensions;
using FiveNightsAtFrederik.CsScripts.Helpers;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.CsScripts.Scenes.Level.Props;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;
using System;
using System.Collections.Generic;

namespace FiveNightsAtFrederik.scenes.level;

public partial class PizzaCraftingMinigame : BaseMinigame, IPlayerUsable
{
    public bool IsInteractionUIDisplayed { get; set; } = true;

    private readonly Random random = new();
    private MeshInstance3D? dough;
    private MeshInstance3D? targetMesh;
    private Ingredient? ingredient;
    private PickupSpawner? pizzaSpawner;
    private Area3D? targetArea;
    private Area3D? failureArea;
    private Queue<MeshInstance3D> placedIngredients = new();

    /// <summary>
    /// Determines how long will the foam rise. Value is derived from how long does player keep pouring. (Max 4)
    /// </summary>
    private float foamRaisingTimerWaitTime;
    private float pizzaValue;
    private bool isFoamShrinking;
    private bool isInWinArea;

    private const float radius = 0.14f;
    private const float maxDistanceFromCenter = 0.23f;
    private const int successfullPlacementValue = 10;
    private const int failurePlacementValue = 3;

    public override void _Ready()
    {
        dough = this.TryGetNode<MeshInstance3D>(NodeNames.Dough, nameof(dough));
        targetMesh = this.TryGetNode<MeshInstance3D>(NodeNames.TargetMesh, nameof(targetMesh));
        targetArea = this.TryGetNode<Area3D>(NodeNames.TargetArea, nameof(targetArea));
        failureArea = this.TryGetNode<Area3D>(NodeNames.FailureArea, nameof(failureArea));
        ingredient = this.TryGetNode<Ingredient>(NodeNames.Ingredient, nameof(ingredient));
        pizzaSpawner = this.TryGetNode<PickupSpawner>(NodeNames.PizzaSpawner, nameof(pizzaSpawner));

        targetArea.BodyEntered += TargetArea_BodyEntered;
        failureArea.BodyEntered += FailureArea_BodyEntered;
        base._Ready();
    }

    private void ResetIngredient()
    {
        ingredient.GravityScale = 0;
        ingredient.LinearVelocity = Vector3.Zero;
        ingredient.Position = new Vector3()
        {
            X = dough.Position.X,
            Y = 0.124f,
            Z = dough.Position.Z
        };
    }

    /// <summary>
    /// If ingredient enters the area, player successfully hit the target area => Reset position, add points, move target area
    /// </summary>
    /// <param name="body"></param>
    private void TargetArea_BodyEntered(Node3D body)
    {
        // If is needed to prevent retriggers. If the ingredient is falling, hit's and get's reset, gravity will be "0" => retriggers can't happen
        if (ingredient.GravityScale == 0)
        {
            return;
        }

        GD.Print("Successful Body Entered");

        // Add amount to pizza value dependent on distance from target
        var distanceToTargetMesh = ingredient.Position.DistanceTo(targetMesh.Position);
        pizzaValue += Mathf.Remap(distanceToTargetMesh, 0.1f, 0.05f, 6, successfullPlacementValue);

        PlaceIngredient();

        // Set new targetPosition & refresh
        targetMesh.Position = Vector3Helper.GetRandomPositionInCircle(dough.Position, targetMesh.Position.Y, radius);
        ResetIngredient();
    }

    /// <summary>
    /// Spawns the "placed" ingredient on the dropped position
    /// and enqueues the ingredient into Queue
    /// </summary>
    private void PlaceIngredient()
    {
        var placedIngredient = new MeshInstance3D()
        {
            Mesh = ingredient.MeshInstance.Mesh,
            Position = new Vector3()
            {
                X = ingredient.Position.X,
                Y = 0.01f,
                Z = ingredient.Position.Z
            },
            Name = ingredient.Type.ToString(),
        };

        placedIngredients.Enqueue(placedIngredient);
        AddChild(placedIngredient);
    }

    /// <summary>
    /// Should be activated only, when player misses
    /// </summary>
    /// <param name="body"></param>
    private void FailureArea_BodyEntered(Node3D body)
    {
        // If is needed to prevent retriggers. If the ingredient is falling, hit's and get's reset, gravity will be "0" => retriggers can't happen
        if (ingredient.GravityScale == 0)
        {
            return;
        }

        GD.Print("Failure Body Entered");

        // Spawn the "placed" ingredient on the dropped position
        PlaceIngredient();

        pizzaValue += failurePlacementValue;
        ResetIngredient();
    }

    public override void _Input(InputEvent @event)
    {
        if (!isActive)
        {
            return;
        }

        if (Input.IsActionPressed(ActionNames.DEBUG_TOGGLEMOUSE) && isActive)
        {
            LeaveMinigame();
        }
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

        if (Input.IsActionJustPressed(ActionNames.Use))
        {
            GD.Print("Touched");
            ingredient.GravityScale = 1;
        }

        // Player can move the ingredient only, if the GravityScale is 0
        Vector2 inputDir = Input.GetVector(ActionNames.Move_Left, ActionNames.Move_Right, ActionNames.Move_Forward, ActionNames.Move_Backwards);
        if (inputDir != Vector2.Zero && ingredient.GravityScale == 0)
        {
            var newPosition = new Vector3()
            {
                X = ingredient.Position.X - inputDir.X * 0.002f,
                Y = ingredient.Position.Y,
                Z = ingredient.Position.Z - inputDir.Y * 0.002f
            };

            if (dough.Position.DistanceTo(newPosition) < maxDistanceFromCenter)
            {
                ingredient.Position = newPosition;
            }
        }
    }

    /// <summary>
    /// Resets the minigame into its original state
    /// </summary>
    protected override void ResetMinigame()
    {
        
    }
}
