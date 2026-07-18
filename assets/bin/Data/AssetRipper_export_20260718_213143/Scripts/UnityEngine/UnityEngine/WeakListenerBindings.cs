using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	internal sealed class WeakListenerBindings
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void InvokeCallbacks(object inst, GCHandle gchandle, object[] parameters);
	}
}
