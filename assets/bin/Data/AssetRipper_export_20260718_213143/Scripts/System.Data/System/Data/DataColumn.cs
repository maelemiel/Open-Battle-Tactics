using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using Mono.Data.SqlExpressions;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.DataColumnEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DesignTimeVisible(false)]
	[DefaultProperty("ColumnName")]
	[ToolboxItem(false)]
	public class DataColumn : MarshalByValueComponent
	{
		private EventHandlerList _eventHandlers = new EventHandlerList();

		private static readonly object _propertyChangedKey = new object();

		private bool _allowDBNull = true;

		private bool _autoIncrement;

		private long _autoIncrementSeed;

		private long _autoIncrementStep = 1L;

		private long _nextAutoIncrementValue;

		private string _caption;

		private MappingType _columnMapping;

		private string _columnName = string.Empty;

		private object _defaultValue = GetDefaultValueForType(null);

		private string _expression;

		private IExpression _compiledExpression;

		private PropertyCollection _extendedProperties = new PropertyCollection();

		private int _maxLength = -1;

		private string _nameSpace;

		private int _ordinal = -1;

		private string _prefix = string.Empty;

		private bool _readOnly;

		private DataTable _table;

		private bool _unique;

		private DataContainer _dataContainer;

		private DataSetDateTime _datetimeMode = DataSetDateTime.UnspecifiedLocal;

		internal object this[int index]
		{
			get
			{
				return DataContainer[index];
			}
			set
			{
				if (value != null || !AutoIncrement)
				{
					try
					{
						DataContainer[index] = value;
					}
					catch (Exception ex)
					{
						throw new ArgumentException(string.Format("{0}. Couldn't store <{1}> in Column named '{2}'. Expected type is {3}.", ex.Message, value, ColumnName, DataType.Name), ex);
					}
				}
				if (AutoIncrement && !DataContainer.IsNull(index))
				{
					long value2 = Convert.ToInt64(value);
					UpdateAutoIncrementValue(value2);
				}
			}
		}

		[DefaultValue(DataSetDateTime.UnspecifiedLocal)]
		[RefreshProperties(RefreshProperties.All)]
		public DataSetDateTime DateTimeMode
		{
			get
			{
				return _datetimeMode;
			}
			set
			{
				if (DataType != typeof(DateTime))
				{
					throw new InvalidOperationException("The DateTimeMode can be set only on DataColumns of type DateTime.");
				}
				if (!Enum.IsDefined(typeof(DataSetDateTime), value))
				{
					throw new InvalidEnumArgumentException(string.Format(CultureInfo.InvariantCulture, "The {0} enumeration value, {1}, is invalid", typeof(DataSetDateTime).Name, value));
				}
				if (_datetimeMode == value)
				{
					return;
				}
				if (_table == null || _table.Rows.Count == 0)
				{
					_datetimeMode = value;
					return;
				}
				if ((_datetimeMode == DataSetDateTime.Unspecified || _datetimeMode == DataSetDateTime.UnspecifiedLocal) && (value == DataSetDateTime.Unspecified || value == DataSetDateTime.UnspecifiedLocal))
				{
					_datetimeMode = value;
					return;
				}
				throw new InvalidOperationException(string.Format("Cannot change DateTimeMode from '{0}' to '{1}' once the table has data.", _datetimeMode, value));
			}
		}

		[DefaultValue(true)]
		[DataCategory("Data")]
		public bool AllowDBNull
		{
			get
			{
				return _allowDBNull;
			}
			set
			{
				if (!value && _table != null)
				{
					for (int i = 0; i < _table.Rows.Count; i++)
					{
						DataRow dataRow = _table.Rows[i];
						DataRowVersion version = ((!dataRow.HasVersion(DataRowVersion.Default)) ? DataRowVersion.Original : DataRowVersion.Default);
						if (dataRow.IsNull(this, version))
						{
							throw new DataException("Column '" + ColumnName + "' has null values in it.");
						}
					}
				}
				_allowDBNull = value;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		public bool AutoIncrement
		{
			get
			{
				return _autoIncrement;
			}
			set
			{
				if (value)
				{
					if (Expression != string.Empty)
					{
						throw new ArgumentException("Can not Auto Increment a computed column.");
					}
					if (DefaultValue != DBNull.Value)
					{
						throw new ArgumentException("Can not set AutoIncrement while default value exists for this column.");
					}
					if (!CanAutoIncrement(DataType))
					{
						DataType = typeof(int);
					}
				}
				if (_table != null)
				{
					_table.Columns.UpdateAutoIncrement(this, value);
				}
				_autoIncrement = value;
			}
		}

		[DefaultValue(0)]
		[DataCategory("Data")]
		public long AutoIncrementSeed
		{
			get
			{
				return _autoIncrementSeed;
			}
			set
			{
				_autoIncrementSeed = value;
				_nextAutoIncrementValue = _autoIncrementSeed;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(1)]
		public long AutoIncrementStep
		{
			get
			{
				return _autoIncrementStep;
			}
			set
			{
				_autoIncrementStep = value;
			}
		}

		[DataCategory("Data")]
		public string Caption
		{
			get
			{
				return (_caption != null) ? _caption : ColumnName;
			}
			set
			{
				_caption = ((value != null) ? value : string.Empty);
			}
		}

		[DefaultValue(MappingType.Element)]
		public virtual MappingType ColumnMapping
		{
			get
			{
				return _columnMapping;
			}
			set
			{
				_columnMapping = value;
			}
		}

		[DataCategory("Data")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		public string ColumnName
		{
			get
			{
				return _columnName;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				CultureInfo culture = ((Table == null) ? CultureInfo.CurrentCulture : Table.Locale);
				if (string.Compare(value, _columnName, true, culture) != 0)
				{
					if (Table != null)
					{
						if (value.Length == 0)
						{
							throw new ArgumentException("ColumnName is required when it is part of a DataTable.");
						}
						Table.Columns.RegisterName(value, this);
						if (_columnName.Length > 0)
						{
							Table.Columns.UnregisterName(_columnName);
						}
					}
					RaisePropertyChanging("ColumnName");
					_columnName = value;
					if (Table != null)
					{
						Table.ResetPropertyDescriptorsCache();
					}
				}
				else if (string.Compare(value, _columnName, false, culture) != 0)
				{
					RaisePropertyChanging("ColumnName");
					_columnName = value;
					if (Table != null)
					{
						Table.ResetPropertyDescriptorsCache();
					}
				}
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DataCategory("Data")]
		[DefaultValue(typeof(string))]
		[TypeConverter(typeof(ColumnTypeConverter))]
		public Type DataType
		{
			get
			{
				return DataContainer.Type;
			}
			set
			{
				if (value == null)
				{
					return;
				}
				if (_dataContainer != null)
				{
					if (value == _dataContainer.Type)
					{
						return;
					}
					if (_dataContainer.Capacity > 0)
					{
						throw new ArgumentException("The column already has data stored.");
					}
				}
				if (GetParentRelation() != null || GetChildRelation() != null)
				{
					throw new InvalidConstraintException("Cannot change datatype when column is part of a relation");
				}
				Type type = ((_dataContainer == null) ? null : _dataContainer.Type);
				if (_dataContainer != null && _dataContainer.Type == typeof(DateTime))
				{
					_datetimeMode = DataSetDateTime.UnspecifiedLocal;
				}
				_dataContainer = DataContainer.Create(value, this);
				if (AutoIncrement && !CanAutoIncrement(value))
				{
					AutoIncrement = false;
				}
				if (DefaultValue != GetDefaultValueForType(type))
				{
					SetDefaultValue(DefaultValue, true);
				}
				else
				{
					_defaultValue = GetDefaultValueForType(DataType);
				}
			}
		}

		[DataCategory("Data")]
		[TypeConverter(typeof(DefaultValueTypeConverter))]
		public object DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				if (AutoIncrement)
				{
					throw new ArgumentException("Can not set default value while AutoIncrement is true on this column.");
				}
				SetDefaultValue(value, false);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue("")]
		[DataCategory("Data")]
		public string Expression
		{
			get
			{
				return _expression;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (value != string.Empty)
				{
					if (AutoIncrement || Unique)
					{
						throw new ArgumentException("Cannot create an expression on a column that has AutoIncrement or Unique.");
					}
					if (Table != null)
					{
						for (int i = 0; i < Table.Constraints.Count; i++)
						{
							if (Table.Constraints[i].IsColumnContained(this))
							{
								throw new ArgumentException(string.Format("Cannot set Expression property on column {0}, because it is a part of a constraint.", ColumnName));
							}
						}
					}
					Parser parser = new Parser();
					IExpression expression = parser.Compile(value);
					if (Table != null)
					{
						if (expression.DependsOn(this))
						{
							throw new ArgumentException("Cannot set Expression property due to circular reference in the expression.");
						}
						if (Table.Rows.Count == 0)
						{
							expression.Eval(Table.NewRow());
						}
						else
						{
							expression.Eval(Table.Rows[0]);
						}
					}
					ReadOnly = true;
					_compiledExpression = expression;
				}
				else
				{
					_compiledExpression = null;
					if (Table != null)
					{
						int defaultValuesRowIndex = Table.DefaultValuesRowIndex;
						if (defaultValuesRowIndex != -1)
						{
							DataContainer.FillValues(defaultValuesRowIndex);
						}
					}
				}
				_expression = value;
			}
		}

		internal IExpression CompiledExpression
		{
			get
			{
				return _compiledExpression;
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
			internal set
			{
				_extendedProperties = value;
			}
		}

		[DefaultValue(-1)]
		[DataCategory("Data")]
		public int MaxLength
		{
			get
			{
				return _maxLength;
			}
			set
			{
				if (value >= 0 && _columnMapping == MappingType.SimpleContent)
				{
					throw new ArgumentException(string.Format("Cannot set MaxLength property on '{0}' column which is mapped to SimpleContent.", ColumnName));
				}
				_maxLength = value;
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
				if (Table != null && _columnMapping != MappingType.Attribute)
				{
					return Table.Namespace;
				}
				return string.Empty;
			}
			set
			{
				_nameSpace = value;
			}
		}

		[DataCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int Ordinal
		{
			get
			{
				return _ordinal;
			}
			internal set
			{
				_ordinal = value;
			}
		}

		[DefaultValue("")]
		[DataCategory("Data")]
		public string Prefix
		{
			get
			{
				return _prefix;
			}
			set
			{
				_prefix = ((value != null) ? value : string.Empty);
			}
		}

		[DefaultValue(false)]
		[DataCategory("Data")]
		public bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
			}
		}

		[DataCategory("Data")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DataTable Table
		{
			get
			{
				return _table;
			}
			internal set
			{
				_table = value;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool Unique
		{
			get
			{
				return _unique;
			}
			set
			{
				if (_unique == value)
				{
					return;
				}
				_unique = value;
				if (_table == null)
				{
					return;
				}
				try
				{
					if (value)
					{
						if (Expression != null && Expression != string.Empty)
						{
							throw new ArgumentException("Cannot change Unique property for the expression column.");
						}
						_table.Constraints.Add(null, this, false);
					}
					else
					{
						UniqueConstraint uniqueConstraintForColumnSet = UniqueConstraint.GetUniqueConstraintForColumnSet(_table.Constraints, new DataColumn[1] { this });
						_table.Constraints.Remove(uniqueConstraintForColumnSet);
					}
				}
				catch (Exception ex)
				{
					_unique = !value;
					throw ex;
				}
			}
		}

		internal DataContainer DataContainer
		{
			get
			{
				return _dataContainer;
			}
		}

		internal event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				_eventHandlers.AddHandler(_propertyChangedKey, value);
			}
			remove
			{
				_eventHandlers.RemoveHandler(_propertyChangedKey, value);
			}
		}

		public DataColumn()
			: this(string.Empty, typeof(string), string.Empty, MappingType.Element)
		{
		}

		public DataColumn(string columnName)
			: this(columnName, typeof(string), string.Empty, MappingType.Element)
		{
		}

		public DataColumn(string columnName, Type dataType)
			: this(columnName, dataType, string.Empty, MappingType.Element)
		{
		}

		public DataColumn(string columnName, Type dataType, string expr)
			: this(columnName, dataType, expr, MappingType.Element)
		{
		}

		public DataColumn(string columnName, Type dataType, string expr, MappingType type)
		{
			ColumnName = ((columnName != null) ? columnName : string.Empty);
			if (dataType == null)
			{
				throw new ArgumentNullException("dataType");
			}
			DataType = dataType;
			Expression = ((expr != null) ? expr : string.Empty);
			ColumnMapping = type;
		}

		internal void UpdateAutoIncrementValue(long value64)
		{
			if (_autoIncrementStep > 0)
			{
				if (value64 >= _nextAutoIncrementValue)
				{
					_nextAutoIncrementValue = value64;
					AutoIncrementValue();
				}
			}
			else if (value64 <= _nextAutoIncrementValue)
			{
				AutoIncrementValue();
			}
		}

		internal long AutoIncrementValue()
		{
			long nextAutoIncrementValue = _nextAutoIncrementValue;
			_nextAutoIncrementValue += AutoIncrementStep;
			return nextAutoIncrementValue;
		}

		internal long GetAutoIncrementValue()
		{
			return _nextAutoIncrementValue;
		}

		internal void SetDefaultValue(int index)
		{
			if (AutoIncrement)
			{
				this[index] = _nextAutoIncrementValue;
			}
			else
			{
				DataContainer.CopyValue(Table.DefaultValuesRowIndex, index);
			}
		}

		private void SetDefaultValue(object value, bool forcedTypeCheck)
		{
			if (forcedTypeCheck || !_defaultValue.Equals(value))
			{
				if (value == null || value == DBNull.Value)
				{
					_defaultValue = GetDefaultValueForType(DataType);
				}
				else if (DataType.IsInstanceOfType(value))
				{
					_defaultValue = value;
				}
				else
				{
					try
					{
						_defaultValue = Convert.ChangeType(value, DataType);
					}
					catch (InvalidCastException)
					{
						string s = string.Format("Default Value of type '{0}' is not compatible with column type '{1}'", value.GetType(), DataType);
						throw new DataException(s);
					}
				}
			}
			if (Table != null && Table.DefaultValuesRowIndex != -1)
			{
				DataContainer[Table.DefaultValuesRowIndex] = _defaultValue;
			}
		}

		public void SetOrdinal(int ordinal)
		{
			if (_ordinal == -1)
			{
				throw new ArgumentException("Column must belong to a table.");
			}
			_table.Columns.MoveColumn(_ordinal, ordinal);
			_ordinal = ordinal;
		}

		internal static bool CanAutoIncrement(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Decimal:
				return true;
			default:
				return false;
			}
		}

		[System.MonoTODO]
		internal DataColumn Clone()
		{
			DataColumn dataColumn = new DataColumn();
			dataColumn._allowDBNull = _allowDBNull;
			dataColumn._autoIncrement = _autoIncrement;
			dataColumn._autoIncrementSeed = _autoIncrementSeed;
			dataColumn._autoIncrementStep = _autoIncrementStep;
			dataColumn._caption = _caption;
			dataColumn._columnMapping = _columnMapping;
			dataColumn._columnName = _columnName;
			dataColumn.DataType = DataType;
			dataColumn._defaultValue = _defaultValue;
			dataColumn.Expression = _expression;
			dataColumn._maxLength = _maxLength;
			dataColumn._nameSpace = _nameSpace;
			dataColumn._prefix = _prefix;
			dataColumn._readOnly = _readOnly;
			if (DataType == typeof(DateTime))
			{
				dataColumn.DateTimeMode = _datetimeMode;
			}
			dataColumn._extendedProperties = _extendedProperties;
			return dataColumn;
		}

		internal void SetUnique()
		{
			_unique = true;
		}

		[System.MonoTODO]
		internal void AssertCanAddToCollection()
		{
		}

		[System.MonoTODO]
		protected internal void CheckNotAllowNull()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected void CheckUnique()
		{
			throw new NotImplementedException();
		}

		protected internal virtual void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			PropertyChangedEventHandler propertyChangedEventHandler = _eventHandlers[_propertyChangedKey] as PropertyChangedEventHandler;
			if (propertyChangedEventHandler != null)
			{
				propertyChangedEventHandler(this, pcevent);
			}
		}

		protected internal void RaisePropertyChanging(string name)
		{
			PropertyChangedEventArgs pcevent = new PropertyChangedEventArgs(name);
			OnPropertyChanging(pcevent);
		}

		public override string ToString()
		{
			if (_expression != string.Empty)
			{
				return ColumnName + " + " + _expression;
			}
			return ColumnName;
		}

		internal void SetTable(DataTable table)
		{
			if (_table != null)
			{
				throw new ArgumentException("The column already belongs to a different table");
			}
			_table = table;
			if (_unique)
			{
				UniqueConstraint constraint = new UniqueConstraint(this);
				_table.Constraints.Add(constraint);
			}
			DataContainer.Capacity = _table.RecordCache.CurrentCapacity;
			int defaultValuesRowIndex = _table.DefaultValuesRowIndex;
			if (defaultValuesRowIndex != -1)
			{
				DataContainer[defaultValuesRowIndex] = _defaultValue;
				DataContainer.FillValues(defaultValuesRowIndex);
			}
		}

		internal static bool AreColumnSetsTheSame(DataColumn[] columnSet, DataColumn[] compareSet)
		{
			if (columnSet == null && compareSet == null)
			{
				return true;
			}
			if (columnSet == null || compareSet == null)
			{
				return false;
			}
			if (columnSet.Length != compareSet.Length)
			{
				return false;
			}
			foreach (DataColumn dataColumn in columnSet)
			{
				bool flag = false;
				foreach (DataColumn dataColumn2 in compareSet)
				{
					if (dataColumn == dataColumn2)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		internal int CompareValues(int index1, int index2)
		{
			return DataContainer.CompareValues(index1, index2);
		}

		private DataRelation GetParentRelation()
		{
			if (_table == null)
			{
				return null;
			}
			foreach (DataRelation parentRelation in _table.ParentRelations)
			{
				if (parentRelation.Contains(this))
				{
					return parentRelation;
				}
			}
			return null;
		}

		private DataRelation GetChildRelation()
		{
			if (_table == null)
			{
				return null;
			}
			foreach (DataRelation childRelation in _table.ChildRelations)
			{
				if (childRelation.Contains(this))
				{
					return childRelation;
				}
			}
			return null;
		}

		internal void ResetColumnInfo()
		{
			_ordinal = -1;
			_table = null;
			if (_compiledExpression != null)
			{
				_compiledExpression.ResetExpression();
			}
		}

		internal bool DataTypeMatches(DataColumn col)
		{
			if (DataType != col.DataType)
			{
				return false;
			}
			if (DataType != typeof(DateTime))
			{
				return true;
			}
			if (DateTimeMode == col.DateTimeMode)
			{
				return true;
			}
			if (DateTimeMode == DataSetDateTime.Local || DateTimeMode == DataSetDateTime.Utc)
			{
				return false;
			}
			if (col.DateTimeMode == DataSetDateTime.Local || col.DateTimeMode == DataSetDateTime.Utc)
			{
				return false;
			}
			return true;
		}

		internal static object GetDefaultValueForType(Type type)
		{
			if (type == null)
			{
				return DBNull.Value;
			}
			if (type.Namespace == "System.Data.SqlTypes" && type.Assembly == typeof(DataColumn).Assembly)
			{
				if (type == typeof(SqlBinary))
				{
					return SqlBinary.Null;
				}
				if (type == typeof(SqlBoolean))
				{
					return SqlBoolean.Null;
				}
				if (type == typeof(SqlByte))
				{
					return SqlByte.Null;
				}
				if (type == typeof(SqlBytes))
				{
					return SqlBytes.Null;
				}
				if (type == typeof(SqlChars))
				{
					return SqlChars.Null;
				}
				if (type == typeof(SqlDateTime))
				{
					return SqlDateTime.Null;
				}
				if (type == typeof(SqlDecimal))
				{
					return SqlDecimal.Null;
				}
				if (type == typeof(SqlDouble))
				{
					return SqlDouble.Null;
				}
				if (type == typeof(SqlGuid))
				{
					return SqlGuid.Null;
				}
				if (type == typeof(SqlInt16))
				{
					return SqlInt16.Null;
				}
				if (type == typeof(SqlInt32))
				{
					return SqlInt32.Null;
				}
				if (type == typeof(SqlInt64))
				{
					return SqlInt64.Null;
				}
				if (type == typeof(SqlMoney))
				{
					return SqlMoney.Null;
				}
				if (type == typeof(SqlSingle))
				{
					return SqlSingle.Null;
				}
				if (type == typeof(SqlString))
				{
					return SqlString.Null;
				}
				if (type == typeof(SqlXml))
				{
					return SqlXml.Null;
				}
			}
			return DBNull.Value;
		}
	}
}
