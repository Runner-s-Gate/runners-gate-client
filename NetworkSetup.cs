using Godot;
using System;

public class NetworkSetup : Control
{
	private string playerName = "";
	
	private void _on_PlayerName_text_changed(String new_text)
	{
		playerName = new_text;
	}

	private void _on_JoinButton_pressed()
	{
		Network.GetInstance().SetPlayerName(playerName);
		Network.GetInstance().JoinGame();
		Hide();
	}
}
