namespace UnityEngine
{
	public struct JointMotor2D
	{
		private float m_MotorSpeed;

		private float m_MaximumMotorTorque;

		public float motorSpeed
		{
			get
			{
				return m_MotorSpeed;
			}
			set
			{
				m_MotorSpeed = value;
			}
		}

		public float maxMotorTorque
		{
			get
			{
				return m_MaximumMotorTorque;
			}
			set
			{
				m_MaximumMotorTorque = value;
			}
		}
	}
}
