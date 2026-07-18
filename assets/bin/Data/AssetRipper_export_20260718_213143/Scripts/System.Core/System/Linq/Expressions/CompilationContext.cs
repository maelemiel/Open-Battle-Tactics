using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
	internal class CompilationContext
	{
		private class ParameterReplacer : ExpressionTransformer
		{
			private CompilationContext context;

			private ExecutionScope scope;

			private object[] locals;

			public ParameterReplacer(CompilationContext context, ExecutionScope scope, object[] locals)
			{
				this.context = context;
				this.scope = scope;
				this.locals = locals;
			}

			protected override Expression VisitParameter(ParameterExpression parameter)
			{
				ExecutionScope parent = scope;
				object[] array = locals;
				while (parent != null)
				{
					int num = IndexOfHoistedLocal(parent, parameter);
					if (num != -1)
					{
						return ReadHoistedLocalFromArray(array, num);
					}
					array = parent.Locals;
					parent = parent.Parent;
				}
				return parameter;
			}

			private Expression ReadHoistedLocalFromArray(object[] locals, int position)
			{
				return Expression.Field(Expression.Convert(Expression.ArrayIndex(Expression.Constant(locals), Expression.Constant(position)), locals[position].GetType()), "Value");
			}

			private int IndexOfHoistedLocal(ExecutionScope scope, ParameterExpression parameter)
			{
				return context.units[scope.compilation_unit].IndexOfHoistedLocal(parameter);
			}
		}

		private class HoistedVariableDetector : ExpressionVisitor
		{
			private Dictionary<ParameterExpression, LambdaExpression> parameter_to_lambda = new Dictionary<ParameterExpression, LambdaExpression>();

			private Dictionary<LambdaExpression, List<ParameterExpression>> hoisted_map;

			private LambdaExpression lambda;

			public Dictionary<LambdaExpression, List<ParameterExpression>> Process(LambdaExpression lambda)
			{
				Visit(lambda);
				return hoisted_map;
			}

			protected override void VisitLambda(LambdaExpression lambda)
			{
				this.lambda = lambda;
				foreach (ParameterExpression parameter in lambda.Parameters)
				{
					parameter_to_lambda[parameter] = lambda;
				}
				base.VisitLambda(lambda);
			}

			protected override void VisitParameter(ParameterExpression parameter)
			{
				if (!lambda.Parameters.Contains(parameter))
				{
					Hoist(parameter);
				}
			}

			private void Hoist(ParameterExpression parameter)
			{
				LambdaExpression value;
				if (parameter_to_lambda.TryGetValue(parameter, out value))
				{
					if (hoisted_map == null)
					{
						hoisted_map = new Dictionary<LambdaExpression, List<ParameterExpression>>();
					}
					List<ParameterExpression> value2;
					if (!hoisted_map.TryGetValue(value, out value2))
					{
						value2 = new List<ParameterExpression>();
						hoisted_map[value] = value2;
					}
					value2.Add(parameter);
				}
			}
		}

		private List<object> globals = new List<object>();

		private List<EmitContext> units = new List<EmitContext>();

		private Dictionary<LambdaExpression, List<ParameterExpression>> hoisted_map;

		public int AddGlobal(object global)
		{
			return AddItemToList(global, globals);
		}

		public object[] GetGlobals()
		{
			return globals.ToArray();
		}

		private static int AddItemToList<T>(T item, IList<T> list)
		{
			list.Add(item);
			return list.Count - 1;
		}

		public int AddCompilationUnit(LambdaExpression lambda)
		{
			DetectHoistedVariables(lambda);
			return AddCompilationUnit(null, lambda);
		}

		public int AddCompilationUnit(EmitContext parent, LambdaExpression lambda)
		{
			EmitContext emitContext = new EmitContext(this, parent, lambda);
			int result = AddItemToList(emitContext, units);
			emitContext.Emit();
			return result;
		}

		private void DetectHoistedVariables(LambdaExpression lambda)
		{
			hoisted_map = new HoistedVariableDetector().Process(lambda);
		}

		public List<ParameterExpression> GetHoistedLocals(LambdaExpression lambda)
		{
			if (hoisted_map == null)
			{
				return null;
			}
			List<ParameterExpression> value;
			hoisted_map.TryGetValue(lambda, out value);
			return value;
		}

		public object[] CreateHoistedLocals(int unit)
		{
			List<ParameterExpression> hoistedLocals = GetHoistedLocals(units[unit].Lambda);
			return new object[(hoistedLocals != null) ? hoistedLocals.Count : 0];
		}

		public Expression IsolateExpression(ExecutionScope scope, object[] locals, Expression expression)
		{
			return new ParameterReplacer(this, scope, locals).Transform(expression);
		}

		public Delegate CreateDelegate()
		{
			return CreateDelegate(0, new ExecutionScope(this));
		}

		public Delegate CreateDelegate(int unit, ExecutionScope scope)
		{
			return units[unit].CreateDelegate(scope);
		}
	}
}
