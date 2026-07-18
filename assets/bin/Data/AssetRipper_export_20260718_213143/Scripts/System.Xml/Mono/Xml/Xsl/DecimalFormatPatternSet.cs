using System;

namespace Mono.Xml.Xsl
{
	internal class DecimalFormatPatternSet
	{
		private DecimalFormatPattern positivePattern;

		private DecimalFormatPattern negativePattern;

		public DecimalFormatPatternSet(string pattern, XslDecimalFormat decimalFormat)
		{
			Parse(pattern, decimalFormat);
		}

		private void Parse(string pattern, XslDecimalFormat format)
		{
			if (pattern.Length == 0)
			{
				throw new ArgumentException("Invalid number format pattern string.");
			}
			positivePattern = new DecimalFormatPattern();
			negativePattern = positivePattern;
			int num = positivePattern.ParsePattern(0, pattern, format);
			if (num < pattern.Length && pattern[num] == format.PatternSeparator)
			{
				num++;
				negativePattern = new DecimalFormatPattern();
				num = negativePattern.ParsePattern(num, pattern, format);
				if (num < pattern.Length)
				{
					throw new ArgumentException("Number format pattern string ends with extraneous part.");
				}
			}
		}

		public string FormatNumber(double number)
		{
			if (number >= 0.0)
			{
				return positivePattern.FormatNumber(number);
			}
			return negativePattern.FormatNumber(number);
		}
	}
}
