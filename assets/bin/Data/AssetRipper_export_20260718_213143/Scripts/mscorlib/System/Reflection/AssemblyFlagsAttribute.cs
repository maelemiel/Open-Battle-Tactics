using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyFlagsAttribute : Attribute
	{
		private uint flags;

		[Obsolete("")]
		[CLSCompliant(false)]
		public uint Flags
		{
			get
			{
				return flags;
			}
		}

		public int AssemblyFlags
		{
			get
			{
				return (int)flags;
			}
		}

		[CLSCompliant(false)]
		[Obsolete("")]
		public AssemblyFlagsAttribute(uint flags)
		{
			this.flags = flags;
		}

		[Obsolete("")]
		public AssemblyFlagsAttribute(int assemblyFlags)
		{
			flags = (uint)assemblyFlags;
		}

		public AssemblyFlagsAttribute(AssemblyNameFlags assemblyFlags)
		{
			flags = (uint)assemblyFlags;
		}
	}
}
