using System.Collections;
using UnityEngine;

public class LoginSceneController : SceneController
{
	[SerializeField]
	private tk2dTextMesh guestButtonLabel;

	[SerializeField]
	private tk2dTextMesh legalTextLabel;

	[SerializeField]
	private tk2dTextMesh loginFbRewardLabel;

	private bool buttonDisabled;

	public override void Awake()
	{
		_showHomeButton = false;
		_showTopBar = false;
		loginFbRewardLabel.text = string.Format("ui_login_screen_fbLoginRewardLabel".Localize("YOU GET {0} IN-GAME CASH!"), Constants.FacebookConnectIncentive.ToString());
		base.Awake();
		SceneController.fireWorksEnable = true;
		base.SectionTitle = "Login Scene";
	}

	public override bool OnHomeButton()
	{
		return false;
	}

	private void Start()
	{
		StartCoroutine(WaitForLogin());
		LoadingPopupManager.ClearAllLoadingPopups();
	}

	private void Init()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
			Debug.LogWarning("Closing the app");
		}
	}

	private IEnumerator WaitForLogin()
	{
		while (!LoginController.IsLogged)
		{
			yield return 0;
		}
		SceneController.fireWorksEnable = false;
		Object.Destroy(base.gameObject);
	}

	private void OnLoadAccount()
	{
		LoginController.OnLoadAccount();
	}

	private void OnMobageLogin()
	{
		if (!buttonDisabled)
		{
			buttonDisabled = true;
			LoginController.Login(string.Empty);
		}
	}

	private void Renabled(bool success)
	{
		if (!success)
		{
			buttonDisabled = false;
		}
	}

	private void OnGuestLogin()
	{
		if (!buttonDisabled)
		{
			buttonDisabled = true;
			LoginController.Login(string.Empty);
		}
	}

	public void OnClickTermsOfService()
	{
		Application.OpenURL("terms_of_service_url".Localize("http://app.mobage.com/terms"));
	}

	public void OnClickPrivacyPolicy()
	{
		Application.OpenURL("privacy_policy_url".Localize("http://app.mobage.com/privacy"));
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		if (focusStatus)
		{
			LoadingPopupManager.ClearAllLoadingPopups();
		}
	}
}
