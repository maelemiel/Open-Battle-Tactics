using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class People
	{
		public delegate void getUsersForIds_onCompleteCallback(SimpleAPIStatus status, Error error, List<User> users);

		public delegate void getFriendsForUser_onCompleteCallback(SimpleAPIStatus status, Error error, List<User> users, int startOffset, int totalPossibleResultCount);

		public delegate void getFriendsWithGameForUser_onCompleteCallback(SimpleAPIStatus status, Error error, List<User> users, int startOffset, int totalPossibleResultCount);

		public static void getCurrentUser(Action<SimpleAPIStatus, Error, User> completeCb)
		{
			string userId = NativeAPI.Instance.UserId;
			getUserForId(userId, completeCb);
		}

		public static void getUserForId(string userId, Action<SimpleAPIStatus, Error, User> onComplete)
		{
			User.FetchUserWithUserID(userId, delegate(Error err, User user)
			{
				onComplete((err != null) ? SimpleAPIStatus.Error : SimpleAPIStatus.Success, err, user);
			});
		}

		public static void getUsersForIds(List<string> userIds, getUsersForIds_onCompleteCallback onComplete)
		{
			List<User> users = new List<User>();
			if (userIds.Count == 0)
			{
				onComplete(SimpleAPIStatus.Success, null, users);
				return;
			}
			foreach (string userId in userIds)
			{
				User.FetchUserWithUserID(userId, delegate(Error err, User user)
				{
					if (err == null)
					{
						users.Add(user);
						if (users.Count == userIds.Count)
						{
							onComplete(SimpleAPIStatus.Success, null, users);
						}
					}
					else
					{
						onComplete(SimpleAPIStatus.Error, err, null);
					}
				});
			}
		}

		public static void getFriendsForUser(User user, int howMany, int offset, getFriendsForUser_onCompleteCallback onComplete)
		{
			user.MutualFriends.Get(howMany, (uint)offset, delegate(SimpleAPIStatus status, Error error, List<User> users, int startOffset, int totalPossibleResultCount)
			{
				onComplete(status, error, users, startOffset, totalPossibleResultCount);
			});
		}

		public static void getFriendsWithGameForUser(User user, int howMany, int offset, getFriendsWithGameForUser_onCompleteCallback onComplete)
		{
			user.MutualFriendsWithCurrentGame.Get(howMany, (uint)offset, delegate(SimpleAPIStatus status, Error error, List<User> users, int startOffset, int totalPossibleResultCount)
			{
				onComplete(status, error, users, startOffset, totalPossibleResultCount);
			});
		}

		public static void SendInvitationToCurrentGame(List<string> recipientIds)
		{
			if (recipientIds.Count < 1)
			{
				return;
			}
			string userId;
			foreach (string recipientId in recipientIds)
			{
				userId = recipientId;
				MobageRequest request = MobageRequest.Request;
				request.APIMethod = "invitations";
				request.HTTPMethod = "POST";
				request.PostBody = new Dictionary<string, object> { { "recipient_id", userId } };
				request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
				{
					if (err != null)
					{
						Debug.Log("Sending invite email failed for recipient id: " + userId);
					}
					else
					{
						Debug.Log("Invite email sent for recipient id: " + userId);
					}
				});
			}
		}
	}
}
