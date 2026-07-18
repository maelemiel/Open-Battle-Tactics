using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace System.Reflection
{
	[Serializable]
	internal class MonoMethod : MethodInfo, ISerializable
	{
		internal IntPtr mhandle;

		private string name;

		private Type reftype;

		public override ParameterInfo ReturnParameter
		{
			get
			{
				return MonoMethodInfo.GetReturnParameterInfo(this);
			}
		}

		public override Type ReturnType
		{
			get
			{
				return MonoMethodInfo.GetReturnType(mhandle);
			}
		}

		public override ICustomAttributeProvider ReturnTypeCustomAttributes
		{
			get
			{
				return MonoMethodInfo.GetReturnParameterInfo(this);
			}
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return new RuntimeMethodHandle(mhandle);
			}
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return MonoMethodInfo.GetAttributes(mhandle);
			}
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return MonoMethodInfo.GetCallingConvention(mhandle);
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return reftype;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return MonoMethodInfo.GetDeclaringType(mhandle);
			}
		}

		public override string Name
		{
			get
			{
				if (name != null)
				{
					return name;
				}
				return get_name(this);
			}
		}

		public override extern bool IsGenericMethodDefinition
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public override extern bool IsGenericMethod
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				if (IsGenericMethod)
				{
					Type[] genericArguments = GetGenericArguments();
					foreach (Type type in genericArguments)
					{
						if (type.ContainsGenericParameters)
						{
							return true;
						}
					}
				}
				return DeclaringType.ContainsGenericParameters;
			}
		}

		internal MonoMethod()
		{
		}

		internal MonoMethod(RuntimeMethodHandle mhandle)
		{
			this.mhandle = mhandle.Value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern string get_name(MethodBase method);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MonoMethod get_base_definition(MonoMethod method);

		public override MethodInfo GetBaseDefinition()
		{
			return get_base_definition(this);
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MonoMethodInfo.GetMethodImplementationFlags(mhandle);
		}

		public override ParameterInfo[] GetParameters()
		{
			ParameterInfo[] parametersInfo = MonoMethodInfo.GetParametersInfo(mhandle, this);
			ParameterInfo[] array = new ParameterInfo[parametersInfo.Length];
			parametersInfo.CopyTo(array, 0);
			return array;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern object InternalInvoke(object obj, object[] parameters, out Exception exc);

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			if (binder == null)
			{
				binder = Binder.DefaultBinder;
			}
			ParameterInfo[] parametersInfo = MonoMethodInfo.GetParametersInfo(mhandle, this);
			if ((parameters == null && parametersInfo.Length != 0) || (parameters != null && parameters.Length != parametersInfo.Length))
			{
				throw new TargetParameterCountException("parameters do not match signature");
			}
			if ((invokeAttr & BindingFlags.ExactBinding) == 0)
			{
				if (!Binder.ConvertArgs(binder, parameters, parametersInfo, culture))
				{
					throw new ArgumentException("failed to convert parameters");
				}
			}
			else
			{
				for (int i = 0; i < parametersInfo.Length; i++)
				{
					if (parameters[i].GetType() != parametersInfo[i].ParameterType)
					{
						throw new ArgumentException("parameters do not match signature");
					}
				}
			}
			if (ContainsGenericParameters)
			{
				throw new InvalidOperationException("Late bound operations cannot be performed on types or methods for which ContainsGenericParameters is true.");
			}
			object obj2 = null;
			Exception exc;
			try
			{
				obj2 = InternalInvoke(obj, parameters, out exc);
			}
			catch (ThreadAbortException)
			{
				throw;
			}
			catch (MethodAccessException)
			{
				throw;
			}
			catch (Exception inner)
			{
				throw new TargetInvocationException(inner);
			}
			if (exc != null)
			{
				throw exc;
			}
			return obj2;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern DllImportAttribute GetDllImportAttribute(IntPtr mhandle);

		internal object[] GetPseudoCustomAttributes()
		{
			int num = 0;
			MonoMethodInfo methodInfo = MonoMethodInfo.GetMethodInfo(mhandle);
			if ((methodInfo.iattrs & MethodImplAttributes.PreserveSig) != MethodImplAttributes.IL)
			{
				num++;
			}
			if ((methodInfo.attrs & MethodAttributes.PinvokeImpl) != MethodAttributes.PrivateScope)
			{
				num++;
			}
			if (num == 0)
			{
				return null;
			}
			object[] array = new object[num];
			num = 0;
			if ((methodInfo.iattrs & MethodImplAttributes.PreserveSig) != MethodImplAttributes.IL)
			{
				array[num++] = new PreserveSigAttribute();
			}
			if ((methodInfo.attrs & MethodAttributes.PinvokeImpl) != MethodAttributes.PrivateScope)
			{
				DllImportAttribute dllImportAttribute = GetDllImportAttribute(mhandle);
				if ((methodInfo.iattrs & MethodImplAttributes.PreserveSig) != MethodImplAttributes.IL)
				{
					dllImportAttribute.PreserveSig = true;
				}
				array[num++] = dllImportAttribute;
			}
			return array;
		}

		private static bool ShouldPrintFullName(Type type)
		{
			return type.IsClass && (!type.IsPointer || (!type.GetElementType().IsPrimitive && !type.GetElementType().IsNested));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Type returnType = ReturnType;
			if (ShouldPrintFullName(returnType))
			{
				stringBuilder.Append(returnType.ToString());
			}
			else
			{
				stringBuilder.Append(returnType.Name);
			}
			stringBuilder.Append(" ");
			stringBuilder.Append(Name);
			if (IsGenericMethod)
			{
				Type[] genericArguments = GetGenericArguments();
				stringBuilder.Append("[");
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(genericArguments[i].Name);
				}
				stringBuilder.Append("]");
			}
			stringBuilder.Append("(");
			ParameterInfo[] parameters = GetParameters();
			for (int j = 0; j < parameters.Length; j++)
			{
				if (j > 0)
				{
					stringBuilder.Append(", ");
				}
				Type type = parameters[j].ParameterType;
				bool isByRef = type.IsByRef;
				if (isByRef)
				{
					type = type.GetElementType();
				}
				if (ShouldPrintFullName(type))
				{
					stringBuilder.Append(type.ToString());
				}
				else
				{
					stringBuilder.Append(type.Name);
				}
				if (isByRef)
				{
					stringBuilder.Append(" ByRef");
				}
			}
			if ((CallingConvention & CallingConventions.VarArgs) != 0)
			{
				if (parameters.Length > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("...");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			Type[] genericArguments = ((!IsGenericMethod || IsGenericMethodDefinition) ? null : GetGenericArguments());
			MemberInfoSerializationHolder.Serialize(info, Name, ReflectedType, ToString(), MemberTypes.Method, genericArguments);
		}

		public override MethodInfo MakeGenericMethod(Type[] methodInstantiation)
		{
			if (methodInstantiation == null)
			{
				throw new ArgumentNullException("methodInstantiation");
			}
			foreach (Type type in methodInstantiation)
			{
				if (type == null)
				{
					throw new ArgumentNullException();
				}
			}
			MethodInfo methodInfo = MakeGenericMethod_impl(methodInstantiation);
			if (methodInfo == null)
			{
				throw new ArgumentException(string.Format("The method has {0} generic parameter(s) but {1} generic argument(s) were provided.", GetGenericArguments().Length, methodInstantiation.Length));
			}
			return methodInfo;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern MethodInfo MakeGenericMethod_impl(Type[] types);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public override extern Type[] GetGenericArguments();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern MethodInfo GetGenericMethodDefinition_impl();

		public override MethodInfo GetGenericMethodDefinition()
		{
			MethodInfo genericMethodDefinition_impl = GetGenericMethodDefinition_impl();
			if (genericMethodDefinition_impl == null)
			{
				throw new InvalidOperationException();
			}
			return genericMethodDefinition_impl;
		}

		public override MethodBody GetMethodBody()
		{
			return MethodBase.GetMethodBody(mhandle);
		}
	}
}
