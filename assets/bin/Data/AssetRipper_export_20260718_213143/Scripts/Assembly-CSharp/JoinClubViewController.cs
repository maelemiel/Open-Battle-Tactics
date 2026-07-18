using System.Collections.Generic;
using UnityEngine;

public class JoinClubViewController : MonoBehaviour
{
	[SerializeField]
	private ClubSceneController clubSceneController;

	[SerializeField]
	private ScrollableAreaController clubsScrollableAreaController;

	[SerializeField]
	private GameObject loadingIcon;

	[SerializeField]
	private tk2dUITextInput searchField;

	[SerializeField]
	private ClubDataViewController clubDataViewController;

	[SerializeField]
	private GameObject searchClubGameObject;

	[SerializeField]
	private MyClubView myClubView;

	[SerializeField]
	private GameObject joinButton;

	[SerializeField]
	private GameObject fullClubButton;

	[SerializeField]
	private tk2dUIItem searchClubButton;

	[SerializeField]
	private tk2dUIItem clearButton;

	[SerializeField]
	private GameObject noResultsLabel;

	public string searchClubLabelKey = string.Empty;

	public string joinClubLabelKey = string.Empty;

	private MyClubStates currentState;

	private bool initialized;

	private bool isDestroying;

	private List<UserClub> fetchedClubs;

	private UserClub clubToJoin;

	private void OnEnable()
	{
		if ((bool)clubDataViewController)
		{
			clubDataViewController.gameObject.SetActive(false);
		}
		SetViewControllerState(MyClubStates.SEARCH_CLUB);
		ClearSearch();
		if (fetchedClubs != null)
		{
			ResetView();
		}
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
		Singleton<SessionManager>.instance.SearchClub(cb: delegate(List<UserClub> fetchedClubs)
		{
			this.fetchedClubs = fetchedClubs;
			if ((bool)loadingIcon)
			{
				loadingIcon.SetActive(false);
			}
			if (!isDestroying)
			{
				InitializeView(fetchedClubs);
			}
		}, clubName: string.Empty);
	}

	private void InitializeView(List<UserClub> clubsData)
	{
		clubSceneController.UpdateTabLabels();
		ConfigureView(clubsData);
		if ((bool)loadingIcon)
		{
			loadingIcon.SetActive(false);
		}
		initialized = true;
	}

	public void ConfigureView(List<UserClub> clubsData)
	{
		if (!clubsScrollableAreaController)
		{
			return;
		}
		clubsScrollableAreaController.gameObject.SetActive(true);
		clubsScrollableAreaController.DataSource = clubsData;
		if (initialized)
		{
			return;
		}
		foreach (ClubToJoinCell cellComponent in clubsScrollableAreaController.GetCellComponents<ClubToJoinCell>())
		{
			cellComponent.OnDetailsPressed += OnDetails;
			cellComponent.OnJoinPressed += OnJoin;
		}
	}

	private void ResetView()
	{
		if ((bool)searchField)
		{
			searchField.Text = string.Empty;
		}
		if (fetchedClubs != null)
		{
			ConfigureView(fetchedClubs);
		}
		if ((bool)clubsScrollableAreaController)
		{
			clubsScrollableAreaController.ContentToTop();
		}
		if ((bool)clubDataViewController)
		{
			clubDataViewController.gameObject.SetActive(false);
		}
		if ((bool)noResultsLabel)
		{
			noResultsLabel.SetActive(false);
		}
	}

	private void ClearSearch()
	{
		if ((bool)searchField)
		{
			searchField.Text = string.Empty;
		}
		if ((bool)clubsScrollableAreaController)
		{
			clubsScrollableAreaController.ContentToTop();
			clubsScrollableAreaController.gameObject.SetActive(false);
		}
		if ((bool)clubDataViewController)
		{
			clubDataViewController.gameObject.SetActive(false);
		}
		if ((bool)noResultsLabel)
		{
			noResultsLabel.SetActive(false);
		}
	}

	private void SetViewControllerState(MyClubStates state)
	{
		switch (state)
		{
		case MyClubStates.SEARCH_CLUB:
			searchClubGameObject.SetActive(true);
			myClubView.gameObject.SetActive(false);
			loadingIcon.gameObject.SetActive(false);
			break;
		case MyClubStates.CLUB_DETAILS:
			searchClubGameObject.SetActive(false);
			myClubView.gameObject.SetActive(false);
			loadingIcon.gameObject.SetActive(true);
			break;
		}
		currentState = state;
	}

	private void OnSearchClub()
	{
		string term = string.Empty;
		if ((bool)searchField)
		{
			term = searchField.Text.Trim();
		}
		if ((bool)loadingIcon)
		{
			loadingIcon.SetActive(true);
		}
		if ((bool)searchClubButton)
		{
			searchClubButton.enabled = false;
		}
		if ((bool)clearButton)
		{
			clearButton.enabled = false;
		}
		SearchWithClubName(term);
	}

	private void SearchWithClubName(string term)
	{
		if (string.IsNullOrEmpty(term))
		{
			Singleton<SessionManager>.instance.SearchClub(cb: delegate(List<UserClub> fetchedClubs)
			{
				if ((bool)loadingIcon)
				{
					loadingIcon.SetActive(false);
				}
				if (!isDestroying)
				{
					if ((bool)searchClubButton)
					{
						searchClubButton.enabled = true;
					}
					if ((bool)clearButton)
					{
						clearButton.enabled = true;
					}
					ConfigureView(fetchedClubs);
					if ((bool)noResultsLabel)
					{
						noResultsLabel.SetActive(fetchedClubs.Count == 0);
					}
				}
			}, clubName: string.Empty);
			return;
		}
		Singleton<SessionManager>.instance.SearchClub(searchField.Text, delegate(List<UserClub> fetchedClubs)
		{
			if ((bool)loadingIcon)
			{
				loadingIcon.SetActive(false);
			}
			if (!isDestroying)
			{
				if ((bool)searchClubButton)
				{
					searchClubButton.enabled = true;
				}
				if ((bool)clearButton)
				{
					clearButton.enabled = true;
				}
				ConfigureView(fetchedClubs);
				if ((bool)noResultsLabel)
				{
					noResultsLabel.SetActive(fetchedClubs.Count == 0);
				}
			}
		});
	}

	private void OnJoin()
	{
		PopupManager.ShowPopup(PopupDataModel.NoYes("ui_clubs_join_title".Localize("Join Club"), "ui_clubs_join_description".Localize("Are you sure you want to join this club?"), OnJoinConfirmation));
	}

	private void OnJoin(ClubToJoinCell pressedCell)
	{
		clubToJoin = pressedCell.clubData;
		OnJoin();
	}

	private void OnJoinConfirmation()
	{
		UserClub userClub = ((clubToJoin == null) ? myClubView.ClubData : clubToJoin);
		if (Singleton<SessionManager>.instance.JoinClubLogic(userClub))
		{
			if (userClub.teamType == ClubTypes.PUBLIC)
			{
				TryJoinClub(userClub, string.Empty);
			}
			else
			{
				PopupManager.ShowPopup(PopupDataModel.PasswordPopUp(OnPasswordEntered));
			}
		}
	}

	private void OnPasswordEntered(string password)
	{
		UserClub userClub = ((clubToJoin == null) ? myClubView.ClubData : clubToJoin);
		if ((bool)myClubView && userClub != null)
		{
			Log.Warning("Password Entered: " + password);
			TryJoinClub(userClub, password);
		}
	}

	private void TryJoinClub(UserClub clubToJoin, string password)
	{
		Singleton<SessionManager>.instance.JoinClub(clubToJoin, password, delegate(UserClub joinedClub)
		{
			if (!isDestroying && joinedClub != null)
			{
				UserProfile.player.userClub = joinedClub;
				UserProfile.player.clubID = joinedClub.clubID;
				if ((bool)clubSceneController)
				{
					clubSceneController.SetToggleGroupIndex(0);
				}
				Reporting.JoinClubEvent(UserProfile.player.id, joinedClub.clubID, joinedClub.name);
				PopupManager.ShowPopup(PopupDataModel.Ok("ui_clubs_join_club_title".Localize("Yay!"), "ui_clubs_join_club_description".Localize("You are now part of a club!")));
				clubToJoin = null;
			}
		});
	}

	private void OnDetails(ClubToJoinCell pressedCell)
	{
		if (pressedCell == null)
		{
			return;
		}
		string clubID = pressedCell.clubData.clubID;
		loadingIcon.gameObject.SetActive(true);
		SetViewControllerState(MyClubStates.CLUB_DETAILS);
		Singleton<SessionManager>.instance.GetClubWithID(clubID, delegate(UserClub fetchedClub)
		{
			if (!isDestroying && currentState == MyClubStates.CLUB_DETAILS)
			{
				if (fetchedClub == null)
				{
					PopupManager.ShowPopup(PopupDataModel.Ok("ui_clubs_club_not_found_title".Localize("Club not found"), "ui_clubs_club_not_found_description".Localize("This club doesn't exist")));
					SetViewControllerState(MyClubStates.SEARCH_CLUB);
					ResetView();
				}
				else
				{
					loadingIcon.gameObject.SetActive(false);
					myClubView.gameObject.SetActive(true);
					myClubView.ConfigureView(fetchedClub);
					if (UserProfile.player != null)
					{
						bool flag = string.IsNullOrEmpty(UserProfile.player.clubID);
						flag &= myClubView.ClubData.members.Count < Constants.ClubsTotalMembers;
						if ((bool)joinButton)
						{
							joinButton.gameObject.SetActive(flag);
						}
						if ((bool)fullClubButton)
						{
							fullClubButton.gameObject.SetActive(myClubView.ClubData.members.Count >= Constants.ClubsTotalMembers);
						}
					}
				}
			}
		});
	}

	private void OnBack()
	{
		SetViewControllerState(MyClubStates.SEARCH_CLUB);
	}

	private void OnDestroy()
	{
		isDestroying = true;
		initialized = false;
	}
}
