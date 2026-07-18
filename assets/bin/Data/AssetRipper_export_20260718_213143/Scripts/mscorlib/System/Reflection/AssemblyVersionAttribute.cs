using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyVersionAttribute : Attribute
	{
		private string name;

		public string Version
		{
			get
			{
				return name;
			}
		}

		public AssemblyVersionAttribute(string version)
		{
			name = version;
		}
	}
}
