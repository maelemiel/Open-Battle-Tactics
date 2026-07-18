using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.TablesCollectionEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ListBindable(false)]
	[DefaultEvent("CollectionChanged")]
	public sealed class DataTableCollection : InternalDataCollectionBase
	{
		private DataSet dataSet;

		private DataTable[] mostRecentTables;

		public DataTable this[int index]
		{
			get
			{
				if (index < 0 || index >= List.Count)
				{
					throw new IndexOutOfRangeException(string.Format("Cannot find table {0}", index));
				}
				return (DataTable)List[index];
			}
		}

		public DataTable this[string name]
		{
			get
			{
				int num = IndexOf(name, true);
				return (num >= 0) ? ((DataTable)List[num]) : null;
			}
		}

		protected override ArrayList List
		{
			get
			{
				return base.List;
			}
		}

		public DataTable this[string name, string tbNamespace]
		{
			get
			{
				int num = IndexOf(name, tbNamespace, true);
				return (num >= 0) ? ((DataTable)List[num]) : null;
			}
		}

		[ResDescription("Occurs whenever this collection's membership changes.")]
		public event CollectionChangeEventHandler CollectionChanged;

		public event CollectionChangeEventHandler CollectionChanging;

		internal DataTableCollection(DataSet dataSet)
		{
			this.dataSet = dataSet;
		}

		public DataTable Add()
		{
			DataTable dataTable = new DataTable();
			Add(dataTable);
			return dataTable;
		}

		public void Add(DataTable table)
		{
			OnCollectionChanging(new CollectionChangeEventArgs(CollectionChangeAction.Add, table));
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			if (List.Contains(table))
			{
				throw new ArgumentException("DataTable already belongs to this DataSet.");
			}
			if (table.DataSet != null && table.DataSet != dataSet)
			{
				throw new ArgumentException("DataTable already belongs to another DataSet");
			}
			if (table.TableName == null || table.TableName == string.Empty)
			{
				NameTable(table);
			}
			int num = IndexOf(table.TableName, table.Namespace);
			if (num != -1 && table.TableName == this[num].TableName)
			{
				throw new DuplicateNameException("A DataTable named '" + table.TableName + "' already belongs to this DataSet.");
			}
			List.Add(table);
			table.dataSet = dataSet;
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, table));
		}

		public DataTable Add(string name)
		{
			DataTable dataTable = new DataTable(name);
			Add(dataTable);
			return dataTable;
		}

		public void AddRange(DataTable[] tables)
		{
			if (dataSet != null && dataSet.InitInProgress)
			{
				mostRecentTables = tables;
			}
			else
			{
				if (tables == null)
				{
					return;
				}
				foreach (DataTable dataTable in tables)
				{
					if (dataTable != null)
					{
						Add(dataTable);
					}
				}
			}
		}

		internal void PostAddRange()
		{
			if (mostRecentTables == null)
			{
				return;
			}
			DataTable[] array = mostRecentTables;
			foreach (DataTable dataTable in array)
			{
				if (dataTable != null)
				{
					Add(dataTable);
				}
			}
			mostRecentTables = null;
		}

		public bool CanRemove(DataTable table)
		{
			return CanRemove(table, false);
		}

		public void Clear()
		{
			List.Clear();
		}

		public bool Contains(string name)
		{
			return -1 != IndexOf(name, false);
		}

		public int IndexOf(DataTable table)
		{
			return List.IndexOf(table);
		}

		public int IndexOf(string tableName)
		{
			return IndexOf(tableName, false);
		}

		public void Remove(DataTable table)
		{
			OnCollectionChanging(new CollectionChangeEventArgs(CollectionChangeAction.Remove, table));
			if (CanRemove(table, true))
			{
				table.dataSet = null;
			}
			List.Remove(table);
			table.dataSet = null;
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, table));
		}

		public void Remove(string name)
		{
			int num = IndexOf(name, false);
			if (num == -1)
			{
				throw new ArgumentException("Table " + name + " does not belong to this DataSet");
			}
			RemoveAt(num);
		}

		public void RemoveAt(int index)
		{
			Remove(this[index]);
		}

		internal void OnCollectionChanging(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanging != null)
			{
				this.CollectionChanging(this, ccevent);
			}
		}

		internal void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, ccevent);
			}
		}

		private int IndexOf(string name, bool error, int start)
		{
			int num = 0;
			int result = -1;
			for (int i = start; i < List.Count; i++)
			{
				string tableName = ((DataTable)List[i]).TableName;
				if (string.Compare(name, tableName, false) == 0)
				{
					return i;
				}
				if (string.Compare(name, tableName, true) == 0)
				{
					result = i;
					num++;
				}
			}
			if (num == 1)
			{
				return result;
			}
			if (num > 1 && error)
			{
				throw new ArgumentException("There is no match for the name in the same case and there are multiple matches in different case.");
			}
			return -1;
		}

		private void NameTable(DataTable Table)
		{
			string text = "Table";
			int i;
			for (i = 1; Contains(text + i); i++)
			{
			}
			Table.TableName = text + i;
		}

		private bool CanRemove(DataTable table, bool throwException)
		{
			if (table == null)
			{
				if (throwException)
				{
					throw new ArgumentNullException("table");
				}
				return false;
			}
			if (table.DataSet != dataSet)
			{
				if (!throwException)
				{
					return false;
				}
				throw new ArgumentException("Table " + table.TableName + " does not belong to this DataSet.");
			}
			if (table.ParentRelations.Count > 0 || table.ChildRelations.Count > 0)
			{
				if (!throwException)
				{
					return false;
				}
				throw new ArgumentException("Cannot remove a table that has existing relations. Remove relations first.");
			}
			foreach (Constraint constraint in table.Constraints)
			{
				UniqueConstraint uniqueConstraint = constraint as UniqueConstraint;
				if (uniqueConstraint != null)
				{
					if (uniqueConstraint.ChildConstraint == null)
					{
						continue;
					}
					if (!throwException)
					{
						return false;
					}
					RaiseForeignKeyReferenceException(table.TableName, uniqueConstraint.ChildConstraint.ConstraintName);
				}
				ForeignKeyConstraint foreignKeyConstraint = constraint as ForeignKeyConstraint;
				if (foreignKeyConstraint != null)
				{
					if (!throwException)
					{
						return false;
					}
					RaiseForeignKeyReferenceException(table.TableName, foreignKeyConstraint.ConstraintName);
				}
			}
			return true;
		}

		private void RaiseForeignKeyReferenceException(string table, string constraint)
		{
			throw new ArgumentException(string.Format("Cannot remove table {0}, because it is referenced in ForeignKeyConstraint {1}. Remove the constraint first.", table, constraint));
		}

		public DataTable Add(string name, string tbNamespace)
		{
			DataTable dataTable = new DataTable(name, tbNamespace);
			Add(dataTable);
			return dataTable;
		}

		public bool Contains(string name, string tableNamespace)
		{
			return IndexOf(name, tableNamespace) != -1;
		}

		public int IndexOf(string tableName, string tableNamespace)
		{
			if (tableNamespace == null)
			{
				throw new ArgumentNullException("'tableNamespace' argument cannot be null.", "tableNamespace");
			}
			return IndexOf(tableName, tableNamespace, false);
		}

		public void Remove(string name, string tableNamespace)
		{
			int num = IndexOf(name, tableNamespace, true);
			if (num == -1)
			{
				throw new ArgumentException("Table " + name + " does not belong to this DataSet");
			}
			RemoveAt(num);
		}

		private int IndexOf(string name, string ns, bool error)
		{
			int num = -1;
			int num2 = 0;
			int result = -1;
			do
			{
				num = IndexOf(name, error, num + 1);
				if (num == -1)
				{
					break;
				}
				if (ns == null)
				{
					if (num2 > 1)
					{
						break;
					}
					num2++;
					result = num;
				}
				else if (this[num].Namespace.Equals(ns))
				{
					return num;
				}
			}
			while (num != -1 && num < Count);
			switch (num2)
			{
			case 1:
				return result;
			default:
				if (error)
				{
					throw new ArgumentException("The given name '" + name + "' matches atleast two namesin the collection object with different namespaces");
				}
				goto case 0;
			case 0:
				return -1;
			}
		}

		private int IndexOf(string name, bool error)
		{
			return IndexOf(name, null, error);
		}

		public void CopyTo(DataTable[] array, int index)
		{
			CopyTo((Array)array, index);
		}

		internal void BinarySerialize_Schema(SerializationInfo si)
		{
			si.AddValue("DataSet.Tables.Count", Count);
			for (int i = 0; i < Count; i++)
			{
				DataTable dataTable = (DataTable)List[i];
				if (dataTable.dataSet != dataSet)
				{
					throw new SystemException("Internal Error: inconsistent DataTable");
				}
				MemoryStream memoryStream = new MemoryStream();
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, dataTable);
				byte[] value = memoryStream.ToArray();
				memoryStream.Close();
				si.AddValue("DataSet.Tables_" + i, value, typeof(byte[]));
			}
		}

		internal void BinarySerialize_Data(SerializationInfo si)
		{
			for (int i = 0; i < Count; i++)
			{
				DataTable dataTable = (DataTable)List[i];
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					si.AddValue("DataTable_" + i + ".DataColumn_" + j + ".Expression", dataTable.Columns[j].Expression);
				}
				dataTable.BinarySerialize(si, "DataTable_" + i + ".");
			}
		}
	}
}
