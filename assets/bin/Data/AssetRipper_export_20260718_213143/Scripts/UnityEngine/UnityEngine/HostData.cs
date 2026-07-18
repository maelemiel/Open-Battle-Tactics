using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class HostData
	{
		private int m_Nat;

		private string m_GameType;

		private string m_GameName;

		private int m_ConnectedPlayers;

		private int m_PlayerLimit;

		private string[] m_IP;

		private int m_Port;

		private int m_PasswordProtected;

		private string m_Comment;

		private string m_GUID;

		public bool useNat
		{
			get
			{
				return m_Nat != 0;
			}
			set
			{
				m_Nat = (value ? 1 : 0);
			}
		}

		public string gameType
		{
			get
			{
				return m_GameType;
			}
			set
			{
				m_GameType = value;
			}
		}

		public string gameName
		{
			get
			{
				return m_GameName;
			}
			set
			{
				m_GameName = value;
			}
		}

		public int connectedPlayers
		{
			get
			{
				return m_ConnectedPlayers;
			}
			set
			{
				m_ConnectedPlayers = value;
			}
		}

		public int playerLimit
		{
			get
			{
				return m_PlayerLimit;
			}
			set
			{
				m_PlayerLimit = value;
			}
		}

		public string[] ip
		{
			get
			{
				return m_IP;
			}
			set
			{
				m_IP = value;
			}
		}

		public int port
		{
			get
			{
				return m_Port;
			}
			set
			{
				m_Port = value;
			}
		}

		public bool passwordProtected
		{
			get
			{
				return m_PasswordProtected != 0;
			}
			set
			{
				m_PasswordProtected = (value ? 1 : 0);
			}
		}

		public string comment
		{
			get
			{
				return m_Comment;
			}
			set
			{
				m_Comment = value;
			}
		}

		public string guid
		{
			get
			{
				return m_GUID;
			}
			set
			{
				m_GUID = value;
			}
		}
	}
}
