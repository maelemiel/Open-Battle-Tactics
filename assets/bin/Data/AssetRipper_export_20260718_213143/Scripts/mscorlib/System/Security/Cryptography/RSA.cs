using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class RSA : AsymmetricAlgorithm
	{
		public new static RSA Create()
		{
			return Create("System.Security.Cryptography.RSA");
		}

		public new static RSA Create(string algName)
		{
			return (RSA)CryptoConfig.CreateFromName(algName);
		}

		public abstract byte[] EncryptValue(byte[] rgb);

		public abstract byte[] DecryptValue(byte[] rgb);

		public abstract RSAParameters ExportParameters(bool includePrivateParameters);

		public abstract void ImportParameters(RSAParameters parameters);

		internal void ZeroizePrivateKey(RSAParameters parameters)
		{
			if (parameters.P != null)
			{
				Array.Clear(parameters.P, 0, parameters.P.Length);
			}
			if (parameters.Q != null)
			{
				Array.Clear(parameters.Q, 0, parameters.Q.Length);
			}
			if (parameters.DP != null)
			{
				Array.Clear(parameters.DP, 0, parameters.DP.Length);
			}
			if (parameters.DQ != null)
			{
				Array.Clear(parameters.DQ, 0, parameters.DQ.Length);
			}
			if (parameters.InverseQ != null)
			{
				Array.Clear(parameters.InverseQ, 0, parameters.InverseQ.Length);
			}
			if (parameters.D != null)
			{
				Array.Clear(parameters.D, 0, parameters.D.Length);
			}
		}

		public override void FromXmlString(string xmlString)
		{
			if (xmlString == null)
			{
				throw new ArgumentNullException("xmlString");
			}
			RSAParameters parameters = default(RSAParameters);
			try
			{
				parameters.P = AsymmetricAlgorithm.GetNamedParam(xmlString, "P");
				parameters.Q = AsymmetricAlgorithm.GetNamedParam(xmlString, "Q");
				parameters.D = AsymmetricAlgorithm.GetNamedParam(xmlString, "D");
				parameters.DP = AsymmetricAlgorithm.GetNamedParam(xmlString, "DP");
				parameters.DQ = AsymmetricAlgorithm.GetNamedParam(xmlString, "DQ");
				parameters.InverseQ = AsymmetricAlgorithm.GetNamedParam(xmlString, "InverseQ");
				parameters.Exponent = AsymmetricAlgorithm.GetNamedParam(xmlString, "Exponent");
				parameters.Modulus = AsymmetricAlgorithm.GetNamedParam(xmlString, "Modulus");
				ImportParameters(parameters);
			}
			catch (Exception inner)
			{
				ZeroizePrivateKey(parameters);
				throw new CryptographicException(Locale.GetText("Couldn't decode XML"), inner);
			}
			finally
			{
				ZeroizePrivateKey(parameters);
			}
		}

		public override string ToXmlString(bool includePrivateParameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			RSAParameters parameters = ExportParameters(includePrivateParameters);
			try
			{
				stringBuilder.Append("<RSAKeyValue>");
				stringBuilder.Append("<Modulus>");
				stringBuilder.Append(Convert.ToBase64String(parameters.Modulus));
				stringBuilder.Append("</Modulus>");
				stringBuilder.Append("<Exponent>");
				stringBuilder.Append(Convert.ToBase64String(parameters.Exponent));
				stringBuilder.Append("</Exponent>");
				if (includePrivateParameters)
				{
					if (parameters.D == null)
					{
						string text = Locale.GetText("Missing D parameter for the private key.");
						throw new ArgumentNullException(text);
					}
					if (parameters.P == null || parameters.Q == null || parameters.DP == null || parameters.DQ == null || parameters.InverseQ == null)
					{
						string text2 = Locale.GetText("Missing some CRT parameters for the private key.");
						throw new CryptographicException(text2);
					}
					stringBuilder.Append("<P>");
					stringBuilder.Append(Convert.ToBase64String(parameters.P));
					stringBuilder.Append("</P>");
					stringBuilder.Append("<Q>");
					stringBuilder.Append(Convert.ToBase64String(parameters.Q));
					stringBuilder.Append("</Q>");
					stringBuilder.Append("<DP>");
					stringBuilder.Append(Convert.ToBase64String(parameters.DP));
					stringBuilder.Append("</DP>");
					stringBuilder.Append("<DQ>");
					stringBuilder.Append(Convert.ToBase64String(parameters.DQ));
					stringBuilder.Append("</DQ>");
					stringBuilder.Append("<InverseQ>");
					stringBuilder.Append(Convert.ToBase64String(parameters.InverseQ));
					stringBuilder.Append("</InverseQ>");
					stringBuilder.Append("<D>");
					stringBuilder.Append(Convert.ToBase64String(parameters.D));
					stringBuilder.Append("</D>");
				}
				stringBuilder.Append("</RSAKeyValue>");
			}
			catch
			{
				ZeroizePrivateKey(parameters);
				throw;
			}
			return stringBuilder.ToString();
		}
	}
}
