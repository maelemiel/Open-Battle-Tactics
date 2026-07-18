using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class SpriteRenderer : Renderer
	{
		public Sprite sprite
		{
			get
			{
				return GetSprite_INTERNAL();
			}
			set
			{
				SetSprite_INTERNAL(value);
			}
		}

		public Color color
		{
			get
			{
				Color value;
				INTERNAL_get_color(out value);
				return value;
			}
			set
			{
				INTERNAL_set_color(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Sprite GetSprite_INTERNAL();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetSprite_INTERNAL(Sprite sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_color(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_color(ref Color value);
	}
}
