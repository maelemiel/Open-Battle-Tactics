namespace Mono.Data.SqlExpressions.yyParser
{
	internal class yyUnexpectedEof : yyException
	{
		public yyUnexpectedEof(string message)
			: base(message)
		{
		}

		public yyUnexpectedEof()
			: base(string.Empty)
		{
		}
	}
}
