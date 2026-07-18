namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class ImportedFromTypeLibAttribute : Attribute
	{
		private string TlbFile;

		public string Value
		{
			get
			{
				return TlbFile;
			}
		}

		public ImportedFromTypeLibAttribute(string tlbFile)
		{
			TlbFile = tlbFile;
		}
	}
}
