using System;
using System.Globalization;
using System.Text;

namespace Mono.Xml.Xsl
{
	internal class DecimalFormatPattern
	{
		public string Prefix = string.Empty;

		public string Suffix = string.Empty;

		public string NumberPart;

		private NumberFormatInfo info;

		private StringBuilder builder = new StringBuilder();

		internal int ParsePattern(int start, string pattern, XslDecimalFormat format)
		{
			if (start == 0)
			{
				info = format.Info;
			}
			else
			{
				info = format.Info.Clone() as NumberFormatInfo;
				info.NegativeSign = string.Empty;
			}
			int i;
			for (i = start; i < pattern.Length && pattern[i] != format.ZeroDigit && pattern[i] != format.Digit && pattern[i] != format.Info.CurrencySymbol[0]; i++)
			{
			}
			Prefix = pattern.Substring(start, i - start);
			if (i == pattern.Length)
			{
				return i;
			}
			i = ParseNumber(i, pattern, format);
			int num = i;
			for (; i < pattern.Length && pattern[i] != format.ZeroDigit && pattern[i] != format.Digit && pattern[i] != format.PatternSeparator && pattern[i] != format.Info.CurrencySymbol[0]; i++)
			{
			}
			Suffix = pattern.Substring(num, i - num);
			return i;
		}

		private int ParseNumber(int start, string pattern, XslDecimalFormat format)
		{
			int i;
			for (i = start; i < pattern.Length; i++)
			{
				if (pattern[i] == format.Digit)
				{
					builder.Append('#');
					continue;
				}
				if (pattern[i] == format.Info.NumberGroupSeparator[0])
				{
					builder.Append(',');
					continue;
				}
				break;
			}
			for (; i < pattern.Length; i++)
			{
				if (pattern[i] == format.ZeroDigit)
				{
					builder.Append('0');
					continue;
				}
				if (pattern[i] == format.Info.NumberGroupSeparator[0])
				{
					builder.Append(',');
					continue;
				}
				break;
			}
			if (i < pattern.Length)
			{
				if (pattern[i] == format.Info.NumberDecimalSeparator[0])
				{
					builder.Append('.');
					i++;
				}
				while (i < pattern.Length && pattern[i] == format.ZeroDigit)
				{
					i++;
					builder.Append('0');
				}
				while (i < pattern.Length && pattern[i] == format.Digit)
				{
					i++;
					builder.Append('#');
				}
			}
			if (i + 1 < pattern.Length && pattern[i] == 'E' && pattern[i + 1] == format.ZeroDigit)
			{
				i += 2;
				builder.Append("E0");
				while (i < pattern.Length && pattern[i] == format.ZeroDigit)
				{
					i++;
					builder.Append('0');
				}
			}
			if (i < pattern.Length)
			{
				if (pattern[i] == info.PercentSymbol[0])
				{
					builder.Append('%');
				}
				else if (pattern[i] == info.PerMilleSymbol[0])
				{
					builder.Append('‰');
				}
				else
				{
					if (pattern[i] == info.CurrencySymbol[0])
					{
						throw new ArgumentException("Currency symbol is not supported for number format pattern string.");
					}
					i--;
				}
				i++;
			}
			NumberPart = builder.ToString();
			return i;
		}

		public string FormatNumber(double number)
		{
			builder.Length = 0;
			builder.Append(Prefix);
			builder.Append(number.ToString(NumberPart, info));
			builder.Append(Suffix);
			return builder.ToString();
		}
	}
}
