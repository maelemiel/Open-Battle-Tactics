using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public class CodeConnectAccess
	{
		public static readonly string AnyScheme = "*";

		public static readonly int DefaultPort = -3;

		public static readonly int OriginPort = -4;

		public static readonly string OriginScheme = "$origin";

		private string _scheme;

		private int _port;

		public int Port
		{
			get
			{
				return _port;
			}
		}

		public string Scheme
		{
			get
			{
				return _scheme;
			}
		}

		[MonoTODO("(2.0) validations incomplete")]
		public CodeConnectAccess(string allowScheme, int allowPort)
		{
			if (allowScheme == null || allowScheme.Length == 0)
			{
				throw new ArgumentOutOfRangeException("allowScheme");
			}
			if (allowPort < 0 || allowPort > 65535)
			{
				throw new ArgumentOutOfRangeException("allowPort");
			}
			_scheme = allowScheme;
			_port = allowPort;
		}

		public override bool Equals(object o)
		{
			CodeConnectAccess codeConnectAccess = o as CodeConnectAccess;
			if (codeConnectAccess == null)
			{
				return false;
			}
			return _scheme == codeConnectAccess._scheme && _port == codeConnectAccess._port;
		}

		public override int GetHashCode()
		{
			return _scheme.GetHashCode() ^ _port;
		}

		public static CodeConnectAccess CreateAnySchemeAccess(int allowPort)
		{
			return new CodeConnectAccess(AnyScheme, allowPort);
		}

		public static CodeConnectAccess CreateOriginSchemeAccess(int allowPort)
		{
			return new CodeConnectAccess(OriginScheme, allowPort);
		}
	}
}
