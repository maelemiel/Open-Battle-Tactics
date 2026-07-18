using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	public class RuntimeEnvironment
	{
		public static string SystemConfigurationFile
		{
			get
			{
				string machineConfigPath = Environment.GetMachineConfigPath();
				if (SecurityManager.SecurityEnabled)
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, machineConfigPath).Demand();
				}
				return machineConfigPath;
			}
		}

		public static bool FromGlobalAccessCache(Assembly a)
		{
			return a.GlobalAssemblyCache;
		}

		public static string GetRuntimeDirectory()
		{
			return Path.GetDirectoryName(typeof(int).Assembly.Location);
		}

		public static string GetSystemVersion()
		{
			return "v" + Environment.Version.Major + "." + Environment.Version.Minor + "." + Environment.Version.Build;
		}
	}
}
