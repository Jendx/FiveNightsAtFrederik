using Godot;

namespace FiveNightsAtFrederik.CsScripts.Extensions;


#nullable enable
public static class GodotObjectExtensions
{
    public static TCast? TryConvertTo<TCast>(this GodotObject? godotObject) where TCast : class
    {
        return godotObject is TCast playerUsable
            ? playerUsable
            : ((Node?)godotObject)?.Owner as TCast;
    }
}
