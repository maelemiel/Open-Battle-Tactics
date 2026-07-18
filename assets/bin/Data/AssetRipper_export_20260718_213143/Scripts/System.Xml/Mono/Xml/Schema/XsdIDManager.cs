using System.Collections;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml.Schema
{
	internal class XsdIDManager
	{
		private Hashtable idList = new Hashtable();

		private ArrayList missingIDReferences;

		private string thisElementId;

		private ArrayList MissingIDReferences
		{
			get
			{
				if (missingIDReferences == null)
				{
					missingIDReferences = new ArrayList();
				}
				return missingIDReferences;
			}
		}

		public void OnStartElement()
		{
			thisElementId = null;
		}

		public string AssessEachAttributeIdentityConstraint(XmlSchemaDatatype dt, object parsedValue, string elementName)
		{
			string text = parsedValue as string;
			switch (dt.TokenizedType)
			{
			case XmlTokenizedType.ID:
				if (thisElementId != null)
				{
					return "ID type attribute was already assigned in the containing element.";
				}
				thisElementId = text;
				if (idList.ContainsKey(text))
				{
					return "Duplicate ID value was found.";
				}
				idList.Add(text, elementName);
				if (MissingIDReferences.Contains(text))
				{
					MissingIDReferences.Remove(text);
				}
				break;
			case XmlTokenizedType.IDREF:
				if (!idList.Contains(text))
				{
					MissingIDReferences.Add(text);
				}
				break;
			case XmlTokenizedType.IDREFS:
			{
				string[] array = (string[])parsedValue;
				foreach (string text2 in array)
				{
					if (!idList.Contains(text2))
					{
						MissingIDReferences.Add(text2);
					}
				}
				break;
			}
			}
			return null;
		}

		public bool HasMissingIDReferences()
		{
			return missingIDReferences != null && missingIDReferences.Count > 0;
		}

		public string GetMissingIDString()
		{
			return string.Join(" ", MissingIDReferences.ToArray(typeof(string)) as string[]);
		}
	}
}
