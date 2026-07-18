using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyProductAttribute : Attribute
	{
		private string name;

		public string Product
		{
			get
			{
				return name;
			}
		}

		public AssemblyProductAttribute(string product)
		{
			name = product;
		}
	}
}
