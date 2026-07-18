namespace UnityEngine
{
	public struct LocationInfo
	{
		private double m_Timestamp;

		private float m_Latitude;

		private float m_Longitude;

		private float m_Altitude;

		private float m_HorizontalAccuracy;

		private float m_VerticalAccuracy;

		public float latitude
		{
			get
			{
				return m_Latitude;
			}
		}

		public float longitude
		{
			get
			{
				return m_Longitude;
			}
		}

		public float altitude
		{
			get
			{
				return m_Altitude;
			}
		}

		public float horizontalAccuracy
		{
			get
			{
				return m_HorizontalAccuracy;
			}
		}

		public float verticalAccuracy
		{
			get
			{
				return m_VerticalAccuracy;
			}
		}

		public double timestamp
		{
			get
			{
				return m_Timestamp;
			}
		}
	}
}
