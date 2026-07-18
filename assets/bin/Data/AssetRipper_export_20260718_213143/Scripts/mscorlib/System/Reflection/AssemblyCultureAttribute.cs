using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyCultureAttribute : Attribute
	{
		private string name;

		public string Culture
		{
			get
			{
				return name;
			}
		}

		public AssemblyCultureAttribute(string culture)
		{
			name = culture;
		}
	}
}
