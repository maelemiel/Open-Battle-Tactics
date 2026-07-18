namespace System.Data
{
	public sealed class StateChangeEventArgs : EventArgs
	{
		private ConnectionState originalState;

		private ConnectionState currentState;

		public ConnectionState CurrentState
		{
			get
			{
				return currentState;
			}
		}

		public ConnectionState OriginalState
		{
			get
			{
				return originalState;
			}
		}

		public StateChangeEventArgs(ConnectionState originalState, ConnectionState currentState)
		{
			this.originalState = originalState;
			this.currentState = currentState;
		}
	}
}
