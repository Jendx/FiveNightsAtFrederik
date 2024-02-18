using System;

namespace FiveNightsAtFrederik.CsScripts.Interfaces;

/// <summary>
/// Handles Animations of other objects
/// TEnum must be of any Enum Type!
/// </summary>
internal interface IAnimated<TEnum>
{
    public TEnum? HandleAnimations();
}
