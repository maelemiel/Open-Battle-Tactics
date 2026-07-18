namespace System.Runtime.Versioning
{
	public static class VersioningHelper
	{
		private static int GetDomainId()
		{
			return 0;
		}

		private static int GetProcessId()
		{
			return 0;
		}

		private static string SafeName(string name, bool process, bool appdomain)
		{
			if (process && appdomain)
			{
				return name + "_" + GetProcessId() + "_" + GetDomainId();
			}
			if (process)
			{
				return name + "_" + GetProcessId();
			}
			if (appdomain)
			{
				return name + "_" + GetDomainId();
			}
			return name;
		}

		private static string ConvertFromMachine(string name, ResourceScope to, Type type)
		{
			switch (to)
			{
			case ResourceScope.Machine:
				return SafeName(name, false, false);
			case ResourceScope.Process:
				return SafeName(name, true, false);
			case ResourceScope.AppDomain:
				return SafeName(name, true, true);
			default:
				throw new ArgumentException("to");
			}
		}

		private static string ConvertFromProcess(string name, ResourceScope to, Type type)
		{
			if (to < ResourceScope.Process || to >= ResourceScope.Private)
			{
				throw new ArgumentException("to");
			}
			bool appdomain = (to & ResourceScope.AppDomain) == ResourceScope.AppDomain;
			return SafeName(name, false, appdomain);
		}

		private static string ConvertFromAppDomain(string name, ResourceScope to, Type type)
		{
			if (to < ResourceScope.AppDomain || to >= ResourceScope.Private)
			{
				throw new ArgumentException("to");
			}
			return SafeName(name, false, false);
		}

		[MonoTODO("process id is always 0")]
		public static string MakeVersionSafeName(string name, ResourceScope from, ResourceScope to)
		{
			return MakeVersionSafeName(name, from, to, null);
		}

		[MonoTODO("type?")]
		public static string MakeVersionSafeName(string name, ResourceScope from, ResourceScope to, Type type)
		{
			if ((from & ResourceScope.Private) != ResourceScope.None)
			{
				to &= ~(ResourceScope.Private | ResourceScope.Assembly);
			}
			else if ((from & ResourceScope.Assembly) != ResourceScope.None)
			{
				to &= ~ResourceScope.Assembly;
			}
			string name2 = ((name != null) ? name : string.Empty);
			switch (from)
			{
			case ResourceScope.Machine:
			case ResourceScope.Machine | ResourceScope.Private:
			case ResourceScope.Machine | ResourceScope.Assembly:
				return ConvertFromMachine(name2, to, type);
			case ResourceScope.Process:
			case ResourceScope.Process | ResourceScope.Private:
			case ResourceScope.Process | ResourceScope.Assembly:
				return ConvertFromProcess(name2, to, type);
			case ResourceScope.AppDomain:
			case ResourceScope.AppDomain | ResourceScope.Private:
			case ResourceScope.AppDomain | ResourceScope.Assembly:
				return ConvertFromAppDomain(name2, to, type);
			default:
				throw new ArgumentException("from");
			}
		}
	}
}
