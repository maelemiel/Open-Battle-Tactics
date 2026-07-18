using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Serialization.Formatters
{
	[ComVisible(true)]
	public interface ISoapMessage
	{
		Header[] Headers { get; set; }

		string MethodName { get; set; }

		string[] ParamNames { get; set; }

		Type[] ParamTypes { get; set; }

		object[] ParamValues { get; set; }

		string XmlNameSpace { get; set; }
	}
}
