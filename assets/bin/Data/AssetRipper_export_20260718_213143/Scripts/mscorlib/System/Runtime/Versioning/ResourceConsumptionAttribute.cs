using System.Diagnostics;

namespace System.Runtime.Versioning
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
	[Conditional("RESOURCE_ANNOTATION_WORK")]
	public sealed class ResourceConsumptionAttribute : Attribute
	{
		private ResourceScope resource;

		private ResourceScope consumption;

		public ResourceScope ConsumptionScope
		{
			get
			{
				return consumption;
			}
		}

		public ResourceScope ResourceScope
		{
			get
			{
				return resource;
			}
		}

		public ResourceConsumptionAttribute(ResourceScope resourceScope)
		{
			resource = resourceScope;
			consumption = resourceScope;
		}

		public ResourceConsumptionAttribute(ResourceScope resourceScope, ResourceScope consumptionScope)
		{
			resource = resourceScope;
			consumption = consumptionScope;
		}
	}
}
