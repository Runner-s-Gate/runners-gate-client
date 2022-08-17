using Godot;
using System;
using System.Collections.Generic;

public class Network : Node
{
	private static Network _instance;
	
	private readonly string ipAddr = (string)ConfigLoader.GetValue("network", "server_ip");
	private readonly int defaultPort = (int)ConfigLoader.GetValue("network", "server_port");

	private string PlayerName { get; set; }

	private Dictionary<int, string> Players = new Dictionary<int, string>();

	public static Network GetInstance() {
		return _instance;
	}

	public override void _Ready()
	{
		GetTree().Connect("network_peer_connected", this, nameof(PlayerConnected));
		GetTree().Connect("network_peer_disconnected", this, nameof(PlayerDisconnected));
		GetTree().Connect("connected_to_server", this, nameof(ConnectedToServer));
		GetTree().Connect("connection_failed", this, nameof(ConnectionFailed));
		GetTree().Connect("server_disconnected", this, nameof(ServerDisconnected));
		
		_instance = this;
	}

	public void JoinGame()
	{
		GD.Print($"Joining game with address: {ipAddr}, port: {defaultPort}");

		var clientPeer = new NetworkedMultiplayerENet();
		var result = clientPeer.CreateClient(ipAddr, defaultPort);

		GetTree().NetworkPeer = clientPeer;
	}

	public void LeaveGame()
	{
		GD.Print("Leaving current game");
		foreach(var player in Players)
		{
			GetNode(player.Key.ToString()).QueueFree();
		}

		Players.Clear();
		GetNode(GetTree().GetNetworkUniqueId().ToString()).QueueFree();
		Rpc(nameof(RemovePlayer), GetTree().GetNetworkUniqueId());

		((NetworkedMultiplayerENet)GetTree().NetworkPeer).CloseConnection();
		GetTree().NetworkPeer = null;
	}
	
	public void SetPlayerName(string playerName) {
		this.PlayerName = playerName + "cube";
	}

	public string GetPlayerName() {
		return this.PlayerName;
	}

	private void PlayerConnected(int id)
	{
		GD.Print($"tell other player my name is {PlayerName}");
		// tell the player that just connected who we are by sending an rpc back to them with your name.
		RpcId(id, nameof(RegisterPlayer), PlayerName);
	}

	private void PlayerDisconnected(int id)
	{
		GD.Print("Player disconnected");
		RemovePlayer(id);
	}

	private void ConnectedToServer()
	{
		GD.Print("Successfully connected to the server");
		StartGame();
	}

	private void ConnectionFailed()
	{
		GetTree().NetworkPeer = null;
		GD.Print("Failed to connect.");
	}

	private void ServerDisconnected()
	{
		GD.Print($"Disconnected from the server");
		LeaveGame();
	}

	[Remote]
	private void RegisterPlayer(string playerName)
	{
		var id = GetTree().GetRpcSenderId();
		Players.Add(id, playerName);
		GD.Print($"{playerName} added with ID {id}");
		// a player has been added spawn in the right location
		SpawnPlayer(id, playerName);
	}

	[Remote]
	private void StartGame()
	{
		// spawn yourself
		SpawnPlayer(GetTree().GetNetworkUniqueId(), PlayerName);
	}

	private void SpawnPlayer(int id, string playerName)
	{
		// load the players
		var playerScene = (PackedScene)ResourceLoader.Load("res://PlayerCharacter.tscn");

		var playerNode = (PlayerController)playerScene.Instance();
		playerNode.Name = id.ToString();
		playerNode.SetNetworkMaster(id);

		Transform t = playerNode.Transform;
		t.origin = new Vector3(0, 5, 0);
		playerNode.Transform = t;

		playerNode.SetPlayerName(GetTree().GetNetworkUniqueId() == id ? PlayerName : playerName);

		AddChild(playerNode);
	}

	[Remote]
	private void RemovePlayer(int id)
	{
		if (Players.ContainsKey(id))
		{
			Players.Remove(id);
			GetNode(id.ToString()).QueueFree();
		}
	}

}
