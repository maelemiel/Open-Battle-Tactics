using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Resources
{
	[Serializable]
	[ComVisible(true)]
	public class MissingSatelliteAssemblyException : SystemException
	{
		private string culture;

		public string CultureName
		{
			get
			{
				return culture;
			}
		}

		public MissingSatelliteAssemblyException()
			: base(Locale.GetText("The satellite assembly was not found for the required culture."))
		{
		}

		public MissingSatelliteAssemblyException(string message)
			: base(message)
		{
		}

		public MissingSatelliteAssemblyException(string message, string cultureName)
			: base(message)
		{
			culture = cultureName;
		}

		protected MissingSatelliteAssemblyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public MissingSatelliteAssemblyException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
