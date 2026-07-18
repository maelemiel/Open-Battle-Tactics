using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public struct AnimationInfo
	{
		private int m_ClipInstanceID;

		private float m_Weight;

		public AnimationClip clip
		{
			get
			{
				return (m_ClipInstanceID == 0) ? null : ClipInstanceToScriptingObject(m_ClipInstanceID);
			}
		}

		public float weight
		{
			get
			{
				return m_Weight;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern AnimationClip ClipInstanceToScriptingObject(int instanceID);
	}
}
