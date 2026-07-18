using System;

namespace Mono.Xml
{
	internal class DTDContentModel : DTDNode
	{
		private DTDObjectModel root;

		private DTDAutomata compiledAutomata;

		private string ownerElementName;

		private string elementName;

		private DTDContentOrderType orderType;

		private DTDContentModelCollection childModels = new DTDContentModelCollection();

		private DTDOccurence occurence;

		public DTDContentModelCollection ChildModels
		{
			get
			{
				return childModels;
			}
			set
			{
				childModels = value;
			}
		}

		public DTDElementDeclaration ElementDecl
		{
			get
			{
				return root.ElementDecls[ownerElementName];
			}
		}

		public string ElementName
		{
			get
			{
				return elementName;
			}
			set
			{
				elementName = value;
			}
		}

		public DTDOccurence Occurence
		{
			get
			{
				return occurence;
			}
			set
			{
				occurence = value;
			}
		}

		public DTDContentOrderType OrderType
		{
			get
			{
				return orderType;
			}
			set
			{
				orderType = value;
			}
		}

		internal DTDContentModel(DTDObjectModel root, string ownerElementName)
		{
			this.root = root;
			this.ownerElementName = ownerElementName;
		}

		public DTDAutomata GetAutomata()
		{
			if (compiledAutomata == null)
			{
				Compile();
			}
			return compiledAutomata;
		}

		public DTDAutomata Compile()
		{
			compiledAutomata = CompileInternal();
			return compiledAutomata;
		}

		private DTDAutomata CompileInternal()
		{
			if (ElementDecl.IsAny)
			{
				return root.Any;
			}
			if (ElementDecl.IsEmpty)
			{
				return root.Empty;
			}
			DTDAutomata basicContentAutomata = GetBasicContentAutomata();
			switch (Occurence)
			{
			case DTDOccurence.One:
				return basicContentAutomata;
			case DTDOccurence.Optional:
				return Choice(root.Empty, basicContentAutomata);
			case DTDOccurence.OneOrMore:
				return new DTDOneOrMoreAutomata(root, basicContentAutomata);
			case DTDOccurence.ZeroOrMore:
				return Choice(root.Empty, new DTDOneOrMoreAutomata(root, basicContentAutomata));
			default:
				throw new InvalidOperationException();
			}
		}

		private DTDAutomata GetBasicContentAutomata()
		{
			if (ElementName != null)
			{
				return new DTDElementAutomata(root, ElementName);
			}
			switch (ChildModels.Count)
			{
			case 0:
				return root.Empty;
			case 1:
				return ChildModels[0].GetAutomata();
			default:
			{
				DTDAutomata dTDAutomata = null;
				int count = ChildModels.Count;
				switch (OrderType)
				{
				case DTDContentOrderType.Seq:
				{
					dTDAutomata = Sequence(ChildModels[count - 2].GetAutomata(), ChildModels[count - 1].GetAutomata());
					for (int num2 = count - 2; num2 > 0; num2--)
					{
						dTDAutomata = Sequence(ChildModels[num2 - 1].GetAutomata(), dTDAutomata);
					}
					return dTDAutomata;
				}
				case DTDContentOrderType.Or:
				{
					dTDAutomata = Choice(ChildModels[count - 2].GetAutomata(), ChildModels[count - 1].GetAutomata());
					for (int num = count - 2; num > 0; num--)
					{
						dTDAutomata = Choice(ChildModels[num - 1].GetAutomata(), dTDAutomata);
					}
					return dTDAutomata;
				}
				default:
					throw new InvalidOperationException("Invalid pattern specification");
				}
			}
			}
		}

		private DTDAutomata Sequence(DTDAutomata l, DTDAutomata r)
		{
			return root.Factory.Sequence(l, r);
		}

		private DTDAutomata Choice(DTDAutomata l, DTDAutomata r)
		{
			return l.MakeChoice(r);
		}
	}
}
