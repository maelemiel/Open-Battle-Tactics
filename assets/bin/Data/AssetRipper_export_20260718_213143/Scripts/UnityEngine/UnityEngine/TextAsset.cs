using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class TextAsset : Object
	{
		public extern string text
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern byte[] bytes
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public override string ToString()
		{
			return text;
		}
	}
}
