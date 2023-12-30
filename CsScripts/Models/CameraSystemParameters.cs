using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Models;

public partial class CameraSystemParameters : Resource, IUsableParameters
{
	[Export]
	public string SelectedCameraName { get; set; }
}

