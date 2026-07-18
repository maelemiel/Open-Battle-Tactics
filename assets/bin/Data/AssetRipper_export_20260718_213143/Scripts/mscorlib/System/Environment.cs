using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;

namespace System
{
	[ComVisible(true)]
	public static class Environment
	{
		[ComVisible(true)]
		public enum SpecialFolder
		{
			MyDocuments = 5,
			Desktop = 0,
			MyComputer = 17,
			Programs = 2,
			Personal = 5,
			Favorites = 6,
			Startup = 7,
			Recent = 8,
			SendTo = 9,
			StartMenu = 11,
			MyMusic = 13,
			DesktopDirectory = 16,
			Templates = 21,
			ApplicationData = 26,
			LocalApplicationData = 28,
			InternetCache = 32,
			Cookies = 33,
			History = 34,
			CommonApplicationData = 35,
			System = 37,
			ProgramFiles = 38,
			MyPictures = 39,
			CommonProgramFiles = 43
		}

		private const int mono_corlib_version = 82;

		private static OperatingSystem os;

		public static string CommandLine
		{
			get
			{
				return string.Join(" ", GetCommandLineArgs());
			}
		}

		public static string CurrentDirectory
		{
			get
			{
				return Directory.GetCurrentDirectory();
			}
			set
			{
				Directory.SetCurrentDirectory(value);
			}
		}

		public static extern int ExitCode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static extern bool HasShutdownStarted
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string EmbeddingHostName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern bool SocketSecurityEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static bool UnityWebSecurityEnabled
		{
			get
			{
				return SocketSecurityEnabled;
			}
		}

		public static extern string MachineName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern string NewLine
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static extern PlatformID Platform
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static OperatingSystem OSVersion
		{
			get
			{
				if (os == null)
				{
					Version version = Version.CreateFromString(GetOSVersionString());
					PlatformID platform = Platform;
					os = new OperatingSystem(platform, version);
				}
				return os;
			}
		}

		public static string StackTrace
		{
			get
			{
				StackTrace stackTrace = new StackTrace(0, true);
				return stackTrace.ToString();
			}
		}

		public static extern int TickCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static string UserDomainName
		{
			get
			{
				return MachineName;
			}
		}

		[MonoTODO("Currently always returns false, regardless of interactive state")]
		public static bool UserInteractive
		{
			get
			{
				return false;
			}
		}

		public static extern string UserName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static Version Version
		{
			get
			{
				return new Version("3.0.40818.0");
			}
		}

		[MonoTODO("Currently always returns zero")]
		public static long WorkingSet
		{
			get
			{
				return 0L;
			}
		}

		public static extern int ProcessorCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static bool IsRunningOnWindows
		{
			get
			{
				return Platform < PlatformID.Unix;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetOSVersionString();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Exit(int exitCode);

		public static string ExpandEnvironmentVariables(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			int num = name.IndexOf('%');
			if (num == -1)
			{
				return name;
			}
			int length = name.Length;
			int num2 = 0;
			if (num == length - 1 || (num2 = name.IndexOf('%', num + 1)) == -1)
			{
				return name;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(name, 0, num);
			Hashtable hashtable = null;
			do
			{
				string text = name.Substring(num + 1, num2 - num - 1);
				string text2 = GetEnvironmentVariable(text);
				if (text2 == null && IsRunningOnWindows)
				{
					if (hashtable == null)
					{
						hashtable = GetEnvironmentVariablesNoCase();
					}
					text2 = hashtable[text] as string;
				}
				if (text2 == null)
				{
					stringBuilder.Append('%');
					stringBuilder.Append(text);
					num2--;
				}
				else
				{
					stringBuilder.Append(text2);
				}
				int num3 = num2;
				num = name.IndexOf('%', num2 + 1);
				num2 = ((num != -1 && num2 <= length - 1) ? name.IndexOf('%', num + 1) : (-1));
				int count = ((num == -1 || num2 == -1) ? (length - num3 - 1) : ((text2 == null) ? (num - num3) : (num - num3 - 1)));
				if (num >= num3 || num == -1)
				{
					stringBuilder.Append(name, num3 + 1, count);
				}
			}
			while (num2 > -1 && num2 < length);
			return stringBuilder.ToString();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetCommandLineArgs();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string internalGetEnvironmentVariable(string variable);

		public static string GetEnvironmentVariable(string variable)
		{
			return internalGetEnvironmentVariable(variable);
		}

		private static Hashtable GetEnvironmentVariablesNoCase()
		{
			Hashtable hashtable = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			string[] environmentVariableNames = GetEnvironmentVariableNames();
			foreach (string text in environmentVariableNames)
			{
				hashtable[text] = internalGetEnvironmentVariable(text);
			}
			return hashtable;
		}

		public static IDictionary GetEnvironmentVariables()
		{
			Hashtable hashtable = new Hashtable();
			string[] environmentVariableNames = GetEnvironmentVariableNames();
			foreach (string text in environmentVariableNames)
			{
				hashtable[text] = internalGetEnvironmentVariable(text);
			}
			return hashtable;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetWindowsFolderPath(int folder);

		public static string GetFolderPath(SpecialFolder folder)
		{
			string text = null;
			if (IsRunningOnWindows)
			{
				return GetWindowsFolderPath((int)folder);
			}
			return InternalGetFolderPath(folder);
		}

		private static string ReadXdgUserDir(string config_dir, string home_dir, string key, string fallback)
		{
			string text = internalGetEnvironmentVariable(key);
			if (text != null && text != string.Empty)
			{
				return text;
			}
			string path = Path.Combine(config_dir, "user-dirs.dirs");
			if (!File.Exists(path))
			{
				return Path.Combine(home_dir, fallback);
			}
			try
			{
				using (StreamReader streamReader = new StreamReader(path))
				{
					string text2;
					while ((text2 = streamReader.ReadLine()) != null)
					{
						text2 = text2.Trim();
						int num = text2.IndexOf('=');
						if (num > 8 && text2.Substring(0, num) == key)
						{
							string text3 = text2.Substring(num + 1).Trim('"');
							bool flag = false;
							if (text3.StartsWith("$HOME/"))
							{
								flag = true;
								text3 = text3.Substring(6);
							}
							else if (!text3.StartsWith("/"))
							{
								flag = true;
							}
							return (!flag) ? text3 : Path.Combine(home_dir, text3);
						}
					}
				}
			}
			catch (FileNotFoundException)
			{
			}
			return Path.Combine(home_dir, fallback);
		}

		internal static string InternalGetFolderPath(SpecialFolder folder)
		{
			string text = internalGetHome();
			string text2 = internalGetEnvironmentVariable("XDG_DATA_HOME");
			if (text2 == null || text2 == string.Empty)
			{
				text2 = Path.Combine(text, ".local");
				text2 = Path.Combine(text2, "share");
			}
			string text3 = internalGetEnvironmentVariable("XDG_CONFIG_HOME");
			if (text3 == null || text3 == string.Empty)
			{
				text3 = Path.Combine(text, ".config");
			}
			switch (folder)
			{
			case SpecialFolder.MyComputer:
				return string.Empty;
			case SpecialFolder.MyDocuments:
				return Path.Combine(text, "Documents");
			case SpecialFolder.ApplicationData:
				return text3;
			case SpecialFolder.LocalApplicationData:
				return text2;
			case SpecialFolder.Desktop:
			case SpecialFolder.DesktopDirectory:
				return ReadXdgUserDir(text3, text, "XDG_DESKTOP_DIR", "Desktop");
			case SpecialFolder.MyMusic:
				return ReadXdgUserDir(text3, text, "XDG_MUSIC_DIR", "Music");
			case SpecialFolder.MyPictures:
				return ReadXdgUserDir(text3, text, "XDG_PICTURES_DIR", "Pictures");
			case SpecialFolder.Programs:
			case SpecialFolder.Favorites:
			case SpecialFolder.Startup:
			case SpecialFolder.Recent:
			case SpecialFolder.SendTo:
			case SpecialFolder.StartMenu:
			case SpecialFolder.Templates:
			case SpecialFolder.InternetCache:
			case SpecialFolder.Cookies:
			case SpecialFolder.History:
			case SpecialFolder.System:
			case SpecialFolder.ProgramFiles:
			case SpecialFolder.CommonProgramFiles:
				return string.Empty;
			case SpecialFolder.CommonApplicationData:
				return "/usr/share";
			default:
				throw new ArgumentException("Invalid SpecialFolder");
			}
		}

		public static string[] GetLogicalDrives()
		{
			return GetLogicalDrivesInternal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void internalBroadcastSettingChange();

		public static string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)
		{
			switch (target)
			{
			case EnvironmentVariableTarget.Process:
				return GetEnvironmentVariable(variable);
			case EnvironmentVariableTarget.Machine:
			{
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				if (!IsRunningOnWindows)
				{
					return null;
				}
				using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment"))
				{
					object value2 = registryKey2.GetValue(variable);
					return (value2 != null) ? value2.ToString() : null;
				}
			}
			case EnvironmentVariableTarget.User:
			{
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				if (!IsRunningOnWindows)
				{
					return null;
				}
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Environment", false))
				{
					object value = registryKey.GetValue(variable);
					return (value != null) ? value.ToString() : null;
				}
			}
			default:
				throw new ArgumentException("target");
			}
		}

		public static IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target)
		{
			IDictionary dictionary = new Hashtable();
			switch (target)
			{
			case EnvironmentVariableTarget.Process:
				dictionary = GetEnvironmentVariables();
				break;
			case EnvironmentVariableTarget.Machine:
			{
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				if (!IsRunningOnWindows)
				{
					break;
				}
				using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment"))
				{
					string[] valueNames2 = registryKey2.GetValueNames();
					string[] array2 = valueNames2;
					foreach (string text2 in array2)
					{
						dictionary.Add(text2, registryKey2.GetValue(text2));
					}
				}
				break;
			}
			case EnvironmentVariableTarget.User:
			{
				new EnvironmentPermission(PermissionState.Unrestricted).Demand();
				if (!IsRunningOnWindows)
				{
					break;
				}
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Environment"))
				{
					string[] valueNames = registryKey.GetValueNames();
					string[] array = valueNames;
					foreach (string text in array)
					{
						dictionary.Add(text, registryKey.GetValue(text));
					}
				}
				break;
			}
			default:
				throw new ArgumentException("target");
			}
			return dictionary;
		}

		public static void SetEnvironmentVariable(string variable, string value)
		{
			SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process);
		}

		public static void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target)
		{
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}
			if (variable == string.Empty)
			{
				throw new ArgumentException("String cannot be of zero length.", "variable");
			}
			if (variable.IndexOf('=') != -1)
			{
				throw new ArgumentException("Environment variable name cannot contain an equal character.", "variable");
			}
			if (variable[0] == '\0')
			{
				throw new ArgumentException("The first char in the string is the null character.", "variable");
			}
			switch (target)
			{
			case EnvironmentVariableTarget.Process:
				InternalSetEnvironmentVariable(variable, value);
				break;
			case EnvironmentVariableTarget.Machine:
			{
				if (!IsRunningOnWindows)
				{
					break;
				}
				using (RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Environment", true))
				{
					if (string.IsNullOrEmpty(value))
					{
						registryKey2.DeleteValue(variable, false);
					}
					else
					{
						registryKey2.SetValue(variable, value);
					}
					internalBroadcastSettingChange();
					break;
				}
			}
			case EnvironmentVariableTarget.User:
			{
				if (!IsRunningOnWindows)
				{
					break;
				}
				using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Environment", true))
				{
					if (string.IsNullOrEmpty(value))
					{
						registryKey.DeleteValue(variable, false);
					}
					else
					{
						registryKey.SetValue(variable, value);
					}
					internalBroadcastSettingChange();
					break;
				}
			}
			default:
				throw new ArgumentException("target");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalSetEnvironmentVariable(string variable, string value);

		[MonoTODO("Not implemented")]
		public static void FailFast(string message)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetLogicalDrivesInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string[] GetEnvironmentVariableNames();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string GetMachineConfigPath();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string internalGetHome();
	}
}
