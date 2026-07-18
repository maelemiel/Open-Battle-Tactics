using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public class DaylightTime
	{
		private DateTime m_start;

		private DateTime m_end;

		private TimeSpan m_delta;

		public DateTime Start
		{
			get
			{
				return m_start;
			}
		}

		public DateTime End
		{
			get
			{
				return m_end;
			}
		}

		public TimeSpan Delta
		{
			get
			{
				return m_delta;
			}
		}

		public DaylightTime(DateTime start, DateTime end, TimeSpan delta)
		{
			m_start = start;
			m_end = end;
			m_delta = delta;
		}
	}
}
