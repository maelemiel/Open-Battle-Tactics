using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class ControllerColliderHit
	{
		internal CharacterController m_Controller;

		internal Collider m_Collider;

		internal Vector3 m_Point;

		internal Vector3 m_Normal;

		internal Vector3 m_MoveDirection;

		internal float m_MoveLength;

		internal int m_Push;

		public CharacterController controller
		{
			get
			{
				return m_Controller;
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
				return m_Collider.attachedRigidbody;
			}
		}

		public GameObject gameObject
		{
			get
			{
				return m_Collider.gameObject;
			}
		}

		public Transform transform
		{
			get
			{
				return m_Collider.transform;
			}
		}

		public Vector3 point
		{
			get
			{
				return m_Point;
			}
		}

		public Vector3 normal
		{
			get
			{
				return m_Normal;
			}
		}

		public Vector3 moveDirection
		{
			get
			{
				return m_MoveDirection;
			}
		}

		public float moveLength
		{
			get
			{
				return m_MoveLength;
			}
		}

		private bool push
		{
			get
			{
				return m_Push != 0;
			}
			set
			{
				m_Push = (value ? 1 : 0);
			}
		}
	}
}
