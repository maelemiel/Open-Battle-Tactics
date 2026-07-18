using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public sealed class DataColumnMappingCollection : MarshalByRefObject, IList, ICollection, IEnumerable, IColumnMappingCollection
	{
		private readonly ArrayList list;

		private readonly Hashtable sourceColumns;

		private readonly Hashtable dataSetColumns;

		object ICollection.SyncRoot
		{
			get
			{
				return list.SyncRoot;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return list.IsSynchronized;
			}
		}

		object IColumnMappingCollection.this[string index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!(value is DataColumnMapping))
				{
					throw new ArgumentException();
				}
				this[index] = (DataColumnMapping)value;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				if (!(value is DataColumnMapping))
				{
					throw new ArgumentException();
				}
				this[index] = (DataColumnMapping)value;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataColumnMapping this[int index]
		{
			get
			{
				return (DataColumnMapping)list[index];
			}
			set
			{
				DataColumnMapping key = (DataColumnMapping)list[index];
				sourceColumns[key] = value;
				dataSetColumns[key] = value;
				list[index] = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataColumnMapping this[string sourceColumn]
		{
			get
			{
				if (!Contains(sourceColumn))
				{
					throw new IndexOutOfRangeException("DataColumnMappingCollection doesn't contain DataColumnMapping with SourceColumn '" + sourceColumn + "'.");
				}
				return (DataColumnMapping)sourceColumns[sourceColumn];
			}
			set
			{
				this[list.IndexOf(sourceColumns[sourceColumn])] = value;
			}
		}

		public DataColumnMappingCollection()
		{
			list = new ArrayList();
			sourceColumns = new Hashtable();
			dataSetColumns = new Hashtable();
		}

		IColumnMapping IColumnMappingCollection.Add(string sourceColumnName, string dataSetColumnName)
		{
			return Add(sourceColumnName, dataSetColumnName);
		}

		IColumnMapping IColumnMappingCollection.GetByDataSetColumn(string dataSetColumnName)
		{
			return GetByDataSetColumn(dataSetColumnName);
		}

		public int Add(object value)
		{
			if (!(value is DataColumnMapping))
			{
				throw new InvalidCastException();
			}
			list.Add(value);
			sourceColumns[((DataColumnMapping)value).SourceColumn] = value;
			dataSetColumns[((DataColumnMapping)value).DataSetColumn] = value;
			return list.IndexOf(value);
		}

		public DataColumnMapping Add(string sourceColumn, string dataSetColumn)
		{
			DataColumnMapping dataColumnMapping = new DataColumnMapping(sourceColumn, dataSetColumn);
			Add(dataColumnMapping);
			return dataColumnMapping;
		}

		public void AddRange(Array values)
		{
			for (int i = 0; i < values.Length; i++)
			{
				Add(values.GetValue(i));
			}
		}

		public void AddRange(DataColumnMapping[] values)
		{
			foreach (DataColumnMapping value in values)
			{
				Add(value);
			}
		}

		public void Clear()
		{
			list.Clear();
		}

		public bool Contains(object value)
		{
			if (!(value is DataColumnMapping))
			{
				throw new InvalidCastException("Object is not of type DataColumnMapping");
			}
			return list.Contains(value);
		}

		public bool Contains(string value)
		{
			return sourceColumns.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			list.CopyTo(array, index);
		}

		public void CopyTo(DataColumnMapping[] arr, int index)
		{
			list.CopyTo(arr, index);
		}

		public DataColumnMapping GetByDataSetColumn(string value)
		{
			if (dataSetColumns[value] != null)
			{
				return (DataColumnMapping)dataSetColumns[value];
			}
			string text = value.ToLower();
			object[] array = new object[dataSetColumns.Count];
			dataSetColumns.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = (string)array[i];
				if (text.Equals(text2.ToLower()))
				{
					return (DataColumnMapping)dataSetColumns[array[i]];
				}
			}
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static DataColumnMapping GetColumnMappingBySchemaAction(DataColumnMappingCollection columnMappings, string sourceColumn, MissingMappingAction mappingAction)
		{
			if (columnMappings.Contains(sourceColumn))
			{
				return columnMappings[sourceColumn];
			}
			switch (mappingAction)
			{
			case MissingMappingAction.Ignore:
				return null;
			case MissingMappingAction.Error:
				throw new InvalidOperationException(string.Format("Missing SourceColumn mapping for '{0}'", sourceColumn));
			default:
				return new DataColumnMapping(sourceColumn, sourceColumn);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[System.MonoTODO]
		public static DataColumn GetDataColumn(DataColumnMappingCollection columnMappings, string sourceColumn, Type dataType, DataTable dataTable, MissingMappingAction mappingAction, MissingSchemaAction schemaAction)
		{
			throw new NotImplementedException();
		}

		public IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public int IndexOf(object value)
		{
			return list.IndexOf(value);
		}

		public int IndexOf(string sourceColumn)
		{
			return list.IndexOf(sourceColumns[sourceColumn]);
		}

		public int IndexOfDataSetColumn(string dataSetColumn)
		{
			if (dataSetColumns[dataSetColumn] != null)
			{
				return list.IndexOf(dataSetColumns[dataSetColumn]);
			}
			string text = dataSetColumn.ToLower();
			object[] array = new object[dataSetColumns.Count];
			dataSetColumns.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = (string)array[i];
				if (text.Equals(text2.ToLower()))
				{
					return list.IndexOf(dataSetColumns[array[i]]);
				}
			}
			return -1;
		}

		public void Insert(int index, object value)
		{
			list.Insert(index, value);
			sourceColumns[((DataColumnMapping)value).SourceColumn] = value;
			dataSetColumns[((DataColumnMapping)value).DataSetColumn] = value;
		}

		public void Insert(int index, DataColumnMapping mapping)
		{
			list.Insert(index, mapping);
			sourceColumns[mapping.SourceColumn] = mapping;
			dataSetColumns[mapping.DataSetColumn] = mapping;
		}

		public void Remove(object value)
		{
			int num = list.IndexOf(value);
			sourceColumns.Remove(((DataColumnMapping)value).SourceColumn);
			dataSetColumns.Remove(((DataColumnMapping)value).DataSetColumn);
			if (num < 0 || num >= list.Count)
			{
				throw new ArgumentException("There is no such element in collection.");
			}
			list.Remove(value);
		}

		public void Remove(DataColumnMapping value)
		{
			int num = list.IndexOf(value);
			sourceColumns.Remove(value.SourceColumn);
			dataSetColumns.Remove(value.DataSetColumn);
			if (num < 0 || num >= list.Count)
			{
				throw new ArgumentException("There is no such element in collection.");
			}
			list.Remove(value);
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= list.Count)
			{
				throw new IndexOutOfRangeException("There is no element in collection.");
			}
			Remove(list[index]);
		}

		public void RemoveAt(string sourceColumn)
		{
			RemoveAt(list.IndexOf(sourceColumns[sourceColumn]));
		}
	}
}
