using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	public sealed class ObfuscationAttribute : Attribute
	{
		private bool exclude;

		private bool strip;

		private bool applyToMembers;

		private string feature;

		public bool Exclude
		{
			get
			{
				return exclude;
			}
			set
			{
				exclude = value;
			}
		}

		public bool StripAfterObfuscation
		{
			get
			{
				return strip;
			}
			set
			{
				strip = value;
			}
		}

		public bool ApplyToMembers
		{
			get
			{
				return applyToMembers;
			}
			set
			{
				applyToMembers = value;
			}
		}

		public string Feature
		{
			get
			{
				return feature;
			}
			set
			{
				feature = value;
			}
		}

		public ObfuscationAttribute()
		{
			exclude = true;
			strip = true;
			applyToMembers = true;
			feature = "all";
		}
	}
}
