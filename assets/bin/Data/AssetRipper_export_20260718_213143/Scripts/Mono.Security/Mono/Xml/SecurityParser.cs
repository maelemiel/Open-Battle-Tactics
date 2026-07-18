using System;
using System.Collections;
using System.Security;

namespace Mono.Xml
{
	[CLSCompliant(false)]
	public class SecurityParser : MiniParser, MiniParser.IReader, MiniParser.IHandler
	{
		private SecurityElement root;

		private string xmldoc;

		private int pos;

		private SecurityElement current;

		private Stack stack;

		public SecurityParser()
		{
			stack = new Stack();
		}

		public void LoadXml(string xml)
		{
			root = null;
			xmldoc = xml;
			pos = 0;
			stack.Clear();
			Parse(this, this);
		}

		public SecurityElement ToXml()
		{
			return root;
		}

		public int Read()
		{
			if (pos >= xmldoc.Length)
			{
				return -1;
			}
			return xmldoc[pos++];
		}

		public void OnStartParsing(MiniParser parser)
		{
		}

		public void OnStartElement(string name, IAttrList attrs)
		{
			SecurityElement securityElement = new SecurityElement(name);
			if (root == null)
			{
				root = securityElement;
				current = securityElement;
			}
			else
			{
				SecurityElement securityElement2 = (SecurityElement)stack.Peek();
				securityElement2.AddChild(securityElement);
			}
			stack.Push(securityElement);
			current = securityElement;
			int length = attrs.Length;
			for (int i = 0; i < length; i++)
			{
				current.AddAttribute(attrs.GetName(i), SecurityElement.Escape(attrs.GetValue(i)));
			}
		}

		public void OnEndElement(string name)
		{
			current = (SecurityElement)stack.Pop();
		}

		public void OnChars(string ch)
		{
			current.Text = SecurityElement.Escape(ch);
		}

		public void OnEndParsing(MiniParser parser)
		{
		}
	}
}
