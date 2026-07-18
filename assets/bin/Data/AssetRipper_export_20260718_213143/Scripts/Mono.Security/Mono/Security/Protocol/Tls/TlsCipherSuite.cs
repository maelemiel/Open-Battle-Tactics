using System;
using System.Security.Cryptography;

namespace Mono.Security.Protocol.Tls
{
	internal class TlsCipherSuite : CipherSuite
	{
		private const int MacHeaderLength = 13;

		private byte[] header;

		public TlsCipherSuite(short code, string name, CipherAlgorithmType cipherAlgorithmType, HashAlgorithmType hashAlgorithmType, ExchangeAlgorithmType exchangeAlgorithmType, bool exportable, bool blockMode, byte keyMaterialSize, byte expandedKeyMaterialSize, short effectiveKeyBytes, byte ivSize, byte blockSize)
			: base(code, name, cipherAlgorithmType, hashAlgorithmType, exchangeAlgorithmType, exportable, blockMode, keyMaterialSize, expandedKeyMaterialSize, effectiveKeyBytes, ivSize, blockSize)
		{
		}

		public override byte[] ComputeServerRecordMAC(ContentType contentType, byte[] fragment)
		{
			if (header == null)
			{
				header = new byte[13];
			}
			ulong value = ((!(base.Context is ClientContext)) ? base.Context.WriteSequenceNumber : base.Context.ReadSequenceNumber);
			Write(header, 0, value);
			header[8] = (byte)contentType;
			Write(header, 9, base.Context.Protocol);
			Write(header, 11, (short)fragment.Length);
			HashAlgorithm hashAlgorithm = base.ServerHMAC;
			hashAlgorithm.TransformBlock(header, 0, header.Length, header, 0);
			hashAlgorithm.TransformBlock(fragment, 0, fragment.Length, fragment, 0);
			hashAlgorithm.TransformFinalBlock(CipherSuite.EmptyArray, 0, 0);
			return hashAlgorithm.Hash;
		}

		public override byte[] ComputeClientRecordMAC(ContentType contentType, byte[] fragment)
		{
			if (header == null)
			{
				header = new byte[13];
			}
			ulong value = ((!(base.Context is ClientContext)) ? base.Context.ReadSequenceNumber : base.Context.WriteSequenceNumber);
			Write(header, 0, value);
			header[8] = (byte)contentType;
			Write(header, 9, base.Context.Protocol);
			Write(header, 11, (short)fragment.Length);
			HashAlgorithm hashAlgorithm = base.ClientHMAC;
			hashAlgorithm.TransformBlock(header, 0, header.Length, header, 0);
			hashAlgorithm.TransformBlock(fragment, 0, fragment.Length, fragment, 0);
			hashAlgorithm.TransformFinalBlock(CipherSuite.EmptyArray, 0, 0);
			return hashAlgorithm.Hash;
		}

		public override void ComputeMasterSecret(byte[] preMasterSecret)
		{
			base.Context.MasterSecret = new byte[preMasterSecret.Length];
			base.Context.MasterSecret = PRF(preMasterSecret, "master secret", base.Context.RandomCS, 48);
		}

		public override void ComputeKeys()
		{
			TlsStream tlsStream = new TlsStream(PRF(base.Context.MasterSecret, "key expansion", base.Context.RandomSC, base.KeyBlockSize));
			base.Context.Negotiating.ClientWriteMAC = tlsStream.ReadBytes(base.HashSize);
			base.Context.Negotiating.ServerWriteMAC = tlsStream.ReadBytes(base.HashSize);
			base.Context.ClientWriteKey = tlsStream.ReadBytes(base.KeyMaterialSize);
			base.Context.ServerWriteKey = tlsStream.ReadBytes(base.KeyMaterialSize);
			if (!base.IsExportable)
			{
				if (base.IvSize != 0)
				{
					base.Context.ClientWriteIV = tlsStream.ReadBytes(base.IvSize);
					base.Context.ServerWriteIV = tlsStream.ReadBytes(base.IvSize);
				}
				else
				{
					base.Context.ClientWriteIV = CipherSuite.EmptyArray;
					base.Context.ServerWriteIV = CipherSuite.EmptyArray;
				}
			}
			else
			{
				byte[] clientWriteKey = PRF(base.Context.ClientWriteKey, "client write key", base.Context.RandomCS, base.ExpandedKeyMaterialSize);
				byte[] serverWriteKey = PRF(base.Context.ServerWriteKey, "server write key", base.Context.RandomCS, base.ExpandedKeyMaterialSize);
				base.Context.ClientWriteKey = clientWriteKey;
				base.Context.ServerWriteKey = serverWriteKey;
				if (base.IvSize > 0)
				{
					byte[] src = PRF(CipherSuite.EmptyArray, "IV block", base.Context.RandomCS, base.IvSize * 2);
					base.Context.ClientWriteIV = new byte[base.IvSize];
					Buffer.BlockCopy(src, 0, base.Context.ClientWriteIV, 0, base.Context.ClientWriteIV.Length);
					base.Context.ServerWriteIV = new byte[base.IvSize];
					Buffer.BlockCopy(src, base.IvSize, base.Context.ServerWriteIV, 0, base.Context.ServerWriteIV.Length);
				}
				else
				{
					base.Context.ClientWriteIV = CipherSuite.EmptyArray;
					base.Context.ServerWriteIV = CipherSuite.EmptyArray;
				}
			}
			ClientSessionCache.SetContextInCache(base.Context);
			tlsStream.Reset();
		}
	}
}
