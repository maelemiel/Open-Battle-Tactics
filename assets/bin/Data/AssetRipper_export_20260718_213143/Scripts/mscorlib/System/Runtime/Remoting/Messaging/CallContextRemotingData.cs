namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	internal class CallContextRemotingData : ICloneable
	{
		private string _logicalCallID;

		public string LogicalCallID
		{
			get
			{
				return _logicalCallID;
			}
			set
			{
				_logicalCallID = value;
			}
		}

		public object Clone()
		{
			CallContextRemotingData callContextRemotingData = new CallContextRemotingData();
			callContextRemotingData._logicalCallID = _logicalCallID;
			return callContextRemotingData;
		}
	}
}
