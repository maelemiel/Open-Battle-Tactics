using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyDescriptionAttribute : Attribute
	{
		private string name;

		public string Description
		{
			get
			{
				return name;
			}
		}

		public AssemblyDescriptionAttribute(string description)
		{
			name = description;
		}
	}
}
