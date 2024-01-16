namespace FiveNightsAtFrederik.CsScripts.Interfaces;

internal interface IMovableCharacter
{
    public float MovementSpeed { get; }

    public float JumpVelocity { get; set; }

    public float RotationSpeed { get; set; }
}
