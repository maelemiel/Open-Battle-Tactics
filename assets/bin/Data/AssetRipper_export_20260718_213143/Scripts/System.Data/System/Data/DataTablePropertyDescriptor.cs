using System.ComponentModel;

namespace System.Data
{
	internal class DataTablePropertyDescriptor : PropertyDescriptor
	{
		private DataTable table;

		public DataTable Table
		{
			get
			{
				return table;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type ComponentType
		{
			get
			{
				return typeof(DataRowView);
			}
		}

		public override Type PropertyType
		{
			get
			{
				return typeof(IBindingList);
			}
		}

		internal DataTablePropertyDescriptor(DataTable table)
			: base(table.TableName, null)
		{
			this.table = table;
		}

		public override object GetValue(object component)
		{
			DataViewManagerListItemTypeDescriptor dataViewManagerListItemTypeDescriptor = component as DataViewManagerListItemTypeDescriptor;
			if (dataViewManagerListItemTypeDescriptor == null)
			{
				return null;
			}
			return new DataView(table, dataViewManagerListItemTypeDescriptor.DataViewManager);
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override bool Equals(object other)
		{
			return other is DataTablePropertyDescriptor && ((DataTablePropertyDescriptor)other).table == table;
		}

		public override int GetHashCode()
		{
			return table.GetHashCode();
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
		}
	}
}
