using Godot;

namespace FiveNightsAtFrederik.CsScripts.Handlers;

public class PlayerMovementHandler
{
    private Player _player;
    private Vector3 _velocity = new Vector3();

    public PlayerMovementHandler(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// Handles movement on XY Axis
    /// </summary>
    public void HandleMovement()
    {
        Vector2 inputDir = Input.GetVector("Move_Left", "Move_Right", "Move_Forward", "Move_Backwards");
        Vector3 direction = (_player.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            _velocity.X = direction.X * _player.MovementSpeed;
            _velocity.Z = direction.Z * _player.MovementSpeed;
        }
        else
        {
            _velocity.X = Mathf.MoveToward(_player.Velocity.X, 0, _player.MovementSpeed);
            _velocity.Z = Mathf.MoveToward(_player.Velocity.Z, 0, _player.MovementSpeed);
        }

        _player.Velocity = _velocity;
        _player.MoveAndSlide();
    }

    private Vector2 HandleJump()
    {
        //// Add the gravity.
        //if (!IsOnFloor())
        //    velocity.Y -= gravity * (float)delta;

        //// Handle Jump.
        //if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        //    velocity.Y = JumpVelocity;
        return Vector2.Zero;
    }

    /// <summary>
    /// Rotates Player model & camera
    /// </summary>
    /// <param name="mouseDelta">new rotation</param>
    /// <param name="camera">Player camera</param>
    public void RotateByMouseDelta(Vector2 mouseDelta, Camera3D camera)
    {
        // Rotate the player around the Y-axis (left and right)
        _player.RotateY(-mouseDelta.X * _player.RotationSpeed);

        // Rotate the camera around the X-axis (up and down)
        camera.RotateX(-mouseDelta.Y * _player.RotationSpeed);

        // Clamp the camera's rotation so you can't look too far up or down
        float cameraRotation = Mathf.Clamp(camera.RotationDegrees.X, -70, 70);
        camera.RotationDegrees = new Vector3(cameraRotation, 0, 0);
    }
}
