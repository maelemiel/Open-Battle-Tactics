using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	[Editor("Microsoft.VSDesigner.Data.Design.DataTableMappingCollectionEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ListBindable(false)]
	public sealed class DataTableMappingCollection : MarshalByRefObject, IList, ICollection, IEnumerable, ITableMappingCollection
	{
		private ArrayList mappings;

		private Hashtable sourceTables;

		private Hashtable dataSetTables;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!(value is DataTableMapping))
				{
					throw new ArgumentException();
				}
				this[index] = (DataTableMapping)value;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return mappings.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return mappings.SyncRoot;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object ITableMappingCollection.this[string index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!(value is DataTableMapping))
				{
					throw new ArgumentException();
				}
				this[index] = (DataTableMapping)value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int Count
		{
			get
			{
				return mappings.Count;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTableMapping this[int index]
		{
			get
			{
				return (DataTableMapping)mappings[index];
			}
			set
			{
				DataTableMapping dataTableMapping = (DataTableMapping)mappings[index];
				sourceTables[dataTableMapping.SourceTable] = value;
				dataSetTables[dataTableMapping.DataSetTable] = value;
				mappings[index] = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DataTableMapping this[string sourceTable]
		{
			get
			{
				return (DataTableMapping)sourceTables[sourceTable];
			}
			set
			{
				this[mappings.IndexOf(sourceTables[sourceTable])] = value;
			}
		}

		public DataTableMappingCollection()
		{
			mappings = new ArrayList();
			sourceTables = new Hashtable();
			dataSetTables = new Hashtable();
		}

		ITableMapping ITableMappingCollection.Add(string sourceTableName, string dataSetTableName)
		{
			ITableMapping tableMapping = new DataTableMapping(sourceTableName, dataSetTableName);
			Add(tableMapping);
			return tableMapping;
		}

		ITableMapping ITableMappingCollection.GetByDataSetTable(string dataSetTableName)
		{
			return this[mappings.IndexOf(dataSetTables[dataSetTableName])];
		}

		public int Add(object value)
		{
			if (!(value is DataTableMapping))
			{
				throw new InvalidCastException("The object passed in was not a DataTableMapping object.");
			}
			sourceTables[((DataTableMapping)value).SourceTable] = value;
			dataSetTables[((DataTableMapping)value).DataSetTable] = value;
			return mappings.Add(value);
		}

		public DataTableMapping Add(string sourceTable, string dataSetTable)
		{
			DataTableMapping dataTableMapping = new DataTableMapping(sourceTable, dataSetTable);
			Add(dataTableMapping);
			return dataTableMapping;
		}

		public void AddRange(Array values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				Add(values.GetValue(i));
			}
		}

		public void AddRange(DataTableMapping[] values)
		{
			foreach (DataTableMapping value in values)
			{
				Add(value);
			}
		}

		public void Clear()
		{
			sourceTables.Clear();
			dataSetTables.Clear();
			mappings.Clear();
		}

		public bool Contains(object value)
		{
			return mappings.Contains(value);
		}

		public bool Contains(string value)
		{
			return sourceTables.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			mappings.CopyTo(array, index);
		}

		public void CopyTo(DataTableMapping[] array, int index)
		{
			mappings.CopyTo(array, index);
		}

		public DataTableMapping GetByDataSetTable(string dataSetTable)
		{
			if (dataSetTables[dataSetTable] != null)
			{
				return (DataTableMapping)dataSetTables[dataSetTable];
			}
			string text = dataSetTable.ToLower();
			object[] array = new object[dataSetTables.Count];
			dataSetTables.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = (string)array[i];
				if (text.Equals(text2.ToLower()))
				{
					return (DataTableMapping)dataSetTables[array[i]];
				}
			}
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static DataTableMapping GetTableMappingBySchemaAction(DataTableMappingCollection tableMappings, string sourceTable, string dataSetTable, MissingMappingAction mappingAction)
		{
			if (tableMappings.Contains(sourceTable))
			{
				return tableMappings[sourceTable];
			}
			switch (mappingAction)
			{
			case MissingMappingAction.Error:
				throw new InvalidOperationException(string.Format("Missing source table mapping: '{0}'", sourceTable));
			case MissingMappingAction.Ignore:
				return null;
			default:
				return new DataTableMapping(sourceTable, dataSetTable);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return mappings.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return mappings.IndexOf(value);
		}

		public int IndexOf(string sourceTable)
		{
			return IndexOf(sourceTables[sourceTable]);
		}

		public int IndexOfDataSetTable(string dataSetTable)
		{
			if (dataSetTables[dataSetTable] != null)
			{
				return IndexOf((DataTableMapping)dataSetTables[dataSetTable]);
			}
			string text = dataSetTable.ToLower();
			object[] array = new object[dataSetTables.Count];
			dataSetTables.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = (string)array[i];
				if (text.Equals(text2.ToLower()))
				{
					return IndexOf((DataTableMapping)dataSetTables[array[i]]);
				}
			}
			return -1;
		}

		public void Insert(int index, object value)
		{
			mappings.Insert(index, value);
			sourceTables[((DataTableMapping)value).SourceTable] = value;
			dataSetTables[((DataTableMapping)value).DataSetTable] = value;
		}

		public void Insert(int index, DataTableMapping value)
		{
			mappings.Insert(index, value);
			sourceTables[value.SourceTable] = value;
			dataSetTables[value.DataSetTable] = value;
		}

		public void Remove(object value)
		{
			if (!(value is DataTableMapping))
			{
				throw new InvalidCastException();
			}
			int num = mappings.IndexOf(value);
			if (num < 0 || num >= mappings.Count)
			{
				throw new ArgumentException("There is no such element in collection.");
			}
			mappings.Remove((DataTableMapping)value);
		}

		public void Remove(DataTableMapping value)
		{
			int num = mappings.IndexOf(value);
			if (num < 0 || num >= mappings.Count)
			{
				throw new ArgumentException("There is no such element in collection.");
			}
			mappings.Remove(value);
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= mappings.Count)
			{
				throw new IndexOutOfRangeException("There is no element in collection.");
			}
			mappings.RemoveAt(index);
		}

		public void RemoveAt(string sourceTable)
		{
			RemoveAt(mappings.IndexOf(sourceTables[sourceTable]));
		}
	}
}
