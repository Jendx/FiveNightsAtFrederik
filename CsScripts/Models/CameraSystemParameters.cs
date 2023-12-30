using FiveNightsAtFrederik.CsScripts.Models.Interfaces;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Models;

[GlobalClass]
public partial class CameraSystemParameters : BaseUsableParameters
{
	[Export]
	public string CameraName { get; set; }
}

