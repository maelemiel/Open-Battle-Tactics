using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastDynamicMemberAccessor
{
	internal class PropertyAccessor : MemberAccessor
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

		internal PropertyAccessor(PropertyInfo info)
			: base(info)
		{
			_canRead = info.CanRead;
			_canWrite = info.CanWrite;
			_propertyType = info.PropertyType;
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
			MethodInfo method = _targetType.GetMethod("set_" + _fieldName);
			if (method != null)
			{
				Type parameterType = method.GetParameters()[0].ParameterType;
				iLGenerator.DeclareLocal(parameterType);
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Castclass, _targetType);
				iLGenerator.Emit(OpCodes.Ldarg_2);
				if (parameterType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Unbox, parameterType);
					if (MemberAccessor.s_TypeHash[parameterType] != null)
					{
						OpCode opcode = (OpCode)MemberAccessor.s_TypeHash[parameterType];
						iLGenerator.Emit(opcode);
					}
					else
					{
						iLGenerator.Emit(OpCodes.Ldobj, parameterType);
					}
				}
				else
				{
					iLGenerator.Emit(OpCodes.Castclass, parameterType);
				}
				iLGenerator.EmitCall(OpCodes.Callvirt, method, null);
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
			MethodInfo method = _targetType.GetMethod("get_" + _fieldName);
			if (method != null)
			{
				iLGenerator.DeclareLocal(typeof(object));
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Castclass, _targetType);
				iLGenerator.EmitCall(OpCodes.Call, method, null);
				if (method.ReturnType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, method.ReturnType);
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
