using Godot;

namespace FiveNightsAtFrederik.CsScripts.Models;

public sealed class LocationMarkers
{
    public bool IsOccupied { get; set; }
    public Marker3D Marker { get; set; }
    public string OccupiedBy { get; set; }
}