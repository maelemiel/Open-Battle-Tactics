using System.Configuration.Assemblies;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	[ComVisible(true)]
	public sealed class AssemblyAlgorithmIdAttribute : Attribute
	{
		private uint id;

		[CLSCompliant(false)]
		public uint AlgorithmId
		{
			get
			{
				return id;
			}
		}

		public AssemblyAlgorithmIdAttribute(AssemblyHashAlgorithm algorithmId)
		{
			id = (uint)algorithmId;
		}

		[CLSCompliant(false)]
		public AssemblyAlgorithmIdAttribute(uint algorithmId)
		{
			id = algorithmId;
		}
	}
}
