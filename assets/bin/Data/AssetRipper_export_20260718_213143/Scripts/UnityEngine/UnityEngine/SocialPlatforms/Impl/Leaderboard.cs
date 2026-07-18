using System;

namespace UnityEngine.SocialPlatforms.Impl
{
	public class Leaderboard : ILeaderboard
	{
		private bool m_Loading;

		private IScore m_LocalUserScore;

		private uint m_MaxRange;

		private IScore[] m_Scores;

		private string m_Title;

		private string[] m_UserIDs;

		public bool loading
		{
			get
			{
				return ActivePlatform.Instance.GetLoading(this);
			}
		}

		public string id { get; set; }

		public UserScope userScope { get; set; }

		public Range range { get; set; }

		public TimeScope timeScope { get; set; }

		public IScore localUserScore
		{
			get
			{
				return m_LocalUserScore;
			}
		}

		public uint maxRange
		{
			get
			{
				return m_MaxRange;
			}
		}

		public IScore[] scores
		{
			get
			{
				return m_Scores;
			}
		}

		public string title
		{
			get
			{
				return m_Title;
			}
		}

		public Leaderboard()
		{
			id = "Invalid";
			range = new Range(1, 10);
			userScope = UserScope.Global;
			timeScope = TimeScope.AllTime;
			m_Loading = false;
			m_LocalUserScore = new Score("Invalid", 0L);
			m_MaxRange = 0u;
			m_Scores = new Score[0];
			m_Title = "Invalid";
			m_UserIDs = new string[0];
		}

		public void SetUserFilter(string[] userIDs)
		{
			m_UserIDs = userIDs;
		}

		public override string ToString()
		{
			return string.Concat("ID: '", id, "' Title: '", m_Title, "' Loading: '", m_Loading, "' Range: [", range.from, ",", range.count, "] MaxRange: '", m_MaxRange, "' Scores: '", m_Scores.Length, "' UserScope: '", userScope, "' TimeScope: '", timeScope, "' UserFilter: '", m_UserIDs.Length);
		}

		public void LoadScores(Action<bool> callback)
		{
			ActivePlatform.Instance.LoadScores(this, callback);
		}

		public void SetLocalUserScore(IScore score)
		{
			m_LocalUserScore = score;
		}

		public void SetMaxRange(uint maxRange)
		{
			m_MaxRange = maxRange;
		}

		public void SetScores(IScore[] scores)
		{
			m_Scores = scores;
		}

		public void SetTitle(string title)
		{
			m_Title = title;
		}

		public string[] GetUserFilter()
		{
			return m_UserIDs;
		}
	}
}
