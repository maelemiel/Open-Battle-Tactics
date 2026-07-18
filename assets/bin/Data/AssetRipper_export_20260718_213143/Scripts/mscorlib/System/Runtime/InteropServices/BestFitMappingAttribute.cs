namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
	[ComVisible(true)]
	public sealed class BestFitMappingAttribute : Attribute
	{
		private bool bfm;

		public bool ThrowOnUnmappableChar;

		public bool BestFitMapping
		{
			get
			{
				return bfm;
			}
		}

		public BestFitMappingAttribute(bool BestFitMapping)
		{
			bfm = BestFitMapping;
		}
	}
}
