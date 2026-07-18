using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyCopyrightAttribute : Attribute
	{
		private string name;

		public string Copyright
		{
			get
			{
				return name;
			}
		}

		public AssemblyCopyrightAttribute(string copyright)
		{
			name = copyright;
		}
	}
}
