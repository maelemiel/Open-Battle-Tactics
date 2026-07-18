namespace MobageEditor
{
	public class DBContext
	{
		private TableContext info;

		public TableContext Info
		{
			get
			{
				if (info == null)
				{
					info = new TableContext();
				}
				return info;
			}
		}
	}
}
