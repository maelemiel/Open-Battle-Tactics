using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
	[ComVisible(true)]
	public sealed class SoapTypeAttribute : SoapAttribute
	{
		private SoapOption _soapOption;

		private bool _useAttribute;

		private string _xmlElementName;

		private XmlFieldOrderOption _xmlFieldOrder;

		private string _xmlNamespace;

		private string _xmlTypeName;

		private string _xmlTypeNamespace;

		private bool _isType;

		private bool _isElement;

		public SoapOption SoapOptions
		{
			get
			{
				return _soapOption;
			}
			set
			{
				_soapOption = value;
			}
		}

		public override bool UseAttribute
		{
			get
			{
				return _useAttribute;
			}
			set
			{
				_useAttribute = value;
			}
		}

		public string XmlElementName
		{
			get
			{
				return _xmlElementName;
			}
			set
			{
				_isElement = value != null;
				_xmlElementName = value;
			}
		}

		public XmlFieldOrderOption XmlFieldOrder
		{
			get
			{
				return _xmlFieldOrder;
			}
			set
			{
				_xmlFieldOrder = value;
			}
		}

		public override string XmlNamespace
		{
			get
			{
				return _xmlNamespace;
			}
			set
			{
				_isElement = value != null;
				_xmlNamespace = value;
			}
		}

		public string XmlTypeName
		{
			get
			{
				return _xmlTypeName;
			}
			set
			{
				_isType = value != null;
				_xmlTypeName = value;
			}
		}

		public string XmlTypeNamespace
		{
			get
			{
				return _xmlTypeNamespace;
			}
			set
			{
				_isType = value != null;
				_xmlTypeNamespace = value;
			}
		}

		internal bool IsInteropXmlElement
		{
			get
			{
				return _isElement;
			}
		}

		internal bool IsInteropXmlType
		{
			get
			{
				return _isType;
			}
		}

		internal override void SetReflectionObject(object reflectionObject)
		{
			Type type = (Type)reflectionObject;
			if (_xmlElementName == null)
			{
				_xmlElementName = type.Name;
			}
			if (_xmlTypeName == null)
			{
				_xmlTypeName = type.Name;
			}
			if (_xmlTypeNamespace == null)
			{
				_xmlTypeNamespace = SoapServices.CodeXmlNamespaceForClrTypeNamespace(assemblyName: (type.Assembly != typeof(object).Assembly) ? type.Assembly.GetName().Name : string.Empty, typeNamespace: type.Namespace);
			}
			if (_xmlNamespace == null)
			{
				_xmlNamespace = _xmlTypeNamespace;
			}
		}
	}
}
