namespace Mono.Data.SqlExpressions.yyParser
{
	internal interface yyInput
	{
		bool advance();

		int token();

		object value();
	}
}
