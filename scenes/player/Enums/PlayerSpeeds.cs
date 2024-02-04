namespace FiveNightsAtFrederik.scenes.player.Enums;

public enum PlayerSpeeds
{
    // speed of standing up/down. Values > 5 can introduce unexpected behaviour (teleporting to random direction when standing up)
    CrouchTransition = 3,
    Crouch = 100,
    ExhaustedWalk = 150,
    Walk = 200,
    Sprint = 275,
}
