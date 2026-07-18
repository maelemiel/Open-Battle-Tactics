using System.Collections;
using System.Collections.Generic;

namespace System.Net
{
	public class HttpListenerPrefixCollection : IEnumerable, ICollection<string>, IEnumerable<string>
	{
		private List<string> prefixes = new List<string>();

		private HttpListener listener;

		public int Count
		{
			get
			{
				return prefixes.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		internal HttpListenerPrefixCollection(HttpListener listener)
		{
			this.listener = listener;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return prefixes.GetEnumerator();
		}

		public void Add(string uriPrefix)
		{
			listener.CheckDisposed();
			System.Net.ListenerPrefix.CheckUri(uriPrefix);
			if (!prefixes.Contains(uriPrefix))
			{
				prefixes.Add(uriPrefix);
				if (listener.IsListening)
				{
					System.Net.EndPointManager.AddPrefix(uriPrefix, listener);
				}
			}
		}

		public void Clear()
		{
			listener.CheckDisposed();
			prefixes.Clear();
			if (listener.IsListening)
			{
				System.Net.EndPointManager.RemoveListener(listener);
			}
		}

		public bool Contains(string uriPrefix)
		{
			listener.CheckDisposed();
			return prefixes.Contains(uriPrefix);
		}

		public void CopyTo(string[] array, int offset)
		{
			listener.CheckDisposed();
			prefixes.CopyTo(array, offset);
		}

		public void CopyTo(Array array, int offset)
		{
			listener.CheckDisposed();
			((ICollection)prefixes).CopyTo(array, offset);
		}

		public IEnumerator<string> GetEnumerator()
		{
			return prefixes.GetEnumerator();
		}

		public bool Remove(string uriPrefix)
		{
			listener.CheckDisposed();
			if (uriPrefix == null)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			bool flag = prefixes.Remove(uriPrefix);
			if (flag && listener.IsListening)
			{
				System.Net.EndPointManager.RemovePrefix(uriPrefix, listener);
			}
			return flag;
		}
	}
}
