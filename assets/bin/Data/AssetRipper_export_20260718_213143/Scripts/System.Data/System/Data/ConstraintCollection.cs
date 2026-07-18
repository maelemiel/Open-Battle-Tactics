using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.ConstraintsCollectionEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[DefaultEvent("CollectionChanged")]
	public sealed class ConstraintCollection : InternalDataCollectionBase
	{
		private DataTable table;

		private Constraint[] _mostRecentConstraints;

		internal DataTable Table
		{
			get
			{
				return table;
			}
		}

		public Constraint this[string name]
		{
			get
			{
				int num = IndexOf(name);
				return (num != -1) ? ((Constraint)List[num]) : null;
			}
		}

		public Constraint this[int index]
		{
			get
			{
				if (index < 0 || index >= List.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return (Constraint)List[index];
			}
		}

		protected override ArrayList List
		{
			get
			{
				return base.List;
			}
		}

		public event CollectionChangeEventHandler CollectionChanged;

		internal ConstraintCollection(DataTable table)
		{
			this.table = table;
		}

		private void _handleBeforeConstraintNameChange(object sender, string newName)
		{
			if (newName == null || newName == string.Empty)
			{
				throw new ArgumentException("ConstraintName cannot be set to null or empty after adding it to a ConstraintCollection.");
			}
			if (_isDuplicateConstraintName(newName, (Constraint)sender))
			{
				throw new DuplicateNameException("Constraint name already exists.");
			}
		}

		private bool _isDuplicateConstraintName(string constraintName, Constraint excludeFromComparison)
		{
			foreach (Constraint item in List)
			{
				if (item == excludeFromComparison || string.Compare(constraintName, item.ConstraintName, false, Table.Locale) != 0)
				{
					continue;
				}
				return true;
			}
			return false;
		}

		private string _createNewConstraintName()
		{
			int num = 1;
			string text;
			while (true)
			{
				text = "Constraint" + num;
				if (IndexOf(text) == -1)
				{
					break;
				}
				num++;
			}
			return text;
		}

		public void Add(Constraint constraint)
		{
			if (constraint == null)
			{
				throw new ArgumentNullException("Can not add null.");
			}
			if (constraint.InitInProgress)
			{
				throw new ArgumentException("Hmm .. Failed to Add to collection");
			}
			if (this == constraint.ConstraintCollection)
			{
				throw new ArgumentException("Constraint already belongs to this collection.");
			}
			if (constraint.ConstraintCollection != null)
			{
				throw new ArgumentException("Constraint already belongs to another collection.");
			}
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Constraint constraint2 = (Constraint)enumerator.Current;
					if (constraint2.Equals(constraint))
					{
						throw new DataException("Constraint matches contraint named '" + constraint2.ConstraintName + "' already in collection");
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
			if (_isDuplicateConstraintName(constraint.ConstraintName, null))
			{
				throw new DuplicateNameException("Constraint name already exists.");
			}
			constraint.AddToConstraintCollectionSetup(this);
			if (constraint.ConstraintName == null || constraint.ConstraintName == string.Empty)
			{
				constraint.ConstraintName = _createNewConstraintName();
			}
			constraint.BeforeConstraintNameChange += _handleBeforeConstraintNameChange;
			constraint.ConstraintCollection = this;
			List.Add(constraint);
			if (constraint is UniqueConstraint && ((UniqueConstraint)constraint).IsPrimaryKey)
			{
				table.PrimaryKey = ((UniqueConstraint)constraint).Columns;
			}
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, this));
		}

		public Constraint Add(string name, DataColumn column, bool primaryKey)
		{
			UniqueConstraint uniqueConstraint = new UniqueConstraint(name, column, primaryKey);
			Add(uniqueConstraint);
			return uniqueConstraint;
		}

		public Constraint Add(string name, DataColumn primaryKeyColumn, DataColumn foreignKeyColumn)
		{
			ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(name, primaryKeyColumn, foreignKeyColumn);
			Add(foreignKeyConstraint);
			return foreignKeyConstraint;
		}

		public Constraint Add(string name, DataColumn[] columns, bool primaryKey)
		{
			UniqueConstraint uniqueConstraint = new UniqueConstraint(name, columns, primaryKey);
			Add(uniqueConstraint);
			return uniqueConstraint;
		}

		public Constraint Add(string name, DataColumn[] primaryKeyColumns, DataColumn[] foreignKeyColumns)
		{
			ForeignKeyConstraint foreignKeyConstraint = new ForeignKeyConstraint(name, primaryKeyColumns, foreignKeyColumns);
			Add(foreignKeyConstraint);
			return foreignKeyConstraint;
		}

		public void AddRange(Constraint[] constraints)
		{
			if (Table.InitInProgress)
			{
				_mostRecentConstraints = constraints;
			}
			else
			{
				if (constraints == null)
				{
					return;
				}
				for (int i = 0; i < constraints.Length; i++)
				{
					if (constraints[i] != null)
					{
						Add(constraints[i]);
					}
				}
			}
		}

		internal void PostAddRange()
		{
			if (_mostRecentConstraints == null)
			{
				return;
			}
			for (int i = 0; i < _mostRecentConstraints.Length; i++)
			{
				Constraint constraint = _mostRecentConstraints[i];
				if (constraint != null)
				{
					if (constraint.InitInProgress)
					{
						constraint.FinishInit(Table);
					}
					Add(constraint);
				}
			}
			_mostRecentConstraints = null;
		}

		public bool CanRemove(Constraint constraint)
		{
			return constraint.CanRemoveFromCollection(this, false);
		}

		public void Clear()
		{
			Table.PrimaryKey = null;
			foreach (Constraint item in List)
			{
				item.ConstraintCollection = null;
				item.BeforeConstraintNameChange -= _handleBeforeConstraintNameChange;
			}
			List.Clear();
			OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, this));
		}

		public bool Contains(string name)
		{
			return -1 != IndexOf(name);
		}

		public int IndexOf(Constraint constraint)
		{
			int num = 0;
			IEnumerator enumerator = GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Constraint constraint2 = (Constraint)enumerator.Current;
					if (constraint2 == constraint)
					{
						return num;
					}
					num++;
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
			return -1;
		}

		public int IndexOf(string constraintName)
		{
			int num = 0;
			foreach (Constraint item in List)
			{
				if (string.Compare(constraintName, item.ConstraintName, !Table.CaseSensitive, Table.Locale) == 0)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public void Remove(Constraint constraint)
		{
			if (constraint == null)
			{
				throw new ArgumentNullException();
			}
			if (constraint.CanRemoveFromCollection(this, true))
			{
				constraint.RemoveFromConstraintCollectionCleanup(this);
				constraint.ConstraintCollection = null;
				List.Remove(constraint);
				OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, this));
			}
		}

		public void Remove(string name)
		{
			int num = IndexOf(name);
			if (num == -1)
			{
				throw new ArgumentException("Constraint '" + name + "' does not belong to this DataTable.");
			}
			Remove(this[num]);
		}

		public void RemoveAt(int index)
		{
			Remove(this[index]);
		}

		internal void OnCollectionChanged(CollectionChangeEventArgs ccevent)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, ccevent);
			}
		}

		public void CopyTo(Constraint[] array, int index)
		{
			base.CopyTo(array, index);
		}
	}
}
