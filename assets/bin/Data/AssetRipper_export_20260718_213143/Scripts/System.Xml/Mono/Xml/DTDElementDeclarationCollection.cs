using System.Xml;

namespace Mono.Xml
{
	internal class DTDElementDeclarationCollection : DTDCollectionBase
	{
		public DTDElementDeclaration this[string name]
		{
			get
			{
				return Get(name);
			}
		}

		public DTDElementDeclarationCollection(DTDObjectModel root)
			: base(root)
		{
		}

		public DTDElementDeclaration Get(string name)
		{
			return BaseGet(name) as DTDElementDeclaration;
		}

		public void Add(string name, DTDElementDeclaration decl)
		{
			if (Contains(name))
			{
				base.Root.AddError(new XmlException(string.Format("Element declaration for {0} was already added.", name), null));
				return;
			}
			decl.SetRoot(base.Root);
			BaseAdd(name, decl);
		}
	}
}
