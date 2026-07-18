namespace Mono.Data.Sqlite
{
	internal interface ISQLiteSchemaExtensions
	{
		void BuildTempSchema(SqliteConnection cnn);
	}
}
