using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace System.Data.Common
{
	[Obsolete]
	internal class DbConnectionString : DbConnectionOptions, ISerializable
	{
		private KeyRestrictionBehavior behavior;

		public KeyRestrictionBehavior Behavior
		{
			get
			{
				return behavior;
			}
		}

		[System.MonoTODO]
		public string Restrictions
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected internal DbConnectionString(DbConnectionString constr)
		{
			options = constr.options;
		}

		public DbConnectionString(string connectionString)
			: base(connectionString)
		{
			options = new NameValueCollection();
			ParseConnectionString(connectionString);
		}

		[System.MonoTODO]
		protected DbConnectionString(SerializationInfo si, StreamingContext sc)
		{
		}

		[System.MonoTODO]
		public DbConnectionString(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
			: this(connectionString)
		{
			this.behavior = behavior;
		}

		[System.MonoTODO]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		protected virtual string KeywordLookup(string keyname)
		{
			return keyname;
		}

		[System.MonoTODO]
		public virtual void PermissionDemand()
		{
			throw new NotImplementedException();
		}
	}
}
