namespace UnityEngine.SocialPlatforms.Impl
{
	public class UserProfile : IUserProfile
	{
		protected string m_UserName;

		protected string m_ID;

		protected bool m_IsFriend;

		protected UserState m_State;

		protected Texture2D m_Image;

		public string userName
		{
			get
			{
				return m_UserName;
			}
		}

		public string id
		{
			get
			{
				return m_ID;
			}
		}

		public bool isFriend
		{
			get
			{
				return m_IsFriend;
			}
		}

		public UserState state
		{
			get
			{
				return m_State;
			}
		}

		public Texture2D image
		{
			get
			{
				return m_Image;
			}
		}

		public UserProfile()
		{
			m_UserName = "Uninitialized";
			m_ID = "0";
			m_IsFriend = false;
			m_State = UserState.Offline;
			m_Image = new Texture2D(32, 32);
		}

		public UserProfile(string name, string id, bool friend)
			: this(name, id, friend, UserState.Offline, new Texture2D(0, 0))
		{
		}

		public UserProfile(string name, string id, bool friend, UserState state, Texture2D image)
		{
			m_UserName = name;
			m_ID = id;
			m_IsFriend = friend;
			m_State = state;
			m_Image = image;
		}

		public override string ToString()
		{
			return id + " - " + userName + " - " + isFriend + " - " + state;
		}

		public void SetUserName(string name)
		{
			m_UserName = name;
		}

		public void SetUserID(string id)
		{
			m_ID = id;
		}

		public void SetImage(Texture2D image)
		{
			m_Image = image;
		}

		public void SetIsFriend(bool value)
		{
			m_IsFriend = value;
		}

		public void SetState(UserState state)
		{
			m_State = state;
		}
	}
}
