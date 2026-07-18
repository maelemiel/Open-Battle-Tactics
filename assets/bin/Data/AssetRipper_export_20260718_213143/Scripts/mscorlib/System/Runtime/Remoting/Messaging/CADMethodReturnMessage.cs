using System.Collections;
using System.IO;
using System.Runtime.Remoting.Channels;

namespace System.Runtime.Remoting.Messaging
{
	internal class CADMethodReturnMessage : CADMessageBase
	{
		private object _returnValue;

		private CADArgHolder _exception;

		internal int PropertiesCount
		{
			get
			{
				return _propertyCount;
			}
		}

		internal CADMethodReturnMessage(IMethodReturnMessage retMsg)
		{
			ArrayList args = null;
			_propertyCount = CADMessageBase.MarshalProperties(retMsg.Properties, ref args);
			_returnValue = MarshalArgument(retMsg.ReturnValue, ref args);
			_args = MarshalArguments(retMsg.Args, ref args);
			if (retMsg.Exception != null)
			{
				if (args == null)
				{
					args = new ArrayList();
				}
				_exception = new CADArgHolder(args.Count);
				args.Add(retMsg.Exception);
			}
			SaveLogicalCallContext(retMsg, ref args);
			if (args != null)
			{
				MemoryStream memoryStream = CADSerializer.SerializeObject(args.ToArray());
				_serializedArgs = memoryStream.GetBuffer();
			}
		}

		internal static CADMethodReturnMessage Create(IMessage callMsg)
		{
			IMethodReturnMessage methodReturnMessage = callMsg as IMethodReturnMessage;
			if (methodReturnMessage == null)
			{
				return null;
			}
			return new CADMethodReturnMessage(methodReturnMessage);
		}

		internal ArrayList GetArguments()
		{
			ArrayList result = null;
			if (_serializedArgs != null)
			{
				object[] c = (object[])CADSerializer.DeserializeObject(new MemoryStream(_serializedArgs));
				result = new ArrayList(c);
				_serializedArgs = null;
			}
			return result;
		}

		internal object[] GetArgs(ArrayList args)
		{
			return UnmarshalArguments(_args, args);
		}

		internal object GetReturnValue(ArrayList args)
		{
			return UnmarshalArgument(_returnValue, args);
		}

		internal Exception GetException(ArrayList args)
		{
			if (_exception == null)
			{
				return null;
			}
			return (Exception)args[_exception.index];
		}
	}
}
