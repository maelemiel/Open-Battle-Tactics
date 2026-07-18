namespace System.Data
{
	internal class RelationStructure
	{
		public string ExplicitName;

		public string ParentTableName;

		public string ChildTableName;

		public string ParentColumnName;

		public string ChildColumnName;

		public bool IsNested;

		public bool CreateConstraint;
	}
}
