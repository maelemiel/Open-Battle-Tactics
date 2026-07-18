using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Mono.Xml
{
	internal class DTDAttributeDefinition : DTDNode
	{
		private string name;

		private XmlSchemaDatatype datatype;

		private ArrayList enumeratedLiterals;

		private string unresolvedDefault;

		private ArrayList enumeratedNotations;

		private DTDAttributeOccurenceType occurenceType;

		private string resolvedDefaultValue;

		private string resolvedNormalizedDefaultValue;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public XmlSchemaDatatype Datatype
		{
			get
			{
				return datatype;
			}
			set
			{
				datatype = value;
			}
		}

		public DTDAttributeOccurenceType OccurenceType
		{
			get
			{
				return occurenceType;
			}
			set
			{
				occurenceType = value;
			}
		}

		public ArrayList EnumeratedAttributeDeclaration
		{
			get
			{
				if (enumeratedLiterals == null)
				{
					enumeratedLiterals = new ArrayList();
				}
				return enumeratedLiterals;
			}
		}

		public ArrayList EnumeratedNotations
		{
			get
			{
				if (enumeratedNotations == null)
				{
					enumeratedNotations = new ArrayList();
				}
				return enumeratedNotations;
			}
		}

		public string DefaultValue
		{
			get
			{
				if (resolvedDefaultValue == null)
				{
					resolvedDefaultValue = ComputeDefaultValue();
				}
				return resolvedDefaultValue;
			}
		}

		public string NormalizedDefaultValue
		{
			get
			{
				if (resolvedNormalizedDefaultValue == null)
				{
					string s = ComputeDefaultValue();
					try
					{
						object obj = Datatype.ParseValue(s, null, null);
						resolvedNormalizedDefaultValue = ((obj is string[]) ? string.Join(" ", (string[])obj) : ((!(obj is IFormattable)) ? obj.ToString() : ((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture)));
					}
					catch (Exception)
					{
						resolvedNormalizedDefaultValue = Datatype.Normalize(s);
					}
				}
				return resolvedNormalizedDefaultValue;
			}
		}

		public string UnresolvedDefaultValue
		{
			get
			{
				return unresolvedDefault;
			}
			set
			{
				unresolvedDefault = value;
			}
		}

		public char QuoteChar
		{
			get
			{
				return (UnresolvedDefaultValue.Length <= 0) ? '"' : UnresolvedDefaultValue[0];
			}
		}

		internal DTDAttributeDefinition(DTDObjectModel root)
		{
			SetRoot(root);
		}

		internal string ComputeDefaultValue()
		{
			if (UnresolvedDefaultValue == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int startIndex = 0;
			int num = 0;
			string unresolvedDefaultValue = UnresolvedDefaultValue;
			while ((num = unresolvedDefaultValue.IndexOf('&', startIndex)) >= 0)
			{
				int num2 = unresolvedDefaultValue.IndexOf(';', num);
				if (unresolvedDefaultValue[num + 1] == '#')
				{
					char c = unresolvedDefaultValue[num + 2];
					NumberStyles numberStyles = NumberStyles.Integer;
					string s;
					if (c == 'x' || c == 'X')
					{
						s = unresolvedDefaultValue.Substring(num + 3, num2 - num - 3);
						numberStyles |= NumberStyles.HexNumber;
					}
					else
					{
						s = unresolvedDefaultValue.Substring(num + 2, num2 - num - 2);
					}
					stringBuilder.Append((char)int.Parse(s, numberStyles, CultureInfo.InvariantCulture));
				}
				else
				{
					stringBuilder.Append(unresolvedDefaultValue.Substring(startIndex, num - 1));
					string text = unresolvedDefaultValue.Substring(num + 1, num2 - 2);
					int predefinedEntity = XmlChar.GetPredefinedEntity(text);
					if (predefinedEntity >= 0)
					{
						stringBuilder.Append(predefinedEntity);
					}
					else
					{
						stringBuilder.Append(base.Root.ResolveEntity(text));
					}
				}
				startIndex = num2 + 1;
			}
			stringBuilder.Append(unresolvedDefaultValue.Substring(startIndex));
			string result = stringBuilder.ToString(1, stringBuilder.Length - 2);
			stringBuilder.Length = 0;
			return result;
		}
	}
}
