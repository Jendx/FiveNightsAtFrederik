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
using System.Linq;

namespace FiveNightsAtFrederik.scenes.level.Minigames.PizzaCrafting;

public partial class PizzaCraftingMinigame : BaseMinigame, IPlayerUsable
{
    public bool IsInteractionUIDisplayed { get; set; } = true;

    private readonly Queue<Ingredient> placedIngredients = new();
    private readonly List<Ingredient> availableIngredients = new();
    private MeshInstance3D? dough;
    private MeshInstance3D? targetMesh;
    private Ingredient? selectedIngredient;
    private PickupSpawner? pizzaSpawner;
    private Area3D? targetArea;
    private Area3D? failureArea;
    private Area3D? addIngredientArea;
    private Label? selectedIngredientText;
    private Label? noIngredientsLabel;

    private float foamRaisingTimerWaitTime;
    private float pizzaValue;
    private const float radius = 0.14f;
    private const float maxDistanceFromCenter = 0.23f;
    private const int successfullPlacementValue = 10;
    private const int failurePlacementValue = 3;
    private int selectedIngredientIndex = 0;

    public override void _Ready()
    {
        dough = this.TryGetNode<MeshInstance3D>(NodeNames.Dough, nameof(dough));
        targetMesh = this.TryGetNode<MeshInstance3D>(NodeNames.TargetMesh, nameof(targetMesh));
        targetArea = this.TryGetNode<Area3D>(NodeNames.TargetArea, nameof(targetArea));
        failureArea = this.TryGetNode<Area3D>(NodeNames.FailureArea, nameof(failureArea));
        addIngredientArea = this.TryGetNode<Area3D>(NodeNames.AddIngredientArea, nameof(addIngredientArea));
        selectedIngredient = this.TryGetNode<Ingredient>(NodeNames.Ingredient, nameof(selectedIngredient));
        pizzaSpawner = this.TryGetNode<PickupSpawner>(NodeNames.PizzaSpawner, nameof(pizzaSpawner));
        selectedIngredientText = this.TryGetNode<Label>(NodeNames.SelectedIngredientText, nameof(selectedIngredientText));
        noIngredientsLabel = this.TryGetNode<Label>(NodeNames.NoIngredientsLabel, nameof(noIngredientsLabel));

        targetArea.BodyEntered += TargetArea_BodyEntered;
        failureArea.BodyEntered += FailureArea_BodyEntered;
        addIngredientArea.BodyEntered += AddIngredientArea_BodyEntered;
        pizzaSpawner.OnObjectLeftSpawnArea += () =>
        {
            Visible = true;
            interactionCollision.Disabled = false;
        };

        base._Ready();
    }

    /// <summary>
    /// Spawns the "placed" ingredient on the dropped position
    /// and enqueues the ingredient into Queue
    /// </summary>
    private void PlaceIngredient()
    {
        var placedIngredient = new Ingredient()
        {
            Mesh = selectedIngredient.Mesh,
            PizzaDisplayMesh = selectedIngredient.PizzaDisplayMesh,
            Type = selectedIngredient.Type,
            Scale = selectedIngredient.Scale,
            Visible = true,
            Freeze = true,
            Position = new Vector3()
            {
                X = selectedIngredient.Position.X,
                Y = 0.01f,
                Z = selectedIngredient.Position.Z
            }
        };

        placedIngredients.Enqueue(placedIngredient);
        AddChild(placedIngredient);
        placedIngredient.UpdateCurrentDisplayedMesh(placedIngredient.PizzaDisplayMesh);
        selectedIngredient.Count--;

        // If we run out of the ingredient, delete & change to different
        if (selectedIngredient.Count <= 0)
        {
            availableIngredients.RemoveAt(selectedIngredientIndex);
            ChangeSelectedIngredient((int)ScrollingDirections.Up);
        }
    }

    /// <summary>
    /// Moves the selectedIngredient to position for next placement, turns off gravity and resets velocity
    /// </summary>
    private void ResetIngredient()
    {
        selectedIngredient.GravityScale = 0;
        selectedIngredient.LinearVelocity = Vector3.Zero;
        selectedIngredient.Position = new Vector3()
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
        if (selectedIngredient.GravityScale == 0)
        {
            return;
        }

        GD.Print("Successful Body Entered");

        // Add amount to pizza value dependent on distance from target
        var distanceToTargetMesh = selectedIngredient.Position.DistanceTo(targetMesh.Position);
        pizzaValue += Mathf.Remap(distanceToTargetMesh, 0.1f, 0.05f, 6, successfullPlacementValue);

        PlaceIngredient();

        // Set new targetPosition & refresh
        targetMesh.Position = Vector3Helper.GetRandomPositionInCircle(dough.Position, targetMesh.Position.Y, radius);
        ResetIngredient();
    }

    /// <summary>
    /// Should be activated only, when player misses
    /// </summary>
    /// <param name="body"></param>
    private void FailureArea_BodyEntered(Node3D body)
    {
        // If is needed to prevent retriggers. If the ingredient is falling, hit's and get's reset, gravity will be "0" => retriggers can't happen
        if (selectedIngredient.GravityScale == 0)
        {
            return;
        }

        GD.Print("Failure Body Entered");

        // Spawn the "placed" ingredient on the dropped position
        PlaceIngredient();

        pizzaValue += failurePlacementValue;
        ResetIngredient();
    }

    private void AddIngredientArea_BodyEntered(Node3D body)
    {
        if (body is Ingredient ingredient)
        {
            noIngredientsLabel.Hide();

            GD.Print("ADDED INGREDIENT");
            availableIngredients.Add(ingredient);
            ingredient.Visible = false;
            ingredient.ProcessMode = ProcessModeEnum.Disabled;

            // When we add first ingredient, we need to change to it & make selected ingredient visible
            if (availableIngredients.Count == 1)
            {
                ChangeSelectedIngredient((int)ScrollingDirections.Up);
                selectedIngredient.Show();
            }
        }
    }

    /// <summary>
    /// Handles switching of current ingredients
    /// </summary>
    /// <param name="direction"></param>
    private void ChangeSelectedIngredient(int direction)
    {
        if (availableIngredients.Count == 0)
        {
            noIngredientsLabel.Show();
            selectedIngredient.Hide();
            return;
        }

        //selectedIngredientIndex = Math.Clamp(selectedIngredientIndex, 0, availableIngredients.Count - 1);
        //availableIngredients[selectedIngredientIndex].Count = selectedIngredient.Count;
        //availableIngredients[selectedIngredientIndex].Mesh = selectedIngredient.Mesh;
        //availableIngredients[selectedIngredientIndex].PizzaDisplayMesh = selectedIngredient.PizzaDisplayMesh;
        //availableIngredients[selectedIngredientIndex].Type = selectedIngredient.Type;

        selectedIngredientIndex += direction;
        if (selectedIngredientIndex == availableIngredients.Count)
        {
            selectedIngredientIndex = 0;
        }

        if (selectedIngredientIndex == -1)
        {
            selectedIngredientIndex = availableIngredients.Count - 1;
        }

        selectedIngredient.Count = availableIngredients[selectedIngredientIndex].Count;
        selectedIngredient.Mesh = availableIngredients[selectedIngredientIndex].Mesh;
        selectedIngredient.PizzaDisplayMesh = availableIngredients[selectedIngredientIndex].PizzaDisplayMesh;
        selectedIngredient.Type = availableIngredients[selectedIngredientIndex].Type;
        selectedIngredient.UpdateCurrentDisplayedMesh(selectedIngredient.PizzaDisplayMesh);
        selectedIngredientText.Text = $"Selected Item: {Enum.GetName(selectedIngredient.Type)}";
        GD.Print("Scroll UP: " + selectedIngredient.Name);
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed(ActionNames.DEBUG_TOGGLEMOUSE) && isActive)
        {
            LeaveMinigame();
        }


        if (Input.IsActionJustPressed(ActionNames.Submit) && isActive)
        {
            FinishMinigame();
        }

        if (!isActive || availableIngredients.Count == 0)
        {
            return;
        }

        if (Input.IsActionPressed(ActionNames.Scroll_UP) && availableIngredients.Count > 1)
        {
            ChangeSelectedIngredient((int)ScrollingDirections.Up);
        }

        if (Input.IsActionPressed(ActionNames.Scroll_DOWN) && availableIngredients.Count > 1)
        {
            ChangeSelectedIngredient((int)ScrollingDirections.Down);
        }
    }

    public override void _Process(double delta)
    {
        if (!isActive || availableIngredients.Count == 0)
        {
            return;
        }

        if (Input.IsActionJustPressed(ActionNames.Use))
        {
            selectedIngredient.GravityScale = 1;
        }

        // Player can move the ingredient only, if the GravityScale is 0
        Vector2 inputDir = Input.GetVector(ActionNames.Move_Left, ActionNames.Move_Right, ActionNames.Move_Forward, ActionNames.Move_Backwards);
        if (inputDir != Vector2.Zero && selectedIngredient.GravityScale == 0 && availableIngredients.Count != 0)
        {
            var newPosition = new Vector3()
            {
                X = selectedIngredient.Position.X + inputDir.X * 0.002f,
                Y = selectedIngredient.Position.Y,
                Z = selectedIngredient.Position.Z + inputDir.Y * 0.002f
            };

            if (dough.Position.DistanceTo(newPosition) < maxDistanceFromCenter)
            {
                selectedIngredient.Position = newPosition;
            }
        }
    }

    protected override void FinishMinigame()
    {
        var pizza = pizzaSpawner.TrySpawnItem<CarriablePizza>();
        pizza.Reparent(GetTree().GetNodesInGroup(GroupNames.Levels.ToString()).FirstOrDefault());
        pizza.GlobalPosition = GlobalPosition;
        pizza.Value = pizzaValue;
        pizza.AddIngredients(placedIngredients);

        Visible = false;
        interactionCollision.Disabled = true;

        ResetMinigame();
        LeaveMinigame();
    }

    /// <summary>
    /// Resets the minigame into its original state
    /// </summary>
    protected override void ResetMinigame()
    {
        pizzaValue = 0;
        targetArea.Position = new Vector3()
        {
            X = 0,
            Y = targetArea.Position.Y,
            Z = 0
        };

        selectedIngredient.Position = new Vector3()
        {
            X = 0,
            Y = selectedIngredient.Position.Y,
            Z = 0
        };

        placedIngredients.Clear();
    }
}
