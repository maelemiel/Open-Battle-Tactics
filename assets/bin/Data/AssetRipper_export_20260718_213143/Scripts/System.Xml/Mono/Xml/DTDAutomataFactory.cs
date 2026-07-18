using System.Collections;

namespace Mono.Xml
{
	internal class DTDAutomataFactory
	{
		private DTDObjectModel root;

		private Hashtable choiceTable = new Hashtable();

		private Hashtable sequenceTable = new Hashtable();

		public DTDAutomataFactory(DTDObjectModel root)
		{
			this.root = root;
		}

		public DTDChoiceAutomata Choice(DTDAutomata left, DTDAutomata right)
		{
			Hashtable hashtable = choiceTable[left] as Hashtable;
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				choiceTable[left] = hashtable;
			}
			DTDChoiceAutomata dTDChoiceAutomata = hashtable[right] as DTDChoiceAutomata;
			if (dTDChoiceAutomata == null)
			{
				dTDChoiceAutomata = (DTDChoiceAutomata)(hashtable[right] = new DTDChoiceAutomata(root, left, right));
			}
			return dTDChoiceAutomata;
		}

		public DTDSequenceAutomata Sequence(DTDAutomata left, DTDAutomata right)
		{
			Hashtable hashtable = sequenceTable[left] as Hashtable;
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				sequenceTable[left] = hashtable;
			}
			DTDSequenceAutomata dTDSequenceAutomata = hashtable[right] as DTDSequenceAutomata;
			if (dTDSequenceAutomata == null)
			{
				dTDSequenceAutomata = (DTDSequenceAutomata)(hashtable[right] = new DTDSequenceAutomata(root, left, right));
			}
			return dTDSequenceAutomata;
		}
	}
}
