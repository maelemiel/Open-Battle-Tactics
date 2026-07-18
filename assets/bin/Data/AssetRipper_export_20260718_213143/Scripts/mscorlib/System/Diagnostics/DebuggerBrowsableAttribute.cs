using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class DebuggerBrowsableAttribute : Attribute
	{
		private DebuggerBrowsableState state;

		public DebuggerBrowsableState State
		{
			get
			{
				return state;
			}
		}

		public DebuggerBrowsableAttribute(DebuggerBrowsableState state)
		{
			this.state = state;
		}
	}
}
