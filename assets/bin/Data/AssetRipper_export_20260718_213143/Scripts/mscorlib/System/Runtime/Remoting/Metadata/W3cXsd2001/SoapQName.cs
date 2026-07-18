using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapQName : ISoapXsd
	{
		private string _name;

		private string _key;

		private string _namespace;

		public string Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public string Namespace
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

		public static string XsdType
		{
			get
			{
				return "QName";
			}
		}

		public SoapQName()
		{
		}

		public SoapQName(string value)
		{
			_name = value;
		}

		public SoapQName(string key, string name)
		{
			_key = key;
			_name = name;
		}

		public SoapQName(string key, string name, string namespaceValue)
		{
			_key = key;
			_name = name;
			_namespace = namespaceValue;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapQName Parse(string value)
		{
			SoapQName soapQName = new SoapQName();
			int num = value.IndexOf(':');
			if (num != -1)
			{
				soapQName.Key = value.Substring(0, num);
				soapQName.Name = value.Substring(num + 1);
			}
			else
			{
				soapQName.Name = value;
			}
			return soapQName;
		}

		public override string ToString()
		{
			if (_key == null || _key == string.Empty)
			{
				return _name;
			}
			return _key + ":" + _name;
		}
	}
}
