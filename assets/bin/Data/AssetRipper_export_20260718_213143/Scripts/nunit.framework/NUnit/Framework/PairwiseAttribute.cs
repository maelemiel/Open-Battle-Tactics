using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class PairwiseAttribute : PropertyAttribute
	{
		public PairwiseAttribute()
			: base("_JOINTYPE", "Pairwise")
		{
		}
	}
}
