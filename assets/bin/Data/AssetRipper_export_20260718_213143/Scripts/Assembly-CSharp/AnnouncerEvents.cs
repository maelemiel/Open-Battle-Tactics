using System;

public class AnnouncerEvents
{
	public event Action<int> OnSequenceAdvance;

	public event Action<string> OnSequenceAction;

	public event Action OnSequenceExit;

	public void SequenceAdvance(int sequenceIndex)
	{
		if (this.OnSequenceAdvance != null)
		{
			this.OnSequenceAdvance(sequenceIndex);
		}
	}

	public void SequenceAction(string action)
	{
		if (this.OnSequenceAction != null)
		{
			this.OnSequenceAction(action);
		}
	}

	public void SequenceExit()
	{
		if (this.OnSequenceExit != null)
		{
			this.OnSequenceExit();
		}
	}
}
