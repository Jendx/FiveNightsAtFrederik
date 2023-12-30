namespace FiveNightsAtFrederik.CsScripts.Interfaces;

internal interface IMovableCharacter
{
    public float MovementSpeed { get; set; }

    public float JumpVelocity { get; set; }

    public float RotationSpeed { get; set; }
}
