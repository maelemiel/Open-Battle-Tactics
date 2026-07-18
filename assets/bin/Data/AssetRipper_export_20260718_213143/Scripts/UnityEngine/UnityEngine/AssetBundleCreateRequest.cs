using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AssetBundleCreateRequest : AsyncOperation
	{
		public extern AssetBundle assetBundle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern void DisableCompatibilityChecks();
	}
}
