using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace Mono.Data.Sqlite
{
	public sealed class SqliteCommandBuilder : DbCommandBuilder
	{
		public new SqliteDataAdapter DataAdapter
		{
			get
			{
				return (SqliteDataAdapter)base.DataAdapter;
			}
			set
			{
				base.DataAdapter = value;
			}
		}

		[Browsable(false)]
		public override CatalogLocation CatalogLocation
		{
			get
			{
				return base.CatalogLocation;
			}
			set
			{
				base.CatalogLocation = value;
			}
		}

		[Browsable(false)]
		public override string CatalogSeparator
		{
			get
			{
				return base.CatalogSeparator;
			}
			set
			{
				base.CatalogSeparator = value;
			}
		}

		[Browsable(false)]
		[DefaultValue("[")]
		public override string QuotePrefix
		{
			get
			{
				return base.QuotePrefix;
			}
			set
			{
				base.QuotePrefix = value;
			}
		}

		[Browsable(false)]
		public override string QuoteSuffix
		{
			get
			{
				return base.QuoteSuffix;
			}
			set
			{
				base.QuoteSuffix = value;
			}
		}

		[Browsable(false)]
		public override string SchemaSeparator
		{
			get
			{
				return base.SchemaSeparator;
			}
			set
			{
				base.SchemaSeparator = value;
			}
		}

		public SqliteCommandBuilder()
			: this(null)
		{
		}

		public SqliteCommandBuilder(SqliteDataAdapter adp)
		{
			QuotePrefix = "[";
			QuoteSuffix = "]";
			DataAdapter = adp;
		}

		protected override void ApplyParameterInfo(DbParameter parameter, DataRow row, StatementType statementType, bool whereClause)
		{
			SqliteParameter sqliteParameter = (SqliteParameter)parameter;
			sqliteParameter.DbType = (DbType)(int)row[SchemaTableColumn.ProviderType];
		}

		protected override string GetParameterName(string parameterName)
		{
			return string.Format(CultureInfo.InvariantCulture, "@{0}", parameterName);
		}

		protected override string GetParameterName(int parameterOrdinal)
		{
			return string.Format(CultureInfo.InvariantCulture, "@param{0}", parameterOrdinal);
		}

		protected override string GetParameterPlaceholder(int parameterOrdinal)
		{
			return GetParameterName(parameterOrdinal);
		}

		protected override void SetRowUpdatingHandler(DbDataAdapter adapter)
		{
			if (adapter == base.DataAdapter)
			{
				((SqliteDataAdapter)adapter).RowUpdating -= RowUpdatingEventHandler;
			}
			else
			{
				((SqliteDataAdapter)adapter).RowUpdating += RowUpdatingEventHandler;
			}
		}

		private void RowUpdatingEventHandler(object sender, RowUpdatingEventArgs e)
		{
			RowUpdatingHandler(e);
		}

		public new SqliteCommand GetDeleteCommand()
		{
			return (SqliteCommand)base.GetDeleteCommand();
		}

		public new SqliteCommand GetDeleteCommand(bool useColumnsForParameterNames)
		{
			return (SqliteCommand)base.GetDeleteCommand(useColumnsForParameterNames);
		}

		public new SqliteCommand GetUpdateCommand()
		{
			return (SqliteCommand)base.GetUpdateCommand();
		}

		public new SqliteCommand GetUpdateCommand(bool useColumnsForParameterNames)
		{
			return (SqliteCommand)base.GetUpdateCommand(useColumnsForParameterNames);
		}

		public new SqliteCommand GetInsertCommand()
		{
			return (SqliteCommand)base.GetInsertCommand();
		}

		public new SqliteCommand GetInsertCommand(bool useColumnsForParameterNames)
		{
			return (SqliteCommand)base.GetInsertCommand(useColumnsForParameterNames);
		}

		public override string QuoteIdentifier(string unquotedIdentifier)
		{
			if (string.IsNullOrEmpty(QuotePrefix) || string.IsNullOrEmpty(QuoteSuffix) || string.IsNullOrEmpty(unquotedIdentifier))
			{
				return unquotedIdentifier;
			}
			return QuotePrefix + unquotedIdentifier.Replace(QuoteSuffix, QuoteSuffix + QuoteSuffix) + QuoteSuffix;
		}

		public override string UnquoteIdentifier(string quotedIdentifier)
		{
			if (string.IsNullOrEmpty(QuotePrefix) || string.IsNullOrEmpty(QuoteSuffix) || string.IsNullOrEmpty(quotedIdentifier))
			{
				return quotedIdentifier;
			}
			if (!quotedIdentifier.StartsWith(QuotePrefix, StringComparison.InvariantCultureIgnoreCase) || !quotedIdentifier.EndsWith(QuoteSuffix, StringComparison.InvariantCultureIgnoreCase))
			{
				return quotedIdentifier;
			}
			return quotedIdentifier.Substring(QuotePrefix.Length, quotedIdentifier.Length - (QuotePrefix.Length + QuoteSuffix.Length)).Replace(QuoteSuffix + QuoteSuffix, QuoteSuffix);
		}

		protected override DataTable GetSchemaTable(DbCommand sourceCommand)
		{
			using (IDataReader dataReader = sourceCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
			{
				DataTable schemaTable = dataReader.GetSchemaTable();
				if (HasSchemaPrimaryKey(schemaTable))
				{
					ResetIsUniqueSchemaColumn(schemaTable);
				}
				return schemaTable;
			}
		}

		private bool HasSchemaPrimaryKey(DataTable schema)
		{
			DataColumn column = schema.Columns[SchemaTableColumn.IsKey];
			foreach (DataRow row in schema.Rows)
			{
				if ((bool)row[column])
				{
					return true;
				}
			}
			return false;
		}

		private void ResetIsUniqueSchemaColumn(DataTable schema)
		{
			DataColumn column = schema.Columns[SchemaTableColumn.IsUnique];
			DataColumn column2 = schema.Columns[SchemaTableColumn.IsKey];
			foreach (DataRow row in schema.Rows)
			{
				if (!(bool)row[column2])
				{
					row[column] = false;
				}
			}
			schema.AcceptChanges();
		}
	}
}
