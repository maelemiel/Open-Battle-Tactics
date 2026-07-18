using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public class LambdaExpression : Expression
	{
		private Expression body;

		private ReadOnlyCollection<ParameterExpression> parameters;

		public Expression Body
		{
			get
			{
				return body;
			}
		}

		public ReadOnlyCollection<ParameterExpression> Parameters
		{
			get
			{
				return parameters;
			}
		}

		internal LambdaExpression(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
			: base(ExpressionType.Lambda, delegateType)
		{
			this.body = body;
			this.parameters = parameters;
		}

		private void EmitPopIfNeeded(EmitContext ec)
		{
			if (GetReturnType() == typeof(void) && body.Type != typeof(void))
			{
				ec.ig.Emit(OpCodes.Pop);
			}
		}

		internal override void Emit(EmitContext ec)
		{
			ec.EmitCreateDelegate(this);
		}

		internal void EmitBody(EmitContext ec)
		{
			body.Emit(ec);
			EmitPopIfNeeded(ec);
			ec.ig.Emit(OpCodes.Ret);
		}

		internal Type GetReturnType()
		{
			return base.Type.GetInvokeMethod().ReturnType;
		}

		public Delegate Compile()
		{
			CompilationContext compilationContext = new CompilationContext();
			compilationContext.AddCompilationUnit(this);
			return compilationContext.CreateDelegate();
		}
	}
}
