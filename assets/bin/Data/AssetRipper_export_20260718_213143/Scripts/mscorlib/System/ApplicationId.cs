using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ApplicationId
	{
		private byte[] _token;

		private string _name;

		private Version _version;

		private string _proc;

		private string _culture;

		public string Culture
		{
			get
			{
				return _culture;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public string ProcessorArchitecture
		{
			get
			{
				return _proc;
			}
		}

		public byte[] PublicKeyToken
		{
			get
			{
				return (byte[])_token.Clone();
			}
		}

		public Version Version
		{
			get
			{
				return _version;
			}
		}

		public ApplicationId(byte[] publicKeyToken, string name, Version version, string processorArchitecture, string culture)
		{
			if (publicKeyToken == null)
			{
				throw new ArgumentNullException("publicKeyToken");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			_token = (byte[])publicKeyToken.Clone();
			_name = name;
			_version = version;
			_proc = processorArchitecture;
			_culture = culture;
		}

		public ApplicationId Copy()
		{
			return new ApplicationId(_token, _name, _version, _proc, _culture);
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			ApplicationId applicationId = o as ApplicationId;
			if (applicationId == null)
			{
				return false;
			}
			if (_name != applicationId._name)
			{
				return false;
			}
			if (_proc != applicationId._proc)
			{
				return false;
			}
			if (_culture != applicationId._culture)
			{
				return false;
			}
			if (!_version.Equals(applicationId._version))
			{
				return false;
			}
			if (_token.Length != applicationId._token.Length)
			{
				return false;
			}
			for (int i = 0; i < _token.Length; i++)
			{
				if (_token[i] != applicationId._token[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = _name.GetHashCode() ^ _version.GetHashCode();
			for (int i = 0; i < _token.Length; i++)
			{
				num ^= _token[i];
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(_name);
			if (_culture != null)
			{
				stringBuilder.AppendFormat(", culture=\"{0}\"", _culture);
			}
			stringBuilder.AppendFormat(", version=\"{0}\", publicKeyToken=\"", _version);
			for (int i = 0; i < _token.Length; i++)
			{
				stringBuilder.Append(_token[i].ToString("X2"));
			}
			if (_proc != null)
			{
				stringBuilder.AppendFormat("\", processorArchitecture =\"{0}\"", _proc);
			}
			else
			{
				stringBuilder.Append("\"");
			}
			return stringBuilder.ToString();
		}
	}
}
