namespace UnityEngine
{
	public struct RaycastHit2D
	{
		private Vector2 m_Centroid;

		private Vector2 m_Point;

		private Vector2 m_Normal;

		private float m_Distance;

		private float m_Fraction;

		private Collider2D m_Collider;

		public Vector2 centroid
		{
			get
			{
				return m_Centroid;
			}
			set
			{
				m_Centroid = value;
			}
		}

		public Vector2 point
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

		public Vector2 normal
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

		public float fraction
		{
			get
			{
				return m_Fraction;
			}
			set
			{
				m_Fraction = value;
			}
		}

		public Collider2D collider
		{
			get
			{
				return m_Collider;
			}
		}

		public Rigidbody2D rigidbody
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
				Rigidbody2D rigidbody2D = rigidbody;
				if (rigidbody2D != null)
				{
					return rigidbody2D.transform;
				}
				if (collider != null)
				{
					return collider.transform;
				}
				return null;
			}
		}

		public int CompareTo(RaycastHit2D other)
		{
			if (collider == null)
			{
				return 1;
			}
			if (other.collider == null)
			{
				return -1;
			}
			return fraction.CompareTo(other.fraction);
		}

		public static implicit operator bool(RaycastHit2D hit)
		{
			return hit.collider != null;
		}
	}
}
