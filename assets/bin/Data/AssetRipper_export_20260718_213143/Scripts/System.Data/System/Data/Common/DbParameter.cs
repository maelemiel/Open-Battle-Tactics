using System.Collections;
using System.ComponentModel;

namespace System.Data.Common
{
	public abstract class DbParameter : MarshalByRefObject, IDataParameter, IDbDataParameter
	{
		internal static Hashtable dbTypeMapping;

		byte IDbDataParameter.Precision
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		byte IDbDataParameter.Scale
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public abstract DbType DbType { get; set; }

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(ParameterDirection.Input)]
		public abstract ParameterDirection Direction { get; set; }

		[DefaultValue("")]
		public abstract string ParameterName { get; set; }

		public abstract int Size { get; set; }

		[DefaultValue(null)]
		[RefreshProperties(RefreshProperties.All)]
		public abstract object Value { get; set; }

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignOnly(true)]
		public abstract bool IsNullable { get; set; }

		[DefaultValue("")]
		public abstract string SourceColumn { get; set; }

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(false)]
		public abstract bool SourceColumnNullMapping { get; set; }

		[DefaultValue(DataRowVersion.Current)]
		public abstract DataRowVersion SourceVersion { get; set; }

		internal virtual object FrameworkDbType
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		protected internal static Hashtable DbTypeMapping
		{
			get
			{
				return dbTypeMapping;
			}
			set
			{
				dbTypeMapping = value;
			}
		}

		internal virtual Type SystemType
		{
			get
			{
				return (Type)dbTypeMapping[DbType];
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public abstract void ResetDbType();
	}
}
