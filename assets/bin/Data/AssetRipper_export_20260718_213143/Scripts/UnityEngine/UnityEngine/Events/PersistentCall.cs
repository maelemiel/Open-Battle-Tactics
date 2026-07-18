using System;
using System.Reflection;

namespace UnityEngine.Events
{
	[Serializable]
	internal class PersistentCall
	{
		[SerializeField]
		private Object m_Target;

		[SerializeField]
		private string m_MethodName;

		[SerializeField]
		private PersistentListenerMode m_Mode;

		[SerializeField]
		private ArgumentCache m_Arguments = new ArgumentCache();

		[SerializeField]
		private bool m_Enabled = true;

		public Object target
		{
			get
			{
				return m_Target;
			}
		}

		public string methodName
		{
			get
			{
				return m_MethodName;
			}
		}

		public PersistentListenerMode mode
		{
			get
			{
				return m_Mode;
			}
			set
			{
				m_Mode = value;
			}
		}

		public ArgumentCache arguments
		{
			get
			{
				return m_Arguments;
			}
		}

		public bool enabled
		{
			get
			{
				return m_Enabled;
			}
			set
			{
				m_Enabled = value;
			}
		}

		public BaseInvokableCall GetRuntimeCall(UnityEventBase theEvent)
		{
			if (!enabled || theEvent == null)
			{
				return null;
			}
			MethodInfo methodInfo = theEvent.FindMethod(this);
			if (methodInfo == null)
			{
				return null;
			}
			switch (m_Mode)
			{
			case PersistentListenerMode.EventDefined:
				return theEvent.GetDelegate(target, methodInfo);
			case PersistentListenerMode.Object:
				return GetObjectCall(target, methodInfo, m_Arguments);
			case PersistentListenerMode.Float:
				return new CachedInvokableCall<float>(target, methodInfo, m_Arguments.floatArgument);
			case PersistentListenerMode.Int:
				return new CachedInvokableCall<int>(target, methodInfo, m_Arguments.intArgument);
			case PersistentListenerMode.String:
				return new CachedInvokableCall<string>(target, methodInfo, m_Arguments.stringArgument);
			case PersistentListenerMode.Void:
				return new InvokableCall(target, methodInfo);
			default:
				return null;
			}
		}

		private static T RuntimeHackCast<T>(object o) where T : class
		{
			return o as T;
		}

		private static BaseInvokableCall GetObjectCall(Object target, MethodInfo method, ArgumentCache arguments)
		{
			Type type = typeof(Object);
			if (!string.IsNullOrEmpty(arguments.unityObjectArgumentAssemblyTypeName))
			{
				type = Type.GetType(arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(Object);
			}
			Type typeFromHandle = typeof(CachedInvokableCall<>);
			Type type2 = typeFromHandle.MakeGenericType(type);
			ConstructorInfo constructor = type2.GetConstructor(new Type[3]
			{
				typeof(Object),
				typeof(MethodInfo),
				type
			});
			MethodInfo methodInfo = typeof(PersistentCall).GetMethod("RuntimeHackCast", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type);
			object obj = methodInfo.Invoke(null, new object[1] { arguments.unityObjectArgument });
			return constructor.Invoke(new object[3] { target, method, obj }) as BaseInvokableCall;
		}

		public void RegisterPersistentListener(Object ttarget, string mmethodName)
		{
			m_Target = ttarget;
			m_MethodName = mmethodName;
		}

		public void UnregisterPersistentListener()
		{
			m_MethodName = string.Empty;
			m_Target = null;
		}
	}
}
