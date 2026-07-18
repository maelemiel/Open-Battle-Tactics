using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyTrademarkAttribute : Attribute
	{
		private string name;

		public string Trademark
		{
			get
			{
				return name;
			}
		}

		public AssemblyTrademarkAttribute(string trademark)
		{
			name = trademark;
		}
	}
}
