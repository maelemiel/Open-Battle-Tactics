using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	[AttributeUsage(AttributeTargets.Method)]
	[ComVisible(true)]
	public sealed class SoapMethodAttribute : SoapAttribute
	{
		private string _responseElement;

		private string _responseNamespace;

		private string _returnElement;

		private string _soapAction;

		private bool _useAttribute;

		private string _namespace;

		public string ResponseXmlElementName
		{
			get
			{
				return _responseElement;
			}
			set
			{
				_responseElement = value;
			}
		}

		public string ResponseXmlNamespace
		{
			get
			{
				return _responseNamespace;
			}
			set
			{
				_responseNamespace = value;
			}
		}

		public string ReturnXmlElementName
		{
			get
			{
				return _returnElement;
			}
			set
			{
				_returnElement = value;
			}
		}

		public string SoapAction
		{
			get
			{
				return _soapAction;
			}
			set
			{
				_soapAction = value;
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

		public override string XmlNamespace
		{
			get
			{
				return _namespace;
			}
			set
			{
				_namespace = value;
			}
		}

		internal override void SetReflectionObject(object reflectionObject)
		{
			MethodBase methodBase = (MethodBase)reflectionObject;
			if (_responseElement == null)
			{
				_responseElement = methodBase.Name + "Response";
			}
			if (_responseNamespace == null)
			{
				_responseNamespace = SoapServices.GetXmlNamespaceForMethodResponse(methodBase);
			}
			if (_returnElement == null)
			{
				_returnElement = "return";
			}
			if (_soapAction == null)
			{
				_soapAction = SoapServices.GetXmlNamespaceForMethodCall(methodBase) + "#" + methodBase.Name;
			}
			if (_namespace == null)
			{
				_namespace = SoapServices.GetXmlNamespaceForMethodCall(methodBase);
			}
		}
	}
}
