using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Sql;
using System.Text;
using System.Xml;
using Mono.Data.Tds;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	[DefaultEvent("RecordsAffected")]
	[Designer("Microsoft.VSDesigner.Data.VS.SqlCommandDesigner, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[ToolboxItem("System.Drawing.Design.ToolboxItem, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public sealed class SqlCommand : DbCommand, IDisposable, ICloneable, IDbCommand
	{
		private const int DEFAULT_COMMAND_TIMEOUT = 30;

		private int commandTimeout;

		private bool designTimeVisible;

		private string commandText;

		private CommandType commandType;

		private SqlConnection connection;

		private SqlTransaction transaction;

		private UpdateRowSource updatedRowSource;

		private CommandBehavior behavior;

		private SqlParameterCollection parameters;

		private string preparedStatement;

		private bool disposed;

		private SqlNotificationRequest notification;

		private bool notificationAutoEnlist;

		internal CommandBehavior CommandBehavior
		{
			get
			{
				return behavior;
			}
		}

		[DefaultValue("")]
		[Editor("Microsoft.VSDesigner.Data.SQL.Design.SqlCommandTextEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[RefreshProperties(RefreshProperties.All)]
		public override string CommandText
		{
			get
			{
				if (commandText == null)
				{
					return string.Empty;
				}
				return commandText;
			}
			set
			{
				if (value != commandText && preparedStatement != null)
				{
					Unprepare();
				}
				commandText = value;
			}
		}

		public override int CommandTimeout
		{
			get
			{
				return commandTimeout;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("The property value assigned is less than 0.", "CommandTimeout");
				}
				commandTimeout = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(CommandType.Text)]
		public override CommandType CommandType
		{
			get
			{
				return commandType;
			}
			set
			{
				if (value == CommandType.TableDirect)
				{
					throw new ArgumentOutOfRangeException("CommandType.TableDirect is not supported by the Mono SqlClient Data Provider.");
				}
				ExceptionHelper.CheckEnumValue(typeof(CommandType), value);
				commandType = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DbConnectionEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DefaultValue(null)]
		public new SqlConnection Connection
		{
			get
			{
				return connection;
			}
			set
			{
				connection = value;
			}
		}

		[Browsable(false)]
		[DefaultValue(true)]
		[DesignOnly(true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool DesignTimeVisible
		{
			get
			{
				return designTimeVisible;
			}
			set
			{
				designTimeVisible = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SqlParameterCollection Parameters
		{
			get
			{
				return parameters;
			}
		}

		internal Tds Tds
		{
			get
			{
				return Connection.Tds;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SqlTransaction Transaction
		{
			get
			{
				if (transaction != null && !transaction.IsOpen)
				{
					transaction = null;
				}
				return transaction;
			}
			set
			{
				transaction = value;
			}
		}

		[DefaultValue(UpdateRowSource.Both)]
		public override UpdateRowSource UpdatedRowSource
		{
			get
			{
				return updatedRowSource;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(UpdateRowSource), value);
				updatedRowSource = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SqlNotificationRequest Notification
		{
			get
			{
				return notification;
			}
			set
			{
				notification = value;
			}
		}

		[DefaultValue(true)]
		public bool NotificationAutoEnlist
		{
			get
			{
				return notificationAutoEnlist;
			}
			set
			{
				notificationAutoEnlist = value;
			}
		}

		protected override DbConnection DbConnection
		{
			get
			{
				return Connection;
			}
			set
			{
				Connection = (SqlConnection)value;
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get
			{
				return Parameters;
			}
		}

		protected override DbTransaction DbTransaction
		{
			get
			{
				return Transaction;
			}
			set
			{
				Transaction = (SqlTransaction)value;
			}
		}

		public event StatementCompletedEventHandler StatementCompleted;

		public SqlCommand()
			: this(string.Empty, null, null)
		{
		}

		public SqlCommand(string cmdText)
			: this(cmdText, null, null)
		{
		}

		public SqlCommand(string cmdText, SqlConnection connection)
			: this(cmdText, connection, null)
		{
		}

		public SqlCommand(string cmdText, SqlConnection connection, SqlTransaction transaction)
		{
			commandText = cmdText;
			this.connection = connection;
			this.transaction = transaction;
			commandType = CommandType.Text;
			updatedRowSource = UpdateRowSource.Both;
			commandTimeout = 30;
			notificationAutoEnlist = true;
			designTimeVisible = true;
			parameters = new SqlParameterCollection(this);
		}

		private SqlCommand(string commandText, SqlConnection connection, SqlTransaction transaction, CommandType commandType, UpdateRowSource updatedRowSource, bool designTimeVisible, int commandTimeout, SqlParameterCollection parameters)
		{
			this.commandText = commandText;
			this.connection = connection;
			this.transaction = transaction;
			this.commandType = commandType;
			this.updatedRowSource = updatedRowSource;
			this.designTimeVisible = designTimeVisible;
			this.commandTimeout = commandTimeout;
			this.parameters = new SqlParameterCollection(this);
			for (int i = 0; i < parameters.Count; i++)
			{
				this.parameters.Add(((ICloneable)parameters[i]).Clone());
			}
		}

		object ICloneable.Clone()
		{
			return new SqlCommand(commandText, connection, transaction, commandType, updatedRowSource, designTimeVisible, commandTimeout, parameters);
		}

		public override void Cancel()
		{
			if (Connection != null && Connection.Tds != null)
			{
				Connection.Tds.Cancel();
			}
		}

		public SqlCommand Clone()
		{
			return new SqlCommand(commandText, connection, transaction, commandType, updatedRowSource, designTimeVisible, commandTimeout, parameters);
		}

		internal void CloseDataReader()
		{
			if (Connection != null)
			{
				Connection.DataReader = null;
				if ((behavior & CommandBehavior.CloseConnection) != CommandBehavior.Default)
				{
					Connection.Close();
				}
				if (Tds != null)
				{
					Tds.SequentialAccess = false;
				}
			}
			behavior = CommandBehavior.Default;
		}

		public new SqlParameter CreateParameter()
		{
			return new SqlParameter();
		}

		private string EscapeProcName(string name, bool schema)
		{
			string text = name.Trim();
			int length = text.Length;
			char[] anyOf = new char[2] { '[', ']' };
			bool flag = false;
			int startIndex = 0;
			int length2 = length;
			int num = -1;
			int num2 = -1;
			if (length > 1)
			{
				flag = (((num = text.IndexOf('[')) <= 0) ? true : false);
				if (flag && num > -1)
				{
					num2 = text.IndexOf(']');
					if (num > num2 && num2 != -1)
					{
						flag = false;
					}
					else if (num2 != length - 1)
					{
						flag = ((num2 == -1 && schema) ? true : false);
					}
					else if (text.IndexOfAny(anyOf, 1, length - 2) != -1)
					{
						flag = false;
					}
					else
					{
						startIndex = 1;
						length2 = length - 2;
					}
				}
				if (flag)
				{
					return text.Substring(startIndex, length2);
				}
				throw new ArgumentException(string.Format("SqlCommand.CommandText property value is an invalid multipart name {0}, incorrect usage of quotes", CommandText));
			}
			return text;
		}

		internal void DeriveParameters()
		{
			if (commandType != CommandType.StoredProcedure)
			{
				throw new InvalidOperationException(string.Format("SqlCommand DeriveParameters only supports CommandType.StoredProcedure, not CommandType.{0}", commandType));
			}
			ValidateCommand("DeriveParameters", false);
			string text = CommandText;
			string name = string.Empty;
			int num = text.IndexOf('.');
			if (num >= 0)
			{
				name = text.Substring(0, num);
				text = text.Substring(num + 1);
			}
			text = EscapeProcName(text, false);
			name = EscapeProcName(name, true);
			SqlParameterCollection sqlParameterCollection = new SqlParameterCollection(this);
			sqlParameterCollection.Add("@procedure_name", SqlDbType.NVarChar, text.Length).Value = text;
			if (name.Length > 0)
			{
				sqlParameterCollection.Add("@procedure_schema", SqlDbType.NVarChar, name.Length).Value = name;
			}
			string sql = "sp_procedure_params_rowset";
			try
			{
				Connection.Tds.ExecProc(sql, sqlParameterCollection.MetaParameters, 0, true);
			}
			catch (TdsTimeoutException e)
			{
				Connection.Tds.Reset();
				throw SqlException.FromTdsInternalException(e);
			}
			catch (TdsInternalException e2)
			{
				Connection.Close();
				throw SqlException.FromTdsInternalException(e2);
			}
			SqlDataReader sqlDataReader = new SqlDataReader(this);
			parameters.Clear();
			object[] array = new object[sqlDataReader.FieldCount];
			while (sqlDataReader.Read())
			{
				sqlDataReader.GetValues(array);
				parameters.Add(new SqlParameter(array));
			}
			sqlDataReader.Close();
			if (parameters.Count == 0)
			{
				throw new InvalidOperationException("Stored procedure '" + text + "' does not exist.");
			}
		}

		private void Execute(bool wantResults)
		{
			int num = 0;
			Connection.Tds.RecordsAffected = -1;
			TdsMetaParameterCollection metaParameters = Parameters.MetaParameters;
			foreach (TdsMetaParameter item in (IEnumerable)metaParameters)
			{
				item.Validate(num++);
			}
			if (preparedStatement == null)
			{
				bool flag = (behavior & CommandBehavior.SchemaOnly) > CommandBehavior.Default;
				bool flag2 = (behavior & CommandBehavior.KeyInfo) > CommandBehavior.Default;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (flag || flag2)
				{
					stringBuilder.Append("SET FMTONLY OFF;");
				}
				if (flag2)
				{
					stringBuilder.Append("SET NO_BROWSETABLE ON;");
					stringBuilder2.Append("SET NO_BROWSETABLE OFF;");
				}
				if (flag)
				{
					stringBuilder.Append("SET FMTONLY ON;");
					stringBuilder2.Append("SET FMTONLY OFF;");
				}
				switch (CommandType)
				{
				case CommandType.StoredProcedure:
					try
					{
						if (flag2 || flag)
						{
							Connection.Tds.Execute(stringBuilder.ToString());
						}
						Connection.Tds.ExecProc(CommandText, metaParameters, CommandTimeout, wantResults);
						if (flag2 || flag)
						{
							Connection.Tds.Execute(stringBuilder2.ToString());
						}
						break;
					}
					catch (TdsTimeoutException e3)
					{
						Connection.Tds.Reset();
						throw SqlException.FromTdsInternalException(e3);
					}
					catch (TdsInternalException e4)
					{
						Connection.Close();
						throw SqlException.FromTdsInternalException(e4);
					}
				case CommandType.Text:
				{
					string sql = ((stringBuilder2.Length <= 0) ? string.Format("{0}{1}", stringBuilder.ToString(), CommandText) : string.Format("{0}{1};{2}", stringBuilder.ToString(), CommandText, stringBuilder2.ToString()));
					try
					{
						Connection.Tds.Execute(sql, metaParameters, CommandTimeout, wantResults);
						break;
					}
					catch (TdsTimeoutException e)
					{
						Connection.Tds.Reset();
						throw SqlException.FromTdsInternalException(e);
					}
					catch (TdsInternalException e2)
					{
						Connection.Close();
						throw SqlException.FromTdsInternalException(e2);
					}
				}
				case (CommandType)2:
				case (CommandType)3:
					break;
				}
				return;
			}
			try
			{
				Connection.Tds.ExecPrepared(preparedStatement, metaParameters, CommandTimeout, wantResults);
			}
			catch (TdsTimeoutException e5)
			{
				Connection.Tds.Reset();
				throw SqlException.FromTdsInternalException(e5);
			}
			catch (TdsInternalException e6)
			{
				Connection.Close();
				throw SqlException.FromTdsInternalException(e6);
			}
		}

		public override int ExecuteNonQuery()
		{
			ValidateCommand("ExecuteNonQuery", false);
			int num = 0;
			behavior = CommandBehavior.Default;
			try
			{
				Execute(false);
				num = Connection.Tds.RecordsAffected;
			}
			catch (TdsTimeoutException e)
			{
				Connection.Tds.Reset();
				throw SqlException.FromTdsInternalException(e);
			}
			GetOutputParameters();
			return num;
		}

		public new SqlDataReader ExecuteReader()
		{
			return ExecuteReader(CommandBehavior.Default);
		}

		public new SqlDataReader ExecuteReader(CommandBehavior behavior)
		{
			ValidateCommand("ExecuteReader", false);
			if ((behavior & CommandBehavior.SingleRow) != CommandBehavior.Default)
			{
				behavior |= CommandBehavior.SingleResult;
			}
			this.behavior = behavior;
			if ((behavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default)
			{
				Tds.SequentialAccess = true;
			}
			try
			{
				Execute(true);
				Connection.DataReader = new SqlDataReader(this);
				return Connection.DataReader;
			}
			catch
			{
				if ((behavior & CommandBehavior.CloseConnection) != CommandBehavior.Default)
				{
					Connection.Close();
				}
				throw;
			}
		}

		public override object ExecuteScalar()
		{
			try
			{
				object result = null;
				ValidateCommand("ExecuteScalar", false);
				behavior = CommandBehavior.Default;
				Execute(true);
				try
				{
					if (Connection.Tds.NextResult() && Connection.Tds.NextRow())
					{
						result = Connection.Tds.ColumnValues[0];
					}
					if (commandType == CommandType.StoredProcedure)
					{
						Connection.Tds.SkipToEnd();
						GetOutputParameters();
					}
				}
				catch (TdsTimeoutException e)
				{
					Connection.Tds.Reset();
					throw SqlException.FromTdsInternalException(e);
				}
				catch (TdsInternalException e2)
				{
					Connection.Close();
					throw SqlException.FromTdsInternalException(e2);
				}
				return result;
			}
			finally
			{
				CloseDataReader();
			}
		}

		public XmlReader ExecuteXmlReader()
		{
			ValidateCommand("ExecuteXmlReader", false);
			behavior = CommandBehavior.Default;
			try
			{
				Execute(true);
			}
			catch (TdsTimeoutException e)
			{
				Connection.Tds.Reset();
				throw SqlException.FromTdsInternalException(e);
			}
			SqlDataReader reader = new SqlDataReader(this);
			SqlXmlTextReader input = new SqlXmlTextReader(reader);
			return new XmlTextReader(input);
		}

		internal void GetOutputParameters()
		{
			IList outputParameters = Connection.Tds.OutputParameters;
			if (outputParameters == null || outputParameters.Count <= 0)
			{
				return;
			}
			int num = 0;
			foreach (SqlParameter parameter in parameters)
			{
				if (parameter.Direction != ParameterDirection.Input && parameter.Direction != ParameterDirection.ReturnValue)
				{
					parameter.Value = outputParameters[num];
					num++;
				}
				if (num >= outputParameters.Count)
				{
					break;
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					parameters.Clear();
				}
				base.Dispose(disposing);
				disposed = true;
			}
		}

		public override void Prepare()
		{
			if (Connection == null)
			{
				throw new NullReferenceException();
			}
			if (CommandType == CommandType.StoredProcedure || (CommandType == CommandType.Text && Parameters.Count == 0))
			{
				return;
			}
			ValidateCommand("Prepare", false);
			try
			{
				foreach (SqlParameter parameter in Parameters)
				{
					parameter.CheckIfInitialized();
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("SqlCommand.Prepare requires " + ex.Message);
			}
			preparedStatement = Connection.Tds.Prepare(CommandText, Parameters.MetaParameters);
		}

		public void ResetCommandTimeout()
		{
			commandTimeout = 30;
		}

		private void Unprepare()
		{
			Connection.Tds.Unprepare(preparedStatement);
			preparedStatement = null;
		}

		private void ValidateCommand(string method, bool async)
		{
			if (Connection == null)
			{
				throw new InvalidOperationException(string.Format("{0}: A Connection object is required to continue.", method));
			}
			if (Transaction == null && Connection.Transaction != null)
			{
				throw new InvalidOperationException(string.Format("{0} requires a transaction if the command's connection is in a pending transaction.", method));
			}
			if (Transaction != null && Transaction.Connection != Connection)
			{
				throw new InvalidOperationException("The connection does not have the same transaction as the command.");
			}
			if (Connection.State != ConnectionState.Open)
			{
				throw new InvalidOperationException(string.Format("{0} requires an open connection to continue. This connection is closed.", method));
			}
			if (CommandText.Length == 0)
			{
				throw new InvalidOperationException(string.Format("{0}: CommandText has not been set for this Command.", method));
			}
			if (Connection.DataReader != null)
			{
				throw new InvalidOperationException("There is already an open DataReader associated with this Connection which must be closed first.");
			}
			if (Connection.XmlReader != null)
			{
				throw new InvalidOperationException("There is already an open XmlReader associated with this Connection which must be closed first.");
			}
			if (async && !Connection.AsyncProcessing)
			{
				throw new InvalidOperationException("This Connection object is not in Asynchronous mode. Use 'Asynchronous Processing = true' to set it.");
			}
		}

		protected override DbParameter CreateDbParameter()
		{
			return CreateParameter();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			return ExecuteReader(behavior);
		}

		internal IAsyncResult BeginExecuteInternal(CommandBehavior behavior, bool wantResults, AsyncCallback callback, object state)
		{
			IAsyncResult result = null;
			Connection.Tds.RecordsAffected = -1;
			TdsMetaParameterCollection metaParameters = Parameters.MetaParameters;
			if (preparedStatement == null)
			{
				bool flag = (behavior & CommandBehavior.SchemaOnly) > CommandBehavior.Default;
				bool flag2 = (behavior & CommandBehavior.KeyInfo) > CommandBehavior.Default;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				if (flag || flag2)
				{
					stringBuilder.Append("SET FMTONLY OFF;");
				}
				if (flag2)
				{
					stringBuilder.Append("SET NO_BROWSETABLE ON;");
					stringBuilder2.Append("SET NO_BROWSETABLE OFF;");
				}
				if (flag)
				{
					stringBuilder.Append("SET FMTONLY ON;");
					stringBuilder2.Append("SET FMTONLY OFF;");
				}
				switch (CommandType)
				{
				case CommandType.StoredProcedure:
				{
					string prolog = string.Empty;
					string epilog = string.Empty;
					if (flag2 || flag)
					{
						prolog = stringBuilder.ToString();
					}
					if (flag2 || flag)
					{
						epilog = stringBuilder2.ToString();
					}
					try
					{
						Connection.Tds.BeginExecuteProcedure(prolog, epilog, CommandText, !wantResults, metaParameters, callback, state);
					}
					catch (TdsTimeoutException e3)
					{
						Connection.Tds.Reset();
						throw SqlException.FromTdsInternalException(e3);
					}
					catch (TdsInternalException e4)
					{
						Connection.Close();
						throw SqlException.FromTdsInternalException(e4);
					}
					break;
				}
				case CommandType.Text:
				{
					string sql = string.Format("{0}{1};{2}", stringBuilder.ToString(), CommandText, stringBuilder2.ToString());
					try
					{
						result = ((!wantResults) ? Connection.Tds.BeginExecuteNonQuery(sql, metaParameters, callback, state) : Connection.Tds.BeginExecuteQuery(sql, metaParameters, callback, state));
					}
					catch (TdsTimeoutException e)
					{
						Connection.Tds.Reset();
						throw SqlException.FromTdsInternalException(e);
					}
					catch (TdsInternalException e2)
					{
						Connection.Close();
						throw SqlException.FromTdsInternalException(e2);
					}
					break;
				}
				}
			}
			else
			{
				try
				{
					Connection.Tds.ExecPrepared(preparedStatement, metaParameters, CommandTimeout, wantResults);
				}
				catch (TdsTimeoutException e5)
				{
					Connection.Tds.Reset();
					throw SqlException.FromTdsInternalException(e5);
				}
				catch (TdsInternalException e6)
				{
					Connection.Close();
					throw SqlException.FromTdsInternalException(e6);
				}
			}
			return result;
		}

		internal void EndExecuteInternal(IAsyncResult ar)
		{
			SqlAsyncResult sqlAsyncResult = (SqlAsyncResult)ar;
			Connection.Tds.WaitFor(sqlAsyncResult.InternalResult);
			Connection.Tds.CheckAndThrowException(sqlAsyncResult.InternalResult);
		}

		public IAsyncResult BeginExecuteNonQuery()
		{
			return BeginExecuteNonQuery(null, null);
		}

		public IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, object stateObject)
		{
			ValidateCommand("BeginExecuteNonQuery", true);
			SqlAsyncResult sqlAsyncResult = new SqlAsyncResult(callback, stateObject);
			sqlAsyncResult.EndMethod = "EndExecuteNonQuery";
			sqlAsyncResult.InternalResult = BeginExecuteInternal(CommandBehavior.Default, false, sqlAsyncResult.BubbleCallback, sqlAsyncResult);
			return sqlAsyncResult;
		}

		public int EndExecuteNonQuery(IAsyncResult asyncResult)
		{
			ValidateAsyncResult(asyncResult, "EndExecuteNonQuery");
			EndExecuteInternal(asyncResult);
			int recordsAffected = Connection.Tds.RecordsAffected;
			GetOutputParameters();
			((SqlAsyncResult)asyncResult).Ended = true;
			return recordsAffected;
		}

		public IAsyncResult BeginExecuteReader()
		{
			return BeginExecuteReader(null, null, CommandBehavior.Default);
		}

		public IAsyncResult BeginExecuteReader(CommandBehavior behavior)
		{
			return BeginExecuteReader(null, null, behavior);
		}

		public IAsyncResult BeginExecuteReader(AsyncCallback callback, object stateObject)
		{
			return BeginExecuteReader(callback, stateObject, CommandBehavior.Default);
		}

		public IAsyncResult BeginExecuteReader(AsyncCallback callback, object stateObject, CommandBehavior behavior)
		{
			ValidateCommand("BeginExecuteReader", true);
			this.behavior = behavior;
			SqlAsyncResult sqlAsyncResult = new SqlAsyncResult(callback, stateObject);
			sqlAsyncResult.EndMethod = "EndExecuteReader";
			IAsyncResult internalResult = BeginExecuteInternal(behavior, true, sqlAsyncResult.BubbleCallback, stateObject);
			sqlAsyncResult.InternalResult = internalResult;
			return sqlAsyncResult;
		}

		public SqlDataReader EndExecuteReader(IAsyncResult asyncResult)
		{
			ValidateAsyncResult(asyncResult, "EndExecuteReader");
			EndExecuteInternal(asyncResult);
			SqlDataReader sqlDataReader = null;
			try
			{
				sqlDataReader = new SqlDataReader(this);
			}
			catch (TdsTimeoutException e)
			{
				throw SqlException.FromTdsInternalException(e);
			}
			catch (TdsInternalException e2)
			{
				if ((behavior & CommandBehavior.CloseConnection) != CommandBehavior.Default)
				{
					Connection.Close();
				}
				throw SqlException.FromTdsInternalException(e2);
			}
			((SqlAsyncResult)asyncResult).Ended = true;
			return sqlDataReader;
		}

		public IAsyncResult BeginExecuteXmlReader(AsyncCallback callback, object stateObject)
		{
			ValidateCommand("BeginExecuteXmlReader", true);
			SqlAsyncResult sqlAsyncResult = new SqlAsyncResult(callback, stateObject);
			sqlAsyncResult.EndMethod = "EndExecuteXmlReader";
			sqlAsyncResult.InternalResult = BeginExecuteInternal(behavior, true, sqlAsyncResult.BubbleCallback, stateObject);
			return sqlAsyncResult;
		}

		public IAsyncResult BeginExecuteXmlReader()
		{
			return BeginExecuteXmlReader(null, null);
		}

		public XmlReader EndExecuteXmlReader(IAsyncResult asyncResult)
		{
			ValidateAsyncResult(asyncResult, "EndExecuteXmlReader");
			EndExecuteInternal(asyncResult);
			SqlDataReader reader = new SqlDataReader(this);
			SqlXmlTextReader input = new SqlXmlTextReader(reader);
			XmlReader result = new XmlTextReader(input);
			((SqlAsyncResult)asyncResult).Ended = true;
			return result;
		}

		internal void ValidateAsyncResult(IAsyncResult ar, string endMethod)
		{
			if (ar == null)
			{
				throw new ArgumentException("result passed is null!");
			}
			if (!(ar is SqlAsyncResult))
			{
				throw new ArgumentException(string.Format("cannot test validity of types {0}", ar.GetType()));
			}
			SqlAsyncResult sqlAsyncResult = (SqlAsyncResult)ar;
			if (sqlAsyncResult.EndMethod != endMethod)
			{
				throw new InvalidOperationException(string.Format("Mismatched {0} called for AsyncResult. Expected call to {1} but {0} is called instead.", endMethod, sqlAsyncResult.EndMethod));
			}
			if (sqlAsyncResult.Ended)
			{
				throw new InvalidOperationException(string.Format("The method {0} cannot be called more than once for the same AsyncResult.", endMethod));
			}
		}
	}
}
