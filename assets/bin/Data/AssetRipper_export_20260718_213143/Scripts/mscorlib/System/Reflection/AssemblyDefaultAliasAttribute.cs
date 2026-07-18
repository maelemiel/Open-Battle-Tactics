using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyDefaultAliasAttribute : Attribute
	{
		private string name;

		public string DefaultAlias
		{
			get
			{
				return name;
			}
		}

		public AssemblyDefaultAliasAttribute(string defaultAlias)
		{
			name = defaultAlias;
		}
	}
}
