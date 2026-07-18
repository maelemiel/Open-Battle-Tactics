using System.Collections.Generic;
using Facebook;
using Facebook.MiniJSON;
using UnityEngine;

public sealed class GameGroupsConsole : ConsoleBase
{
	public string GamerGroupName = "Test group";

	public string GamerGroupDesc = "Test group for testing.";

	public string GamerGroupPrivacy = "closed";

	public string GamerGroupAdmin = string.Empty;

	public string GamerGroupCurrentGroup = string.Empty;

	private void GroupCreateCB(FBResult result)
	{
		Callback(result);
		if (result.Text != null)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(result.Text);
			if (dictionary.ContainsKey("id"))
			{
				GamerGroupCurrentGroup = (string)dictionary["id"];
			}
		}
	}

	private void GroupDeleteCB(FBResult result)
	{
		Callback(result);
		GamerGroupCurrentGroup = string.Empty;
	}

	private void GetAllGroupsCB(FBResult result)
	{
		if (!string.IsNullOrEmpty(result.Text))
		{
			lastResponse = result.Text;
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(result.Text);
			if (dictionary.ContainsKey("data"))
			{
				List<object> list = (List<object>)dictionary["data"];
				if (list.Count > 0)
				{
					Dictionary<string, object> dictionary2 = (Dictionary<string, object>)list[0];
					GamerGroupCurrentGroup = (string)dictionary2["id"];
				}
			}
		}
		if (!string.IsNullOrEmpty(result.Error))
		{
			lastResponse = result.Error;
		}
	}

	private void CallFbGetAllOwnedGroups()
	{
		FB.API(FB.AppId + "/groups", HttpMethod.GET, GetAllGroupsCB);
	}

	private void CallFbGetUserGroups()
	{
		FB.API("/me/groups?parent=" + FB.AppId, HttpMethod.GET, base.Callback);
	}

	private void CallCreateGroupDialog()
	{
		FB.GameGroupCreate(GamerGroupName, GamerGroupDesc, GamerGroupPrivacy, GroupCreateCB);
	}

	private void CallJoinGroupDialog()
	{
		FB.GameGroupJoin(GamerGroupCurrentGroup, GroupCreateCB);
	}

	private void CallFbPostToGamerGroup()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["message"] = "herp derp a post";
		FB.API(GamerGroupCurrentGroup + "/feed", HttpMethod.POST, base.Callback, dictionary);
	}

	private void OnGUI()
	{
		AddCommonHeader();
		GUI.enabled = FB.IsLoggedIn;
		if (Button("Get All App Managed Groups"))
		{
			CallFbGetAllOwnedGroups();
		}
		LabelAndTextField("Group Name", ref GamerGroupName);
		LabelAndTextField("Group Description", ref GamerGroupDesc);
		LabelAndTextField("Group Privacy", ref GamerGroupPrivacy);
		if (Button("Call Create Group Dialog"))
		{
			CallCreateGroupDialog();
		}
		LabelAndTextField("Group To Join", ref GamerGroupCurrentGroup);
		if (Button("Call Join Group Dialog"))
		{
			CallJoinGroupDialog();
		}
		if (Button("Get Gamer Groups Logged in User Belongs to"))
		{
			CallFbGetUserGroups();
		}
		GUILayout.Space(10f);
		if (Button("Make Group Post As User"))
		{
			CallFbPostToGamerGroup();
		}
		GUILayout.Space(10f);
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		if (IsHorizontalLayout())
		{
			GUILayout.EndVertical();
		}
		GUI.enabled = true;
		AddCommonFooter();
		if (IsHorizontalLayout())
		{
			GUILayout.EndHorizontal();
		}
	}
}
