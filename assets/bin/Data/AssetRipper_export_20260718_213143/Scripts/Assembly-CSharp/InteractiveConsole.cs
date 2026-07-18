using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Facebook;
using UnityEngine;

public sealed class InteractiveConsole : ConsoleBase
{
	public string FriendSelectorTitle = string.Empty;

	public string FriendSelectorMessage = "Derp";

	private string[] FriendFilterTypes = new string[3] { "None (default)", "app_users", "app_non_users" };

	private int FriendFilterSelection;

	private List<string> FriendFilterGroupNames = new List<string>();

	private List<string> FriendFilterGroupIDs = new List<string>();

	private int NumFriendFilterGroups;

	public string FriendSelectorData = "{}";

	public string FriendSelectorExcludeIds = string.Empty;

	public string FriendSelectorMax = string.Empty;

	public string DirectRequestTitle = string.Empty;

	public string DirectRequestMessage = "Herp";

	private string DirectRequestTo = string.Empty;

	public string FeedToId = string.Empty;

	public string FeedLink = string.Empty;

	public string FeedLinkName = string.Empty;

	public string FeedLinkCaption = string.Empty;

	public string FeedLinkDescription = string.Empty;

	public string FeedPicture = string.Empty;

	public string FeedMediaSource = string.Empty;

	public string FeedActionName = string.Empty;

	public string FeedActionLink = string.Empty;

	public string FeedReference = string.Empty;

	public bool IncludeFeedProperties;

	private Dictionary<string, string[]> FeedProperties = new Dictionary<string, string[]>();

	public string PayProduct = string.Empty;

	public string ApiQuery = string.Empty;

	public string Width = "800";

	public string Height = "600";

	public bool CenterHorizontal = true;

	public bool CenterVertical;

	public string Top = "10";

	public string Left = "10";

	private void CallFBActivateApp()
	{
		FB.ActivateApp();
		Callback(new FBResult("Check Insights section for your app in the App Dashboard under \"Mobile App Installs\""));
	}

	private void CallAppRequestAsFriendSelector()
	{
		int? maxRecipients = null;
		if (FriendSelectorMax != string.Empty)
		{
			try
			{
				maxRecipients = int.Parse(FriendSelectorMax);
			}
			catch (Exception ex)
			{
				status = ex.Message;
			}
		}
		string[] excludeIds = ((!(FriendSelectorExcludeIds == string.Empty)) ? FriendSelectorExcludeIds.Split(',') : null);
		List<object> list = new List<object>();
		if (FriendFilterSelection > 0)
		{
			list.Add(FriendFilterTypes[FriendFilterSelection]);
		}
		if (NumFriendFilterGroups > 0)
		{
			for (int i = 0; i < NumFriendFilterGroups; i++)
			{
				list.Add(new FBAppRequestsFilterGroup(FriendFilterGroupNames[i], FriendFilterGroupIDs[i].Split(',').ToList()));
			}
		}
		FB.AppRequest(FriendSelectorMessage, null, (list.Count <= 0) ? null : list, excludeIds, maxRecipients, FriendSelectorData, FriendSelectorTitle, base.Callback);
	}

	private void CallAppRequestAsDirectRequest()
	{
		if (DirectRequestTo == string.Empty)
		{
			throw new ArgumentException("\"To Comma Ids\" must be specificed", "to");
		}
		FB.AppRequest(DirectRequestMessage, DirectRequestTo.Split(','), null, null, null, string.Empty, DirectRequestTitle, base.Callback);
	}

	private void CallFBFeed()
	{
		Dictionary<string, string[]> properties = null;
		if (IncludeFeedProperties)
		{
			properties = FeedProperties;
		}
		FB.Feed(FeedToId, FeedLink, FeedLinkName, FeedLinkCaption, FeedLinkDescription, FeedPicture, FeedMediaSource, FeedActionName, FeedActionLink, FeedReference, properties, base.Callback);
	}

	private void CallFBPay()
	{
		FB.Canvas.Pay(PayProduct);
	}

	private void CallFBAPI()
	{
		FB.API(ApiQuery, HttpMethod.GET, base.Callback);
	}

	private void CallFBGetDeepLink()
	{
		FB.GetDeepLink(base.Callback);
	}

	public void CallAppEventLogEvent()
	{
		FB.AppEvents.LogEvent("fb_mobile_achievement_unlocked", null, new Dictionary<string, object> { { "fb_description", "Clicked 'Log AppEvent' button" } });
		Callback(new FBResult("You may see results showing up at https://www.facebook.com/insights/" + FB.AppId + "?section=AppEvents"));
	}

	public void CallCanvasSetResolution()
	{
		int result;
		if (!int.TryParse(Width, out result))
		{
			result = 800;
		}
		int result2;
		if (!int.TryParse(Height, out result2))
		{
			result2 = 600;
		}
		float result3;
		if (!float.TryParse(Top, out result3))
		{
			result3 = 0f;
		}
		float result4;
		if (!float.TryParse(Left, out result4))
		{
			result4 = 0f;
		}
		if (CenterHorizontal && CenterVertical)
		{
			FB.Canvas.SetResolution(result, result2, false, 0, FBScreen.CenterVertical(), FBScreen.CenterHorizontal());
		}
		else if (CenterHorizontal)
		{
			FB.Canvas.SetResolution(result, result2, false, 0, FBScreen.Top(result3), FBScreen.CenterHorizontal());
		}
		else if (CenterVertical)
		{
			FB.Canvas.SetResolution(result, result2, false, 0, FBScreen.CenterVertical(), FBScreen.Left(result4));
		}
		else
		{
			FB.Canvas.SetResolution(result, result2, false, 0, FBScreen.Top(result3), FBScreen.Left(result4));
		}
	}

	protected override void Awake()
	{
		base.Awake();
		FeedProperties.Add("key1", new string[1] { "valueString1" });
		FeedProperties.Add("key2", new string[2] { "valueString2", "http://www.facebook.com" });
	}

	private void FriendFilterArea()
	{
		GUILayout.Label("Filters:");
		FriendFilterSelection = GUILayout.SelectionGrid(FriendFilterSelection, FriendFilterTypes, 3, GUILayout.MinHeight(buttonHeight));
	}

	private void OnGUI()
	{
		AddCommonHeader();
		if (Button("Publish Install"))
		{
			CallFBActivateApp();
			status = "Install Published";
		}
		GUI.enabled = FB.IsLoggedIn;
		GUILayout.Space(10f);
		LabelAndTextField("Title (optional): ", ref FriendSelectorTitle);
		LabelAndTextField("Message: ", ref FriendSelectorMessage);
		LabelAndTextField("Exclude Ids (optional): ", ref FriendSelectorExcludeIds);
		FriendFilterArea();
		LabelAndTextField("Max Recipients (optional): ", ref FriendSelectorMax);
		LabelAndTextField("Data (optional): ", ref FriendSelectorData);
		if (Button("Open Friend Selector"))
		{
			try
			{
				CallAppRequestAsFriendSelector();
				status = "Friend Selector called";
			}
			catch (Exception ex)
			{
				status = ex.Message;
			}
		}
		GUILayout.Space(10f);
		LabelAndTextField("Title (optional): ", ref DirectRequestTitle);
		LabelAndTextField("Message: ", ref DirectRequestMessage);
		LabelAndTextField("To Comma Ids: ", ref DirectRequestTo);
		if (Button("Open Direct Request"))
		{
			try
			{
				CallAppRequestAsDirectRequest();
				status = "Direct Request called";
			}
			catch (Exception ex2)
			{
				status = ex2.Message;
			}
		}
		GUILayout.Space(10f);
		LabelAndTextField("To Id (optional): ", ref FeedToId);
		LabelAndTextField("Link (optional): ", ref FeedLink);
		LabelAndTextField("Link Name (optional): ", ref FeedLinkName);
		LabelAndTextField("Link Desc (optional): ", ref FeedLinkDescription);
		LabelAndTextField("Link Caption (optional): ", ref FeedLinkCaption);
		LabelAndTextField("Picture (optional): ", ref FeedPicture);
		LabelAndTextField("Media Source (optional): ", ref FeedMediaSource);
		LabelAndTextField("Action Name (optional): ", ref FeedActionName);
		LabelAndTextField("Action Link (optional): ", ref FeedActionLink);
		LabelAndTextField("Reference (optional): ", ref FeedReference);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Properties (optional)", GUILayout.Width(150f));
		IncludeFeedProperties = GUILayout.Toggle(IncludeFeedProperties, "Include");
		GUILayout.EndHorizontal();
		if (Button("Open Feed Dialog"))
		{
			try
			{
				CallFBFeed();
				status = "Feed dialog called";
			}
			catch (Exception ex3)
			{
				status = ex3.Message;
			}
		}
		GUILayout.Space(10f);
		LabelAndTextField("API: ", ref ApiQuery);
		if (Button("Call API"))
		{
			status = "API called";
			CallFBAPI();
		}
		GUILayout.Space(10f);
		if (Button("Take & upload screenshot"))
		{
			status = "Take screenshot";
			StartCoroutine(TakeScreenshot());
		}
		if (Button("Get Deep Link"))
		{
			CallFBGetDeepLink();
		}
		GUI.enabled = true;
		if (Button("Log FB App Event"))
		{
			status = "Logged FB.AppEvent";
			CallAppEventLogEvent();
		}
		GUILayout.Space(10f);
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		if (IsHorizontalLayout())
		{
			GUILayout.EndVertical();
		}
		AddCommonFooter();
		if (IsHorizontalLayout())
		{
			GUILayout.EndHorizontal();
		}
	}

	private IEnumerator TakeScreenshot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		byte[] screenshot = tex.EncodeToPNG();
		WWWForm wwwForm = new WWWForm();
		wwwForm.AddBinaryData("image", screenshot, "InteractiveConsole.png");
		wwwForm.AddField("message", "herp derp.  I did a thing!  Did I do this right?");
		FB.API("me/photos", HttpMethod.POST, base.Callback, wwwForm);
	}
}
