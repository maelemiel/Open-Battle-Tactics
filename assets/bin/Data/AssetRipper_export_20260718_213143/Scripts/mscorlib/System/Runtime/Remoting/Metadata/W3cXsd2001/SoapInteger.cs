using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapInteger : ISoapXsd
	{
		private decimal _value;

		public decimal Value
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
				return "integer";
			}
		}

		public SoapInteger()
		{
		}

		public SoapInteger(decimal value)
		{
			_value = value;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapInteger Parse(string value)
		{
			return new SoapInteger(decimal.Parse(value));
		}

		public override string ToString()
		{
			return _value.ToString();
		}
	}
}
