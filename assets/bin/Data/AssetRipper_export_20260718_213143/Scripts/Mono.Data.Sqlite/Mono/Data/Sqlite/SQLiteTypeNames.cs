using System.Data;

namespace Mono.Data.Sqlite
{
	internal struct SQLiteTypeNames
	{
		internal string typeName;

		internal DbType dataType;

		internal SQLiteTypeNames(string newtypeName, DbType newdataType)
		{
			typeName = newtypeName;
			dataType = newdataType;
		}
	}
}
