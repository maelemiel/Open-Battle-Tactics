using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class SocialService
	{
		public class BalanceUpdateNotification
		{
		}

		public delegate void openFriendPicker_onCompleteCallback(DismissableAPIStatus status, Error error, List<User> pickedFriends, List<User> invitedFriends);

		public delegate void showPromotions_onCompleteCallback(DismissableAPIStatus status, Error error);

		public delegate void openPlayerInviter_onCompleteCallback(CancelableAPIStatus status, Error Error);

		public delegate void executeLoginWithParams_onCompleteCallback(CancelableAPIStatus status, Error error);

		public delegate void executeLogout_onCompleteCallback(SimpleAPIStatus status, Error error);

		public delegate void openBankDialog_onCompleteCallback(SimpleAPIStatus status, Error error);

		public delegate void showBalanceButton_onCompleteCallback(SimpleAPIStatus status, Error error);

		public delegate void getCurrentBalance_onCompleteCallback(SimpleAPIStatus status, Error error, string currencyName, string currencyIconUrl, int balance);

		public delegate void getCurrentBalanceDetails_onCompleteCallback(SimpleAPIStatus status, Error error, int imageWidth, string currencyName, string currencyIconUrl, string balanceImageUrl);

		public delegate void purchaseASCItem_onCompleteCallback(CancelableAPIStatus status, Error error);

		public delegate void giftASC_onCompleteCallback(SimpleAPIStatus status, Error error, int giftAmount);

		public delegate void checkFacebookStatus_onCompleteCallback(SimpleAPIStatus status, Error error, string userId, string facebookId, string accessToken);

		public delegate void getFacebookId_onCompleteCallback(SimpleAPIStatus status, Error error, string userId, string facebookId);

		public delegate void getASCItems_onCompleteCallback(SimpleAPIStatus status, Error error, List<ItemData> items);

		public delegate void BalanceUpdateNotificationDelegate(BalanceUpdateNotification notification);

		private static List<BalanceUpdateNotificationDelegate> balanceUpdateNotificationList = new List<BalanceUpdateNotificationDelegate>();

		public static event BalanceUpdateNotificationDelegate BalanceUpdate
		{
			add
			{
				balanceUpdateNotificationList.Add(value);
			}
			remove
			{
				balanceUpdateNotificationList.Remove(value);
			}
		}

		public static void BalanceUpdatePost()
		{
			for (int i = 0; i < balanceUpdateNotificationList.Count; i++)
			{
				balanceUpdateNotificationList[i](new BalanceUpdateNotification());
			}
		}

		public static void openFriendPicker(int maxFriendsToSelect, openFriendPicker_onCompleteCallback onComplete)
		{
			Error error = new Error();
			error.domain = "com.mobage.error.api";
			error.code = 20004;
			Error error2 = error;
			onComplete(DismissableAPIStatus.Error, error2, null, null);
		}

		public static void openOtherUserProfile(User user)
		{
			if (user == null)
			{
				Debug.LogError("user is null");
				return;
			}
			JsonData jsonData = new JsonData();
			jsonData["userId"] = user.uid;
			MobageUI.Instance.PresentExperienceNamed("OtherUserProfile", jsonData);
		}

		public static void showBalanceButton(int x, int y, int width, int height, showBalanceButton_onCompleteCallback onComplete)
		{
			BalanceButtonController.Instance.ShowButton(x, y, width, height, delegate(SimpleAPIStatus status, Error error)
			{
				onComplete(status, error);
			});
		}

		public static void getCurrentBalance(getCurrentBalance_onCompleteCallback onComplete)
		{
			BankBalance.GetBalanceWithCallback(delegate(SimpleAPIStatus status, Error error, int balance, string currencyName, string currencyUrl)
			{
				onComplete(status, error, currencyName, currencyUrl, balance);
			});
		}

		public static void hideBalanceButton()
		{
			BalanceButtonController.Instance.HideButton();
		}

		public static void getCurrentBalanceDetails(int imageHeight, getCurrentBalanceDetails_onCompleteCallback onComplete)
		{
			getCurrentBalanceDetails(imageHeight, Color.white, onComplete);
		}

		public static void getCurrentBalanceDetails(int imageHeight, Color color, getCurrentBalanceDetails_onCompleteCallback onComplete)
		{
			BankUI.getCurrentBalanceDetails(imageHeight, color, delegate(SimpleAPIStatus status, Error error, int imageWidth, string currencyName, string currencyIconUrl, string balanceImageUrl)
			{
				onComplete(status, error, imageWidth, currencyName, currencyIconUrl, balanceImageUrl);
			});
		}

		public static void purchaseASCItem(ItemData item, purchaseASCItem_onCompleteCallback onComplete)
		{
			MobageWeb.purchaseASCItem(item, delegate(CancelableAPIStatus status, Error error)
			{
				onComplete(status, error);
			});
		}

		public static void giftASC(string giftCode, giftASC_onCompleteCallback onComplete)
		{
			MobageRequest mobageRequest = MobageRequest.NewBankRequestWithContext(MobageSession.CurrentSession);
			mobageRequest.APIMethod = "bank/gift/@app/" + giftCode;
			mobageRequest.HTTPMethod = "PUT";
			mobageRequest.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (data != null && err == null)
				{
					onComplete(SimpleAPIStatus.Success, err, (int)data["amount"]);
				}
				else
				{
					onComplete(SimpleAPIStatus.Error, err, -1);
				}
			});
		}

		public static void getASCItems(getASCItems_onCompleteCallback onComplete)
		{
			BankInventory.getASCItems(delegate(SimpleAPIStatus status, Error error, List<ItemData> items)
			{
				onComplete(status, error, items);
			});
		}

		public static void pauseWebViewCoreThread()
		{
		}

		public static void resumeWebViewCoreThread()
		{
		}

		public static void ShowWelcomeBackToast()
		{
			People.getCurrentUser(delegate(SimpleAPIStatus status, Error error, User user)
			{
				ShowToast(string.Format("Welcome back to Mobage, {0}.", user.nickname));
			});
		}

		public static void ShowToast(string displayText)
		{
			GameObject gameObject = GameObject.Find("NotificationViewController");
			if (gameObject != null)
			{
				gameObject.SendMessage("Show", displayText);
			}
		}
	}
}
