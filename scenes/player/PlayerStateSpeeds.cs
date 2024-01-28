namespace FiveNightsAtFrederik.scenes.player;

public enum PlayerStateSpeeds
{
	// speed of standing up/down. Values > 5 can introduce unexpected behaviour (teleporting to random direction when standing up)
	CrouchTransition = 3,
	Crouch = 100,
	Walk = 200,	
	Sprint = 275,
}
