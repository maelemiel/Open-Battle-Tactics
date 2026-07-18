namespace Mono.Xml
{
	internal class DTDSequenceAutomata : DTDAutomata
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
					cachedEmptiable = left.Emptiable && right.Emptiable;
					hasComputedEmptiable = true;
				}
				return cachedEmptiable;
			}
		}

		public DTDSequenceAutomata(DTDObjectModel root, DTDAutomata left, DTDAutomata right)
			: base(root)
		{
			this.left = left;
			this.right = right;
		}

		public override DTDAutomata TryStartElement(string name)
		{
			DTDAutomata dTDAutomata = left.TryStartElement(name);
			DTDAutomata dTDAutomata2 = right.TryStartElement(name);
			if (dTDAutomata == base.Root.Invalid)
			{
				return (!left.Emptiable) ? dTDAutomata : dTDAutomata2;
			}
			DTDAutomata dTDAutomata3 = dTDAutomata.MakeSequence(right);
			if (left.Emptiable)
			{
				return dTDAutomata2.MakeChoice(dTDAutomata3);
			}
			return dTDAutomata3;
		}

		public override DTDAutomata TryEndElement()
		{
			return (!left.Emptiable) ? base.Root.Invalid : right;
		}
	}
}
