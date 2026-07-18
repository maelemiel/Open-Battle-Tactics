using System.ComponentModel;

namespace System.Data
{
	public class DataRowView : ICustomTypeDescriptor, IEditableObject, IDataErrorInfo, INotifyPropertyChanged
	{
		private DataView _dataView;

		private DataRow _dataRow;

		private int _index = -1;

		string IDataErrorInfo.Error
		{
			[System.MonoTODO("Not implemented, always returns String.Empty")]
			get
			{
				return string.Empty;
			}
		}

		string IDataErrorInfo.this[string colName]
		{
			[System.MonoTODO("Not implemented, always returns String.Empty")]
			get
			{
				return string.Empty;
			}
		}

		public DataView DataView
		{
			get
			{
				return _dataView;
			}
		}

		public bool IsEdit
		{
			get
			{
				return _dataRow.HasVersion(DataRowVersion.Proposed);
			}
		}

		public bool IsNew
		{
			get
			{
				return Row == DataView._lastAdded;
			}
		}

		public object this[string property]
		{
			get
			{
				DataColumn dataColumn = _dataView.Table.Columns[property];
				if (dataColumn == null)
				{
					throw new ArgumentException(property + " is neither a DataColumn nor a DataRelation for table " + _dataView.Table.TableName);
				}
				return _dataRow[dataColumn, GetActualRowVersion()];
			}
			set
			{
				CheckAllowEdit();
				DataColumn dataColumn = _dataView.Table.Columns[property];
				if (dataColumn == null)
				{
					throw new ArgumentException(property + " is neither a DataColumn nor a DataRelation for table " + _dataView.Table.TableName);
				}
				_dataRow[dataColumn] = value;
			}
		}

		public object this[int ndx]
		{
			get
			{
				DataColumn dataColumn = _dataView.Table.Columns[ndx];
				if (dataColumn == null)
				{
					throw new ArgumentException(ndx + " is neither a DataColumn nor a DataRelation for table " + _dataView.Table.TableName);
				}
				return _dataRow[dataColumn, GetActualRowVersion()];
			}
			set
			{
				CheckAllowEdit();
				DataColumn dataColumn = _dataView.Table.Columns[ndx];
				if (dataColumn == null)
				{
					throw new ArgumentException(ndx + " is neither a DataColumn nor a DataRelation for table " + _dataView.Table.TableName);
				}
				_dataRow[dataColumn] = value;
			}
		}

		public DataRow Row
		{
			get
			{
				return _dataRow;
			}
		}

		public DataRowVersion RowVersion
		{
			get
			{
				DataRowVersion dataRowVersion = DataView.GetRowVersion(_index);
				if (dataRowVersion != DataRowVersion.Original)
				{
					dataRowVersion = DataRowVersion.Current;
				}
				return dataRowVersion;
			}
		}

		internal int Index
		{
			get
			{
				return _index;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		internal DataRowView(DataView dataView, DataRow row, int index)
		{
			_dataView = dataView;
			_dataRow = row;
			_index = index;
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return AttributeCollection.Empty;
		}

		[System.MonoTODO("Not implemented.   Always returns String.Empty")]
		string ICustomTypeDescriptor.GetClassName()
		{
			return string.Empty;
		}

		[System.MonoTODO("Not implemented.   Always returns null")]
		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		[System.MonoTODO("Not implemented.   Always returns null")]
		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return null;
		}

		[System.MonoTODO("Not implemented.   Always returns null")]
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		[System.MonoTODO("Not implemented.   Always returns null")]
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		[System.MonoTODO("Not implemented.   Always returns null")]
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		[System.MonoTODO("Not implemented.   Always returns an empty collection")]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return new EventDescriptorCollection(null);
		}

		[System.MonoTODO("Not implemented.   Always returns an empty collection")]
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return new EventDescriptorCollection(null);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			if (DataView == null)
			{
				ITypedList dataView = _dataView;
				return dataView.GetItemProperties(new PropertyDescriptor[0]);
			}
			return DataView.Table.GetPropertyDescriptorCollection();
		}

		[System.MonoTODO("It currently reports more descriptors than necessary")]
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return ((ICustomTypeDescriptor)this).GetProperties();
		}

		[System.MonoTODO]
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public override bool Equals(object other)
		{
			return other != null && other is DataRowView && ((DataRowView)other)._dataRow != null && ((DataRowView)other)._dataRow.Equals(_dataRow);
		}

		public void BeginEdit()
		{
			_dataRow.BeginEdit();
		}

		public void CancelEdit()
		{
			if (Row == DataView._lastAdded)
			{
				DataView.CompleteLastAdded(false);
			}
			else
			{
				_dataRow.CancelEdit();
			}
		}

		public DataView CreateChildView(DataRelation relation)
		{
			return DataView.CreateChildView(relation, _index);
		}

		public DataView CreateChildView(string relationName)
		{
			return CreateChildView(Row.Table.ChildRelations[relationName]);
		}

		public void Delete()
		{
			DataView.Delete(_index);
		}

		public void EndEdit()
		{
			if (Row == DataView._lastAdded)
			{
				DataView.CompleteLastAdded(true);
			}
			else
			{
				_dataRow.EndEdit();
			}
		}

		private void CheckAllowEdit()
		{
			if (!DataView.AllowEdit && Row != DataView._lastAdded)
			{
				throw new DataException("Cannot edit on a DataSource where AllowEdit is false.");
			}
		}

		private DataRowVersion GetActualRowVersion()
		{
			switch (_dataView.RowStateFilter)
			{
			case DataViewRowState.Added:
				return DataRowVersion.Proposed;
			case DataViewRowState.Unchanged:
			case DataViewRowState.Deleted:
			case DataViewRowState.ModifiedOriginal:
			case DataViewRowState.OriginalRows:
				return DataRowVersion.Original;
			case DataViewRowState.ModifiedCurrent:
				return DataRowVersion.Current;
			default:
				return DataRowVersion.Default;
			}
		}

		public override int GetHashCode()
		{
			return _dataRow.GetHashCode();
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				PropertyChangedEventArgs e = new PropertyChangedEventArgs(propertyName);
				this.PropertyChanged(this, e);
			}
		}
	}
}
