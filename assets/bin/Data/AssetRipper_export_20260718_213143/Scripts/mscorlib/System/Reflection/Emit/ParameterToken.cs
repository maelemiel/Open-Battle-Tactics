using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public struct ParameterToken
	{
		internal int tokValue;

		public static readonly ParameterToken Empty;

		public int Token
		{
			get
			{
				return tokValue;
			}
		}

		internal ParameterToken(int val)
		{
			tokValue = val;
		}

		static ParameterToken()
		{
			Empty = default(ParameterToken);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is ParameterToken;
			if (flag)
			{
				ParameterToken parameterToken = (ParameterToken)obj;
				flag = tokValue == parameterToken.tokValue;
			}
			return flag;
		}

		public bool Equals(ParameterToken obj)
		{
			return tokValue == obj.tokValue;
		}

		public override int GetHashCode()
		{
			return tokValue;
		}

		public static bool operator ==(ParameterToken a, ParameterToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(ParameterToken a, ParameterToken b)
		{
			return !object.Equals(a, b);
		}
	}
}
