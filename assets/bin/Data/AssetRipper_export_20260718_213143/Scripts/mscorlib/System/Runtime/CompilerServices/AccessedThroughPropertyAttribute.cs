using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Field)]
	[ComVisible(true)]
	public sealed class AccessedThroughPropertyAttribute : Attribute
	{
		private string name;

		public string PropertyName
		{
			get
			{
				return name;
			}
		}

		public AccessedThroughPropertyAttribute(string propertyName)
		{
			name = propertyName;
		}
	}
}
