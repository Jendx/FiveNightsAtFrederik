using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Controllers;
using FiveNightsAtFrederik.CsScripts.Extensions;
using Godot;

namespace FiveNightsAtFrederik.scenes.level.Controls.WinControl;

public partial class WinScreen : Control
{
    private Label totalValueLabel;
    private Label winLabel;

    public override void _Ready()
    {
        totalValueLabel = this.TryGetNode<Label>(NodeNames.TotalValue, nameof(totalValueLabel));
        winLabel = this.TryGetNode<Label>(NodeNames.WinLabel, nameof(winLabel));
    }

    public void FinishGame()
    {
        var totalValue = OrderController.GetTotalValue();
        totalValueLabel.Text = $"{totalValue} Kč";

        if (totalValue < 0)
        {
            winLabel.Text = "YOU WIN, DID NOT!";
        }

        Show();

        var timer = GetTree().CreateTimer(10);
        timer.Timeout += () => GetTree().ChangeSceneToFile("res://scenes/main_menu/main_menu.tscn");
    }
}
