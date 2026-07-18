using System.Linq.Expressions;

namespace System.Runtime.CompilerServices
{
	public class ExecutionScope
	{
		public object[] Globals;

		public object[] Locals;

		public ExecutionScope Parent;

		internal CompilationContext context;

		internal int compilation_unit;

		private ExecutionScope(CompilationContext context, int compilation_unit)
		{
			this.context = context;
			this.compilation_unit = compilation_unit;
			Globals = context.GetGlobals();
		}

		internal ExecutionScope(CompilationContext context)
			: this(context, 0)
		{
		}

		internal ExecutionScope(CompilationContext context, int compilation_unit, ExecutionScope parent, object[] locals)
			: this(context, compilation_unit)
		{
			Parent = parent;
			Locals = locals;
		}

		public Delegate CreateDelegate(int indexLambda, object[] locals)
		{
			return context.CreateDelegate(indexLambda, new ExecutionScope(context, indexLambda, this, locals));
		}

		public object[] CreateHoistedLocals()
		{
			return context.CreateHoistedLocals(compilation_unit);
		}

		public Expression IsolateExpression(Expression expression, object[] locals)
		{
			return context.IsolateExpression(this, locals, expression);
		}
	}
}
