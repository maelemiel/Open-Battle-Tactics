using System.Collections;

namespace System.Data
{
	internal class Doublet
	{
		public int count;

		public ArrayList columnNames = new ArrayList();

		public Doublet(int count, string columnname)
		{
			this.count = count;
			columnNames.Add(columnname);
		}
	}
}
