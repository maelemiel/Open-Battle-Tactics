namespace Mono.Xml
{
	internal class DTDElementDeclaration : DTDNode
	{
		private DTDObjectModel root;

		private DTDContentModel contentModel;

		private string name;

		private bool isEmpty;

		private bool isAny;

		private bool isMixedContent;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return isEmpty;
			}
			set
			{
				isEmpty = value;
			}
		}

		public bool IsAny
		{
			get
			{
				return isAny;
			}
			set
			{
				isAny = value;
			}
		}

		public bool IsMixedContent
		{
			get
			{
				return isMixedContent;
			}
			set
			{
				isMixedContent = value;
			}
		}

		public DTDContentModel ContentModel
		{
			get
			{
				if (contentModel == null)
				{
					contentModel = new DTDContentModel(root, Name);
				}
				return contentModel;
			}
		}

		public DTDAttListDeclaration Attributes
		{
			get
			{
				return base.Root.AttListDecls[Name];
			}
		}

		internal DTDElementDeclaration(DTDObjectModel root)
		{
			this.root = root;
		}
	}
}
