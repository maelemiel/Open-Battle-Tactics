using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	[ComVisible(true)]
	public sealed class IndexerNameAttribute : Attribute
	{
		public IndexerNameAttribute(string indexerName)
		{
		}
	}
}
