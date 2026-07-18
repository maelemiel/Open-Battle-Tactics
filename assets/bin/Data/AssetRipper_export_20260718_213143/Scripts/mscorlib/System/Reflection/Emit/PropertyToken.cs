using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public struct PropertyToken
	{
		internal int tokValue;

		public static readonly PropertyToken Empty;

		public int Token
		{
			get
			{
				return tokValue;
			}
		}

		internal PropertyToken(int val)
		{
			tokValue = val;
		}

		static PropertyToken()
		{
			Empty = default(PropertyToken);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is PropertyToken;
			if (flag)
			{
				PropertyToken propertyToken = (PropertyToken)obj;
				flag = tokValue == propertyToken.tokValue;
			}
			return flag;
		}

		public bool Equals(PropertyToken obj)
		{
			return tokValue == obj.tokValue;
		}

		public override int GetHashCode()
		{
			return tokValue;
		}

		public static bool operator ==(PropertyToken a, PropertyToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(PropertyToken a, PropertyToken b)
		{
			return !object.Equals(a, b);
		}
	}
}
