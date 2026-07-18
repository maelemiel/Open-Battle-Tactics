using System.Collections;
using System.Xml.XPath;

namespace System.Xml.Xsl
{
	public class XsltArgumentList
	{
		internal Hashtable extensionObjects;

		internal Hashtable parameters;

		public event XsltMessageEncounteredEventHandler XsltMessageEncountered;

		public XsltArgumentList()
		{
			extensionObjects = new Hashtable();
			parameters = new Hashtable();
		}

		public void AddExtensionObject(string namespaceUri, object extension)
		{
			if (namespaceUri == null)
			{
				throw new ArgumentException("The namespaceUri is a null reference.");
			}
			if (namespaceUri == "http://www.w3.org/1999/XSL/Transform")
			{
				throw new ArgumentException("The namespaceUri is http://www.w3.org/1999/XSL/Transform.");
			}
			if (extensionObjects.Contains(namespaceUri))
			{
				throw new ArgumentException("The namespaceUri already has an extension object associated with it.");
			}
			extensionObjects[namespaceUri] = extension;
		}

		public void AddParam(string name, string namespaceUri, object parameter)
		{
			if (namespaceUri == null)
			{
				throw new ArgumentException("The namespaceUri is a null reference.");
			}
			if (namespaceUri == "http://www.w3.org/1999/XSL/Transform")
			{
				throw new ArgumentException("The namespaceUri is http://www.w3.org/1999/XSL/Transform.");
			}
			if (name == null)
			{
				throw new ArgumentException("The parameter name is a null reference.");
			}
			XmlQualifiedName key = new XmlQualifiedName(name, namespaceUri);
			if (parameters.Contains(key))
			{
				throw new ArgumentException("The namespaceUri already has a parameter associated with it.");
			}
			parameter = ValidateParam(parameter);
			parameters[key] = parameter;
		}

		public void Clear()
		{
			extensionObjects.Clear();
			parameters.Clear();
		}

		public object GetExtensionObject(string namespaceUri)
		{
			return extensionObjects[namespaceUri];
		}

		public object GetParam(string name, string namespaceUri)
		{
			if (name == null)
			{
				throw new ArgumentException("The parameter name is a null reference.");
			}
			XmlQualifiedName key = new XmlQualifiedName(name, namespaceUri);
			return parameters[key];
		}

		public object RemoveExtensionObject(string namespaceUri)
		{
			object extensionObject = GetExtensionObject(namespaceUri);
			extensionObjects.Remove(namespaceUri);
			return extensionObject;
		}

		public object RemoveParam(string name, string namespaceUri)
		{
			XmlQualifiedName key = new XmlQualifiedName(name, namespaceUri);
			object param = GetParam(name, namespaceUri);
			parameters.Remove(key);
			return param;
		}

		private object ValidateParam(object parameter)
		{
			if (parameter is string)
			{
				return parameter;
			}
			if (parameter is bool)
			{
				return parameter;
			}
			if (parameter is double)
			{
				return parameter;
			}
			if (parameter is XPathNavigator)
			{
				return parameter;
			}
			if (parameter is XPathNodeIterator)
			{
				return parameter;
			}
			if (parameter is short)
			{
				return (double)(short)parameter;
			}
			if (parameter is ushort)
			{
				return (double)(int)(ushort)parameter;
			}
			if (parameter is int)
			{
				return (double)(int)parameter;
			}
			if (parameter is long)
			{
				return (double)(long)parameter;
			}
			if (parameter is ulong)
			{
				return (double)(ulong)parameter;
			}
			if (parameter is float)
			{
				return (double)(float)parameter;
			}
			if (parameter is decimal)
			{
				return (double)(decimal)parameter;
			}
			return parameter.ToString();
		}
	}
}
