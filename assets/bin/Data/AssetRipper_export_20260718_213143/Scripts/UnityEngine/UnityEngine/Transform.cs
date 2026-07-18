using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Transform : Component, IEnumerable
	{
		private sealed class Enumerator : IEnumerator
		{
			private Transform outer;

			private int currentIndex = -1;

			public object Current
			{
				get
				{
					return outer.GetChild(currentIndex);
				}
			}

			internal Enumerator(Transform outer)
			{
				this.outer = outer;
			}

			public bool MoveNext()
			{
				int childCount = outer.childCount;
				return ++currentIndex < childCount;
			}

			public void Reset()
			{
				currentIndex = -1;
			}
		}

		public Vector3 position
		{
			get
			{
				Vector3 value;
				INTERNAL_get_position(out value);
				return value;
			}
			set
			{
				INTERNAL_set_position(ref value);
			}
		}

		public Vector3 localPosition
		{
			get
			{
				Vector3 value;
				INTERNAL_get_localPosition(out value);
				return value;
			}
			set
			{
				INTERNAL_set_localPosition(ref value);
			}
		}

		public Vector3 eulerAngles
		{
			get
			{
				return rotation.eulerAngles;
			}
			set
			{
				rotation = Quaternion.Euler(value);
			}
		}

		public Vector3 localEulerAngles
		{
			get
			{
				Vector3 value;
				INTERNAL_get_localEulerAngles(out value);
				return value;
			}
			set
			{
				INTERNAL_set_localEulerAngles(ref value);
			}
		}

		public Vector3 right
		{
			get
			{
				return rotation * Vector3.right;
			}
			set
			{
				rotation = Quaternion.FromToRotation(Vector3.right, value);
			}
		}

		public Vector3 up
		{
			get
			{
				return rotation * Vector3.up;
			}
			set
			{
				rotation = Quaternion.FromToRotation(Vector3.up, value);
			}
		}

		public Vector3 forward
		{
			get
			{
				return rotation * Vector3.forward;
			}
			set
			{
				rotation = Quaternion.LookRotation(value);
			}
		}

		public Quaternion rotation
		{
			get
			{
				Quaternion value;
				INTERNAL_get_rotation(out value);
				return value;
			}
			set
			{
				INTERNAL_set_rotation(ref value);
			}
		}

		public Quaternion localRotation
		{
			get
			{
				Quaternion value;
				INTERNAL_get_localRotation(out value);
				return value;
			}
			set
			{
				INTERNAL_set_localRotation(ref value);
			}
		}

		public Vector3 localScale
		{
			get
			{
				Vector3 value;
				INTERNAL_get_localScale(out value);
				return value;
			}
			set
			{
				INTERNAL_set_localScale(ref value);
			}
		}

		public extern Transform parent
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Matrix4x4 worldToLocalMatrix
		{
			get
			{
				Matrix4x4 value;
				INTERNAL_get_worldToLocalMatrix(out value);
				return value;
			}
		}

		public Matrix4x4 localToWorldMatrix
		{
			get
			{
				Matrix4x4 value;
				INTERNAL_get_localToWorldMatrix(out value);
				return value;
			}
		}

		public extern Transform root
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int childCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public Vector3 lossyScale
		{
			get
			{
				Vector3 value;
				INTERNAL_get_lossyScale(out value);
				return value;
			}
		}

		public extern bool hasChanged
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		private Transform()
		{
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_position(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_position(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_localPosition(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_localPosition(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_localEulerAngles(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_localEulerAngles(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_rotation(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_rotation(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_localRotation(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_localRotation(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_localScale(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_localScale(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_worldToLocalMatrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_localToWorldMatrix(out Matrix4x4 value);

		[ExcludeFromDocs]
		public void Translate(Vector3 translation)
		{
			Space relativeTo = Space.Self;
			Translate(translation, relativeTo);
		}

		public void Translate(Vector3 translation, [DefaultValue("Space.Self")] Space relativeTo)
		{
			if (relativeTo == Space.World)
			{
				position += translation;
			}
			else
			{
				position += TransformDirection(translation);
			}
		}

		[ExcludeFromDocs]
		public void Translate(float x, float y, float z)
		{
			Space relativeTo = Space.Self;
			Translate(x, y, z, relativeTo);
		}

		public void Translate(float x, float y, float z, [DefaultValue("Space.Self")] Space relativeTo)
		{
			Translate(new Vector3(x, y, z), relativeTo);
		}

		public void Translate(Vector3 translation, Transform relativeTo)
		{
			if ((bool)relativeTo)
			{
				position += relativeTo.TransformDirection(translation);
			}
			else
			{
				position += translation;
			}
		}

		public void Translate(float x, float y, float z, Transform relativeTo)
		{
			Translate(new Vector3(x, y, z), relativeTo);
		}

		[ExcludeFromDocs]
		public void Rotate(Vector3 eulerAngles)
		{
			Space relativeTo = Space.Self;
			Rotate(eulerAngles, relativeTo);
		}

		public void Rotate(Vector3 eulerAngles, [DefaultValue("Space.Self")] Space relativeTo)
		{
			Quaternion quaternion = Quaternion.Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
			if (relativeTo == Space.Self)
			{
				localRotation *= quaternion;
			}
			else
			{
				rotation *= Quaternion.Inverse(rotation) * quaternion * rotation;
			}
		}

		[ExcludeFromDocs]
		public void Rotate(float xAngle, float yAngle, float zAngle)
		{
			Space relativeTo = Space.Self;
			Rotate(xAngle, yAngle, zAngle, relativeTo);
		}

		public void Rotate(float xAngle, float yAngle, float zAngle, [DefaultValue("Space.Self")] Space relativeTo)
		{
			Rotate(new Vector3(xAngle, yAngle, zAngle), relativeTo);
		}

		internal void RotateAroundInternal(Vector3 axis, float angle)
		{
			INTERNAL_CALL_RotateAroundInternal(this, ref axis, angle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_RotateAroundInternal(Transform self, ref Vector3 axis, float angle);

		[ExcludeFromDocs]
		public void Rotate(Vector3 axis, float angle)
		{
			Space relativeTo = Space.Self;
			Rotate(axis, angle, relativeTo);
		}

		public void Rotate(Vector3 axis, float angle, [DefaultValue("Space.Self")] Space relativeTo)
		{
			if (relativeTo == Space.Self)
			{
				RotateAroundInternal(base.transform.TransformDirection(axis), angle * ((float)Math.PI / 180f));
			}
			else
			{
				RotateAroundInternal(axis, angle * ((float)Math.PI / 180f));
			}
		}

		public void RotateAround(Vector3 point, Vector3 axis, float angle)
		{
			Vector3 vector = position;
			Quaternion quaternion = Quaternion.AngleAxis(angle, axis);
			Vector3 vector2 = vector - point;
			vector2 = quaternion * vector2;
			vector = point + vector2;
			position = vector;
			RotateAroundInternal(axis, angle * ((float)Math.PI / 180f));
		}

		[ExcludeFromDocs]
		public void LookAt(Transform target)
		{
			Vector3 worldUp = Vector3.up;
			LookAt(target, worldUp);
		}

		public void LookAt(Transform target, [DefaultValue("Vector3.up")] Vector3 worldUp)
		{
			if ((bool)target)
			{
				LookAt(target.position, worldUp);
			}
		}

		public void LookAt(Vector3 worldPosition, [DefaultValue("Vector3.up")] Vector3 worldUp)
		{
			INTERNAL_CALL_LookAt(this, ref worldPosition, ref worldUp);
		}

		[ExcludeFromDocs]
		public void LookAt(Vector3 worldPosition)
		{
			Vector3 worldUp = Vector3.up;
			INTERNAL_CALL_LookAt(this, ref worldPosition, ref worldUp);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_LookAt(Transform self, ref Vector3 worldPosition, ref Vector3 worldUp);

		public Vector3 TransformDirection(Vector3 direction)
		{
			return INTERNAL_CALL_TransformDirection(this, ref direction);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_TransformDirection(Transform self, ref Vector3 direction);

		public Vector3 TransformDirection(float x, float y, float z)
		{
			return TransformDirection(new Vector3(x, y, z));
		}

		public Vector3 InverseTransformDirection(Vector3 direction)
		{
			return INTERNAL_CALL_InverseTransformDirection(this, ref direction);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_InverseTransformDirection(Transform self, ref Vector3 direction);

		public Vector3 InverseTransformDirection(float x, float y, float z)
		{
			return InverseTransformDirection(new Vector3(x, y, z));
		}

		public Vector3 TransformPoint(Vector3 position)
		{
			return INTERNAL_CALL_TransformPoint(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_TransformPoint(Transform self, ref Vector3 position);

		public Vector3 TransformPoint(float x, float y, float z)
		{
			return TransformPoint(new Vector3(x, y, z));
		}

		public Vector3 InverseTransformPoint(Vector3 position)
		{
			return INTERNAL_CALL_InverseTransformPoint(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_InverseTransformPoint(Transform self, ref Vector3 position);

		public Vector3 InverseTransformPoint(float x, float y, float z)
		{
			return InverseTransformPoint(new Vector3(x, y, z));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void DetachChildren();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetAsFirstSibling();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetAsLastSibling();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetSiblingIndex(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern int GetSiblingIndex();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Transform Find(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_lossyScale(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool IsChildOf(Transform parent);

		public Transform FindChild(string name)
		{
			return Find(name);
		}

		public IEnumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		[Obsolete("use Transform.Rotate instead.")]
		public void RotateAround(Vector3 axis, float angle)
		{
			INTERNAL_CALL_RotateAround(this, ref axis, angle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_RotateAround(Transform self, ref Vector3 axis, float angle);

		[Obsolete("use Transform.Rotate instead.")]
		public void RotateAroundLocal(Vector3 axis, float angle)
		{
			INTERNAL_CALL_RotateAroundLocal(this, ref axis, angle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_RotateAroundLocal(Transform self, ref Vector3 axis, float angle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Transform GetChild(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("use Transform.childCount instead.")]
		public extern int GetChildCount();
	}
}
