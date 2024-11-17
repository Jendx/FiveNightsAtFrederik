using Godot;
using System;

namespace FiveNightsAtFrederik.CsScripts.Helpers;

public static class Vector3Helper
{
    private static readonly Random random = new Random();

    /// <summary>
    /// Finds random position inside a circle
    /// </summary>
    /// <param name="centerOfCircle"></param>
    /// <param name="radius"></param>
    /// <param name="minAngle">min: 0</param>
    /// <param name="maxAngle">max: 361</param>
    /// <returns></returns>
    public static Vector3 GetRandomPositionInCircle(Vector3 centerOfCircle, float y, float radius, int minAngle = 0, int maxAngle = 361)
    {
        return new Vector3()
        {
            X = centerOfCircle.X + radius * Mathf.Cos(random.Next(minAngle, maxAngle)),
            Y = y,
            Z = centerOfCircle.Z + radius * Mathf.Sin(random.Next(minAngle, maxAngle))
        };
    }

    public static Vector3 GetRandomPositionInCircle(Vector3 centerOfCircle, float radius, int minAngle = 0, int maxAngle = 361) 
        => GetRandomPositionInCircle(centerOfCircle, centerOfCircle.Y, radius, minAngle, maxAngle);
}
