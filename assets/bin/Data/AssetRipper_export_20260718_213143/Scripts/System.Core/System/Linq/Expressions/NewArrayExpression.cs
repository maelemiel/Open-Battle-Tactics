using System.Collections.ObjectModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class NewArrayExpression : Expression
	{
		private ReadOnlyCollection<Expression> expressions;

		public ReadOnlyCollection<Expression> Expressions
		{
			get
			{
				return expressions;
			}
		}

		internal NewArrayExpression(ExpressionType et, Type type, ReadOnlyCollection<Expression> expressions)
			: base(et, type)
		{
			this.expressions = expressions;
		}

		private void EmitNewArrayInit(EmitContext ec, Type type)
		{
			int count = expressions.Count;
			ec.ig.Emit(OpCodes.Ldc_I4, count);
			ec.ig.Emit(OpCodes.Newarr, type);
			for (int i = 0; i < count; i++)
			{
				ec.ig.Emit(OpCodes.Dup);
				ec.ig.Emit(OpCodes.Ldc_I4, i);
				expressions[i].Emit(ec);
				ec.ig.Emit(OpCodes.Stelem, type);
			}
		}

		private void EmitNewArrayBounds(EmitContext ec, Type type)
		{
			int count = expressions.Count;
			ec.EmitCollection(expressions);
			if (count == 1)
			{
				ec.ig.Emit(OpCodes.Newarr, type);
			}
			else
			{
				ec.ig.Emit(OpCodes.Newobj, GetArrayConstructor(type, count));
			}
		}

		private static ConstructorInfo GetArrayConstructor(Type type, int rank)
		{
			return CreateArray(type, rank).GetConstructor(CreateTypeParameters(rank));
		}

		private static Type[] CreateTypeParameters(int rank)
		{
			return Enumerable.Repeat(typeof(int), rank).ToArray();
		}

		private static Type CreateArray(Type type, int rank)
		{
			return type.MakeArrayType(rank);
		}

		internal override void Emit(EmitContext ec)
		{
			Type elementType = base.Type.GetElementType();
			switch (base.NodeType)
			{
			case ExpressionType.NewArrayInit:
				EmitNewArrayInit(ec, elementType);
				break;
			case ExpressionType.NewArrayBounds:
				EmitNewArrayBounds(ec, elementType);
				break;
			default:
				throw new NotSupportedException();
			}
		}
	}
}
