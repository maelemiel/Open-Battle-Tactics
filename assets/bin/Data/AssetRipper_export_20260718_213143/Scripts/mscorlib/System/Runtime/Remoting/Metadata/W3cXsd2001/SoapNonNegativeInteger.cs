using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapNonNegativeInteger : ISoapXsd
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
				return "nonNegativeInteger";
			}
		}

		public SoapNonNegativeInteger()
		{
		}

		public SoapNonNegativeInteger(decimal value)
		{
			if (value < 0m)
			{
				throw SoapHelper.GetException(this, "invalid " + value);
			}
			_value = value;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapNonNegativeInteger Parse(string value)
		{
			return new SoapNonNegativeInteger(decimal.Parse(value));
		}

		public override string ToString()
		{
			return _value.ToString();
		}
	}
}
