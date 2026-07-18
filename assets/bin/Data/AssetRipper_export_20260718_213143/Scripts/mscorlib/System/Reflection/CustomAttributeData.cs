using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public sealed class CustomAttributeData
	{
		private ConstructorInfo ctorInfo;

		private IList<CustomAttributeTypedArgument> ctorArgs;

		private IList<CustomAttributeNamedArgument> namedArgs;

		[ComVisible(true)]
		public ConstructorInfo Constructor
		{
			get
			{
				return ctorInfo;
			}
		}

		[ComVisible(true)]
		public IList<CustomAttributeTypedArgument> ConstructorArguments
		{
			get
			{
				return ctorArgs;
			}
		}

		public IList<CustomAttributeNamedArgument> NamedArguments
		{
			get
			{
				return namedArgs;
			}
		}

		internal CustomAttributeData(ConstructorInfo ctorInfo, object[] ctorArgs, object[] namedArgs)
		{
			this.ctorInfo = ctorInfo;
			this.ctorArgs = Array.AsReadOnly((ctorArgs == null) ? new CustomAttributeTypedArgument[0] : UnboxValues<CustomAttributeTypedArgument>(ctorArgs));
			this.namedArgs = Array.AsReadOnly((namedArgs == null) ? new CustomAttributeNamedArgument[0] : UnboxValues<CustomAttributeNamedArgument>(namedArgs));
		}

		public static IList<CustomAttributeData> GetCustomAttributes(Assembly target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target);
		}

		public static IList<CustomAttributeData> GetCustomAttributes(MemberInfo target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target);
		}

		public static IList<CustomAttributeData> GetCustomAttributes(Module target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target);
		}

		public static IList<CustomAttributeData> GetCustomAttributes(ParameterInfo target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[" + ctorInfo.DeclaringType.FullName + "(");
			for (int i = 0; i < ctorArgs.Count; i++)
			{
				stringBuilder.Append(ctorArgs[i].ToString());
				if (i + 1 < ctorArgs.Count)
				{
					stringBuilder.Append(", ");
				}
			}
			if (namedArgs.Count > 0)
			{
				stringBuilder.Append(", ");
			}
			for (int j = 0; j < namedArgs.Count; j++)
			{
				stringBuilder.Append(namedArgs[j].ToString());
				if (j + 1 < namedArgs.Count)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.AppendFormat(")]");
			return stringBuilder.ToString();
		}

		private static T[] UnboxValues<T>(object[] values)
		{
			T[] array = new T[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = (T)values[i];
			}
			return array;
		}

		public override bool Equals(object obj)
		{
			CustomAttributeData customAttributeData = obj as CustomAttributeData;
			if (customAttributeData == null || customAttributeData.ctorInfo != ctorInfo || customAttributeData.ctorArgs.Count != ctorArgs.Count || customAttributeData.namedArgs.Count != namedArgs.Count)
			{
				return false;
			}
			for (int i = 0; i < ctorArgs.Count; i++)
			{
				if (ctorArgs[i].Equals(customAttributeData.ctorArgs[i]))
				{
					return false;
				}
			}
			for (int j = 0; j < namedArgs.Count; j++)
			{
				bool flag = false;
				for (int k = 0; k < customAttributeData.namedArgs.Count; k++)
				{
					if (namedArgs[j].Equals(customAttributeData.namedArgs[k]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = ctorInfo.GetHashCode() << 16;
			for (int i = 0; i < ctorArgs.Count; i++)
			{
				num += num ^ (7 + ctorArgs[i].GetHashCode() << i * 4);
			}
			for (int j = 0; j < namedArgs.Count; j++)
			{
				num += namedArgs[j].GetHashCode() << 5;
			}
			return num;
		}
	}
}
