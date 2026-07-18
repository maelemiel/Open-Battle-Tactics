using System;

namespace Mono.Data.SqlExpressions.yyParser
{
	internal class yyException : Exception
	{
		public yyException(string message)
			: base(message)
		{
		}
	}
}
