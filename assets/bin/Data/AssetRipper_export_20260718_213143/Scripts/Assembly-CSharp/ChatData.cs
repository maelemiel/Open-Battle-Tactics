using System.Collections.Generic;

public class ChatData
{
	public List<ChatMessage> chatMessages = new List<ChatMessage>();

	public string lastChatSequence = "0";

	public float lastFetchTime;
}
