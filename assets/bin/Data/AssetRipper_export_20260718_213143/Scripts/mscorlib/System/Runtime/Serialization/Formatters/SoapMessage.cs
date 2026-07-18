using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Serialization.Formatters
{
	[Serializable]
	[ComVisible(true)]
	public class SoapMessage : ISoapMessage
	{
		private Header[] headers;

		private string methodName;

		private string[] paramNames;

		private Type[] paramTypes;

		private object[] paramValues;

		private string xmlNameSpace;

		public Header[] Headers
		{
			get
			{
				return headers;
			}
			set
			{
				headers = value;
			}
		}

		public string MethodName
		{
			get
			{
				return methodName;
			}
			set
			{
				methodName = value;
			}
		}

		public string[] ParamNames
		{
			get
			{
				return paramNames;
			}
			set
			{
				paramNames = value;
			}
		}

		public Type[] ParamTypes
		{
			get
			{
				return paramTypes;
			}
			set
			{
				paramTypes = value;
			}
		}

		public object[] ParamValues
		{
			get
			{
				return paramValues;
			}
			set
			{
				paramValues = value;
			}
		}

		public string XmlNameSpace
		{
			get
			{
				return xmlNameSpace;
			}
			set
			{
				xmlNameSpace = value;
			}
		}
	}
}
