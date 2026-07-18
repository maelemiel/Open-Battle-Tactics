using System.Reflection;
using System.Reflection.Emit;

namespace System.Linq.Expressions
{
	public sealed class UnaryExpression : Expression
	{
		private Expression operand;

		private MethodInfo method;

		private bool is_lifted;

		public Expression Operand
		{
			get
			{
				return operand;
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
				return is_lifted && base.Type.IsNullable();
			}
		}

		internal UnaryExpression(ExpressionType node_type, Expression operand, Type type)
			: base(node_type, type)
		{
			this.operand = operand;
		}

		internal UnaryExpression(ExpressionType node_type, Expression operand, Type type, MethodInfo method, bool is_lifted)
			: base(node_type, type)
		{
			this.operand = operand;
			this.method = method;
			this.is_lifted = is_lifted;
		}

		private void EmitArrayLength(EmitContext ec)
		{
			operand.Emit(ec);
			ec.ig.Emit(OpCodes.Ldlen);
		}

		private void EmitTypeAs(EmitContext ec)
		{
			Type type = base.Type;
			ec.EmitIsInst(operand, type);
			if (type.IsNullable())
			{
				ec.ig.Emit(OpCodes.Unbox_Any, type);
			}
		}

		private void EmitLiftedUnary(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			LocalBuilder local = ec.EmitStored(operand);
			LocalBuilder local2 = ig.DeclareLocal(base.Type);
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			ec.EmitNullableHasValue(local);
			ig.Emit(OpCodes.Brtrue, label);
			ec.EmitNullableInitialize(local2);
			ig.Emit(OpCodes.Br, label2);
			ig.MarkLabel(label);
			ec.EmitNullableGetValueOrDefault(local);
			EmitUnaryOperator(ec);
			ec.EmitNullableNew(base.Type);
			ig.MarkLabel(label2);
		}

		private void EmitUnaryOperator(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			switch (base.NodeType)
			{
			case ExpressionType.Not:
				if (operand.Type.GetNotNullableType() == typeof(bool))
				{
					ig.Emit(OpCodes.Ldc_I4_0);
					ig.Emit(OpCodes.Ceq);
				}
				else
				{
					ig.Emit(OpCodes.Not);
				}
				break;
			case ExpressionType.Negate:
				ig.Emit(OpCodes.Neg);
				break;
			case ExpressionType.NegateChecked:
				ig.Emit(OpCodes.Ldc_I4_M1);
				ig.Emit((!Expression.IsUnsigned(operand.Type)) ? OpCodes.Mul_Ovf : OpCodes.Mul_Ovf_Un);
				break;
			case ExpressionType.Convert:
			case ExpressionType.ConvertChecked:
				EmitPrimitiveConversion(ec, operand.Type.GetNotNullableType(), base.Type.GetNotNullableType());
				break;
			}
		}

		private void EmitConvert(EmitContext ec)
		{
			Type type = operand.Type;
			Type type2 = base.Type;
			if (type == type2)
			{
				operand.Emit(ec);
				return;
			}
			if (type.IsNullable() && !type2.IsNullable())
			{
				EmitConvertFromNullable(ec);
				return;
			}
			if (!type.IsNullable() && type2.IsNullable())
			{
				EmitConvertToNullable(ec);
				return;
			}
			if (type.IsNullable() && type2.IsNullable())
			{
				EmitConvertFromNullableToNullable(ec);
				return;
			}
			if (Expression.IsReferenceConversion(type, type2))
			{
				EmitCast(ec);
				return;
			}
			if (Expression.IsPrimitiveConversion(type, type2))
			{
				EmitPrimitiveConversion(ec);
				return;
			}
			throw new NotImplementedException();
		}

		private void EmitConvertFromNullableToNullable(EmitContext ec)
		{
			EmitLiftedUnary(ec);
		}

		private void EmitConvertToNullable(EmitContext ec)
		{
			ec.Emit(operand);
			if (IsUnBoxing())
			{
				EmitUnbox(ec);
				return;
			}
			if (operand.Type != base.Type.GetNotNullableType())
			{
				EmitPrimitiveConversion(ec, operand.Type, base.Type.GetNotNullableType());
			}
			ec.EmitNullableNew(base.Type);
		}

		private void EmitConvertFromNullable(EmitContext ec)
		{
			if (IsBoxing())
			{
				ec.Emit(operand);
				EmitBox(ec);
				return;
			}
			ec.EmitCall(operand, operand.Type.GetMethod("get_Value"));
			if (operand.Type.GetNotNullableType() != base.Type)
			{
				EmitPrimitiveConversion(ec, operand.Type.GetNotNullableType(), base.Type);
			}
		}

		private bool IsBoxing()
		{
			return operand.Type.IsValueType && !base.Type.IsValueType;
		}

		private void EmitBox(EmitContext ec)
		{
			ec.ig.Emit(OpCodes.Box, operand.Type);
		}

		private bool IsUnBoxing()
		{
			return !operand.Type.IsValueType && base.Type.IsValueType;
		}

		private void EmitUnbox(EmitContext ec)
		{
			ec.ig.Emit(OpCodes.Unbox_Any, base.Type);
		}

		private void EmitCast(EmitContext ec)
		{
			operand.Emit(ec);
			if (IsBoxing())
			{
				EmitBox(ec);
			}
			else if (IsUnBoxing())
			{
				EmitUnbox(ec);
			}
			else
			{
				ec.ig.Emit(OpCodes.Castclass, base.Type);
			}
		}

		private void EmitPrimitiveConversion(EmitContext ec, bool is_unsigned, OpCode signed, OpCode unsigned, OpCode signed_checked, OpCode unsigned_checked)
		{
			if (base.NodeType != ExpressionType.ConvertChecked)
			{
				ec.ig.Emit((!is_unsigned) ? signed : unsigned);
			}
			else
			{
				ec.ig.Emit((!is_unsigned) ? signed_checked : unsigned_checked);
			}
		}

		private void EmitPrimitiveConversion(EmitContext ec)
		{
			operand.Emit(ec);
			EmitPrimitiveConversion(ec, operand.Type, base.Type);
		}

		private void EmitPrimitiveConversion(EmitContext ec, Type from, Type to)
		{
			bool flag = Expression.IsUnsigned(from);
			switch (Type.GetTypeCode(to))
			{
			case TypeCode.SByte:
				EmitPrimitiveConversion(ec, flag, OpCodes.Conv_I1, OpCodes.Conv_U1, OpCodes.Conv_Ovf_I1, OpCodes.Conv_Ovf_I1_Un);
				break;
			case TypeCode.Byte:
				EmitPrimitiveConversion(ec, flag, OpCodes.Conv_I1, OpCodes.Conv_U1, OpCodes.Conv_Ovf_U1, OpCodes.Conv_Ovf_U1_Un);
				break;
			case TypeCode.Int16:
				EmitPrimitiveConversion(ec, flag, OpCodes.Conv_I2, OpCodes.Conv_U2, OpCodes.Conv_Ovf_I2, OpCodes.Conv_Ovf_I2_Un);
				break;
			case TypeCode.UInt16:
				EmitPrimitiveConversion(ec, flag, OpCodes.Conv_I2, OpCodes.Conv_U2, OpCodes.Conv_Ovf_U2, OpCodes.Conv_Ovf_U2_Un);
				break;
			case TypeCode.Int32:
				EmitPrimitiveConversion(ec, flag, OpCodes.Conv_I4, OpCodes.Conv_U4, OpCodes.Conv_Ovf_I4, OpCodes.Conv_Ovf_I4_Un);
				break;
			case TypeCode.UInt32:
				EmitPrimitiveConversion(ec, flag, OpCodes.Conv_I4, OpCodes.Conv_U4, OpCodes.Conv_Ovf_U4, OpCodes.Conv_Ovf_U4_Un);
				break;
			case TypeCode.Int64:
				EmitPrimitiveConversion(ec, flag, OpCodes.Conv_I8, OpCodes.Conv_U8, OpCodes.Conv_Ovf_I8, OpCodes.Conv_Ovf_I8_Un);
				break;
			case TypeCode.UInt64:
				EmitPrimitiveConversion(ec, flag, OpCodes.Conv_I8, OpCodes.Conv_U8, OpCodes.Conv_Ovf_U8, OpCodes.Conv_Ovf_U8_Un);
				break;
			case TypeCode.Single:
				if (flag)
				{
					ec.ig.Emit(OpCodes.Conv_R_Un);
				}
				ec.ig.Emit(OpCodes.Conv_R4);
				break;
			case TypeCode.Double:
				if (flag)
				{
					ec.ig.Emit(OpCodes.Conv_R_Un);
				}
				ec.ig.Emit(OpCodes.Conv_R8);
				break;
			default:
				throw new NotImplementedException(base.Type.ToString());
			}
		}

		private void EmitArithmeticUnary(EmitContext ec)
		{
			if (!IsLifted)
			{
				operand.Emit(ec);
				EmitUnaryOperator(ec);
			}
			else
			{
				EmitLiftedUnary(ec);
			}
		}

		private void EmitUserDefinedLiftedToNullOperator(EmitContext ec)
		{
			ILGenerator ig = ec.ig;
			LocalBuilder local = ec.EmitStored(operand);
			Label label = ig.DefineLabel();
			Label label2 = ig.DefineLabel();
			ec.EmitNullableHasValue(local);
			ig.Emit(OpCodes.Brfalse, label);
			ec.EmitNullableGetValueOrDefault(local);
			ec.EmitCall(method);
			ec.EmitNullableNew(base.Type);
			ig.Emit(OpCodes.Br, label2);
			ig.MarkLabel(label);
			LocalBuilder local2 = ig.DeclareLocal(base.Type);
			ec.EmitNullableInitialize(local2);
			ig.MarkLabel(label2);
		}

		private void EmitUserDefinedLiftedOperator(EmitContext ec)
		{
			LocalBuilder local = ec.EmitStored(operand);
			ec.EmitNullableGetValue(local);
			ec.EmitCall(method);
		}

		private void EmitUserDefinedOperator(EmitContext ec)
		{
			if (!IsLifted)
			{
				ec.Emit(operand);
				ec.EmitCall(method);
			}
			else if (IsLiftedToNull)
			{
				EmitUserDefinedLiftedToNullOperator(ec);
			}
			else
			{
				EmitUserDefinedLiftedOperator(ec);
			}
		}

		private void EmitQuote(EmitContext ec)
		{
			ec.EmitScope();
			ec.EmitReadGlobal(operand, typeof(Expression));
			if (ec.HasHoistedLocals)
			{
				ec.EmitLoadHoistedLocalsStore();
			}
			else
			{
				ec.ig.Emit(OpCodes.Ldnull);
			}
			ec.EmitIsolateExpression();
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
			case ExpressionType.ArrayLength:
				EmitArrayLength(ec);
				break;
			case ExpressionType.TypeAs:
				EmitTypeAs(ec);
				break;
			case ExpressionType.Convert:
			case ExpressionType.ConvertChecked:
				EmitConvert(ec);
				break;
			case ExpressionType.Negate:
			case ExpressionType.UnaryPlus:
			case ExpressionType.NegateChecked:
			case ExpressionType.Not:
				EmitArithmeticUnary(ec);
				break;
			case ExpressionType.Quote:
				EmitQuote(ec);
				break;
			default:
				throw new NotImplementedException(base.NodeType.ToString());
			}
		}
	}
}
