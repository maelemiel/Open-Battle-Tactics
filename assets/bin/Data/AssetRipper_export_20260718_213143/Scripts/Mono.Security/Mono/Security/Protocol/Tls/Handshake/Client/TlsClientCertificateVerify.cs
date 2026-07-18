using System;
using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsClientCertificateVerify : HandshakeMessage
	{
		public TlsClientCertificateVerify(Context context)
			: base(context, HandshakeType.CertificateVerify)
		{
		}

		public override void Update()
		{
			base.Update();
			Reset();
		}

		protected override void ProcessAsSsl3()
		{
			AsymmetricAlgorithm asymmetricAlgorithm = null;
			ClientContext clientContext = (ClientContext)base.Context;
			asymmetricAlgorithm = clientContext.SslStream.RaisePrivateKeySelection(clientContext.ClientSettings.ClientCertificate, clientContext.ClientSettings.TargetHost);
			if (asymmetricAlgorithm == null)
			{
				throw new TlsException(AlertDescription.UserCancelled, "Client certificate Private Key unavailable.");
			}
			SslHandshakeHash sslHandshakeHash = new SslHandshakeHash(clientContext.MasterSecret);
			sslHandshakeHash.TransformFinalBlock(clientContext.HandshakeMessages.ToArray(), 0, (int)clientContext.HandshakeMessages.Length);
			byte[] array = null;
			if (!(asymmetricAlgorithm is RSACryptoServiceProvider))
			{
				try
				{
					array = sslHandshakeHash.CreateSignature((RSA)asymmetricAlgorithm);
				}
				catch (NotImplementedException)
				{
				}
			}
			if (array == null)
			{
				RSA clientCertRSA = getClientCertRSA((RSA)asymmetricAlgorithm);
				array = sslHandshakeHash.CreateSignature(clientCertRSA);
			}
			Write((short)array.Length);
			Write(array, 0, array.Length);
		}

		protected override void ProcessAsTls1()
		{
			AsymmetricAlgorithm asymmetricAlgorithm = null;
			ClientContext clientContext = (ClientContext)base.Context;
			asymmetricAlgorithm = clientContext.SslStream.RaisePrivateKeySelection(clientContext.ClientSettings.ClientCertificate, clientContext.ClientSettings.TargetHost);
			if (asymmetricAlgorithm == null)
			{
				throw new TlsException(AlertDescription.UserCancelled, "Client certificate Private Key unavailable.");
			}
			MD5SHA1 mD5SHA = new MD5SHA1();
			mD5SHA.ComputeHash(clientContext.HandshakeMessages.ToArray(), 0, (int)clientContext.HandshakeMessages.Length);
			byte[] array = null;
			if (!(asymmetricAlgorithm is RSACryptoServiceProvider))
			{
				try
				{
					array = mD5SHA.CreateSignature((RSA)asymmetricAlgorithm);
				}
				catch (NotImplementedException)
				{
				}
			}
			if (array == null)
			{
				RSA clientCertRSA = getClientCertRSA((RSA)asymmetricAlgorithm);
				array = mD5SHA.CreateSignature(clientCertRSA);
			}
			Write((short)array.Length);
			Write(array, 0, array.Length);
		}

		private RSA getClientCertRSA(RSA privKey)
		{
			RSAParameters parameters = default(RSAParameters);
			RSAParameters rSAParameters = privKey.ExportParameters(true);
			ASN1 aSN = new ASN1(base.Context.ClientSettings.Certificates[0].GetPublicKey());
			ASN1 aSN2 = aSN[0];
			if (aSN2 == null || aSN2.Tag != 2)
			{
				return null;
			}
			ASN1 aSN3 = aSN[1];
			if (aSN3.Tag != 2)
			{
				return null;
			}
			parameters.Modulus = getUnsignedBigInteger(aSN2.Value);
			parameters.Exponent = aSN3.Value;
			parameters.D = rSAParameters.D;
			parameters.DP = rSAParameters.DP;
			parameters.DQ = rSAParameters.DQ;
			parameters.InverseQ = rSAParameters.InverseQ;
			parameters.P = rSAParameters.P;
			parameters.Q = rSAParameters.Q;
			int keySize = parameters.Modulus.Length << 3;
			RSAManaged rSAManaged = new RSAManaged(keySize);
			rSAManaged.ImportParameters(parameters);
			return rSAManaged;
		}

		private byte[] getUnsignedBigInteger(byte[] integer)
		{
			if (integer[0] == 0)
			{
				int num = integer.Length - 1;
				byte[] array = new byte[num];
				Buffer.BlockCopy(integer, 1, array, 0, num);
				return array;
			}
			return integer;
		}
	}
}
