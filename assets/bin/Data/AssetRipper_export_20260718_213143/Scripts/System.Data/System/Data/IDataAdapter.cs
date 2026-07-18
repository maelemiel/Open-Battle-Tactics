namespace System.Data
{
	public interface IDataAdapter
	{
		MissingMappingAction MissingMappingAction { get; set; }

		MissingSchemaAction MissingSchemaAction { get; set; }

		ITableMappingCollection TableMappings { get; }

		int Fill(DataSet dataSet);

		DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType);

		IDataParameter[] GetFillParameters();

		int Update(DataSet dataSet);
	}
}
