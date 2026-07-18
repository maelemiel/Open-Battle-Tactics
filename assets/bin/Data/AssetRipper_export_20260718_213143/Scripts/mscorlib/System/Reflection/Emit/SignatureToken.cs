using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	public struct SignatureToken
	{
		internal int tokValue;

		public static readonly SignatureToken Empty;

		public int Token
		{
			get
			{
				return tokValue;
			}
		}

		internal SignatureToken(int val)
		{
			tokValue = val;
		}

		static SignatureToken()
		{
			Empty = default(SignatureToken);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj is SignatureToken;
			if (flag)
			{
				SignatureToken signatureToken = (SignatureToken)obj;
				flag = tokValue == signatureToken.tokValue;
			}
			return flag;
		}

		public bool Equals(SignatureToken obj)
		{
			return tokValue == obj.tokValue;
		}

		public override int GetHashCode()
		{
			return tokValue;
		}

		public static bool operator ==(SignatureToken a, SignatureToken b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(SignatureToken a, SignatureToken b)
		{
			return !object.Equals(a, b);
		}
	}
}
