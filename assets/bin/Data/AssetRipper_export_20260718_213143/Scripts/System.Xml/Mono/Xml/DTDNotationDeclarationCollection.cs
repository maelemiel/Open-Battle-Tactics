using System;

namespace Mono.Xml
{
	internal class DTDNotationDeclarationCollection : DTDCollectionBase
	{
		public DTDNotationDeclaration this[string name]
		{
			get
			{
				return BaseGet(name) as DTDNotationDeclaration;
			}
		}

		public DTDNotationDeclarationCollection(DTDObjectModel root)
			: base(root)
		{
		}

		public void Add(string name, DTDNotationDeclaration decl)
		{
			if (Contains(name))
			{
				throw new InvalidOperationException(string.Format("Notation declaration for {0} was already added.", name));
			}
			decl.SetRoot(base.Root);
			BaseAdd(name, decl);
		}
	}
}
