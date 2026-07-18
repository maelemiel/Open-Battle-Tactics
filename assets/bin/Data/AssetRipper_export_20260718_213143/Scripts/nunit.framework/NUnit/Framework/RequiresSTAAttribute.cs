using System;
using System.Threading;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RequiresSTAAttribute : PropertyAttribute
	{
		public RequiresSTAAttribute()
		{
			base.Properties.Add("APARTMENT_STATE", ApartmentState.STA);
		}
	}
}
