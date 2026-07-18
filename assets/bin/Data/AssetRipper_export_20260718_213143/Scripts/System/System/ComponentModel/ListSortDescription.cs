namespace System.ComponentModel
{
	public class ListSortDescription
	{
		private PropertyDescriptor propertyDescriptor;

		private ListSortDirection sortDirection;

		public PropertyDescriptor PropertyDescriptor
		{
			get
			{
				return propertyDescriptor;
			}
			set
			{
				propertyDescriptor = value;
			}
		}

		public ListSortDirection SortDirection
		{
			get
			{
				return sortDirection;
			}
			set
			{
				sortDirection = value;
			}
		}

		public ListSortDescription(PropertyDescriptor propertyDescriptor, ListSortDirection sortDirection)
		{
			this.propertyDescriptor = propertyDescriptor;
			this.sortDirection = sortDirection;
		}
	}
}
