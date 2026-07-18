namespace UnityEngine.SocialPlatforms.Impl
{
	public class AchievementDescription : IAchievementDescription
	{
		private string m_Title;

		private Texture2D m_Image;

		private string m_AchievedDescription;

		private string m_UnachievedDescription;

		private bool m_Hidden;

		private int m_Points;

		public string id { get; set; }

		public string title
		{
			get
			{
				return m_Title;
			}
		}

		public Texture2D image
		{
			get
			{
				return m_Image;
			}
		}

		public string achievedDescription
		{
			get
			{
				return m_AchievedDescription;
			}
		}

		public string unachievedDescription
		{
			get
			{
				return m_UnachievedDescription;
			}
		}

		public bool hidden
		{
			get
			{
				return m_Hidden;
			}
		}

		public int points
		{
			get
			{
				return m_Points;
			}
		}

		public AchievementDescription(string id, string title, Texture2D image, string achievedDescription, string unachievedDescription, bool hidden, int points)
		{
			this.id = id;
			m_Title = title;
			m_Image = image;
			m_AchievedDescription = achievedDescription;
			m_UnachievedDescription = unachievedDescription;
			m_Hidden = hidden;
			m_Points = points;
		}

		public override string ToString()
		{
			return id + " - " + title + " - " + achievedDescription + " - " + unachievedDescription + " - " + points + " - " + hidden;
		}

		public void SetImage(Texture2D image)
		{
			m_Image = image;
		}
	}
}
