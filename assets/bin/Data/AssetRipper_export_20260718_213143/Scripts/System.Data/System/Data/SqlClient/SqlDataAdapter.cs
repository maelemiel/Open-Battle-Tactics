using System.ComponentModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
	[ToolboxItem("Microsoft.VSDesigner.Data.VS.SqlDataAdapterToolboxItem, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[Designer("Microsoft.VSDesigner.Data.VS.SqlDataAdapterDesigner, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[DefaultEvent("RowUpdated")]
	public sealed class SqlDataAdapter : DbDataAdapter, ICloneable, IDataAdapter, IDbDataAdapter
	{
		private int updateBatchSize;

		IDbCommand IDbDataAdapter.SelectCommand
		{
			get
			{
				return SelectCommand;
			}
			set
			{
				SelectCommand = (SqlCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.InsertCommand
		{
			get
			{
				return InsertCommand;
			}
			set
			{
				InsertCommand = (SqlCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.UpdateCommand
		{
			get
			{
				return UpdateCommand;
			}
			set
			{
				UpdateCommand = (SqlCommand)value;
			}
		}

		IDbCommand IDbDataAdapter.DeleteCommand
		{
			get
			{
				return DeleteCommand;
			}
			set
			{
				DeleteCommand = (SqlCommand)value;
			}
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqlCommand DeleteCommand
		{
			get
			{
				return (SqlCommand)base.DeleteCommand;
			}
			set
			{
				base.DeleteCommand = value;
			}
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqlCommand InsertCommand
		{
			get
			{
				return (SqlCommand)base.InsertCommand;
			}
			set
			{
				base.InsertCommand = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[DefaultValue(null)]
		public new SqlCommand SelectCommand
		{
			get
			{
				return (SqlCommand)base.SelectCommand;
			}
			set
			{
				base.SelectCommand = value;
			}
		}

		[DefaultValue(null)]
		[Editor("Microsoft.VSDesigner.Data.Design.DBCommandEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public new SqlCommand UpdateCommand
		{
			get
			{
				return (SqlCommand)base.UpdateCommand;
			}
			set
			{
				base.UpdateCommand = value;
			}
		}

		public override int UpdateBatchSize
		{
			get
			{
				return updateBatchSize;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("UpdateBatchSize");
				}
				updateBatchSize = value;
			}
		}

		public event SqlRowUpdatedEventHandler RowUpdated;

		public event SqlRowUpdatingEventHandler RowUpdating;

		public SqlDataAdapter()
			: this(null)
		{
		}

		public SqlDataAdapter(SqlCommand selectCommand)
		{
			SelectCommand = selectCommand;
			UpdateBatchSize = 1;
		}

		public SqlDataAdapter(string selectCommandText, SqlConnection selectConnection)
			: this(new SqlCommand(selectCommandText, selectConnection))
		{
		}

		public SqlDataAdapter(string selectCommandText, string selectConnectionString)
			: this(selectCommandText, new SqlConnection(selectConnectionString))
		{
		}

		[System.MonoTODO]
		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new SqlRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command, StatementType statementType, DataTableMapping tableMapping)
		{
			return new SqlRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
		}

		protected override void OnRowUpdated(RowUpdatedEventArgs value)
		{
			if (this.RowUpdated != null)
			{
				this.RowUpdated(this, (SqlRowUpdatedEventArgs)value);
			}
		}

		protected override void OnRowUpdating(RowUpdatingEventArgs value)
		{
			if (this.RowUpdating != null)
			{
				this.RowUpdating(this, (SqlRowUpdatingEventArgs)value);
			}
		}

		[System.MonoTODO]
		protected override int AddToBatch(IDbCommand command)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected override void ClearBatch()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected override int ExecuteBatch()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected override IDataParameter GetBatchedParameter(int commandIdentifier, int parameterIndex)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected override void InitializeBatching()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected override void TerminateBatching()
		{
			throw new NotImplementedException();
		}
	}
}
