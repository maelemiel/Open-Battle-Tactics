using System.Collections;
using System.Xml;

internal class XmlHelper
{
	private static Hashtable localSchemaNameCache = new Hashtable();

	private static Hashtable localXmlNameCache = new Hashtable();

	internal static string Decode(string xmlName)
	{
		string text = (string)localSchemaNameCache[xmlName];
		if (text == null)
		{
			text = XmlConvert.DecodeName(xmlName);
			localSchemaNameCache[xmlName] = text;
		}
		return text;
	}

	internal static string Encode(string schemaName)
	{
		string text = (string)localXmlNameCache[schemaName];
		if (text == null)
		{
			text = XmlConvert.EncodeLocalName(schemaName);
			localXmlNameCache[schemaName] = text;
		}
		return text;
	}

	internal static void ClearCache()
	{
		localSchemaNameCache.Clear();
		localXmlNameCache.Clear();
	}
}
