using System;
using System.Data.SqlTypes;

namespace Microsoft.SqlServer.Server
{
	public sealed class SqlTriggerContext
	{
		private TriggerAction triggerAction;

		private bool[] columnsUpdated;

		private SqlXml eventData;

		public int ColumnCount
		{
			get
			{
				return (columnsUpdated != null) ? columnsUpdated.Length : 0;
			}
		}

		public SqlXml EventData
		{
			get
			{
				return eventData;
			}
		}

		public TriggerAction TriggerAction
		{
			get
			{
				return triggerAction;
			}
		}

		internal SqlTriggerContext(TriggerAction triggerAction, bool[] columnsUpdated, SqlXml eventData)
		{
			this.triggerAction = triggerAction;
			this.columnsUpdated = columnsUpdated;
			this.eventData = eventData;
		}

		public bool IsUpdatedColumn(int columnOrdinal)
		{
			if (columnsUpdated == null)
			{
				throw new IndexOutOfRangeException("The index specified does not exist");
			}
			return columnsUpdated[columnOrdinal];
		}
	}
}
