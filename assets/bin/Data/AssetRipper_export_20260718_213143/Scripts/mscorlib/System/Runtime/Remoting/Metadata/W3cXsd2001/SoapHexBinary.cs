using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapHexBinary : ISoapXsd
	{
		private byte[] _value;

		private StringBuilder sb = new StringBuilder();

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
				return "hexBinary";
			}
		}

		public SoapHexBinary()
		{
		}

		public SoapHexBinary(byte[] value)
		{
			_value = value;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapHexBinary Parse(string value)
		{
			byte[] value2 = FromBinHexString(value);
			return new SoapHexBinary(value2);
		}

		internal static byte[] FromBinHexString(string value)
		{
			char[] array = value.ToCharArray();
			byte[] array2 = new byte[array.Length / 2 + array.Length % 2];
			int num = array.Length;
			if (num % 2 != 0)
			{
				throw CreateInvalidValueException(value);
			}
			int num2 = 0;
			for (int i = 0; i < num - 1; i += 2)
			{
				array2[num2] = FromHex(array[i], value);
				array2[num2] <<= 4;
				array2[num2] += FromHex(array[i + 1], value);
				num2++;
			}
			return array2;
		}

		private static byte FromHex(char hexDigit, string value)
		{
			try
			{
				return byte.Parse(hexDigit.ToString(), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}
			catch (FormatException)
			{
				throw CreateInvalidValueException(value);
			}
		}

		private static Exception CreateInvalidValueException(string value)
		{
			return new RemotingException(string.Format(CultureInfo.InvariantCulture, "Invalid value '{0}' for xsd:{1}.", value, XsdType));
		}

		public override string ToString()
		{
			sb.Length = 0;
			byte[] value = _value;
			foreach (byte b in value)
			{
				sb.Append(b.ToString("X2"));
			}
			return sb.ToString();
		}
	}
}
