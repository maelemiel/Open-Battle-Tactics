using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public sealed class Missing : ISerializable
	{
		public static readonly Missing Value = new Missing();

		internal Missing()
		{
		}

		[MonoTODO]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}
	}
}
