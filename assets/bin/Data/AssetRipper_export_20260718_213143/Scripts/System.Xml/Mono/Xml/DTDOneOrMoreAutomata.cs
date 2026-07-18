namespace Mono.Xml
{
	internal class DTDOneOrMoreAutomata : DTDAutomata
	{
		private DTDAutomata children;

		public DTDAutomata Children
		{
			get
			{
				return children;
			}
		}

		public DTDOneOrMoreAutomata(DTDObjectModel root, DTDAutomata children)
			: base(root)
		{
			this.children = children;
		}

		public override DTDAutomata TryStartElement(string name)
		{
			DTDAutomata dTDAutomata = children.TryStartElement(name);
			if (dTDAutomata != base.Root.Invalid)
			{
				return dTDAutomata.MakeSequence(base.Root.Empty.MakeChoice(this));
			}
			return base.Root.Invalid;
		}

		public override DTDAutomata TryEndElement()
		{
			return (!Emptiable) ? base.Root.Invalid : children.TryEndElement();
		}
	}
}
