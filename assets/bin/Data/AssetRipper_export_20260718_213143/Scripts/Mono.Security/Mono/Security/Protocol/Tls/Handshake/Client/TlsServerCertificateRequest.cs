using System.Text;

namespace Mono.Security.Protocol.Tls.Handshake.Client
{
	internal class TlsServerCertificateRequest : HandshakeMessage
	{
		private ClientCertificateType[] certificateTypes;

		private string[] distinguisedNames;

		public TlsServerCertificateRequest(Context context, byte[] buffer)
			: base(context, HandshakeType.CertificateRequest, buffer)
		{
		}

		public override void Update()
		{
			base.Update();
			base.Context.ServerSettings.CertificateTypes = certificateTypes;
			base.Context.ServerSettings.DistinguisedNames = distinguisedNames;
			base.Context.ServerSettings.CertificateRequest = true;
		}

		protected override void ProcessAsSsl3()
		{
			ProcessAsTls1();
		}

		protected override void ProcessAsTls1()
		{
			int num = ReadByte();
			certificateTypes = new ClientCertificateType[num];
			for (int i = 0; i < num; i++)
			{
				certificateTypes[i] = (ClientCertificateType)ReadByte();
			}
			if (ReadInt16() != 0)
			{
				ASN1 aSN = new ASN1(ReadBytes(ReadInt16()));
				distinguisedNames = new string[aSN.Count];
				for (int j = 0; j < aSN.Count; j++)
				{
					ASN1 aSN2 = new ASN1(aSN[j].Value);
					distinguisedNames[j] = Encoding.UTF8.GetString(aSN2[1].Value);
				}
			}
		}
	}
}
