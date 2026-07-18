using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public struct StringToken
	{
		internal int tokValue;

		public int Token
		{
			get
			{
				return tokValue;
			}
		}

		internal StringToken(int val)
		{
			tokValue = val;
		}

		static StringToken()
		{
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is StringToken;
			if (flag)
			{
				StringToken stringToken = (StringToken)obj;
				flag = tokValue == stringToken.tokValue;
			}
			return flag;
		}

		public bool Equals(StringToken obj)
		{
			return tokValue == obj.tokValue;
		}

		public override int GetHashCode()
		{
			return tokValue;
		}

		public static bool operator ==(StringToken a, StringToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(StringToken a, StringToken b)
		{
			return !object.Equals(a, b);
		}
	}
}
