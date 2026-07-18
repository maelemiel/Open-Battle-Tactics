using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public struct Bounds
	{
		private Vector3 m_Center;

		private Vector3 m_Extents;

		public Vector3 center
		{
			get
			{
				return m_Center;
			}
			set
			{
				m_Center = value;
			}
		}

		public Vector3 size
		{
			get
			{
				return m_Extents * 2f;
			}
			set
			{
				m_Extents = value * 0.5f;
			}
		}

		public Vector3 extents
		{
			get
			{
				return m_Extents;
			}
			set
			{
				m_Extents = value;
			}
		}

		public Vector3 min
		{
			get
			{
				return center - extents;
			}
			set
			{
				SetMinMax(value, max);
			}
		}

		public Vector3 max
		{
			get
			{
				return center + extents;
			}
			set
			{
				SetMinMax(min, value);
			}
		}

		public Bounds(Vector3 center, Vector3 size)
		{
			m_Center = center;
			m_Extents = size * 0.5f;
		}

		public override int GetHashCode()
		{
			return center.GetHashCode() ^ (extents.GetHashCode() << 2);
		}

		public override bool Equals(object other)
		{
			if (!(other is Bounds))
			{
				return false;
			}
			Bounds bounds = (Bounds)other;
			return center.Equals(bounds.center) && extents.Equals(bounds.extents);
		}

		public void SetMinMax(Vector3 min, Vector3 max)
		{
			extents = (max - min) * 0.5f;
			center = min + extents;
		}

		public void Encapsulate(Vector3 point)
		{
			SetMinMax(Vector3.Min(min, point), Vector3.Max(max, point));
		}

		public void Encapsulate(Bounds bounds)
		{
			Encapsulate(bounds.center - bounds.extents);
			Encapsulate(bounds.center + bounds.extents);
		}

		public void Expand(float amount)
		{
			amount *= 0.5f;
			extents += new Vector3(amount, amount, amount);
		}

		public void Expand(Vector3 amount)
		{
			extents += amount * 0.5f;
		}

		public bool Intersects(Bounds bounds)
		{
			return min.x <= bounds.max.x && max.x >= bounds.min.x && min.y <= bounds.max.y && max.y >= bounds.min.y && min.z <= bounds.max.z && max.z >= bounds.min.z;
		}

		private static bool Internal_Contains(Bounds m, Vector3 point)
		{
			return INTERNAL_CALL_Internal_Contains(ref m, ref point);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_Internal_Contains(ref Bounds m, ref Vector3 point);

		public bool Contains(Vector3 point)
		{
			return Internal_Contains(this, point);
		}

		private static float Internal_SqrDistance(Bounds m, Vector3 point)
		{
			return INTERNAL_CALL_Internal_SqrDistance(ref m, ref point);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern float INTERNAL_CALL_Internal_SqrDistance(ref Bounds m, ref Vector3 point);

		public float SqrDistance(Vector3 point)
		{
			return Internal_SqrDistance(this, point);
		}

		private static bool Internal_IntersectRay(ref Ray ray, ref Bounds bounds, out float distance)
		{
			return INTERNAL_CALL_Internal_IntersectRay(ref ray, ref bounds, out distance);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_Internal_IntersectRay(ref Ray ray, ref Bounds bounds, out float distance);

		public bool IntersectRay(Ray ray)
		{
			float distance;
			return Internal_IntersectRay(ref ray, ref this, out distance);
		}

		public bool IntersectRay(Ray ray, out float distance)
		{
			return Internal_IntersectRay(ref ray, ref this, out distance);
		}

		public override string ToString()
		{
			return UnityString.Format("Center: {0}, Extents: {1}", m_Center, m_Extents);
		}

		public string ToString(string format)
		{
			return UnityString.Format("Center: {0}, Extents: {1}", m_Center.ToString(format), m_Extents.ToString(format));
		}

		public static bool operator ==(Bounds lhs, Bounds rhs)
		{
			return lhs.center == rhs.center && lhs.extents == rhs.extents;
		}

		public static bool operator !=(Bounds lhs, Bounds rhs)
		{
			return !(lhs == rhs);
		}
	}
}
