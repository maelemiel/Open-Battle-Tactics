using System.Runtime.Serialization;

namespace System.Transactions
{
	[Serializable]
	[System.MonoTODO("Not supported yet")]
	public sealed class DependentTransaction : Transaction, ISerializable
	{
		private bool completed;

		internal bool Completed
		{
			get
			{
				return completed;
			}
		}

		internal DependentTransaction(Transaction parent, DependentCloneOption option)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			completed = info.GetBoolean("completed");
		}

		[System.MonoTODO]
		public void Complete()
		{
			throw new NotImplementedException();
		}
	}
}
