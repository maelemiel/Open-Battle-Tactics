using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface IObjectReference
	{
		object GetRealObject(StreamingContext context);
	}
}
