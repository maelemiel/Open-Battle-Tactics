using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyFileVersionAttribute : Attribute
	{
		private string name;

		public string Version
		{
			get
			{
				return name;
			}
		}

		public AssemblyFileVersionAttribute(string version)
		{
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			name = version;
		}
	}
}
