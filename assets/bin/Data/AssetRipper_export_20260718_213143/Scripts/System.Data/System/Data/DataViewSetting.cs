using System.ComponentModel;

namespace System.Data
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class DataViewSetting
	{
		private bool applyDefaultSort;

		private DataViewManager dataViewManager;

		private string rowFilter = string.Empty;

		private DataViewRowState rowStateFilter = DataViewRowState.CurrentRows;

		private string sort = string.Empty;

		private DataTable dataTable;

		public bool ApplyDefaultSort
		{
			get
			{
				return applyDefaultSort;
			}
			set
			{
				applyDefaultSort = value;
			}
		}

		[Browsable(false)]
		public DataViewManager DataViewManager
		{
			get
			{
				return dataViewManager;
			}
		}

		public string RowFilter
		{
			get
			{
				return rowFilter;
			}
			set
			{
				rowFilter = value;
			}
		}

		public DataViewRowState RowStateFilter
		{
			get
			{
				return rowStateFilter;
			}
			set
			{
				rowStateFilter = value;
			}
		}

		public string Sort
		{
			get
			{
				return sort;
			}
			set
			{
				sort = value;
			}
		}

		[Browsable(false)]
		public DataTable Table
		{
			get
			{
				return dataTable;
			}
		}

		internal DataViewSetting(DataViewManager manager, DataTable table)
		{
			dataViewManager = manager;
			dataTable = table;
		}
	}
}
