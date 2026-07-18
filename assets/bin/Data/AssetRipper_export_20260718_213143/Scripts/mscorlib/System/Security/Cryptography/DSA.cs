using System.Runtime.InteropServices;
using System.Text;
using Mono.Security;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class DSA : AsymmetricAlgorithm
	{
		public new static DSA Create()
		{
			return Create("System.Security.Cryptography.DSA");
		}

		public new static DSA Create(string algName)
		{
			return (DSA)CryptoConfig.CreateFromName(algName);
		}

		public abstract byte[] CreateSignature(byte[] rgbHash);

		public abstract DSAParameters ExportParameters(bool includePrivateParameters);

		internal void ZeroizePrivateKey(DSAParameters parameters)
		{
			if (parameters.X != null)
			{
				Array.Clear(parameters.X, 0, parameters.X.Length);
			}
		}

		public override void FromXmlString(string xmlString)
		{
			if (xmlString == null)
			{
				throw new ArgumentNullException("xmlString");
			}
			DSAParameters parameters = default(DSAParameters);
			try
			{
				parameters.P = AsymmetricAlgorithm.GetNamedParam(xmlString, "P");
				parameters.Q = AsymmetricAlgorithm.GetNamedParam(xmlString, "Q");
				parameters.G = AsymmetricAlgorithm.GetNamedParam(xmlString, "G");
				parameters.J = AsymmetricAlgorithm.GetNamedParam(xmlString, "J");
				parameters.Y = AsymmetricAlgorithm.GetNamedParam(xmlString, "Y");
				parameters.X = AsymmetricAlgorithm.GetNamedParam(xmlString, "X");
				parameters.Seed = AsymmetricAlgorithm.GetNamedParam(xmlString, "Seed");
				byte[] namedParam = AsymmetricAlgorithm.GetNamedParam(xmlString, "PgenCounter");
				if (namedParam != null)
				{
					byte[] array = new byte[4];
					Buffer.BlockCopy(namedParam, 0, array, 0, namedParam.Length);
					parameters.Counter = BitConverterLE.ToInt32(array, 0);
				}
				ImportParameters(parameters);
			}
			catch
			{
				ZeroizePrivateKey(parameters);
				throw;
			}
			finally
			{
				ZeroizePrivateKey(parameters);
			}
		}

		public abstract void ImportParameters(DSAParameters parameters);

		public override string ToXmlString(bool includePrivateParameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DSAParameters parameters = ExportParameters(includePrivateParameters);
			try
			{
				stringBuilder.Append("<DSAKeyValue>");
				stringBuilder.Append("<P>");
				stringBuilder.Append(Convert.ToBase64String(parameters.P));
				stringBuilder.Append("</P>");
				stringBuilder.Append("<Q>");
				stringBuilder.Append(Convert.ToBase64String(parameters.Q));
				stringBuilder.Append("</Q>");
				stringBuilder.Append("<G>");
				stringBuilder.Append(Convert.ToBase64String(parameters.G));
				stringBuilder.Append("</G>");
				stringBuilder.Append("<Y>");
				stringBuilder.Append(Convert.ToBase64String(parameters.Y));
				stringBuilder.Append("</Y>");
				if (parameters.J != null)
				{
					stringBuilder.Append("<J>");
					stringBuilder.Append(Convert.ToBase64String(parameters.J));
					stringBuilder.Append("</J>");
				}
				if (parameters.Seed != null)
				{
					stringBuilder.Append("<Seed>");
					stringBuilder.Append(Convert.ToBase64String(parameters.Seed));
					stringBuilder.Append("</Seed>");
					stringBuilder.Append("<PgenCounter>");
					if (parameters.Counter != 0)
					{
						byte[] bytes = BitConverterLE.GetBytes(parameters.Counter);
						int num = bytes.Length;
						while (bytes[num - 1] == 0)
						{
							num--;
						}
						stringBuilder.Append(Convert.ToBase64String(bytes, 0, num));
					}
					else
					{
						stringBuilder.Append("AA==");
					}
					stringBuilder.Append("</PgenCounter>");
				}
				if (parameters.X != null)
				{
					stringBuilder.Append("<X>");
					stringBuilder.Append(Convert.ToBase64String(parameters.X));
					stringBuilder.Append("</X>");
				}
				else if (includePrivateParameters)
				{
					throw new ArgumentNullException("X");
				}
				stringBuilder.Append("</DSAKeyValue>");
			}
			catch
			{
				ZeroizePrivateKey(parameters);
				throw;
			}
			return stringBuilder.ToString();
		}

		public abstract bool VerifySignature(byte[] rgbHash, byte[] rgbSignature);
	}
}
