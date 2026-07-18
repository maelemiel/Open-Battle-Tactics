using System.Collections;

namespace Mono.Xml.Xsl
{
	internal class XslEmptyTemplate : XslTemplate
	{
		private static XslEmptyTemplate instance = new XslEmptyTemplate();

		public static XslTemplate Instance
		{
			get
			{
				return instance;
			}
		}

		private XslEmptyTemplate()
			: base(null)
		{
		}

		public override void Evaluate(XslTransformProcessor p, Hashtable withParams)
		{
		}
	}
}
