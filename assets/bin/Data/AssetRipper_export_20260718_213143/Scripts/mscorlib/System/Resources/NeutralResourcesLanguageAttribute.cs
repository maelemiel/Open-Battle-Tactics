using System.Runtime.InteropServices;

namespace System.Resources
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class NeutralResourcesLanguageAttribute : Attribute
	{
		private string culture;

		private UltimateResourceFallbackLocation loc;

		public string CultureName
		{
			get
			{
				return culture;
			}
		}

		public UltimateResourceFallbackLocation Location
		{
			get
			{
				return loc;
			}
		}

		public NeutralResourcesLanguageAttribute(string cultureName)
		{
			if (cultureName == null)
			{
				throw new ArgumentNullException("culture is null");
			}
			culture = cultureName;
		}

		public NeutralResourcesLanguageAttribute(string cultureName, UltimateResourceFallbackLocation location)
		{
			if (cultureName == null)
			{
				throw new ArgumentNullException("culture is null");
			}
			culture = cultureName;
			loc = location;
		}
	}
}
