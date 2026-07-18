using System;
using System.Reflection;

namespace UnityEngine.Events
{
	[Serializable]
	internal abstract class UnityEventBase : ISerializationCallbackReceiver
	{
		private InvokableCallList m_Calls;

		[SerializeField]
		private PersistentCallGroup m_PersistentCalls;

		[SerializeField]
		private string m_TypeName;

		protected UnityEventBase()
		{
			m_Calls = new InvokableCallList();
			m_PersistentCalls = new PersistentCallGroup();
			m_TypeName = GetType().AssemblyQualifiedName;
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			m_PersistentCalls.Initialize(m_Calls, this);
			m_TypeName = GetType().AssemblyQualifiedName;
		}

		protected abstract MethodInfo FindMethod_Impl(string name, object targetObj);

		internal abstract BaseInvokableCall GetDelegate(object target, MethodInfo theFunction);

		internal MethodInfo FindMethod(PersistentCall call)
		{
			Type argumentType = typeof(Object);
			if (!string.IsNullOrEmpty(call.arguments.unityObjectArgumentAssemblyTypeName))
			{
				argumentType = Type.GetType(call.arguments.unityObjectArgumentAssemblyTypeName, false) ?? typeof(Object);
			}
			return FindMethod(call.methodName, call.target, call.mode, argumentType);
		}

		internal MethodInfo FindMethod(string name, object listener, PersistentListenerMode mode, Type argumentType)
		{
			switch (mode)
			{
			case PersistentListenerMode.EventDefined:
				return FindMethod_Impl(name, listener);
			case PersistentListenerMode.Void:
				return GetValidMethodInfo(listener, name, new Type[0]);
			case PersistentListenerMode.Float:
				return GetValidMethodInfo(listener, name, new Type[1] { typeof(float) });
			case PersistentListenerMode.Int:
				return GetValidMethodInfo(listener, name, new Type[1] { typeof(int) });
			case PersistentListenerMode.String:
				return GetValidMethodInfo(listener, name, new Type[1] { typeof(string) });
			case PersistentListenerMode.Object:
				return GetValidMethodInfo(listener, name, new Type[1] { argumentType ?? typeof(Object) });
			default:
				return null;
			}
		}

		public int GetPersistentEventCount()
		{
			return m_PersistentCalls.Count;
		}

		public Object GetPersistentTarget(int index)
		{
			PersistentCall listener = m_PersistentCalls.GetListener(index);
			return (listener == null) ? null : listener.target;
		}

		public string GetPersistentMethodName(int index)
		{
			PersistentCall listener = m_PersistentCalls.GetListener(index);
			return (listener == null) ? string.Empty : listener.methodName;
		}

		private void RebuildPersistentCalls()
		{
			m_Calls.ClearPersistent();
			m_PersistentCalls.Initialize(m_Calls, this);
		}

		public void DisablePersistentListener(int index)
		{
			PersistentCall listener = m_PersistentCalls.GetListener(index);
			if (listener != null)
			{
				listener.enabled = false;
			}
			RebuildPersistentCalls();
		}

		public void EnablePersistentListener(int index)
		{
			PersistentCall listener = m_PersistentCalls.GetListener(index);
			if (listener != null)
			{
				listener.enabled = true;
			}
			RebuildPersistentCalls();
		}

		protected void AddListener(object targetObj, MethodInfo method)
		{
			m_Calls.AddListener(GetDelegate(targetObj, method));
		}

		internal void AddCall(BaseInvokableCall call)
		{
			m_Calls.AddListener(call);
		}

		protected void RemoveListener(object targetObj, MethodInfo method)
		{
			m_Calls.RemoveListener(targetObj, method);
		}

		public void RemoveAllListeners()
		{
			m_Calls.Clear();
		}

		protected void Invoke(object[] parameters)
		{
			m_Calls.Invoke(parameters);
		}

		public override string ToString()
		{
			return base.ToString() + " " + GetType().FullName;
		}

		public static MethodInfo GetValidMethodInfo(object obj, string functionName, Type[] argumentTypes)
		{
			Type type = obj.GetType();
			while (type != typeof(object) && type != null)
			{
				MethodInfo method = type.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, argumentTypes, null);
				if (method != null)
				{
					return method;
				}
				type = type.BaseType;
			}
			return null;
		}
	}
}
