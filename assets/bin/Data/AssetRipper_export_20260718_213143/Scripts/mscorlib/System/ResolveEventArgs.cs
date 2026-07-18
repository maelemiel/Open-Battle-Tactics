using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	public class ResolveEventArgs : EventArgs
	{
		private string m_Name;

		public string Name
		{
			get
			{
				return m_Name;
			}
		}

		public ResolveEventArgs(string name)
		{
			m_Name = name;
		}
	}
}
