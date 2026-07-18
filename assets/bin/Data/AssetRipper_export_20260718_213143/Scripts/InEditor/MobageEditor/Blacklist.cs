using System.Collections.Generic;

namespace MobageEditor
{
	public class Blacklist
	{
		public delegate void fetchBlacklistOfUser_onCompleteCallback(SimpleAPIStatus status, Error error, List<User> blacklistedUsers, int startOffset, int totalPossibleResultCount);

		public delegate void checkBlacklistOfUserForUser_onCompleteCallback(SimpleAPIStatus status, Error error, bool isBlacklisted);

		public static void fetchBlacklistOfUser(User user, int howMany, int startOffset, fetchBlacklistOfUser_onCompleteCallback completeCb)
		{
			if (user == null)
			{
				completeCb(SimpleAPIStatus.Error, new Error
				{
					localizedDescription = "Error fetchBlackListOfUser expects a user",
					domain = "com.mobage.error.api",
					code = 20002
				}, null, 0, 0);
				return;
			}
			user.BlacklistedUsers.Get(howMany, (uint)startOffset, delegate(SimpleAPIStatus status, Error error, List<User> list, int so, int totalPossibleResultCount)
			{
				if (error != null)
				{
					error = new Error
					{
						localizedDescription = error.localizedDescription,
						domain = "com.mobage.error.api",
						code = error.code
					};
				}
				if (error != null)
				{
					completeCb(SimpleAPIStatus.Error, error, null, 0, 0);
				}
				else
				{
					completeCb(SimpleAPIStatus.Success, null, list, so, totalPossibleResultCount);
				}
			});
		}

		public static void checkBlacklistOfUserForUser(User user, User targetUser, checkBlacklistOfUserForUser_onCompleteCallback completeCb)
		{
			if (user == null || targetUser == null)
			{
				completeCb(SimpleAPIStatus.Error, new Error
				{
					localizedDescription = "Error fetchBlackListOfUser expects a user",
					domain = "com.mobage.error.api",
					code = 20002
				}, false);
				return;
			}
			string targetUserId = targetUser.uid;
			user.BlacklistedUsers.Get(-1, 0u, delegate(SimpleAPIStatus status, Error error, List<User> list, int startOffset, int totalPossibleResultCount)
			{
				if (error == null)
				{
					foreach (User item in list)
					{
						if (item.uid == targetUserId)
						{
							completeCb(SimpleAPIStatus.Success, null, true);
							return;
						}
					}
				}
				completeCb((error != null) ? SimpleAPIStatus.Error : SimpleAPIStatus.Success, error, false);
			});
		}
	}
}
