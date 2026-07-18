using System;
using System.Collections.Generic;

namespace Spine
{
	public class Skin
	{
		private Dictionary<KeyValuePair<int, string>, Attachment> attachments = new Dictionary<KeyValuePair<int, string>, Attachment>();

		public string Name { get; private set; }

		public Skin(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name cannot be null.");
			}
			Name = name;
		}

		public void AddAttachment(int slotIndex, string name, Attachment attachment)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment cannot be null.");
			}
			attachments.Add(new KeyValuePair<int, string>(slotIndex, name), attachment);
		}

		public Attachment GetAttachment(int slotIndex, string name)
		{
			Attachment value;
			attachments.TryGetValue(new KeyValuePair<int, string>(slotIndex, name), out value);
			return value;
		}

		public void FindNamesForSlot(int slotIndex, List<string> names)
		{
			if (names == null)
			{
				throw new ArgumentNullException("names cannot be null.");
			}
			foreach (KeyValuePair<int, string> key in attachments.Keys)
			{
				if (key.Key == slotIndex)
				{
					names.Add(key.Value);
				}
			}
		}

		public void FindAttachmentsForSlot(int slotIndex, List<Attachment> attachments)
		{
			if (attachments == null)
			{
				throw new ArgumentNullException("attachments cannot be null.");
			}
			foreach (KeyValuePair<KeyValuePair<int, string>, Attachment> attachment in this.attachments)
			{
				if (attachment.Key.Key == slotIndex)
				{
					attachments.Add(attachment.Value);
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}

		internal void AttachAll(Skeleton skeleton, Skin oldSkin)
		{
			foreach (KeyValuePair<KeyValuePair<int, string>, Attachment> attachment2 in oldSkin.attachments)
			{
				int key = attachment2.Key.Key;
				Slot slot = skeleton.Slots[key];
				if (slot.Attachment == attachment2.Value)
				{
					Attachment attachment = GetAttachment(key, attachment2.Key.Value);
					if (attachment != null)
					{
						slot.Attachment = attachment;
					}
				}
			}
		}
	}
}
