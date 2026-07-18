using System.Security;

namespace System.Threading
{
	public struct AsyncFlowControl : IDisposable
	{
		private Thread _t;

		private AsyncFlowControlType _type;

		internal AsyncFlowControl(Thread t, AsyncFlowControlType type)
		{
			_t = t;
			_type = type;
		}

		void IDisposable.Dispose()
		{
			if (_t != null)
			{
				Undo();
				_t = null;
				_type = AsyncFlowControlType.None;
			}
		}

		public void Undo()
		{
			if (_t == null)
			{
				throw new InvalidOperationException(Locale.GetText("Can only be called once."));
			}
			switch (_type)
			{
			case AsyncFlowControlType.Execution:
				ExecutionContext.RestoreFlow();
				break;
			case AsyncFlowControlType.Security:
				SecurityContext.RestoreFlow();
				break;
			}
			_t = null;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is AsyncFlowControl))
			{
				return false;
			}
			return obj.Equals(this);
		}

		public bool Equals(AsyncFlowControl obj)
		{
			if (_t == obj._t && _type == obj._type)
			{
				return true;
			}
			return false;
		}

		public static bool operator ==(AsyncFlowControl a, AsyncFlowControl b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(AsyncFlowControl a, AsyncFlowControl b)
		{
			return !a.Equals(b);
		}
	}
}
