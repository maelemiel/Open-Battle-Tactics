using System;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Mono.Xml;

namespace Mono.Security.Cryptography
{
	public abstract class DiffieHellman : AsymmetricAlgorithm
	{
		public new static DiffieHellman Create()
		{
			return Create("Mono.Security.Cryptography.DiffieHellman");
		}

		public new static DiffieHellman Create(string algName)
		{
			return (DiffieHellman)CryptoConfig.CreateFromName(algName);
		}

		public abstract byte[] CreateKeyExchange();

		public abstract byte[] DecryptKeyExchange(byte[] keyex);

		public abstract DHParameters ExportParameters(bool includePrivate);

		public abstract void ImportParameters(DHParameters parameters);

		private byte[] GetNamedParam(SecurityElement se, string param)
		{
			SecurityElement securityElement = se.SearchForChildByTag(param);
			if (securityElement == null)
			{
				return null;
			}
			return Convert.FromBase64String(securityElement.Text);
		}

		public override void FromXmlString(string xmlString)
		{
			if (xmlString == null)
			{
				throw new ArgumentNullException("xmlString");
			}
			DHParameters parameters = default(DHParameters);
			try
			{
				SecurityParser securityParser = new SecurityParser();
				securityParser.LoadXml(xmlString);
				SecurityElement securityElement = securityParser.ToXml();
				if (securityElement.Tag != "DHKeyValue")
				{
					throw new CryptographicException();
				}
				parameters.P = GetNamedParam(securityElement, "P");
				parameters.G = GetNamedParam(securityElement, "G");
				parameters.X = GetNamedParam(securityElement, "X");
				ImportParameters(parameters);
			}
			finally
			{
				if (parameters.P != null)
				{
					Array.Clear(parameters.P, 0, parameters.P.Length);
				}
				if (parameters.G != null)
				{
					Array.Clear(parameters.G, 0, parameters.G.Length);
				}
				if (parameters.X != null)
				{
					Array.Clear(parameters.X, 0, parameters.X.Length);
				}
			}
		}

		public override string ToXmlString(bool includePrivateParameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DHParameters dHParameters = ExportParameters(includePrivateParameters);
			try
			{
				stringBuilder.Append("<DHKeyValue>");
				stringBuilder.Append("<P>");
				stringBuilder.Append(Convert.ToBase64String(dHParameters.P));
				stringBuilder.Append("</P>");
				stringBuilder.Append("<G>");
				stringBuilder.Append(Convert.ToBase64String(dHParameters.G));
				stringBuilder.Append("</G>");
				if (includePrivateParameters)
				{
					stringBuilder.Append("<X>");
					stringBuilder.Append(Convert.ToBase64String(dHParameters.X));
					stringBuilder.Append("</X>");
				}
				stringBuilder.Append("</DHKeyValue>");
			}
			finally
			{
				Array.Clear(dHParameters.P, 0, dHParameters.P.Length);
				Array.Clear(dHParameters.G, 0, dHParameters.G.Length);
				if (dHParameters.X != null)
				{
					Array.Clear(dHParameters.X, 0, dHParameters.X.Length);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
