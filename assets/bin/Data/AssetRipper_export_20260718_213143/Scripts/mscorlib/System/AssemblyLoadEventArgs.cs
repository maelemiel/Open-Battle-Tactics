using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public class AssemblyLoadEventArgs : EventArgs
	{
		private Assembly m_loadedAssembly;

		public Assembly LoadedAssembly
		{
			get
			{
				return m_loadedAssembly;
			}
		}

		public AssemblyLoadEventArgs(Assembly loadedAssembly)
		{
			m_loadedAssembly = loadedAssembly;
		}
	}
}
