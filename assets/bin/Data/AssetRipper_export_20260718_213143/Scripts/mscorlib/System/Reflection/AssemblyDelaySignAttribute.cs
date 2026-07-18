using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyDelaySignAttribute : Attribute
	{
		private bool delay;

		public bool DelaySign
		{
			get
			{
				return delay;
			}
		}

		public AssemblyDelaySignAttribute(bool delaySign)
		{
			delay = delaySign;
		}
	}
}
