using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
	[Export]
	public int Speed = 100; // Movement speed

	private Vector2 _velocity = new Vector2();

	private Camera2D camera;

	public override void _Ready()
	{
		GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").SetMultiplayerAuthority(int.Parse(Name));
		camera = GetNode<Camera2D>("Camera2D");

		if (GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			camera.MakeCurrent();
		}
	}
	public override void _PhysicsProcess(double delta)
	{
		if(GetNode<MultiplayerSynchronizer>("MultiplayerSynchronizer").GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			// Reset the velocity
			_velocity = Vector2.Zero;

			// Get input from WASD keys
			if (Input.IsActionPressed("move_right"))
			{
				_velocity.X += 1;
			}
			if (Input.IsActionPressed("move_left"))
			{
				_velocity.X -= 1;
			}
			if (Input.IsActionPressed("move_down"))
			{
				_velocity.Y += 1;
			}
			if (Input.IsActionPressed("move_up"))
			{
				_velocity.Y -= 1;
			}

			// Normalize the velocity so diagonal movement isn't faster
			if (_velocity.Length() > 0)
			{
				_velocity = _velocity.Normalized() * Speed;
			}

			// Move the character
			Velocity = _velocity;
			MoveAndSlide();
		}
	}
}
