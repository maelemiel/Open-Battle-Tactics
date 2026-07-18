using System;
using System.Security.Cryptography;
using Mono.Security.Protocol.Tls.Handshake;

namespace Mono.Security.Protocol.Tls
{
	internal abstract class Context
	{
		internal const short MAX_FRAGMENT_SIZE = 16384;

		internal const short TLS1_PROTOCOL_CODE = 769;

		internal const short SSL3_PROTOCOL_CODE = 768;

		internal const long UNIX_BASE_TICKS = 621355968000000000L;

		private SecurityProtocolType securityProtocol;

		private byte[] sessionId;

		private SecurityCompressionType compressionMethod;

		private TlsServerSettings serverSettings;

		private TlsClientSettings clientSettings;

		private SecurityParameters current;

		private SecurityParameters negotiating;

		private SecurityParameters read;

		private SecurityParameters write;

		private CipherSuiteCollection supportedCiphers;

		private HandshakeType lastHandshakeMsg;

		private HandshakeState handshakeState;

		private bool abbreviatedHandshake;

		private bool receivedConnectionEnd;

		private bool sentConnectionEnd;

		private bool protocolNegotiated;

		private ulong writeSequenceNumber;

		private ulong readSequenceNumber;

		private byte[] clientRandom;

		private byte[] serverRandom;

		private byte[] randomCS;

		private byte[] randomSC;

		private byte[] masterSecret;

		private byte[] clientWriteKey;

		private byte[] serverWriteKey;

		private byte[] clientWriteIV;

		private byte[] serverWriteIV;

		private TlsStream handshakeMessages;

		private RandomNumberGenerator random;

		private RecordProtocol recordProtocol;

		public bool AbbreviatedHandshake
		{
			get
			{
				return abbreviatedHandshake;
			}
			set
			{
				abbreviatedHandshake = value;
			}
		}

		public bool ProtocolNegotiated
		{
			get
			{
				return protocolNegotiated;
			}
			set
			{
				protocolNegotiated = value;
			}
		}

		public SecurityProtocolType SecurityProtocol
		{
			get
			{
				if ((securityProtocol & SecurityProtocolType.Tls) == SecurityProtocolType.Tls || (securityProtocol & SecurityProtocolType.Default) == SecurityProtocolType.Default)
				{
					return SecurityProtocolType.Tls;
				}
				if ((securityProtocol & SecurityProtocolType.Ssl3) == SecurityProtocolType.Ssl3)
				{
					return SecurityProtocolType.Ssl3;
				}
				throw new NotSupportedException("Unsupported security protocol type");
			}
			set
			{
				securityProtocol = value;
			}
		}

		public SecurityProtocolType SecurityProtocolFlags
		{
			get
			{
				return securityProtocol;
			}
		}

		public short Protocol
		{
			get
			{
				switch (SecurityProtocol)
				{
				case SecurityProtocolType.Default:
				case SecurityProtocolType.Tls:
					return 769;
				case SecurityProtocolType.Ssl3:
					return 768;
				default:
					throw new NotSupportedException("Unsupported security protocol type");
				}
			}
		}

		public byte[] SessionId
		{
			get
			{
				return sessionId;
			}
			set
			{
				sessionId = value;
			}
		}

		public SecurityCompressionType CompressionMethod
		{
			get
			{
				return compressionMethod;
			}
			set
			{
				compressionMethod = value;
			}
		}

		public TlsServerSettings ServerSettings
		{
			get
			{
				return serverSettings;
			}
		}

		public TlsClientSettings ClientSettings
		{
			get
			{
				return clientSettings;
			}
		}

		public HandshakeType LastHandshakeMsg
		{
			get
			{
				return lastHandshakeMsg;
			}
			set
			{
				lastHandshakeMsg = value;
			}
		}

		public HandshakeState HandshakeState
		{
			get
			{
				return handshakeState;
			}
			set
			{
				handshakeState = value;
			}
		}

		public bool ReceivedConnectionEnd
		{
			get
			{
				return receivedConnectionEnd;
			}
			set
			{
				receivedConnectionEnd = value;
			}
		}

		public bool SentConnectionEnd
		{
			get
			{
				return sentConnectionEnd;
			}
			set
			{
				sentConnectionEnd = value;
			}
		}

		public CipherSuiteCollection SupportedCiphers
		{
			get
			{
				return supportedCiphers;
			}
			set
			{
				supportedCiphers = value;
			}
		}

		public TlsStream HandshakeMessages
		{
			get
			{
				return handshakeMessages;
			}
		}

		public ulong WriteSequenceNumber
		{
			get
			{
				return writeSequenceNumber;
			}
			set
			{
				writeSequenceNumber = value;
			}
		}

		public ulong ReadSequenceNumber
		{
			get
			{
				return readSequenceNumber;
			}
			set
			{
				readSequenceNumber = value;
			}
		}

		public byte[] ClientRandom
		{
			get
			{
				return clientRandom;
			}
			set
			{
				clientRandom = value;
			}
		}

		public byte[] ServerRandom
		{
			get
			{
				return serverRandom;
			}
			set
			{
				serverRandom = value;
			}
		}

		public byte[] RandomCS
		{
			get
			{
				return randomCS;
			}
			set
			{
				randomCS = value;
			}
		}

		public byte[] RandomSC
		{
			get
			{
				return randomSC;
			}
			set
			{
				randomSC = value;
			}
		}

		public byte[] MasterSecret
		{
			get
			{
				return masterSecret;
			}
			set
			{
				masterSecret = value;
			}
		}

		public byte[] ClientWriteKey
		{
			get
			{
				return clientWriteKey;
			}
			set
			{
				clientWriteKey = value;
			}
		}

		public byte[] ServerWriteKey
		{
			get
			{
				return serverWriteKey;
			}
			set
			{
				serverWriteKey = value;
			}
		}

		public byte[] ClientWriteIV
		{
			get
			{
				return clientWriteIV;
			}
			set
			{
				clientWriteIV = value;
			}
		}

		public byte[] ServerWriteIV
		{
			get
			{
				return serverWriteIV;
			}
			set
			{
				serverWriteIV = value;
			}
		}

		public RecordProtocol RecordProtocol
		{
			get
			{
				return recordProtocol;
			}
			set
			{
				recordProtocol = value;
			}
		}

		public SecurityParameters Current
		{
			get
			{
				if (current == null)
				{
					current = new SecurityParameters();
				}
				if (current.Cipher != null)
				{
					current.Cipher.Context = this;
				}
				return current;
			}
		}

		public SecurityParameters Negotiating
		{
			get
			{
				if (negotiating == null)
				{
					negotiating = new SecurityParameters();
				}
				if (negotiating.Cipher != null)
				{
					negotiating.Cipher.Context = this;
				}
				return negotiating;
			}
		}

		public SecurityParameters Read
		{
			get
			{
				return read;
			}
		}

		public SecurityParameters Write
		{
			get
			{
				return write;
			}
		}

		public Context(SecurityProtocolType securityProtocolType)
		{
			SecurityProtocol = securityProtocolType;
			compressionMethod = SecurityCompressionType.None;
			serverSettings = new TlsServerSettings();
			clientSettings = new TlsClientSettings();
			handshakeMessages = new TlsStream();
			sessionId = null;
			handshakeState = HandshakeState.None;
			random = RandomNumberGenerator.Create();
		}

		public int GetUnixTime()
		{
			return (int)((DateTime.UtcNow.Ticks - 621355968000000000L) / 10000000);
		}

		public byte[] GetSecureRandomBytes(int count)
		{
			byte[] array = new byte[count];
			random.GetNonZeroBytes(array);
			return array;
		}

		public virtual void Clear()
		{
			compressionMethod = SecurityCompressionType.None;
			serverSettings = new TlsServerSettings();
			clientSettings = new TlsClientSettings();
			handshakeMessages = new TlsStream();
			sessionId = null;
			handshakeState = HandshakeState.None;
			ClearKeyInfo();
		}

		public virtual void ClearKeyInfo()
		{
			if (masterSecret != null)
			{
				Array.Clear(masterSecret, 0, masterSecret.Length);
				masterSecret = null;
			}
			if (clientRandom != null)
			{
				Array.Clear(clientRandom, 0, clientRandom.Length);
				clientRandom = null;
			}
			if (serverRandom != null)
			{
				Array.Clear(serverRandom, 0, serverRandom.Length);
				serverRandom = null;
			}
			if (randomCS != null)
			{
				Array.Clear(randomCS, 0, randomCS.Length);
				randomCS = null;
			}
			if (randomSC != null)
			{
				Array.Clear(randomSC, 0, randomSC.Length);
				randomSC = null;
			}
			if (clientWriteKey != null)
			{
				Array.Clear(clientWriteKey, 0, clientWriteKey.Length);
				clientWriteKey = null;
			}
			if (clientWriteIV != null)
			{
				Array.Clear(clientWriteIV, 0, clientWriteIV.Length);
				clientWriteIV = null;
			}
			if (serverWriteKey != null)
			{
				Array.Clear(serverWriteKey, 0, serverWriteKey.Length);
				serverWriteKey = null;
			}
			if (serverWriteIV != null)
			{
				Array.Clear(serverWriteIV, 0, serverWriteIV.Length);
				serverWriteIV = null;
			}
			handshakeMessages.Reset();
			if (securityProtocol == SecurityProtocolType.Ssl3)
			{
			}
		}

		public SecurityProtocolType DecodeProtocolCode(short code)
		{
			switch (code)
			{
			case 769:
				return SecurityProtocolType.Tls;
			case 768:
				return SecurityProtocolType.Ssl3;
			default:
				throw new NotSupportedException("Unsupported security protocol type");
			}
		}

		public void ChangeProtocol(short protocol)
		{
			SecurityProtocolType securityProtocolType = DecodeProtocolCode(protocol);
			if ((securityProtocolType & SecurityProtocolFlags) == securityProtocolType || (SecurityProtocolFlags & SecurityProtocolType.Default) == SecurityProtocolType.Default)
			{
				SecurityProtocol = securityProtocolType;
				SupportedCiphers.Clear();
				SupportedCiphers = null;
				SupportedCiphers = CipherSuiteFactory.GetSupportedCiphers(securityProtocolType);
				return;
			}
			throw new TlsException(AlertDescription.ProtocolVersion, "Incorrect protocol version received from server");
		}

		public void StartSwitchingSecurityParameters(bool client)
		{
			if (client)
			{
				write = negotiating;
				read = current;
			}
			else
			{
				read = negotiating;
				write = current;
			}
			current = negotiating;
		}

		public void EndSwitchingSecurityParameters(bool client)
		{
			SecurityParameters securityParameters;
			if (client)
			{
				securityParameters = read;
				read = current;
			}
			else
			{
				securityParameters = write;
				write = current;
			}
			if (securityParameters != null)
			{
				securityParameters.Clear();
			}
			negotiating = securityParameters;
		}
	}
}
