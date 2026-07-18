using System.Reflection;

namespace UnityEngine.Events
{
	internal class CachedInvokableCall<T> : InvokableCall<T>
	{
		private readonly object[] m_Arg1 = new object[1];

		public CachedInvokableCall(Object target, MethodInfo theFunction, T argument)
			: base((object)target, theFunction)
		{
			m_Arg1[0] = argument;
		}

		public override void Invoke(object[] args)
		{
			base.Invoke(m_Arg1);
		}
	}
}
