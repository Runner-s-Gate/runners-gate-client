using Godot;
using System;

public class NetworkSetup : Control
{
	private string ipAddr = "20.91.204.218";
	private string playerName = "";

	private void _on_IpAddress_text_changed(String new_text)
	{
		ipAddr = new_text;
	}
	
	private void _on_PlayerName_text_changed(String new_text)
	{
		playerName = new_text;
	}

	private void _on_JoinButton_pressed()
	{
		Network.GetInstance().SetPlayerName(playerName);
		Network.GetInstance().JoinGame(ipAddr);
		Hide();
	}
}
