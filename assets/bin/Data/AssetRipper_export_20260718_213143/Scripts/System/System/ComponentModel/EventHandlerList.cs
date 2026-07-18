namespace System.ComponentModel
{
	public sealed class EventHandlerList : IDisposable
	{
		private System.ComponentModel.ListEntry entries;

		private Delegate null_entry;

		public Delegate this[object key]
		{
			get
			{
				if (key == null)
				{
					return null_entry;
				}
				System.ComponentModel.ListEntry listEntry = FindEntry(key);
				if (listEntry != null)
				{
					return listEntry.value;
				}
				return null;
			}
			set
			{
				AddHandler(key, value);
			}
		}

		public void AddHandler(object key, Delegate value)
		{
			if (key == null)
			{
				null_entry = Delegate.Combine(null_entry, value);
				return;
			}
			System.ComponentModel.ListEntry listEntry = FindEntry(key);
			if (listEntry == null)
			{
				listEntry = new System.ComponentModel.ListEntry();
				listEntry.key = key;
				listEntry.value = null;
				listEntry.next = entries;
				entries = listEntry;
			}
			listEntry.value = Delegate.Combine(listEntry.value, value);
		}

		public void AddHandlers(EventHandlerList listToAddFrom)
		{
			if (listToAddFrom != null)
			{
				for (System.ComponentModel.ListEntry next = listToAddFrom.entries; next != null; next = next.next)
				{
					AddHandler(next.key, next.value);
				}
			}
		}

		public void RemoveHandler(object key, Delegate value)
		{
			if (key == null)
			{
				null_entry = Delegate.Remove(null_entry, value);
				return;
			}
			System.ComponentModel.ListEntry listEntry = FindEntry(key);
			if (listEntry != null)
			{
				listEntry.value = Delegate.Remove(listEntry.value, value);
			}
		}

		public void Dispose()
		{
			entries = null;
		}

		private System.ComponentModel.ListEntry FindEntry(object key)
		{
			for (System.ComponentModel.ListEntry next = entries; next != null; next = next.next)
			{
				if (next.key == key)
				{
					return next;
				}
			}
			return null;
		}
	}
}
