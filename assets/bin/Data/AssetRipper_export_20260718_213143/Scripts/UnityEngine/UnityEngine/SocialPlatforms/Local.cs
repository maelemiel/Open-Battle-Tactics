using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;

namespace UnityEngine.SocialPlatforms
{
	public class Local : ISocialPlatform
	{
		private static LocalUser m_LocalUser;

		private List<UserProfile> m_Friends = new List<UserProfile>();

		private List<UserProfile> m_Users = new List<UserProfile>();

		private List<AchievementDescription> m_AchievementDescriptions = new List<AchievementDescription>();

		private List<Achievement> m_Achievements = new List<Achievement>();

		private List<Leaderboard> m_Leaderboards = new List<Leaderboard>();

		private Texture2D m_DefaultTexture;

		public ILocalUser localUser
		{
			get
			{
				if (m_LocalUser == null)
				{
					m_LocalUser = new LocalUser();
				}
				return m_LocalUser;
			}
		}

		void ISocialPlatform.Authenticate(ILocalUser user, Action<bool> callback)
		{
			LocalUser localUser = (LocalUser)user;
			m_DefaultTexture = CreateDummyTexture(32, 32);
			PopulateStaticData();
			localUser.SetAuthenticated(true);
			localUser.SetUnderage(false);
			localUser.SetUserID("1000");
			localUser.SetUserName("Lerpz");
			localUser.SetImage(m_DefaultTexture);
			if (callback != null)
			{
				callback(true);
			}
		}

		void ISocialPlatform.LoadFriends(ILocalUser user, Action<bool> callback)
		{
			if (VerifyUser())
			{
				((LocalUser)user).SetFriends(m_Friends.ToArray());
				if (callback != null)
				{
					callback(true);
				}
			}
		}

		void ISocialPlatform.LoadScores(ILeaderboard board, Action<bool> callback)
		{
			if (!VerifyUser())
			{
				return;
			}
			Leaderboard leaderboard = (Leaderboard)board;
			foreach (Leaderboard leaderboard2 in m_Leaderboards)
			{
				if (leaderboard2.id == leaderboard.id)
				{
					leaderboard.SetTitle(leaderboard2.title);
					leaderboard.SetScores(leaderboard2.scores);
					leaderboard.SetMaxRange((uint)leaderboard2.scores.Length);
				}
			}
			SortScores(leaderboard);
			SetLocalPlayerScore(leaderboard);
			if (callback != null)
			{
				callback(true);
			}
		}

		bool ISocialPlatform.GetLoading(ILeaderboard board)
		{
			if (!VerifyUser())
			{
				return false;
			}
			return ((Leaderboard)board).loading;
		}

		public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
			List<UserProfile> list = new List<UserProfile>();
			if (!VerifyUser())
			{
				return;
			}
			foreach (string text in userIDs)
			{
				foreach (UserProfile user in m_Users)
				{
					if (user.id == text)
					{
						list.Add(user);
					}
				}
				foreach (UserProfile friend in m_Friends)
				{
					if (friend.id == text)
					{
						list.Add(friend);
					}
				}
			}
			callback(list.ToArray());
		}

		public void ReportProgress(string id, double progress, Action<bool> callback)
		{
			if (!VerifyUser())
			{
				return;
			}
			foreach (Achievement achievement in m_Achievements)
			{
				if (achievement.id == id && achievement.percentCompleted <= progress)
				{
					if (progress >= 100.0)
					{
						achievement.SetCompleted(true);
					}
					achievement.SetHidden(false);
					achievement.SetLastReportedDate(DateTime.Now);
					achievement.percentCompleted = progress;
					if (callback != null)
					{
						callback(true);
					}
					return;
				}
			}
			foreach (AchievementDescription achievementDescription in m_AchievementDescriptions)
			{
				if (achievementDescription.id == id)
				{
					bool completed = ((progress >= 100.0) ? true : false);
					Achievement item = new Achievement(id, progress, completed, false, DateTime.Now);
					m_Achievements.Add(item);
					if (callback != null)
					{
						callback(true);
					}
					return;
				}
			}
			Debug.LogError("Achievement ID not found");
			if (callback != null)
			{
				callback(false);
			}
		}

		public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			if (VerifyUser() && callback != null)
			{
				callback(m_AchievementDescriptions.ToArray());
			}
		}

		public void LoadAchievements(Action<IAchievement[]> callback)
		{
			if (VerifyUser() && callback != null)
			{
				callback(m_Achievements.ToArray());
			}
		}

		public void ReportScore(long score, string board, Action<bool> callback)
		{
			if (!VerifyUser())
			{
				return;
			}
			foreach (Leaderboard leaderboard in m_Leaderboards)
			{
				if (leaderboard.id == board)
				{
					List<Score> list = new List<Score>((Score[])leaderboard.scores);
					list.Add(new Score(board, score, localUser.id, DateTime.Now, score + " points", 0));
					leaderboard.SetScores(list.ToArray());
					if (callback != null)
					{
						callback(true);
					}
					return;
				}
			}
			Debug.LogError("Leaderboard not found");
			if (callback != null)
			{
				callback(false);
			}
		}

		public void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			if (!VerifyUser())
			{
				return;
			}
			foreach (Leaderboard leaderboard in m_Leaderboards)
			{
				if (leaderboard.id == leaderboardID)
				{
					SortScores(leaderboard);
					if (callback != null)
					{
						callback(leaderboard.scores);
					}
					return;
				}
			}
			Debug.LogError("Leaderboard not found");
			if (callback != null)
			{
				callback(new Score[0]);
			}
		}

		private void SortScores(Leaderboard board)
		{
			List<Score> list = new List<Score>((Score[])board.scores);
			list.Sort((Score s1, Score s2) => s2.value.CompareTo(s1.value));
			for (int num = 0; num < list.Count; num++)
			{
				list[num].SetRank(num + 1);
			}
		}

		private void SetLocalPlayerScore(Leaderboard board)
		{
			IScore[] scores = board.scores;
			for (int i = 0; i < scores.Length; i++)
			{
				Score score = (Score)scores[i];
				if (score.userID == localUser.id)
				{
					board.SetLocalUserScore(score);
					break;
				}
			}
		}

		public void ShowAchievementsUI()
		{
			Debug.Log("ShowAchievementsUI not implemented");
		}

		public void ShowLeaderboardUI()
		{
			Debug.Log("ShowLeaderboardUI not implemented");
		}

		public ILeaderboard CreateLeaderboard()
		{
			return new Leaderboard();
		}

		public IAchievement CreateAchievement()
		{
			return new Achievement();
		}

		private bool VerifyUser()
		{
			if (!localUser.authenticated)
			{
				Debug.LogError("Must authenticate first");
				return false;
			}
			return true;
		}

		private void PopulateStaticData()
		{
			m_Friends.Add(new UserProfile("Fred", "1001", true, UserState.Online, m_DefaultTexture));
			m_Friends.Add(new UserProfile("Julia", "1002", true, UserState.Online, m_DefaultTexture));
			m_Friends.Add(new UserProfile("Jeff", "1003", true, UserState.Online, m_DefaultTexture));
			m_Users.Add(new UserProfile("Sam", "1004", false, UserState.Offline, m_DefaultTexture));
			m_Users.Add(new UserProfile("Max", "1005", false, UserState.Offline, m_DefaultTexture));
			m_AchievementDescriptions.Add(new AchievementDescription("Achievement01", "First achievement", m_DefaultTexture, "Get first achievement", "Received first achievement", false, 10));
			m_AchievementDescriptions.Add(new AchievementDescription("Achievement02", "Second achievement", m_DefaultTexture, "Get second achievement", "Received second achievement", false, 20));
			m_AchievementDescriptions.Add(new AchievementDescription("Achievement03", "Third achievement", m_DefaultTexture, "Get third achievement", "Received third achievement", false, 15));
			Leaderboard leaderboard = new Leaderboard();
			leaderboard.SetTitle("High Scores");
			leaderboard.id = "Leaderboard01";
			List<Score> list = new List<Score>();
			list.Add(new Score("Leaderboard01", 300L, "1001", DateTime.Now.AddDays(-1.0), "300 points", 1));
			list.Add(new Score("Leaderboard01", 255L, "1002", DateTime.Now.AddDays(-1.0), "255 points", 2));
			list.Add(new Score("Leaderboard01", 55L, "1003", DateTime.Now.AddDays(-1.0), "55 points", 3));
			list.Add(new Score("Leaderboard01", 10L, "1004", DateTime.Now.AddDays(-1.0), "10 points", 4));
			leaderboard.SetScores(list.ToArray());
			m_Leaderboards.Add(leaderboard);
		}

		private Texture2D CreateDummyTexture(int width, int height)
		{
			Texture2D texture2D = new Texture2D(width, height);
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					Color color = (((j & i) <= 0) ? Color.gray : Color.white);
					texture2D.SetPixel(j, i, color);
				}
			}
			texture2D.Apply();
			return texture2D;
		}
	}
}
