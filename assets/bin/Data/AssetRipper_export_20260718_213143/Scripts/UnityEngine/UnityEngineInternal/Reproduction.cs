using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEngineInternal
{
	public sealed class Reproduction
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void CaptureScreenshot();
	}
}
