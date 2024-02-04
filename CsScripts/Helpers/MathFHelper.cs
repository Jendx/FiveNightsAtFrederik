namespace FiveNightsAtFrederik.CsScripts.Helpers;

public static class MathFHelper
{
    /// <summary>
    /// Maps value from input range to output range
    /// </summary>
    /// <param name="value"></param>
    /// <param name="inputA"></param>
    /// <param name="inputB"></param>
    /// <param name="outputA"></param>
    /// <param name="outputB"></param>
    /// <returns></returns>
    public static float Map(float value, float inputA, float inputB, float outputA, float outputB)
        => (value - inputA) / (inputB - inputA) * (outputB - outputA) + outputA;
}
