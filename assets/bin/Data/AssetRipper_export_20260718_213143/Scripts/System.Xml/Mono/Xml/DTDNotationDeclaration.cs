namespace Mono.Xml
{
	internal class DTDNotationDeclaration : DTDNode
	{
		private string name;

		private string localName;

		private string prefix;

		private string publicId;

		private string systemId;

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

		public string PublicId
		{
			get
			{
				return publicId;
			}
			set
			{
				publicId = value;
			}
		}

		public string SystemId
		{
			get
			{
				return systemId;
			}
			set
			{
				systemId = value;
			}
		}

		public string LocalName
		{
			get
			{
				return localName;
			}
			set
			{
				localName = value;
			}
		}

		public string Prefix
		{
			get
			{
				return prefix;
			}
			set
			{
				prefix = value;
			}
		}

		internal DTDNotationDeclaration(DTDObjectModel root)
		{
			SetRoot(root);
		}
	}
}
