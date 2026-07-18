using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Xml;

namespace System.Data
{
	[Designer("Microsoft.VSDesigner.Data.VS.DataViewManagerDesigner, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.IDesigner")]
	public class DataViewManager : MarshalByValueComponent, IList, ICollection, IEnumerable, IBindingList, ITypedList
	{
		private DataSet dataSet;

		private DataViewManagerListItemTypeDescriptor descriptor;

		private DataViewSettingCollection settings;

		private string xml;

		int ICollection.Count
		{
			get
			{
				return 1;
			}
		}

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
				return true;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		object IList.this[int index]
		{
			get
			{
				if (descriptor == null)
				{
					descriptor = new DataViewManagerListItemTypeDescriptor(this);
				}
				return descriptor;
			}
			set
			{
				throw new ArgumentException("Not modifiable");
			}
		}

		bool IBindingList.AllowEdit
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.AllowNew
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return false;
			}
		}

		bool IBindingList.IsSorted
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		PropertyDescriptor IBindingList.SortProperty
		{
			get
			{
				throw new NotSupportedException();
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
				return false;
			}
		}

		bool IBindingList.SupportsSorting
		{
			get
			{
				return false;
			}
		}

		[DefaultValue(null)]
		public DataSet DataSet
		{
			get
			{
				return dataSet;
			}
			set
			{
				if (value == null)
				{
					throw new DataException("Cannot set null DataSet.");
				}
				SetDataSet(value);
			}
		}

		public string DataViewSettingCollectionString
		{
			get
			{
				return xml;
			}
			set
			{
				try
				{
					ParseSettingString(value);
					xml = BuildSettingString();
				}
				catch (XmlException innerException)
				{
					throw new DataException("Cannot set DataViewSettingCollectionString.", innerException);
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DataViewSettingCollection DataViewSettings
		{
			get
			{
				return settings;
			}
		}

		public event ListChangedEventHandler ListChanged;

		public DataViewManager()
			: this(null)
		{
		}

		public DataViewManager(DataSet dataSet)
		{
			SetDataSet(dataSet);
		}

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
		}

		object IBindingList.AddNew()
		{
			throw new NotSupportedException();
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
		}

		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			array.SetValue(descriptor, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			DataViewManagerListItemTypeDescriptor[] array = new DataViewManagerListItemTypeDescriptor[((ICollection)this).Count];
			((ICollection)this).CopyTo((Array)array, 0);
			return array.GetEnumerator();
		}

		int IList.Add(object value)
		{
			throw new ArgumentException("Not modifiable");
		}

		void IList.Clear()
		{
			throw new ArgumentException("Not modifiable");
		}

		bool IList.Contains(object value)
		{
			return value == descriptor;
		}

		int IList.IndexOf(object value)
		{
			if (value == descriptor)
			{
				return 0;
			}
			return -1;
		}

		void IList.Insert(int index, object value)
		{
			throw new ArgumentException("Not modifiable");
		}

		void IList.Remove(object value)
		{
			throw new ArgumentException("Not modifiable");
		}

		void IList.RemoveAt(int index)
		{
			throw new ArgumentException("Not modifiable");
		}

		[System.MonoLimitation("Supported only empty list of listAccessors")]
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			if (dataSet == null)
			{
				throw new DataException("dataset is null");
			}
			if (listAccessors == null || listAccessors.Length == 0)
			{
				ICustomTypeDescriptor customTypeDescriptor = new DataViewManagerListItemTypeDescriptor(this);
				return customTypeDescriptor.GetProperties();
			}
			throw new NotImplementedException();
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			if (dataSet != null && (listAccessors == null || listAccessors.Length == 0))
			{
				return dataSet.DataSetName;
			}
			return string.Empty;
		}

		private void SetDataSet(DataSet ds)
		{
			if (dataSet != null)
			{
				dataSet.Tables.CollectionChanged -= TableCollectionChanged;
				dataSet.Relations.CollectionChanged -= RelationCollectionChanged;
			}
			dataSet = ds;
			settings = new DataViewSettingCollection(this);
			xml = BuildSettingString();
			if (dataSet != null)
			{
				dataSet.Tables.CollectionChanged += TableCollectionChanged;
				dataSet.Relations.CollectionChanged += RelationCollectionChanged;
			}
		}

		private void ParseSettingString(string source)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(source, XmlNodeType.Element, null);
			xmlTextReader.Read();
			if (xmlTextReader.Name != "DataViewSettingCollectionString")
			{
				xmlTextReader.ReadStartElement("DataViewSettingCollectionString");
			}
			if (xmlTextReader.IsEmptyElement)
			{
				return;
			}
			xmlTextReader.Read();
			do
			{
				xmlTextReader.MoveToContent();
				if (xmlTextReader.NodeType == XmlNodeType.EndElement)
				{
					break;
				}
				if (xmlTextReader.NodeType == XmlNodeType.Element)
				{
					ReadTableSetting(xmlTextReader);
				}
				else
				{
					xmlTextReader.Skip();
				}
			}
			while (!xmlTextReader.EOF);
			if (xmlTextReader.NodeType == XmlNodeType.EndElement)
			{
				xmlTextReader.ReadEndElement();
			}
		}

		private void ReadTableSetting(XmlReader reader)
		{
			DataTable table = DataSet.Tables[XmlConvert.DecodeName(reader.LocalName)];
			DataViewSetting dataViewSetting = settings[table];
			string attribute = reader.GetAttribute("Sort");
			if (attribute != null)
			{
				dataViewSetting.Sort = attribute.Trim();
			}
			string attribute2 = reader.GetAttribute("ApplyDefaultSort");
			if (attribute2 != null && attribute2.Trim() == "true")
			{
				dataViewSetting.ApplyDefaultSort = true;
			}
			string attribute3 = reader.GetAttribute("RowFilter");
			if (attribute3 != null)
			{
				dataViewSetting.RowFilter = attribute3.Trim();
			}
			string attribute4 = reader.GetAttribute("RowStateFilter");
			if (attribute4 != null)
			{
				dataViewSetting.RowStateFilter = (DataViewRowState)(int)Enum.Parse(typeof(DataViewRowState), attribute4.Trim());
			}
			reader.Skip();
		}

		private string BuildSettingString()
		{
			if (dataSet == null)
			{
				return string.Empty;
			}
			StringWriter stringWriter = new StringWriter();
			stringWriter.Write('<');
			stringWriter.Write("DataViewSettingCollectionString>");
			foreach (DataViewSetting dataViewSetting in DataViewSettings)
			{
				stringWriter.Write('<');
				stringWriter.Write(XmlConvert.EncodeName(dataViewSetting.Table.TableName));
				stringWriter.Write(" Sort=\"");
				stringWriter.Write(Escape(dataViewSetting.Sort));
				stringWriter.Write('"');
				if (dataViewSetting.ApplyDefaultSort)
				{
					stringWriter.Write(" ApplyDefaultSort=\"true\"");
				}
				stringWriter.Write(" RowFilter=\"");
				stringWriter.Write(Escape(dataViewSetting.RowFilter));
				stringWriter.Write("\" RowStateFilter=\"");
				stringWriter.Write(dataViewSetting.RowStateFilter.ToString());
				stringWriter.Write("\"/>");
			}
			stringWriter.Write("</DataViewSettingCollectionString>");
			return stringWriter.ToString();
		}

		private string Escape(string s)
		{
			return s.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("'", "&apos;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;");
		}

		public DataView CreateDataView(DataTable table)
		{
			if (settings[table] != null)
			{
				DataViewSetting dataViewSetting = settings[table];
				return new DataView(table, this, dataViewSetting.Sort, dataViewSetting.RowFilter, dataViewSetting.RowStateFilter);
			}
			return new DataView(table);
		}

		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			if (this.ListChanged != null)
			{
				this.ListChanged(this, e);
			}
		}

		protected virtual void RelationCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			OnListChanged(CollectionToListChangeEventArgs(e));
		}

		protected virtual void TableCollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			OnListChanged(CollectionToListChangeEventArgs(e));
		}

		private ListChangedEventArgs CollectionToListChangeEventArgs(CollectionChangeEventArgs e)
		{
			if (e.Action == CollectionChangeAction.Remove)
			{
				return null;
			}
			if (e.Action == CollectionChangeAction.Refresh)
			{
				return new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, null);
			}
			object obj = ((!typeof(DataTable).IsAssignableFrom(e.Element.GetType())) ? ((PropertyDescriptor)new DataRelationPropertyDescriptor((DataRelation)e.Element)) : ((PropertyDescriptor)new DataTablePropertyDescriptor((DataTable)e.Element)));
			if (e.Action == CollectionChangeAction.Add)
			{
				return new ListChangedEventArgs(ListChangedType.PropertyDescriptorAdded, (PropertyDescriptor)obj);
			}
			return new ListChangedEventArgs(ListChangedType.PropertyDescriptorDeleted, (PropertyDescriptor)obj);
		}
	}
}
