using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastDynamicMemberAccessor
{
	internal class FieldAccessor : MemberAccessor
	{
		private readonly Type _propertyType;

		private readonly bool _canRead;

		private readonly bool _canWrite;

		internal override Type MemberType
		{
			get
			{
				return _propertyType;
			}
		}

		internal override bool CanRead
		{
			get
			{
				return _canRead;
			}
		}

		internal override bool CanWrite
		{
			get
			{
				return _canWrite;
			}
		}

		internal FieldAccessor(FieldInfo fieldInfo)
			: base(fieldInfo)
		{
			_canRead = true;
			_canWrite = !fieldInfo.IsLiteral && !fieldInfo.IsInitOnly;
			_propertyType = fieldInfo.FieldType;
		}

		protected override void _EmitSetter(TypeBuilder myType)
		{
			Type[] parameterTypes = new Type[2]
			{
				typeof(object),
				typeof(object)
			};
			Type returnType = null;
			MethodBuilder methodBuilder = myType.DefineMethod("Set", MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes);
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			FieldInfo field = _targetType.GetField(_fieldName);
			if (field != null)
			{
				Type fieldType = field.FieldType;
				iLGenerator.DeclareLocal(fieldType);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Castclass, _targetType);
				iLGenerator.Emit(OpCodes.Ldarg_2);
				if (fieldType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Unbox, fieldType);
					if (MemberAccessor.s_TypeHash[fieldType] != null)
					{
						OpCode opcode = (OpCode)MemberAccessor.s_TypeHash[fieldType];
						iLGenerator.Emit(opcode);
					}
					else
					{
						iLGenerator.Emit(OpCodes.Ldobj, fieldType);
					}
				}
				else
				{
					iLGenerator.Emit(OpCodes.Castclass, fieldType);
				}
				iLGenerator.Emit(OpCodes.Stfld, field);
			}
			else
			{
				iLGenerator.ThrowException(typeof(MissingMethodException));
			}
			iLGenerator.Emit(OpCodes.Ret);
		}

		protected override void _EmitGetter(TypeBuilder myType)
		{
			Type[] parameterTypes = new Type[1] { typeof(object) };
			Type typeFromHandle = typeof(object);
			MethodBuilder methodBuilder = myType.DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual, typeFromHandle, parameterTypes);
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			FieldInfo field = _targetType.GetField(_fieldName);
			if (field != null)
			{
				iLGenerator.DeclareLocal(typeof(object));
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Castclass, _targetType);
				iLGenerator.Emit(OpCodes.Ldfld, field);
				if (field.FieldType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, field.FieldType);
				}
				iLGenerator.Emit(OpCodes.Stloc_0);
				iLGenerator.Emit(OpCodes.Ldloc_0);
			}
			else
			{
				iLGenerator.ThrowException(typeof(MissingMethodException));
			}
			iLGenerator.Emit(OpCodes.Ret);
		}
	}
}
