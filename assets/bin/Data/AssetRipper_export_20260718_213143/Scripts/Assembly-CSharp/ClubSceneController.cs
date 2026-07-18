using System;
using UnityEngine;

public class ClubSceneController : SceneController
{
	[SerializeField]
	private GameObject[] tabGameObjects;

	[SerializeField]
	private tk2dUIToggleButtonGroup toggleButtonGroup;

	[SerializeField]
	private tk2dTextMesh myClubSelectedTabLabel;

	[SerializeField]
	private tk2dTextMesh myClubUnselectedTabLabel;

	[SerializeField]
	private tk2dTextMesh joinSelectedTabLabel;

	[SerializeField]
	private tk2dTextMesh joinUnselectedTabLabel;

	public string myClubLabelKey = string.Empty;

	public string createClubLabelKey = string.Empty;

	public string searchClubLabelKey = string.Empty;

	public string joinClubLabelKey = string.Empty;

	[SerializeField]
	private GameObject loadingIcon;

	private void Start()
	{
		base.SectionTitle = "Clubs";
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
		}
	}

	private void Init()
	{
		AudioTrigger.Map_Music.PlayMusic();
		allowsBackButton = true;
		UpdateTabLabels();
		UpdateClub();
	}

	private void OnToggleGroupButtonChange(tk2dUIToggleButtonGroup toggleButton)
	{
		if (tabGameObjects != null && tabGameObjects.Length != 0)
		{
			int selectedIndex = toggleButton.SelectedIndex;
			for (int i = 0; i < tabGameObjects.Length; i++)
			{
				tabGameObjects[i].SetActive(i == selectedIndex);
			}
		}
	}

	public void SetToggleGroupIndex(int selectedIndex)
	{
		if ((bool)toggleButtonGroup)
		{
			toggleButtonGroup.SelectedIndex = selectedIndex;
		}
	}

	public void UpdateClubInfo(Action callback)
	{
		if ((bool)loadingIcon)
		{
			loadingIcon.SetActive(true);
		}
		if (UserProfile.player == null)
		{
			return;
		}
		Singleton<SessionManager>.instance.GetClub(delegate(UserClub fetchedUserClub)
		{
			if (fetchedUserClub != null)
			{
				UserProfile.player.userClub = fetchedUserClub;
			}
			if ((bool)loadingIcon)
			{
				loadingIcon.SetActive(false);
			}
			if (callback != null)
			{
				callback();
			}
		});
	}

	public void UpdateMyClub()
	{
		if (UserProfile.player == null || !UserProfile.player.IsClubMember)
		{
			SetToggleGroupIndex(1);
		}
		else
		{
			SetToggleGroupIndex(0);
		}
		UpdateTabLabels();
	}

	public void UpdateClub()
	{
		UpdateClubInfo(UpdateMyClub);
	}

	public void UpdateTabLabels()
	{
		bool flag = UserProfile.player != null && UserProfile.player.IsClubMember;
		string text = ((!flag) ? createClubLabelKey : myClubLabelKey);
		string text2 = ((!flag) ? joinClubLabelKey : searchClubLabelKey);
		SetTabLabelKey(text, text2);
	}

	private void SetTabLabelKey(string myClubLabelKey, string joinClubLabelKey)
	{
		if ((bool)myClubSelectedTabLabel)
		{
			myClubSelectedTabLabel.text = myClubLabelKey.Localize("MyClub/CreateClub");
		}
		if ((bool)myClubUnselectedTabLabel)
		{
			myClubUnselectedTabLabel.text = myClubLabelKey.Localize("MyClub/CreateClub");
		}
		if ((bool)joinSelectedTabLabel)
		{
			joinSelectedTabLabel.text = joinClubLabelKey.Localize("MyClub/JoinClub");
		}
		if ((bool)joinUnselectedTabLabel)
		{
			joinUnselectedTabLabel.text = joinClubLabelKey.Localize("MyClub/JoinClub");
		}
	}
}
