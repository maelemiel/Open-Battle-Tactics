using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class TerrainCollider : Collider
	{
		public extern TerrainData terrainData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}
	}
}
