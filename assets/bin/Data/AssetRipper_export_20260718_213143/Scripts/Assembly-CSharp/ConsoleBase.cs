using UnityEngine;

public class ConsoleBase : MonoBehaviour
{
	protected string status = "Ready";

	protected string lastResponse = string.Empty;

	public GUIStyle textStyle = new GUIStyle();

	protected Texture2D lastResponseTexture;

	protected Vector2 scrollPosition = Vector2.zero;

	protected int buttonHeight = 60;

	protected int mainWindowWidth = Screen.width - 30;

	protected int mainWindowFullWidth = Screen.width;

	protected int TextWindowHeight
	{
		get
		{
			return (!IsHorizontalLayout()) ? 85 : Screen.height;
		}
	}

	protected virtual void Awake()
	{
		textStyle.alignment = TextAnchor.UpperLeft;
		textStyle.wordWrap = true;
		textStyle.padding = new RectOffset(10, 10, 10, 10);
		textStyle.stretchHeight = true;
		textStyle.stretchWidth = false;
	}

	protected bool Button(string label)
	{
		return GUILayout.Button(label, GUILayout.MinHeight(buttonHeight), GUILayout.MaxWidth(mainWindowWidth));
	}

	protected void LabelAndTextField(string label, ref string text)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label(label, GUILayout.MaxWidth(150f));
		text = GUILayout.TextField(text);
		GUILayout.EndHorizontal();
	}

	protected bool IsHorizontalLayout()
	{
		return Screen.orientation == ScreenOrientation.LandscapeLeft;
	}

	protected void Callback(FBResult result)
	{
		lastResponseTexture = null;
		if (!string.IsNullOrEmpty(result.Error))
		{
			lastResponse = "Error Response:\n" + result.Error;
		}
		else if (!string.IsNullOrEmpty(result.Text))
		{
			lastResponse = "Success Response:\n" + result.Text;
		}
		else if (result.Texture != null)
		{
			lastResponseTexture = result.Texture;
			lastResponse = "Success Response: texture\n";
		}
		else
		{
			lastResponse = "Empty Response\n";
		}
	}

	protected void AddCommonFooter()
	{
		Rect rect = GUILayoutUtility.GetRect(640f, TextWindowHeight);
		GUI.TextArea(rect, string.Format(" AppId: {0} \n UserId: {1}\n IsLoggedIn: {2}\n {3}", FB.AppId, FB.UserId, FB.IsLoggedIn, lastResponse), textStyle);
		if (lastResponseTexture != null)
		{
			float num = rect.y + 200f;
			if ((float)(Screen.height - lastResponseTexture.height) < num)
			{
				num = Screen.height - lastResponseTexture.height;
			}
			GUI.Label(new Rect(rect.x + 5f, num, lastResponseTexture.width, lastResponseTexture.height), lastResponseTexture);
		}
	}

	protected void AddCommonHeader()
	{
		if (IsHorizontalLayout())
		{
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
		}
		GUILayout.Space(5f);
		GUILayout.Box("Status: " + status, GUILayout.MinWidth(mainWindowWidth));
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			scrollPosition.y += Input.GetTouch(0).deltaPosition.y;
		}
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.MinWidth(mainWindowFullWidth));
		GUILayout.BeginVertical();
		GUI.enabled = !FB.IsInitialized;
		if (Button("FB.Init"))
		{
			CallFBInit();
			status = "FB.Init() called with " + FB.AppId;
		}
		GUILayout.BeginHorizontal();
		GUI.enabled = FB.IsInitialized;
		if (Button("Login"))
		{
			CallFBLogin();
			status = "Login called";
		}
		GUI.enabled = FB.IsLoggedIn;
		if (Button("Get publish_actions"))
		{
			CallFBLoginForPublish();
			status = "Login (for publish_actions) called";
		}
		if (Button("Logout"))
		{
			CallFBLogout();
			status = "Logout called";
		}
		GUI.enabled = FB.IsInitialized;
		GUILayout.EndHorizontal();
	}

	private void CallFBInit()
	{
		FB.Init(OnInitComplete, OnHideUnity);
	}

	private void OnInitComplete()
	{
		Debug.Log("FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
	}

	private void OnHideUnity(bool isGameShown)
	{
		Debug.Log("Is game showing? " + isGameShown);
	}

	private void CallFBLogin()
	{
		FB.Login("public_profile,email,user_friends", LoginCallback);
	}

	private void CallFBLoginForPublish()
	{
		FB.Login("publish_actions", LoginCallback);
	}

	private void LoginCallback(FBResult result)
	{
		if (result.Error != null)
		{
			lastResponse = "Error Response:\n" + result.Error;
		}
		else if (!FB.IsLoggedIn)
		{
			lastResponse = "Login cancelled by Player";
		}
		else
		{
			lastResponse = "Login was successful!";
		}
	}

	private void CallFBLogout()
	{
		FB.Logout();
	}
}
