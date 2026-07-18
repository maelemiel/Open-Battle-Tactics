using System.Diagnostics;

namespace System.Runtime.Versioning
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	[Conditional("RESOURCE_ANNOTATION_WORK")]
	public sealed class ResourceExposureAttribute : Attribute
	{
		private ResourceScope exposure;

		public ResourceScope ResourceExposureLevel
		{
			get
			{
				return exposure;
			}
		}

		public ResourceExposureAttribute(ResourceScope exposureLevel)
		{
			exposure = exposureLevel;
		}
	}
}
