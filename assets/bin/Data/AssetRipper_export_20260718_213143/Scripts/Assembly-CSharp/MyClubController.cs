using UnityEngine;

public class MyClubController : MonoBehaviour
{
	[SerializeField]
	private ClubSceneController clubSceneController;

	[SerializeField]
	private GameObject loadingIcon;

	[SerializeField]
	private GameObject EditButton;

	[SerializeField]
	private GameObject clubGameObject;

	[SerializeField]
	private GameObject createClubGameObject;

	[SerializeField]
	private GameObject editGameObject;

	[SerializeField]
	private tk2dUIItem createTeamButton;

	[SerializeField]
	private tk2dUIItem editTeamButton;

	[SerializeField]
	private tk2dUIItem cancelEditTeamButton;

	[SerializeField]
	private ClubDataViewController dataForm;

	[SerializeField]
	private MyClubView myClubView;

	private bool initialized;

	private bool isDestroying;

	private MyClubStates currentState;

	private void Awake()
	{
		if ((bool)clubSceneController)
		{
			clubSceneController.UpdateTabLabels();
		}
		if (!myClubView)
		{
			Log.Warning("My Club View not found");
			return;
		}
		if ((bool)myClubView.ScrollableAreaController)
		{
			myClubView.ScrollableAreaController.OnDataChange += Refresh;
		}
		myClubView.gameObject.SetActive(false);
	}

	private void Start()
	{
		Singleton<InitializationManager>.instance.ExecuteIfStateEquals(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		if (initialized)
		{
			return;
		}
		if ((bool)loadingIcon)
		{
			loadingIcon.SetActive(true);
		}
		Singleton<SessionManager>.instance.GetClub(delegate(UserClub fetchedUserClub)
		{
			if ((bool)loadingIcon)
			{
				loadingIcon.SetActive(false);
			}
			if (fetchedUserClub != null)
			{
				UserProfile.player.userClub = fetchedUserClub;
				InitializeView(fetchedUserClub);
			}
		});
	}

	private void InitializeView(UserClub club)
	{
		ConfigureView(club);
		if ((bool)loadingIcon)
		{
			loadingIcon.SetActive(false);
		}
		initialized = true;
	}

	private void OnEnable()
	{
		if (UserProfile.player == null)
		{
			return;
		}
		UserClub userClub = UserProfile.player.userClub;
		if (UserProfile.player.IsClubMember)
		{
			if (!initialized)
			{
				InitializeView(userClub);
			}
			else
			{
				ConfigureView(userClub);
			}
		}
		else if (!string.IsNullOrEmpty(UserProfile.player.clubID))
		{
			Init();
		}
		else
		{
			SetPlayerClubState(MyClubStates.CREATE_CLUB);
		}
	}

	public void Refresh()
	{
		OnRefresh();
	}

	public void ConfigureView(UserClub clubData)
	{
		bool flag = clubData != null;
		SetPlayerClubState((!flag) ? MyClubStates.CREATE_CLUB : MyClubStates.CLUB_DATA);
		if (flag)
		{
			UserProfile player = UserProfile.player;
			if ((bool)myClubView)
			{
				myClubView.gameObject.SetActive(true);
				myClubView.ConfigureView(player.userClub);
			}
			if ((bool)EditButton)
			{
				EditButton.gameObject.SetActive(clubData.leaderID == player.id);
			}
		}
	}

	private void SetPlayerClubState(MyClubStates state)
	{
		switch (state)
		{
		case MyClubStates.CLUB_DATA:
			clubGameObject.SetActive(true);
			createClubGameObject.SetActive(false);
			editGameObject.SetActive(false);
			dataForm.gameObject.SetActive(false);
			break;
		case MyClubStates.CREATE_CLUB:
			clubGameObject.SetActive(false);
			createClubGameObject.SetActive(true);
			editGameObject.SetActive(false);
			dataForm.gameObject.SetActive(true);
			dataForm.Reset();
			if ((bool)createTeamButton)
			{
				createTeamButton.collider.enabled = true;
			}
			break;
		case MyClubStates.EDIT_CLUB:
			clubGameObject.SetActive(false);
			createClubGameObject.SetActive(false);
			editGameObject.SetActive(true);
			dataForm.gameObject.SetActive(true);
			if ((bool)editTeamButton)
			{
				editTeamButton.collider.enabled = true;
			}
			if ((bool)cancelEditTeamButton)
			{
				cancelEditTeamButton.collider.enabled = true;
			}
			break;
		}
		currentState = state;
		if ((bool)clubSceneController)
		{
			clubSceneController.UpdateTabLabels();
		}
	}

	private void OnEdit()
	{
		UserProfile player = UserProfile.player;
		if (player != null)
		{
			SetPlayerClubState(MyClubStates.EDIT_CLUB);
			if ((bool)dataForm)
			{
				dataForm.SetClubData(player.userClub);
			}
		}
	}

	private void OnAcceptEdit()
	{
		if (!dataForm)
		{
			return;
		}
		UserClub clubData = dataForm.GetClubData();
		if (clubData == null)
		{
			return;
		}
		if ((bool)loadingIcon)
		{
			loadingIcon.gameObject.SetActive(true);
		}
		dataForm.gameObject.SetActive(false);
		if ((bool)editTeamButton)
		{
			editTeamButton.collider.enabled = false;
		}
		if ((bool)cancelEditTeamButton)
		{
			cancelEditTeamButton.collider.enabled = false;
		}
		Singleton<SessionManager>.instance.EditClub(clubData, delegate(UserClub createdClub)
		{
			if (createdClub != null)
			{
				UserProfile.player.userClub = createdClub;
				dataForm.gameObject.SetActive(false);
				loadingIcon.gameObject.SetActive(false);
				PopupManager.ShowPopup(PopupDataModel.Ok("ui_clubs_club_updated_title".Localize("Success!"), "ui_clubs_club_updated_description".Localize("Your club's been updated!"), Refresh).ShowCloseButton(false));
			}
			else
			{
				dataForm.gameObject.SetActive(true);
				if ((bool)editTeamButton)
				{
					editTeamButton.collider.enabled = true;
				}
				if ((bool)cancelEditTeamButton)
				{
					cancelEditTeamButton.collider.enabled = true;
				}
			}
			loadingIcon.gameObject.SetActive(false);
		});
	}

	private void OnCancelEdit()
	{
		if ((bool)dataForm)
		{
			dataForm.Reset();
		}
		SetPlayerClubState(MyClubStates.CLUB_DATA);
	}

	private void OnCreateTeamPressed()
	{
		if (currentState != MyClubStates.CREATE_CLUB)
		{
			return;
		}
		UserClub userClub = null;
		if ((bool)dataForm)
		{
			userClub = dataForm.GetClubData();
		}
		if (userClub == null)
		{
			return;
		}
		if ((bool)createTeamButton)
		{
			createTeamButton.collider.enabled = false;
		}
		Singleton<SessionManager>.instance.CreateClub(userClub, delegate(UserClub createdClub)
		{
			if (!isDestroying)
			{
				if (createdClub != null)
				{
					UserProfile.player.clubID = createdClub.clubID;
					UserProfile.player.userClub = createdClub;
					PopupManager.ShowPopup(PopupDataModel.Ok("ui_clubs_club_created_title".Localize("Success!"), "ui_clubs_club_created_description".Localize("Your club's been created!"), Refresh).ShowCloseButton(false));
				}
				else if ((bool)createTeamButton)
				{
					createTeamButton.collider.enabled = true;
				}
			}
		});
	}

	private void OnRefresh()
	{
		if (UserProfile.player != null && base.gameObject.activeSelf && UserProfile.player.userClub != null)
		{
			ConfigureView(UserProfile.player.userClub);
		}
		else if ((bool)clubSceneController)
		{
			clubSceneController.UpdateClub();
		}
	}

	private void OnDestroy()
	{
		initialized = false;
		isDestroying = true;
		if ((bool)myClubView && (bool)myClubView.ScrollableAreaController)
		{
			myClubView.ScrollableAreaController.OnDataChange -= Refresh;
		}
	}
}
