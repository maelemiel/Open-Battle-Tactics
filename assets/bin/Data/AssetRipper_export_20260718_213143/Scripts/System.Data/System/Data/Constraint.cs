using System.ComponentModel;
using System.Data.Common;

namespace System.Data
{
	[DefaultProperty("ConstraintName")]
	[TypeConverter(typeof(ConstraintConverter))]
	public abstract class Constraint
	{
		private static readonly object beforeConstraintNameChange = new object();

		private EventHandlerList events = new EventHandlerList();

		private string _constraintName;

		private PropertyCollection _properties;

		private Index _index;

		private ConstraintCollection _constraintCollection;

		private DataSet dataSet;

		private bool initInProgress;

		[CLSCompliant(false)]
		protected internal virtual DataSet _DataSet
		{
			get
			{
				return dataSet;
			}
		}

		[DefaultValue("")]
		[DataCategory("Data")]
		public virtual string ConstraintName
		{
			get
			{
				return (_constraintName != null) ? _constraintName : string.Empty;
			}
			set
			{
				_onConstraintNameChange(value);
				_constraintName = value;
			}
		}

		[Browsable(false)]
		[DataCategory("Data")]
		public PropertyCollection ExtendedProperties
		{
			get
			{
				return _properties;
			}
		}

		public abstract DataTable Table { get; }

		internal ConstraintCollection ConstraintCollection
		{
			get
			{
				return _constraintCollection;
			}
			set
			{
				_constraintCollection = value;
			}
		}

		internal virtual bool InitInProgress
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

		internal Index Index
		{
			get
			{
				return _index;
			}
			set
			{
				if (_index != null)
				{
					_index.RemoveRef();
					Table.DropIndex(_index);
				}
				_index = value;
				if (_index != null)
				{
					_index.AddRef();
				}
			}
		}

		internal event DelegateConstraintNameChange BeforeConstraintNameChange
		{
			add
			{
				events.AddHandler(beforeConstraintNameChange, value);
			}
			remove
			{
				events.RemoveHandler(beforeConstraintNameChange, value);
			}
		}

		protected Constraint()
		{
			dataSet = null;
			_properties = new PropertyCollection();
		}

		private void _onConstraintNameChange(string newName)
		{
			DelegateConstraintNameChange delegateConstraintNameChange = events[beforeConstraintNameChange] as DelegateConstraintNameChange;
			if (delegateConstraintNameChange != null)
			{
				delegateConstraintNameChange(this, newName);
			}
		}

		internal abstract void AddToConstraintCollectionSetup(ConstraintCollection collection);

		internal abstract bool IsConstraintViolated();

		internal static void ThrowConstraintException()
		{
			throw new ConstraintException("Failed to enable constraints. One or more rows contain values violating non-null, unique, or foreign-key constraints.");
		}

		internal virtual void FinishInit(DataTable table)
		{
		}

		internal void AssertConstraint()
		{
			if (IsConstraintViolated() && !Table._duringDataLoad && (Table.DataSet == null || Table.DataSet.EnforceConstraints))
			{
				ThrowConstraintException();
			}
		}

		internal abstract void AssertConstraint(DataRow row);

		internal virtual void RollbackAssert(DataRow row)
		{
		}

		internal abstract void RemoveFromConstraintCollectionCleanup(ConstraintCollection collection);

		[System.MonoTODO]
		protected void CheckStateForProperty()
		{
			throw new NotImplementedException();
		}

		protected internal void SetDataSet(DataSet dataSet)
		{
			this.dataSet = dataSet;
		}

		internal void SetExtendedProperties(PropertyCollection properties)
		{
			_properties = properties;
		}

		internal abstract bool IsColumnContained(DataColumn column);

		internal abstract bool CanRemoveFromCollection(ConstraintCollection col, bool shouldThrow);

		public override string ToString()
		{
			return (_constraintName != null) ? _constraintName : string.Empty;
		}
	}
}
