using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	internal class DelegateSerializationHolder : ISerializable, IObjectReference
	{
		[Serializable]
		private class DelegateEntry
		{
			private string type;

			private string assembly;

			public object target;

			private string targetTypeAssembly;

			private string targetTypeName;

			private string methodName;

			public DelegateEntry delegateEntry;

			public DelegateEntry(Delegate del, string targetLabel)
			{
				type = del.GetType().FullName;
				assembly = del.GetType().Assembly.FullName;
				target = targetLabel;
				targetTypeAssembly = del.Method.DeclaringType.Assembly.FullName;
				targetTypeName = del.Method.DeclaringType.FullName;
				methodName = del.Method.Name;
			}

			public Delegate DeserializeDelegate(SerializationInfo info)
			{
				object obj = null;
				if (target != null)
				{
					obj = info.GetValue(target.ToString(), typeof(object));
				}
				Assembly assembly = Assembly.Load(this.assembly);
				Type type = assembly.GetType(this.type);
				if (obj != null)
				{
					if (RemotingServices.IsTransparentProxy(obj))
					{
						Assembly assembly2 = Assembly.Load(targetTypeAssembly);
						Type type2 = assembly2.GetType(targetTypeName);
						if (!type2.IsInstanceOfType(obj))
						{
							throw new RemotingException("Unexpected proxy type.");
						}
					}
					return Delegate.CreateDelegate(type, obj, methodName);
				}
				Assembly assembly3 = Assembly.Load(targetTypeAssembly);
				Type type3 = assembly3.GetType(targetTypeName);
				return Delegate.CreateDelegate(type, type3, methodName);
			}
		}

		private Delegate _delegate;

		private DelegateSerializationHolder(SerializationInfo info, StreamingContext ctx)
		{
			DelegateEntry delegateEntry = (DelegateEntry)info.GetValue("Delegate", typeof(DelegateEntry));
			int num = 0;
			DelegateEntry delegateEntry2 = delegateEntry;
			while (delegateEntry2 != null)
			{
				delegateEntry2 = delegateEntry2.delegateEntry;
				num++;
			}
			if (num == 1)
			{
				_delegate = delegateEntry.DeserializeDelegate(info);
				return;
			}
			Delegate[] array = new Delegate[num];
			delegateEntry2 = delegateEntry;
			for (int i = 0; i < num; i++)
			{
				array[i] = delegateEntry2.DeserializeDelegate(info);
				delegateEntry2 = delegateEntry2.delegateEntry;
			}
			_delegate = Delegate.Combine(array);
		}

		public static void GetDelegateData(Delegate instance, SerializationInfo info, StreamingContext ctx)
		{
			Delegate[] invocationList = instance.GetInvocationList();
			DelegateEntry delegateEntry = null;
			for (int i = 0; i < invocationList.Length; i++)
			{
				Delegate obj = invocationList[i];
				string text = ((obj.Target == null) ? null : ("target" + i));
				DelegateEntry delegateEntry2 = new DelegateEntry(obj, text);
				if (delegateEntry == null)
				{
					info.AddValue("Delegate", delegateEntry2);
				}
				else
				{
					delegateEntry.delegateEntry = delegateEntry2;
				}
				delegateEntry = delegateEntry2;
				if (obj.Target != null)
				{
					info.AddValue(text, obj.Target);
				}
			}
			info.SetType(typeof(DelegateSerializationHolder));
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException();
		}

		public object GetRealObject(StreamingContext context)
		{
			return _delegate;
		}
	}
}
