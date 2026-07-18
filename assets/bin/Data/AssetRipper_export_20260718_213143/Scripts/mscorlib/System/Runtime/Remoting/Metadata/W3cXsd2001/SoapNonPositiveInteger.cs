using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapNonPositiveInteger : ISoapXsd
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
				return "nonPositiveInteger";
			}
		}

		public SoapNonPositiveInteger()
		{
		}

		public SoapNonPositiveInteger(decimal value)
		{
			if (value > 0m)
			{
				throw SoapHelper.GetException(this, "invalid " + value);
			}
			_value = value;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapNonPositiveInteger Parse(string value)
		{
			return new SoapNonPositiveInteger(decimal.Parse(value));
		}

		public override string ToString()
		{
			return _value.ToString();
		}
	}
}
