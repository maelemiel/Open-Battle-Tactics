namespace Mono.Data.Sqlite
{
	public class SqliteFunctionEx : SqliteFunction
	{
		protected CollationSequence GetCollationSequence()
		{
			return _base.GetCollationSequence(this, _context);
		}
	}
}
