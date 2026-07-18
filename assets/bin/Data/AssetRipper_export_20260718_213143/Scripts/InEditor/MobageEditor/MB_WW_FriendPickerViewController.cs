using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobageEditor
{
	public class MB_WW_FriendPickerViewController : ExperienceViewController
	{
		private bool isVisible;

		private bool isPresenting;

		private bool isFilteredByCurrentGame;

		private int maxFriendsSelectCount;

		public List<friendsListFriend> DisplayedFriendsList;

		private List<friendsListFriend> completeFriendsList;

		private HashSet<friendsListFriend> selectedUsers;

		private ProgressDialog modalSpinner;

		private TableView tableView;

		private Action<DismissableAPIStatus, Error, List<User>, List<User>> callback;

		private void showModalSpinner()
		{
			if (!(modalSpinner != null))
			{
				modalSpinner = new GameObject().AddComponent<ProgressDialog>();
				modalSpinner.Show();
			}
		}

		private void dataReadyShowUI()
		{
			if (completeFriendsList.Count <= 0)
			{
				runNoFriendsPath();
				return;
			}
			tableView = new GameObject().AddComponent<TableView>();
			filterFriendsByService();
			MobageUI.Instance.PresentExperience(this, true, delegate
			{
				isVisible = true;
				FriendPickerEvent analyticsEvent = new FriendPickerEvent("FRIENDPICKERSHOW", null);
				Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
			});
		}

		private void runNoFriendsPath()
		{
		}

		private void filterFriendsByService()
		{
			DisplayedFriendsList.Clear();
			selectedUsers.Clear();
			DisplayedFriendsList.AddRange(completeFriendsList);
			isFilteredByCurrentGame = true;
			tableView.Load(this);
		}

		private void hideModalSpinner()
		{
			if (!(modalSpinner == null))
			{
				modalSpinner.Hide();
			}
		}

		public void OnFriendPickerSelection(bool[] selectMe)
		{
			Debug.Log("OnFriendPickerSelection");
			if (!isVisible || !isPresenting)
			{
				return;
			}
			Debug.Log("isVisible and isPresenting");
			List<User> list = null;
			List<User> list2 = null;
			for (int i = 0; i < selectMe.Length; i++)
			{
				if (selectMe[i])
				{
					selectedUsers.Add(DisplayedFriendsList[i]);
				}
			}
			if (selectedUsers.Count > 0)
			{
				list = new List<User>();
				list2 = new List<User>();
				List<string> list3 = new List<string>();
				foreach (friendsListFriend selectedUser in selectedUsers)
				{
					if (selectedUser.HasCurrentGame)
					{
						list.Add(selectedUser.UserObject);
						continue;
					}
					list2.Add(selectedUser.UserObject);
					list3.Add(selectedUser.UserObject.uid);
				}
				People.SendInvitationToCurrentGame(list3);
				JsonData jsonData = new JsonData();
				jsonData["numFriendsPicked"] = selectedUsers.Count;
				FriendPickerEvent analyticsEvent = new FriendPickerEvent("FRIENDPICKERPICK", jsonData);
				Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
			}
			callback(DismissableAPIStatus.Success, null, list, list2);
			MobageUI.Instance.PopExperienceAnimated(false, delegate
			{
				isVisible = false;
				hideEvent();
			});
			callback = null;
		}

		private void hideEvent()
		{
			FriendPickerEvent analyticsEvent = new FriendPickerEvent("FRIENDPICKERHIDE", null);
			Mobage.sharedInstance.AnalyticsSession.Report(analyticsEvent);
		}
	}
}
