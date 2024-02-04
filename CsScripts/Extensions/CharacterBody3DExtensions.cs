using Godot;

namespace FiveNightsAtFrederik.CsScripts.Extensions;

public static class CharacterBody3DExtensions
{
    /// <summary>
    /// Handles smooth rotation
    /// Ps: DO NOT TOUCH RETURNING THE VALUE BREAKS EVERYTHING
    /// </summary>
    /// <param name="node"></param>
    /// <param name="forwardPositionOrigin"></param>
    /// <param name="nextPosition"></param>
    /// <param name="rotationSpeed"> recommended value is between <0.1, 5> and 5 is really fast!</param>
    /// <param name="delta"></param>
    /// <returns></returns>
    public static void RotateYByShortestWayToTarget(this CharacterBody3D node, Marker3D LookForwardMarker, Vector3 nextPosition, float rotationSpeed, float delta)
    {
        Vector3 directionToNextPosition = (nextPosition - LookForwardMarker.GlobalTransform.Origin).Normalized();
        float targetYRotation = Mathf.Atan2(directionToNextPosition.X, directionToNextPosition.Z);

        float currentYRotation = node.GlobalTransform.Basis.GetEuler().Y;
        float clockwiseDifference = (targetYRotation - currentYRotation + 2 * Mathf.Pi) % (2 * Mathf.Pi);
        float counterClockwiseDifference = (currentYRotation - targetYRotation + 2 * Mathf.Pi) % (2 * Mathf.Pi);

        var newRotation = clockwiseDifference < counterClockwiseDifference
            ? node.GlobalRotation.Y - rotationSpeed * delta
            : node.GlobalRotation.Y + rotationSpeed * delta;

        GD.Print(Mathf.RadToDeg(clockwiseDifference), Mathf.RadToDeg(counterClockwiseDifference));
        node.GlobalRotation = new Vector3()
        {
            X = node.GlobalRotation.X,
            Y = newRotation,
            Z = node.GlobalRotation.Z,
        };
    }
}
