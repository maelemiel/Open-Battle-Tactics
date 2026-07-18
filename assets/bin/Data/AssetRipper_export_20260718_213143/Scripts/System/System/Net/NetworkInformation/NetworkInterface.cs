using System.IO;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation
{
	public abstract class NetworkInterface
	{
		private static Version windowsVer51 = new Version(5, 1);

		internal static readonly bool runningOnUnix = Environment.OSVersion.Platform == PlatformID.Unix;

		[System.MonoTODO("Only works on Linux. Returns 0 on other systems.")]
		public static int LoopbackInterfaceIndex
		{
			get
			{
				if (runningOnUnix)
				{
					try
					{
						return System.Net.NetworkInformation.UnixNetworkInterface.IfNameToIndex("lo");
					}
					catch
					{
						return 0;
					}
				}
				return 0;
			}
		}

		public abstract string Description { get; }

		public abstract string Id { get; }

		public abstract bool IsReceiveOnly { get; }

		public abstract string Name { get; }

		public abstract NetworkInterfaceType NetworkInterfaceType { get; }

		public abstract OperationalStatus OperationalStatus { get; }

		public abstract long Speed { get; }

		public abstract bool SupportsMulticast { get; }

		[DllImport("libc")]
		private static extern int uname(IntPtr buf);

		[System.MonoTODO("Only works on Linux and Windows")]
		public static NetworkInterface[] GetAllNetworkInterfaces()
		{
			if (runningOnUnix)
			{
				bool flag = false;
				IntPtr intPtr = Marshal.AllocHGlobal(8192);
				if (uname(intPtr) == 0)
				{
					string text = Marshal.PtrToStringAnsi(intPtr);
					if (text == "Darwin")
					{
						flag = true;
					}
				}
				Marshal.FreeHGlobal(intPtr);
				try
				{
					if (flag)
					{
						return System.Net.NetworkInformation.MacOsNetworkInterface.ImplGetAllNetworkInterfaces();
					}
					return System.Net.NetworkInformation.LinuxNetworkInterface.ImplGetAllNetworkInterfaces();
				}
				catch (SystemException ex)
				{
					throw ex;
				}
				catch
				{
					return new NetworkInterface[0];
				}
			}
			if (Environment.OSVersion.Version >= windowsVer51)
			{
				return System.Net.NetworkInformation.Win32NetworkInterface2.ImplGetAllNetworkInterfaces();
			}
			return new NetworkInterface[0];
		}

		[System.MonoTODO("Always returns true")]
		public static bool GetIsNetworkAvailable()
		{
			return true;
		}

		internal static string ReadLine(string path)
		{
			using (FileStream stream = File.OpenRead(path))
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					return streamReader.ReadLine();
				}
			}
		}

		public abstract IPInterfaceProperties GetIPProperties();

		public abstract IPv4InterfaceStatistics GetIPv4Statistics();

		public abstract PhysicalAddress GetPhysicalAddress();

		public abstract bool Supports(NetworkInterfaceComponent networkInterfaceComponent);
	}
}
