namespace System.Runtime.ConstrainedExecution
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Interface, Inherited = false)]
	public sealed class ReliabilityContractAttribute : Attribute
	{
		private Consistency consistency;

		private Cer cer;

		public Cer Cer
		{
			get
			{
				return cer;
			}
		}

		public Consistency ConsistencyGuarantee
		{
			get
			{
				return consistency;
			}
		}

		public ReliabilityContractAttribute(Consistency consistencyGuarantee, Cer cer)
		{
			consistency = consistencyGuarantee;
			this.cer = cer;
		}
	}
}
