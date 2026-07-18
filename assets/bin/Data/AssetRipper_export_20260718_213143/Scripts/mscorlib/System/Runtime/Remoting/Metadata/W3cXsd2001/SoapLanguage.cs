using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapLanguage : ISoapXsd
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
				return "language";
			}
		}

		public SoapLanguage()
		{
		}

		public SoapLanguage(string value)
		{
			_value = SoapHelper.Normalize(value);
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapLanguage Parse(string value)
		{
			return new SoapLanguage(value);
		}

		public override string ToString()
		{
			return _value;
		}
	}
}
