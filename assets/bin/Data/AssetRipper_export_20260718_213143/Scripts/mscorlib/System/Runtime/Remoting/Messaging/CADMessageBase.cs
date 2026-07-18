using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace System.Runtime.Remoting.Messaging
{
	internal class CADMessageBase
	{
		protected object[] _args;

		protected byte[] _serializedArgs;

		protected int _propertyCount;

		protected CADArgHolder _callContext;

		internal static int MarshalProperties(IDictionary dict, ref ArrayList args)
		{
			IDictionary dictionary = dict;
			int num = 0;
			MethodDictionary methodDictionary = dict as MethodDictionary;
			if (methodDictionary != null)
			{
				if (methodDictionary.HasInternalProperties)
				{
					dictionary = methodDictionary.InternalProperties;
					if (dictionary != null)
					{
						foreach (DictionaryEntry item in dictionary)
						{
							if (args == null)
							{
								args = new ArrayList();
							}
							args.Add(item);
							num++;
						}
					}
				}
			}
			else if (dict != null)
			{
				foreach (DictionaryEntry item2 in dictionary)
				{
					if (args == null)
					{
						args = new ArrayList();
					}
					args.Add(item2);
					num++;
				}
			}
			return num;
		}

		internal static void UnmarshalProperties(IDictionary dict, int count, ArrayList args)
		{
			for (int i = 0; i < count; i++)
			{
				DictionaryEntry dictionaryEntry = (DictionaryEntry)args[i];
				dict[dictionaryEntry.Key] = dictionaryEntry.Value;
			}
		}

		private static bool IsPossibleToIgnoreMarshal(object obj)
		{
			Type type = obj.GetType();
			if (type.IsPrimitive || type == typeof(void))
			{
				return true;
			}
			if (type.IsArray && type.GetElementType().IsPrimitive && ((Array)obj).Rank == 1)
			{
				return true;
			}
			if (obj is string || obj is DateTime || obj is TimeSpan)
			{
				return true;
			}
			return false;
		}

		protected object MarshalArgument(object arg, ref ArrayList args)
		{
			if (arg == null)
			{
				return null;
			}
			if (IsPossibleToIgnoreMarshal(arg))
			{
				return arg;
			}
			MarshalByRefObject marshalByRefObject = arg as MarshalByRefObject;
			if (marshalByRefObject != null && !RemotingServices.IsTransparentProxy(marshalByRefObject))
			{
				ObjRef o = RemotingServices.Marshal(marshalByRefObject);
				return new CADObjRef(o, Thread.GetDomainID());
			}
			if (args == null)
			{
				args = new ArrayList();
			}
			args.Add(arg);
			return new CADArgHolder(args.Count - 1);
		}

		protected object UnmarshalArgument(object arg, ArrayList args)
		{
			if (arg == null)
			{
				return null;
			}
			CADArgHolder cADArgHolder = arg as CADArgHolder;
			if (cADArgHolder != null)
			{
				return args[cADArgHolder.index];
			}
			CADObjRef cADObjRef = arg as CADObjRef;
			if (cADObjRef != null)
			{
				string typeName = string.Copy(cADObjRef.TypeName);
				string uri = string.Copy(cADObjRef.URI);
				int sourceDomain = cADObjRef.SourceDomain;
				ChannelInfo cinfo = new ChannelInfo(new CrossAppDomainData(sourceDomain));
				ObjRef objectRef = new ObjRef(typeName, uri, cinfo);
				return RemotingServices.Unmarshal(objectRef);
			}
			if (arg is Array)
			{
				Array array = (Array)arg;
				Array array2;
				switch (Type.GetTypeCode(arg.GetType().GetElementType()))
				{
				case TypeCode.Boolean:
					array2 = new bool[array.Length];
					break;
				case TypeCode.Byte:
					array2 = new byte[array.Length];
					break;
				case TypeCode.Char:
					array2 = new char[array.Length];
					break;
				case TypeCode.Decimal:
					array2 = new decimal[array.Length];
					break;
				case TypeCode.Double:
					array2 = new double[array.Length];
					break;
				case TypeCode.Int16:
					array2 = new short[array.Length];
					break;
				case TypeCode.Int32:
					array2 = new int[array.Length];
					break;
				case TypeCode.Int64:
					array2 = new long[array.Length];
					break;
				case TypeCode.SByte:
					array2 = new sbyte[array.Length];
					break;
				case TypeCode.Single:
					array2 = new float[array.Length];
					break;
				case TypeCode.UInt16:
					array2 = new ushort[array.Length];
					break;
				case TypeCode.UInt32:
					array2 = new uint[array.Length];
					break;
				case TypeCode.UInt64:
					array2 = new ulong[array.Length];
					break;
				default:
					throw new NotSupportedException();
				}
				array.CopyTo(array2, 0);
				return array2;
			}
			switch (Type.GetTypeCode(arg.GetType()))
			{
			case TypeCode.Boolean:
				return (bool)arg;
			case TypeCode.Byte:
				return (byte)arg;
			case TypeCode.Char:
				return (char)arg;
			case TypeCode.Decimal:
				return (decimal)arg;
			case TypeCode.Double:
				return (double)arg;
			case TypeCode.Int16:
				return (short)arg;
			case TypeCode.Int32:
				return (int)arg;
			case TypeCode.Int64:
				return (long)arg;
			case TypeCode.SByte:
				return (sbyte)arg;
			case TypeCode.Single:
				return (float)arg;
			case TypeCode.UInt16:
				return (ushort)arg;
			case TypeCode.UInt32:
				return (uint)arg;
			case TypeCode.UInt64:
				return (ulong)arg;
			case TypeCode.String:
				return string.Copy((string)arg);
			case TypeCode.DateTime:
				return new DateTime(((DateTime)arg).Ticks);
			default:
				if (arg is TimeSpan)
				{
					return new TimeSpan(((TimeSpan)arg).Ticks);
				}
				if (arg is IntPtr)
				{
					return (IntPtr)arg;
				}
				throw new NotSupportedException(string.Concat("Parameter of type ", arg.GetType(), " cannot be unmarshalled"));
			}
		}

		internal object[] MarshalArguments(object[] arguments, ref ArrayList args)
		{
			object[] array = new object[arguments.Length];
			int num = arguments.Length;
			for (int i = 0; i < num; i++)
			{
				array[i] = MarshalArgument(arguments[i], ref args);
			}
			return array;
		}

		internal object[] UnmarshalArguments(object[] arguments, ArrayList args)
		{
			object[] array = new object[arguments.Length];
			int num = arguments.Length;
			for (int i = 0; i < num; i++)
			{
				array[i] = UnmarshalArgument(arguments[i], args);
			}
			return array;
		}

		protected void SaveLogicalCallContext(IMethodMessage msg, ref ArrayList serializeList)
		{
			if (msg.LogicalCallContext != null && msg.LogicalCallContext.HasInfo)
			{
				if (serializeList == null)
				{
					serializeList = new ArrayList();
				}
				_callContext = new CADArgHolder(serializeList.Count);
				serializeList.Add(msg.LogicalCallContext);
			}
		}

		internal LogicalCallContext GetLogicalCallContext(ArrayList args)
		{
			if (_callContext == null)
			{
				return null;
			}
			return (LogicalCallContext)args[_callContext.index];
		}
	}
}
