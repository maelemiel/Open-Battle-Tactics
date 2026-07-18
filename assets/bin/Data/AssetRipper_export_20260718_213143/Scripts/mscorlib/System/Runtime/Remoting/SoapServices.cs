using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	public class SoapServices
	{
		private class TypeInfo
		{
			public Hashtable Attributes;

			public Hashtable Elements;
		}

		private static Hashtable _xmlTypes = new Hashtable();

		private static Hashtable _xmlElements = new Hashtable();

		private static Hashtable _soapActions = new Hashtable();

		private static Hashtable _soapActionsMethods = new Hashtable();

		private static Hashtable _typeInfos = new Hashtable();

		public static string XmlNsForClrType
		{
			get
			{
				return "http://schemas.microsoft.com/clr/";
			}
		}

		public static string XmlNsForClrTypeWithAssembly
		{
			get
			{
				return "http://schemas.microsoft.com/clr/assem/";
			}
		}

		public static string XmlNsForClrTypeWithNs
		{
			get
			{
				return "http://schemas.microsoft.com/clr/ns/";
			}
		}

		public static string XmlNsForClrTypeWithNsAndAssembly
		{
			get
			{
				return "http://schemas.microsoft.com/clr/nsassem/";
			}
		}

		private SoapServices()
		{
		}

		public static string CodeXmlNamespaceForClrTypeNamespace(string typeNamespace, string assemblyName)
		{
			if (assemblyName == string.Empty)
			{
				return XmlNsForClrTypeWithNs + typeNamespace;
			}
			if (typeNamespace == string.Empty)
			{
				return EncodeNs(XmlNsForClrTypeWithAssembly + assemblyName);
			}
			return EncodeNs(XmlNsForClrTypeWithNsAndAssembly + typeNamespace + "/" + assemblyName);
		}

		public static bool DecodeXmlNamespaceForClrTypeNamespace(string inNamespace, out string typeNamespace, out string assemblyName)
		{
			if (inNamespace == null)
			{
				throw new ArgumentNullException("inNamespace");
			}
			inNamespace = DecodeNs(inNamespace);
			typeNamespace = null;
			assemblyName = null;
			if (inNamespace.StartsWith(XmlNsForClrTypeWithNsAndAssembly))
			{
				int length = XmlNsForClrTypeWithNsAndAssembly.Length;
				if (length >= inNamespace.Length)
				{
					return false;
				}
				int num = inNamespace.IndexOf('/', length + 1);
				if (num == -1)
				{
					return false;
				}
				typeNamespace = inNamespace.Substring(length, num - length);
				assemblyName = inNamespace.Substring(num + 1);
				return true;
			}
			if (inNamespace.StartsWith(XmlNsForClrTypeWithNs))
			{
				int length2 = XmlNsForClrTypeWithNs.Length;
				typeNamespace = inNamespace.Substring(length2);
				return true;
			}
			if (inNamespace.StartsWith(XmlNsForClrTypeWithAssembly))
			{
				int length3 = XmlNsForClrTypeWithAssembly.Length;
				assemblyName = inNamespace.Substring(length3);
				return true;
			}
			return false;
		}

		public static void GetInteropFieldTypeAndNameFromXmlAttribute(Type containingType, string xmlAttribute, string xmlNamespace, out Type type, out string name)
		{
			TypeInfo typeInfo = (TypeInfo)_typeInfos[containingType];
			Hashtable fields = ((typeInfo == null) ? null : typeInfo.Attributes);
			GetInteropFieldInfo(fields, xmlAttribute, xmlNamespace, out type, out name);
		}

		public static void GetInteropFieldTypeAndNameFromXmlElement(Type containingType, string xmlElement, string xmlNamespace, out Type type, out string name)
		{
			TypeInfo typeInfo = (TypeInfo)_typeInfos[containingType];
			Hashtable fields = ((typeInfo == null) ? null : typeInfo.Elements);
			GetInteropFieldInfo(fields, xmlElement, xmlNamespace, out type, out name);
		}

		private static void GetInteropFieldInfo(Hashtable fields, string xmlName, string xmlNamespace, out Type type, out string name)
		{
			if (fields != null)
			{
				FieldInfo fieldInfo = (FieldInfo)fields[GetNameKey(xmlName, xmlNamespace)];
				if (fieldInfo != null)
				{
					type = fieldInfo.FieldType;
					name = fieldInfo.Name;
					return;
				}
			}
			type = null;
			name = null;
		}

		private static string GetNameKey(string name, string namspace)
		{
			if (namspace == null)
			{
				return name;
			}
			return name + " " + namspace;
		}

		public static Type GetInteropTypeFromXmlElement(string xmlElement, string xmlNamespace)
		{
			lock (_xmlElements.SyncRoot)
			{
				return (Type)_xmlElements[xmlElement + " " + xmlNamespace];
			}
		}

		public static Type GetInteropTypeFromXmlType(string xmlType, string xmlTypeNamespace)
		{
			lock (_xmlTypes.SyncRoot)
			{
				return (Type)_xmlTypes[xmlType + " " + xmlTypeNamespace];
			}
		}

		private static string GetAssemblyName(MethodBase mb)
		{
			if (mb.DeclaringType.Assembly == typeof(object).Assembly)
			{
				return string.Empty;
			}
			return mb.DeclaringType.Assembly.GetName().Name;
		}

		public static string GetSoapActionFromMethodBase(MethodBase mb)
		{
			return InternalGetSoapAction(mb);
		}

		public static bool GetTypeAndMethodNameFromSoapAction(string soapAction, out string typeName, out string methodName)
		{
			lock (_soapActions.SyncRoot)
			{
				MethodBase methodBase = (MethodBase)_soapActionsMethods[soapAction];
				if (methodBase != null)
				{
					typeName = methodBase.DeclaringType.AssemblyQualifiedName;
					methodName = methodBase.Name;
					return true;
				}
			}
			typeName = null;
			methodName = null;
			int num = soapAction.LastIndexOf('#');
			if (num == -1)
			{
				return false;
			}
			methodName = soapAction.Substring(num + 1);
			string typeNamespace;
			string assemblyName;
			if (!DecodeXmlNamespaceForClrTypeNamespace(soapAction.Substring(0, num), out typeNamespace, out assemblyName))
			{
				return false;
			}
			if (assemblyName == null)
			{
				typeName = typeNamespace + ", " + typeof(object).Assembly.GetName().Name;
			}
			else
			{
				typeName = typeNamespace + ", " + assemblyName;
			}
			return true;
		}

		public static bool GetXmlElementForInteropType(Type type, out string xmlElement, out string xmlNamespace)
		{
			SoapTypeAttribute soapTypeAttribute = (SoapTypeAttribute)InternalRemotingServices.GetCachedSoapAttribute(type);
			if (!soapTypeAttribute.IsInteropXmlElement)
			{
				xmlElement = null;
				xmlNamespace = null;
				return false;
			}
			xmlElement = soapTypeAttribute.XmlElementName;
			xmlNamespace = soapTypeAttribute.XmlNamespace;
			return true;
		}

		public static string GetXmlNamespaceForMethodCall(MethodBase mb)
		{
			return CodeXmlNamespaceForClrTypeNamespace(mb.DeclaringType.FullName, GetAssemblyName(mb));
		}

		public static string GetXmlNamespaceForMethodResponse(MethodBase mb)
		{
			return CodeXmlNamespaceForClrTypeNamespace(mb.DeclaringType.FullName, GetAssemblyName(mb));
		}

		public static bool GetXmlTypeForInteropType(Type type, out string xmlType, out string xmlTypeNamespace)
		{
			SoapTypeAttribute soapTypeAttribute = (SoapTypeAttribute)InternalRemotingServices.GetCachedSoapAttribute(type);
			if (!soapTypeAttribute.IsInteropXmlType)
			{
				xmlType = null;
				xmlTypeNamespace = null;
				return false;
			}
			xmlType = soapTypeAttribute.XmlTypeName;
			xmlTypeNamespace = soapTypeAttribute.XmlTypeNamespace;
			return true;
		}

		public static bool IsClrTypeNamespace(string namespaceString)
		{
			return namespaceString.StartsWith(XmlNsForClrType);
		}

		public static bool IsSoapActionValidForMethodBase(string soapAction, MethodBase mb)
		{
			string typeName;
			string methodName;
			GetTypeAndMethodNameFromSoapAction(soapAction, out typeName, out methodName);
			if (methodName != mb.Name)
			{
				return false;
			}
			string assemblyQualifiedName = mb.DeclaringType.AssemblyQualifiedName;
			return typeName == assemblyQualifiedName;
		}

		public static void PreLoad(Assembly assembly)
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				PreLoad(type);
			}
		}

		public static void PreLoad(Type type)
		{
			TypeInfo typeInfo = _typeInfos[type] as TypeInfo;
			if (typeInfo != null)
			{
				return;
			}
			string xmlType;
			string xmlTypeNamespace;
			if (GetXmlTypeForInteropType(type, out xmlType, out xmlTypeNamespace))
			{
				RegisterInteropXmlType(xmlType, xmlTypeNamespace, type);
			}
			if (GetXmlElementForInteropType(type, out xmlType, out xmlTypeNamespace))
			{
				RegisterInteropXmlElement(xmlType, xmlTypeNamespace, type);
			}
			lock (_typeInfos.SyncRoot)
			{
				typeInfo = new TypeInfo();
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo[] array = fields;
				foreach (FieldInfo fieldInfo in array)
				{
					SoapFieldAttribute soapFieldAttribute = (SoapFieldAttribute)InternalRemotingServices.GetCachedSoapAttribute(fieldInfo);
					if (!soapFieldAttribute.IsInteropXmlElement())
					{
						continue;
					}
					string nameKey = GetNameKey(soapFieldAttribute.XmlElementName, soapFieldAttribute.XmlNamespace);
					if (soapFieldAttribute.UseAttribute)
					{
						if (typeInfo.Attributes == null)
						{
							typeInfo.Attributes = new Hashtable();
						}
						typeInfo.Attributes[nameKey] = fieldInfo;
					}
					else
					{
						if (typeInfo.Elements == null)
						{
							typeInfo.Elements = new Hashtable();
						}
						typeInfo.Elements[nameKey] = fieldInfo;
					}
				}
				_typeInfos[type] = typeInfo;
			}
		}

		public static void RegisterInteropXmlElement(string xmlElement, string xmlNamespace, Type type)
		{
			lock (_xmlElements.SyncRoot)
			{
				_xmlElements[xmlElement + " " + xmlNamespace] = type;
			}
		}

		public static void RegisterInteropXmlType(string xmlType, string xmlTypeNamespace, Type type)
		{
			lock (_xmlTypes.SyncRoot)
			{
				_xmlTypes[xmlType + " " + xmlTypeNamespace] = type;
			}
		}

		public static void RegisterSoapActionForMethodBase(MethodBase mb)
		{
			InternalGetSoapAction(mb);
		}

		private static string InternalGetSoapAction(MethodBase mb)
		{
			lock (_soapActions.SyncRoot)
			{
				string text = (string)_soapActions[mb];
				if (text == null)
				{
					SoapMethodAttribute soapMethodAttribute = (SoapMethodAttribute)InternalRemotingServices.GetCachedSoapAttribute(mb);
					text = soapMethodAttribute.SoapAction;
					_soapActions[mb] = text;
					_soapActionsMethods[text] = mb;
				}
				return text;
			}
		}

		public static void RegisterSoapActionForMethodBase(MethodBase mb, string soapAction)
		{
			lock (_soapActions.SyncRoot)
			{
				_soapActions[mb] = soapAction;
				_soapActionsMethods[soapAction] = mb;
			}
		}

		private static string EncodeNs(string ns)
		{
			ns = ns.Replace(",", "%2C");
			ns = ns.Replace(" ", "%20");
			return ns.Replace("=", "%3D");
		}

		private static string DecodeNs(string ns)
		{
			ns = ns.Replace("%2C", ",");
			ns = ns.Replace("%20", " ");
			return ns.Replace("%3D", "=");
		}
	}
}
