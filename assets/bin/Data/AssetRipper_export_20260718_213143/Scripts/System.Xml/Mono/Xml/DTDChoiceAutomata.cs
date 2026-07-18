namespace Mono.Xml
{
	internal class DTDChoiceAutomata : DTDAutomata
	{
		private DTDAutomata left;

		private DTDAutomata right;

		private bool hasComputedEmptiable;

		private bool cachedEmptiable;

		public DTDAutomata Left
		{
			get
			{
				return left;
			}
		}

		public DTDAutomata Right
		{
			get
			{
				return right;
			}
		}

		public override bool Emptiable
		{
			get
			{
				if (!hasComputedEmptiable)
				{
					cachedEmptiable = left.Emptiable || right.Emptiable;
					hasComputedEmptiable = true;
				}
				return cachedEmptiable;
			}
		}

		public DTDChoiceAutomata(DTDObjectModel root, DTDAutomata left, DTDAutomata right)
			: base(root)
		{
			this.left = left;
			this.right = right;
		}

		public override DTDAutomata TryStartElement(string name)
		{
			return left.TryStartElement(name).MakeChoice(right.TryStartElement(name));
		}

		public override DTDAutomata TryEndElement()
		{
			return left.TryEndElement().MakeChoice(right.TryEndElement());
		}
	}
}
