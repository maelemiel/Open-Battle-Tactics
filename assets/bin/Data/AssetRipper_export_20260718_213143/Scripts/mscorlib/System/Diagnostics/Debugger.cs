using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	[MonoTODO("The Debugger class is not functional")]
	public sealed class Debugger
	{
		public static readonly string DefaultCategory = string.Empty;

		public static bool IsAttached
		{
			get
			{
				return IsAttached_internal();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAttached_internal();

		public static void Break()
		{
		}

		public static bool IsLogging()
		{
			return false;
		}

		[MonoTODO("Not implemented")]
		public static bool Launch()
		{
			throw new NotImplementedException();
		}

		public static void Log(int level, string category, string message)
		{
		}
	}
}
