using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyCompanyAttribute : Attribute
	{
		private string name;

		public string Company
		{
			get
			{
				return name;
			}
		}

		public AssemblyCompanyAttribute(string company)
		{
			name = company;
		}
	}
}
