using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class WaitForSeconds : YieldInstruction
	{
		internal float m_Seconds;

		public WaitForSeconds(float seconds)
		{
			m_Seconds = seconds;
		}
	}
}
