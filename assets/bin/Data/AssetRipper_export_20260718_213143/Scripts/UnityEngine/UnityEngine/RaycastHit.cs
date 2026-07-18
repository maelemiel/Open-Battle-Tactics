using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public struct RaycastHit
	{
		private Vector3 m_Point;

		private Vector3 m_Normal;

		private int m_FaceID;

		private float m_Distance;

		private Vector2 m_UV;

		private Collider m_Collider;

		public Vector3 point
		{
			get
			{
				return m_Point;
			}
			set
			{
				m_Point = value;
			}
		}

		public Vector3 normal
		{
			get
			{
				return m_Normal;
			}
			set
			{
				m_Normal = value;
			}
		}

		public Vector3 barycentricCoordinate
		{
			get
			{
				return new Vector3(1f - (m_UV.y + m_UV.x), m_UV.x, m_UV.y);
			}
			set
			{
				m_UV = value;
			}
		}

		public float distance
		{
			get
			{
				return m_Distance;
			}
			set
			{
				m_Distance = value;
			}
		}

		public int triangleIndex
		{
			get
			{
				return m_FaceID;
			}
		}

		public Vector2 textureCoord
		{
			get
			{
				Vector2 output;
				CalculateRaycastTexCoord(out output, collider, m_UV, m_Point, m_FaceID, 0);
				return output;
			}
		}

		public Vector2 textureCoord2
		{
			get
			{
				Vector2 output;
				CalculateRaycastTexCoord(out output, collider, m_UV, m_Point, m_FaceID, 1);
				return output;
			}
		}

		[Obsolete("Use textureCoord2 instead")]
		public Vector2 textureCoord1
		{
			get
			{
				Vector2 output;
				CalculateRaycastTexCoord(out output, collider, m_UV, m_Point, m_FaceID, 1);
				return output;
			}
		}

		public Vector2 lightmapCoord
		{
			get
			{
				Vector2 output;
				CalculateRaycastTexCoord(out output, collider, m_UV, m_Point, m_FaceID, 1);
				if (collider.renderer != null)
				{
					Vector4 lightmapTilingOffset = collider.renderer.lightmapTilingOffset;
					output.x = output.x * lightmapTilingOffset.x + lightmapTilingOffset.z;
					output.y = output.y * lightmapTilingOffset.y + lightmapTilingOffset.w;
				}
				return output;
			}
		}

		public Collider collider
		{
			get
			{
				return m_Collider;
			}
		}

		public Rigidbody rigidbody
		{
			get
			{
				return (!(collider != null)) ? null : collider.attachedRigidbody;
			}
		}

		public Transform transform
		{
			get
			{
				Rigidbody rigidbody = this.rigidbody;
				if (rigidbody != null)
				{
					return rigidbody.transform;
				}
				if (collider != null)
				{
					return collider.transform;
				}
				return null;
			}
		}

		private static void CalculateRaycastTexCoord(out Vector2 output, Collider col, Vector2 uv, Vector3 point, int face, int index)
		{
			INTERNAL_CALL_CalculateRaycastTexCoord(out output, col, ref uv, ref point, face, index);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_CalculateRaycastTexCoord(out Vector2 output, Collider col, ref Vector2 uv, ref Vector3 point, int face, int index);
	}
}
