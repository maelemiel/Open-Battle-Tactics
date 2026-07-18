using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_EventInfo))]
	[ClassInterface(ClassInterfaceType.None)]
	public abstract class EventInfo : MemberInfo, _EventInfo
	{
		private delegate void AddEventAdapter(object _this, Delegate dele);

		private delegate void AddEvent<T, D>(T _this, D dele);

		private delegate void StaticAddEvent<D>(D dele);

		private AddEventAdapter cached_add_event;

		public abstract EventAttributes Attributes { get; }

		public Type EventHandlerType
		{
			get
			{
				MethodInfo addMethod = GetAddMethod(true);
				ParameterInfo[] parameters = addMethod.GetParameters();
				if (parameters.Length > 0)
				{
					return parameters[0].ParameterType;
				}
				return null;
			}
		}

		public bool IsMulticast
		{
			get
			{
				return true;
			}
		}

		public bool IsSpecialName
		{
			get
			{
				return (Attributes & EventAttributes.SpecialName) != 0;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Event;
			}
		}

		void _EventInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _EventInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _EventInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _EventInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[DebuggerStepThrough]
		[DebuggerHidden]
		public void AddEventHandler(object target, Delegate handler)
		{
			if (cached_add_event == null)
			{
				MethodInfo addMethod = GetAddMethod();
				if (addMethod == null)
				{
					throw new InvalidOperationException("Cannot add a handler to an event that doesn't have a visible add method");
				}
				if (addMethod.DeclaringType.IsValueType)
				{
					if (target == null && !addMethod.IsStatic)
					{
						throw new TargetException("Cannot add a handler to a non static event with a null target");
					}
					addMethod.Invoke(target, new object[1] { handler });
					return;
				}
				cached_add_event = CreateAddEventDelegate(addMethod);
			}
			cached_add_event(target, handler);
		}

		public MethodInfo GetAddMethod()
		{
			return GetAddMethod(false);
		}

		public abstract MethodInfo GetAddMethod(bool nonPublic);

		public MethodInfo GetRaiseMethod()
		{
			return GetRaiseMethod(false);
		}

		public abstract MethodInfo GetRaiseMethod(bool nonPublic);

		public MethodInfo GetRemoveMethod()
		{
			return GetRemoveMethod(false);
		}

		public abstract MethodInfo GetRemoveMethod(bool nonPublic);

		public virtual MethodInfo[] GetOtherMethods(bool nonPublic)
		{
			return new MethodInfo[0];
		}

		public MethodInfo[] GetOtherMethods()
		{
			return GetOtherMethods(false);
		}

		[DebuggerStepThrough]
		[DebuggerHidden]
		public void RemoveEventHandler(object target, Delegate handler)
		{
			MethodInfo removeMethod = GetRemoveMethod();
			if (removeMethod == null)
			{
				throw new InvalidOperationException("Cannot remove a handler to an event that doesn't have a visible remove method");
			}
			removeMethod.Invoke(target, new object[1] { handler });
		}

		private static void AddEventFrame<T, D>(AddEvent<T, D> addEvent, object obj, object dele)
		{
			if (obj == null)
			{
				throw new TargetException("Cannot add a handler to a non static event with a null target");
			}
			if (!(obj is T))
			{
				throw new TargetException("Object doesn't match target");
			}
			addEvent((T)obj, (D)dele);
		}

		private static void StaticAddEventAdapterFrame<D>(StaticAddEvent<D> addEvent, object obj, object dele)
		{
			addEvent((D)dele);
		}

		private static AddEventAdapter CreateAddEventDelegate(MethodInfo method)
		{
			Type[] typeArguments;
			Type typeFromHandle;
			string name;
			if (method.IsStatic)
			{
				typeArguments = new Type[1] { method.GetParameters()[0].ParameterType };
				typeFromHandle = typeof(StaticAddEvent<>);
				name = "StaticAddEventAdapterFrame";
			}
			else
			{
				typeArguments = new Type[2]
				{
					method.DeclaringType,
					method.GetParameters()[0].ParameterType
				};
				typeFromHandle = typeof(AddEvent<, >);
				name = "AddEventFrame";
			}
			Type type = typeFromHandle.MakeGenericType(typeArguments);
			object obj = Delegate.CreateDelegate(type, method, false);
			if (obj == null)
			{
				throw new MethodAccessException();
			}
			MethodInfo method2 = typeof(EventInfo).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
			method2 = method2.MakeGenericMethod(typeArguments);
			return (AddEventAdapter)Delegate.CreateDelegate(typeof(AddEventAdapter), obj, method2, true);
		}

		virtual Type _EventInfo.GetType()
		{
			return GetType();
		}
	}
}
