using System.Collections;

namespace System.Runtime.Serialization
{
	public sealed class SerializationObjectManager
	{
		private readonly StreamingContext context;

		private readonly Hashtable seen = new Hashtable();

		private event SerializationCallbacks.CallbackHandler callbacks;

		public SerializationObjectManager(StreamingContext context)
		{
			this.context = context;
		}

		public void RegisterObject(object obj)
		{
			if (seen.Contains(obj))
			{
				return;
			}
			SerializationCallbacks sc = SerializationCallbacks.GetSerializationCallbacks(obj.GetType());
			seen[obj] = 1;
			sc.RaiseOnSerializing(obj, context);
			if (sc.HasSerializedCallbacks)
			{
				this.callbacks = (SerializationCallbacks.CallbackHandler)Delegate.Combine(this.callbacks, (SerializationCallbacks.CallbackHandler)delegate(StreamingContext ctx)
				{
					sc.RaiseOnSerialized(obj, ctx);
				});
			}
		}

		public void RaiseOnSerializedEvent()
		{
			if (this.callbacks != null)
			{
				this.callbacks(context);
			}
		}
	}
}
