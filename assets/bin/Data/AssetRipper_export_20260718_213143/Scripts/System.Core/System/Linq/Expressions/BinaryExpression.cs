using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class BinaryExpression : Expression
	{
		private Expression left;

		private Expression right;

		private LambdaExpression conversion;

		private MethodInfo method;

		private bool lift_to_null;

		private bool is_lifted;

		public Expression Left
		{
			get
			{
				return left;
			}
		}

		public Expression Right
		{
			get
			{
				return right;
			}
		}

		public MethodInfo Method
		{
			get
			{
				return method;
			}
		}

		public bool IsLifted
		{
			get
			{
				return is_lifted;
			}
		}

		public bool IsLiftedToNull
		{
			get
			{
				return lift_to_null;
			}
		}

		public LambdaExpression Conversion
		{
			get
			{
				return conversion;
			}
		}

		internal BinaryExpression(ExpressionType node_type, Type type, Expression left, Expression right)
			: base(node_type, type)
		{
			this.left = left;
			this.right = right;
		}

		internal BinaryExpression(ExpressionType node_type, Type type, Expression left, Expression right, MethodInfo method)
			: base(node_type, type)
		{
			this.left = left;
			this.right = right;
			this.method = method;
		}

		internal BinaryExpression(ExpressionType node_type, Type type, Expression left, Expression right, bool lift_to_null, bool is_lifted, MethodInfo method, LambdaExpression conversion)
			: base(node_type, type)
		{
			this.left = left;
			this.right = right;
			this.method = method;
			this.conversion = conversion;
			this.lift_to_null = lift_to_null;
			this.is_lifted = is_lifted;
		}

		private void EmitArrayAccess(EmitContext ec)
		{
			left.Emit(ec);
			right.Emit(ec);
			ec.ig.Emit(OpCodes.Ldelem, base.Type);
		}

		private void EmitLogicalBinary(EmitContext ec)
		{
			switch (base.NodeType)
			{
			case ExpressionType.And:
			case ExpressionType.Or:
				if (!IsLifted)
				{
					EmitLogical(ec);
				}
				else if (base.Type == typeof(bool?))
				{
					EmitLiftedLogical(ec);
				}
				else
				{
					EmitLiftedArithmeticBinary(ec);
				}
				break;
			case ExpressionType.AndAlso:
			case ExpressionType.OrElse:
				if (!IsLifted)
				{
					EmitLogicalShortCircuit(ec);
				}
				else
				{
					EmitLiftedLogicalShortCircuit(ec);
				}
				break;
			}
		}

		private void EmitLogical(EmitContext ec)
		{
			EmitNonLiftedBinary(ec);
		}

		private void EmitLiftedLogical(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			bool flag = base.NodeType == ExpressionType.And;
			LocalBuilder localBuilder = ec.EmitStored(left);
			LocalBuilder localBuilder2 = ec.EmitStored(right);
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			Label label3 = ig.DefineLabel();
			ec.EmitNullableGetValueOrDefault(localBuilder);
			ig.Emit(OpCodes.Brtrue, label);
			ec.EmitNullableGetValueOrDefault(localBuilder2);
			ig.Emit(OpCodes.Brtrue, label2);
			ec.EmitNullableHasValue(localBuilder);
			ig.Emit(OpCodes.Brfalse, label);
			ig.MarkLabel(label2);
			ec.EmitLoad((!flag) ? localBuilder2 : localBuilder);
			ig.Emit(OpCodes.Br, label3);
			ig.MarkLabel(label);
			ec.EmitLoad((!flag) ? localBuilder : localBuilder2);
			ig.MarkLabel(label3);
		}

		private void EmitLogicalShortCircuit(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			bool flag = base.NodeType == ExpressionType.AndAlso;
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			ec.Emit(left);
			ig.Emit((!flag) ? OpCodes.Brtrue : OpCodes.Brfalse, label);
			ec.Emit(right);
			ig.Emit(OpCodes.Br, label2);
			ig.MarkLabel(label);
			ig.Emit((!flag) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
			ig.MarkLabel(label2);
		}

		private MethodInfo GetFalseOperator()
		{
			return Expression.GetFalseOperator(left.Type.GetNotNullableType());
		}

		private MethodInfo GetTrueOperator()
		{
			return Expression.GetTrueOperator(left.Type.GetNotNullableType());
		}

		private void EmitUserDefinedLogicalShortCircuit(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			bool flag = base.NodeType == ExpressionType.AndAlso;
			Label label = ig.DefineLabel();
			LocalBuilder local = ec.EmitStored(left);
			ec.EmitLoad(local);
			ig.Emit(OpCodes.Dup);
			ec.EmitCall((!flag) ? GetTrueOperator() : GetFalseOperator());
			ig.Emit(OpCodes.Brtrue, label);
			ec.Emit(right);
			ec.EmitCall(method);
			ig.MarkLabel(label);
		}

		private void EmitLiftedLogicalShortCircuit(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			bool flag = base.NodeType == ExpressionType.AndAlso;
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			Label label3 = ig.DefineLabel();
			Label label4 = ig.DefineLabel();
			Label label5 = ig.DefineLabel();
			LocalBuilder local = ec.EmitStored(left);
			ec.EmitNullableHasValue(local);
			ig.Emit(OpCodes.Brfalse, label);
			ec.EmitNullableGetValueOrDefault(local);
			ig.Emit(OpCodes.Ldc_I4_0);
			ig.Emit(OpCodes.Ceq);
			ig.Emit((!flag) ? OpCodes.Brfalse : OpCodes.Brtrue, label2);
			ig.MarkLabel(label);
			LocalBuilder local2 = ec.EmitStored(right);
			ec.EmitNullableHasValue(local2);
			ig.Emit(OpCodes.Brfalse_S, label3);
			ec.EmitNullableGetValueOrDefault(local2);
			ig.Emit(OpCodes.Ldc_I4_0);
			ig.Emit(OpCodes.Ceq);
			ig.Emit((!flag) ? OpCodes.Brfalse : OpCodes.Brtrue, label2);
			ec.EmitNullableHasValue(local);
			ig.Emit(OpCodes.Brfalse, label3);
			ig.Emit((!flag) ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
			ig.Emit(OpCodes.Br_S, label4);
			ig.MarkLabel(label2);
			ig.Emit((!flag) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
			ig.MarkLabel(label4);
			ec.EmitNullableNew(base.Type);
			ig.Emit(OpCodes.Br, label5);
			ig.MarkLabel(label3);
			LocalBuilder local3 = ig.DeclareLocal(base.Type);
			ec.EmitNullableInitialize(local3);
			ig.MarkLabel(label5);
		}

		private void EmitCoalesce(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			LocalBuilder localBuilder = ec.EmitStored(left);
			bool flag = localBuilder.LocalType.IsNullable();
			if (flag)
			{
				ec.EmitNullableHasValue(localBuilder);
			}
			else
			{
				ec.EmitLoad(localBuilder);
			}
			ig.Emit(OpCodes.Brfalse, label2);
			if (flag && !base.Type.IsNullable())
			{
				ec.EmitNullableGetValue(localBuilder);
			}
			else
			{
				ec.EmitLoad(localBuilder);
			}
			ig.Emit(OpCodes.Br, label);
			ig.MarkLabel(label2);
			ec.Emit(right);
			ig.MarkLabel(label);
		}

		private void EmitConvertedCoalesce(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			LocalBuilder localBuilder = ec.EmitStored(left);
			if (localBuilder.LocalType.IsNullable())
			{
				ec.EmitNullableHasValue(localBuilder);
			}
			else
			{
				ec.EmitLoad(localBuilder);
			}
			ig.Emit(OpCodes.Brfalse, label2);
			ec.Emit(conversion);
			ec.EmitLoad(localBuilder);
			ig.Emit(OpCodes.Callvirt, conversion.Type.GetInvokeMethod());
			ig.Emit(OpCodes.Br, label);
			ig.MarkLabel(label2);
			ec.Emit(right);
			ig.MarkLabel(label);
		}

		private static bool IsInt32OrInt64(Type type)
		{
			return type == typeof(int) || type == typeof(long);
		}

		private static bool IsSingleOrDouble(Type type)
		{
			return type == typeof(float) || type == typeof(double);
		}

		private void EmitBinaryOperator(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			bool flag = Expression.IsUnsigned(left.Type);
			switch (base.NodeType)
			{
			case ExpressionType.Add:
				ig.Emit(OpCodes.Add);
				break;
			case ExpressionType.AddChecked:
				if (IsInt32OrInt64(left.Type))
				{
					ig.Emit(OpCodes.Add_Ovf);
				}
				else
				{
					ig.Emit((!flag) ? OpCodes.Add : OpCodes.Add_Ovf_Un);
				}
				break;
			case ExpressionType.Subtract:
				ig.Emit(OpCodes.Sub);
				break;
			case ExpressionType.SubtractChecked:
				if (IsInt32OrInt64(left.Type))
				{
					ig.Emit(OpCodes.Sub_Ovf);
				}
				else
				{
					ig.Emit((!flag) ? OpCodes.Sub : OpCodes.Sub_Ovf_Un);
				}
				break;
			case ExpressionType.Multiply:
				ig.Emit(OpCodes.Mul);
				break;
			case ExpressionType.MultiplyChecked:
				if (IsInt32OrInt64(left.Type))
				{
					ig.Emit(OpCodes.Mul_Ovf);
				}
				else
				{
					ig.Emit((!flag) ? OpCodes.Mul : OpCodes.Mul_Ovf_Un);
				}
				break;
			case ExpressionType.Divide:
				ig.Emit((!flag) ? OpCodes.Div : OpCodes.Div_Un);
				break;
			case ExpressionType.Modulo:
				ig.Emit((!flag) ? OpCodes.Rem : OpCodes.Rem_Un);
				break;
			case ExpressionType.LeftShift:
			case ExpressionType.RightShift:
				ig.Emit(OpCodes.Ldc_I4, (left.Type != typeof(int)) ? 63 : 31);
				ig.Emit(OpCodes.And);
				if (base.NodeType == ExpressionType.RightShift)
				{
					ig.Emit((!flag) ? OpCodes.Shr : OpCodes.Shr_Un);
				}
				else
				{
					ig.Emit(OpCodes.Shl);
				}
				break;
			case ExpressionType.And:
				ig.Emit(OpCodes.And);
				break;
			case ExpressionType.Or:
				ig.Emit(OpCodes.Or);
				break;
			case ExpressionType.ExclusiveOr:
				ig.Emit(OpCodes.Xor);
				break;
			case ExpressionType.GreaterThan:
				ig.Emit((!flag) ? OpCodes.Cgt : OpCodes.Cgt_Un);
				break;
			case ExpressionType.GreaterThanOrEqual:
				if (flag || IsSingleOrDouble(left.Type))
				{
					ig.Emit(OpCodes.Clt_Un);
				}
				else
				{
					ig.Emit(OpCodes.Clt);
				}
				ig.Emit(OpCodes.Ldc_I4_0);
				ig.Emit(OpCodes.Ceq);
				break;
			case ExpressionType.LessThan:
				ig.Emit((!flag) ? OpCodes.Clt : OpCodes.Clt_Un);
				break;
			case ExpressionType.LessThanOrEqual:
				if (flag || IsSingleOrDouble(left.Type))
				{
					ig.Emit(OpCodes.Cgt_Un);
				}
				else
				{
					ig.Emit(OpCodes.Cgt);
				}
				ig.Emit(OpCodes.Ldc_I4_0);
				ig.Emit(OpCodes.Ceq);
				break;
			case ExpressionType.Equal:
				ig.Emit(OpCodes.Ceq);
				break;
			case ExpressionType.NotEqual:
				ig.Emit(OpCodes.Ceq);
				ig.Emit(OpCodes.Ldc_I4_0);
				ig.Emit(OpCodes.Ceq);
				break;
			case ExpressionType.Power:
				ig.Emit(OpCodes.Call, typeof(Math).GetMethod("Pow"));
				break;
			default:
				throw new InvalidOperationException(string.Format("Internal error: BinaryExpression contains non-Binary nodetype {0}", base.NodeType));
			}
		}

		private bool IsLeftLiftedBinary()
		{
			return left.Type.IsNullable() && !right.Type.IsNullable();
		}

		private void EmitLeftLiftedToNullBinary(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			LocalBuilder local = ec.EmitStored(left);
			ec.EmitNullableHasValue(local);
			ig.Emit(OpCodes.Brfalse, label);
			ec.EmitNullableGetValueOrDefault(local);
			ec.Emit(right);
			EmitBinaryOperator(ec);
			ec.EmitNullableNew(base.Type);
			ig.Emit(OpCodes.Br, label2);
			ig.MarkLabel(label);
			LocalBuilder local2 = ig.DeclareLocal(base.Type);
			ec.EmitNullableInitialize(local2);
			ig.MarkLabel(label2);
		}

		private void EmitLiftedArithmeticBinary(EmitContext ec)
		{
			if (IsLeftLiftedBinary())
			{
				EmitLeftLiftedToNullBinary(ec);
			}
			else
			{
				EmitLiftedToNullBinary(ec);
			}
		}

		private void EmitLiftedToNullBinary(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			LocalBuilder local = ec.EmitStored(left);
			LocalBuilder local2 = ec.EmitStored(right);
			LocalBuilder localBuilder = ig.DeclareLocal(base.Type);
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			ec.EmitNullableHasValue(local);
			ec.EmitNullableHasValue(local2);
			ig.Emit(OpCodes.And);
			ig.Emit(OpCodes.Brtrue, label);
			ec.EmitNullableInitialize(localBuilder);
			ig.Emit(OpCodes.Br, label2);
			ig.MarkLabel(label);
			ec.EmitNullableGetValueOrDefault(local);
			ec.EmitNullableGetValueOrDefault(local2);
			EmitBinaryOperator(ec);
			ec.EmitNullableNew(localBuilder.LocalType);
			ig.MarkLabel(label2);
		}

		private void EmitLiftedRelationalBinary(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			LocalBuilder local = ec.EmitStored(left);
			LocalBuilder local2 = ec.EmitStored(right);
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			ec.EmitNullableGetValueOrDefault(local);
			ec.EmitNullableGetValueOrDefault(local2);
			ExpressionType nodeType = base.NodeType;
			if (nodeType == ExpressionType.Equal || nodeType == ExpressionType.NotEqual)
			{
				ig.Emit(OpCodes.Bne_Un, label);
			}
			else
			{
				EmitBinaryOperator(ec);
				ig.Emit(OpCodes.Brfalse, label);
			}
			ec.EmitNullableHasValue(local);
			ec.EmitNullableHasValue(local2);
			switch (base.NodeType)
			{
			case ExpressionType.Equal:
				ig.Emit(OpCodes.Ceq);
				break;
			case ExpressionType.NotEqual:
				ig.Emit(OpCodes.Ceq);
				ig.Emit(OpCodes.Ldc_I4_0);
				ig.Emit(OpCodes.Ceq);
				break;
			default:
				ig.Emit(OpCodes.And);
				break;
			}
			ig.Emit(OpCodes.Br, label2);
			ig.MarkLabel(label);
			ig.Emit((base.NodeType != ExpressionType.NotEqual) ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
			ig.MarkLabel(label2);
		}

		private void EmitArithmeticBinary(EmitContext ec)
		{
			if (!IsLifted)
			{
				EmitNonLiftedBinary(ec);
			}
			else
			{
				EmitLiftedArithmeticBinary(ec);
			}
		}

		private void EmitNonLiftedBinary(EmitContext ec)
		{
			ec.Emit(left);
			ec.Emit(right);
			EmitBinaryOperator(ec);
		}

		private void EmitRelationalBinary(EmitContext ec)
		{
			if (!IsLifted)
			{
				EmitNonLiftedBinary(ec);
			}
			else if (IsLiftedToNull)
			{
				EmitLiftedToNullBinary(ec);
			}
			else
			{
				EmitLiftedRelationalBinary(ec);
			}
		}

		private void EmitLiftedUserDefinedOperator(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			Label label3 = ig.DefineLabel();
			LocalBuilder local = ec.EmitStored(left);
			LocalBuilder local2 = ec.EmitStored(right);
			ec.EmitNullableHasValue(local);
			ec.EmitNullableHasValue(local2);
			switch (base.NodeType)
			{
			case ExpressionType.Equal:
				ig.Emit(OpCodes.Bne_Un, label2);
				ec.EmitNullableHasValue(local);
				ig.Emit(OpCodes.Brfalse, label);
				break;
			case ExpressionType.NotEqual:
				ig.Emit(OpCodes.Bne_Un, label);
				ec.EmitNullableHasValue(local);
				ig.Emit(OpCodes.Brfalse, label2);
				break;
			default:
				ig.Emit(OpCodes.And);
				ig.Emit(OpCodes.Brfalse, label2);
				break;
			}
			ec.EmitNullableGetValueOrDefault(local);
			ec.EmitNullableGetValueOrDefault(local2);
			ec.EmitCall(method);
			ig.Emit(OpCodes.Br, label3);
			ig.MarkLabel(label);
			ig.Emit(OpCodes.Ldc_I4_1);
			ig.Emit(OpCodes.Br, label3);
			ig.MarkLabel(label2);
			ig.Emit(OpCodes.Ldc_I4_0);
			ig.Emit(OpCodes.Br, label3);
			ig.MarkLabel(label3);
		}

		private void EmitLiftedToNullUserDefinedOperator(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			LocalBuilder local = ec.EmitStored(left);
			LocalBuilder local2 = ec.EmitStored(right);
			ec.EmitNullableHasValue(local);
			ec.EmitNullableHasValue(local2);
			ig.Emit(OpCodes.And);
			ig.Emit(OpCodes.Brfalse, label);
			ec.EmitNullableGetValueOrDefault(local);
			ec.EmitNullableGetValueOrDefault(local2);
			ec.EmitCall(method);
			ec.EmitNullableNew(base.Type);
			ig.Emit(OpCodes.Br, label2);
			ig.MarkLabel(label);
			LocalBuilder local3 = ig.DeclareLocal(base.Type);
			ec.EmitNullableInitialize(local3);
			ig.MarkLabel(label2);
		}

		private void EmitUserDefinedLiftedLogicalShortCircuit(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			bool flag = base.NodeType == ExpressionType.AndAlso;
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			Label label3 = ig.DefineLabel();
			Label label4 = ig.DefineLabel();
			LocalBuilder local = ec.EmitStored(left);
			ec.EmitNullableHasValue(local);
			ig.Emit(OpCodes.Brfalse, (!flag) ? label : label3);
			ec.EmitNullableGetValueOrDefault(local);
			ec.EmitCall((!flag) ? GetTrueOperator() : GetFalseOperator());
			ig.Emit(OpCodes.Brtrue, label2);
			ig.MarkLabel(label);
			LocalBuilder local2 = ec.EmitStored(right);
			ec.EmitNullableHasValue(local2);
			ig.Emit(OpCodes.Brfalse, label3);
			ec.EmitNullableGetValueOrDefault(local);
			ec.EmitNullableGetValueOrDefault(local2);
			ec.EmitCall(method);
			ec.EmitNullableNew(base.Type);
			ig.Emit(OpCodes.Br, label4);
			ig.MarkLabel(label2);
			ec.EmitLoad(local);
			ig.Emit(OpCodes.Br, label4);
			ig.MarkLabel(label3);
			LocalBuilder local3 = ig.DeclareLocal(base.Type);
			ec.EmitNullableInitialize(local3);
			ig.MarkLabel(label4);
		}

		private void EmitUserDefinedOperator(EmitContext ec)
		{
			if (!IsLifted)
			{
				ExpressionType nodeType = base.NodeType;
				if (nodeType == ExpressionType.AndAlso || nodeType == ExpressionType.OrElse)
				{
					EmitUserDefinedLogicalShortCircuit(ec);
					return;
				}
				left.Emit(ec);
				right.Emit(ec);
				ec.EmitCall(method);
			}
			else if (IsLiftedToNull)
			{
				ExpressionType nodeType = base.NodeType;
				if (nodeType == ExpressionType.AndAlso || nodeType == ExpressionType.OrElse)
				{
					EmitUserDefinedLiftedLogicalShortCircuit(ec);
				}
				else
				{
					EmitLiftedToNullUserDefinedOperator(ec);
				}
			}
			else
			{
				EmitLiftedUserDefinedOperator(ec);
			}
		}

		internal override void Emit(EmitContext ec)
		{
			if (method != null)
			{
				EmitUserDefinedOperator(ec);
				return;
			}
			switch (base.NodeType)
			{
			case ExpressionType.ArrayIndex:
				EmitArrayAccess(ec);
				break;
			case ExpressionType.Coalesce:
				if (conversion != null)
				{
					EmitConvertedCoalesce(ec);
				}
				else
				{
					EmitCoalesce(ec);
				}
				break;
			case ExpressionType.Add:
			case ExpressionType.AddChecked:
			case ExpressionType.Divide:
			case ExpressionType.ExclusiveOr:
			case ExpressionType.LeftShift:
			case ExpressionType.Modulo:
			case ExpressionType.Multiply:
			case ExpressionType.MultiplyChecked:
			case ExpressionType.Power:
			case ExpressionType.RightShift:
			case ExpressionType.Subtract:
			case ExpressionType.SubtractChecked:
				EmitArithmeticBinary(ec);
				break;
			case ExpressionType.Equal:
			case ExpressionType.GreaterThan:
			case ExpressionType.GreaterThanOrEqual:
			case ExpressionType.LessThan:
			case ExpressionType.LessThanOrEqual:
			case ExpressionType.NotEqual:
				EmitRelationalBinary(ec);
				break;
			case ExpressionType.And:
			case ExpressionType.AndAlso:
			case ExpressionType.Or:
			case ExpressionType.OrElse:
				EmitLogicalBinary(ec);
				break;
			default:
				throw new NotSupportedException(base.NodeType.ToString());
			}
		}
	}
}
