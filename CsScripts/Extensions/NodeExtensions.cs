using FiveNightsAtFrederik.CsScripts.Constants;
using Godot;
using static Godot.GodotObject;

namespace FiveNightsAtFrederik.CsScripts.Extensions;

public static class NodeExtensions
{
    public static TNode TryGetNode<TNode>(this Node node, StringName nodeName, string variableName) where TNode: Node
    {
        return node.GetNode<TNode>(nodeName.ToString())
            ?? throw new NativeMemberNotFoundException($"Node: {node.Name} failed to find {variableName} at {nodeName}");
    }
}
