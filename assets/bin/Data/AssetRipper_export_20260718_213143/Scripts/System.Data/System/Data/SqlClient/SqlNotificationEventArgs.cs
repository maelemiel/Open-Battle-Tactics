namespace System.Data.SqlClient
{
	public class SqlNotificationEventArgs : EventArgs
	{
		private SqlNotificationType type;

		private SqlNotificationInfo info;

		private SqlNotificationSource source;

		public SqlNotificationType Type
		{
			get
			{
				return type;
			}
		}

		public SqlNotificationInfo Info
		{
			get
			{
				return info;
			}
		}

		public SqlNotificationSource Source
		{
			get
			{
				return source;
			}
		}

		public SqlNotificationEventArgs(SqlNotificationType type, SqlNotificationInfo info, SqlNotificationSource source)
		{
			this.type = type;
			this.info = info;
			this.source = source;
		}
	}
}
