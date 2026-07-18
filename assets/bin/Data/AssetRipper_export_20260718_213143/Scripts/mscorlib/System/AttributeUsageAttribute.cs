using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(true)]
	public sealed class AttributeUsageAttribute : Attribute
	{
		private AttributeTargets valid_on;

		private bool allow_multiple;

		private bool inherited = true;

		public bool AllowMultiple
		{
			get
			{
				return allow_multiple;
			}
			set
			{
				allow_multiple = value;
			}
		}

		public bool Inherited
		{
			get
			{
				return inherited;
			}
			set
			{
				inherited = value;
			}
		}

		public AttributeTargets ValidOn
		{
			get
			{
				return valid_on;
			}
		}

		public AttributeUsageAttribute(AttributeTargets validOn)
		{
			valid_on = validOn;
		}
	}
}
