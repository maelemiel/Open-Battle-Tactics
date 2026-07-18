using System.ComponentModel;
using System.Data.Common;

namespace System.Data
{
	internal class DataColumnPropertyDescriptor : PropertyDescriptor
	{
		private bool readOnly = true;

		private Type componentType;

		private Type propertyType;

		private bool browsable = true;

		private int columnIndex;

		public override Type ComponentType
		{
			get
			{
				return componentType;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return readOnly;
			}
		}

		public override bool IsBrowsable
		{
			get
			{
				return browsable && base.IsBrowsable;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return propertyType;
			}
		}

		public DataColumnPropertyDescriptor(string name, int columnIndex, Attribute[] attrs)
			: base(name, attrs)
		{
			this.columnIndex = columnIndex;
		}

		public DataColumnPropertyDescriptor(DataColumn dc)
			: base(dc.ColumnName, null)
		{
			columnIndex = dc.Ordinal;
			componentType = typeof(DataRowView);
			propertyType = dc.DataType;
			readOnly = dc.ReadOnly;
		}

		public void SetReadOnly(bool value)
		{
			readOnly = value;
		}

		public void SetComponentType(Type type)
		{
			componentType = type;
		}

		public void SetPropertyType(Type type)
		{
			propertyType = type;
		}

		public void SetBrowsable(bool browsable)
		{
			this.browsable = browsable;
		}

		public override object GetValue(object component)
		{
			if (componentType == typeof(DataRowView) && component is DataRowView)
			{
				DataRowView dataRowView = (DataRowView)component;
				return dataRowView[base.Name];
			}
			if (componentType == typeof(DbDataRecord) && component is DbDataRecord)
			{
				DbDataRecord dbDataRecord = (DbDataRecord)component;
				return dbDataRecord[columnIndex];
			}
			throw new InvalidOperationException();
		}

		public override void SetValue(object component, object value)
		{
			DataRowView dataRowView = (DataRowView)component;
			dataRowView[base.Name] = value;
		}

		[System.MonoTODO]
		public override void ResetValue(object component)
		{
		}

		[System.MonoTODO]
		public override bool CanResetValue(object component)
		{
			return false;
		}

		[System.MonoTODO]
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
