using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SoapFieldAttribute : SoapAttribute
	{
		private int _order;

		private string _elementName;

		private bool _isElement;

		public int Order
		{
			get
			{
				return _order;
			}
			set
			{
				_order = value;
			}
		}

		public string XmlElementName
		{
			get
			{
				return _elementName;
			}
			set
			{
				_isElement = value != null;
				_elementName = value;
			}
		}

		public bool IsInteropXmlElement()
		{
			return _isElement;
		}

		internal override void SetReflectionObject(object reflectionObject)
		{
			FieldInfo fieldInfo = (FieldInfo)reflectionObject;
			if (_elementName == null)
			{
				_elementName = fieldInfo.Name;
			}
		}
	}
}
