namespace Mono.Xml
{
	internal abstract class DTDAutomata
	{
		private DTDObjectModel root;

		public DTDObjectModel Root
		{
			get
			{
				return root;
			}
		}

		public virtual bool Emptiable
		{
			get
			{
				return false;
			}
		}

		public DTDAutomata(DTDObjectModel root)
		{
			this.root = root;
		}

		public DTDAutomata MakeChoice(DTDAutomata other)
		{
			if (this == Root.Invalid)
			{
				return other;
			}
			if (other == Root.Invalid)
			{
				return this;
			}
			if (this == Root.Empty && other == Root.Empty)
			{
				return this;
			}
			if (this == Root.Any && other == Root.Any)
			{
				return this;
			}
			if (other == Root.Empty)
			{
				return Root.Factory.Choice(other, this);
			}
			return Root.Factory.Choice(this, other);
		}

		public DTDAutomata MakeSequence(DTDAutomata other)
		{
			if (this == Root.Invalid || other == Root.Invalid)
			{
				return Root.Invalid;
			}
			if (this == Root.Empty)
			{
				return other;
			}
			if (other == Root.Empty)
			{
				return this;
			}
			return Root.Factory.Sequence(this, other);
		}

		public abstract DTDAutomata TryStartElement(string name);

		public virtual DTDAutomata TryEndElement()
		{
			return Root.Invalid;
		}
	}
}
