using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslNumber : XslCompiledElement
	{
		private class XslNumberFormatter
		{
			private class NumberFormatterScanner
			{
				private int pos;

				private int len;

				private string fmt;

				public NumberFormatterScanner(string fmt)
				{
					this.fmt = fmt;
					len = fmt.Length;
				}

				public string Advance(bool alphaNum)
				{
					int num = pos;
					while (pos < len && char.IsLetterOrDigit(fmt, pos) == alphaNum)
					{
						pos++;
					}
					if (pos == num)
					{
						return null;
					}
					return fmt.Substring(num, pos - num);
				}
			}

			private abstract class FormatItem
			{
				public readonly string sep;

				public FormatItem(string sep)
				{
					this.sep = sep;
				}

				public abstract void Format(StringBuilder b, double num);

				public static FormatItem GetItem(string sep, string item, char gpSep, int gpSize)
				{
					switch (item[0])
					{
					default:
						return new DigitItem(sep, 1, gpSep, gpSize);
					case '0':
					case '1':
					{
						int i;
						for (i = 1; i < item.Length && char.IsDigit(item, i); i++)
						{
						}
						return new DigitItem(sep, i, gpSep, gpSize);
					}
					case 'a':
						return new AlphaItem(sep, false);
					case 'A':
						return new AlphaItem(sep, true);
					case 'i':
						return new RomanItem(sep, false);
					case 'I':
						return new RomanItem(sep, true);
					}
				}
			}

			private class AlphaItem : FormatItem
			{
				private bool uc;

				private static readonly char[] ucl = new char[26]
				{
					'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
					'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
					'U', 'V', 'W', 'X', 'Y', 'Z'
				};

				private static readonly char[] lcl = new char[26]
				{
					'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
					'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
					'u', 'v', 'w', 'x', 'y', 'z'
				};

				public AlphaItem(string sep, bool uc)
					: base(sep)
				{
					this.uc = uc;
				}

				public override void Format(StringBuilder b, double num)
				{
					alphaSeq(b, num, (!uc) ? lcl : ucl);
				}

				private static void alphaSeq(StringBuilder b, double n, char[] alphabet)
				{
					n = Round(n);
					if (n != 0.0)
					{
						if (n > (double)alphabet.Length)
						{
							alphaSeq(b, System.Math.Floor((n - 1.0) / (double)alphabet.Length), alphabet);
						}
						b.Append(alphabet[((int)n - 1) % alphabet.Length]);
					}
				}
			}

			private class RomanItem : FormatItem
			{
				private bool uc;

				private static readonly string[] ucrDigits = new string[13]
				{
					"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX",
					"V", "IV", "I"
				};

				private static readonly string[] lcrDigits = new string[13]
				{
					"m", "cm", "d", "cd", "c", "xc", "l", "xl", "x", "ix",
					"v", "iv", "i"
				};

				private static readonly int[] decValues = new int[13]
				{
					1000, 900, 500, 400, 100, 90, 50, 40, 10, 9,
					5, 4, 1
				};

				public RomanItem(string sep, bool uc)
					: base(sep)
				{
					this.uc = uc;
				}

				public override void Format(StringBuilder b, double num)
				{
					if (num < 1.0 || num > 4999.0)
					{
						b.Append(num);
						return;
					}
					num = Round(num);
					for (int i = 0; i < decValues.Length; i++)
					{
						while ((double)decValues[i] <= num)
						{
							if (uc)
							{
								b.Append(ucrDigits[i]);
							}
							else
							{
								b.Append(lcrDigits[i]);
							}
							num -= (double)decValues[i];
						}
						if (num == 0.0)
						{
							break;
						}
					}
				}
			}

			private class DigitItem : FormatItem
			{
				private NumberFormatInfo nfi;

				private int decimalSectionLength;

				private StringBuilder numberBuilder;

				public DigitItem(string sep, int len, char gpSep, int gpSize)
					: base(sep)
				{
					nfi = new NumberFormatInfo();
					nfi.NumberDecimalDigits = 0;
					nfi.NumberGroupSizes = new int[1];
					if (gpSep != 0 && gpSize > 0)
					{
						nfi.NumberGroupSeparator = gpSep.ToString();
						nfi.NumberGroupSizes = new int[1] { gpSize };
					}
					decimalSectionLength = len;
				}

				public override void Format(StringBuilder b, double num)
				{
					string text = num.ToString("N", nfi);
					int num2 = decimalSectionLength;
					if (num2 > 1)
					{
						if (numberBuilder == null)
						{
							numberBuilder = new StringBuilder();
						}
						for (int num3 = num2; num3 > text.Length; num3--)
						{
							numberBuilder.Append('0');
						}
						numberBuilder.Append((text.Length > num2) ? text.Substring(text.Length - num2, num2) : text);
						text = numberBuilder.ToString();
						numberBuilder.Length = 0;
					}
					b.Append(text);
				}
			}

			private string firstSep;

			private string lastSep;

			private ArrayList fmtList = new ArrayList();

			public XslNumberFormatter(string format, string lang, string letterValue, char groupingSeparator, int groupingSize)
			{
				if (format == null || format == string.Empty)
				{
					fmtList.Add(FormatItem.GetItem(null, "1", groupingSeparator, groupingSize));
					return;
				}
				NumberFormatterScanner numberFormatterScanner = new NumberFormatterScanner(format);
				string text = ".";
				firstSep = numberFormatterScanner.Advance(false);
				string text2 = numberFormatterScanner.Advance(true);
				if (text2 == null)
				{
					text = firstSep;
					firstSep = null;
					fmtList.Add(FormatItem.GetItem(text, "1", groupingSeparator, groupingSize));
					return;
				}
				fmtList.Add(FormatItem.GetItem(".", text2, groupingSeparator, groupingSize));
				do
				{
					text = numberFormatterScanner.Advance(false);
					text2 = numberFormatterScanner.Advance(true);
					if (text2 == null)
					{
						lastSep = text;
						break;
					}
					fmtList.Add(FormatItem.GetItem(text, text2, groupingSeparator, groupingSize));
				}
				while (text2 != null);
			}

			public string Format(double value)
			{
				return Format(value, true);
			}

			public string Format(double value, bool formatContent)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (firstSep != null)
				{
					stringBuilder.Append(firstSep);
				}
				if (formatContent)
				{
					((FormatItem)fmtList[0]).Format(stringBuilder, value);
				}
				if (lastSep != null)
				{
					stringBuilder.Append(lastSep);
				}
				return stringBuilder.ToString();
			}

			public string Format(int[] values)
			{
				StringBuilder stringBuilder = new StringBuilder();
				if (firstSep != null)
				{
					stringBuilder.Append(firstSep);
				}
				int num = 0;
				int num2 = fmtList.Count - 1;
				if (values.Length > 0)
				{
					if (fmtList.Count > 0)
					{
						FormatItem formatItem = (FormatItem)fmtList[num];
						formatItem.Format(stringBuilder, values[0]);
					}
					if (num < num2)
					{
						num++;
					}
				}
				for (int i = 1; i < values.Length; i++)
				{
					FormatItem formatItem2 = (FormatItem)fmtList[num];
					stringBuilder.Append(formatItem2.sep);
					int num3 = values[i];
					formatItem2.Format(stringBuilder, num3);
					if (num < num2)
					{
						num++;
					}
				}
				if (lastSep != null)
				{
					stringBuilder.Append(lastSep);
				}
				return stringBuilder.ToString();
			}
		}

		private XslNumberingLevel level;

		private Pattern count;

		private Pattern from;

		private XPathExpression value;

		private XslAvt format;

		private XslAvt lang;

		private XslAvt letterValue;

		private XslAvt groupingSeparator;

		private XslAvt groupingSize;

		public XslNumber(Compiler c)
			: base(c)
		{
		}

		public static double Round(double n)
		{
			double num = System.Math.Floor(n);
			return (!(n - num >= 0.5)) ? num : (num + 1.0);
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(base.DebugInput);
			}
			c.CheckExtraAttributes("number", "level", "count", "from", "value", "format", "lang", "letter-value", "grouping-separator", "grouping-size");
			switch (c.GetAttribute("level"))
			{
			case "single":
				level = XslNumberingLevel.Single;
				break;
			case "multiple":
				level = XslNumberingLevel.Multiple;
				break;
			case "any":
				level = XslNumberingLevel.Any;
				break;
			default:
				level = XslNumberingLevel.Single;
				break;
			}
			count = c.CompilePattern(c.GetAttribute("count"), c.Input);
			from = c.CompilePattern(c.GetAttribute("from"), c.Input);
			value = c.CompileExpression(c.GetAttribute("value"));
			format = c.ParseAvtAttribute("format");
			lang = c.ParseAvtAttribute("lang");
			letterValue = c.ParseAvtAttribute("letter-value");
			groupingSeparator = c.ParseAvtAttribute("grouping-separator");
			groupingSize = c.ParseAvtAttribute("grouping-size");
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			string text = GetFormat(p);
			if (text != string.Empty)
			{
				p.Out.WriteString(text);
			}
		}

		private XslNumberFormatter GetNumberFormatter(XslTransformProcessor p)
		{
			string text = "1";
			string text2 = null;
			string text3 = null;
			char c = '\0';
			decimal num = 0m;
			if (format != null)
			{
				text = format.Evaluate(p);
			}
			if (lang != null)
			{
				text2 = lang.Evaluate(p);
			}
			if (letterValue != null)
			{
				text3 = letterValue.Evaluate(p);
			}
			if (groupingSeparator != null)
			{
				c = groupingSeparator.Evaluate(p)[0];
			}
			if (groupingSize != null)
			{
				num = decimal.Parse(groupingSize.Evaluate(p), CultureInfo.InvariantCulture);
			}
			if (num > 2147483647m || num < 1m)
			{
				num = 0m;
			}
			return new XslNumberFormatter(text, text2, text3, c, (int)num);
		}

		private string GetFormat(XslTransformProcessor p)
		{
			XslNumberFormatter numberFormatter = GetNumberFormatter(p);
			if (value != null)
			{
				double num = p.EvaluateNumber(value);
				return numberFormatter.Format(num);
			}
			switch (level)
			{
			case XslNumberingLevel.Single:
			{
				int num2 = NumberSingle(p);
				return numberFormatter.Format(num2, num2 != 0);
			}
			case XslNumberingLevel.Multiple:
				return numberFormatter.Format(NumberMultiple(p));
			case XslNumberingLevel.Any:
			{
				int num2 = NumberAny(p);
				return numberFormatter.Format(num2, num2 != 0);
			}
			default:
				throw new XsltException("Should not get here", null, p.CurrentNode);
			}
		}

		private int[] NumberMultiple(XslTransformProcessor p)
		{
			ArrayList arrayList = new ArrayList();
			XPathNavigator xPathNavigator = p.CurrentNode.Clone();
			bool flag = false;
			do
			{
				if (MatchesFrom(xPathNavigator, p))
				{
					flag = true;
					break;
				}
				if (!MatchesCount(xPathNavigator, p))
				{
					continue;
				}
				int num = 1;
				while (xPathNavigator.MoveToPrevious())
				{
					if (MatchesCount(xPathNavigator, p))
					{
						num++;
					}
				}
				arrayList.Add(num);
			}
			while (xPathNavigator.MoveToParent());
			if (!flag)
			{
				return new int[0];
			}
			int[] array = new int[arrayList.Count];
			int num2 = arrayList.Count;
			for (int i = 0; i < arrayList.Count; i++)
			{
				array[--num2] = (int)arrayList[i];
			}
			return array;
		}

		private int NumberAny(XslTransformProcessor p)
		{
			int num = 0;
			XPathNavigator xPathNavigator = p.CurrentNode.Clone();
			xPathNavigator.MoveToRoot();
			bool flag = from == null;
			while (true)
			{
				if (from != null && MatchesFrom(xPathNavigator, p))
				{
					flag = true;
					num = 0;
				}
				else if (flag && MatchesCount(xPathNavigator, p))
				{
					num++;
				}
				if (xPathNavigator.IsSamePosition(p.CurrentNode))
				{
					break;
				}
				if (xPathNavigator.MoveToFirstChild())
				{
					continue;
				}
				while (!xPathNavigator.MoveToNext())
				{
					if (!xPathNavigator.MoveToParent())
					{
						return 0;
					}
				}
			}
			return num;
		}

		private int NumberSingle(XslTransformProcessor p)
		{
			XPathNavigator xPathNavigator = p.CurrentNode.Clone();
			while (!MatchesCount(xPathNavigator, p))
			{
				if (from != null && MatchesFrom(xPathNavigator, p))
				{
					return 0;
				}
				if (!xPathNavigator.MoveToParent())
				{
					return 0;
				}
			}
			if (from != null)
			{
				XPathNavigator xPathNavigator2 = xPathNavigator.Clone();
				if (MatchesFrom(xPathNavigator2, p))
				{
					return 0;
				}
				bool flag = false;
				while (xPathNavigator2.MoveToParent())
				{
					if (MatchesFrom(xPathNavigator2, p))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return 0;
				}
			}
			int num = 1;
			while (xPathNavigator.MoveToPrevious())
			{
				if (MatchesCount(xPathNavigator, p))
				{
					num++;
				}
			}
			return num;
		}

		private bool MatchesCount(XPathNavigator item, XslTransformProcessor p)
		{
			if (count == null)
			{
				return item.NodeType == p.CurrentNode.NodeType && item.LocalName == p.CurrentNode.LocalName && item.NamespaceURI == p.CurrentNode.NamespaceURI;
			}
			return p.Matches(count, item);
		}

		private bool MatchesFrom(XPathNavigator item, XslTransformProcessor p)
		{
			if (from == null)
			{
				return item.NodeType == XPathNodeType.Root;
			}
			return p.Matches(from, item);
		}
	}
}
