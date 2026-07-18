namespace Mono.Xml
{
	internal class DTDElementAutomata : DTDAutomata
	{
		private string name;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public DTDElementAutomata(DTDObjectModel root, string name)
			: base(root)
		{
			this.name = name;
		}

		public override DTDAutomata TryStartElement(string name)
		{
			if (name == Name)
			{
				return base.Root.Empty;
			}
			return base.Root.Invalid;
		}
	}
}
