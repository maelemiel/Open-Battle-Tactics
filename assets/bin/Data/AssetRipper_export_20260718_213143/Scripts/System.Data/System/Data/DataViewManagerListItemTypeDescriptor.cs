using System.ComponentModel;

namespace System.Data
{
	internal class DataViewManagerListItemTypeDescriptor : ICustomTypeDescriptor
	{
		private DataViewManager dvm;

		internal DataViewManager DataViewManager
		{
			get
			{
				return dvm;
			}
		}

		internal DataViewManagerListItemTypeDescriptor(DataViewManager dvm)
		{
			this.dvm = dvm;
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return new AttributeCollection((Attribute[])null);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return null;
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return null;
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return null;
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return null;
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return null;
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return null;
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return new EventDescriptorCollection(null);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return new EventDescriptorCollection(null);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			return GetProperties();
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public PropertyDescriptorCollection GetProperties()
		{
			DataSet dataSet = dvm.DataSet;
			if (dataSet == null)
			{
				return null;
			}
			DataTableCollection tables = dataSet.Tables;
			int num = 0;
			PropertyDescriptor[] array = new PropertyDescriptor[tables.Count];
			foreach (DataTable item in tables)
			{
				array[num++] = new DataTablePropertyDescriptor(item);
			}
			return new PropertyDescriptorCollection(array);
		}
	}
}
