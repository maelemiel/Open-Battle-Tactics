using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public struct NetworkMessageInfo
	{
		private double m_TimeStamp;

		private NetworkPlayer m_Sender;

		private NetworkViewID m_ViewID;

		public double timestamp
		{
			get
			{
				return m_TimeStamp;
			}
		}

		public NetworkPlayer sender
		{
			get
			{
				return m_Sender;
			}
		}

		public NetworkView networkView
		{
			get
			{
				if (m_ViewID == NetworkViewID.unassigned)
				{
					Debug.LogError("No NetworkView is assigned to this NetworkMessageInfo object. Note that this is expected in OnNetworkInstantiate().");
					return NullNetworkView();
				}
				return NetworkView.Find(m_ViewID);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern NetworkView NullNetworkView();
	}
}
