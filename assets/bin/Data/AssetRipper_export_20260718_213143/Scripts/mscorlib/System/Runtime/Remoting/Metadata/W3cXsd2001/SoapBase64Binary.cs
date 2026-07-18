using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapBase64Binary : ISoapXsd
	{
		private byte[] _value;

		public byte[] Value
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
				return "base64Binary";
			}
		}

		public SoapBase64Binary()
		{
		}

		public SoapBase64Binary(byte[] value)
		{
			_value = value;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapBase64Binary Parse(string value)
		{
			return new SoapBase64Binary(Convert.FromBase64String(value));
		}

		public override string ToString()
		{
			return Convert.ToBase64String(_value);
		}
	}
}
