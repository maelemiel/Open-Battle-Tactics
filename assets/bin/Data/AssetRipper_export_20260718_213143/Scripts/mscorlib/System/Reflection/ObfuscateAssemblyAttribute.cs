using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class ObfuscateAssemblyAttribute : Attribute
	{
		private bool is_private;

		private bool strip;

		public bool AssemblyIsPrivate
		{
			get
			{
				return is_private;
			}
		}

		public bool StripAfterObfuscation
		{
			get
			{
				return strip;
			}
			set
			{
				strip = value;
			}
		}

		public ObfuscateAssemblyAttribute(bool assemblyIsPrivate)
		{
			strip = true;
			is_private = assemblyIsPrivate;
		}
	}
}
