using System.Net;

namespace Mono.Data.Tds.Protocol
{
	public class TdsConnectionParameters
	{
		public string ApplicationName;

		public string Database;

		public string Charset;

		public string Hostname;

		public string Language;

		public string LibraryName;

		public string Password;

		public string ProgName;

		public string User;

		public bool DomainLogin;

		public string DefaultDomain;

		public string AttachDBFileName;

		public TdsConnectionParameters()
		{
			Reset();
		}

		public void Reset()
		{
			ApplicationName = "Mono";
			Database = string.Empty;
			Charset = string.Empty;
			Hostname = Dns.GetHostName();
			Language = string.Empty;
			LibraryName = "Mono";
			Password = string.Empty;
			ProgName = "Mono";
			User = string.Empty;
			DomainLogin = false;
			DefaultDomain = string.Empty;
			AttachDBFileName = string.Empty;
		}
	}
}
