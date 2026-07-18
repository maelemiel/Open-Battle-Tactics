namespace UnityEngine
{
	public struct Plane
	{
		private Vector3 m_Normal;

		private float m_Distance;

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

		public Plane(Vector3 inNormal, Vector3 inPoint)
		{
			m_Normal = Vector3.Normalize(inNormal);
			m_Distance = 0f - Vector3.Dot(inNormal, inPoint);
		}

		public Plane(Vector3 inNormal, float d)
		{
			m_Normal = Vector3.Normalize(inNormal);
			m_Distance = d;
		}

		public Plane(Vector3 a, Vector3 b, Vector3 c)
		{
			m_Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
			m_Distance = 0f - Vector3.Dot(m_Normal, a);
		}

		public void SetNormalAndPosition(Vector3 inNormal, Vector3 inPoint)
		{
			normal = Vector3.Normalize(inNormal);
			distance = 0f - Vector3.Dot(inNormal, inPoint);
		}

		public void Set3Points(Vector3 a, Vector3 b, Vector3 c)
		{
			normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
			distance = 0f - Vector3.Dot(normal, a);
		}

		public float GetDistanceToPoint(Vector3 inPt)
		{
			return Vector3.Dot(normal, inPt) + distance;
		}

		public bool GetSide(Vector3 inPt)
		{
			return Vector3.Dot(normal, inPt) + distance > 0f;
		}

		public bool SameSide(Vector3 inPt0, Vector3 inPt1)
		{
			float distanceToPoint = GetDistanceToPoint(inPt0);
			float distanceToPoint2 = GetDistanceToPoint(inPt1);
			if (distanceToPoint > 0f && distanceToPoint2 > 0f)
			{
				return true;
			}
			if (distanceToPoint <= 0f && distanceToPoint2 <= 0f)
			{
				return true;
			}
			return false;
		}

		public bool Raycast(Ray ray, out float enter)
		{
			float num = Vector3.Dot(ray.direction, normal);
			float num2 = 0f - Vector3.Dot(ray.origin, normal) - distance;
			if (Mathf.Approximately(num, 0f))
			{
				enter = 0f;
				return false;
			}
			enter = num2 / num;
			return enter > 0f;
		}
	}
}
