using System.Runtime.InteropServices;

namespace System
{
	[AttributeUsage(AttributeTargets.Method)]
	[ComVisible(true)]
	public sealed class LoaderOptimizationAttribute : Attribute
	{
		private LoaderOptimization lo;

		public LoaderOptimization Value
		{
			get
			{
				return lo;
			}
		}

		public LoaderOptimizationAttribute(byte value)
		{
			lo = (LoaderOptimization)value;
		}

		public LoaderOptimizationAttribute(LoaderOptimization value)
		{
			lo = value;
		}
	}
}
