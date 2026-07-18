using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Resources
{
	[Serializable]
	[ComVisible(true)]
	public class MissingManifestResourceException : SystemException
	{
		public MissingManifestResourceException()
			: base(Locale.GetText("The assembly does not contain the resources for the required culture."))
		{
		}

		public MissingManifestResourceException(string message)
			: base(message)
		{
		}

		protected MissingManifestResourceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public MissingManifestResourceException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
