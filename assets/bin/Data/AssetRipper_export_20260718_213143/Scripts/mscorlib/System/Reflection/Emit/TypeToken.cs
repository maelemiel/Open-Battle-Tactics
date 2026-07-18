using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public struct TypeToken
	{
		internal int tokValue;

		public static readonly TypeToken Empty;

		public int Token
		{
			get
			{
				return tokValue;
			}
		}

		internal TypeToken(int val)
		{
			tokValue = val;
		}

		static TypeToken()
		{
			Empty = default(TypeToken);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is TypeToken;
			if (flag)
			{
				TypeToken typeToken = (TypeToken)obj;
				flag = tokValue == typeToken.tokValue;
			}
			return flag;
		}

		public bool Equals(TypeToken obj)
		{
			return tokValue == obj.tokValue;
		}

		public override int GetHashCode()
		{
			return tokValue;
		}

		public static bool operator ==(TypeToken a, TypeToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(TypeToken a, TypeToken b)
		{
			return !object.Equals(a, b);
		}
	}
}
