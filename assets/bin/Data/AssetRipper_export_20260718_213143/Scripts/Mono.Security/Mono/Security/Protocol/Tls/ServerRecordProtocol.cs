using System;
using System.Globalization;
using System.IO;
using Mono.Security.Protocol.Tls.Handshake;
using Mono.Security.Protocol.Tls.Handshake.Server;

namespace Mono.Security.Protocol.Tls
{
	internal class ServerRecordProtocol : RecordProtocol
	{
		public ServerRecordProtocol(Stream innerStream, ServerContext context)
			: base(innerStream, context)
		{
		}

		public override HandshakeMessage GetMessage(HandshakeType type)
		{
			return createServerHandshakeMessage(type);
		}

		protected override void ProcessHandshakeMessage(TlsStream handMsg)
		{
			HandshakeType handshakeType = (HandshakeType)handMsg.ReadByte();
			HandshakeMessage handshakeMessage = null;
			int num = handMsg.ReadInt24();
			byte[] array = new byte[num];
			handMsg.Read(array, 0, num);
			handshakeMessage = createClientHandshakeMessage(handshakeType, array);
			handshakeMessage.Process();
			base.Context.LastHandshakeMsg = handshakeType;
			if (handshakeMessage != null)
			{
				handshakeMessage.Update();
				base.Context.HandshakeMessages.WriteByte((byte)handshakeType);
				base.Context.HandshakeMessages.WriteInt24(num);
				base.Context.HandshakeMessages.Write(array, 0, array.Length);
			}
		}

		private HandshakeMessage createClientHandshakeMessage(HandshakeType type, byte[] buffer)
		{
			switch (type)
			{
			case HandshakeType.ClientHello:
				return new TlsClientHello(context, buffer);
			case HandshakeType.Certificate:
				return new TlsClientCertificate(context, buffer);
			case HandshakeType.ClientKeyExchange:
				return new TlsClientKeyExchange(context, buffer);
			case HandshakeType.CertificateVerify:
				return new TlsClientCertificateVerify(context, buffer);
			case HandshakeType.Finished:
				return new TlsClientFinished(context, buffer);
			default:
				throw new TlsException(AlertDescription.UnexpectedMessage, string.Format(CultureInfo.CurrentUICulture, "Unknown server handshake message received ({0})", type.ToString()));
			}
		}

		private HandshakeMessage createServerHandshakeMessage(HandshakeType type)
		{
			switch (type)
			{
			case HandshakeType.HelloRequest:
				SendRecord(HandshakeType.ClientHello);
				return null;
			case HandshakeType.ServerHello:
				return new TlsServerHello(context);
			case HandshakeType.Certificate:
				return new TlsServerCertificate(context);
			case HandshakeType.ServerKeyExchange:
				return new TlsServerKeyExchange(context);
			case HandshakeType.CertificateRequest:
				return new TlsServerCertificateRequest(context);
			case HandshakeType.ServerHelloDone:
				return new TlsServerHelloDone(context);
			case HandshakeType.Finished:
				return new TlsServerFinished(context);
			default:
				throw new InvalidOperationException("Unknown server handshake message type: " + type);
			}
		}
	}
}
