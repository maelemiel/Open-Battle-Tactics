using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class OperatingSystem : ICloneable, ISerializable
	{
		private PlatformID _platform;

		private Version _version;

		private string _servicePack = string.Empty;

		public PlatformID Platform
		{
			get
			{
				return _platform;
			}
		}

		public Version Version
		{
			get
			{
				return _version;
			}
		}

		public string ServicePack
		{
			get
			{
				return _servicePack;
			}
		}

		public string VersionString
		{
			get
			{
				return ToString();
			}
		}

		public OperatingSystem(PlatformID platform, Version version)
		{
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			_platform = platform;
			_version = version;
		}

		public object Clone()
		{
			return new OperatingSystem(_platform, _version);
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("_platform", _platform);
			info.AddValue("_version", _version);
			info.AddValue("_servicePack", _servicePack);
		}

		public override string ToString()
		{
			string text;
			switch ((int)_platform)
			{
			case 2:
				text = "Microsoft Windows NT";
				break;
			case 0:
				text = "Microsoft Win32S";
				break;
			case 1:
				text = "Microsoft Windows 98";
				break;
			case 3:
				text = "Microsoft Windows CE";
				break;
			case 4:
			case 128:
				text = "Unix";
				break;
			case 5:
				text = "XBox";
				break;
			case 6:
				text = "OSX";
				break;
			default:
				text = Locale.GetText("<unknown>");
				break;
			}
			return text + " " + _version.ToString();
		}
	}
}
