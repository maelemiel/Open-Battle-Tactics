using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Text;
using Mono.Data.SqlExpressions;

namespace System.Data
{
	[DefaultEvent("PositionChanged")]
	[DefaultProperty("Table")]
	[Designer("Microsoft.VSDesigner.Data.VS.DataViewDesigner, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	[Editor("Microsoft.VSDesigner.Data.Design.DataSourceEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class DataView : MarshalByValueComponent, IList, ICollection, IEnumerable, IBindingList, ITypedList, IBindingListView, ISupportInitialize, ISupportInitializeNotification
	{
		internal DataTable dataTable;

		private string rowFilter = string.Empty;

		private IExpression rowFilterExpr;

		private string sort = string.Empty;

		private ListSortDirection[] sortOrder;

		private PropertyDescriptor sortProperty;

		private DataColumn[] sortColumns;

		internal DataViewRowState rowState;

		internal DataRowView[] rowCache = new DataRowView[0];

		private bool isInitPhase;

		private bool inEndInit;

		private DataTable initTable;

		private bool initApplyDefaultSort;

		private string initSort;

		private string initRowFilter;

		private DataViewRowState initRowState;

		private bool allowNew = true;

		private bool allowEdit = true;

		private bool allowDelete = true;

		private bool applyDefaultSort;

		private bool isOpen;

		private bool useDefaultSort = true;

		private Index _index;

		internal DataRow _lastAdded;

		private DataViewManager dataViewManager;

		internal static ListChangedEventArgs ListResetEventArgs = new ListChangedEventArgs(ListChangedType.Reset, -1, -1);

		private bool dataViewInitialized = true;

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		object IList.this[int recordIndex]
		{
			get
			{
				return this[recordIndex];
			}
			[System.MonoTODO]
			set
			{
				throw new InvalidOperationException();
			}
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return AllowEdit;
			}
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return AllowNew;
			}
		}

		bool IBindingList.AllowRemove
		{
			[System.MonoTODO]
			get
			{
				return AllowDelete;
			}
		}

		bool IBindingList.IsSorted
		{
			get
			{
				return Sort != null && Sort.Length != 0;
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				if (sortOrder != null && sortOrder.Length > 0)
				{
					return sortOrder[0];
				}
				return ListSortDirection.Ascending;
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				if (sortProperty == null && sortColumns != null && sortColumns.Length > 0)
				{
					PropertyDescriptorCollection itemProperties = ((ITypedList)this).GetItemProperties((PropertyDescriptor[])null);
					return itemProperties.Find(sortColumns[0].ColumnName, false);
				}
				return sortProperty;
			}
		}

		bool IBindingList.SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSearching
		{
			get
			{
				return true;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return true;
			}
		}

		string IBindingListView.Filter
		{
			get
			{
				return RowFilter;
			}
			set
			{
				RowFilter = value;
			}
		}

		ListSortDescriptionCollection IBindingListView.SortDescriptions
		{
			get
			{
				ListSortDescriptionCollection listSortDescriptionCollection = new ListSortDescriptionCollection();
				for (int i = 0; i < sortColumns.Length; i++)
				{
					ListSortDescription value = new ListSortDescription(new DataColumnPropertyDescriptor(sortColumns[i]), sortOrder[i]);
					((IList)listSortDescriptionCollection).Add((object)value);
				}
				return listSortDescriptionCollection;
			}
		}

		bool IBindingListView.SupportsAdvancedSorting
		{
			get
			{
				return true;
			}
		}

		bool IBindingListView.SupportsFiltering
		{
			get
			{
				return true;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(true)]
		public bool AllowDelete
		{
			get
			{
				return allowDelete;
			}
			set
			{
				allowDelete = value;
			}
		}

		[DataCategory("Data")]
		[DefaultValue(true)]
		public bool AllowEdit
		{
			get
			{
				return allowEdit;
			}
			set
			{
				allowEdit = value;
			}
		}

		[DefaultValue(true)]
		[DataCategory("Data")]
		public bool AllowNew
		{
			get
			{
				return allowNew;
			}
			set
			{
				allowNew = value;
			}
		}

		[DataCategory("Data")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(false)]
		public bool ApplyDefaultSort
		{
			get
			{
				return applyDefaultSort;
			}
			set
			{
				if (isInitPhase)
				{
					initApplyDefaultSort = value;
				}
				else if (applyDefaultSort != value)
				{
					applyDefaultSort = value;
					if (applyDefaultSort && (sort == null || sort == string.Empty))
					{
						PopulateDefaultSort();
					}
					if (!inEndInit)
					{
						UpdateIndex(true);
						OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
					}
				}
			}
		}

		[Browsable(false)]
		public int Count
		{
			get
			{
				return rowCache.Length;
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

		public DataRowView this[int recordIndex]
		{
			get
			{
				if (recordIndex > rowCache.Length)
				{
					throw new IndexOutOfRangeException("There is no row at position: " + recordIndex + ".");
				}
				return rowCache[recordIndex];
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public virtual string RowFilter
		{
			get
			{
				return rowFilter;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				if (isInitPhase)
				{
					initRowFilter = value;
					return;
				}
				CultureInfo culture = ((Table == null) ? CultureInfo.CurrentCulture : Table.Locale);
				if (string.Compare(rowFilter, value, false, culture) != 0)
				{
					if (value.Length == 0)
					{
						rowFilterExpr = null;
					}
					else
					{
						Parser parser = new Parser();
						rowFilterExpr = parser.Compile(value);
					}
					rowFilter = value;
					if (!inEndInit)
					{
						UpdateIndex(true);
						OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
					}
				}
			}
		}

		[DataCategory("Data")]
		[DefaultValue(DataViewRowState.CurrentRows)]
		public DataViewRowState RowStateFilter
		{
			get
			{
				return rowState;
			}
			set
			{
				if (isInitPhase)
				{
					initRowState = value;
				}
				else if (value != rowState)
				{
					rowState = value;
					if (!inEndInit)
					{
						UpdateIndex(true);
						OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
					}
				}
			}
		}

		[DataCategory("Data")]
		[DefaultValue("")]
		public string Sort
		{
			get
			{
				if (useDefaultSort)
				{
					return string.Empty;
				}
				return sort;
			}
			set
			{
				if (isInitPhase)
				{
					initSort = value;
				}
				else
				{
					if (value == sort)
					{
						return;
					}
					if (value == null || value.Length == 0)
					{
						useDefaultSort = true;
						if (ApplyDefaultSort)
						{
							PopulateDefaultSort();
						}
					}
					else
					{
						useDefaultSort = false;
						sort = value;
					}
					if (!inEndInit)
					{
						UpdateIndex(true);
						OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
					}
				}
			}
		}

		[TypeConverter(typeof(DataTableTypeConverter))]
		[DataCategory("Data")]
		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		public DataTable Table
		{
			get
			{
				return dataTable;
			}
			set
			{
				if (value == dataTable)
				{
					return;
				}
				if (isInitPhase)
				{
					initTable = value;
					return;
				}
				if (value != null && value.TableName.Equals(string.Empty))
				{
					throw new DataException("Cannot bind to DataTable with no name.");
				}
				if (dataTable != null)
				{
					UnregisterEventHandlers();
				}
				dataTable = value;
				if (dataTable != null)
				{
					RegisterEventHandlers();
					OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, 0, 0));
					sort = string.Empty;
					rowFilter = string.Empty;
					if (!inEndInit)
					{
						UpdateIndex(true);
						OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
					}
				}
			}
		}

		[Browsable(false)]
		protected bool IsOpen
		{
			get
			{
				return isOpen;
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

		internal virtual IExpression FilterExpression
		{
			get
			{
				return rowFilterExpr;
			}
		}

		[Browsable(false)]
		public bool IsInitialized
		{
			get
			{
				return dataViewInitialized;
			}
		}

		[DataCategory("Data")]
		public event ListChangedEventHandler ListChanged;

		public event EventHandler Initialized;

		public DataView()
		{
			rowState = DataViewRowState.CurrentRows;
			Open();
		}

		public DataView(DataTable table)
			: this(table, null)
		{
		}

		internal DataView(DataTable table, DataViewManager manager)
		{
			dataTable = table;
			rowState = DataViewRowState.CurrentRows;
			dataViewManager = manager;
			Open();
		}

		public DataView(DataTable table, string RowFilter, string Sort, DataViewRowState RowState)
			: this(table, null, RowFilter, Sort, RowState)
		{
		}

		internal DataView(DataTable table, DataViewManager manager, string RowFilter, string Sort, DataViewRowState RowState)
		{
			dataTable = table;
			dataViewManager = manager;
			rowState = DataViewRowState.CurrentRows;
			this.RowFilter = RowFilter;
			this.Sort = Sort;
			rowState = RowState;
			Open();
		}

		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (dataTable == null)
			{
				return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
			}
			PropertyDescriptor[] array = new PropertyDescriptor[dataTable.Columns.Count + dataTable.ChildRelations.Count];
			int num = 0;
			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				DataColumn dataColumn = dataTable.Columns[i];
				DataColumnPropertyDescriptor dataColumnPropertyDescriptor = new DataColumnPropertyDescriptor(dataColumn.ColumnName, i, null);
				dataColumnPropertyDescriptor.SetComponentType(typeof(DataRowView));
				dataColumnPropertyDescriptor.SetPropertyType(dataColumn.DataType);
				dataColumnPropertyDescriptor.SetReadOnly(dataColumn.ReadOnly);
				dataColumnPropertyDescriptor.SetBrowsable(dataColumn.ColumnMapping != MappingType.Hidden);
				array[num++] = dataColumnPropertyDescriptor;
			}
			for (int j = 0; j < dataTable.ChildRelations.Count; j++)
			{
				DataRelation relation = dataTable.ChildRelations[j];
				DataRelationPropertyDescriptor dataRelationPropertyDescriptor = new DataRelationPropertyDescriptor(relation);
				array[num++] = dataRelationPropertyDescriptor;
			}
			return new PropertyDescriptorCollection(array);
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			if (dataTable != null)
			{
				return dataTable.TableName;
			}
			return string.Empty;
		}

		int IList.Add(object value)
		{
			throw new ArgumentException("Cannot add external objects to this list.");
		}

		void IList.Clear()
		{
			throw new ArgumentException("Cannot clear this list.");
		}

		bool IList.Contains(object value)
		{
			DataRowView dataRowView = value as DataRowView;
			if (dataRowView == null)
			{
				return false;
			}
			return dataRowView.DataView == this;
		}

		int IList.IndexOf(object value)
		{
			DataRowView dataRowView = value as DataRowView;
			if (dataRowView != null && dataRowView.DataView == this)
			{
				return dataRowView.Index;
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			throw new ArgumentException("Cannot insert external objects to this list.");
		}

		void IList.Remove(object value)
		{
			DataRowView dataRowView = value as DataRowView;
			if (dataRowView != null && dataRowView.DataView == this)
			{
				((IList)this).RemoveAt(dataRowView.Index);
			}
			throw new ArgumentException("Cannot remove external objects to this list.");
		}

		void IList.RemoveAt(int index)
		{
			Delete(index);
		}

		[System.MonoTODO]
		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			throw new NotImplementedException();
		}

		object IBindingList.AddNew()
		{
			return AddNew();
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			if (!(property is DataColumnPropertyDescriptor))
			{
				throw new ArgumentException("Dataview accepts only DataColumnPropertyDescriptors", "property");
			}
			sortProperty = property;
			string text = string.Format("[{0}]", property.Name);
			if (direction == ListSortDirection.Descending)
			{
				text += " DESC";
			}
			Sort = text;
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			DataColumn dataColumn = Table.Columns[property.Name];
			Index index = Table.FindIndex(new DataColumn[1] { dataColumn }, sortOrder, RowStateFilter, FilterExpression);
			if (index == null)
			{
				index = new Index(new Key(Table, new DataColumn[1] { dataColumn }, sortOrder, RowStateFilter, FilterExpression));
			}
			return index.FindIndex(new object[1] { key });
		}

		[System.MonoTODO]
		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			throw new NotImplementedException();
		}

		void IBindingList.RemoveSort()
		{
			sortProperty = null;
			Sort = string.Empty;
		}

		[System.MonoTODO]
		void IBindingListView.ApplySort(ListSortDescriptionCollection sorts)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ListSortDescription item in (IEnumerable)sorts)
			{
				stringBuilder.AppendFormat("[{0}]{1},", item.PropertyDescriptor.Name, (item.SortDirection != ListSortDirection.Descending) ? string.Empty : " DESC");
			}
			Sort = stringBuilder.ToString(0, stringBuilder.Length - 1);
		}

		void IBindingListView.RemoveFilter()
		{
			((IBindingListView)this).Filter = string.Empty;
		}

		public virtual DataRowView AddNew()
		{
			if (!IsOpen)
			{
				throw new DataException("DataView is not open.");
			}
			if (!AllowNew)
			{
				throw new DataException("Cannot call AddNew on a DataView where AllowNew is false.");
			}
			if (_lastAdded != null)
			{
				CompleteLastAdded(true);
			}
			_lastAdded = dataTable.NewRow();
			UpdateIndex(true);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, Count - 1, -1));
			return this[Count - 1];
		}

		internal void CompleteLastAdded(bool add)
		{
			DataRow lastAdded = _lastAdded;
			if (add)
			{
				try
				{
					dataTable.Rows.Add(_lastAdded);
					_lastAdded = null;
					UpdateIndex();
					return;
				}
				catch (Exception)
				{
					_lastAdded = lastAdded;
					throw;
				}
			}
			_lastAdded.CancelEdit();
			_lastAdded = null;
			UpdateIndex();
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, Count, -1));
		}

		public void BeginInit()
		{
			initTable = Table;
			initApplyDefaultSort = ApplyDefaultSort;
			initSort = Sort;
			initRowFilter = RowFilter;
			initRowState = RowStateFilter;
			isInitPhase = true;
			DataViewInitialized(false);
		}

		public void CopyTo(Array array, int index)
		{
			if (index + rowCache.Length > array.Length)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = 0; i < rowCache.Length && i < array.Length; i++)
			{
				array.SetValue(rowCache[i], index + i);
			}
		}

		public void Delete(int index)
		{
			if (!IsOpen)
			{
				throw new DataException("DataView is not open.");
			}
			if (_lastAdded != null && index == Count)
			{
				CompleteLastAdded(false);
				return;
			}
			if (!AllowDelete)
			{
				throw new DataException("Cannot delete on a DataSource where AllowDelete is false.");
			}
			if (index > rowCache.Length)
			{
				throw new IndexOutOfRangeException("There is no row at position: " + index + ".");
			}
			DataRowView dataRowView = rowCache[index];
			dataRowView.Row.Delete();
		}

		public void EndInit()
		{
			isInitPhase = false;
			inEndInit = true;
			Table = initTable;
			ApplyDefaultSort = initApplyDefaultSort;
			Sort = initSort;
			RowFilter = initRowFilter;
			RowStateFilter = initRowState;
			inEndInit = false;
			UpdateIndex(true);
			DataViewInitialized(true);
		}

		public int Find(object key)
		{
			object[] key2 = new object[1] { key };
			return Find(key2);
		}

		public int Find(object[] key)
		{
			if (sort == null || sort.Length == 0)
			{
				throw new ArgumentException("Find finds a row based on a Sort order, and no Sort order is specified");
			}
			if (Index == null)
			{
				UpdateIndex(true);
			}
			int result = -1;
			try
			{
				result = Index.FindIndex(key);
			}
			catch (FormatException)
			{
			}
			catch (InvalidCastException)
			{
			}
			return result;
		}

		public DataRowView[] FindRows(object key)
		{
			return FindRows(new object[1] { key });
		}

		public DataRowView[] FindRows(object[] key)
		{
			if (sort == null || sort.Length == 0)
			{
				throw new ArgumentException("Find finds a row based on a Sort order, and no Sort order is specified");
			}
			if (Index == null)
			{
				UpdateIndex(true);
			}
			int[] array = Index.FindAllIndexes(key);
			DataRowView[] array2 = new DataRowView[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = rowCache[array[i]];
			}
			return array2;
		}

		public IEnumerator GetEnumerator()
		{
			DataRowView[] array = new DataRowView[Count];
			CopyTo(array, 0);
			return array.GetEnumerator();
		}

		protected void Close()
		{
			if (dataTable != null)
			{
				UnregisterEventHandlers();
			}
			Index = null;
			rowCache = new DataRowView[0];
			isOpen = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Close();
			}
			base.Dispose(disposing);
		}

		protected virtual void IndexListChanged(object sender, ListChangedEventArgs e)
		{
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			try
			{
				if (this.ListChanged != null)
				{
					this.ListChanged(this, e);
				}
			}
			catch
			{
			}
		}

		internal void ChangedList(ListChangedType listChangedType, int newIndex, int oldIndex)
		{
			ListChangedEventArgs e = new ListChangedEventArgs(listChangedType, newIndex, oldIndex);
			OnListChanged(e);
		}

		protected void Open()
		{
			UpdateIndex(true);
			if (dataTable != null)
			{
				RegisterEventHandlers();
			}
			isOpen = true;
		}

		private void RegisterEventHandlers()
		{
			dataTable.ColumnChanged += OnColumnChanged;
			dataTable.RowChanged += OnRowChanged;
			dataTable.RowDeleted += OnRowDeleted;
			dataTable.Columns.CollectionChanged += ColumnCollectionChanged;
			dataTable.Columns.CollectionMetaDataChanged += ColumnCollectionChanged;
			dataTable.Constraints.CollectionChanged += OnConstraintCollectionChanged;
			dataTable.ChildRelations.CollectionChanged += OnRelationCollectionChanged;
			dataTable.ParentRelations.CollectionChanged += OnRelationCollectionChanged;
			dataTable.Rows.ListChanged += OnRowCollectionChanged;
		}

		private void OnRowCollectionChanged(object sender, ListChangedEventArgs args)
		{
			if (args.ListChangedType == ListChangedType.Reset)
			{
				rowCache = new DataRowView[0];
				UpdateIndex(true);
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
			}
		}

		private void UnregisterEventHandlers()
		{
			dataTable.ColumnChanged -= OnColumnChanged;
			dataTable.RowChanged -= OnRowChanged;
			dataTable.RowDeleted -= OnRowDeleted;
			dataTable.Columns.CollectionChanged -= ColumnCollectionChanged;
			dataTable.Columns.CollectionMetaDataChanged -= ColumnCollectionChanged;
			dataTable.Constraints.CollectionChanged -= OnConstraintCollectionChanged;
			dataTable.ChildRelations.CollectionChanged -= OnRelationCollectionChanged;
			dataTable.ParentRelations.CollectionChanged -= OnRelationCollectionChanged;
			dataTable.Rows.ListChanged -= OnRowCollectionChanged;
		}

		private void OnColumnChanged(object sender, DataColumnChangeEventArgs args)
		{
		}

		private void OnRowChanged(object sender, DataRowChangeEventArgs args)
		{
			int num = -1;
			int num2 = IndexOf(args.Row);
			UpdateIndex(true);
			num = IndexOf(args.Row);
			if (args.Action == DataRowAction.Add && num2 != num)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, num, -1));
			}
			if (args.Action == DataRowAction.Change)
			{
				if (num2 != -1 && num2 == num)
				{
					OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, num, -1));
				}
				else if (num2 != num)
				{
					if (num < 0)
					{
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, num, num2));
					}
					else
					{
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemMoved, num, num2));
					}
				}
			}
			if (args.Action == DataRowAction.Rollback)
			{
				if (num2 < 0 && num > -1)
				{
					OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, num, -1));
				}
				else if (num2 > -1 && num < 0)
				{
					OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, num, num2));
				}
				else if (num2 != -1 && num2 == num)
				{
					OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, num, -1));
				}
			}
		}

		private void OnRowDeleted(object sender, DataRowChangeEventArgs args)
		{
			int count = Count;
			int newIndex = IndexOf(args.Row);
			UpdateIndex(true);
			if (count != Count)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, newIndex, -1));
			}
		}

		protected virtual void ColumnCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Add)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, 0, 0));
			}
			if (e.Action == CollectionChangeAction.Remove)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, 0, 0));
			}
			if (e.Action == CollectionChangeAction.Refresh)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, 0, 0));
			}
		}

		private void OnConstraintCollectionChanged(object sender, CollectionChangeEventArgs args)
		{
			if (args.Action == CollectionChangeAction.Add && args.Element is UniqueConstraint && ApplyDefaultSort && useDefaultSort)
			{
				PopulateDefaultSort((UniqueConstraint)args.Element);
			}
		}

		private void OnRelationCollectionChanged(object sender, CollectionChangeEventArgs args)
		{
			if (args.Action == CollectionChangeAction.Add)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, 0, 0));
			}
			if (args.Action == CollectionChangeAction.Remove)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, 0, 0));
			}
			if (args.Action == CollectionChangeAction.Refresh)
			{
				OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, 0, 0));
			}
		}

		protected void Reset()
		{
			Close();
			rowCache = new DataRowView[0];
			Open();
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1, -1));
		}

		protected void UpdateIndex()
		{
			UpdateIndex(false);
		}

		protected virtual void UpdateIndex(bool force)
		{
			if (Table != null)
			{
				if (Index == null || force)
				{
					sortColumns = DataTable.ParseSortString(Table, Sort, out sortOrder, false);
					Index = dataTable.GetIndex(sortColumns, sortOrder, RowStateFilter, FilterExpression, true);
				}
				else
				{
					Index.Key.RowStateFilter = RowStateFilter;
					Index.Reset();
				}
				int[] all = Index.GetAll();
				if (all != null)
				{
					InitDataRowViewArray(all, Index.Size);
				}
				else
				{
					rowCache = new DataRowView[0];
				}
			}
		}

		private void InitDataRowViewArray(int[] records, int size)
		{
			if (_lastAdded != null)
			{
				rowCache = new DataRowView[size + 1];
			}
			else
			{
				rowCache = new DataRowView[size];
			}
			for (int i = 0; i < size; i++)
			{
				rowCache[i] = new DataRowView(this, Table.RecordCache[records[i]], i);
			}
			if (_lastAdded != null)
			{
				rowCache[size] = new DataRowView(this, _lastAdded, size);
			}
		}

		private int IndexOf(DataRow dr)
		{
			for (int i = 0; i < rowCache.Length; i++)
			{
				if (dr.Equals(rowCache[i].Row))
				{
					return i;
				}
			}
			return -1;
		}

		private void PopulateDefaultSort()
		{
			sort = string.Empty;
			foreach (Constraint constraint in dataTable.Constraints)
			{
				if (constraint is UniqueConstraint)
				{
					PopulateDefaultSort((UniqueConstraint)constraint);
					break;
				}
			}
		}

		private void PopulateDefaultSort(UniqueConstraint uc)
		{
			if (isInitPhase)
			{
				return;
			}
			DataColumn[] columns = uc.Columns;
			if (columns.Length == 0)
			{
				sort = string.Empty;
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(columns[0].ColumnName);
			for (int i = 1; i < columns.Length; i++)
			{
				stringBuilder.Append(", ");
				stringBuilder.Append(columns[i].ColumnName);
			}
			sort = stringBuilder.ToString();
		}

		internal DataView CreateChildView(DataRelation relation, int index)
		{
			if (relation == null || relation.ParentTable != Table)
			{
				throw new ArgumentException("The relation is not parented to the table to which this DataView points.");
			}
			int record = GetRecord(index);
			object[] array = new object[relation.ParentColumns.Length];
			for (int i = 0; i < relation.ParentColumns.Length; i++)
			{
				array[i] = relation.ParentColumns[i][record];
			}
			return new RelatedDataView(relation.ChildColumns, array);
		}

		private int GetRecord(int index)
		{
			if (index < 0 || index >= Count)
			{
				throw new IndexOutOfRangeException(string.Format("There is no row at position {0}.", index));
			}
			return (index != Index.Size) ? Index.IndexToRecord(index) : _lastAdded.IndexFromVersion(DataRowVersion.Default);
		}

		internal DataRowVersion GetRowVersion(int index)
		{
			int record = GetRecord(index);
			return Table.RecordCache[record].VersionFromIndex(record);
		}

		private void DataViewInitialized(bool value)
		{
			dataViewInitialized = value;
			if (value)
			{
				OnDataViewInitialized(new EventArgs());
			}
		}

		private void OnDataViewInitialized(EventArgs e)
		{
			if (this.Initialized != null)
			{
				this.Initialized(this, e);
			}
		}

		public virtual bool Equals(DataView dv)
		{
			if (this == dv)
			{
				return true;
			}
			if (Table != dv.Table || !(Sort == dv.Sort) || !(RowFilter == dv.RowFilter) || RowStateFilter != dv.RowStateFilter || AllowEdit != dv.AllowEdit || AllowNew != dv.AllowNew || AllowDelete != dv.AllowDelete || Count != dv.Count)
			{
				return false;
			}
			for (int i = 0; i < Count; i++)
			{
				if (!this[i].Equals(dv[i]))
				{
					return false;
				}
			}
			return true;
		}

		public DataTable ToTable()
		{
			return ToTable(Table.TableName, false);
		}

		public DataTable ToTable(string tableName)
		{
			return ToTable(tableName, false);
		}

		public DataTable ToTable(bool isDistinct, params string[] columnNames)
		{
			return ToTable(Table.TableName, isDistinct, columnNames);
		}

		public DataTable ToTable(string tablename, bool isDistinct, params string[] columnNames)
		{
			if (columnNames == null)
			{
				throw new ArgumentNullException("columnNames", "'columnNames' argument cannot be null.");
			}
			DataTable dataTable = new DataTable(tablename);
			ListSortDirection[] array = null;
			DataColumn[] array2;
			if (columnNames.Length > 0)
			{
				array2 = new DataColumn[columnNames.Length];
				for (int i = 0; i < columnNames.Length; i++)
				{
					array2[i] = Table.Columns[columnNames[i]];
				}
				if (sortColumns != null)
				{
					array = new ListSortDirection[columnNames.Length];
					for (int j = 0; j < columnNames.Length; j++)
					{
						array[j] = ListSortDirection.Ascending;
						for (int k = 0; k < sortColumns.Length; k++)
						{
							if (sortColumns[k] == array2[j])
							{
								array[j] = sortOrder[k];
							}
						}
					}
				}
			}
			else
			{
				array2 = (DataColumn[])Table.Columns.ToArray(typeof(DataColumn));
				array = sortOrder;
			}
			ArrayList arrayList = new ArrayList();
			for (int l = 0; l < array2.Length; l++)
			{
				DataColumn dataColumn = array2[l].Clone();
				if (dataColumn.Expression != string.Empty)
				{
					dataColumn.Expression = string.Empty;
					arrayList.Add(dataColumn);
				}
				if (dataColumn.ReadOnly)
				{
					dataColumn.ReadOnly = false;
				}
				dataTable.Columns.Add(dataColumn);
			}
			Index index = null;
			index = ((!(sort != string.Empty)) ? new Index(new Key(Table, array2, array, RowStateFilter, rowFilterExpr)) : Table.GetIndex(sortColumns, sortOrder, RowStateFilter, FilterExpression, true));
			DataRow[] array3 = ((!isDistinct) ? index.GetAllRows() : index.GetDistinctRows());
			DataRow[] array4 = array3;
			foreach (DataRow dataRow in array4)
			{
				DataRow dataRow2 = dataTable.NewNotInitializedRow();
				dataTable.Rows.AddInternal(dataRow2);
				dataRow2.Original = -1;
				if (dataRow.HasVersion(DataRowVersion.Current))
				{
					dataRow2.Current = dataTable.RecordCache.CopyRecord(Table, dataRow.Current, -1);
				}
				else if (dataRow.HasVersion(DataRowVersion.Original))
				{
					dataRow2.Current = dataTable.RecordCache.CopyRecord(Table, dataRow.Original, -1);
				}
				foreach (DataColumn item in arrayList)
				{
					dataRow2[item] = dataRow[item.ColumnName];
				}
				dataRow2.Original = -1;
			}
			return dataTable;
		}
	}
}
