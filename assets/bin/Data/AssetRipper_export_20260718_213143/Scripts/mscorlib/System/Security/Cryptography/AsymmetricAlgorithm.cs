using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class AsymmetricAlgorithm : IDisposable
	{
		protected int KeySizeValue;

		protected KeySizes[] LegalKeySizesValue;

		public abstract string KeyExchangeAlgorithm { get; }

		public virtual int KeySize
		{
			get
			{
				return KeySizeValue;
			}
			set
			{
				if (!KeySizes.IsLegalKeySize(LegalKeySizesValue, value))
				{
					throw new CryptographicException(Locale.GetText("Key size not supported by algorithm."));
				}
				KeySizeValue = value;
			}
		}

		public virtual KeySizes[] LegalKeySizes
		{
			get
			{
				return LegalKeySizesValue;
			}
		}

		public abstract string SignatureAlgorithm { get; }

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Clear()
		{
			Dispose(false);
		}

		protected abstract void Dispose(bool disposing);

		public abstract void FromXmlString(string xmlString);

		public abstract string ToXmlString(bool includePrivateParameters);

		public static AsymmetricAlgorithm Create()
		{
			return Create("System.Security.Cryptography.AsymmetricAlgorithm");
		}

		public static AsymmetricAlgorithm Create(string algName)
		{
			return (AsymmetricAlgorithm)CryptoConfig.CreateFromName(algName);
		}

		internal static byte[] GetNamedParam(string xml, string param)
		{
			string text = "<" + param + ">";
			int num = xml.IndexOf(text);
			if (num == -1)
			{
				return null;
			}
			string value = "</" + param + ">";
			int num2 = xml.IndexOf(value);
			if (num2 == -1 || num2 <= num)
			{
				return null;
			}
			num += text.Length;
			string s = xml.Substring(num, num2 - num);
			return Convert.FromBase64String(s);
		}
	}
}
