using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.Reflection
{
	internal class MonoCMethod : ConstructorInfo, ISerializable
	{
		internal IntPtr mhandle;

		private string name;

		private Type reftype;

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
				return MonoMethod.get_name(this);
			}
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return MonoMethodInfo.GetMethodImplementationFlags(mhandle);
		}

		public override ParameterInfo[] GetParameters()
		{
			return MonoMethodInfo.GetParametersInfo(mhandle, this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern object InternalInvoke(object obj, object[] parameters, out Exception exc);

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			if (binder == null)
			{
				binder = Binder.DefaultBinder;
			}
			ParameterInfo[] parameters2 = GetParameters();
			if ((parameters == null && parameters2.Length != 0) || (parameters != null && parameters.Length != parameters2.Length))
			{
				throw new TargetParameterCountException("parameters do not match signature");
			}
			if ((invokeAttr & BindingFlags.ExactBinding) == 0)
			{
				if (!Binder.ConvertArgs(binder, parameters, parameters2, culture))
				{
					throw new ArgumentException("failed to convert parameters");
				}
			}
			else
			{
				for (int i = 0; i < parameters2.Length; i++)
				{
					if (parameters[i].GetType() != parameters2[i].ParameterType)
					{
						throw new ArgumentException("parameters do not match signature");
					}
				}
			}
			if (obj == null && DeclaringType.ContainsGenericParameters)
			{
				throw new MemberAccessException(string.Concat("Cannot create an instance of ", DeclaringType, " because Type.ContainsGenericParameters is true."));
			}
			if ((invokeAttr & BindingFlags.CreateInstance) != BindingFlags.Default && DeclaringType.IsAbstract)
			{
				throw new MemberAccessException(string.Format("Cannot create an instance of {0} because it is an abstract class", DeclaringType));
			}
			Exception exc = null;
			object obj2 = null;
			try
			{
				obj2 = InternalInvoke(obj, parameters, out exc);
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
			return (obj != null) ? null : obj2;
		}

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return Invoke(null, invokeAttr, binder, parameters, culture);
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

		public override MethodBody GetMethodBody()
		{
			return MethodBase.GetMethodBody(mhandle);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Void ");
			stringBuilder.Append(Name);
			stringBuilder.Append("(");
			ParameterInfo[] parameters = GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(parameters[i].ParameterType.Name);
			}
			if (CallingConvention == CallingConventions.Any)
			{
				stringBuilder.Append(", ...");
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			MemberInfoSerializationHolder.Serialize(info, Name, ReflectedType, ToString(), MemberTypes.Constructor);
		}
	}
}
