using System.Collections.ObjectModel;
using System.Text;

namespace System.Net.Mail
{
	public class MailAddressCollection : Collection<MailAddress>
	{
		public void Add(string addresses)
		{
			string[] array = addresses.Split(',');
			foreach (string address in array)
			{
				Add(new MailAddress(address));
			}
		}

		protected override void InsertItem(int index, MailAddress item)
		{
			if (item == null)
			{
				throw new ArgumentNullException();
			}
			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, MailAddress item)
		{
			if (item == null)
			{
				throw new ArgumentNullException();
			}
			base.SetItem(index, item);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < Count; i++)
			{
				if (i > 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(this[i].ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
