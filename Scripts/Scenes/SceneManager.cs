using Godot;
using System;

public partial class SceneManager : Node2D
{
	[Export]
	private PackedScene playerScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int index = 0;
		foreach (var item in GameManager.Players)
		{
			PlayerMovement currentPlayer = playerScene.Instantiate<PlayerMovement>();
			currentPlayer.Name = item.Id.ToString();
			AddChild(currentPlayer);
			foreach(Node2D spawnPoint in GetTree().GetNodesInGroup("PlayerSpawnPoints"))
			{
				if(spawnPoint.Name == index.ToString()) 
				{
					index++;
					currentPlayer.GlobalPosition = spawnPoint.GlobalPosition;
					Camera2D camera = new Camera2D();
					if (currentPlayer.Name != 1.ToString())
					{
						currentPlayer.AddChild(camera);
					}
					
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
