namespace System.Data
{
	internal class ConstraintStructure
	{
		public readonly string TableName;

		public readonly string[] Columns;

		public readonly bool[] IsAttribute;

		public readonly string ConstraintName;

		public readonly bool IsPrimaryKey;

		public readonly string ReferName;

		public readonly bool IsNested;

		public readonly bool IsConstraintOnly;

		public ConstraintStructure(string tname, string[] cols, bool[] isAttr, string cname, bool isPK, string refName, bool isNested, bool isConstraintOnly)
		{
			TableName = tname;
			Columns = cols;
			IsAttribute = isAttr;
			ConstraintName = XmlHelper.Decode(cname);
			IsPrimaryKey = isPK;
			ReferName = refName;
			IsNested = isNested;
			IsConstraintOnly = isConstraintOnly;
		}
	}
}
