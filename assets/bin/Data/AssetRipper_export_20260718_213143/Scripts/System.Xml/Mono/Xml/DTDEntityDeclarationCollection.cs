using System;

namespace Mono.Xml
{
	internal class DTDEntityDeclarationCollection : DTDCollectionBase
	{
		public DTDEntityDeclaration this[string name]
		{
			get
			{
				return BaseGet(name) as DTDEntityDeclaration;
			}
		}

		public DTDEntityDeclarationCollection(DTDObjectModel root)
			: base(root)
		{
		}

		public void Add(string name, DTDEntityDeclaration decl)
		{
			if (Contains(name))
			{
				throw new InvalidOperationException(string.Format("Entity declaration for {0} was already added.", name));
			}
			decl.SetRoot(base.Root);
			BaseAdd(name, decl);
		}
	}
}
