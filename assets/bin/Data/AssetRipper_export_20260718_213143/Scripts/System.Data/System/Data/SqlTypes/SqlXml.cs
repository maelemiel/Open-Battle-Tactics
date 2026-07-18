using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	[XmlSchemaProvider("GetXsdType")]
	public sealed class SqlXml : INullable, IXmlSerializable
	{
		private bool notNull;

		private string xmlValue;

		public bool IsNull
		{
			get
			{
				return !notNull;
			}
		}

		public static SqlXml Null
		{
			get
			{
				return new SqlXml();
			}
		}

		public string Value
		{
			get
			{
				if (notNull)
				{
					return xmlValue;
				}
				throw new SqlNullValueException();
			}
		}

		public SqlXml()
		{
			notNull = false;
			xmlValue = null;
		}

		public SqlXml(Stream value)
		{
			if (value == null)
			{
				notNull = false;
				xmlValue = null;
				return;
			}
			int num = (int)value.Length;
			if (num < 1)
			{
				xmlValue = string.Empty;
			}
			else
			{
				int num2 = 8192;
				StringBuilder stringBuilder = new StringBuilder(num);
				value.Position = 0L;
				byte[] array = null;
				if (num < num2)
				{
					num2 = num;
				}
				array = new byte[num2];
				while (num > 0)
				{
					int num3 = value.Read(array, 0, num2);
					stringBuilder.Append(Encoding.Unicode.GetString(array, 0, num3));
					if (num3 == 0)
					{
						break;
					}
					num -= num3;
				}
				xmlValue = stringBuilder.ToString();
			}
			notNull = true;
		}

		public SqlXml(XmlReader value)
		{
			if (value == null)
			{
				notNull = false;
				xmlValue = null;
				return;
			}
			if (value.Read())
			{
				value.MoveToContent();
				xmlValue = value.ReadOuterXml();
			}
			else
			{
				xmlValue = string.Empty;
			}
			notNull = true;
		}

		[System.MonoTODO]
		XmlSchema IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		void IXmlSerializable.ReadXml(XmlReader r)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			throw new NotImplementedException();
		}

		public static XmlQualifiedName GetXsdType(XmlSchemaSet schemaSet)
		{
			return new XmlQualifiedName("anyType", "http://www.w3.org/2001/XMLSchema");
		}

		public XmlReader CreateReader()
		{
			if (notNull)
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
				return XmlReader.Create(new StringReader(xmlValue), xmlReaderSettings);
			}
			throw new SqlNullValueException();
		}
	}
}
