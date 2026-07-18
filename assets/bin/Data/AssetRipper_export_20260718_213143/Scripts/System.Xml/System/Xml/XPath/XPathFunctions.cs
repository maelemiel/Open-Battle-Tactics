using System.Globalization;

namespace System.Xml.XPath
{
	internal class XPathFunctions
	{
		public static bool ToBoolean(object arg)
		{
			if (arg == null)
			{
				throw new ArgumentNullException();
			}
			if (arg is bool)
			{
				return (bool)arg;
			}
			if (arg is double)
			{
				double num = (double)arg;
				return num != 0.0 && !double.IsNaN(num);
			}
			if (arg is string)
			{
				return ((string)arg).Length != 0;
			}
			if (arg is XPathNodeIterator)
			{
				XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)arg;
				return xPathNodeIterator.MoveNext();
			}
			if (arg is XPathNavigator)
			{
				return ToBoolean(((XPathNavigator)arg).SelectChildren(XPathNodeType.All));
			}
			throw new ArgumentException();
		}

		public static bool ToBoolean(bool b)
		{
			return b;
		}

		public static bool ToBoolean(double d)
		{
			return d != 0.0 && !double.IsNaN(d);
		}

		public static bool ToBoolean(string s)
		{
			return s != null && s.Length > 0;
		}

		public static bool ToBoolean(BaseIterator iter)
		{
			return iter != null && iter.MoveNext();
		}

		public static string ToString(object arg)
		{
			if (arg == null)
			{
				throw new ArgumentNullException();
			}
			if (arg is string)
			{
				return (string)arg;
			}
			if (arg is bool)
			{
				return (!(bool)arg) ? "false" : "true";
			}
			if (arg is double)
			{
				return ToString((double)arg);
			}
			if (arg is XPathNodeIterator)
			{
				XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)arg;
				if (!xPathNodeIterator.MoveNext())
				{
					return string.Empty;
				}
				return xPathNodeIterator.Current.Value;
			}
			if (arg is XPathNavigator)
			{
				return ((XPathNavigator)arg).Value;
			}
			throw new ArgumentException();
		}

		public static string ToString(double d)
		{
			if (d == double.NegativeInfinity)
			{
				return "-Infinity";
			}
			if (d == double.PositiveInfinity)
			{
				return "Infinity";
			}
			return d.ToString("R", NumberFormatInfo.InvariantInfo);
		}

		public static double ToNumber(object arg)
		{
			if (arg == null)
			{
				throw new ArgumentNullException();
			}
			if (arg is BaseIterator || arg is XPathNavigator)
			{
				arg = ToString(arg);
			}
			if (arg is string)
			{
				string arg2 = arg as string;
				return ToNumber(arg2);
			}
			if (arg is double)
			{
				return (double)arg;
			}
			if (arg is bool)
			{
				return Convert.ToDouble((bool)arg);
			}
			throw new ArgumentException();
		}

		public static double ToNumber(string arg)
		{
			if (arg == null)
			{
				throw new ArgumentNullException();
			}
			string text = arg.Trim(XmlChar.WhitespaceChars);
			if (text.Length == 0)
			{
				return double.NaN;
			}
			try
			{
				if (text[0] == '.')
				{
					text = '.' + text;
				}
				return double.Parse(text, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo);
			}
			catch (OverflowException)
			{
				return double.NaN;
			}
			catch (FormatException)
			{
				return double.NaN;
			}
		}
	}
}
