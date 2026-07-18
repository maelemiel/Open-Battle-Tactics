using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class XslDecimalFormat
	{
		private NumberFormatInfo info = new NumberFormatInfo();

		private char digit = '#';

		private char zeroDigit = '0';

		private char patternSeparator = ';';

		private string baseUri;

		private int lineNumber;

		private int linePosition;

		public static readonly XslDecimalFormat Default = new XslDecimalFormat();

		public char Digit
		{
			get
			{
				return digit;
			}
		}

		public char ZeroDigit
		{
			get
			{
				return zeroDigit;
			}
		}

		public NumberFormatInfo Info
		{
			get
			{
				return info;
			}
		}

		public char PatternSeparator
		{
			get
			{
				return patternSeparator;
			}
		}

		private XslDecimalFormat()
		{
		}

		public XslDecimalFormat(Compiler c)
		{
			XPathNavigator input = c.Input;
			IXmlLineInfo xmlLineInfo = input as IXmlLineInfo;
			if (xmlLineInfo != null)
			{
				lineNumber = xmlLineInfo.LineNumber;
				linePosition = xmlLineInfo.LinePosition;
			}
			baseUri = input.BaseURI;
			if (!input.MoveToFirstAttribute())
			{
				return;
			}
			do
			{
				if (input.NamespaceURI != string.Empty)
				{
					continue;
				}
				switch (input.LocalName)
				{
				case "decimal-separator":
					if (input.Value.Length != 1)
					{
						throw new XsltCompileException("XSLT decimal-separator value must be exact one character", null, input);
					}
					info.NumberDecimalSeparator = input.Value;
					break;
				case "grouping-separator":
					if (input.Value.Length != 1)
					{
						throw new XsltCompileException("XSLT grouping-separator value must be exact one character", null, input);
					}
					info.NumberGroupSeparator = input.Value;
					break;
				case "infinity":
					info.PositiveInfinitySymbol = input.Value;
					break;
				case "minus-sign":
					if (input.Value.Length != 1)
					{
						throw new XsltCompileException("XSLT minus-sign value must be exact one character", null, input);
					}
					info.NegativeSign = input.Value;
					break;
				case "NaN":
					info.NaNSymbol = input.Value;
					break;
				case "percent":
					if (input.Value.Length != 1)
					{
						throw new XsltCompileException("XSLT percent value must be exact one character", null, input);
					}
					info.PercentSymbol = input.Value;
					break;
				case "per-mille":
					if (input.Value.Length != 1)
					{
						throw new XsltCompileException("XSLT per-mille value must be exact one character", null, input);
					}
					info.PerMilleSymbol = input.Value;
					break;
				case "digit":
					if (input.Value.Length != 1)
					{
						throw new XsltCompileException("XSLT digit value must be exact one character", null, input);
					}
					digit = input.Value[0];
					break;
				case "zero-digit":
					if (input.Value.Length != 1)
					{
						throw new XsltCompileException("XSLT zero-digit value must be exact one character", null, input);
					}
					zeroDigit = input.Value[0];
					break;
				case "pattern-separator":
					if (input.Value.Length != 1)
					{
						throw new XsltCompileException("XSLT pattern-separator value must be exact one character", null, input);
					}
					patternSeparator = input.Value[0];
					break;
				}
			}
			while (input.MoveToNextAttribute());
			input.MoveToParent();
			info.NegativeInfinitySymbol = info.NegativeSign + info.PositiveInfinitySymbol;
		}

		public void CheckSameAs(XslDecimalFormat other)
		{
			if (digit != other.digit || patternSeparator != other.patternSeparator || zeroDigit != other.zeroDigit || info.NumberDecimalSeparator != other.info.NumberDecimalSeparator || info.NumberGroupSeparator != other.info.NumberGroupSeparator || info.PositiveInfinitySymbol != other.info.PositiveInfinitySymbol || info.NegativeSign != other.info.NegativeSign || info.NaNSymbol != other.info.NaNSymbol || info.PercentSymbol != other.info.PercentSymbol || info.PerMilleSymbol != other.info.PerMilleSymbol)
			{
				throw new XsltCompileException(null, other.baseUri, other.lineNumber, other.linePosition);
			}
		}

		public string FormatNumber(double number, string pattern)
		{
			return ParsePatternSet(pattern).FormatNumber(number);
		}

		private DecimalFormatPatternSet ParsePatternSet(string pattern)
		{
			return new DecimalFormatPatternSet(pattern, this);
		}
	}
}
