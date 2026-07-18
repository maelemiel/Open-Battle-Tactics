using System.Collections;
using System.Reflection;

namespace System.Runtime.Serialization
{
	internal sealed class SerializationCallbacks
	{
		public delegate void CallbackHandler(StreamingContext context);

		private const BindingFlags DefaultBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private readonly ArrayList onSerializingList;

		private readonly ArrayList onSerializedList;

		private readonly ArrayList onDeserializingList;

		private readonly ArrayList onDeserializedList;

		private static Hashtable cache = new Hashtable();

		private static object cache_lock = new object();

		public bool HasSerializingCallbacks
		{
			get
			{
				return onSerializingList != null;
			}
		}

		public bool HasSerializedCallbacks
		{
			get
			{
				return onSerializedList != null;
			}
		}

		public bool HasDeserializingCallbacks
		{
			get
			{
				return onDeserializingList != null;
			}
		}

		public bool HasDeserializedCallbacks
		{
			get
			{
				return onDeserializedList != null;
			}
		}

		public SerializationCallbacks(Type type)
		{
			onSerializingList = GetMethodsByAttribute(type, typeof(OnSerializingAttribute));
			onSerializedList = GetMethodsByAttribute(type, typeof(OnSerializedAttribute));
			onDeserializingList = GetMethodsByAttribute(type, typeof(OnDeserializingAttribute));
			onDeserializedList = GetMethodsByAttribute(type, typeof(OnDeserializedAttribute));
		}

		private static ArrayList GetMethodsByAttribute(Type type, Type attr)
		{
			ArrayList arrayList = new ArrayList();
			for (Type type2 = type; type2 != typeof(object); type2 = type2.BaseType)
			{
				int num = 0;
				MethodInfo[] methods = type2.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.IsDefined(attr, false))
					{
						arrayList.Add(methodInfo);
						num++;
					}
				}
				if (num > 1)
				{
					throw new TypeLoadException(string.Format("Type '{0}' has more than one method with the following attribute: '{1}'.", type.AssemblyQualifiedName, attr.FullName));
				}
			}
			return (arrayList.Count != 0) ? arrayList : null;
		}

		private static void Invoke(ArrayList list, object target, StreamingContext context)
		{
			if (list == null)
			{
				return;
			}
			CallbackHandler callbackHandler = null;
			foreach (MethodInfo item in list)
			{
				callbackHandler = (CallbackHandler)Delegate.Combine(Delegate.CreateDelegate(typeof(CallbackHandler), target, item), callbackHandler);
			}
			callbackHandler(context);
		}

		public void RaiseOnSerializing(object target, StreamingContext contex)
		{
			Invoke(onSerializingList, target, contex);
		}

		public void RaiseOnSerialized(object target, StreamingContext contex)
		{
			Invoke(onSerializedList, target, contex);
		}

		public void RaiseOnDeserializing(object target, StreamingContext contex)
		{
			Invoke(onDeserializingList, target, contex);
		}

		public void RaiseOnDeserialized(object target, StreamingContext contex)
		{
			Invoke(onDeserializedList, target, contex);
		}

		public static SerializationCallbacks GetSerializationCallbacks(Type t)
		{
			SerializationCallbacks serializationCallbacks = (SerializationCallbacks)cache[t];
			if (serializationCallbacks != null)
			{
				return serializationCallbacks;
			}
			lock (cache_lock)
			{
				serializationCallbacks = (SerializationCallbacks)cache[t];
				if (serializationCallbacks == null)
				{
					Hashtable hashtable = (Hashtable)cache.Clone();
					serializationCallbacks = (SerializationCallbacks)(hashtable[t] = new SerializationCallbacks(t));
					cache = hashtable;
				}
				return serializationCallbacks;
			}
		}
	}
}
