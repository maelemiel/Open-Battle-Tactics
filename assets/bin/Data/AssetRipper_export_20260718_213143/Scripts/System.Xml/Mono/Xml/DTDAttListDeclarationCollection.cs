namespace Mono.Xml
{
	internal class DTDAttListDeclarationCollection : DTDCollectionBase
	{
		public DTDAttListDeclaration this[string name]
		{
			get
			{
				return BaseGet(name) as DTDAttListDeclaration;
			}
		}

		public DTDAttListDeclarationCollection(DTDObjectModel root)
			: base(root)
		{
		}

		public void Add(string name, DTDAttListDeclaration decl)
		{
			DTDAttListDeclaration dTDAttListDeclaration = this[name];
			if (dTDAttListDeclaration != null)
			{
				foreach (DTDAttributeDefinition definition in decl.Definitions)
				{
					if (decl.Get(definition.Name) == null)
					{
						dTDAttListDeclaration.Add(definition);
					}
				}
				return;
			}
			decl.SetRoot(base.Root);
			BaseAdd(name, decl);
		}
	}
}
