using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AnimatorOverrideController : RuntimeAnimatorController
	{
		public extern RuntimeAnimatorController runtimeAnimatorController
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public AnimationClip this[string name]
		{
			get
			{
				return Internal_GetClipByName(name, true);
			}
			set
			{
				Internal_SetClipByName(name, value);
			}
		}

		public AnimationClip this[AnimationClip clip]
		{
			get
			{
				return Internal_GetClip(clip, true);
			}
			set
			{
				Internal_SetClip(clip, value);
			}
		}

		public AnimationClipPair[] clips
		{
			get
			{
				AnimationClip[] originalClips = GetOriginalClips();
				AnimationClipPair[] array = new AnimationClipPair[originalClips.Length];
				for (int i = 0; i < originalClips.Length; i++)
				{
					array[i] = new AnimationClipPair();
					array[i].originalClip = originalClips[i];
					array[i].overrideClip = Internal_GetClip(originalClips[i], false);
				}
				return array;
			}
			set
			{
				for (int i = 0; i < value.Length; i++)
				{
					Internal_SetClip(value[i].originalClip, value[i].overrideClip);
				}
			}
		}

		public AnimatorOverrideController()
		{
			Internal_CreateAnimationSet(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateAnimationSet([Writable] AnimatorOverrideController self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern AnimationClip Internal_GetClipByName(string name, bool returnEffectiveClip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetClipByName(string name, AnimationClip clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern AnimationClip Internal_GetClip(AnimationClip originalClip, bool returnEffectiveClip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetClip(AnimationClip originalClip, AnimationClip overrideClip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern AnimationClip[] GetOriginalClips();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern AnimationClip[] GetOverrideClips();
	}
}
