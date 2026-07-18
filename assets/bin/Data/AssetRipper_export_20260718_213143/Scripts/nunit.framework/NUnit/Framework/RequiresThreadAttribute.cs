using System;
using System.Threading;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RequiresThreadAttribute : PropertyAttribute
	{
		public RequiresThreadAttribute()
			: base(true)
		{
		}

		public RequiresThreadAttribute(ApartmentState apartment)
			: base(true)
		{
			base.Properties.Add("APARTMENT_STATE", apartment);
		}
	}
}
