using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public struct EventToken
	{
		internal int tokValue;

		public static readonly EventToken Empty;

		public int Token
		{
			get
			{
				return tokValue;
			}
		}

		internal EventToken(int val)
		{
			tokValue = val;
		}

		static EventToken()
		{
			Empty = default(EventToken);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is EventToken;
			if (flag)
			{
				EventToken eventToken = (EventToken)obj;
				flag = tokValue == eventToken.tokValue;
			}
			return flag;
		}

		public bool Equals(EventToken obj)
		{
			return tokValue == obj.tokValue;
		}

		public override int GetHashCode()
		{
			return tokValue;
		}

		public static bool operator ==(EventToken a, EventToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(EventToken a, EventToken b)
		{
			return !object.Equals(a, b);
		}
	}
}
