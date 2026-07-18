using System;
using System.Globalization;
using System.IO;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.Protocol.Tls.Handshake.Client;

namespace Mono.Security.Protocol.Tls
{
	internal class ClientRecordProtocol : RecordProtocol
	{
		public ClientRecordProtocol(Stream innerStream, ClientContext context)
			: base(innerStream, context)
		{
		}

		public override HandshakeMessage GetMessage(HandshakeType type)
		{
			return createClientHandshakeMessage(type);
		}

		protected override void ProcessHandshakeMessage(TlsStream handMsg)
		{
			HandshakeType handshakeType = (HandshakeType)handMsg.ReadByte();
			HandshakeMessage handshakeMessage = null;
			int num = handMsg.ReadInt24();
			byte[] array = null;
			if (num > 0)
			{
				array = new byte[num];
				handMsg.Read(array, 0, num);
			}
			handshakeMessage = createServerHandshakeMessage(handshakeType, array);
			if (handshakeMessage != null)
			{
				handshakeMessage.Process();
			}
			base.Context.LastHandshakeMsg = handshakeType;
			if (handshakeMessage != null)
			{
				handshakeMessage.Update();
				base.Context.HandshakeMessages.WriteByte((byte)handshakeType);
				base.Context.HandshakeMessages.WriteInt24(num);
				if (num > 0)
				{
					base.Context.HandshakeMessages.Write(array, 0, array.Length);
				}
			}
		}

		private HandshakeMessage createClientHandshakeMessage(HandshakeType type)
		{
			switch (type)
			{
			case HandshakeType.ClientHello:
				return new TlsClientHello(context);
			case HandshakeType.Certificate:
				return new TlsClientCertificate(context);
			case HandshakeType.ClientKeyExchange:
				return new TlsClientKeyExchange(context);
			case HandshakeType.CertificateVerify:
				return new TlsClientCertificateVerify(context);
			case HandshakeType.Finished:
				return new TlsClientFinished(context);
			default:
				throw new InvalidOperationException("Unknown client handshake message type: " + type);
			}
		}

		private HandshakeMessage createServerHandshakeMessage(HandshakeType type, byte[] buffer)
		{
			ClientContext clientContext = (ClientContext)context;
			switch (type)
			{
			case HandshakeType.HelloRequest:
				if (clientContext.HandshakeState != HandshakeState.Started)
				{
					clientContext.HandshakeState = HandshakeState.None;
				}
				else
				{
					SendAlert(AlertLevel.Warning, AlertDescription.NoRenegotiation);
				}
				return null;
			case HandshakeType.ServerHello:
				return new TlsServerHello(context, buffer);
			case HandshakeType.Certificate:
				return new TlsServerCertificate(context, buffer);
			case HandshakeType.ServerKeyExchange:
				return new TlsServerKeyExchange(context, buffer);
			case HandshakeType.CertificateRequest:
				return new TlsServerCertificateRequest(context, buffer);
			case HandshakeType.ServerHelloDone:
				return new TlsServerHelloDone(context, buffer);
			case HandshakeType.Finished:
				return new TlsServerFinished(context, buffer);
			default:
				throw new TlsException(AlertDescription.UnexpectedMessage, string.Format(CultureInfo.CurrentUICulture, "Unknown server handshake message received ({0})", type.ToString()));
			}
		}
	}
}
