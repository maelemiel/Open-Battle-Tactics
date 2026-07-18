using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapIdrefs : ISoapXsd
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
				return "IDREFS";
			}
		}

		public SoapIdrefs()
		{
		}

		public SoapIdrefs(string value)
		{
			_value = SoapHelper.Normalize(value);
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapIdrefs Parse(string value)
		{
			return new SoapIdrefs(value);
		}

		public override string ToString()
		{
			return _value;
		}
	}
}
