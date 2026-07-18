using System.Collections;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	public sealed class SqlInfoMessageEventArgs : EventArgs
	{
		private SqlErrorCollection errors = new SqlErrorCollection();

		public SqlErrorCollection Errors
		{
			get
			{
				return errors;
			}
		}

		public string Message
		{
			get
			{
				return errors[0].Message;
			}
		}

		public string Source
		{
			get
			{
				return errors[0].Source;
			}
		}

		internal SqlInfoMessageEventArgs(TdsInternalErrorCollection tdsErrors)
		{
			foreach (TdsInternalError item in (IEnumerable)tdsErrors)
			{
				errors.Add(item.Class, item.LineNumber, item.Message, item.Number, item.Procedure, item.Server, "Mono SqlClient Data Provider", item.State);
			}
		}

		public override string ToString()
		{
			return Message;
		}
	}
}
