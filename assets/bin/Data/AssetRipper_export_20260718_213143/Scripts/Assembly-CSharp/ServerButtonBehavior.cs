using UnityEngine;

public class ServerButtonBehavior : MonoBehaviour
{
	public TextMesh text;

	[HideInInspector]
	public string ButtonName = string.Empty;

	[HideInInspector]
	public string ButtonUrl = string.Empty;

	[HideInInspector]
	public ServersSelectionPopUpController serverSelectionController;

	public void SetValues(string name, string url, ServersSelectionPopUpController sc)
	{
		ButtonName = name;
		text.text = name;
		ButtonUrl = url;
		serverSelectionController = sc;
	}

	private void OnMouseDown()
	{
	}
}
