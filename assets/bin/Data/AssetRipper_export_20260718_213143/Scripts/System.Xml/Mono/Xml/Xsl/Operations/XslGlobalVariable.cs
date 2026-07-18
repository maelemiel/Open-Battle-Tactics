using System.Collections;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslGlobalVariable : XslGeneralVariable
	{
		private static object busyObject = new object();

		public override bool IsLocal
		{
			get
			{
				return false;
			}
		}

		public override bool IsParam
		{
			get
			{
				return false;
			}
		}

		public XslGlobalVariable(Compiler c)
			: base(c)
		{
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			Hashtable globalVariableTable = p.globalVariableTable;
			if (globalVariableTable.Contains(this))
			{
				if (globalVariableTable[this] == busyObject)
				{
					throw new XsltException("Circular dependency was detected", null, p.CurrentNode);
				}
			}
			else
			{
				globalVariableTable[this] = busyObject;
				globalVariableTable[this] = var.Evaluate(p);
			}
		}

		protected override object GetValue(XslTransformProcessor p)
		{
			Evaluate(p);
			return p.globalVariableTable[this];
		}
	}
}
