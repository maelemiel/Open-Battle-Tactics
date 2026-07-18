namespace System.ComponentModel
{
	public class CollectionChangeEventArgs : EventArgs
	{
		private CollectionChangeAction changeAction;

		private object theElement;

		public virtual CollectionChangeAction Action
		{
			get
			{
				return changeAction;
			}
		}

		public virtual object Element
		{
			get
			{
				return theElement;
			}
		}

		public CollectionChangeEventArgs(CollectionChangeAction action, object element)
		{
			changeAction = action;
			theElement = element;
		}
	}
}
