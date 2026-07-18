using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Animator : Behaviour
	{
		public extern bool isOptimizable
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool isHuman
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool hasRootMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float humanScale
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Vector3 deltaPosition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Quaternion deltaRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public Vector3 rootPosition
		{
			get
			{
				Vector3 value;
				INTERNAL_get_rootPosition(out value);
				return value;
			}
			set
			{
				INTERNAL_set_rootPosition(ref value);
			}
		}

		public Quaternion rootRotation
		{
			get
			{
				Quaternion value;
				INTERNAL_get_rootRotation(out value);
				return value;
			}
			set
			{
				INTERNAL_set_rootRotation(ref value);
			}
		}

		public extern bool applyRootMotion
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("Use AnimationMode.updateMode instead")]
		public bool animatePhysics
		{
			get
			{
				return updateMode == AnimatorUpdateMode.AnimatePhysics;
			}
			set
			{
				updateMode = (value ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal);
			}
		}

		public extern AnimatorUpdateMode updateMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool hasTransformHierarchy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		internal extern bool allowConstantClipSamplingOptimization
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float gravityWeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public Vector3 bodyPosition
		{
			get
			{
				Vector3 value;
				INTERNAL_get_bodyPosition(out value);
				return value;
			}
			set
			{
				INTERNAL_set_bodyPosition(ref value);
			}
		}

		public Quaternion bodyRotation
		{
			get
			{
				Quaternion value;
				INTERNAL_get_bodyRotation(out value);
				return value;
			}
			set
			{
				INTERNAL_set_bodyRotation(ref value);
			}
		}

		public extern bool stabilizeFeet
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int layerCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float feetPivotActive
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float pivotWeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Vector3 pivotPosition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool isMatchingTarget
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float speed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern Vector3 targetPosition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Quaternion targetRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		internal extern Transform avatarRoot
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern AnimatorCullingMode cullingMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float playbackTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float recorderStartTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float recorderStopTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern RuntimeAnimatorController runtimeAnimatorController
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern Avatar avatar
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool layersAffectMassCenter
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float leftFeetBottomHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float rightFeetBottomHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		private extern bool isInManagerList
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool logWarnings
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool fireEvents
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public float GetFloat(string name)
		{
			return GetFloatString(name);
		}

		public float GetFloat(int id)
		{
			return GetFloatID(id);
		}

		public void SetFloat(string name, float value)
		{
			SetFloatString(name, value);
		}

		public void SetFloat(string name, float value, float dampTime, float deltaTime)
		{
			SetFloatStringDamp(name, value, dampTime, deltaTime);
		}

		public void SetFloat(int id, float value)
		{
			SetFloatID(id, value);
		}

		public void SetFloat(int id, float value, float dampTime, float deltaTime)
		{
			SetFloatIDDamp(id, value, dampTime, deltaTime);
		}

		public bool GetBool(string name)
		{
			return GetBoolString(name);
		}

		public bool GetBool(int id)
		{
			return GetBoolID(id);
		}

		public void SetBool(string name, bool value)
		{
			SetBoolString(name, value);
		}

		public void SetBool(int id, bool value)
		{
			SetBoolID(id, value);
		}

		public int GetInteger(string name)
		{
			return GetIntegerString(name);
		}

		public int GetInteger(int id)
		{
			return GetIntegerID(id);
		}

		public void SetInteger(string name, int value)
		{
			SetIntegerString(name, value);
		}

		public void SetInteger(int id, int value)
		{
			SetIntegerID(id, value);
		}

		public void SetTrigger(string name)
		{
			SetTriggerString(name);
		}

		public void SetTrigger(int id)
		{
			SetTriggerID(id);
		}

		public void ResetTrigger(string name)
		{
			ResetTriggerString(name);
		}

		public void ResetTrigger(int id)
		{
			ResetTriggerID(id);
		}

		public bool IsParameterControlledByCurve(string name)
		{
			return IsParameterControlledByCurveString(name);
		}

		public bool IsParameterControlledByCurve(int id)
		{
			return IsParameterControlledByCurveID(id);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_rootPosition(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_rootPosition(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_rootRotation(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_rootRotation(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_bodyPosition(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_bodyPosition(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_bodyRotation(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_bodyRotation(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Vector3 GetIKPosition(AvatarIKGoal goal);

		public void SetIKPosition(AvatarIKGoal goal, Vector3 goalPosition)
		{
			INTERNAL_CALL_SetIKPosition(this, goal, ref goalPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetIKPosition(Animator self, AvatarIKGoal goal, ref Vector3 goalPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Quaternion GetIKRotation(AvatarIKGoal goal);

		public void SetIKRotation(AvatarIKGoal goal, Quaternion goalRotation)
		{
			INTERNAL_CALL_SetIKRotation(this, goal, ref goalRotation);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetIKRotation(Animator self, AvatarIKGoal goal, ref Quaternion goalRotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern float GetIKPositionWeight(AvatarIKGoal goal);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetIKPositionWeight(AvatarIKGoal goal, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern float GetIKRotationWeight(AvatarIKGoal goal);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetIKRotationWeight(AvatarIKGoal goal, float value);

		public void SetLookAtPosition(Vector3 lookAtPosition)
		{
			INTERNAL_CALL_SetLookAtPosition(this, ref lookAtPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetLookAtPosition(Animator self, ref Vector3 lookAtPosition);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetLookAtWeight(float weight, [DefaultValue("0.00f")] float bodyWeight, [DefaultValue("1.00f")] float headWeight, [DefaultValue("0.00f")] float eyesWeight, [DefaultValue("0.50f")] float clampWeight);

		[ExcludeFromDocs]
		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight, float eyesWeight)
		{
			float clampWeight = 0.5f;
			SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}

		[ExcludeFromDocs]
		public void SetLookAtWeight(float weight, float bodyWeight, float headWeight)
		{
			float clampWeight = 0.5f;
			float eyesWeight = 0f;
			SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}

		[ExcludeFromDocs]
		public void SetLookAtWeight(float weight, float bodyWeight)
		{
			float clampWeight = 0.5f;
			float eyesWeight = 0f;
			float headWeight = 1f;
			SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}

		[ExcludeFromDocs]
		public void SetLookAtWeight(float weight)
		{
			float clampWeight = 0.5f;
			float eyesWeight = 0f;
			float headWeight = 1f;
			float bodyWeight = 0f;
			SetLookAtWeight(weight, bodyWeight, headWeight, eyesWeight, clampWeight);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern string GetLayerName(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern float GetLayerWeight(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetLayerWeight(int layerIndex, float weight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern AnimatorStateInfo GetNextAnimatorStateInfo(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern AnimatorTransitionInfo GetAnimatorTransitionInfo(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern AnimationInfo[] GetCurrentAnimationClipState(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern AnimationInfo[] GetNextAnimationClipState(int layerIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool IsInTransition(int layerIndex);

		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime, [DefaultValue("1")] float targetNormalizedTime)
		{
			INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
		}

		[ExcludeFromDocs]
		public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget targetBodyPart, MatchTargetWeightMask weightMask, float startNormalizedTime)
		{
			float targetNormalizedTime = 1f;
			INTERNAL_CALL_MatchTarget(this, ref matchPosition, ref matchRotation, targetBodyPart, ref weightMask, startNormalizedTime, targetNormalizedTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_MatchTarget(Animator self, ref Vector3 matchPosition, ref Quaternion matchRotation, AvatarTarget targetBodyPart, ref MatchTargetWeightMask weightMask, float startNormalizedTime, float targetNormalizedTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void InterruptMatchTarget([DefaultValue("true")] bool completeMatch);

		[ExcludeFromDocs]
		public void InterruptMatchTarget()
		{
			bool completeMatch = true;
			InterruptMatchTarget(completeMatch);
		}

		[Obsolete("ForceStateNormalizedTime is deprecated. Please use Play or CrossFade instead.")]
		public void ForceStateNormalizedTime(float normalizedTime)
		{
			Play(0, 0, normalizedTime);
		}

		[ExcludeFromDocs]
		public void CrossFade(string stateName, float transitionDuration, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			CrossFade(stateName, transitionDuration, layer, normalizedTime);
		}

		[ExcludeFromDocs]
		public void CrossFade(string stateName, float transitionDuration)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			CrossFade(stateName, transitionDuration, layer, normalizedTime);
		}

		public void CrossFade(string stateName, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			CrossFade(StringToHash(stateName), transitionDuration, layer, normalizedTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void CrossFade(int stateNameHash, float transitionDuration, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);

		[ExcludeFromDocs]
		public void CrossFade(int stateNameHash, float transitionDuration, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			CrossFade(stateNameHash, transitionDuration, layer, normalizedTime);
		}

		[ExcludeFromDocs]
		public void CrossFade(int stateNameHash, float transitionDuration)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			CrossFade(stateNameHash, transitionDuration, layer, normalizedTime);
		}

		[ExcludeFromDocs]
		public void Play(string stateName, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			Play(stateName, layer, normalizedTime);
		}

		[ExcludeFromDocs]
		public void Play(string stateName)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			Play(stateName, layer, normalizedTime);
		}

		public void Play(string stateName, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime)
		{
			Play(StringToHash(stateName), layer, normalizedTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Play(int stateNameHash, [DefaultValue("-1")] int layer, [DefaultValue("float.NegativeInfinity")] float normalizedTime);

		[ExcludeFromDocs]
		public void Play(int stateNameHash, int layer)
		{
			float normalizedTime = float.NegativeInfinity;
			Play(stateNameHash, layer, normalizedTime);
		}

		[ExcludeFromDocs]
		public void Play(int stateNameHash)
		{
			float normalizedTime = float.NegativeInfinity;
			int layer = -1;
			Play(stateNameHash, layer, normalizedTime);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetTarget(AvatarTarget targetIndex, float targetNormalizedTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Obsolete("use mask and layers to control subset of transfroms in a skeleton", true)]
		[WrapperlessIcall]
		public extern bool IsControlled(Transform transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern bool IsBoneTransform(Transform transform);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Transform GetBoneTransform(HumanBodyBones humanBoneId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void StartPlayback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void StopPlayback();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void StartRecording(int frameCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void StopRecording();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern int StringToHash(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern string GetStats();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetFloatString(string name, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetFloatID(int id, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern float GetFloatString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern float GetFloatID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetBoolString(string name, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetBoolID(int id, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool GetBoolString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool GetBoolID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetIntegerString(string name, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetIntegerID(int id, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern int GetIntegerString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern int GetIntegerID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetTriggerString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetTriggerID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void ResetTriggerString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void ResetTriggerID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool IsParameterControlledByCurveString(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool IsParameterControlledByCurveID(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetFloatStringDamp(string name, float value, float dampTime, float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetFloatIDDamp(int id, float value, float dampTime, float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Update(float deltaTime);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Rebind();

		[Obsolete("GetVector is deprecated.")]
		public Vector3 GetVector(string name)
		{
			return Vector3.zero;
		}

		[Obsolete("GetVector is deprecated.")]
		public Vector3 GetVector(int id)
		{
			return Vector3.zero;
		}

		[Obsolete("SetVector is deprecated.")]
		public void SetVector(string name, Vector3 value)
		{
		}

		[Obsolete("SetVector is deprecated.")]
		public void SetVector(int id, Vector3 value)
		{
		}

		[Obsolete("GetQuaternion is deprecated.")]
		public Quaternion GetQuaternion(string name)
		{
			return Quaternion.identity;
		}

		[Obsolete("GetQuaternion is deprecated.")]
		public Quaternion GetQuaternion(int id)
		{
			return Quaternion.identity;
		}

		[Obsolete("SetQuaternion is deprecated.")]
		public void SetQuaternion(string name, Quaternion value)
		{
		}

		[Obsolete("SetQuaternion is deprecated.")]
		public void SetQuaternion(int id, Quaternion value)
		{
		}
	}
}
