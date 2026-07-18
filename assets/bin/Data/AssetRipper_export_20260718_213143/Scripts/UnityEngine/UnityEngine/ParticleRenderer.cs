using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ParticleRenderer : Renderer
	{
		public extern ParticleRenderMode particleRenderMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float lengthScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float velocityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float cameraVelocityScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float maxParticleSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int uvAnimationXTile
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int uvAnimationYTile
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float uvAnimationCycles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("animatedTextureCount has been replaced by uvAnimationXTile and uvAnimationYTile.")]
		public int animatedTextureCount
		{
			get
			{
				return uvAnimationXTile;
			}
			set
			{
				uvAnimationXTile = value;
			}
		}

		public float maxPartileSize
		{
			get
			{
				return maxParticleSize;
			}
			set
			{
				maxParticleSize = value;
			}
		}

		public extern Rect[] uvTiles
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("This function has been removed.", true)]
		public AnimationCurve widthCurve
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("This function has been removed.", true)]
		public AnimationCurve heightCurve
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("This function has been removed.", true)]
		public AnimationCurve rotationCurve
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
	}
}
