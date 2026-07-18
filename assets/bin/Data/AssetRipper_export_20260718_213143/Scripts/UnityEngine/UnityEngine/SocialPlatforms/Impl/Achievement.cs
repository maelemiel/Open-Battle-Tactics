using System;

namespace UnityEngine.SocialPlatforms.Impl
{
	public class Achievement : IAchievement
	{
		private bool m_Completed;

		private bool m_Hidden;

		private DateTime m_LastReportedDate;

		public string id { get; set; }

		public double percentCompleted { get; set; }

		public bool completed
		{
			get
			{
				return m_Completed;
			}
		}

		public bool hidden
		{
			get
			{
				return m_Hidden;
			}
		}

		public DateTime lastReportedDate
		{
			get
			{
				return m_LastReportedDate;
			}
		}

		public Achievement(string id, double percentCompleted, bool completed, bool hidden, DateTime lastReportedDate)
		{
			this.id = id;
			this.percentCompleted = percentCompleted;
			m_Completed = completed;
			m_Hidden = hidden;
			m_LastReportedDate = lastReportedDate;
		}

		public Achievement(string id, double percent)
		{
			this.id = id;
			percentCompleted = percent;
			m_Hidden = false;
			m_Completed = false;
			m_LastReportedDate = DateTime.MinValue;
		}

		public Achievement()
			: this("unknown", 0.0)
		{
		}

		public override string ToString()
		{
			return id + " - " + percentCompleted + " - " + completed + " - " + hidden + " - " + lastReportedDate;
		}

		public void ReportProgress(Action<bool> callback)
		{
			ActivePlatform.Instance.ReportProgress(id, percentCompleted, callback);
		}

		public void SetCompleted(bool value)
		{
			m_Completed = value;
		}

		public void SetHidden(bool value)
		{
			m_Hidden = value;
		}

		public void SetLastReportedDate(DateTime date)
		{
			m_LastReportedDate = date;
		}
	}
}
