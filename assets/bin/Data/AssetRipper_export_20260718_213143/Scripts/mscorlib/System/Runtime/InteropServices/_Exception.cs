using System.Reflection;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[CLSCompliant(false)]
	[Guid("b36b5c63-42ef-38bc-a07e-0b34c98f164a")]
	[InterfaceType(ComInterfaceType.InterfaceIsDual)]
	public interface _Exception
	{
		string HelpLink { get; set; }

		Exception InnerException { get; }

		string Message { get; }

		string Source { get; set; }

		string StackTrace { get; }

		MethodBase TargetSite { get; }

		new bool Equals(object obj);

		Exception GetBaseException();

		new int GetHashCode();

		void GetObjectData(SerializationInfo info, StreamingContext context);

		new Type GetType();

		new string ToString();
	}
}
