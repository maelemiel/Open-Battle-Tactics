using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyConfigurationAttribute : Attribute
	{
		private string name;

		public string Configuration
		{
			get
			{
				return name;
			}
		}

		public AssemblyConfigurationAttribute(string configuration)
		{
			name = configuration;
		}
	}
}
