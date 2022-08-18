using Godot;
using System;

public class ChatController : Node
{
    private LineEdit chatEdit;
    private TextEdit chatLog;

    public override void _Ready() {
        this.chatEdit = (LineEdit)GetNode("VBoxContainer/ChatEdit");
        this.chatLog = (TextEdit)GetNode("VBoxContainer/ChatLog");
    }

	private void _on_ChatEdit_text_entered(string new_text)
	{
        // Call SubmitChatMessage on the server (id 1)
        RpcId(1, "SubmitChatMessage", Network.GetInstance().GetPlayerName(), new_text);
        this.chatEdit.Clear();
	}

    // Chat message receiver (only on the clients)
    [Puppet]
    private void ReceiveChatMessage(string playerName, string text)
    {
        GD.Print("Hello from RpcFunction! " + playerName + ", " + text);
        this.chatLog.Text += $"[{playerName}] {text}\n";
        this.chatLog.ScrollVertical = this.chatLog.GetLineCount();
    }

}
