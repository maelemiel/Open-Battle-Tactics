using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace FastDynamicMemberAccessor
{
	internal abstract class MemberAccessor : IMemberAccessor
	{
		private const string emmitedTypeName = "Member";

		protected readonly Type _targetType;

		protected readonly string _fieldName;

		protected static readonly Hashtable s_TypeHash;

		private IMemberAccessor _emittedMemberAccessor;

		internal abstract bool CanRead { get; }

		internal abstract bool CanWrite { get; }

		internal Type TargetType
		{
			get
			{
				return _targetType;
			}
		}

		internal abstract Type MemberType { get; }

		protected MemberAccessor(MemberInfo member)
		{
			_targetType = member.ReflectedType;
			_fieldName = member.Name;
		}

		internal static MemberAccessor Make(PropertyInfo p_propertyInfo, FieldInfo p_fieldInfo)
		{
			if (p_propertyInfo != null)
			{
				return new PropertyAccessor(p_propertyInfo);
			}
			return new FieldAccessor(p_fieldInfo);
		}

		static MemberAccessor()
		{
			s_TypeHash = new Hashtable();
			s_TypeHash[typeof(sbyte)] = OpCodes.Ldind_I1;
			s_TypeHash[typeof(byte)] = OpCodes.Ldind_U1;
			s_TypeHash[typeof(char)] = OpCodes.Ldind_U2;
			s_TypeHash[typeof(short)] = OpCodes.Ldind_I2;
			s_TypeHash[typeof(ushort)] = OpCodes.Ldind_U2;
			s_TypeHash[typeof(int)] = OpCodes.Ldind_I4;
			s_TypeHash[typeof(uint)] = OpCodes.Ldind_U4;
			s_TypeHash[typeof(long)] = OpCodes.Ldind_I8;
			s_TypeHash[typeof(ulong)] = OpCodes.Ldind_I8;
			s_TypeHash[typeof(bool)] = OpCodes.Ldind_I1;
			s_TypeHash[typeof(double)] = OpCodes.Ldind_R8;
			s_TypeHash[typeof(float)] = OpCodes.Ldind_R4;
		}

		public object Get(object target)
		{
			if (CanRead)
			{
				EnsureInit();
				return _emittedMemberAccessor.Get(target);
			}
			throw new MemberAccessorException(string.Format("Member \"{0}\" does not have a get method.", _fieldName));
		}

		public void Set(object target, object value)
		{
			if (CanWrite)
			{
				EnsureInit();
				_emittedMemberAccessor.Set(target, value);
				return;
			}
			throw new MemberAccessorException(string.Format("Member \"{0}\" does not have a set method.", _fieldName));
		}

		private void EnsureInit()
		{
			if (_emittedMemberAccessor == null)
			{
				Assembly assembly = EmitAssembly();
				_emittedMemberAccessor = assembly.CreateInstance("Member") as IMemberAccessor;
				if (_emittedMemberAccessor == null)
				{
					throw new Exception("Unable to create member accessor.");
				}
			}
		}

		private Assembly EmitAssembly()
		{
			AssemblyName assemblyName = new AssemblyName();
			assemblyName.Name = "PropertyAccessorAssembly";
			AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");
			TypeBuilder typeBuilder = moduleBuilder.DefineType("Member", TypeAttributes.Public | TypeAttributes.Sealed);
			typeBuilder.AddInterfaceImplementation(typeof(IMemberAccessor));
			typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
			_EmitGetter(typeBuilder);
			_EmitSetter(typeBuilder);
			typeBuilder.CreateType();
			return assemblyBuilder;
		}

		protected abstract void _EmitGetter(TypeBuilder type);

		protected abstract void _EmitSetter(TypeBuilder type);
	}
}
