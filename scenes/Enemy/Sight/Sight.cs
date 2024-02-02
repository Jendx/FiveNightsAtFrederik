using FiveNightsAtFrederik.CsScripts.Constants;
using FiveNightsAtFrederik.CsScripts.Extensions;
using Godot;

namespace FiveNightsAtFrederik.Scenes.Enemy;

public partial class Sight : Node3D
{
    private Area3D sightArea;

    public override void _Ready()
    {
        sightArea = GetNode<Area3D>(NodeNames.UseDelayTimer.ToString()) ?? throw new NativeMemberNotFoundException($"Node: {Name} failed to find {nameof(sightArea)} at {NodeNames.UseDelayTimer}");
        sightArea = this.TryGetNode<Area3D>(NodeNames.UseDelayTimer, nameof(sightArea)); 
    }
}
