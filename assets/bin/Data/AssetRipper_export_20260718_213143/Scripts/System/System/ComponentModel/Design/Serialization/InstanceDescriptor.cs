using System.Collections;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization
{
	public sealed class InstanceDescriptor
	{
		private MemberInfo member;

		private ICollection arguments;

		private bool isComplete;

		public ICollection Arguments
		{
			get
			{
				if (arguments == null)
				{
					return new object[0];
				}
				return arguments;
			}
		}

		public bool IsComplete
		{
			get
			{
				return isComplete;
			}
		}

		public MemberInfo MemberInfo
		{
			get
			{
				return member;
			}
		}

		public InstanceDescriptor(MemberInfo member, ICollection arguments)
			: this(member, arguments, true)
		{
		}

		public InstanceDescriptor(MemberInfo member, ICollection arguments, bool isComplete)
		{
			this.isComplete = isComplete;
			ValidateMember(member, arguments);
			this.member = member;
			this.arguments = arguments;
		}

		private void ValidateMember(MemberInfo member, ICollection arguments)
		{
			if (member == null)
			{
				return;
			}
			switch (member.MemberType)
			{
			case MemberTypes.Constructor:
			{
				ConstructorInfo constructorInfo = (ConstructorInfo)member;
				if (arguments == null && constructorInfo.GetParameters().Length != 0)
				{
					throw new ArgumentException("Invalid number of arguments for this constructor");
				}
				if (arguments.Count != constructorInfo.GetParameters().Length)
				{
					throw new ArgumentException("Invalid number of arguments for this constructor");
				}
				break;
			}
			case MemberTypes.Method:
			{
				MethodInfo methodInfo = (MethodInfo)member;
				if (!methodInfo.IsStatic)
				{
					throw new ArgumentException("InstanceDescriptor only describes static (VB.Net: shared) members", "member");
				}
				if (arguments == null && methodInfo.GetParameters().Length != 0)
				{
					throw new ArgumentException("Invalid number of arguments for this method", "arguments");
				}
				if (arguments.Count != methodInfo.GetParameters().Length)
				{
					throw new ArgumentException("Invalid number of arguments for this method");
				}
				break;
			}
			case MemberTypes.Field:
			{
				FieldInfo fieldInfo = (FieldInfo)member;
				if (!fieldInfo.IsStatic)
				{
					throw new ArgumentException("Parameter must be static");
				}
				if (arguments != null && arguments.Count != 0)
				{
					throw new ArgumentException("Field members do not take any arguments");
				}
				break;
			}
			case MemberTypes.Property:
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				if (!propertyInfo.CanRead)
				{
					throw new ArgumentException("Parameter must be readable");
				}
				MethodInfo getMethod = propertyInfo.GetGetMethod();
				if (!getMethod.IsStatic)
				{
					throw new ArgumentException("Parameter must be static");
				}
				break;
			}
			}
		}

		public object Invoke()
		{
			if (member == null)
			{
				return null;
			}
			object[] array;
			if (arguments == null)
			{
				array = new object[0];
			}
			else
			{
				array = new object[arguments.Count];
				arguments.CopyTo(array, 0);
			}
			switch (member.MemberType)
			{
			case MemberTypes.Constructor:
			{
				ConstructorInfo constructorInfo = (ConstructorInfo)member;
				return constructorInfo.Invoke(array);
			}
			case MemberTypes.Method:
			{
				MethodInfo methodInfo = (MethodInfo)member;
				return methodInfo.Invoke(null, array);
			}
			case MemberTypes.Field:
			{
				FieldInfo fieldInfo = (FieldInfo)member;
				return fieldInfo.GetValue(null);
			}
			case MemberTypes.Property:
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				return propertyInfo.GetValue(null, array);
			}
			default:
				return null;
			}
		}
	}
}
