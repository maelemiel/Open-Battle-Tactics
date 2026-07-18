namespace System.Runtime.Serialization.Formatters.Binary
{
	internal abstract class ClrTypeMetadata : TypeMetadata
	{
		public Type InstanceType;

		public override bool RequiresTypes
		{
			get
			{
				return false;
			}
		}

		public ClrTypeMetadata(Type instanceType)
		{
			InstanceType = instanceType;
			InstanceTypeName = instanceType.FullName;
			TypeAssemblyName = instanceType.Assembly.FullName;
		}
	}
}
