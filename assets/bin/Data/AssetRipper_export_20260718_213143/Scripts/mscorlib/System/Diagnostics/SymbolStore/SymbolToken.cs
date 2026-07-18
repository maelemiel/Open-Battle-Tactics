using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	public struct SymbolToken
	{
		private int _val;

		public SymbolToken(int val)
		{
			_val = val;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SymbolToken))
			{
				return false;
			}
			return ((SymbolToken)obj).GetToken() == _val;
		}

		public bool Equals(SymbolToken obj)
		{
			return obj.GetToken() == _val;
		}

		public override int GetHashCode()
		{
			return _val.GetHashCode();
		}

		public int GetToken()
		{
			return _val;
		}

		public static bool operator ==(SymbolToken a, SymbolToken b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(SymbolToken a, SymbolToken b)
		{
			return !a.Equals(b);
		}
	}
}
