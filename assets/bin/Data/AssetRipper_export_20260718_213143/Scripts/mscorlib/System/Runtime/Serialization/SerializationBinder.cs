using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[Serializable]
	[ComVisible(true)]
	public abstract class SerializationBinder
	{
		public abstract Type BindToType(string assemblyName, string typeName);
	}
}
