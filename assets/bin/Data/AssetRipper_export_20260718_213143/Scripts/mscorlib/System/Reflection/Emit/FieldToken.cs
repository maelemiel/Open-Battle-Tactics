using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public struct FieldToken
	{
		internal int tokValue;

		public static readonly FieldToken Empty;

		public int Token
		{
			get
			{
				return tokValue;
			}
		}

		internal FieldToken(int val)
		{
			tokValue = val;
		}

		static FieldToken()
		{
			Empty = default(FieldToken);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is FieldToken;
			if (flag)
			{
				FieldToken fieldToken = (FieldToken)obj;
				flag = tokValue == fieldToken.tokValue;
			}
			return flag;
		}

		public bool Equals(FieldToken obj)
		{
			return tokValue == obj.tokValue;
		}

		public override int GetHashCode()
		{
			return tokValue;
		}

		public static bool operator ==(FieldToken a, FieldToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(FieldToken a, FieldToken b)
		{
			return !object.Equals(a, b);
		}
	}
}
