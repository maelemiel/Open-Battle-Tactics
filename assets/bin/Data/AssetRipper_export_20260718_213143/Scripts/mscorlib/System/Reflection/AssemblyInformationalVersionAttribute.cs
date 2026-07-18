using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyInformationalVersionAttribute : Attribute
	{
		private string name;

		public string InformationalVersion
		{
			get
			{
				return name;
			}
		}

		public AssemblyInformationalVersionAttribute(string informationalVersion)
		{
			name = informationalVersion;
		}
	}
}
