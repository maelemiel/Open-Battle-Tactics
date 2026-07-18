using System.Collections;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace System.Data
{
	[DefaultProperty("Table")]
	[Editor("Microsoft.VSDesigner.Data.Design.DataRelationCollectionEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultEvent("CollectionChanged")]
	public abstract class DataRelationCollection : InternalDataCollectionBase
	{
		internal class DataSetRelationCollection : DataRelationCollection
		{
			private DataSet dataSet;

			private DataRelation[] mostRecentRelations;

			protected override ArrayList List
			{
				get
				{
					return base.List;
				}
			}

			public override DataRelation this[string name]
			{
				get
				{
					int num = IndexOf(name, true);
					return (num >= 0) ? ((DataRelation)List[num]) : null;
				}
			}

			public override DataRelation this[int index]
			{
				get
				{
					if (index < 0 || index >= List.Count)
					{
						throw new IndexOutOfRangeException(string.Format("Cannot find relation {0}.", index));
					}
					return (DataRelation)List[index];
				}
			}

			internal DataSetRelationCollection(DataSet dataSet)
			{
				this.dataSet = dataSet;
			}

			protected override DataSet GetDataSet()
			{
				return dataSet;
			}

			protected override void AddCore(DataRelation relation)
			{
				if (relation.ChildTable.DataSet != dataSet || relation.ParentTable.DataSet != dataSet)
				{
					throw new DataException();
				}
				base.AddCore(relation);
				relation.ParentTable.ChildRelations.Add(relation);
				relation.ChildTable.ParentRelations.Add(relation);
				relation.SetDataSet(dataSet);
				relation.UpdateConstraints();
			}

			protected override void RemoveCore(DataRelation relation)
			{
				base.RemoveCore(relation);
				relation.SetDataSet(null);
				relation.ParentTable.ChildRelations.Remove(relation);
				relation.ChildTable.ParentRelations.Remove(relation);
				relation.SetParentKeyConstraint(null);
				relation.SetChildKeyConstraint(null);
			}

			public override void AddRange(DataRelation[] relations)
			{
				if (relations == null)
				{
					return;
				}
				if (dataSet != null && dataSet.InitInProgress)
				{
					mostRecentRelations = relations;
					return;
				}
				foreach (DataRelation dataRelation in relations)
				{
					if (dataRelation != null)
					{
						Add(dataRelation);
					}
				}
			}

			internal override void PostAddRange()
			{
				if (mostRecentRelations == null)
				{
					return;
				}
				DataRelation[] array = mostRecentRelations;
				foreach (DataRelation dataRelation in array)
				{
					if (dataRelation != null)
					{
						if (dataRelation.InitInProgress)
						{
							dataRelation.FinishInit(dataSet);
						}
						Add(dataRelation);
					}
				}
				mostRecentRelations = null;
			}
		}

		internal class DataTableRelationCollection : DataRelationCollection
		{
			private DataTable dataTable;

			public override DataRelation this[string name]
			{
				get
				{
					int num = IndexOf(name, true);
					return (num >= 0) ? ((DataRelation)List[num]) : null;
				}
			}

			public override DataRelation this[int index]
			{
				get
				{
					if (index < 0 || index >= List.Count)
					{
						throw new IndexOutOfRangeException(string.Format("Cannot find relation {0}.", index));
					}
					return (DataRelation)List[index];
				}
			}

			protected override ArrayList List
			{
				get
				{
					return base.List;
				}
			}

			internal DataTableRelationCollection(DataTable dataTable)
			{
				this.dataTable = dataTable;
			}

			protected override DataSet GetDataSet()
			{
				return dataTable.DataSet;
			}

			protected override void AddCore(DataRelation relation)
			{
				if (dataTable.ParentRelations == this && relation.ChildTable != dataTable)
				{
					throw new ArgumentException("Cannot add a relation to this table's ParentRelations where this table is not the Child table.");
				}
				if (dataTable.ChildRelations == this && relation.ParentTable != dataTable)
				{
					throw new ArgumentException("Cannot add a relation to this table's ChildRelations where this table is not the Parent table.");
				}
				dataTable.DataSet.Relations.Add(relation);
				base.AddCore(relation);
			}

			protected override void RemoveCore(DataRelation relation)
			{
				relation.DataSet.Relations.Remove(relation);
				base.RemoveCore(relation);
			}
		}

		private DataRelation inTransition;

		private int index;

		public abstract DataRelation this[string name] { get; }

		public abstract DataRelation this[int index] { get; }

		[ResDescription("Occurs whenever this collection's membership changes.")]
		public event CollectionChangeEventHandler CollectionChanged;

		protected DataRelationCollection()
		{
			inTransition = null;
		}

		private string GetNextDefaultRelationName()
		{
			int num = 1;
			string text = "Relation" + num;
			while (Contains(text))
			{
				text = "Relation" + num;
				num++;
			}
			return text;
		}

		public void Add(DataRelation relation)
		{
			if (inTransition == relation)
			{
				return;
			}
			inTransition = relation;
			try
			{
				CollectionChangeEventArgs ccevent = new CollectionChangeEventArgs(CollectionChangeAction.Add, this);
				OnCollectionChanging(ccevent);
				AddCore(relation);
				if (relation.RelationName == string.Empty)
				{
					relation.RelationName = GenerateRelationName();
				}
				relation.ParentTable.ResetPropertyDescriptorsCache();
				relation.ChildTable.ResetPropertyDescriptorsCache();
				ccevent = new CollectionChangeEventArgs(CollectionChangeAction.Add, this);
				OnCollectionChanged(ccevent);
			}
			finally
			{
				inTransition = null;
			}
		}

		private string GenerateRelationName()
		{
			index++;
			return "Relation" + index;
		}

		public virtual DataRelation Add(DataColumn parentColumn, DataColumn childColumn)
		{
			DataRelation dataRelation = new DataRelation(GetNextDefaultRelationName(), parentColumn, childColumn);
			Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			DataRelation dataRelation = new DataRelation(GetNextDefaultRelationName(), parentColumns, childColumns);
			Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(string name, DataColumn parentColumn, DataColumn childColumn)
		{
			if (name == null || name == string.Empty)
			{
				name = GetNextDefaultRelationName();
			}
			DataRelation dataRelation = new DataRelation(name, parentColumn, childColumn);
			Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(string name, DataColumn[] parentColumns, DataColumn[] childColumns)
		{
			if (name == null || name == string.Empty)
			{
				name = GetNextDefaultRelationName();
			}
			DataRelation dataRelation = new DataRelation(name, parentColumns, childColumns);
			Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(string name, DataColumn parentColumn, DataColumn childColumn, bool createConstraints)
		{
			if (name == null || name == string.Empty)
			{
				name = GetNextDefaultRelationName();
			}
			DataRelation dataRelation = new DataRelation(name, parentColumn, childColumn, createConstraints);
			Add(dataRelation);
			return dataRelation;
		}

		public virtual DataRelation Add(string name, DataColumn[] parentColumns, DataColumn[] childColumns, bool createConstraints)
		{
			if (name == null || name == string.Empty)
			{
				name = GetNextDefaultRelationName();
			}
			DataRelation dataRelation = new DataRelation(name, parentColumns, childColumns, createConstraints);
			Add(dataRelation);
			return dataRelation;
		}

		protected virtual void AddCore(DataRelation relation)
		{
			if (relation == null)
			{
				throw new ArgumentNullException();
			}
			if (List.IndexOf(relation) != -1)
			{
				throw new ArgumentException();
			}
			int num = IndexOf(relation.RelationName);
			if (num != -1 && relation.RelationName == this[num].RelationName)
			{
				throw new DuplicateNameException("A DataRelation named '" + relation.RelationName + "' already belongs to this DataSet.");
			}
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DataRelation dataRelation = (DataRelation)enumerator.Current;
					bool flag = false;
					DataColumn[] childColumns = relation.ChildColumns;
					foreach (DataColumn dataColumn in childColumns)
					{
						bool flag2 = false;
						DataColumn[] childColumns2 = dataRelation.ChildColumns;
						foreach (DataColumn dataColumn2 in childColumns2)
						{
							if (dataColumn2 == dataColumn)
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
					flag = false;
					DataColumn[] parentColumns = relation.ParentColumns;
					foreach (DataColumn dataColumn3 in parentColumns)
					{
						bool flag3 = false;
						DataColumn[] parentColumns2 = dataRelation.ParentColumns;
						foreach (DataColumn dataColumn4 in parentColumns2)
						{
							if (dataColumn4 == dataColumn3)
							{
								flag3 = true;
								break;
							}
						}
						if (!flag3)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new ArgumentException("A relation already exists for these child columns");
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			List.Add(relation);
		}

		public virtual void AddRange(DataRelation[] relations)
		{
			if (relations != null)
			{
				foreach (DataRelation relation in relations)
				{
					Add(relation);
				}
			}
		}

		internal virtual void PostAddRange()
		{
		}

		public virtual bool CanRemove(DataRelation relation)
		{
			if (relation == null || !GetDataSet().Equals(relation.DataSet))
			{
				return false;
			}
			int num = IndexOf(relation.RelationName);
			return num != -1 && relation.RelationName == this[num].RelationName;
		}

		public virtual void Clear()
		{
			for (int i = 0; i < Count; i++)
			{
				Remove(this[i]);
			}
			List.Clear();
		}

		public virtual bool Contains(string name)
		{
			DataSet dataSet = GetDataSet();
			if (dataSet != null)
			{
				DataRelation dataRelation = dataSet.Relations[name];
				if (dataRelation != null)
				{
					return true;
				}
			}
			return -1 != IndexOf(name, false);
		}

		private CollectionChangeEventArgs CreateCollectionChangeEvent(CollectionChangeAction action)
		{
			return new CollectionChangeEventArgs(action, this);
		}

		protected abstract DataSet GetDataSet();

		public virtual int IndexOf(DataRelation relation)
		{
			return List.IndexOf(relation);
		}

		public virtual int IndexOf(string relationName)
		{
			return IndexOf(relationName, false);
		}

		private int IndexOf(string name, bool error)
		{
			int num = 0;
			int result = -1;
			for (int i = 0; i < List.Count; i++)
			{
				string relationName = ((DataRelation)List[i]).RelationName;
				if (string.Compare(name, relationName, true) == 0)
				{
					if (string.Compare(name, relationName, false) == 0)
					{
						return i;
					}
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

		protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, ccevent);
			}
		}

		protected virtual void OnCollectionChanging(CollectionChangeEventArgs ccevent)
		{
		}

		public void Remove(DataRelation relation)
		{
			if (inTransition == relation)
			{
				return;
			}
			inTransition = relation;
			if (relation == null)
			{
				return;
			}
			try
			{
				if (!List.Contains(relation))
				{
					throw new ArgumentException("Relation doesnot belong to this Collection.");
				}
				OnCollectionChanging(CreateCollectionChangeEvent(CollectionChangeAction.Remove));
				RemoveCore(relation);
				string text = "Relation" + index;
				if (relation.RelationName == text)
				{
					index--;
				}
				OnCollectionChanged(CreateCollectionChangeEvent(CollectionChangeAction.Remove));
			}
			finally
			{
				inTransition = null;
			}
		}

		public void Remove(string name)
		{
			DataRelation dataRelation = this[name];
			if (dataRelation == null)
			{
				throw new ArgumentException("Relation doesnot belong to this Collection.");
			}
			Remove(dataRelation);
		}

		public void RemoveAt(int index)
		{
			DataRelation dataRelation = this[index];
			if (dataRelation == null)
			{
				throw new IndexOutOfRangeException(string.Format("Cannot find relation {0}", index));
			}
			Remove(dataRelation);
		}

		protected virtual void RemoveCore(DataRelation relation)
		{
			List.Remove(relation);
		}

		public void CopyTo(DataRelation[] array, int index)
		{
			CopyTo((Array)array, index);
		}

		internal void BinarySerialize(SerializationInfo si)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < Count; i++)
			{
				DataRelation dataRelation = (DataRelation)List[i];
				ArrayList arrayList2 = new ArrayList();
				arrayList2.Add(dataRelation.RelationName);
				int[] array = new int[2];
				DataTable parentTable = dataRelation.ParentTable;
				array[0] = parentTable.DataSet.Tables.IndexOf(parentTable);
				array[1] = parentTable.Columns.IndexOf(dataRelation.ParentColumns[0]);
				arrayList2.Add(array);
				array = new int[2];
				parentTable = dataRelation.ChildTable;
				array[0] = parentTable.DataSet.Tables.IndexOf(parentTable);
				array[1] = parentTable.Columns.IndexOf(dataRelation.ChildColumns[0]);
				arrayList2.Add(array);
				arrayList2.Add(false);
				arrayList2.Add(null);
				arrayList.Add(arrayList2);
			}
			si.AddValue("DataSet.Relations", arrayList, typeof(ArrayList));
		}
	}
}
