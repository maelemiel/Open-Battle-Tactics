using System;
using System.Reflection;
using System.Xml.XPath;

namespace Mono.Xml.Xsl
{
	internal class XsltDebuggerWrapper
	{
		private readonly MethodInfo on_compile;

		private readonly MethodInfo on_execute;

		private readonly object impl;

		public XsltDebuggerWrapper(object impl)
		{
			this.impl = impl;
			on_compile = impl.GetType().GetMethod("OnCompile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (on_compile == null)
			{
				throw new InvalidOperationException("INTERNAL ERROR: the debugger does not look like what System.Xml.dll expects. OnCompile method was not found");
			}
			on_execute = impl.GetType().GetMethod("OnExecute", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (on_execute == null)
			{
				throw new InvalidOperationException("INTERNAL ERROR: the debugger does not look like what System.Xml.dll expects. OnExecute method was not found");
			}
		}

		public void DebugCompile(XPathNavigator style)
		{
			on_compile.Invoke(impl, new object[1] { style.Clone() });
		}

		public void DebugExecute(XslTransformProcessor p, XPathNavigator style)
		{
			on_execute.Invoke(impl, new object[3]
			{
				p.CurrentNodeset.Clone(),
				style.Clone(),
				p.XPathContext
			});
		}
	}
}
