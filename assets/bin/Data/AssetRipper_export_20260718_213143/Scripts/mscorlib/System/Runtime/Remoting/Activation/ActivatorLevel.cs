using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Activation
{
	[Serializable]
	[ComVisible(true)]
	public enum ActivatorLevel
	{
		Construction = 4,
		Context = 8,
		AppDomain = 12,
		Process = 16,
		Machine = 20
	}
}
