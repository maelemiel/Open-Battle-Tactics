using System.Collections;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class KeyContainerPermissionAccessEntryEnumerator : IEnumerator
	{
		private IEnumerator e;

		object IEnumerator.Current
		{
			get
			{
				return e.Current;
			}
		}

		public KeyContainerPermissionAccessEntry Current
		{
			get
			{
				return (KeyContainerPermissionAccessEntry)e.Current;
			}
		}

		internal KeyContainerPermissionAccessEntryEnumerator(ArrayList list)
		{
			e = list.GetEnumerator();
		}

		public bool MoveNext()
		{
			return e.MoveNext();
		}

		public void Reset()
		{
			e.Reset();
		}
	}
}
