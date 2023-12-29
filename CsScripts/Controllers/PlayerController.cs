using FiveNightsAtFrederik.Constants;
using FiveNightsAtFrederik.CsScripts.Interfaces;
using FiveNightsAtFrederik.Scenes.Player;
using Godot;

namespace FiveNightsAtFrederik.CsScripts.Controllers;

public class PlayerController
{
    private readonly Player _player;
    private Vector3 _velocity = new();

    private GodotObject _colidingObject;
    private StaticBody3D _usableObject;

    public PlayerController(Player player)
    {
        _player = player;
    }

    /// <summary>
    /// Handles movement on XY Axis
    /// </summary>
    public void HandleMovement()
    {
        Vector2 inputDir = Input.GetVector(ActionNames.Move_Left, ActionNames.Move_Right, ActionNames.Move_Forward, ActionNames.Move_Backwards);
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

    public void UpdateLookAtObject(RayCast3D rayCast)
    {
        var newColidingObject = rayCast.GetCollider();

        if (_colidingObject != newColidingObject && Input.IsActionPressed(ActionNames.Use))
        {
            StopUsing();
        }

        _colidingObject = newColidingObject;

        var isUsableObject = _colidingObject is not null || _colidingObject is StaticBody3D;
        if (!isUsableObject)
        {
            _usableObject = null;
            _player.EmitSignal(nameof(_player.UsableObjectChanged), isUsableObject);
            return;
        }

        
        _usableObject = (StaticBody3D)_colidingObject;
        _player.EmitSignal(nameof(_player.UsableObjectChanged), isUsableObject);
    }


    public void Use()
    {
        if (_usableObject is not null && _usableObject.Owner.HasMethod(nameof(IUsable.OnBeginUse)))
        {
            _usableObject.Owner.Call(nameof(IUsable.OnBeginUse), false);
        }
    }

    public void StopUsing()
    {
        if (_usableObject is not null && _usableObject.Owner.HasMethod(nameof(IUsable.OnEndUse)))
        {
            _usableObject.Owner.Call(nameof(IUsable.OnEndUse), false);
        }
    }
}
