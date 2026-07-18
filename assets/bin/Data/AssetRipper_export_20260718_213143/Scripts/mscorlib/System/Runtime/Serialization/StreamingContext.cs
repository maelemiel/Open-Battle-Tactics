using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[Serializable]
	[ComVisible(true)]
	public struct StreamingContext
	{
		private StreamingContextStates state;

		private object additional;

		public object Context
		{
			get
			{
				return additional;
			}
		}

		public StreamingContextStates State
		{
			get
			{
				return state;
			}
		}

		public StreamingContext(StreamingContextStates state)
		{
			this.state = state;
			additional = null;
		}

		public StreamingContext(StreamingContextStates state, object additional)
		{
			this.state = state;
			this.additional = additional;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is StreamingContext))
			{
				return false;
			}
			StreamingContext streamingContext = (StreamingContext)obj;
			return streamingContext.state == state && streamingContext.additional == additional;
		}

		public override int GetHashCode()
		{
			return (int)state;
		}
	}
}
