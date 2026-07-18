using System.Collections;
using System.Reflection;

namespace System.Xml.Serialization
{
	internal class ReflectionHelper
	{
		private Hashtable _clrTypes = new Hashtable();

		private Hashtable _schemaTypes = new Hashtable();

		private static readonly ParameterModifier[] empty_modifiers = new ParameterModifier[0];

		public void RegisterSchemaType(XmlTypeMapping map, string xmlType, string ns)
		{
			string key = xmlType + "/" + ns;
			if (!_schemaTypes.ContainsKey(key))
			{
				_schemaTypes.Add(key, map);
			}
		}

		public XmlTypeMapping GetRegisteredSchemaType(string xmlType, string ns)
		{
			string key = xmlType + "/" + ns;
			return _schemaTypes[key] as XmlTypeMapping;
		}

		public void RegisterClrType(XmlTypeMapping map, Type type, string ns)
		{
			if (type == typeof(object))
			{
				ns = string.Empty;
			}
			string key = type.FullName + "/" + ns;
			if (!_clrTypes.ContainsKey(key))
			{
				_clrTypes.Add(key, map);
			}
		}

		public XmlTypeMapping GetRegisteredClrType(Type type, string ns)
		{
			if (type == typeof(object))
			{
				ns = string.Empty;
			}
			string key = type.FullName + "/" + ns;
			return _clrTypes[key] as XmlTypeMapping;
		}

		public Exception CreateError(XmlTypeMapping map, string message)
		{
			return new InvalidOperationException("There was an error reflecting '" + map.TypeFullName + "': " + message);
		}

		public static void CheckSerializableType(Type type, bool allowPrivateConstructors)
		{
			if (type.IsArray)
			{
				return;
			}
			if (!allowPrivateConstructors && type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, empty_modifiers) == null && !type.IsAbstract && !type.IsValueType)
			{
				throw new InvalidOperationException(type.FullName + " cannot be serialized because it does not have a default public constructor");
			}
			if (type.IsInterface && !TypeTranslator.GetTypeData(type).IsListType)
			{
				throw new InvalidOperationException(type.FullName + " cannot be serialized because it is an interface");
			}
			Type type2 = type;
			Type type3 = null;
			do
			{
				if (!type2.IsPublic && !type2.IsNestedPublic)
				{
					throw new InvalidOperationException(type.FullName + " is inaccessible due to its protection level. Only public types can be processed");
				}
				type3 = type2;
				type2 = type2.DeclaringType;
			}
			while (type2 != null && type2 != type3);
		}

		public static string BuildMapKey(Type type)
		{
			return type.FullName + "::";
		}

		public static string BuildMapKey(MethodInfo method, string tag)
		{
			string text = method.DeclaringType.FullName + ":" + method.ReturnType.FullName + " " + method.Name + "(";
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				if (i > 0)
				{
					text += ", ";
				}
				text += parameters[i].ParameterType.FullName;
			}
			text += ")";
			if (tag != null)
			{
				text = text + ":" + tag;
			}
			return text;
		}
	}
}
