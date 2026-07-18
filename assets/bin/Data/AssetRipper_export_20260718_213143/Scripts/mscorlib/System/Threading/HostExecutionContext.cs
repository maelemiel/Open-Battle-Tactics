namespace System.Threading
{
	[MonoTODO("Useless until the runtime supports it")]
	public class HostExecutionContext
	{
		private object _state;

		protected internal object State
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
			}
		}

		public HostExecutionContext()
		{
			_state = null;
		}

		public HostExecutionContext(object state)
		{
			_state = state;
		}

		public virtual HostExecutionContext CreateCopy()
		{
			return new HostExecutionContext(_state);
		}
	}
}
