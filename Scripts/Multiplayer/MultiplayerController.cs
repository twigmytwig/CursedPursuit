using CursedPursuit.Scripts.Multiplayer;
using Godot;
using System;

public partial class MultiplayerController : Control
{
	[Export]
	private int port = 8910;//Port number that the server is listening on
	[Export]
	private string address = "127.0.0.1"; // local host, no servers... yet

	private ENetMultiplayerPeer peer;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
	}

	


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// runs when conenction fails and only runs on the client
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void ConnectionFailed()
	{
		GD.Print("CONNECTION FAILED");
	}

	/// <summary>
	/// Runs wwhen the connection is successful and only on client
	/// </summary>
	/// <exception cref="NotImplementedException"></exception>
	private void ConnectedToServer()
	{
		GD.Print("CONNECTED TO SERVER");
		RpcId(1, "SendPlayerInfo", GetNode<LineEdit>("NameField").Text, Multiplayer.GetUniqueId()); //RPCID 1 means only the server can ruin this
	}

	/// <summary>
	/// Runs when a player disconnects and runs on all peers
	/// </summary>
	/// <param name="id">id of the player that disconnected</param>
	/// <exception cref="NotImplementedException"></exception>
	private void PeerDisconnected(long id)
	{
		GD.Print($"PLAYER DISCONNECTED: {id}");
	}

	/// <summary>
	/// Runs when a player connects and runs on all peers
	/// </summary>
	/// <param name="id">id of the player that connected</param>
	/// <exception cref="NotImplementedException"></exception>
	public void PeerConnected(long id)
	{
		GD.Print($"PLAYER CONNECTED: {id}");
	}

	private void _on_host_button_down()
	{
		// Replace with function body.
		peer = new ENetMultiplayerPeer();
		var error = peer.CreateServer(port, 5); //TODO: DETERMINE HOW MANY CLIENTS MAX I WANT

		if (error != Error.Ok)
		{
			GD.Print("Error! Cannot host!: " + error.ToString());
			return;
		}

		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder); //TODO: FIND WHAT THE BEST COMPRESSION ALGO IS ACTUALYL
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Waiting for players...");

		SendPlayerInfo(GetNode<LineEdit>("NameField").Text, 1); //ID is 1 bc host
	}


	private void _on_join_button_down()
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateClient(address, port);

		peer.Host.Compress(ENetConnection.CompressionMode.RangeCoder); //TODO: FIND WHAT THE BEST COMPRESSION ALGO IS ACTUALYL -> Has to be the saem compressiong that the host has  ^^^
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Joining game...");
	}


	private void _on_start_button_down()
	{
		Rpc("startGame");
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true,TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void startGame()
	{
		foreach (var item in GameManager.Players)
		{
			GD.Print(item.Name + " : " + item.Id.ToString());
		}
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/game.tscn").Instantiate<Node2D>();
		GetTree().Root.AddChild(scene);
		this.Hide();
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	private void SendPlayerInfo(string name, int id)
	{
		PlayerInfo playerInfo = new PlayerInfo()
		{
			Id = id,
			Name = name,
		};

		if (!GameManager.Players.Contains(playerInfo))
		{
			GameManager.Players.Add(playerInfo);
		}

		if (Multiplayer.IsServer())
		{
			foreach (var item in GameManager.Players)
			{
				Rpc("SendPlayerInfo", item.Name, item.Id);
			}
		}
	}
}



