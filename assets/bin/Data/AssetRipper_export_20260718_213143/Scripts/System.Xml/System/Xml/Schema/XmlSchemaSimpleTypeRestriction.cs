using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleTypeRestriction : XmlSchemaSimpleTypeContent
	{
		private const string xmlname = "restriction";

		private XmlSchemaSimpleType baseType;

		private XmlQualifiedName baseTypeName;

		private XmlSchemaObjectCollection facets;

		private string[] enumarationFacetValues;

		private string[] patternFacetValues;

		private Regex[] rexPatterns;

		private decimal lengthFacet;

		private decimal maxLengthFacet;

		private decimal minLengthFacet;

		private decimal fractionDigitsFacet;

		private decimal totalDigitsFacet;

		private object maxInclusiveFacet;

		private object maxExclusiveFacet;

		private object minInclusiveFacet;

		private object minExclusiveFacet;

		private XmlSchemaFacet.Facet fixedFacets;

		private static NumberStyles lengthStyle = NumberStyles.Integer;

		private static readonly XmlSchemaFacet.Facet listFacets = XmlSchemaFacet.Facet.length | XmlSchemaFacet.Facet.minLength | XmlSchemaFacet.Facet.maxLength | XmlSchemaFacet.Facet.pattern | XmlSchemaFacet.Facet.enumeration | XmlSchemaFacet.Facet.whiteSpace;

		[XmlAttribute("base")]
		public XmlQualifiedName BaseTypeName
		{
			get
			{
				return baseTypeName;
			}
			set
			{
				baseTypeName = value;
			}
		}

		[XmlElement("simpleType", Type = typeof(XmlSchemaSimpleType))]
		public XmlSchemaSimpleType BaseType
		{
			get
			{
				return baseType;
			}
			set
			{
				baseType = value;
			}
		}

		[XmlElement("length", typeof(XmlSchemaLengthFacet))]
		[XmlElement("maxLength", typeof(XmlSchemaMaxLengthFacet))]
		[XmlElement("pattern", typeof(XmlSchemaPatternFacet))]
		[XmlElement("whiteSpace", typeof(XmlSchemaWhiteSpaceFacet))]
		[XmlElement("minExclusive", typeof(XmlSchemaMinExclusiveFacet))]
		[XmlElement("minInclusive", typeof(XmlSchemaMinInclusiveFacet))]
		[XmlElement("maxExclusive", typeof(XmlSchemaMaxExclusiveFacet))]
		[XmlElement("enumeration", typeof(XmlSchemaEnumerationFacet))]
		[XmlElement("maxInclusive", typeof(XmlSchemaMaxInclusiveFacet))]
		[XmlElement("totalDigits", typeof(XmlSchemaTotalDigitsFacet))]
		[XmlElement("fractionDigits", typeof(XmlSchemaFractionDigitsFacet))]
		[XmlElement("minLength", typeof(XmlSchemaMinLengthFacet))]
		public XmlSchemaObjectCollection Facets
		{
			get
			{
				return facets;
			}
		}

		public XmlSchemaSimpleTypeRestriction()
		{
			baseTypeName = XmlQualifiedName.Empty;
			facets = new XmlSchemaObjectCollection();
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			if (BaseType != null)
			{
				BaseType.SetParent(this);
			}
			foreach (XmlSchemaObject facet in Facets)
			{
				facet.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			errorCount = 0;
			if (baseType != null && !BaseTypeName.IsEmpty)
			{
				error(h, "both base and simpletype can't be set");
			}
			if (baseType == null && BaseTypeName.IsEmpty)
			{
				error(h, "one of basetype or simpletype must be present");
			}
			if (baseType != null)
			{
				errorCount += baseType.Compile(h, schema);
			}
			if (!XmlSchemaUtil.CheckQName(BaseTypeName))
			{
				error(h, "BaseTypeName must be a XmlQualifiedName");
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			for (int i = 0; i < Facets.Count; i++)
			{
				if (!(Facets[i] is XmlSchemaFacet))
				{
					error(h, "Only XmlSchemaFacet objects are allowed for Facets property");
				}
			}
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		private bool IsAllowedFacet(XmlSchemaFacet xsf)
		{
			XsdAnySimpleType xsdAnySimpleType = base.ActualBaseSchemaType as XsdAnySimpleType;
			if (xsdAnySimpleType != null)
			{
				return xsdAnySimpleType.AllowsFacet(xsf);
			}
			XmlSchemaSimpleTypeContent content = ((XmlSchemaSimpleType)base.ActualBaseSchemaType).Content;
			if (content != null)
			{
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = content as XmlSchemaSimpleTypeRestriction;
				if (xmlSchemaSimpleTypeRestriction != null && xmlSchemaSimpleTypeRestriction != this)
				{
					return xmlSchemaSimpleTypeRestriction.IsAllowedFacet(xsf);
				}
				XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = content as XmlSchemaSimpleTypeList;
				if (xmlSchemaSimpleTypeList != null)
				{
					return (xsf.ThisFacet & listFacets) != 0;
				}
				XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = content as XmlSchemaSimpleTypeUnion;
				if (xmlSchemaSimpleTypeUnion != null)
				{
					return xsf is XmlSchemaPatternFacet || xsf is XmlSchemaEnumerationFacet;
				}
			}
			return false;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			ValidateActualType(h, schema);
			lengthFacet = (maxLengthFacet = (minLengthFacet = (fractionDigitsFacet = (totalDigitsFacet = -1m))));
			XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = null;
			if (base.ActualBaseSchemaType is XmlSchemaSimpleType)
			{
				XmlSchemaSimpleTypeContent content = ((XmlSchemaSimpleType)base.ActualBaseSchemaType).Content;
				xmlSchemaSimpleTypeRestriction = content as XmlSchemaSimpleTypeRestriction;
			}
			if (xmlSchemaSimpleTypeRestriction != null)
			{
				fixedFacets = xmlSchemaSimpleTypeRestriction.fixedFacets;
				lengthFacet = xmlSchemaSimpleTypeRestriction.lengthFacet;
				maxLengthFacet = xmlSchemaSimpleTypeRestriction.maxLengthFacet;
				minLengthFacet = xmlSchemaSimpleTypeRestriction.minLengthFacet;
				fractionDigitsFacet = xmlSchemaSimpleTypeRestriction.fractionDigitsFacet;
				totalDigitsFacet = xmlSchemaSimpleTypeRestriction.totalDigitsFacet;
				maxInclusiveFacet = xmlSchemaSimpleTypeRestriction.maxInclusiveFacet;
				maxExclusiveFacet = xmlSchemaSimpleTypeRestriction.maxExclusiveFacet;
				minInclusiveFacet = xmlSchemaSimpleTypeRestriction.minInclusiveFacet;
				minExclusiveFacet = xmlSchemaSimpleTypeRestriction.minExclusiveFacet;
			}
			enumarationFacetValues = (patternFacetValues = null);
			rexPatterns = null;
			XmlSchemaFacet.Facet facet = XmlSchemaFacet.Facet.None;
			ArrayList arrayList = null;
			ArrayList arrayList2 = null;
			for (int i = 0; i < facets.Count; i++)
			{
				XmlSchemaFacet xmlSchemaFacet = facets[i] as XmlSchemaFacet;
				if (xmlSchemaFacet == null)
				{
					continue;
				}
				if (!IsAllowedFacet(xmlSchemaFacet))
				{
					xmlSchemaFacet.error(h, string.Concat(xmlSchemaFacet.ThisFacet, " is not a valid facet for this type"));
					continue;
				}
				XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = facets[i] as XmlSchemaEnumerationFacet;
				if (xmlSchemaEnumerationFacet != null)
				{
					if (arrayList == null)
					{
						arrayList = new ArrayList();
					}
					arrayList.Add(xmlSchemaEnumerationFacet.Value);
					continue;
				}
				XmlSchemaPatternFacet xmlSchemaPatternFacet = facets[i] as XmlSchemaPatternFacet;
				if (xmlSchemaPatternFacet != null)
				{
					if (arrayList2 == null)
					{
						arrayList2 = new ArrayList();
					}
					arrayList2.Add(xmlSchemaPatternFacet.Value);
					continue;
				}
				if ((facet & xmlSchemaFacet.ThisFacet) != XmlSchemaFacet.Facet.None)
				{
					xmlSchemaFacet.error(h, string.Concat("This is a duplicate '", xmlSchemaFacet.ThisFacet, "' facet."));
					continue;
				}
				facet |= xmlSchemaFacet.ThisFacet;
				if (xmlSchemaFacet is XmlSchemaLengthFacet)
				{
					checkLengthFacet((XmlSchemaLengthFacet)xmlSchemaFacet, facet, h);
				}
				else if (xmlSchemaFacet is XmlSchemaMaxLengthFacet)
				{
					checkMaxLengthFacet((XmlSchemaMaxLengthFacet)xmlSchemaFacet, facet, h);
				}
				else if (xmlSchemaFacet is XmlSchemaMinLengthFacet)
				{
					checkMinLengthFacet((XmlSchemaMinLengthFacet)xmlSchemaFacet, facet, h);
				}
				else if (xmlSchemaFacet is XmlSchemaMinInclusiveFacet)
				{
					checkMinMaxFacet((XmlSchemaMinInclusiveFacet)xmlSchemaFacet, ref minInclusiveFacet, h);
				}
				else if (xmlSchemaFacet is XmlSchemaMaxInclusiveFacet)
				{
					checkMinMaxFacet((XmlSchemaMaxInclusiveFacet)xmlSchemaFacet, ref maxInclusiveFacet, h);
				}
				else if (xmlSchemaFacet is XmlSchemaMinExclusiveFacet)
				{
					checkMinMaxFacet((XmlSchemaMinExclusiveFacet)xmlSchemaFacet, ref minExclusiveFacet, h);
				}
				else if (xmlSchemaFacet is XmlSchemaMaxExclusiveFacet)
				{
					checkMinMaxFacet((XmlSchemaMaxExclusiveFacet)xmlSchemaFacet, ref maxExclusiveFacet, h);
				}
				else if (xmlSchemaFacet is XmlSchemaFractionDigitsFacet)
				{
					checkFractionDigitsFacet((XmlSchemaFractionDigitsFacet)xmlSchemaFacet, h);
				}
				else if (xmlSchemaFacet is XmlSchemaTotalDigitsFacet)
				{
					checkTotalDigitsFacet((XmlSchemaTotalDigitsFacet)xmlSchemaFacet, h);
				}
				if (xmlSchemaFacet.IsFixed)
				{
					fixedFacets |= xmlSchemaFacet.ThisFacet;
				}
			}
			if (arrayList != null)
			{
				enumarationFacetValues = arrayList.ToArray(typeof(string)) as string[];
			}
			if (arrayList2 != null)
			{
				patternFacetValues = arrayList2.ToArray(typeof(string)) as string[];
				rexPatterns = new Regex[arrayList2.Count];
				for (int j = 0; j < patternFacetValues.Length; j++)
				{
					try
					{
						string text = patternFacetValues[j];
						StringBuilder stringBuilder = null;
						int num = 0;
						for (int k = 0; k < text.Length; k++)
						{
							if (text[k] != '\\' || text.Length <= j + 1)
							{
								continue;
							}
							string text2 = null;
							switch (text[k + 1])
							{
							case 'i':
								text2 = "[\\p{L}_]";
								break;
							case 'I':
								text2 = "[^\\p{L}_]";
								break;
							case 'c':
								text2 = "[\\p{L}\\p{N}_\\.\\-:]";
								break;
							case 'C':
								text2 = "[^\\p{L}\\p{N}_\\.\\-:]";
								break;
							}
							if (text2 != null)
							{
								if (stringBuilder == null)
								{
									stringBuilder = new StringBuilder();
								}
								stringBuilder.Append(text, num, k - num);
								stringBuilder.Append(text2);
								num = k + 2;
							}
						}
						if (stringBuilder != null)
						{
							stringBuilder.Append(text, num, text.Length - num);
							text = stringBuilder.ToString();
						}
						Regex regex = new Regex("^" + text + "$");
						rexPatterns[j] = regex;
					}
					catch (Exception innerException)
					{
						XmlSchemaObject.error(h, "Invalid regular expression pattern was specified.", innerException);
					}
				}
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal void ValidateActualType(ValidationEventHandler h, XmlSchema schema)
		{
			GetActualType(h, schema, true);
		}

		internal object GetActualType(ValidationEventHandler h, XmlSchema schema, bool validate)
		{
			object obj = null;
			XmlSchemaSimpleType xmlSchemaSimpleType = baseType;
			if (xmlSchemaSimpleType == null)
			{
				xmlSchemaSimpleType = schema.FindSchemaType(baseTypeName) as XmlSchemaSimpleType;
			}
			if (xmlSchemaSimpleType != null)
			{
				if (validate)
				{
					errorCount += xmlSchemaSimpleType.Validate(h, schema);
				}
				obj = xmlSchemaSimpleType;
			}
			else if (baseTypeName == XmlSchemaComplexType.AnyTypeName)
			{
				obj = XmlSchemaSimpleType.AnySimpleType;
			}
			else if (baseTypeName.Namespace == "http://www.w3.org/2001/XMLSchema" || baseTypeName.Namespace == "http://www.w3.org/2003/11/xpath-datatypes")
			{
				obj = XmlSchemaDatatype.FromName(baseTypeName);
				if (obj == null && validate)
				{
					error(h, "Invalid schema type name was specified: " + baseTypeName);
				}
			}
			else if (!schema.IsNamespaceAbsent(baseTypeName.Namespace) && validate)
			{
				error(h, string.Concat("Referenced base schema type ", baseTypeName, " was not found in the corresponding schema."));
			}
			return obj;
		}

		private void checkTotalDigitsFacet(XmlSchemaTotalDigitsFacet totf, ValidationEventHandler h)
		{
			if (totf == null)
			{
				return;
			}
			try
			{
				decimal num = decimal.Parse(totf.Value.Trim(), lengthStyle, CultureInfo.InvariantCulture);
				if (num <= 0m)
				{
					totf.error(h, string.Format(CultureInfo.InvariantCulture, "The value '{0}' is an invalid totalDigits value", num));
				}
				if (totalDigitsFacet > 0m && num > totalDigitsFacet)
				{
					totf.error(h, string.Format(CultureInfo.InvariantCulture, "The value '{0}' is not a valid restriction of the base totalDigits facet '{1}'", num, totalDigitsFacet));
				}
				totalDigitsFacet = num;
			}
			catch (FormatException)
			{
				totf.error(h, string.Format("The value '{0}' is an invalid totalDigits facet specification", totf.Value.Trim()));
			}
		}

		private void checkFractionDigitsFacet(XmlSchemaFractionDigitsFacet fracf, ValidationEventHandler h)
		{
			if (fracf == null)
			{
				return;
			}
			try
			{
				decimal num = decimal.Parse(fracf.Value.Trim(), lengthStyle, CultureInfo.InvariantCulture);
				if (num < 0m)
				{
					fracf.error(h, string.Format(CultureInfo.InvariantCulture, "The value '{0}' is an invalid fractionDigits value", num));
				}
				if (fractionDigitsFacet >= 0m && num > fractionDigitsFacet)
				{
					fracf.error(h, string.Format(CultureInfo.InvariantCulture, "The value '{0}' is not a valid restriction of the base fractionDigits facet '{1}'", num, fractionDigitsFacet));
				}
				fractionDigitsFacet = num;
			}
			catch (FormatException)
			{
				fracf.error(h, string.Format("The value '{0}' is an invalid fractionDigits facet specification", fracf.Value.Trim()));
			}
		}

		private void checkMinMaxFacet(XmlSchemaFacet facet, ref object baseFacet, ValidationEventHandler h)
		{
			object obj = ValidateValueWithDatatype(facet.Value);
			if (obj != null)
			{
				if ((fixedFacets & facet.ThisFacet) != XmlSchemaFacet.Facet.None && baseFacet != null)
				{
					XsdAnySimpleType datatype = getDatatype();
					if (datatype.Compare(obj, baseFacet) != XsdOrdering.Equal)
					{
						facet.error(h, string.Format(CultureInfo.InvariantCulture, "{0} is not the same as fixed parent {1} facet.", facet.Value, facet.ThisFacet));
					}
				}
				baseFacet = obj;
			}
			else
			{
				facet.error(h, string.Format("The value '{0}' is not valid against the base type.", facet.Value));
			}
		}

		private void checkLengthFacet(XmlSchemaLengthFacet lf, XmlSchemaFacet.Facet facetsDefined, ValidationEventHandler h)
		{
			if (lf == null)
			{
				return;
			}
			try
			{
				if ((facetsDefined & (XmlSchemaFacet.Facet.minLength | XmlSchemaFacet.Facet.maxLength)) != XmlSchemaFacet.Facet.None)
				{
					lf.error(h, "It is an error for both length and minLength or maxLength to be present.");
					return;
				}
				lengthFacet = decimal.Parse(lf.Value.Trim(), lengthStyle, CultureInfo.InvariantCulture);
				if (lengthFacet < 0m)
				{
					lf.error(h, "The value '" + lengthFacet + "' is an invalid length");
				}
			}
			catch (FormatException)
			{
				lf.error(h, "The value '" + lf.Value + "' is an invalid length facet specification");
			}
		}

		private void checkMaxLengthFacet(XmlSchemaMaxLengthFacet maxlf, XmlSchemaFacet.Facet facetsDefined, ValidationEventHandler h)
		{
			if (maxlf == null)
			{
				return;
			}
			try
			{
				if ((facetsDefined & XmlSchemaFacet.Facet.length) != XmlSchemaFacet.Facet.None)
				{
					maxlf.error(h, "It is an error for both length and minLength or maxLength to be present.");
					return;
				}
				decimal num = decimal.Parse(maxlf.Value.Trim(), lengthStyle, CultureInfo.InvariantCulture);
				if ((fixedFacets & XmlSchemaFacet.Facet.maxLength) != XmlSchemaFacet.Facet.None && num != maxLengthFacet)
				{
					maxlf.error(h, string.Format(CultureInfo.InvariantCulture, "The value '{0}' is not the same as the fixed value '{1}' on the base type", maxlf.Value.Trim(), maxLengthFacet));
				}
				if (maxLengthFacet > 0m && num > maxLengthFacet)
				{
					maxlf.error(h, string.Format(CultureInfo.InvariantCulture, "The value '{0}' is not a valid restriction of the value '{1}' on the base maxLength facet", maxlf.Value.Trim(), maxLengthFacet));
				}
				else
				{
					maxLengthFacet = num;
				}
				if (maxLengthFacet < 0m)
				{
					maxlf.error(h, "The value '" + maxLengthFacet + "' is an invalid maxLength");
				}
				if (minLengthFacet >= 0m && minLengthFacet > maxLengthFacet)
				{
					maxlf.error(h, "minLength is greater than maxLength.");
				}
			}
			catch (FormatException)
			{
				maxlf.error(h, "The value '" + maxlf.Value + "' is an invalid maxLength facet specification");
			}
		}

		private void checkMinLengthFacet(XmlSchemaMinLengthFacet minlf, XmlSchemaFacet.Facet facetsDefined, ValidationEventHandler h)
		{
			if (minlf == null)
			{
				return;
			}
			try
			{
				if (lengthFacet >= 0m)
				{
					minlf.error(h, "It is an error for both length and minLength or maxLength to be present.");
					return;
				}
				decimal num = decimal.Parse(minlf.Value.Trim(), lengthStyle, CultureInfo.InvariantCulture);
				if ((fixedFacets & XmlSchemaFacet.Facet.minLength) != XmlSchemaFacet.Facet.None && num != minLengthFacet)
				{
					minlf.error(h, string.Format(CultureInfo.InvariantCulture, "The value '{0}' is not the same as the fixed value '{1}' on the base type", minlf.Value.Trim(), minLengthFacet));
				}
				if (num < minLengthFacet)
				{
					minlf.error(h, string.Format(CultureInfo.InvariantCulture, "The value '{0}' is not a valid restriction of the value '{1}' on the base minLength facet", minlf.Value.Trim(), minLengthFacet));
				}
				else
				{
					minLengthFacet = num;
				}
				if (minLengthFacet < 0m)
				{
					minlf.error(h, "The value '" + minLengthFacet + "' is an invalid minLength");
				}
				if (maxLengthFacet >= 0m && minLengthFacet > maxLengthFacet)
				{
					minlf.error(h, "minLength is greater than maxLength.");
				}
			}
			catch (FormatException)
			{
				minlf.error(h, "The value '" + minlf.Value + "' is an invalid minLength facet specification");
			}
		}

		private XsdAnySimpleType getDatatype()
		{
			XsdAnySimpleType xsdAnySimpleType = base.ActualBaseSchemaType as XsdAnySimpleType;
			if (xsdAnySimpleType != null)
			{
				return xsdAnySimpleType;
			}
			XmlSchemaSimpleTypeContent content = ((XmlSchemaSimpleType)base.ActualBaseSchemaType).Content;
			if (content is XmlSchemaSimpleTypeRestriction)
			{
				return ((XmlSchemaSimpleTypeRestriction)content).getDatatype();
			}
			if (content is XmlSchemaSimpleTypeList || content is XmlSchemaSimpleTypeUnion)
			{
				return null;
			}
			return null;
		}

		private object ValidateValueWithDatatype(string value)
		{
			XsdAnySimpleType datatype = getDatatype();
			object result = null;
			if (datatype != null)
			{
				try
				{
					result = datatype.ParseValue(value, null, null);
					if (base.ActualBaseSchemaType is XmlSchemaSimpleType)
					{
						XmlSchemaSimpleTypeContent content = ((XmlSchemaSimpleType)base.ActualBaseSchemaType).Content;
						if (content is XmlSchemaSimpleTypeRestriction)
						{
							if (((XmlSchemaSimpleTypeRestriction)content).ValidateValueWithFacets(value, null, null))
							{
								return result;
							}
							return null;
						}
					}
				}
				catch (Exception)
				{
					return null;
				}
			}
			return result;
		}

		internal bool ValidateValueWithFacets(string value, XmlNameTable nt, IXmlNamespaceResolver nsmgr)
		{
			XmlSchemaSimpleType xmlSchemaSimpleType = base.ActualBaseSchemaType as XmlSchemaSimpleType;
			XmlSchemaSimpleTypeList xmlSchemaSimpleTypeList = ((xmlSchemaSimpleType == null) ? null : (xmlSchemaSimpleType.Content as XmlSchemaSimpleTypeList));
			if (xmlSchemaSimpleTypeList != null)
			{
				return ValidateListValueWithFacets(value, nt, nsmgr);
			}
			return ValidateNonListValueWithFacets(value, nt, nsmgr);
		}

		private bool ValidateListValueWithFacets(string value, XmlNameTable nt, IXmlNamespaceResolver nsmgr)
		{
			try
			{
				return ValidateListValueWithFacetsCore(value, nt, nsmgr);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool ValidateListValueWithFacetsCore(string value, XmlNameTable nt, IXmlNamespaceResolver nsmgr)
		{
			string[] array = ((XsdAnySimpleType)XmlSchemaDatatype.FromName("anySimpleType", "http://www.w3.org/2001/XMLSchema")).ParseListValue(value, nt);
			if (patternFacetValues != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < patternFacetValues.Length; j++)
					{
						if (rexPatterns[j] != null && !rexPatterns[j].IsMatch(array[i]))
						{
							return false;
						}
					}
				}
			}
			bool flag = false;
			if (enumarationFacetValues != null)
			{
				for (int k = 0; k < array.Length; k++)
				{
					for (int l = 0; l < enumarationFacetValues.Length; l++)
					{
						if (array[k] == enumarationFacetValues[l])
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag && enumarationFacetValues != null)
			{
				for (int m = 0; m < array.Length; m++)
				{
					XsdAnySimpleType xsdAnySimpleType = getDatatype();
					if (xsdAnySimpleType == null)
					{
						xsdAnySimpleType = (XsdAnySimpleType)XmlSchemaDatatype.FromName("anySimpleType", "http://www.w3.org/2001/XMLSchema");
					}
					object v = xsdAnySimpleType.ParseValue(array[m], nt, nsmgr);
					for (int n = 0; n < enumarationFacetValues.Length; n++)
					{
						if (XmlSchemaUtil.AreSchemaDatatypeEqual(xsdAnySimpleType, v, xsdAnySimpleType, xsdAnySimpleType.ParseValue(enumarationFacetValues[n], nt, nsmgr)))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return false;
					}
				}
			}
			if (lengthFacet >= 0m && (decimal)array.Length != lengthFacet)
			{
				return false;
			}
			if (maxLengthFacet >= 0m && (decimal)array.Length > maxLengthFacet)
			{
				return false;
			}
			if (minLengthFacet >= 0m && (decimal)array.Length < minLengthFacet)
			{
				return false;
			}
			return true;
		}

		private bool ValidateNonListValueWithFacets(string value, XmlNameTable nt, IXmlNamespaceResolver nsmgr)
		{
			try
			{
				return ValidateNonListValueWithFacetsCore(value, nt, nsmgr);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool ValidateNonListValueWithFacetsCore(string value, XmlNameTable nt, IXmlNamespaceResolver nsmgr)
		{
			if (patternFacetValues != null)
			{
				bool flag = false;
				for (int i = 0; i < patternFacetValues.Length; i++)
				{
					if (rexPatterns[i] != null && rexPatterns[i].IsMatch(value))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			XsdAnySimpleType datatype = getDatatype();
			bool flag2 = false;
			if (enumarationFacetValues != null)
			{
				for (int j = 0; j < enumarationFacetValues.Length; j++)
				{
					if (value == enumarationFacetValues[j])
					{
						flag2 = true;
						break;
					}
				}
			}
			if (!flag2 && enumarationFacetValues != null)
			{
				XsdAnySimpleType xsdAnySimpleType = datatype;
				if (xsdAnySimpleType == null)
				{
					xsdAnySimpleType = (XsdAnySimpleType)XmlSchemaDatatype.FromName("anySimpleType", "http://www.w3.org/2001/XMLSchema");
				}
				object v = xsdAnySimpleType.ParseValue(value, nt, nsmgr);
				for (int k = 0; k < enumarationFacetValues.Length; k++)
				{
					if (XmlSchemaUtil.AreSchemaDatatypeEqual(xsdAnySimpleType, v, xsdAnySimpleType, xsdAnySimpleType.ParseValue(enumarationFacetValues[k], nt, nsmgr)))
					{
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					return false;
				}
			}
			if (!(datatype is XsdQName) && !(datatype is XsdNotation) && (!(lengthFacet == -1m) || !(maxLengthFacet == -1m) || !(minLengthFacet == -1m)))
			{
				int num = datatype.Length(value);
				if (lengthFacet >= 0m && (decimal)num != lengthFacet)
				{
					return false;
				}
				if (maxLengthFacet >= 0m && (decimal)num > maxLengthFacet)
				{
					return false;
				}
				if (minLengthFacet >= 0m && (decimal)num < minLengthFacet)
				{
					return false;
				}
			}
			if (totalDigitsFacet >= 0m || fractionDigitsFacet >= 0m)
			{
				string text = value.Trim('+', '-', '0', '.');
				int num2 = 0;
				int num3 = text.Length;
				int num4 = text.IndexOf(".");
				if (num4 != -1)
				{
					num3--;
					num2 = text.Length - num4 - 1;
				}
				if (totalDigitsFacet >= 0m && (decimal)num3 > totalDigitsFacet)
				{
					return false;
				}
				if (fractionDigitsFacet >= 0m && (decimal)num2 > fractionDigitsFacet)
				{
					return false;
				}
			}
			if ((maxInclusiveFacet != null || maxExclusiveFacet != null || minInclusiveFacet != null || minExclusiveFacet != null) && datatype != null)
			{
				object x;
				try
				{
					x = datatype.ParseValue(value, nt, null);
				}
				catch (OverflowException)
				{
					return false;
				}
				catch (FormatException)
				{
					return false;
				}
				if (maxInclusiveFacet != null)
				{
					XsdOrdering xsdOrdering = datatype.Compare(x, maxInclusiveFacet);
					if (xsdOrdering != XsdOrdering.LessThan && xsdOrdering != XsdOrdering.Equal)
					{
						return false;
					}
				}
				if (maxExclusiveFacet != null)
				{
					XsdOrdering xsdOrdering2 = datatype.Compare(x, maxExclusiveFacet);
					if (xsdOrdering2 != XsdOrdering.LessThan)
					{
						return false;
					}
				}
				if (minInclusiveFacet != null)
				{
					XsdOrdering xsdOrdering3 = datatype.Compare(x, minInclusiveFacet);
					if (xsdOrdering3 != XsdOrdering.GreaterThan && xsdOrdering3 != XsdOrdering.Equal)
					{
						return false;
					}
				}
				if (minExclusiveFacet != null)
				{
					XsdOrdering xsdOrdering4 = datatype.Compare(x, minExclusiveFacet);
					if (xsdOrdering4 != XsdOrdering.GreaterThan)
					{
						return false;
					}
				}
			}
			return true;
		}

		internal static XmlSchemaSimpleTypeRestriction Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = new XmlSchemaSimpleTypeRestriction();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "restriction")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaSimpleTypeRestriction.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaSimpleTypeRestriction.LineNumber = reader.LineNumber;
			xmlSchemaSimpleTypeRestriction.LinePosition = reader.LinePosition;
			xmlSchemaSimpleTypeRestriction.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaSimpleTypeRestriction.Id = reader.Value;
				}
				else if (reader.Name == "base")
				{
					Exception innerEx;
					xmlSchemaSimpleTypeRestriction.baseTypeName = XmlSchemaUtil.ReadQNameAttribute(reader, out innerEx);
					if (innerEx != null)
					{
						XmlSchemaObject.error(h, reader.Value + " is not a valid value for base attribute", innerEx);
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for restriction", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSimpleTypeRestriction);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSimpleTypeRestriction;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "restriction")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSimpleTypeRestriction.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSimpleTypeRestriction.Annotation = xmlSchemaAnnotation;
					}
					continue;
				}
				if (num <= 2 && reader.LocalName == "simpleType")
				{
					num = 3;
					XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaSimpleTypeRestriction.baseType = xmlSchemaSimpleType;
					}
					continue;
				}
				if (num <= 3)
				{
					if (reader.LocalName == "minExclusive")
					{
						num = 3;
						XmlSchemaMinExclusiveFacet xmlSchemaMinExclusiveFacet = XmlSchemaMinExclusiveFacet.Read(reader, h);
						if (xmlSchemaMinExclusiveFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaMinExclusiveFacet);
						}
						continue;
					}
					if (reader.LocalName == "minInclusive")
					{
						num = 3;
						XmlSchemaMinInclusiveFacet xmlSchemaMinInclusiveFacet = XmlSchemaMinInclusiveFacet.Read(reader, h);
						if (xmlSchemaMinInclusiveFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaMinInclusiveFacet);
						}
						continue;
					}
					if (reader.LocalName == "maxExclusive")
					{
						num = 3;
						XmlSchemaMaxExclusiveFacet xmlSchemaMaxExclusiveFacet = XmlSchemaMaxExclusiveFacet.Read(reader, h);
						if (xmlSchemaMaxExclusiveFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaMaxExclusiveFacet);
						}
						continue;
					}
					if (reader.LocalName == "maxInclusive")
					{
						num = 3;
						XmlSchemaMaxInclusiveFacet xmlSchemaMaxInclusiveFacet = XmlSchemaMaxInclusiveFacet.Read(reader, h);
						if (xmlSchemaMaxInclusiveFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaMaxInclusiveFacet);
						}
						continue;
					}
					if (reader.LocalName == "totalDigits")
					{
						num = 3;
						XmlSchemaTotalDigitsFacet xmlSchemaTotalDigitsFacet = XmlSchemaTotalDigitsFacet.Read(reader, h);
						if (xmlSchemaTotalDigitsFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaTotalDigitsFacet);
						}
						continue;
					}
					if (reader.LocalName == "fractionDigits")
					{
						num = 3;
						XmlSchemaFractionDigitsFacet xmlSchemaFractionDigitsFacet = XmlSchemaFractionDigitsFacet.Read(reader, h);
						if (xmlSchemaFractionDigitsFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaFractionDigitsFacet);
						}
						continue;
					}
					if (reader.LocalName == "length")
					{
						num = 3;
						XmlSchemaLengthFacet xmlSchemaLengthFacet = XmlSchemaLengthFacet.Read(reader, h);
						if (xmlSchemaLengthFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaLengthFacet);
						}
						continue;
					}
					if (reader.LocalName == "minLength")
					{
						num = 3;
						XmlSchemaMinLengthFacet xmlSchemaMinLengthFacet = XmlSchemaMinLengthFacet.Read(reader, h);
						if (xmlSchemaMinLengthFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaMinLengthFacet);
						}
						continue;
					}
					if (reader.LocalName == "maxLength")
					{
						num = 3;
						XmlSchemaMaxLengthFacet xmlSchemaMaxLengthFacet = XmlSchemaMaxLengthFacet.Read(reader, h);
						if (xmlSchemaMaxLengthFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaMaxLengthFacet);
						}
						continue;
					}
					if (reader.LocalName == "enumeration")
					{
						num = 3;
						XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = XmlSchemaEnumerationFacet.Read(reader, h);
						if (xmlSchemaEnumerationFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaEnumerationFacet);
						}
						continue;
					}
					if (reader.LocalName == "whiteSpace")
					{
						num = 3;
						XmlSchemaWhiteSpaceFacet xmlSchemaWhiteSpaceFacet = XmlSchemaWhiteSpaceFacet.Read(reader, h);
						if (xmlSchemaWhiteSpaceFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaWhiteSpaceFacet);
						}
						continue;
					}
					if (reader.LocalName == "pattern")
					{
						num = 3;
						XmlSchemaPatternFacet xmlSchemaPatternFacet = XmlSchemaPatternFacet.Read(reader, h);
						if (xmlSchemaPatternFacet != null)
						{
							xmlSchemaSimpleTypeRestriction.facets.Add(xmlSchemaPatternFacet);
						}
						continue;
					}
				}
				reader.RaiseInvalidElementError();
			}
			return xmlSchemaSimpleTypeRestriction;
		}
	}
}
