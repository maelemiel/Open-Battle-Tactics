using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public class SurrogateSelector : ISurrogateSelector
	{
		private Hashtable Surrogates = new Hashtable();

		private ISurrogateSelector nextSelector;

		public virtual void AddSurrogate(Type type, StreamingContext context, ISerializationSurrogate surrogate)
		{
			if (type == null || surrogate == null)
			{
				throw new ArgumentNullException("Null reference.");
			}
			string key = type.FullName + "#" + context;
			if (Surrogates.ContainsKey(key))
			{
				throw new ArgumentException("A surrogate for " + type.FullName + " already exists.");
			}
			Surrogates.Add(key, surrogate);
		}

		public virtual void ChainSelector(ISurrogateSelector selector)
		{
			if (selector == null)
			{
				throw new ArgumentNullException("Selector is null.");
			}
			if (nextSelector != null)
			{
				selector.ChainSelector(nextSelector);
			}
			nextSelector = selector;
		}

		public virtual ISurrogateSelector GetNextSelector()
		{
			return nextSelector;
		}

		public virtual ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type is null.");
			}
			string key = type.FullName + "#" + context;
			ISerializationSurrogate serializationSurrogate = (ISerializationSurrogate)Surrogates[key];
			if (serializationSurrogate != null)
			{
				selector = this;
				return serializationSurrogate;
			}
			if (nextSelector != null)
			{
				return nextSelector.GetSurrogate(type, context, out selector);
			}
			selector = null;
			return null;
		}

		public virtual void RemoveSurrogate(Type type, StreamingContext context)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type is null.");
			}
			string key = type.FullName + "#" + context;
			Surrogates.Remove(key);
		}
	}
}
