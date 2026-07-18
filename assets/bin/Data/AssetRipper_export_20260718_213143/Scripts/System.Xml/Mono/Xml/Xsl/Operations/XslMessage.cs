using System;
using System.IO;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslMessage : XslCompiledElement
	{
		private static TextWriter output;

		private bool terminate;

		private XslOperation children;

		public XslMessage(Compiler c)
			: base(c)
		{
		}

		static XslMessage()
		{
			switch (Environment.GetEnvironmentVariable("MONO_XSLT_MESSAGE_OUTPUT"))
			{
			case "none":
				output = TextWriter.Null;
				break;
			case "stderr":
				output = Console.Error;
				break;
			default:
				output = Console.Out;
				break;
			}
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(base.DebugInput);
			}
			c.CheckExtraAttributes("message", "terminate");
			terminate = c.ParseYesNoAttribute("terminate", false);
			if (c.Input.MoveToFirstChild())
			{
				children = c.CompileTemplateContent();
				c.Input.MoveToParent();
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			if (children != null)
			{
				output.Write(children.EvaluateAsString(p));
			}
			if (terminate)
			{
				throw new XsltException("Transformation terminated.", null, p.CurrentNode);
			}
		}
	}
}
