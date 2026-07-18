using System;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdEntities : XsdName
	{
		public override XmlTokenizedType TokenizedType
		{
			get
			{
				return XmlTokenizedType.ENTITIES;
			}
		}

		[System.MonoTODO]
		public override XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.Item;
			}
		}

		public override Type ValueType
		{
			get
			{
				return typeof(string[]);
			}
		}

		internal XsdEntities()
		{
		}

		public override object ParseValue(string value, XmlNameTable nt, IXmlNamespaceResolver nsmgr)
		{
			return GetValidatedArray(value, nt);
		}

		internal override ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return new StringArrayValueType(GetValidatedArray(s, nameTable));
		}

		private string[] GetValidatedArray(string value, XmlNameTable nt)
		{
			string[] array = ParseListValue(value, nt);
			for (int i = 0; i < array.Length; i++)
			{
				if (!XmlChar.IsName(array[i]))
				{
					throw new ArgumentException("Invalid entitiy name.");
				}
			}
			return array;
		}
	}
}
