using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	[CLSCompliant(false)]
	[ComVisible(true)]
	public class ConstructionResponse : MethodResponse, IConstructionReturnMessage, IMessage, IMethodMessage, IMethodReturnMessage
	{
		public override IDictionary Properties
		{
			get
			{
				return base.Properties;
			}
		}

		public ConstructionResponse(Header[] h, IMethodCallMessage mcm)
			: base(h, mcm)
		{
		}

		internal ConstructionResponse(object resultObject, LogicalCallContext callCtx, IMethodCallMessage msg)
			: base(resultObject, null, callCtx, msg)
		{
		}

		internal ConstructionResponse(Exception e, IMethodCallMessage msg)
			: base(e, msg)
		{
		}

		internal ConstructionResponse(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
