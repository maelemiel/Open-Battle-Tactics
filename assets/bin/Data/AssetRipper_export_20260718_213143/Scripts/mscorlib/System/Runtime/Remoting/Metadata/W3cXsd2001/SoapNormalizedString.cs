using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapNormalizedString : ISoapXsd
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
				return "normalizedString";
			}
		}

		public SoapNormalizedString()
		{
		}

		public SoapNormalizedString(string value)
		{
			_value = SoapHelper.Normalize(value);
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapNormalizedString Parse(string value)
		{
			return new SoapNormalizedString(value);
		}

		public override string ToString()
		{
			return _value;
		}
	}
}
