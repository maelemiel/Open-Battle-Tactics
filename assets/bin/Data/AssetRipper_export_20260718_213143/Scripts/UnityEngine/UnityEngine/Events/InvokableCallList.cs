using System.Collections.Generic;
using System.Reflection;

namespace UnityEngine.Events
{
	internal class InvokableCallList
	{
		private readonly List<BaseInvokableCall> m_PersistentCalls = new List<BaseInvokableCall>();

		private readonly List<BaseInvokableCall> m_RuntimeCalls = new List<BaseInvokableCall>();

		public int Count
		{
			get
			{
				return m_PersistentCalls.Count + m_RuntimeCalls.Count;
			}
		}

		public void AddPersistentInvokableCall(BaseInvokableCall call)
		{
			m_PersistentCalls.Add(call);
		}

		public void AddListener(BaseInvokableCall call)
		{
			m_RuntimeCalls.Add(call);
		}

		public void RemoveListener(object targetObj, MethodInfo method)
		{
			List<BaseInvokableCall> list = new List<BaseInvokableCall>();
			for (int i = 0; i < m_RuntimeCalls.Count; i++)
			{
				if (m_RuntimeCalls[i].Find(targetObj, method))
				{
					list.Add(m_RuntimeCalls[i]);
				}
			}
			m_RuntimeCalls.RemoveAll(list.Contains);
		}

		public void Clear()
		{
			m_RuntimeCalls.Clear();
		}

		public void ClearPersistent()
		{
			m_PersistentCalls.Clear();
		}

		public void Invoke(object[] parameters)
		{
			for (int i = 0; i < m_PersistentCalls.Count; i++)
			{
				m_PersistentCalls[i].Invoke(parameters);
			}
			for (int j = 0; j < m_RuntimeCalls.Count; j++)
			{
				m_RuntimeCalls[j].Invoke(parameters);
			}
		}
	}
}
