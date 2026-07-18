using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.All)]
	public sealed class CLSCompliantAttribute : Attribute
	{
		private bool is_compliant;

		public bool IsCompliant
		{
			get
			{
				return is_compliant;
			}
		}

		public CLSCompliantAttribute(bool isCompliant)
		{
			is_compliant = isCompliant;
		}
	}
}
