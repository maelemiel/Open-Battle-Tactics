using System.Collections;
using System.ComponentModel;

namespace System.Data
{
	[Editor("Microsoft.VSDesigner.Data.Design.DataViewSettingsCollectionEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class DataViewSettingCollection : ICollection, IEnumerable
	{
		private readonly ArrayList settingList;

		[Browsable(false)]
		public virtual int Count
		{
			get
			{
				return settingList.Count;
			}
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				return settingList.IsReadOnly;
			}
		}

		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				return settingList.IsSynchronized;
			}
		}

		public virtual DataViewSetting this[DataTable table]
		{
			get
			{
				for (int i = 0; i < settingList.Count; i++)
				{
					DataViewSetting dataViewSetting = (DataViewSetting)settingList[i];
					if (dataViewSetting.Table == table)
					{
						return dataViewSetting;
					}
				}
				return null;
			}
			set
			{
				this[table] = value;
			}
		}

		public virtual DataViewSetting this[string tableName]
		{
			get
			{
				for (int i = 0; i < settingList.Count; i++)
				{
					DataViewSetting dataViewSetting = (DataViewSetting)settingList[i];
					if (dataViewSetting.Table.TableName == tableName)
					{
						return dataViewSetting;
					}
				}
				return null;
			}
		}

		public virtual DataViewSetting this[int index]
		{
			get
			{
				return (DataViewSetting)settingList[index];
			}
			set
			{
				settingList[index] = value;
			}
		}

		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				return settingList.SyncRoot;
			}
		}

		internal DataViewSettingCollection(DataViewManager manager)
		{
			settingList = new ArrayList();
			if (manager.DataSet == null)
			{
				return;
			}
			foreach (DataTable table in manager.DataSet.Tables)
			{
				settingList.Add(new DataViewSetting(manager, table));
			}
		}

		public void CopyTo(Array ar, int index)
		{
			settingList.CopyTo(ar, index);
		}

		public void CopyTo(DataViewSetting[] ar, int index)
		{
			settingList.CopyTo(ar, index);
		}

		public IEnumerator GetEnumerator()
		{
			return settingList.GetEnumerator();
		}
	}
}
