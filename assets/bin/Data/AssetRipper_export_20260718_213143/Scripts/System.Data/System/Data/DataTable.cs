using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Mono.Data.SqlExpressions;

namespace System.Data
{
	[Serializable]
	[XmlSchemaProvider("GetDataTableSchema")]
	[DefaultEvent("RowChanging")]
	[DesignTimeVisible(false)]
	[DefaultProperty("TableName")]
	[Editor("Microsoft.VSDesigner.Data.Design.DataTableEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ToolboxItem(false)]
	public class DataTable : MarshalByValueComponent, IXmlSerializable, ISerializable, IListSource, ISupportInitialize, ISupportInitializeNotification
	{
		internal DataSet dataSet;

		private bool _caseSensitive;

		private DataColumnCollection _columnCollection;

		private ConstraintCollection _constraintCollection;

		private DataView _defaultView;

		private string _displayExpression;

		private PropertyCollection _extendedProperties;

		private bool _hasErrors;

		private CultureInfo _locale;

		private int _minimumCapacity;

		private string _nameSpace;

		private DataRelationCollection _childRelations;

		private DataRelationCollection _parentRelations;

		private string _prefix;

		private UniqueConstraint _primaryKeyConstraint;

		private DataRowCollection _rows;

		private ISite _site;

		private string _tableName;

		private bool _containsListCollection;

		private string _encodedTableName;

		internal bool _duringDataLoad;

		internal bool _nullConstraintViolationDuringDataLoad;

		private bool dataSetPrevEnforceConstraints;

		private bool dataTablePrevEnforceConstraints;

		private bool enforceConstraints = true;

		private DataRowBuilder _rowBuilder;

		private ArrayList _indexes;

		private RecordCache _recordCache;

		private int _defaultValuesRowIndex = -1;

		protected internal bool fInitInProgress;

		private bool _virginCaseSensitive = true;

		private PropertyDescriptorCollection _propertyDescriptorsCache;

		private static DataColumn[] _emptyColumnArray = new DataColumn[0];

		private static Regex SortRegex = new Regex("^((\\[(?<ColName>.+)\\])|(?<ColName>\\S+))([ ]+(?<Order>ASC|DESC))?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

		private DataColumn[] _latestPrimaryKeyCols;

		private DataRow[] empty_rows;

		private bool tableInitialized = true;

		private SerializationFormat remotingFormat;

		bool IListSource.ContainsListCollection
		{
			get
			{
				return false;
			}
		}

		public bool CaseSensitive
		{
			get
			{
				if (_virginCaseSensitive && dataSet != null)
				{
					return dataSet.CaseSensitive;
				}
				return _caseSensitive;
			}
			set
			{
				if (_childRelations.Count > 0 || _parentRelations.Count > 0)
				{
					throw new ArgumentException("Cannot change CaseSensitive or Locale property. This change would lead to at least one DataRelation or Constraint to have different Locale or CaseSensitive settings between its related tables.");
				}
				_virginCaseSensitive = false;
				_caseSensitive = value;
				ResetCaseSensitiveIndexes();
			}
		}

		internal ArrayList Indexes
		{
			get
			{
				return _indexes;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataRelationCollection ChildRelations
		{
			get
			{
				return _childRelations;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DataCategory("Data")]
		public DataColumnCollection Columns
		{
			get
			{
				return _columnCollection;
			}
		}

		[DataCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ConstraintCollection Constraints
		{
			get
			{
				return _constraintCollection;
			}
			internal set
			{
				_constraintCollection = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataSet DataSet
		{
			get
			{
				return dataSet;
			}
		}

		[Browsable(false)]
		public DataView DefaultView
		{
			get
			{
				if (_defaultView == null)
				{
					lock (this)
					{
						if (_defaultView == null)
						{
							if (dataSet != null)
							{
								_defaultView = dataSet.DefaultViewManager.CreateDataView(this);
							}
							else
							{
								_defaultView = new DataView(this);
							}
						}
					}
				}
				return _defaultView;
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public string DisplayExpression
		{
			get
			{
				return (_displayExpression != null) ? _displayExpression : string.Empty;
			}
			set
			{
				_displayExpression = value;
			}
		}

		[Browsable(false)]
		[DataCategory("Data")]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				return _extendedProperties;
			}
		}

		[Browsable(false)]
		public bool HasErrors
		{
			get
			{
				for (int i = 0; i < _rows.Count; i++)
				{
					if (_rows[i].HasErrors)
					{
						return true;
					}
				}
				return false;
			}
		}

		public CultureInfo Locale
		{
			get
			{
				if (_locale != null)
				{
					return _locale;
				}
				if (DataSet != null)
				{
					return DataSet.Locale;
				}
				return CultureInfo.CurrentCulture;
			}
			set
			{
				if (_childRelations.Count > 0 || _parentRelations.Count > 0)
				{
					throw new ArgumentException("Cannot change CaseSensitive or Locale property. This change would lead to at least one DataRelation or Constraint to have different Locale or CaseSensitive settings between its related tables.");
				}
				if (_locale == null || !_locale.Equals(value))
				{
					_locale = value;
				}
			}
		}

		internal bool LocaleSpecified
		{
			get
			{
				return _locale != null;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(50)]
		public int MinimumCapacity
		{
			get
			{
				return _minimumCapacity;
			}
			set
			{
				_minimumCapacity = value;
			}
		}

		[DataCategory("Data")]
		public string Namespace
		{
			get
			{
				if (_nameSpace != null)
				{
					return _nameSpace;
				}
				if (DataSet != null)
				{
					return DataSet.Namespace;
				}
				return string.Empty;
			}
			set
			{
				_nameSpace = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DataRelationCollection ParentRelations
		{
			get
			{
				return _parentRelations;
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public string Prefix
		{
			get
			{
				return (_prefix != null) ? _prefix : string.Empty;
			}
			set
			{
				for (int i = 0; i < value.Length; i++)
				{
					if (!char.IsLetterOrDigit(value[i]) && value[i] != '_' && value[i] != ':')
					{
						throw new DataException("Prefix '" + value + "' is not valid, because it contains special characters.");
					}
				}
				_prefix = value;
			}
		}

		[Editor("Microsoft.VSDesigner.Data.Design.PrimaryKeyEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		[TypeConverter("System.Data.PrimaryKeyTypeConverter, System.Data, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		[DataCategory("Data")]
		public DataColumn[] PrimaryKey
		{
			get
			{
				if (_primaryKeyConstraint == null)
				{
					return new DataColumn[0];
				}
				return _primaryKeyConstraint.Columns;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					if (_primaryKeyConstraint != null)
					{
						_primaryKeyConstraint.SetIsPrimaryKey(false);
						Constraints.Remove(_primaryKeyConstraint);
						_primaryKeyConstraint = null;
					}
				}
				else if (InitInProgress)
				{
					_latestPrimaryKeyCols = value;
				}
				else
				{
					if (_primaryKeyConstraint != null && DataColumn.AreColumnSetsTheSame(value, _primaryKeyConstraint.Columns))
					{
						return;
					}
					UniqueConstraint uniqueConstraint = UniqueConstraint.GetUniqueConstraintForColumnSet(Constraints, value);
					if (uniqueConstraint == null)
					{
						foreach (DataColumn dataColumn in value)
						{
							if (dataColumn.Table == null)
							{
								break;
							}
							if (Columns.IndexOf(dataColumn) < 0)
							{
								throw new ArgumentException("PrimaryKey columns do not belong to this table.");
							}
						}
						uniqueConstraint = new UniqueConstraint(value, false);
						Constraints.Add(uniqueConstraint);
					}
					if (_primaryKeyConstraint != null)
					{
						_primaryKeyConstraint.SetIsPrimaryKey(false);
						Constraints.Remove(_primaryKeyConstraint);
						_primaryKeyConstraint = null;
					}
					UniqueConstraint.SetAsPrimaryKey(Constraints, uniqueConstraint);
					_primaryKeyConstraint = uniqueConstraint;
					for (int j = 0; j < uniqueConstraint.Columns.Length; j++)
					{
						uniqueConstraint.Columns[j].AllowDBNull = false;
					}
				}
			}
		}

		internal UniqueConstraint PrimaryKeyConstraint
		{
			get
			{
				return _primaryKeyConstraint;
			}
		}

		[Browsable(false)]
		public DataRowCollection Rows
		{
			get
			{
				return _rows;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public override ISite Site
		{
			get
			{
				return _site;
			}
			set
			{
				_site = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DataCategory("Data")]
		[DefaultValue("")]
		public string TableName
		{
			get
			{
				return (_tableName != null) ? _tableName : string.Empty;
			}
			set
			{
				_tableName = value;
			}
		}

		internal RecordCache RecordCache
		{
			get
			{
				return _recordCache;
			}
		}

		private DataRowBuilder RowBuilder
		{
			get
			{
				if (_rowBuilder == null)
				{
					_rowBuilder = new DataRowBuilder(this, -1, 0);
				}
				else
				{
					_rowBuilder._rowId = -1;
				}
				return _rowBuilder;
			}
		}

		internal bool EnforceConstraints
		{
			get
			{
				return enforceConstraints;
			}
			set
			{
				if (value == enforceConstraints)
				{
					return;
				}
				if (value)
				{
					ResetIndexes();
					foreach (Constraint constraint in Constraints)
					{
						constraint.AssertConstraint();
					}
					AssertNotNullConstraints();
					if (HasErrors)
					{
						Constraint.ThrowConstraintException();
					}
				}
				enforceConstraints = value;
			}
		}

		internal bool InitInProgress
		{
			get
			{
				return fInitInProgress;
			}
			set
			{
				fInitInProgress = value;
			}
		}

		internal int DefaultValuesRowIndex
		{
			get
			{
				return _defaultValuesRowIndex;
			}
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				return tableInitialized;
			}
		}

		[DefaultValue(SerializationFormat.Xml)]
		public SerializationFormat RemotingFormat
		{
			get
			{
				if (dataSet != null)
				{
					remotingFormat = dataSet.RemotingFormat;
				}
				return remotingFormat;
			}
			set
			{
				if (dataSet != null)
				{
					throw new ArgumentException("Cannot have different remoting format property value for DataSet and DataTable");
				}
				remotingFormat = value;
			}
		}

		[DataCategory("Data")]
		public event DataColumnChangeEventHandler ColumnChanged;

		[DataCategory("Data")]
		public event DataColumnChangeEventHandler ColumnChanging;

		[DataCategory("Data")]
		public event DataRowChangeEventHandler RowChanged;

		[DataCategory("Data")]
		public event DataRowChangeEventHandler RowChanging;

		[DataCategory("Data")]
		public event DataRowChangeEventHandler RowDeleted;

		[DataCategory("Data")]
		public event DataRowChangeEventHandler RowDeleting;

		public event EventHandler Initialized;

		[DataCategory("Data")]
		public event DataTableClearEventHandler TableCleared;

		[DataCategory("Data")]
		public event DataTableClearEventHandler TableClearing;

		public event DataTableNewRowEventHandler TableNewRow;

		public DataTable()
		{
			dataSet = null;
			_columnCollection = new DataColumnCollection(this);
			_constraintCollection = new ConstraintCollection(this);
			_extendedProperties = new PropertyCollection();
			_tableName = string.Empty;
			_nameSpace = null;
			_caseSensitive = false;
			_displayExpression = null;
			_primaryKeyConstraint = null;
			_site = null;
			_rows = new DataRowCollection(this);
			_indexes = new ArrayList();
			_recordCache = new RecordCache(this);
			_minimumCapacity = 50;
			_childRelations = new DataRelationCollection.DataTableRelationCollection(this);
			_parentRelations = new DataRelationCollection.DataTableRelationCollection(this);
		}

		public DataTable(string tableName)
			: this()
		{
			_tableName = tableName;
		}

		protected DataTable(SerializationInfo info, StreamingContext context)
			: this()
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			SerializationFormat serializationFormat = SerializationFormat.Xml;
			while (enumerator.MoveNext())
			{
				if (enumerator.ObjectType == typeof(SerializationFormat))
				{
					serializationFormat = (SerializationFormat)(int)enumerator.Value;
					break;
				}
			}
			if (serializationFormat == SerializationFormat.Xml)
			{
				string s = info.GetString("XmlSchema");
				string s2 = info.GetString("XmlDiffGram");
				DataSet dataSet = new DataSet();
				dataSet.ReadXmlSchema(new StringReader(s));
				dataSet.Tables[0].CopyProperties(this);
				dataSet = new DataSet();
				dataSet.Tables.Add(this);
				dataSet.ReadXml(new StringReader(s2), XmlReadMode.DiffGram);
				dataSet.Tables.Remove(this);
			}
			else
			{
				BinaryDeserializeTable(info);
			}
		}

		public DataTable(string tableName, string tbNamespace)
			: this(tableName)
		{
			_nameSpace = tbNamespace;
		}

		IList IListSource.GetList()
		{
			return DefaultView;
		}

		[System.MonoNotSupported("")]
		XmlSchema IXmlSerializable.GetSchema()
		{
			return GetSchema();
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			ReadXml_internal(reader, true);
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			DataSet dataSet = this.dataSet;
			bool flag = true;
			if (this.dataSet == null)
			{
				dataSet = new DataSet();
				dataSet.Tables.Add(this);
				flag = false;
			}
			XmlSchemaWriter.WriteXmlSchema(writer, new DataTable[1] { this }, null, TableName, dataSet.DataSetName, LocaleSpecified ? Locale : ((!dataSet.LocaleSpecified) ? null : dataSet.Locale));
			dataSet.WriteIndividualTableContent(writer, this, XmlWriteMode.DiffGram);
			writer.Flush();
			if (!flag)
			{
				this.dataSet.Tables.Remove(this);
			}
		}

		internal void ChangedDataColumn(DataRow dr, DataColumn dc, object pv)
		{
			DataColumnChangeEventArgs e = new DataColumnChangeEventArgs(dr, dc, pv);
			OnColumnChanged(e);
		}

		internal void ChangingDataColumn(DataRow dr, DataColumn dc, object pv)
		{
			DataColumnChangeEventArgs e = new DataColumnChangeEventArgs(dr, dc, pv);
			OnColumnChanging(e);
		}

		internal void DeletedDataRow(DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs(dr, action);
			OnRowDeleted(e);
		}

		internal void DeletingDataRow(DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs(dr, action);
			OnRowDeleting(e);
		}

		internal void ChangedDataRow(DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs(dr, action);
			OnRowChanged(e);
		}

		internal void ChangingDataRow(DataRow dr, DataRowAction action)
		{
			DataRowChangeEventArgs e = new DataRowChangeEventArgs(dr, action);
			OnRowChanging(e);
		}

		internal void AssertNotNullConstraints()
		{
			if (_duringDataLoad && !_nullConstraintViolationDuringDataLoad)
			{
				return;
			}
			bool nullConstraintViolationDuringDataLoad = false;
			for (int i = 0; i < Columns.Count; i++)
			{
				DataColumn dataColumn = Columns[i];
				if (dataColumn.AllowDBNull)
				{
					continue;
				}
				for (int j = 0; j < Rows.Count; j++)
				{
					if (Rows[j].HasVersion(DataRowVersion.Default) && Rows[j].IsNull(dataColumn))
					{
						nullConstraintViolationDuringDataLoad = true;
						string text = string.Format("Column '{0}' does not allow DBNull.Value.", dataColumn.ColumnName);
						Rows[j].SetColumnError(i, text);
						Rows[j].RowError = text;
					}
				}
			}
			_nullConstraintViolationDuringDataLoad = nullConstraintViolationDuringDataLoad;
		}

		internal bool RowsExist(DataColumn[] columns, DataColumn[] relatedColumns, DataRow row)
		{
			int from_index = row.IndexFromVersion(DataRowVersion.Default);
			int num = RecordCache.NewRecord();
			try
			{
				for (int i = 0; i < relatedColumns.Length; i++)
				{
					columns[i].DataContainer.CopyValue(relatedColumns[i].DataContainer, from_index, num);
				}
				return RowsExist(columns, num);
			}
			finally
			{
				RecordCache.DisposeRecord(num);
			}
		}

		private bool RowsExist(DataColumn[] columns, int index)
		{
			Index index2 = FindIndex(columns);
			if (index2 != null)
			{
				return index2.Find(index) != -1;
			}
			foreach (DataRow row in Rows)
			{
				if (row.RowState == DataRowState.Deleted)
				{
					continue;
				}
				int index3 = row.IndexFromVersion((row.RowState != DataRowState.Modified) ? DataRowVersion.Current : DataRowVersion.Original);
				bool flag = true;
				foreach (DataColumn dataColumn in columns)
				{
					if (dataColumn.DataContainer.CompareValues(index3, index) != 0)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				return true;
			}
			return false;
		}

		public void AcceptChanges()
		{
			int num = 0;
			while (num < Rows.Count)
			{
				DataRow dataRow = Rows[num];
				dataRow.AcceptChanges();
				if (dataRow.RowState != DataRowState.Detached)
				{
					num++;
				}
			}
			_rows.OnListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
		}

		public virtual void BeginInit()
		{
			InitInProgress = true;
			tableInitialized = false;
		}

		public void BeginLoadData()
		{
			if (!_duringDataLoad)
			{
				_duringDataLoad = true;
				_nullConstraintViolationDuringDataLoad = false;
				if (dataSet != null)
				{
					dataSetPrevEnforceConstraints = dataSet.EnforceConstraints;
					dataSet.EnforceConstraints = false;
				}
				else
				{
					EnforceConstraints = false;
				}
			}
		}

		public void Clear()
		{
			_rows.Clear();
		}

		public virtual DataTable Clone()
		{
			DataTable dataTable = (DataTable)Activator.CreateInstance(GetType(), true);
			CopyProperties(dataTable);
			return dataTable;
		}

		public object Compute(string expression, string filter)
		{
			DataRow[] array = Select(filter);
			if (array == null || array.Length == 0)
			{
				return DBNull.Value;
			}
			Parser parser = new Parser(array);
			IExpression expression2 = parser.Compile(expression);
			return expression2.Eval(array[0]);
		}

		public DataTable Copy()
		{
			DataTable dataTable = Clone();
			dataTable._duringDataLoad = true;
			foreach (DataRow row in Rows)
			{
				DataRow dataRow = dataTable.NewNotInitializedRow();
				dataTable.Rows.AddInternal(dataRow);
				CopyRow(row, dataRow);
			}
			dataTable._duringDataLoad = false;
			dataTable.ResetIndexes();
			return dataTable;
		}

		internal void CopyRow(DataRow fromRow, DataRow toRow)
		{
			if (fromRow.HasErrors)
			{
				fromRow.CopyErrors(toRow);
			}
			if (fromRow.HasVersion(DataRowVersion.Original))
			{
				toRow.Original = toRow.Table.RecordCache.CopyRecord(this, fromRow.Original, -1);
			}
			if (fromRow.HasVersion(DataRowVersion.Current))
			{
				if (fromRow.Original != fromRow.Current)
				{
					toRow.Current = toRow.Table.RecordCache.CopyRecord(this, fromRow.Current, -1);
				}
				else
				{
					toRow.Current = toRow.Original;
				}
			}
		}

		private void CopyProperties(DataTable Copy)
		{
			Copy.CaseSensitive = CaseSensitive;
			Copy._virginCaseSensitive = _virginCaseSensitive;
			Copy.DisplayExpression = DisplayExpression;
			if (ExtendedProperties.Count > 0)
			{
				Array array = Array.CreateInstance(typeof(object), ExtendedProperties.Count);
				ExtendedProperties.Keys.CopyTo(array, 0);
				for (int i = 0; i < ExtendedProperties.Count; i++)
				{
					Copy.ExtendedProperties.Add(array.GetValue(i), ExtendedProperties[array.GetValue(i)]);
				}
			}
			Copy._locale = _locale;
			Copy.MinimumCapacity = MinimumCapacity;
			Copy.Namespace = Namespace;
			Copy.Prefix = Prefix;
			Copy.Site = Site;
			Copy.TableName = TableName;
			bool flag = Copy.Columns.Count == 0;
			foreach (DataColumn column in Columns)
			{
				if (flag || !Copy.Columns.Contains(column.ColumnName))
				{
					Copy.Columns.Add(column.Clone());
				}
			}
			CopyConstraints(Copy);
			if (PrimaryKey.Length > 0)
			{
				DataColumn[] array2 = new DataColumn[PrimaryKey.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = Copy.Columns[PrimaryKey[j].ColumnName];
				}
				Copy.PrimaryKey = array2;
			}
		}

		private void CopyConstraints(DataTable copy)
		{
			for (int i = 0; i < Constraints.Count; i++)
			{
				if (Constraints[i] is UniqueConstraint && !copy.Constraints.Contains(Constraints[i].ConstraintName))
				{
					UniqueConstraint uniqueConstraint = (UniqueConstraint)Constraints[i];
					DataColumn[] array = new DataColumn[uniqueConstraint.Columns.Length];
					for (int j = 0; j < array.Length; j++)
					{
						array[j] = copy.Columns[uniqueConstraint.Columns[j].ColumnName];
					}
					UniqueConstraint constraint = new UniqueConstraint(uniqueConstraint.ConstraintName, array, uniqueConstraint.IsPrimaryKey);
					copy.Constraints.Add(constraint);
				}
			}
		}

		public virtual void EndInit()
		{
			InitInProgress = false;
			DataTableInitialized();
			FinishInit();
		}

		internal void FinishInit()
		{
			UniqueConstraint primaryKeyConstraint = _primaryKeyConstraint;
			Columns.PostAddRange();
			_constraintCollection.PostAddRange();
			if (_primaryKeyConstraint == primaryKeyConstraint)
			{
				PrimaryKey = _latestPrimaryKeyCols;
			}
		}

		public void EndLoadData()
		{
			if (_duringDataLoad)
			{
				if (dataSet != null)
				{
					dataSet.InternalEnforceConstraints(dataSetPrevEnforceConstraints, true);
				}
				else
				{
					EnforceConstraints = true;
				}
				_duringDataLoad = false;
			}
		}

		public DataTable GetChanges()
		{
			return GetChanges(DataRowState.Added | DataRowState.Deleted | DataRowState.Modified);
		}

		public DataTable GetChanges(DataRowState rowStates)
		{
			DataTable dataTable = null;
			foreach (DataRow row in Rows)
			{
				if (row.IsRowChanged(rowStates))
				{
					if (dataTable == null)
					{
						dataTable = Clone();
					}
					DataRow dataRow2 = dataTable.NewNotInitializedRow();
					row.CopyValuesToRow(dataRow2);
					dataRow2.XmlRowID = row.XmlRowID;
					dataTable.Rows.AddInternal(dataRow2);
				}
			}
			return dataTable;
		}

		public DataRow[] GetErrors()
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < _rows.Count; i++)
			{
				if (_rows[i].HasErrors)
				{
					arrayList.Add(_rows[i]);
				}
			}
			DataRow[] array = NewRowArray(arrayList.Count);
			arrayList.CopyTo(array, 0);
			return array;
		}

		protected virtual DataTable CreateInstance()
		{
			return Activator.CreateInstance(GetType(), true) as DataTable;
		}

		protected virtual Type GetRowType()
		{
			return typeof(DataRow);
		}

		public void ImportRow(DataRow row)
		{
			if (row.RowState == DataRowState.Detached)
			{
				return;
			}
			DataRow dataRow = NewNotInitializedRow();
			int num = -1;
			if (row.HasVersion(DataRowVersion.Original))
			{
				num = row.IndexFromVersion(DataRowVersion.Original);
				dataRow.Original = RecordCache.NewRecord();
				RecordCache.CopyRecord(row.Table, num, dataRow.Original);
			}
			if (row.HasVersion(DataRowVersion.Current))
			{
				int num2 = row.IndexFromVersion(DataRowVersion.Current);
				if (num2 == num)
				{
					dataRow.Current = dataRow.Original;
				}
				else
				{
					dataRow.Current = RecordCache.NewRecord();
					RecordCache.CopyRecord(row.Table, num2, dataRow.Current);
				}
			}
			if (row.RowState != DataRowState.Deleted)
			{
				dataRow.Validate();
			}
			else
			{
				AddRowToIndexes(dataRow);
			}
			Rows.AddInternal(dataRow);
			if (row.HasErrors)
			{
				row.CopyErrors(dataRow);
			}
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (RemotingFormat == SerializationFormat.Xml)
			{
				DataSet dataSet;
				if (this.dataSet != null)
				{
					dataSet = this.dataSet;
				}
				else
				{
					dataSet = new DataSet("tmpDataSet");
					dataSet.Tables.Add(this);
				}
				StringWriter stringWriter = new StringWriter();
				XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
				xmlTextWriter.Formatting = Formatting.Indented;
				dataSet.WriteIndividualTableContent(xmlTextWriter, this, XmlWriteMode.DiffGram);
				xmlTextWriter.Close();
				StringWriter stringWriter2 = new StringWriter();
				DataTableCollection dataTableCollection = new DataTableCollection(dataSet);
				dataTableCollection.Add(this);
				XmlSchemaWriter.WriteXmlSchema(dataSet, new XmlTextWriter(stringWriter2), dataTableCollection, null);
				stringWriter2.Close();
				info.AddValue("XmlSchema", stringWriter2.ToString(), typeof(string));
				info.AddValue("XmlDiffGram", stringWriter.ToString(), typeof(string));
				return;
			}
			BinarySerializeProperty(info);
			if (this.dataSet == null)
			{
				for (int i = 0; i < Columns.Count; i++)
				{
					info.AddValue("DataTable.DataColumn_" + i + ".Expression", Columns[i].Expression);
				}
				BinarySerialize(info, "DataTable_0.");
			}
		}

		public DataRow LoadDataRow(object[] values, bool fAcceptChanges)
		{
			DataRow dataRow = null;
			if (PrimaryKey.Length == 0)
			{
				dataRow = Rows.Add(values);
			}
			else
			{
				EnsureDefaultValueRowIndex();
				int num = CreateRecord(values);
				int num2 = _primaryKeyConstraint.Index.Find(num);
				if (num2 < 0)
				{
					dataRow = NewRowFromBuilder(RowBuilder);
					dataRow.Proposed = num;
					Rows.AddInternal(dataRow);
					if (!_duringDataLoad)
					{
						AddRowToIndexes(dataRow);
					}
				}
				else
				{
					dataRow = RecordCache[num2];
					dataRow.BeginEdit();
					dataRow.ImportRecord(num);
					dataRow.EndEdit();
				}
			}
			if (fAcceptChanges)
			{
				dataRow.AcceptChanges();
			}
			return dataRow;
		}

		internal DataRow LoadDataRow(IDataRecord record, int[] mapping, int length, bool fAcceptChanges)
		{
			DataRow dataRow = null;
			int num = RecordCache.NewRecord();
			try
			{
				RecordCache.ReadIDataRecord(num, record, mapping, length);
				if (PrimaryKey.Length != 0)
				{
					bool flag = true;
					DataColumn[] primaryKey = PrimaryKey;
					foreach (DataColumn dataColumn in primaryKey)
					{
						if (dataColumn.Ordinal >= mapping.Length)
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						int num2 = _primaryKeyConstraint.Index.Find(num);
						if (num2 != -1)
						{
							dataRow = RecordCache[num2];
						}
					}
				}
				if (dataRow == null)
				{
					dataRow = NewNotInitializedRow();
					dataRow.Proposed = num;
					Rows.AddInternal(dataRow);
				}
				else
				{
					dataRow.BeginEdit();
					dataRow.ImportRecord(num);
					dataRow.EndEdit();
				}
				if (fAcceptChanges)
				{
					dataRow.AcceptChanges();
				}
			}
			catch
			{
				RecordCache.DisposeRecord(num);
				throw;
			}
			return dataRow;
		}

		public DataRow NewRow()
		{
			EnsureDefaultValueRowIndex();
			DataRow dataRow = NewRowFromBuilder(RowBuilder);
			dataRow.Proposed = CreateRecord(null);
			NewRowAdded(dataRow);
			return dataRow;
		}

		internal int CreateRecord(object[] values)
		{
			int num = ((values != null) ? values.Length : 0);
			if (num > Columns.Count)
			{
				throw new ArgumentException("Input array is longer than the number of columns in this table.");
			}
			int num2 = RecordCache.NewRecord();
			try
			{
				for (int i = 0; i < num; i++)
				{
					object obj = values[i];
					if (obj == null)
					{
						Columns[i].SetDefaultValue(num2);
					}
					else
					{
						Columns[i][num2] = values[i];
					}
				}
				for (int j = num; j < Columns.Count; j++)
				{
					Columns[j].SetDefaultValue(num2);
				}
				return num2;
			}
			catch
			{
				RecordCache.DisposeRecord(num2);
				throw;
			}
		}

		private void EnsureDefaultValueRowIndex()
		{
			if (_defaultValuesRowIndex == -1)
			{
				_defaultValuesRowIndex = RecordCache.NewRecord();
				for (int i = 0; i < Columns.Count; i++)
				{
					DataColumn dataColumn = Columns[i];
					dataColumn.DataContainer[_defaultValuesRowIndex] = dataColumn.DefaultValue;
				}
			}
		}

		protected internal DataRow[] NewRowArray(int size)
		{
			if (size == 0 && empty_rows != null)
			{
				return empty_rows;
			}
			Type rowType = GetRowType();
			DataRow[] result = ((rowType != typeof(DataRow)) ? ((DataRow[])Array.CreateInstance(rowType, size)) : new DataRow[size]);
			if (size == 0)
			{
				empty_rows = result;
			}
			return result;
		}

		protected virtual DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new DataRow(builder);
		}

		internal DataRow NewNotInitializedRow()
		{
			EnsureDefaultValueRowIndex();
			return NewRowFromBuilder(RowBuilder);
		}

		public void RejectChanges()
		{
			for (int num = _rows.Count - 1; num >= 0; num--)
			{
				DataRow dataRow = _rows[num];
				if (dataRow.RowState != DataRowState.Unchanged)
				{
					_rows[num].RejectChanges();
				}
			}
		}

		public virtual void Reset()
		{
			Clear();
			while (ParentRelations.Count > 0)
			{
				if (dataSet.Relations.Contains(ParentRelations[ParentRelations.Count - 1].RelationName))
				{
					dataSet.Relations.Remove(ParentRelations[ParentRelations.Count - 1]);
				}
			}
			while (ChildRelations.Count > 0)
			{
				if (dataSet.Relations.Contains(ChildRelations[ChildRelations.Count - 1].RelationName))
				{
					dataSet.Relations.Remove(ChildRelations[ChildRelations.Count - 1]);
				}
			}
			Constraints.Clear();
			Columns.Clear();
		}

		public DataRow[] Select()
		{
			return Select(string.Empty, string.Empty, DataViewRowState.CurrentRows);
		}

		public DataRow[] Select(string filterExpression)
		{
			return Select(filterExpression, string.Empty, DataViewRowState.CurrentRows);
		}

		public DataRow[] Select(string filterExpression, string sort)
		{
			return Select(filterExpression, sort, DataViewRowState.CurrentRows);
		}

		public DataRow[] Select(string filterExpression, string sort, DataViewRowState recordStates)
		{
			if (filterExpression == null)
			{
				filterExpression = string.Empty;
			}
			IExpression expression = null;
			if (filterExpression != string.Empty)
			{
				Parser parser = new Parser();
				expression = parser.Compile(filterExpression);
			}
			DataColumn[] array = _emptyColumnArray;
			ListSortDirection[] sortDirections = null;
			if (sort != null && !sort.Equals(string.Empty))
			{
				array = ParseSortString(this, sort, out sortDirections, false);
			}
			if (Rows.Count == 0)
			{
				return NewRowArray(0);
			}
			if (array.Length == 0 && expression != null)
			{
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < Columns.Count; i++)
				{
					if (expression.DependsOn(Columns[i]))
					{
						arrayList.Add(Columns[i]);
					}
				}
				array = (DataColumn[])arrayList.ToArray(typeof(DataColumn));
			}
			bool addIndex = true;
			if (filterExpression != string.Empty)
			{
				addIndex = false;
			}
			Index index = GetIndex(array, sortDirections, recordStates, expression, false, addIndex);
			int[] all = index.GetAll();
			DataRow[] array2 = NewRowArray(index.Size);
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = RecordCache[all[j]];
			}
			return array2;
		}

		private void AddIndex(Index index)
		{
			if (_indexes == null)
			{
				_indexes = new ArrayList();
			}
			_indexes.Add(index);
		}

		internal Index GetIndex(DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter, bool reset)
		{
			return GetIndex(columns, sort, rowState, filter, reset, true);
		}

		internal Index GetIndex(DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter, bool reset, bool addIndex)
		{
			Index index = FindIndex(columns, sort, rowState, filter);
			if (index == null)
			{
				index = new Index(new Key(this, columns, sort, rowState, filter));
				if (addIndex)
				{
					AddIndex(index);
				}
			}
			else if (reset)
			{
				index.Reset();
			}
			return index;
		}

		internal Index FindIndex(DataColumn[] columns)
		{
			return FindIndex(columns, null, DataViewRowState.None, null);
		}

		internal Index FindIndex(DataColumn[] columns, ListSortDirection[] sort, DataViewRowState rowState, IExpression filter)
		{
			if (Indexes != null)
			{
				foreach (Index index in Indexes)
				{
					if (index.Key.Equals(columns, sort, rowState, filter))
					{
						return index;
					}
				}
			}
			return null;
		}

		internal void ResetIndexes()
		{
			foreach (Index index in Indexes)
			{
				index.Reset();
			}
		}

		internal void ResetCaseSensitiveIndexes()
		{
			foreach (Index index in Indexes)
			{
				bool flag = false;
				DataColumn[] columns = index.Key.Columns;
				foreach (DataColumn dataColumn in columns)
				{
					if (dataColumn.DataType == typeof(string))
					{
						flag = true;
						break;
					}
				}
				if (!flag && index.Key.HasFilter)
				{
					foreach (DataColumn column in Columns)
					{
						if (column.DataType == DbTypes.TypeOfString && index.Key.DependsOn(column))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					index.Reset();
				}
			}
		}

		internal void DropIndex(Index index)
		{
			if (index != null && index.RefCount == 0)
			{
				_indexes.Remove(index);
			}
		}

		internal void DropReferencedIndexes(DataColumn column)
		{
			if (_indexes == null)
			{
				return;
			}
			for (int num = _indexes.Count - 1; num >= 0; num--)
			{
				Index index = (Index)_indexes[num];
				if (index.Key.DependsOn(column))
				{
					_indexes.Remove(index);
				}
			}
		}

		internal void AddRowToIndexes(DataRow row)
		{
			if (_indexes != null)
			{
				for (int i = 0; i < _indexes.Count; i++)
				{
					((Index)_indexes[i]).Add(row);
				}
			}
		}

		internal void DeleteRowFromIndexes(DataRow row)
		{
			if (_indexes == null)
			{
				return;
			}
			foreach (Index index in _indexes)
			{
				index.Delete(row);
			}
		}

		public override string ToString()
		{
			string text = TableName;
			if (DisplayExpression != null && DisplayExpression != string.Empty)
			{
				text = text + " + " + DisplayExpression;
			}
			return text;
		}

		protected virtual void OnColumnChanged(DataColumnChangeEventArgs e)
		{
			if (this.ColumnChanged != null)
			{
				this.ColumnChanged(this, e);
			}
		}

		internal void RaiseOnColumnChanged(DataColumnChangeEventArgs e)
		{
			OnColumnChanged(e);
		}

		protected virtual void OnColumnChanging(DataColumnChangeEventArgs e)
		{
			if (this.ColumnChanging != null)
			{
				this.ColumnChanging(this, e);
			}
		}

		internal void RaiseOnColumnChanging(DataColumnChangeEventArgs e)
		{
			OnColumnChanging(e);
		}

		[System.MonoTODO]
		protected internal virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			throw new NotImplementedException();
		}

		protected internal virtual void OnRemoveColumn(DataColumn column)
		{
			DropReferencedIndexes(column);
		}

		protected virtual void OnRowChanged(DataRowChangeEventArgs e)
		{
			if (this.RowChanged != null)
			{
				this.RowChanged(this, e);
			}
		}

		protected virtual void OnRowChanging(DataRowChangeEventArgs e)
		{
			if (this.RowChanging != null)
			{
				this.RowChanging(this, e);
			}
		}

		protected virtual void OnRowDeleted(DataRowChangeEventArgs e)
		{
			if (this.RowDeleted != null)
			{
				this.RowDeleted(this, e);
			}
		}

		protected virtual void OnRowDeleting(DataRowChangeEventArgs e)
		{
			if (this.RowDeleting != null)
			{
				this.RowDeleting(this, e);
			}
		}

		internal static DataColumn[] ParseSortString(DataTable table, string sort, out ListSortDirection[] sortDirections, bool rejectNoResult)
		{
			DataColumn[] array = _emptyColumnArray;
			sortDirections = null;
			ArrayList arrayList = null;
			ArrayList arrayList2 = null;
			if (sort != null && !sort.Equals(string.Empty))
			{
				arrayList = new ArrayList();
				arrayList2 = new ArrayList();
				string[] array2 = sort.Trim().Split(',');
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i].Trim();
					Match match = SortRegex.Match(text);
					Group obj = match.Groups["ColName"];
					if (!obj.Success)
					{
						throw new IndexOutOfRangeException("Could not find column: " + text);
					}
					string value = obj.Value;
					DataColumn dataColumn = table.Columns[value];
					if (dataColumn == null)
					{
						try
						{
							dataColumn = table.Columns[int.Parse(value)];
						}
						catch (FormatException)
						{
							throw new IndexOutOfRangeException("Cannot find column " + value);
						}
					}
					arrayList.Add(dataColumn);
					obj = match.Groups["Order"];
					if (!obj.Success || string.Compare(obj.Value, "ASC", true, CultureInfo.InvariantCulture) == 0)
					{
						arrayList2.Add(ListSortDirection.Ascending);
					}
					else
					{
						arrayList2.Add(ListSortDirection.Descending);
					}
				}
				array = (DataColumn[])arrayList.ToArray(typeof(DataColumn));
				sortDirections = new ListSortDirection[arrayList2.Count];
				for (int j = 0; j < sortDirections.Length; j++)
				{
					sortDirections[j] = (ListSortDirection)(int)arrayList2[j];
				}
			}
			if (rejectNoResult)
			{
				if (array == null)
				{
					throw new SystemException("sort expression result is null");
				}
				if (array.Length == 0)
				{
					throw new SystemException("sort expression result is 0");
				}
			}
			return array;
		}

		private void UpdatePropertyDescriptorsCache()
		{
			PropertyDescriptor[] array = new PropertyDescriptor[Columns.Count + ChildRelations.Count];
			int num = 0;
			foreach (DataColumn column in Columns)
			{
				array[num++] = new DataColumnPropertyDescriptor(column);
			}
			foreach (DataRelation childRelation in ChildRelations)
			{
				array[num++] = new DataRelationPropertyDescriptor(childRelation);
			}
			_propertyDescriptorsCache = new PropertyDescriptorCollection(array);
		}

		internal PropertyDescriptorCollection GetPropertyDescriptorCollection()
		{
			if (_propertyDescriptorsCache == null)
			{
				UpdatePropertyDescriptorsCache();
			}
			return _propertyDescriptorsCache;
		}

		internal void ResetPropertyDescriptorsCache()
		{
			_propertyDescriptorsCache = null;
		}

		internal void SetRowsID()
		{
			int num = 0;
			foreach (DataRow row in Rows)
			{
				row.XmlRowID = num;
				num++;
			}
		}

		[System.MonoTODO]
		protected virtual XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		public static XmlSchemaComplexType GetDataTableSchema(XmlSchemaSet schemaSet)
		{
			return new XmlSchemaComplexType();
		}

		public XmlReadMode ReadXml(Stream stream)
		{
			return ReadXml(new XmlTextReader(stream, null));
		}

		public XmlReadMode ReadXml(string fileName)
		{
			XmlReader xmlReader = new XmlTextReader(fileName);
			try
			{
				return ReadXml(xmlReader);
			}
			finally
			{
				xmlReader.Close();
			}
		}

		public XmlReadMode ReadXml(TextReader reader)
		{
			return ReadXml(new XmlTextReader(reader));
		}

		public XmlReadMode ReadXml(XmlReader reader)
		{
			return ReadXml_internal(reader, false);
		}

		public XmlReadMode ReadXml_internal(XmlReader reader, bool serializable)
		{
			bool flag = true;
			bool flag2 = false;
			XmlReadMode result = XmlReadMode.ReadSchema;
			DataSet dataSet = null;
			DataSet dataSet2 = new DataSet();
			reader.MoveToContent();
			if ((Columns.Count > 0 && reader.LocalName != "diffgram") || serializable)
			{
				result = dataSet2.ReadXml(reader);
			}
			else
			{
				if (Columns.Count > 0 && reader.LocalName == "diffgram")
				{
					try
					{
						if (TableName == string.Empty)
						{
							flag2 = true;
						}
						if (DataSet == null)
						{
							flag = false;
							dataSet2.Tables.Add(this);
							result = dataSet2.ReadXml(reader);
						}
						else
						{
							result = DataSet.ReadXml(reader);
						}
					}
					catch (DataException)
					{
						result = XmlReadMode.DiffGram;
						if (flag2)
						{
							TableName = string.Empty;
						}
					}
					finally
					{
						if (!flag)
						{
							dataSet2.Tables.Remove(this);
						}
					}
					return result;
				}
				result = dataSet2.ReadXml(reader, XmlReadMode.ReadSchema);
			}
			if (result == XmlReadMode.InferSchema)
			{
				result = XmlReadMode.IgnoreSchema;
			}
			if (DataSet == null)
			{
				flag = false;
				dataSet = new DataSet();
				if (TableName == string.Empty)
				{
					flag2 = true;
				}
				dataSet.Tables.Add(this);
			}
			DenyXmlResolving(this, dataSet2, result, flag2, flag);
			if (Columns.Count > 0 && TableName != dataSet2.Tables[0].TableName)
			{
				if (!flag)
				{
					dataSet.Tables.Remove(this);
				}
				if (flag2 && !flag)
				{
					TableName = string.Empty;
				}
				return result;
			}
			TableName = dataSet2.Tables[0].TableName;
			if (!flag)
			{
				if (Columns.Count > 0)
				{
					dataSet.Merge(dataSet2, true, MissingSchemaAction.Ignore);
				}
				else
				{
					dataSet.Merge(dataSet2, true, MissingSchemaAction.AddWithKey);
				}
				if (ChildRelations.Count == 0)
				{
					dataSet.Tables.Remove(this);
				}
				else
				{
					dataSet.DataSetName = dataSet2.DataSetName;
				}
			}
			else if (Columns.Count > 0)
			{
				DataSet.Merge(dataSet2, true, MissingSchemaAction.Ignore);
			}
			else
			{
				DataSet.Merge(dataSet2, true, MissingSchemaAction.AddWithKey);
			}
			return result;
		}

		private void DenyXmlResolving(DataTable table, DataSet ds, XmlReadMode mode, bool isTableNameBlank, bool isPartOfDataSet)
		{
			if (ds.Tables.Count == 0 && table.Columns.Count == 0)
			{
				throw new InvalidOperationException("DataTable does not support schema inference from XML");
			}
			if (table.Columns.Count == 0 && ds.Tables[0].TableName != table.TableName && !isTableNameBlank)
			{
				throw new ArgumentException(string.Format("DataTable '{0}' does not match to any DataTable in source", table.TableName));
			}
			if (table.Columns.Count > 0 && ds.Tables[0].TableName != table.TableName && !isTableNameBlank && mode == XmlReadMode.ReadSchema && !isPartOfDataSet)
			{
				throw new ArgumentException(string.Format("DataTable '{0}' does not match to any DataTable in source", table.TableName));
			}
			if (isPartOfDataSet && table.Columns.Count > 0 && mode == XmlReadMode.ReadSchema && table.TableName != ds.Tables[0].TableName)
			{
				throw new ArgumentException(string.Format("DataTable '{0}' does not match to any DataTable in source", table.TableName));
			}
		}

		public void ReadXmlSchema(Stream stream)
		{
			ReadXmlSchema(new XmlTextReader(stream));
		}

		public void ReadXmlSchema(TextReader reader)
		{
			ReadXmlSchema(new XmlTextReader(reader));
		}

		public void ReadXmlSchema(string fileName)
		{
			XmlTextReader xmlTextReader = null;
			try
			{
				xmlTextReader = new XmlTextReader(fileName);
				ReadXmlSchema(xmlTextReader);
			}
			finally
			{
				if (xmlTextReader != null)
				{
					xmlTextReader.Close();
				}
			}
		}

		public void ReadXmlSchema(XmlReader reader)
		{
			if (Columns.Count > 0)
			{
				return;
			}
			DataSet dataSet = new DataSet();
			new XmlSchemaDataImporter(dataSet, reader, false).Process();
			DataTable dataTable = null;
			if (TableName == string.Empty)
			{
				if (dataSet.Tables.Count > 0)
				{
					dataTable = dataSet.Tables[0];
				}
			}
			else
			{
				dataTable = dataSet.Tables[TableName];
				if (dataTable == null)
				{
					throw new ArgumentException(string.Format("DataTable '{0}' does not match to any DataTable in source.", TableName));
				}
			}
			if (dataTable != null)
			{
				dataTable.CopyProperties(this);
			}
		}

		[System.MonoNotSupported("")]
		protected virtual void ReadXmlSerializable(XmlReader reader)
		{
			throw new NotImplementedException();
		}

		private XmlWriterSettings GetWriterSettings()
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.OmitXmlDeclaration = true;
			return xmlWriterSettings;
		}

		public void WriteXml(Stream stream)
		{
			WriteXml(stream, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(TextWriter writer)
		{
			WriteXml(writer, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(XmlWriter writer)
		{
			WriteXml(writer, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(string fileName)
		{
			WriteXml(fileName, XmlWriteMode.IgnoreSchema, false);
		}

		public void WriteXml(Stream stream, XmlWriteMode mode)
		{
			WriteXml(stream, mode, false);
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode)
		{
			WriteXml(writer, mode, false);
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode)
		{
			WriteXml(writer, mode, false);
		}

		public void WriteXml(string fileName, XmlWriteMode mode)
		{
			WriteXml(fileName, mode, false);
		}

		public void WriteXml(Stream stream, bool writeHierarchy)
		{
			WriteXml(stream, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(string fileName, bool writeHierarchy)
		{
			WriteXml(fileName, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(TextWriter writer, bool writeHierarchy)
		{
			WriteXml(writer, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(XmlWriter writer, bool writeHierarchy)
		{
			WriteXml(writer, XmlWriteMode.IgnoreSchema, writeHierarchy);
		}

		public void WriteXml(Stream stream, XmlWriteMode mode, bool writeHierarchy)
		{
			WriteXml(XmlWriter.Create(stream, GetWriterSettings()), mode, writeHierarchy);
		}

		public void WriteXml(string fileName, XmlWriteMode mode, bool writeHierarchy)
		{
			XmlWriter xmlWriter = null;
			try
			{
				xmlWriter = XmlWriter.Create(fileName, GetWriterSettings());
				WriteXml(xmlWriter, mode, writeHierarchy);
			}
			finally
			{
				if (xmlWriter != null)
				{
					xmlWriter.Close();
				}
			}
		}

		public void WriteXml(TextWriter writer, XmlWriteMode mode, bool writeHierarchy)
		{
			WriteXml(XmlWriter.Create(writer, GetWriterSettings()), mode, writeHierarchy);
		}

		public void WriteXml(XmlWriter writer, XmlWriteMode mode, bool writeHierarchy)
		{
			List<DataTable> list = new List<DataTable>();
			if (!writeHierarchy)
			{
				list.Add(this);
			}
			else
			{
				FindAllChildren(list, this);
			}
			List<DataRelation> list2 = new List<DataRelation>();
			if (DataSet != null)
			{
				foreach (DataRelation relation in DataSet.Relations)
				{
					if (list.Contains(relation.ParentTable) && list.Contains(relation.ChildTable))
					{
						list2.Add(relation);
					}
				}
			}
			string mainDataTable = null;
			if (mode == XmlWriteMode.WriteSchema)
			{
				mainDataTable = TableName;
			}
			string text = null;
			text = ((DataSet != null) ? DataSet.DataSetName : ((DataSet != null || mode != XmlWriteMode.WriteSchema) ? "DocumentElement" : "NewDataSet"));
			XmlTableWriter.WriteTables(writer, mode, list, list2, mainDataTable, text);
		}

		private void FindAllChildren(List<DataTable> list, DataTable root)
		{
			if (list.Contains(root))
			{
				return;
			}
			list.Add(root);
			foreach (DataRelation childRelation in root.ChildRelations)
			{
				FindAllChildren(list, childRelation.ChildTable);
			}
		}

		public void WriteXmlSchema(Stream stream)
		{
			if (TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlWriterSettings writerSettings = GetWriterSettings();
			writerSettings.OmitXmlDeclaration = false;
			WriteXmlSchema(XmlWriter.Create(stream, writerSettings));
		}

		public void WriteXmlSchema(TextWriter writer)
		{
			if (TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlWriterSettings writerSettings = GetWriterSettings();
			writerSettings.OmitXmlDeclaration = false;
			WriteXmlSchema(XmlWriter.Create(writer, writerSettings));
		}

		public void WriteXmlSchema(XmlWriter writer)
		{
			WriteXmlSchema(writer, false);
		}

		public void WriteXmlSchema(string fileName)
		{
			if (fileName == string.Empty)
			{
				throw new ArgumentException("Empty path name is not legal.");
			}
			if (TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlTextWriter xmlTextWriter = null;
			try
			{
				XmlWriterSettings writerSettings = GetWriterSettings();
				writerSettings.OmitXmlDeclaration = false;
				xmlTextWriter = new XmlTextWriter(fileName, null);
				WriteXmlSchema(xmlTextWriter);
			}
			finally
			{
				if (xmlTextWriter != null)
				{
					xmlTextWriter.Close();
				}
			}
		}

		public void WriteXmlSchema(Stream stream, bool writeHierarchy)
		{
			if (TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlWriterSettings writerSettings = GetWriterSettings();
			writerSettings.OmitXmlDeclaration = false;
			WriteXmlSchema(XmlWriter.Create(stream, writerSettings), writeHierarchy);
		}

		public void WriteXmlSchema(TextWriter writer, bool writeHierarchy)
		{
			if (TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlWriterSettings writerSettings = GetWriterSettings();
			writerSettings.OmitXmlDeclaration = false;
			WriteXmlSchema(XmlWriter.Create(writer, writerSettings), writeHierarchy);
		}

		public void WriteXmlSchema(XmlWriter writer, bool writeHierarchy)
		{
			if (TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			DataSet dataSet = DataSet;
			DataSet dataSet2 = null;
			try
			{
				if (dataSet == null)
				{
					dataSet2 = (dataSet = new DataSet());
					dataSet.Tables.Add(this);
				}
				writer.WriteStartDocument();
				DataTable[] array = null;
				DataRelation[] array2 = null;
				if (!writeHierarchy || ChildRelations.Count <= 0)
				{
					array = new DataTable[1] { this };
				}
				else
				{
					array2 = new DataRelation[ChildRelations.Count];
					for (int i = 0; i < ChildRelations.Count; i++)
					{
						array2[i] = ChildRelations[i];
					}
					array = new DataTable[dataSet.Tables.Count];
					for (int j = 0; j < dataSet.Tables.Count; j++)
					{
						array[j] = dataSet.Tables[j];
					}
				}
				string mainDataTable = ((!(dataSet.Namespace == string.Empty)) ? (dataSet.Namespace + "_x003A_" + TableName) : TableName);
				XmlSchemaWriter.WriteXmlSchema(writer, array, array2, mainDataTable, dataSet.DataSetName, LocaleSpecified ? Locale : ((!dataSet.LocaleSpecified) ? null : dataSet.Locale));
			}
			finally
			{
				if (dataSet2 != null)
				{
					dataSet.Tables.Remove(this);
				}
			}
		}

		public void WriteXmlSchema(string fileName, bool writeHierarchy)
		{
			if (fileName == string.Empty)
			{
				throw new ArgumentException("Empty path name is not legal.");
			}
			if (TableName == string.Empty)
			{
				throw new InvalidOperationException("Cannot serialize the DataTable. DataTable name is not set.");
			}
			XmlTextWriter xmlTextWriter = null;
			try
			{
				XmlWriterSettings writerSettings = GetWriterSettings();
				writerSettings.OmitXmlDeclaration = false;
				xmlTextWriter = new XmlTextWriter(fileName, null);
				WriteXmlSchema(xmlTextWriter, writeHierarchy);
			}
			finally
			{
				if (xmlTextWriter != null)
				{
					xmlTextWriter.Close();
				}
			}
		}

		private void OnTableInitialized(EventArgs e)
		{
			if (this.Initialized != null)
			{
				this.Initialized(this, e);
			}
		}

		private void DataTableInitialized()
		{
			tableInitialized = true;
			OnTableInitialized(new EventArgs());
		}

		internal void DeserializeConstraints(ArrayList arrayList)
		{
			bool flag = false;
			for (int i = 0; i < arrayList.Count; i++)
			{
				ArrayList arrayList2 = arrayList[i] as ArrayList;
				if (arrayList2 == null)
				{
					continue;
				}
				if ((string)arrayList2[0] == "F")
				{
					int[] array = arrayList2[2] as int[];
					if (array == null)
					{
						continue;
					}
					ArrayList arrayList3 = new ArrayList();
					DataTable dataTable = dataSet.Tables[array[0]];
					for (int j = 0; j < array.Length - 1; j++)
					{
						arrayList3.Add(dataTable.Columns[array[j + 1]]);
					}
					array = arrayList2[3] as int[];
					if (array != null)
					{
						ArrayList arrayList4 = new ArrayList();
						dataTable = dataSet.Tables[array[0]];
						for (int k = 0; k < array.Length - 1; k++)
						{
							arrayList4.Add(dataTable.Columns[array[k + 1]]);
						}
						ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint((string)arrayList2[1], (DataColumn[])arrayList3.ToArray(typeof(DataColumn)), (DataColumn[])arrayList4.ToArray(typeof(DataColumn)));
						Array array2 = (Array)arrayList2[4];
						foreignKeyConstraint.AcceptRejectRule = (AcceptRejectRule)(int)array2.GetValue(0);
						foreignKeyConstraint.UpdateRule = (Rule)(int)array2.GetValue(1);
						foreignKeyConstraint.DeleteRule = (Rule)(int)array2.GetValue(2);
						foreignKeyConstraint.SetExtendedProperties((PropertyCollection)arrayList2[5]);
						Constraints.Add(foreignKeyConstraint);
						flag = true;
					}
				}
				else if (!flag && (string)arrayList2[0] == "U")
				{
					UniqueConstraint uniqueConstraint = null;
					ArrayList arrayList5 = new ArrayList();
					int[] array3 = arrayList2[2] as int[];
					if (array3 != null)
					{
						for (int l = 0; l < array3.Length; l++)
						{
							arrayList5.Add(Columns[array3[l]]);
						}
						uniqueConstraint = new UniqueConstraint((string)arrayList2[1], (DataColumn[])arrayList5.ToArray(typeof(DataColumn)), (bool)arrayList2[3]);
						if (Constraints.IndexOf(uniqueConstraint) == -1 && Constraints.IndexOf((string)arrayList2[1]) == -1)
						{
							uniqueConstraint.SetExtendedProperties((PropertyCollection)arrayList2[4]);
							Constraints.Add(uniqueConstraint);
						}
					}
				}
				else
				{
					flag = false;
				}
			}
		}

		private DataRowState GetCurrentRowState(BitArray rowStateBitArray, int i)
		{
			if (!rowStateBitArray[i] && !rowStateBitArray[i + 1] && rowStateBitArray[i + 2])
			{
				return DataRowState.Detached;
			}
			if (!rowStateBitArray[i] && !rowStateBitArray[i + 1] && !rowStateBitArray[i + 2])
			{
				return DataRowState.Unchanged;
			}
			if (!rowStateBitArray[i] && rowStateBitArray[i + 1] && !rowStateBitArray[i + 2])
			{
				return DataRowState.Added;
			}
			if (rowStateBitArray[i] && rowStateBitArray[i + 1] && !rowStateBitArray[i + 2])
			{
				return DataRowState.Deleted;
			}
			return DataRowState.Modified;
		}

		internal void DeserializeRecords(ArrayList arrayList, ArrayList nullBits, BitArray rowStateBitArray)
		{
			BitArray bitArray = null;
			if (arrayList == null || arrayList.Count < 1)
			{
				return;
			}
			int length = ((Array)arrayList[0]).Length;
			object[] array = new object[arrayList.Count];
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				DataRowState currentRowState = GetCurrentRowState(rowStateBitArray, num * 3);
				for (int j = 0; j < arrayList.Count; j++)
				{
					Array array2 = (Array)arrayList[j];
					bitArray = (BitArray)nullBits[j];
					if (!bitArray[i])
					{
						array[j] = array2.GetValue(i);
					}
					else
					{
						array[j] = null;
					}
				}
				LoadDataRow(array, false);
				switch (currentRowState)
				{
				case DataRowState.Modified:
				{
					Rows[num].AcceptChanges();
					i++;
					for (int k = 0; k < arrayList.Count; k++)
					{
						Array array3 = (Array)arrayList[k];
						bitArray = (BitArray)nullBits[k];
						if (!bitArray[i])
						{
							Rows[num][k] = array3.GetValue(i);
						}
						else
						{
							Rows[num][k] = null;
						}
					}
					break;
				}
				case DataRowState.Unchanged:
					Rows[num].AcceptChanges();
					break;
				case DataRowState.Deleted:
					Rows[num].AcceptChanges();
					Rows[num].Delete();
					break;
				}
				num++;
			}
		}

		private void BinaryDeserializeTable(SerializationInfo info)
		{
			ArrayList arrayList = null;
			TableName = info.GetString("DataTable.TableName");
			Namespace = info.GetString("DataTable.Namespace");
			Prefix = info.GetString("DataTable.Prefix");
			CaseSensitive = info.GetBoolean("DataTable.CaseSensitive");
			Locale = new CultureInfo(info.GetInt32("DataTable.LocaleLCID"));
			_extendedProperties = (PropertyCollection)info.GetValue("DataTable.ExtendedProperties", typeof(PropertyCollection));
			MinimumCapacity = info.GetInt32("DataTable.MinimumCapacity");
			int @int = info.GetInt32("DataTable.Columns.Count");
			for (int i = 0; i < @int; i++)
			{
				Columns.Add();
				string text = "DataTable.DataColumn_" + i + ".";
				Columns[i].ColumnName = info.GetString(text + "ColumnName");
				Columns[i].Namespace = info.GetString(text + "Namespace");
				Columns[i].Caption = info.GetString(text + "Caption");
				Columns[i].Prefix = info.GetString(text + "Prefix");
				Columns[i].DataType = (Type)info.GetValue(text + "DataType", typeof(Type));
				Columns[i].DefaultValue = info.GetValue(text + "DefaultValue", typeof(object));
				Columns[i].AllowDBNull = info.GetBoolean(text + "AllowDBNull");
				Columns[i].AutoIncrement = info.GetBoolean(text + "AutoIncrement");
				Columns[i].AutoIncrementStep = info.GetInt64(text + "AutoIncrementStep");
				Columns[i].AutoIncrementSeed = info.GetInt64(text + "AutoIncrementSeed");
				Columns[i].ReadOnly = info.GetBoolean(text + "ReadOnly");
				Columns[i].MaxLength = info.GetInt32(text + "MaxLength");
				Columns[i].ExtendedProperties = (PropertyCollection)info.GetValue(text + "ExtendedProperties", typeof(PropertyCollection));
				if (Columns[i].DataType == typeof(DataSetDateTime))
				{
					Columns[i].DateTimeMode = (DataSetDateTime)(int)info.GetValue(text + "DateTimeMode", typeof(DataSetDateTime));
				}
				Columns[i].ColumnMapping = (MappingType)(int)info.GetValue(text + "ColumnMapping", typeof(MappingType));
				try
				{
					Columns[i].Expression = info.GetString(text + "Expression");
					text = "DataTable_0.";
					arrayList = (ArrayList)info.GetValue(text + "Constraints", typeof(ArrayList));
					if (Constraints == null)
					{
						Constraints = new ConstraintCollection(this);
					}
					DeserializeConstraints(arrayList);
				}
				catch (SerializationException)
				{
				}
			}
			try
			{
				string text2 = "DataTable_0.";
				ArrayList nullBits = (ArrayList)info.GetValue(text2 + "NullBits", typeof(ArrayList));
				arrayList = (ArrayList)info.GetValue(text2 + "Records", typeof(ArrayList));
				BitArray rowStateBitArray = (BitArray)info.GetValue(text2 + "RowStates", typeof(BitArray));
				Hashtable hashtable = (Hashtable)info.GetValue(text2 + "RowErrors", typeof(Hashtable));
				DeserializeRecords(arrayList, nullBits, rowStateBitArray);
			}
			catch (SerializationException)
			{
			}
		}

		internal void BinarySerializeProperty(SerializationInfo info)
		{
			Version value = new Version(2, 0);
			info.AddValue("DataTable.RemotingVersion", value);
			info.AddValue("DataTable.RemotingFormat", RemotingFormat);
			info.AddValue("DataTable.TableName", TableName);
			info.AddValue("DataTable.Namespace", Namespace);
			info.AddValue("DataTable.Prefix", Prefix);
			info.AddValue("DataTable.CaseSensitive", CaseSensitive);
			info.AddValue("DataTable.caseSensitiveAmbient", true);
			info.AddValue("DataTable.NestedInDataSet", true);
			info.AddValue("DataTable.RepeatableElement", false);
			info.AddValue("DataTable.LocaleLCID", Locale.LCID);
			info.AddValue("DataTable.MinimumCapacity", MinimumCapacity);
			info.AddValue("DataTable.Columns.Count", Columns.Count);
			info.AddValue("DataTable.ExtendedProperties", _extendedProperties);
			for (int i = 0; i < Columns.Count; i++)
			{
				info.AddValue("DataTable.DataColumn_" + i + ".ColumnName", Columns[i].ColumnName);
				info.AddValue("DataTable.DataColumn_" + i + ".Namespace", Columns[i].Namespace);
				info.AddValue("DataTable.DataColumn_" + i + ".Caption", Columns[i].Caption);
				info.AddValue("DataTable.DataColumn_" + i + ".Prefix", Columns[i].Prefix);
				info.AddValue("DataTable.DataColumn_" + i + ".DataType", Columns[i].DataType, typeof(Type));
				info.AddValue("DataTable.DataColumn_" + i + ".DefaultValue", Columns[i].DefaultValue, typeof(DBNull));
				info.AddValue("DataTable.DataColumn_" + i + ".AllowDBNull", Columns[i].AllowDBNull);
				info.AddValue("DataTable.DataColumn_" + i + ".AutoIncrement", Columns[i].AutoIncrement);
				info.AddValue("DataTable.DataColumn_" + i + ".AutoIncrementStep", Columns[i].AutoIncrementStep);
				info.AddValue("DataTable.DataColumn_" + i + ".AutoIncrementSeed", Columns[i].AutoIncrementSeed);
				info.AddValue("DataTable.DataColumn_" + i + ".ReadOnly", Columns[i].ReadOnly);
				info.AddValue("DataTable.DataColumn_" + i + ".MaxLength", Columns[i].MaxLength);
				info.AddValue("DataTable.DataColumn_" + i + ".ExtendedProperties", Columns[i].ExtendedProperties);
				info.AddValue("DataTable.DataColumn_" + i + ".DateTimeMode", Columns[i].DateTimeMode);
				info.AddValue("DataTable.DataColumn_" + i + ".ColumnMapping", Columns[i].ColumnMapping, typeof(MappingType));
				info.AddValue("DataTable.DataColumn_" + i + ".SimpleType", null, typeof(string));
				info.AddValue("DataTable.DataColumn_" + i + ".AutoIncrementCurrent", Columns[i].AutoIncrementValue());
				info.AddValue("DataTable.DataColumn_" + i + ".XmlDataType", null, typeof(string));
			}
			info.AddValue("DataTable.TypeName", null, typeof(string));
		}

		internal void SerializeConstraints(SerializationInfo info, string prefix)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < Constraints.Count; i++)
			{
				ArrayList arrayList2 = new ArrayList();
				if (Constraints[i] is UniqueConstraint)
				{
					arrayList2.Add("U");
					UniqueConstraint uniqueConstraint = (UniqueConstraint)Constraints[i];
					arrayList2.Add(uniqueConstraint.ConstraintName);
					DataColumn[] columns = uniqueConstraint.Columns;
					int[] array = new int[columns.Length];
					for (int j = 0; j < columns.Length; j++)
					{
						array[j] = uniqueConstraint.Table.Columns.IndexOf(uniqueConstraint.Columns[j]);
					}
					arrayList2.Add(array);
					arrayList2.Add(uniqueConstraint.IsPrimaryKey);
					arrayList2.Add(uniqueConstraint.ExtendedProperties);
				}
				else
				{
					if (!(Constraints[i] is ForeignKeyConstraint))
					{
						continue;
					}
					arrayList2.Add("F");
					ForeignKeyConstraint foreignKeyConstraint = (ForeignKeyConstraint)Constraints[i];
					arrayList2.Add(foreignKeyConstraint.ConstraintName);
					int[] array2 = new int[foreignKeyConstraint.RelatedColumns.Length + 1];
					array2[0] = DataSet.Tables.IndexOf(foreignKeyConstraint.RelatedTable);
					for (int k = 0; k < foreignKeyConstraint.Columns.Length; k++)
					{
						array2[k + 1] = foreignKeyConstraint.RelatedColumns[k].Ordinal;
					}
					arrayList2.Add(array2);
					array2 = new int[foreignKeyConstraint.Columns.Length + 1];
					array2[0] = DataSet.Tables.IndexOf(foreignKeyConstraint.Table);
					for (int l = 0; l < foreignKeyConstraint.Columns.Length; l++)
					{
						array2[l + 1] = foreignKeyConstraint.Columns[l].Ordinal;
					}
					arrayList2.Add(array2);
					arrayList2.Add(new int[3]
					{
						(int)foreignKeyConstraint.AcceptRejectRule,
						(int)foreignKeyConstraint.UpdateRule,
						(int)foreignKeyConstraint.DeleteRule
					});
					arrayList2.Add(foreignKeyConstraint.ExtendedProperties);
				}
				arrayList.Add(arrayList2);
			}
			info.AddValue(prefix, arrayList, typeof(ArrayList));
		}

		internal void BinarySerialize(SerializationInfo info, string prefix)
		{
			int count = Columns.Count;
			int count2 = Rows.Count;
			int num = Rows.Count;
			ArrayList arrayList = new ArrayList();
			ArrayList arrayList2 = new ArrayList();
			BitArray bitArray = new BitArray(count2 * 3);
			for (int i = 0; i < Rows.Count; i++)
			{
				if (Rows[i].RowState == DataRowState.Modified)
				{
					num++;
				}
			}
			SerializeConstraints(info, prefix + "Constraints");
			for (int j = 0; j < count; j++)
			{
				if (count2 == 0)
				{
					continue;
				}
				BitArray bitArray2 = new BitArray(count2);
				DataColumn dataColumn = Columns[j];
				Array array = Array.CreateInstance(dataColumn.DataType, num);
				int num2 = 0;
				int num3 = 0;
				while (num2 < Rows.Count)
				{
					DataRow dataRow = Rows[num2];
					DataRowVersion version;
					if (dataRow.RowState != DataRowState.Modified)
					{
						version = ((dataRow.RowState != DataRowState.Deleted) ? DataRowVersion.Default : DataRowVersion.Original);
					}
					else
					{
						version = DataRowVersion.Default;
						bitArray2.Length++;
						if (!dataRow.IsNull(dataColumn, version))
						{
							bitArray2[num3] = false;
							array.SetValue(dataRow[j, version], num3);
						}
						else
						{
							bitArray2[num3] = true;
						}
						num3++;
						version = DataRowVersion.Current;
					}
					if (!dataRow.IsNull(dataColumn, version))
					{
						bitArray2[num3] = false;
						array.SetValue(dataRow[j, version], num3);
					}
					else
					{
						bitArray2[num3] = true;
					}
					num2++;
					num3++;
				}
				arrayList.Add(array);
				arrayList2.Add(bitArray2);
			}
			for (int k = 0; k < Rows.Count; k++)
			{
				int num4 = k * 3;
				switch (Rows[k].RowState)
				{
				case DataRowState.Detached:
					bitArray[num4] = false;
					bitArray[num4 + 1] = false;
					bitArray[num4 + 2] = true;
					break;
				case DataRowState.Unchanged:
					bitArray[num4] = false;
					bitArray[num4 + 1] = false;
					bitArray[num4 + 2] = false;
					break;
				case DataRowState.Added:
					bitArray[num4] = false;
					bitArray[num4 + 1] = true;
					bitArray[num4 + 2] = false;
					break;
				case DataRowState.Deleted:
					bitArray[num4] = true;
					bitArray[num4 + 1] = true;
					bitArray[num4 + 2] = false;
					break;
				default:
					bitArray[num4] = true;
					bitArray[num4 + 1] = false;
					bitArray[num4 + 2] = false;
					break;
				}
			}
			info.AddValue(prefix + "Rows.Count", Rows.Count);
			info.AddValue(prefix + "Records.Count", num);
			info.AddValue(prefix + "Records", arrayList, typeof(ArrayList));
			info.AddValue(prefix + "NullBits", arrayList2, typeof(ArrayList));
			info.AddValue(prefix + "RowStates", bitArray, typeof(BitArray));
			Hashtable value = new Hashtable();
			info.AddValue(prefix + "RowErrors", value, typeof(Hashtable));
			Hashtable value2 = new Hashtable();
			info.AddValue(prefix + "ColumnErrors", value2, typeof(Hashtable));
		}

		public DataTableReader CreateDataReader()
		{
			return new DataTableReader(this);
		}

		public void Load(IDataReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			Load(reader, LoadOption.PreserveChanges);
		}

		public void Load(IDataReader reader, LoadOption loadOption)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			bool flag = EnforceConstraints;
			try
			{
				EnforceConstraints = false;
				int[] mapping = DataAdapter.BuildSchema(reader, this, SchemaType.Mapped, MissingSchemaAction.AddWithKey, MissingMappingAction.Passthrough, new DataTableMappingCollection());
				DbDataAdapter.FillFromReader(this, reader, 0, 0, mapping, loadOption);
			}
			finally
			{
				EnforceConstraints = flag;
			}
		}

		public virtual void Load(IDataReader reader, LoadOption loadOption, FillErrorEventHandler errorHandler)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("Value cannot be null. Parameter name: reader");
			}
			bool flag = EnforceConstraints;
			try
			{
				EnforceConstraints = false;
				int[] mapping = DataAdapter.BuildSchema(reader, this, SchemaType.Mapped, MissingSchemaAction.AddWithKey, MissingMappingAction.Passthrough, new DataTableMappingCollection());
				DbDataAdapter.FillFromReader(this, reader, 0, 0, mapping, loadOption, errorHandler);
			}
			finally
			{
				EnforceConstraints = flag;
			}
		}

		public DataRow LoadDataRow(object[] values, LoadOption loadOption)
		{
			DataRow dataRow = null;
			if (PrimaryKey.Length > 0)
			{
				object[] array = new object[PrimaryKey.Length];
				for (int i = 0; i < PrimaryKey.Length; i++)
				{
					array[i] = values[PrimaryKey[i].Ordinal];
				}
				dataRow = Rows.Find(array, DataViewRowState.OriginalRows);
				if (dataRow == null)
				{
					dataRow = Rows.Find(array);
				}
			}
			if (dataRow == null || (dataRow.RowState == DataRowState.Deleted && loadOption == LoadOption.Upsert))
			{
				dataRow = NewNotInitializedRow();
				dataRow.ImportRecord(CreateRecord(values));
				dataRow.Validate();
				if (loadOption == LoadOption.OverwriteChanges || loadOption == LoadOption.PreserveChanges)
				{
					Rows.AddInternal(dataRow, DataRowAction.ChangeCurrentAndOriginal);
				}
				else
				{
					Rows.AddInternal(dataRow);
				}
				return dataRow;
			}
			dataRow.Load(values, loadOption);
			return dataRow;
		}

		public void Merge(DataTable table)
		{
			Merge(table, false, MissingSchemaAction.Add);
		}

		public void Merge(DataTable table, bool preserveChanges)
		{
			Merge(table, preserveChanges, MissingSchemaAction.Add);
		}

		public void Merge(DataTable table, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			MergeManager.Merge(this, table, preserveChanges, missingSchemaAction);
		}

		internal int CompareRecords(int x, int y)
		{
			for (int i = 0; i < Columns.Count; i++)
			{
				int num = Columns[i].DataContainer.CompareValues(x, y);
				if (num != 0)
				{
					return num;
				}
			}
			return 0;
		}

		protected virtual void OnTableCleared(DataTableClearEventArgs e)
		{
			if (this.TableCleared != null)
			{
				this.TableCleared(this, e);
			}
		}

		internal void DataTableCleared()
		{
			OnTableCleared(new DataTableClearEventArgs(this));
		}

		protected virtual void OnTableClearing(DataTableClearEventArgs e)
		{
			if (this.TableClearing != null)
			{
				this.TableClearing(this, e);
			}
		}

		internal void DataTableClearing()
		{
			OnTableClearing(new DataTableClearEventArgs(this));
		}

		protected virtual void OnTableNewRow(DataTableNewRowEventArgs e)
		{
			if (this.TableNewRow != null)
			{
				this.TableNewRow(this, e);
			}
		}

		private void NewRowAdded(DataRow dr)
		{
			OnTableNewRow(new DataTableNewRowEventArgs(dr));
		}
	}
}
