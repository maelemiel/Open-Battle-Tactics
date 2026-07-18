using System.ComponentModel;

namespace System.Data
{
	[DefaultProperty("RelationName")]
	[Editor("Microsoft.VSDesigner.Data.Design.DataRelationEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[TypeConverter(typeof(RelationshipConverter))]
	public class DataRelation
	{
		private DataSet dataSet;

		private string relationName;

		private UniqueConstraint parentKeyConstraint;

		private ForeignKeyConstraint childKeyConstraint;

		private DataColumn[] parentColumns;

		private DataColumn[] childColumns;

		private bool nested;

		internal bool createConstraints = true;

		private bool initFinished;

		private PropertyCollection extendedProperties;

		private PropertyChangedEventHandler onPropertyChangingDelegate;

		private string _relationName;

		private string _parentTableName;

		private string _childTableName;

		private string[] _parentColumnNames;

		private string[] _childColumnNames;

		private bool _nested;

		private bool initInProgress;

		private string _parentTableNameSpace = string.Empty;

		private string _childTableNameSpace = string.Empty;

		internal bool InitInProgress
		{
			get
			{
				return initInProgress;
			}
			set
			{
				initInProgress = value;
			}
		}

		[DataCategory("Data")]
		public virtual DataColumn[] ChildColumns
		{
			get
			{
				return childColumns;
			}
		}

		public virtual ForeignKeyConstraint ChildKeyConstraint
		{
			get
			{
				return childKeyConstraint;
			}
		}

		public virtual DataTable ChildTable
		{
			get
			{
				return childColumns[0].Table;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual DataSet DataSet
		{
			get
			{
				return childColumns[0].Table.DataSet;
			}
		}

		[DataCategory("Data")]
		[Browsable(false)]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				if (extendedProperties == null)
				{
					extendedProperties = new PropertyCollection();
				}
				return extendedProperties;
			}
		}

		[DefaultValue(false)]
		[DataCategory("Data")]
		public virtual bool Nested
		{
			get
			{
				return nested;
			}
			set
			{
				nested = value;
			}
		}

		[DataCategory("Data")]
		public virtual DataColumn[] ParentColumns
		{
			get
			{
				return parentColumns;
			}
		}

		public virtual UniqueConstraint ParentKeyConstraint
		{
			get
			{
				return parentKeyConstraint;
			}
		}

		public virtual DataTable ParentTable
		{
			get
			{
				return parentColumns[0].Table;
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public virtual string RelationName
		{
			get
			{
				return relationName;
			}
			set
			{
				relationName = value;
			}
		}

		public DataRelation(string relationName, DataColumn parentColumn, DataColumn childColumn)
			: this(relationName, parentColumn, childColumn, true)
		{
		}

		public DataRelation(string relationName, DataColumn[] parentColumns, DataColumn[] childColumns)
			: this(relationName, parentColumns, childColumns, true)
		{
		}

		public DataRelation(string relationName, DataColumn parentColumn, DataColumn childColumn, bool createConstraints)
			: this(relationName, new DataColumn[1] { parentColumn }, new DataColumn[1] { childColumn }, createConstraints)
		{
		}

		public DataRelation(string relationName, DataColumn[] parentColumns, DataColumn[] childColumns, bool createConstraints)
		{
			extendedProperties = new PropertyCollection();
			this.relationName = ((relationName != null) ? relationName : string.Empty);
			if (parentColumns == null)
			{
				throw new ArgumentNullException("parentColumns");
			}
			this.parentColumns = parentColumns;
			if (childColumns == null)
			{
				throw new ArgumentNullException("childColumns");
			}
			this.childColumns = childColumns;
			this.createConstraints = createConstraints;
			if (parentColumns.Length != childColumns.Length)
			{
				throw new ArgumentException("ParentColumns and ChildColumns should be the same length");
			}
			DataTable table = parentColumns[0].Table;
			DataTable table2 = childColumns[0].Table;
			if (table.DataSet != table2.DataSet)
			{
				throw new InvalidConstraintException();
			}
			foreach (DataColumn dataColumn in parentColumns)
			{
				if (dataColumn.Table != table)
				{
					throw new InvalidConstraintException();
				}
			}
			foreach (DataColumn dataColumn2 in childColumns)
			{
				if (dataColumn2.Table != table2)
				{
					throw new InvalidConstraintException();
				}
			}
			for (int k = 0; k < ChildColumns.Length; k++)
			{
				if (!parentColumns[k].DataTypeMatches(childColumns[k]))
				{
					throw new InvalidConstraintException("Parent Columns and Child Columns don't have matching column types");
				}
			}
		}

		[Browsable(false)]
		public DataRelation(string relationName, string parentTableName, string childTableName, string[] parentColumnNames, string[] childColumnNames, bool nested)
		{
			_relationName = relationName;
			_parentTableName = parentTableName;
			_childTableName = childTableName;
			_parentColumnNames = parentColumnNames;
			_childColumnNames = childColumnNames;
			_nested = nested;
			InitInProgress = true;
		}

		[Browsable(false)]
		public DataRelation(string relationName, string parentTableName, string parentTableNameSpace, string childTableName, string childTableNameSpace, string[] parentColumnNames, string[] childColumnNames, bool nested)
		{
			_relationName = relationName;
			_parentTableName = parentTableName;
			_parentTableNameSpace = parentTableNameSpace;
			_childTableName = childTableName;
			_childTableNameSpace = childTableNameSpace;
			_parentColumnNames = parentColumnNames;
			_childColumnNames = childColumnNames;
			_nested = nested;
			InitInProgress = true;
		}

		internal void FinishInit(DataSet ds)
		{
			if (!ds.Tables.Contains(_parentTableName) || !ds.Tables.Contains(_childTableName))
			{
				throw new InvalidOperationException();
			}
			if (_parentColumnNames.Length != _childColumnNames.Length)
			{
				throw new InvalidOperationException();
			}
			DataTable dataTable = ds.Tables[_parentTableName];
			DataTable dataTable2 = ds.Tables[_childTableName];
			parentColumns = new DataColumn[_parentColumnNames.Length];
			childColumns = new DataColumn[_childColumnNames.Length];
			for (int i = 0; i < _parentColumnNames.Length; i++)
			{
				if (!dataTable.Columns.Contains(_parentColumnNames[i]))
				{
					throw new InvalidOperationException();
				}
				parentColumns[i] = dataTable.Columns[_parentColumnNames[i]];
				if (!dataTable2.Columns.Contains(_childColumnNames[i]))
				{
					throw new InvalidOperationException();
				}
				childColumns[i] = dataTable2.Columns[_childColumnNames[i]];
			}
			RelationName = _relationName;
			Nested = _nested;
			initFinished = true;
			extendedProperties = new PropertyCollection();
			InitInProgress = false;
			if (_parentTableNameSpace != string.Empty)
			{
				dataTable.Namespace = _parentTableNameSpace;
			}
			if (_childTableNameSpace != string.Empty)
			{
				dataTable2.Namespace = _childTableNameSpace;
			}
		}

		internal void SetChildKeyConstraint(ForeignKeyConstraint foreignKeyConstraint)
		{
			childKeyConstraint = foreignKeyConstraint;
		}

		internal void SetParentKeyConstraint(UniqueConstraint uniqueConstraint)
		{
			parentKeyConstraint = uniqueConstraint;
		}

		internal void SetDataSet(DataSet ds)
		{
			dataSet = ds;
		}

		protected void CheckStateForProperty()
		{
			DataTable table = parentColumns[0].Table;
			DataTable table2 = childColumns[0].Table;
			if (table.DataSet != table2.DataSet)
			{
				throw new DataException();
			}
			bool flag = false;
			for (int i = 0; i < parentColumns.Length; i++)
			{
				if (!parentColumns[i].DataType.Equals(childColumns[i].DataType))
				{
					throw new DataException();
				}
				if (parentColumns[i] != childColumns[i])
				{
					flag = false;
				}
			}
			if (flag)
			{
				throw new DataException();
			}
		}

		protected internal void OnPropertyChanging(PropertyChangedEventArgs pcevent)
		{
			if (onPropertyChangingDelegate != null)
			{
				onPropertyChangingDelegate(this, pcevent);
			}
		}

		protected internal void RaisePropertyChanging(string name)
		{
			OnPropertyChanging(new PropertyChangedEventArgs(name));
		}

		public override string ToString()
		{
			return relationName;
		}

		internal void UpdateConstraints()
		{
			if (!initFinished && createConstraints)
			{
				ForeignKeyConstraint foreignKeyConstraint = null;
				UniqueConstraint uniqueConstraint = null;
				foreignKeyConstraint = FindForeignKey(ChildTable.Constraints);
				uniqueConstraint = FindUniqueConstraint(ParentTable.Constraints);
				if (uniqueConstraint == null)
				{
					uniqueConstraint = new UniqueConstraint(ParentColumns, false);
					ParentTable.Constraints.Add(uniqueConstraint);
				}
				if (foreignKeyConstraint == null)
				{
					foreignKeyConstraint = new ForeignKeyConstraint(RelationName, ParentColumns, ChildColumns);
					ChildTable.Constraints.Add(foreignKeyConstraint);
				}
				SetParentKeyConstraint(uniqueConstraint);
				SetChildKeyConstraint(foreignKeyConstraint);
			}
		}

		private static bool CompareDataColumns(DataColumn[] dc1, DataColumn[] dc2)
		{
			if (dc1.Length != dc2.Length)
			{
				return false;
			}
			for (int i = 0; i < dc1.Length; i++)
			{
				if (dc1[i] != dc2[i])
				{
					return false;
				}
			}
			return true;
		}

		private ForeignKeyConstraint FindForeignKey(ConstraintCollection cl)
		{
			ForeignKeyConstraint foreignKeyConstraint = null;
			foreach (Constraint item in cl)
			{
				if (item is ForeignKeyConstraint)
				{
					foreignKeyConstraint = (ForeignKeyConstraint)item;
					if (CompareDataColumns(ChildColumns, foreignKeyConstraint.Columns) && CompareDataColumns(ParentColumns, foreignKeyConstraint.RelatedColumns))
					{
						return foreignKeyConstraint;
					}
				}
			}
			return null;
		}

		private UniqueConstraint FindUniqueConstraint(ConstraintCollection cl)
		{
			UniqueConstraint uniqueConstraint = null;
			foreach (Constraint item in cl)
			{
				if (item is UniqueConstraint)
				{
					uniqueConstraint = (UniqueConstraint)item;
					if (CompareDataColumns(ParentColumns, uniqueConstraint.Columns))
					{
						return uniqueConstraint;
					}
				}
			}
			return null;
		}

		internal bool Contains(DataColumn column)
		{
			DataColumn[] array = ParentColumns;
			foreach (DataColumn dataColumn in array)
			{
				if (dataColumn == column)
				{
					return true;
				}
			}
			DataColumn[] array2 = ChildColumns;
			foreach (DataColumn dataColumn2 in array2)
			{
				if (dataColumn2 == column)
				{
					return true;
				}
			}
			return false;
		}
	}
}
