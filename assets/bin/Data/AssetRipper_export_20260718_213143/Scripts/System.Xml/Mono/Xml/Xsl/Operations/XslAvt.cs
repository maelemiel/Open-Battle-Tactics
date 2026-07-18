using System.Collections;
using System.IO;
using System.Text;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslAvt
	{
		private abstract class AvtPart
		{
			public abstract string Evaluate(XslTransformProcessor p);
		}

		private sealed class SimpleAvtPart : AvtPart
		{
			private string val;

			public SimpleAvtPart(string val)
			{
				this.val = val;
			}

			public override string Evaluate(XslTransformProcessor p)
			{
				return val;
			}
		}

		private sealed class XPathAvtPart : AvtPart
		{
			private XPathExpression expr;

			public XPathAvtPart(XPathExpression expr)
			{
				this.expr = expr;
			}

			public override string Evaluate(XslTransformProcessor p)
			{
				return p.EvaluateString(expr);
			}
		}

		private string simpleString;

		private ArrayList avtParts;

		public XslAvt(string str, Compiler comp)
		{
			if (str.IndexOf("{") == -1 && str.IndexOf("}") == -1)
			{
				simpleString = str;
				return;
			}
			avtParts = new ArrayList();
			StringBuilder stringBuilder = new StringBuilder();
			StringReader stringReader = new StringReader(str);
			while (stringReader.Peek() != -1)
			{
				char c = (char)stringReader.Read();
				switch (c)
				{
				case '{':
					if ((ushort)stringReader.Peek() == 123)
					{
						stringBuilder.Append((char)stringReader.Read());
						continue;
					}
					if (stringBuilder.Length != 0)
					{
						avtParts.Add(new SimpleAvtPart(stringBuilder.ToString()));
						stringBuilder.Length = 0;
					}
					while ((c = (char)stringReader.Read()) != '}')
					{
						char c2 = c;
						if (c2 == '"' || c2 == '\'')
						{
							char c3 = c;
							stringBuilder.Append(c);
							while ((c = (char)stringReader.Read()) != c3)
							{
								stringBuilder.Append(c);
								if (stringReader.Peek() == -1)
								{
									throw new XsltCompileException("Unexpected end of AVT", null, comp.Input);
								}
							}
							stringBuilder.Append(c);
						}
						else
						{
							stringBuilder.Append(c);
						}
						if (stringReader.Peek() == -1)
						{
							throw new XsltCompileException("Unexpected end of AVT", null, comp.Input);
						}
					}
					avtParts.Add(new XPathAvtPart(comp.CompileExpression(stringBuilder.ToString())));
					stringBuilder.Length = 0;
					continue;
				case '}':
					c = (char)stringReader.Read();
					if (c != '}')
					{
						throw new XsltCompileException("Braces must be escaped", null, comp.Input);
					}
					break;
				}
				stringBuilder.Append(c);
			}
			if (stringBuilder.Length != 0)
			{
				avtParts.Add(new SimpleAvtPart(stringBuilder.ToString()));
				stringBuilder.Length = 0;
			}
		}

		public static string AttemptPreCalc(ref XslAvt avt)
		{
			if (avt == null)
			{
				return null;
			}
			if (avt.simpleString != null)
			{
				string result = avt.simpleString;
				avt = null;
				return result;
			}
			return null;
		}

		public string Evaluate(XslTransformProcessor p)
		{
			if (simpleString != null)
			{
				return simpleString;
			}
			if (avtParts.Count == 1)
			{
				return ((AvtPart)avtParts[0]).Evaluate(p);
			}
			StringBuilder avtStringBuilder = p.GetAvtStringBuilder();
			int count = avtParts.Count;
			for (int i = 0; i < count; i++)
			{
				avtStringBuilder.Append(((AvtPart)avtParts[i]).Evaluate(p));
			}
			return p.ReleaseAvtStringBuilder();
		}
	}
}
