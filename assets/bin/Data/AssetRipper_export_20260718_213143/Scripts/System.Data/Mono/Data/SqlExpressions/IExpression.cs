using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal interface IExpression
	{
		object Eval(DataRow row);

		bool DependsOn(DataColumn other);

		bool EvalBoolean(DataRow row);

		void ResetExpression();
	}
}
