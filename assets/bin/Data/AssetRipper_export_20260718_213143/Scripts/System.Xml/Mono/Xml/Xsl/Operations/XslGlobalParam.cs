namespace Mono.Xml.Xsl.Operations
{
	internal class XslGlobalParam : XslGlobalVariable
	{
		public override bool IsParam
		{
			get
			{
				return true;
			}
		}

		public XslGlobalParam(Compiler c)
			: base(c)
		{
		}

		public void Override(XslTransformProcessor p, object paramVal)
		{
			p.globalVariableTable[this] = paramVal;
		}
	}
}
