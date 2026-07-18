using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class Version : IComparable, ICloneable, IComparable<Version>, IEquatable<Version>
	{
		private const int UNDEFINED = -1;

		private int _Major;

		private int _Minor;

		private int _Build;

		private int _Revision;

		public int Build
		{
			get
			{
				return _Build;
			}
		}

		public int Major
		{
			get
			{
				return _Major;
			}
		}

		public int Minor
		{
			get
			{
				return _Minor;
			}
		}

		public int Revision
		{
			get
			{
				return _Revision;
			}
		}

		public short MajorRevision
		{
			get
			{
				return (short)(_Revision >> 16);
			}
		}

		public short MinorRevision
		{
			get
			{
				return (short)_Revision;
			}
		}

		public Version()
		{
			CheckedSet(2, 0, 0, -1, -1);
		}

		public Version(string version)
		{
			int major = -1;
			int minor = -1;
			int build = -1;
			int revision = -1;
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			string[] array = version.Split('.');
			int num = array.Length;
			if (num < 2 || num > 4)
			{
				throw new ArgumentException(Locale.GetText("There must be 2, 3 or 4 components in the version string."));
			}
			if (num > 0)
			{
				major = int.Parse(array[0]);
			}
			if (num > 1)
			{
				minor = int.Parse(array[1]);
			}
			if (num > 2)
			{
				build = int.Parse(array[2]);
			}
			if (num > 3)
			{
				revision = int.Parse(array[3]);
			}
			CheckedSet(num, major, minor, build, revision);
		}

		public Version(int major, int minor)
		{
			CheckedSet(2, major, minor, 0, 0);
		}

		public Version(int major, int minor, int build)
		{
			CheckedSet(3, major, minor, build, 0);
		}

		public Version(int major, int minor, int build, int revision)
		{
			CheckedSet(4, major, minor, build, revision);
		}

		private void CheckedSet(int defined, int major, int minor, int build, int revision)
		{
			if (major < 0)
			{
				throw new ArgumentOutOfRangeException("major");
			}
			_Major = major;
			if (minor < 0)
			{
				throw new ArgumentOutOfRangeException("minor");
			}
			_Minor = minor;
			if (defined == 2)
			{
				_Build = -1;
				_Revision = -1;
				return;
			}
			if (build < 0)
			{
				throw new ArgumentOutOfRangeException("build");
			}
			_Build = build;
			if (defined == 3)
			{
				_Revision = -1;
				return;
			}
			if (revision < 0)
			{
				throw new ArgumentOutOfRangeException("revision");
			}
			_Revision = revision;
		}

		public object Clone()
		{
			if (_Build == -1)
			{
				return new Version(_Major, _Minor);
			}
			if (_Revision == -1)
			{
				return new Version(_Major, _Minor, _Build);
			}
			return new Version(_Major, _Minor, _Build, _Revision);
		}

		public int CompareTo(object version)
		{
			if (version == null)
			{
				return 1;
			}
			if (!(version is Version))
			{
				throw new ArgumentException(Locale.GetText("Argument to Version.CompareTo must be a Version."));
			}
			return CompareTo((Version)version);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Version);
		}

		public int CompareTo(Version value)
		{
			if (value == null)
			{
				return 1;
			}
			if (_Major > value._Major)
			{
				return 1;
			}
			if (_Major < value._Major)
			{
				return -1;
			}
			if (_Minor > value._Minor)
			{
				return 1;
			}
			if (_Minor < value._Minor)
			{
				return -1;
			}
			if (_Build > value._Build)
			{
				return 1;
			}
			if (_Build < value._Build)
			{
				return -1;
			}
			if (_Revision > value._Revision)
			{
				return 1;
			}
			if (_Revision < value._Revision)
			{
				return -1;
			}
			return 0;
		}

		public bool Equals(Version obj)
		{
			return obj != null && obj._Major == _Major && obj._Minor == _Minor && obj._Build == _Build && obj._Revision == _Revision;
		}

		public override int GetHashCode()
		{
			return (_Revision << 24) | (_Build << 16) | (_Minor << 8) | _Major;
		}

		public override string ToString()
		{
			string text = _Major + "." + _Minor;
			if (_Build != -1)
			{
				text = text + "." + _Build;
			}
			if (_Revision != -1)
			{
				text = text + "." + _Revision;
			}
			return text;
		}

		public string ToString(int fieldCount)
		{
			switch (fieldCount)
			{
			case 0:
				return string.Empty;
			case 1:
				return _Major.ToString();
			case 2:
				return _Major + "." + _Minor;
			case 3:
				if (_Build == -1)
				{
					throw new ArgumentException(Locale.GetText("fieldCount is larger than the number of components defined in this instance."));
				}
				return _Major + "." + _Minor + "." + _Build;
			case 4:
				if (_Build == -1 || _Revision == -1)
				{
					throw new ArgumentException(Locale.GetText("fieldCount is larger than the number of components defined in this instance."));
				}
				return _Major + "." + _Minor + "." + _Build + "." + _Revision;
			default:
				throw new ArgumentException(Locale.GetText("Invalid fieldCount parameter: ") + fieldCount);
			}
		}

		internal static Version CreateFromString(string info)
		{
			int major = 0;
			int minor = 0;
			int build = 0;
			int revision = 0;
			int num = 1;
			int num2 = -1;
			if (info == null)
			{
				return new Version(0, 0, 0, 0);
			}
			foreach (char c in info)
			{
				if (char.IsDigit(c))
				{
					num2 = ((num2 >= 0) ? (num2 * 10 + (c - 48)) : (c - 48));
				}
				else if (num2 >= 0)
				{
					switch (num)
					{
					case 1:
						major = num2;
						break;
					case 2:
						minor = num2;
						break;
					case 3:
						build = num2;
						break;
					case 4:
						revision = num2;
						break;
					}
					num2 = -1;
					num++;
				}
				if (num == 5)
				{
					break;
				}
			}
			if (num2 >= 0)
			{
				switch (num)
				{
				case 1:
					major = num2;
					break;
				case 2:
					minor = num2;
					break;
				case 3:
					build = num2;
					break;
				case 4:
					revision = num2;
					break;
				}
			}
			return new Version(major, minor, build, revision);
		}

		public static bool operator ==(Version v1, Version v2)
		{
			return object.Equals(v1, v2);
		}

		public static bool operator !=(Version v1, Version v2)
		{
			return !object.Equals(v1, v2);
		}

		public static bool operator >(Version v1, Version v2)
		{
			return v1.CompareTo(v2) > 0;
		}

		public static bool operator >=(Version v1, Version v2)
		{
			return v1.CompareTo(v2) >= 0;
		}

		public static bool operator <(Version v1, Version v2)
		{
			return v1.CompareTo(v2) < 0;
		}

		public static bool operator <=(Version v1, Version v2)
		{
			return v1.CompareTo(v2) <= 0;
		}
	}
}
