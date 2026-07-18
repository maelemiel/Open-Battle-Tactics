using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyKeyFileAttribute : Attribute
	{
		private string name;

		public string KeyFile
		{
			get
			{
				return name;
			}
		}

		public AssemblyKeyFileAttribute(string keyFile)
		{
			name = keyFile;
		}
	}
}
