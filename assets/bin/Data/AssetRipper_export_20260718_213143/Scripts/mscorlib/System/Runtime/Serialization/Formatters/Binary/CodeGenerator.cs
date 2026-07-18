using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal class CodeGenerator
	{
		private static object monitor;

		private static ModuleBuilder _module;

		static CodeGenerator()
		{
			monitor = new object();
			AppDomain domain = Thread.GetDomain();
			AssemblyBuilder assemblyBuilder = domain.DefineInternalDynamicAssembly(new AssemblyName
			{
				Name = "__MetadataTypes"
			}, AssemblyBuilderAccess.Run);
			_module = assemblyBuilder.DefineDynamicModule("__MetadataTypesModule", false);
		}

		public static Type GenerateMetadataType(Type type, StreamingContext context)
		{
			lock (monitor)
			{
				return GenerateMetadataTypeInternal(type, context);
			}
		}

		public static Type GenerateMetadataTypeInternal(Type type, StreamingContext context)
		{
			string text = type.Name + "__TypeMetadata";
			string text2 = string.Empty;
			int num = 0;
			while (_module.GetType(text + text2) != null)
			{
				text2 = (++num).ToString();
			}
			text += text2;
			MemberInfo[] serializableMembers = FormatterServices.GetSerializableMembers(type, context);
			TypeBuilder typeBuilder = _module.DefineType(text, TypeAttributes.Public, typeof(ClrTypeMetadata));
			Type[] emptyTypes = Type.EmptyTypes;
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, emptyTypes);
			ConstructorInfo constructor = typeof(ClrTypeMetadata).GetConstructor(new Type[1] { typeof(Type) });
			ILGenerator iLGenerator = constructorBuilder.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldtoken, type);
			iLGenerator.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
			iLGenerator.Emit(OpCodes.Call, constructor);
			iLGenerator.Emit(OpCodes.Ret);
			emptyTypes = new Type[2]
			{
				typeof(ObjectWriter),
				typeof(BinaryWriter)
			};
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("WriteAssemblies", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), emptyTypes);
			iLGenerator = methodBuilder.GetILGenerator();
			MemberInfo[] array = serializableMembers;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = (FieldInfo)array[i];
				Type type2 = fieldInfo.FieldType;
				while (type2.IsArray)
				{
					type2 = type2.GetElementType();
				}
				if (type2.Assembly != ObjectWriter.CorlibAssembly)
				{
					iLGenerator.Emit(OpCodes.Ldarg_1);
					iLGenerator.Emit(OpCodes.Ldarg_2);
					EmitLoadTypeAssembly(iLGenerator, type2, fieldInfo.Name);
					iLGenerator.EmitCall(OpCodes.Callvirt, typeof(ObjectWriter).GetMethod("WriteAssembly"), null);
					iLGenerator.Emit(OpCodes.Pop);
				}
			}
			iLGenerator.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(methodBuilder, typeof(TypeMetadata).GetMethod("WriteAssemblies"));
			emptyTypes = new Type[3]
			{
				typeof(ObjectWriter),
				typeof(BinaryWriter),
				typeof(bool)
			};
			methodBuilder = typeBuilder.DefineMethod("WriteTypeData", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), emptyTypes);
			iLGenerator = methodBuilder.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_2);
			iLGenerator.Emit(OpCodes.Ldc_I4, serializableMembers.Length);
			EmitWrite(iLGenerator, typeof(int));
			MemberInfo[] array2 = serializableMembers;
			for (int j = 0; j < array2.Length; j++)
			{
				FieldInfo fieldInfo2 = (FieldInfo)array2[j];
				iLGenerator.Emit(OpCodes.Ldarg_2);
				iLGenerator.Emit(OpCodes.Ldstr, fieldInfo2.Name);
				EmitWrite(iLGenerator, typeof(string));
			}
			Label label = iLGenerator.DefineLabel();
			iLGenerator.Emit(OpCodes.Ldarg_3);
			iLGenerator.Emit(OpCodes.Brfalse, label);
			MemberInfo[] array3 = serializableMembers;
			for (int k = 0; k < array3.Length; k++)
			{
				FieldInfo fieldInfo3 = (FieldInfo)array3[k];
				iLGenerator.Emit(OpCodes.Ldarg_2);
				iLGenerator.Emit(OpCodes.Ldc_I4_S, (byte)ObjectWriter.GetTypeTag(fieldInfo3.FieldType));
				EmitWrite(iLGenerator, typeof(byte));
			}
			MemberInfo[] array4 = serializableMembers;
			for (int l = 0; l < array4.Length; l++)
			{
				FieldInfo fieldInfo4 = (FieldInfo)array4[l];
				EmitWriteTypeSpec(iLGenerator, fieldInfo4.FieldType, fieldInfo4.Name);
			}
			iLGenerator.MarkLabel(label);
			iLGenerator.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(methodBuilder, typeof(TypeMetadata).GetMethod("WriteTypeData"));
			emptyTypes = new Type[3]
			{
				typeof(ObjectWriter),
				typeof(BinaryWriter),
				typeof(object)
			};
			methodBuilder = typeBuilder.DefineMethod("WriteObjectData", MethodAttributes.Public | MethodAttributes.Virtual, typeof(void), emptyTypes);
			iLGenerator = methodBuilder.GetILGenerator();
			LocalBuilder local = iLGenerator.DeclareLocal(type);
			OpCode opcode = OpCodes.Ldloc;
			iLGenerator.Emit(OpCodes.Ldarg_3);
			if (type.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Unbox, type);
				LoadFromPtr(iLGenerator, type);
				opcode = OpCodes.Ldloca_S;
			}
			else
			{
				iLGenerator.Emit(OpCodes.Castclass, type);
			}
			iLGenerator.Emit(OpCodes.Stloc, local);
			MemberInfo[] array5 = serializableMembers;
			for (int m = 0; m < array5.Length; m++)
			{
				FieldInfo fieldInfo5 = (FieldInfo)array5[m];
				Type fieldType = fieldInfo5.FieldType;
				if (BinaryCommon.IsPrimitive(fieldType))
				{
					iLGenerator.Emit(OpCodes.Ldarg_2);
					iLGenerator.Emit(opcode, local);
					if (fieldType == typeof(DateTime) || fieldType == typeof(TimeSpan) || fieldType == typeof(decimal))
					{
						iLGenerator.Emit(OpCodes.Ldflda, fieldInfo5);
					}
					else
					{
						iLGenerator.Emit(OpCodes.Ldfld, fieldInfo5);
					}
					EmitWritePrimitiveValue(iLGenerator, fieldType);
					continue;
				}
				iLGenerator.Emit(OpCodes.Ldarg_1);
				iLGenerator.Emit(OpCodes.Ldarg_2);
				iLGenerator.Emit(OpCodes.Ldtoken, fieldType);
				iLGenerator.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
				iLGenerator.Emit(opcode, local);
				iLGenerator.Emit(OpCodes.Ldfld, fieldInfo5);
				if (fieldType.IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, fieldType);
				}
				iLGenerator.EmitCall(OpCodes.Call, typeof(ObjectWriter).GetMethod("WriteValue"), null);
			}
			iLGenerator.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(methodBuilder, typeof(TypeMetadata).GetMethod("WriteObjectData"));
			return typeBuilder.CreateType();
		}

		public static void LoadFromPtr(ILGenerator ig, Type t)
		{
			if (t == typeof(int))
			{
				ig.Emit(OpCodes.Ldind_I4);
			}
			else if (t == typeof(uint))
			{
				ig.Emit(OpCodes.Ldind_U4);
			}
			else if (t == typeof(short))
			{
				ig.Emit(OpCodes.Ldind_I2);
			}
			else if (t == typeof(ushort))
			{
				ig.Emit(OpCodes.Ldind_U2);
			}
			else if (t == typeof(char))
			{
				ig.Emit(OpCodes.Ldind_U2);
			}
			else if (t == typeof(byte))
			{
				ig.Emit(OpCodes.Ldind_U1);
			}
			else if (t == typeof(sbyte))
			{
				ig.Emit(OpCodes.Ldind_I1);
			}
			else if (t == typeof(ulong))
			{
				ig.Emit(OpCodes.Ldind_I8);
			}
			else if (t == typeof(long))
			{
				ig.Emit(OpCodes.Ldind_I8);
			}
			else if (t == typeof(float))
			{
				ig.Emit(OpCodes.Ldind_R4);
			}
			else if (t == typeof(double))
			{
				ig.Emit(OpCodes.Ldind_R8);
			}
			else if (t == typeof(bool))
			{
				ig.Emit(OpCodes.Ldind_I1);
			}
			else if (t == typeof(IntPtr))
			{
				ig.Emit(OpCodes.Ldind_I);
			}
			else if (t.IsEnum)
			{
				if (t == typeof(Enum))
				{
					ig.Emit(OpCodes.Ldind_Ref);
				}
				else
				{
					LoadFromPtr(ig, EnumToUnderlying(t));
				}
			}
			else if (t.IsValueType)
			{
				ig.Emit(OpCodes.Ldobj, t);
			}
			else
			{
				ig.Emit(OpCodes.Ldind_Ref);
			}
		}

		private static void EmitWriteTypeSpec(ILGenerator gen, Type type, string member)
		{
			switch (ObjectWriter.GetTypeTag(type))
			{
			case TypeTag.PrimitiveType:
				gen.Emit(OpCodes.Ldarg_2);
				gen.Emit(OpCodes.Ldc_I4_S, BinaryCommon.GetTypeCode(type));
				EmitWrite(gen, typeof(byte));
				break;
			case TypeTag.RuntimeType:
				gen.Emit(OpCodes.Ldarg_2);
				gen.Emit(OpCodes.Ldstr, type.FullName);
				EmitWrite(gen, typeof(string));
				break;
			case TypeTag.GenericType:
				gen.Emit(OpCodes.Ldarg_2);
				gen.Emit(OpCodes.Ldstr, type.FullName);
				EmitWrite(gen, typeof(string));
				gen.Emit(OpCodes.Ldarg_2);
				gen.Emit(OpCodes.Ldarg_1);
				EmitLoadTypeAssembly(gen, type, member);
				gen.EmitCall(OpCodes.Callvirt, typeof(ObjectWriter).GetMethod("GetAssemblyId"), null);
				gen.Emit(OpCodes.Conv_I4);
				EmitWrite(gen, typeof(int));
				break;
			case TypeTag.ArrayOfPrimitiveType:
				gen.Emit(OpCodes.Ldarg_2);
				gen.Emit(OpCodes.Ldc_I4_S, BinaryCommon.GetTypeCode(type.GetElementType()));
				EmitWrite(gen, typeof(byte));
				break;
			case TypeTag.String:
			case TypeTag.ObjectType:
			case TypeTag.ArrayOfObject:
			case TypeTag.ArrayOfString:
				break;
			}
		}

		private static void EmitLoadTypeAssembly(ILGenerator gen, Type type, string member)
		{
			gen.Emit(OpCodes.Ldtoken, type);
			gen.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
			gen.EmitCall(OpCodes.Callvirt, typeof(Type).GetProperty("Assembly").GetGetMethod(), null);
		}

		private static void EmitWrite(ILGenerator gen, Type type)
		{
			gen.EmitCall(OpCodes.Callvirt, typeof(BinaryWriter).GetMethod("Write", new Type[1] { type }), null);
		}

		public static void EmitWritePrimitiveValue(ILGenerator gen, Type type)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
			case TypeCode.Char:
			case TypeCode.SByte:
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.UInt16:
			case TypeCode.Int32:
			case TypeCode.UInt32:
			case TypeCode.Int64:
			case TypeCode.UInt64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.String:
				EmitWrite(gen, type);
				return;
			case TypeCode.Decimal:
				gen.EmitCall(OpCodes.Call, typeof(CultureInfo).GetProperty("InvariantCulture").GetGetMethod(), null);
				gen.EmitCall(OpCodes.Call, typeof(decimal).GetMethod("ToString", new Type[1] { typeof(IFormatProvider) }), null);
				EmitWrite(gen, typeof(string));
				return;
			case TypeCode.DateTime:
				gen.EmitCall(OpCodes.Call, typeof(DateTime).GetMethod("ToBinary", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), null);
				EmitWrite(gen, typeof(long));
				return;
			}
			if (type == typeof(TimeSpan))
			{
				gen.EmitCall(OpCodes.Call, typeof(TimeSpan).GetProperty("Ticks").GetGetMethod(), null);
				EmitWrite(gen, typeof(long));
				return;
			}
			throw new NotSupportedException("Unsupported primitive type: " + type.FullName);
		}

		public static Type EnumToUnderlying(Type t)
		{
			TypeCode typeCode = Type.GetTypeCode(t);
			switch (typeCode)
			{
			case TypeCode.Boolean:
				return typeof(bool);
			case TypeCode.Byte:
				return typeof(byte);
			case TypeCode.SByte:
				return typeof(sbyte);
			case TypeCode.Char:
				return typeof(char);
			case TypeCode.Int16:
				return typeof(short);
			case TypeCode.UInt16:
				return typeof(ushort);
			case TypeCode.Int32:
				return typeof(int);
			case TypeCode.UInt32:
				return typeof(uint);
			case TypeCode.Int64:
				return typeof(long);
			case TypeCode.UInt64:
				return typeof(ulong);
			default:
				throw new Exception(string.Concat("Unhandled typecode in enum ", typeCode, " from ", t.AssemblyQualifiedName));
			}
		}
	}
}
