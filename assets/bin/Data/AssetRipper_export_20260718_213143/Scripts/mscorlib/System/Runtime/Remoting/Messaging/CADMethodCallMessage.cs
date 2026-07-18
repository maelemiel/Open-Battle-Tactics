using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Channels;

namespace System.Runtime.Remoting.Messaging
{
	internal class CADMethodCallMessage : CADMessageBase
	{
		private string _uri;

		internal RuntimeMethodHandle MethodHandle;

		internal string FullTypeName;

		internal string Uri
		{
			get
			{
				return _uri;
			}
		}

		internal int PropertiesCount
		{
			get
			{
				return _propertyCount;
			}
		}

		internal CADMethodCallMessage(IMethodCallMessage callMsg)
		{
			_uri = callMsg.Uri;
			MethodHandle = callMsg.MethodBase.MethodHandle;
			FullTypeName = callMsg.MethodBase.DeclaringType.AssemblyQualifiedName;
			ArrayList args = null;
			_propertyCount = CADMessageBase.MarshalProperties(callMsg.Properties, ref args);
			_args = MarshalArguments(callMsg.Args, ref args);
			SaveLogicalCallContext(callMsg, ref args);
			if (args != null)
			{
				MemoryStream memoryStream = CADSerializer.SerializeObject(args.ToArray());
				_serializedArgs = memoryStream.GetBuffer();
			}
		}

		internal static CADMethodCallMessage Create(IMessage callMsg)
		{
			IMethodCallMessage methodCallMessage = callMsg as IMethodCallMessage;
			if (methodCallMessage == null)
			{
				return null;
			}
			return new CADMethodCallMessage(methodCallMessage);
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

		private static Type[] GetSignature(MethodBase methodBase, bool load)
		{
			ParameterInfo[] parameters = methodBase.GetParameters();
			Type[] array = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				if (load)
				{
					array[i] = Type.GetType(parameters[i].ParameterType.AssemblyQualifiedName, true);
				}
				else
				{
					array[i] = parameters[i].ParameterType;
				}
			}
			return array;
		}

		internal MethodBase GetMethod()
		{
			MethodBase methodBase = null;
			Type type = Type.GetType(FullTypeName);
			methodBase = ((!type.IsGenericType && !type.IsGenericTypeDefinition) ? MethodBase.GetMethodFromHandle(MethodHandle) : MethodBase.GetMethodFromHandleNoGenericCheck(MethodHandle));
			if (type != methodBase.DeclaringType)
			{
				Type[] signature = GetSignature(methodBase, true);
				if (methodBase.IsGenericMethod)
				{
					MethodBase[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					Type[] genericArguments = methodBase.GetGenericArguments();
					MethodBase[] array = methods;
					foreach (MethodBase methodBase2 in array)
					{
						if (!methodBase2.IsGenericMethod || methodBase2.Name != methodBase.Name)
						{
							continue;
						}
						Type[] genericArguments2 = methodBase2.GetGenericArguments();
						if (genericArguments.Length != genericArguments2.Length)
						{
							continue;
						}
						MethodInfo methodInfo = ((MethodInfo)methodBase2).MakeGenericMethod(genericArguments);
						Type[] signature2 = GetSignature(methodInfo, false);
						if (signature2.Length != signature.Length)
						{
							continue;
						}
						bool flag = false;
						for (int num = signature2.Length - 1; num >= 0; num--)
						{
							if (signature2[num] != signature[num])
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							return methodInfo;
						}
					}
					return methodBase;
				}
				MethodBase method = type.GetMethod(methodBase.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, signature, null);
				if (method == null)
				{
					throw new RemotingException(string.Concat("Method '", methodBase.Name, "' not found in type '", type, "'"));
				}
				return method;
			}
			return methodBase;
		}
	}
}
