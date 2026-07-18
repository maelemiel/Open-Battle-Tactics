using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public class Collision2D
	{
		internal Rigidbody2D m_Rigidbody;

		internal Collider2D m_Collider;

		internal ContactPoint2D[] m_Contacts;

		internal Vector2 m_RelativeVelocity;

		public Rigidbody2D rigidbody
		{
			get
			{
				return m_Rigidbody;
			}
		}

		public Collider2D collider
		{
			get
			{
				return m_Collider;
			}
		}

		public Transform transform
		{
			get
			{
				return (!(rigidbody != null)) ? collider.transform : rigidbody.transform;
			}
		}

		public GameObject gameObject
		{
			get
			{
				return (!(m_Rigidbody != null)) ? m_Collider.gameObject : m_Rigidbody.gameObject;
			}
		}

		public ContactPoint2D[] contacts
		{
			get
			{
				return m_Contacts;
			}
		}

		public Vector2 relativeVelocity
		{
			get
			{
				return m_RelativeVelocity;
			}
		}
	}
}
