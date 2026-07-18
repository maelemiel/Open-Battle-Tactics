using System.Runtime.InteropServices;

namespace System.Resources
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[ComVisible(true)]
	public sealed class SatelliteContractVersionAttribute : Attribute
	{
		private Version ver;

		public string Version
		{
			get
			{
				return ver.ToString();
			}
		}

		public SatelliteContractVersionAttribute(string version)
		{
			ver = new Version(version);
		}
	}
}
