using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public interface ISurrogateSelector
	{
		void ChainSelector(ISurrogateSelector selector);

		ISurrogateSelector GetNextSelector();

		ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector);
	}
}
