using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Serialization.Formatters;
using System.Security;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public sealed class FormatterServices
	{
		private const BindingFlags fieldFlags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private FormatterServices()
		{
		}

		public static object[] GetObjectData(object obj, MemberInfo[] members)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (members == null)
			{
				throw new ArgumentNullException("members");
			}
			int num = members.Length;
			object[] array = new object[num];
			for (int i = 0; i < num; i++)
			{
				MemberInfo memberInfo = members[i];
				if (memberInfo == null)
				{
					throw new ArgumentNullException(string.Format("members[{0}]", i));
				}
				if (memberInfo.MemberType != MemberTypes.Field)
				{
					throw new SerializationException(string.Format("members [{0}] is not a field.", i));
				}
				FieldInfo fieldInfo = memberInfo as FieldInfo;
				array[i] = fieldInfo.GetValue(obj);
			}
			return array;
		}

		public static MemberInfo[] GetSerializableMembers(Type type)
		{
			StreamingContext context = new StreamingContext(StreamingContextStates.All);
			return GetSerializableMembers(type, context);
		}

		public static MemberInfo[] GetSerializableMembers(Type type, StreamingContext context)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			ArrayList arrayList = new ArrayList();
			for (Type type2 = type; type2 != null; type2 = type2.BaseType)
			{
				if (!type2.IsSerializable)
				{
					string message = string.Format("Type {0} in assembly {1} is not marked as serializable.", type2, type2.Assembly.FullName);
					throw new SerializationException(message);
				}
				GetFields(type, type2, arrayList);
			}
			MemberInfo[] array = new MemberInfo[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}

		private static void GetFields(Type reflectedType, Type type, ArrayList fields)
		{
			FieldInfo[] fields2 = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			FieldInfo[] array = fields2;
			foreach (FieldInfo fieldInfo in array)
			{
				if (!fieldInfo.IsNotSerialized)
				{
					MonoField monoField = fieldInfo as MonoField;
					if (monoField != null && reflectedType != type && !monoField.IsPublic)
					{
						string newName = type.Name + "+" + monoField.Name;
						fields.Add(monoField.Clone(newName));
					}
					else
					{
						fields.Add(fieldInfo);
					}
				}
			}
		}

		public static Type GetTypeFromAssembly(Assembly assem, string name)
		{
			if (assem == null)
			{
				throw new ArgumentNullException("assem");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return assem.GetType(name);
		}

		public static object GetUninitializedObject(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type == typeof(string))
			{
				throw new ArgumentException("Uninitialized Strings cannot be created.");
			}
			return ActivationServices.AllocateUninitializedClassInstance(type);
		}

		public static object PopulateObjectMembers(object obj, MemberInfo[] members, object[] data)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (members == null)
			{
				throw new ArgumentNullException("members");
			}
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			int num = members.Length;
			if (num != data.Length)
			{
				throw new ArgumentException("different length in members and data");
			}
			for (int i = 0; i < num; i++)
			{
				MemberInfo memberInfo = members[i];
				if (memberInfo == null)
				{
					throw new ArgumentNullException(string.Format("members[{0}]", i));
				}
				if (memberInfo.MemberType != MemberTypes.Field)
				{
					throw new SerializationException(string.Format("members [{0}] is not a field.", i));
				}
				FieldInfo fieldInfo = memberInfo as FieldInfo;
				fieldInfo.SetValue(obj, data[i]);
			}
			return obj;
		}

		public static void CheckTypeSecurity(Type t, TypeFilterLevel securityLevel)
		{
			if (securityLevel != TypeFilterLevel.Full)
			{
				CheckNotAssignable(typeof(DelegateSerializationHolder), t);
				CheckNotAssignable(typeof(ISponsor), t);
				CheckNotAssignable(typeof(IEnvoyInfo), t);
				CheckNotAssignable(typeof(ObjRef), t);
			}
		}

		private static void CheckNotAssignable(Type basetype, Type type)
		{
			if (basetype.IsAssignableFrom(type))
			{
				string text = string.Concat("Type ", basetype, " and the types derived from it");
				string text2 = text;
				text = string.Concat(text2, " (such as ", type, ") are not permitted to be deserialized at this security level");
				throw new SecurityException(text);
			}
		}

		public static object GetSafeUninitializedObject(Type type)
		{
			return GetUninitializedObject(type);
		}
	}
}
