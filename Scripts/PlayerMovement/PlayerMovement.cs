using Godot;
using System;

public partial class PlayerMovement : CharacterBody2D
{
	[Export]
	public int Speed = 100; // Movement speed

	private Vector2 _velocity = new Vector2();

	public override void _PhysicsProcess(double delta)
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
