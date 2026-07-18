using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using Mono.Data.Tds;

namespace System.Data.SqlClient
{
	[Editor("Microsoft.VSDesigner.Data.Design.DBParametersEditor, Microsoft.VSDesigner, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[ListBindable(false)]
	public sealed class SqlParameterCollection : DbParameterCollection, IList, ICollection, IEnumerable, IDataParameterCollection
	{
		private ArrayList list = new ArrayList();

		private TdsMetaParameterCollection metaParameters;

		private SqlCommand command;

		public override int Count
		{
			get
			{
				return list.Count;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return list.IsFixedSize;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return list.IsReadOnly;
			}
		}

		public override bool IsSynchronized
		{
			get
			{
				return list.IsSynchronized;
			}
		}

		public override object SyncRoot
		{
			get
			{
				return list.SyncRoot;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SqlParameter this[int index]
		{
			get
			{
				if (index < 0 || index >= list.Count)
				{
					throw new IndexOutOfRangeException("The specified index is out of range.");
				}
				return (SqlParameter)list[index];
			}
			set
			{
				if (index < 0 || index >= list.Count)
				{
					throw new IndexOutOfRangeException("The specified index is out of range.");
				}
				list[index] = value;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new SqlParameter this[string parameterName]
		{
			get
			{
				foreach (SqlParameter item in list)
				{
					if (item.ParameterName.Equals(parameterName))
					{
						return item;
					}
				}
				throw new IndexOutOfRangeException("The specified name does not exist: " + parameterName);
			}
			set
			{
				if (!Contains(parameterName))
				{
					throw new IndexOutOfRangeException("The specified name does not exist: " + parameterName);
				}
				this[IndexOf(parameterName)] = value;
			}
		}

		internal TdsMetaParameterCollection MetaParameters
		{
			get
			{
				return metaParameters;
			}
		}

		internal SqlParameterCollection(SqlCommand command)
		{
			this.command = command;
			metaParameters = new TdsMetaParameterCollection();
		}

		protected override DbParameter GetParameter(int index)
		{
			return this[index];
		}

		protected override DbParameter GetParameter(string parameterName)
		{
			return this[parameterName];
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			this[index] = (SqlParameter)value;
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			this[parameterName] = (SqlParameter)value;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int Add(object value)
		{
			if (!(value is SqlParameter))
			{
				throw new InvalidCastException("The parameter was not an SqlParameter.");
			}
			Add((SqlParameter)value);
			return IndexOf(value);
		}

		public SqlParameter Add(SqlParameter value)
		{
			if (value.Container != null)
			{
				throw new ArgumentException("The SqlParameter specified in the value parameter is already added to this or another SqlParameterCollection.");
			}
			value.Container = this;
			list.Add(value);
			metaParameters.Add(value.MetaParameter);
			return value;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Do not call this method.")]
		public SqlParameter Add(string parameterName, object value)
		{
			return Add(new SqlParameter(parameterName, value));
		}

		public SqlParameter AddWithValue(string parameterName, object value)
		{
			return Add(new SqlParameter(parameterName, value));
		}

		public SqlParameter Add(string parameterName, SqlDbType sqlDbType)
		{
			return Add(new SqlParameter(parameterName, sqlDbType));
		}

		public SqlParameter Add(string parameterName, SqlDbType sqlDbType, int size)
		{
			return Add(new SqlParameter(parameterName, sqlDbType, size));
		}

		public SqlParameter Add(string parameterName, SqlDbType sqlDbType, int size, string sourceColumn)
		{
			return Add(new SqlParameter(parameterName, sqlDbType, size, sourceColumn));
		}

		public override void Clear()
		{
			metaParameters.Clear();
			foreach (SqlParameter item in list)
			{
				item.Container = null;
			}
			list.Clear();
		}

		public override bool Contains(object value)
		{
			if (!(value is SqlParameter))
			{
				throw new InvalidCastException("The parameter was not an SqlParameter.");
			}
			return Contains(((SqlParameter)value).ParameterName);
		}

		public override bool Contains(string value)
		{
			foreach (SqlParameter item in list)
			{
				if (item.ParameterName.Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(SqlParameter value)
		{
			return IndexOf(value) != -1;
		}

		public override void CopyTo(Array array, int index)
		{
			list.CopyTo(array, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public override int IndexOf(object value)
		{
			if (!(value is SqlParameter))
			{
				throw new InvalidCastException("The parameter was not an SqlParameter.");
			}
			return IndexOf(((SqlParameter)value).ParameterName);
		}

		public override int IndexOf(string parameterName)
		{
			for (int i = 0; i < Count; i++)
			{
				if (this[i].ParameterName.Equals(parameterName))
				{
					return i;
				}
			}
			return -1;
		}

		public int IndexOf(SqlParameter value)
		{
			return list.IndexOf(value);
		}

		public override void Insert(int index, object value)
		{
			list.Insert(index, value);
		}

		public void Insert(int index, SqlParameter value)
		{
			list.Insert(index, value);
		}

		public override void Remove(object value)
		{
			((SqlParameter)value).Container = null;
			metaParameters.Remove(((SqlParameter)value).MetaParameter);
			list.Remove(value);
		}

		public void Remove(SqlParameter value)
		{
			value.Container = null;
			metaParameters.Remove(value.MetaParameter);
			list.Remove(value);
		}

		public override void RemoveAt(int index)
		{
			this[index].Container = null;
			metaParameters.RemoveAt(index);
			list.RemoveAt(index);
		}

		public override void RemoveAt(string parameterName)
		{
			RemoveAt(IndexOf(parameterName));
		}

		public override void AddRange(Array values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("The argument passed was null");
			}
			foreach (object value in values)
			{
				if (!(value is SqlParameter))
				{
					throw new InvalidCastException("Element in the array parameter was not an SqlParameter.");
				}
				SqlParameter sqlParameter = (SqlParameter)value;
				if (sqlParameter.Container != null)
				{
					throw new ArgumentException("An SqlParameter specified in the array is already added to this or another SqlParameterCollection.");
				}
				sqlParameter.Container = this;
				list.Add(sqlParameter);
				metaParameters.Add(sqlParameter.MetaParameter);
			}
		}

		public void AddRange(SqlParameter[] values)
		{
			AddRange((Array)values);
		}

		public void CopyTo(SqlParameter[] array, int index)
		{
			list.CopyTo(array, index);
		}
	}
}
