using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapNotation : ISoapXsd
	{
		private string _value;

		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public static string XsdType
		{
			get
			{
				return "NOTATION";
			}
		}

		public SoapNotation()
		{
		}

		public SoapNotation(string value)
		{
			_value = value;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapNotation Parse(string value)
		{
			return new SoapNotation(value);
		}

		public override string ToString()
		{
			return _value;
		}
	}
}
