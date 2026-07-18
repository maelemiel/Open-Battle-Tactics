using System;
using System.Reflection;

namespace UnityEngine.Events
{
	[Serializable]
	internal class UnityEvent : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[0];

		public void AddListener(UnityAction call)
		{
			AddCall(GetDelegate(call));
		}

		public void RemoveListener(UnityAction call)
		{
			RemoveListener(call.Target, call.Method);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[0]);
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction action)
		{
			return new InvokableCall(action);
		}

		public void Invoke()
		{
			Invoke(m_InvokeArray);
		}
	}
	[Serializable]
	internal abstract class UnityEvent<T0> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[1];

		public void AddListener(UnityAction<T0> call)
		{
			AddCall(GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0> call)
		{
			RemoveListener(call.Target, call.Method);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[1] { typeof(T0) });
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0> action)
		{
			return new InvokableCall<T0>(action);
		}

		public void Invoke(T0 arg0)
		{
			m_InvokeArray[0] = arg0;
			Invoke(m_InvokeArray);
		}
	}
	[Serializable]
	internal abstract class UnityEvent<T0, T1> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[2];

		public void AddListener(UnityAction<T0, T1> call)
		{
			AddCall(GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0, T1> call)
		{
			RemoveListener(call.Target, call.Method);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[2]
			{
				typeof(T0),
				typeof(T1)
			});
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1> action)
		{
			return new InvokableCall<T0, T1>(action);
		}

		public void Invoke(T0 arg0, T1 arg1)
		{
			m_InvokeArray[0] = arg0;
			m_InvokeArray[1] = arg1;
			Invoke(m_InvokeArray);
		}
	}
	[Serializable]
	internal abstract class UnityEvent<T0, T1, T2> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[3];

		public void AddListener(UnityAction<T0, T1, T2> call)
		{
			AddCall(GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0, T1, T2> call)
		{
			RemoveListener(call.Target, call.Method);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[3]
			{
				typeof(T0),
				typeof(T1),
				typeof(T2)
			});
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2> action)
		{
			return new InvokableCall<T0, T1, T2>(action);
		}

		public void Invoke(T0 arg0, T1 arg1, T2 arg2)
		{
			m_InvokeArray[0] = arg0;
			m_InvokeArray[1] = arg1;
			m_InvokeArray[2] = arg2;
			Invoke(m_InvokeArray);
		}
	}
	[Serializable]
	internal abstract class UnityEvent<T0, T1, T2, T3> : UnityEventBase
	{
		private readonly object[] m_InvokeArray = new object[4];

		public void AddListener(UnityAction<T0, T1, T2, T3> call)
		{
			AddCall(GetDelegate(call));
		}

		public void RemoveListener(UnityAction<T0, T1, T2, T3> call)
		{
			RemoveListener(call.Target, call.Method);
		}

		protected override MethodInfo FindMethod_Impl(string name, object targetObj)
		{
			return UnityEventBase.GetValidMethodInfo(targetObj, name, new Type[4]
			{
				typeof(T0),
				typeof(T1),
				typeof(T2),
				typeof(T3)
			});
		}

		internal override BaseInvokableCall GetDelegate(object target, MethodInfo theFunction)
		{
			return new InvokableCall<T0, T1, T2, T3>(target, theFunction);
		}

		private static BaseInvokableCall GetDelegate(UnityAction<T0, T1, T2, T3> action)
		{
			return new InvokableCall<T0, T1, T2, T3>(action);
		}

		public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
		{
			m_InvokeArray[0] = arg0;
			m_InvokeArray[1] = arg1;
			m_InvokeArray[2] = arg2;
			m_InvokeArray[3] = arg3;
			Invoke(m_InvokeArray);
		}
	}
}
