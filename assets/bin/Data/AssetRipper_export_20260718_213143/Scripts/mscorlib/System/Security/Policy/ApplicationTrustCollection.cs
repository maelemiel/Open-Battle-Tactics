using System.Collections;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public sealed class ApplicationTrustCollection : IEnumerable, ICollection
	{
		private ArrayList _list;

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public ApplicationTrust this[int index]
		{
			get
			{
				return (ApplicationTrust)_list[index];
			}
		}

		public ApplicationTrust this[string appFullName]
		{
			get
			{
				for (int i = 0; i < _list.Count; i++)
				{
					ApplicationTrust applicationTrust = _list[i] as ApplicationTrust;
					if (applicationTrust.ApplicationIdentity.FullName == appFullName)
					{
						return applicationTrust;
					}
				}
				return null;
			}
		}

		internal ApplicationTrustCollection()
		{
			_list = new ArrayList();
		}

		void ICollection.CopyTo(Array array, int index)
		{
			_list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new ApplicationTrustEnumerator(this);
		}

		public int Add(ApplicationTrust trust)
		{
			if (trust == null)
			{
				throw new ArgumentNullException("trust");
			}
			if (trust.ApplicationIdentity == null)
			{
				throw new ArgumentException(Locale.GetText("ApplicationTrust.ApplicationIdentity can't be null."), "trust");
			}
			return _list.Add(trust);
		}

		public void AddRange(ApplicationTrust[] trusts)
		{
			if (trusts == null)
			{
				throw new ArgumentNullException("trusts");
			}
			foreach (ApplicationTrust applicationTrust in trusts)
			{
				if (applicationTrust.ApplicationIdentity == null)
				{
					throw new ArgumentException(Locale.GetText("ApplicationTrust.ApplicationIdentity can't be null."), "trust");
				}
				_list.Add(applicationTrust);
			}
		}

		public void AddRange(ApplicationTrustCollection trusts)
		{
			if (trusts == null)
			{
				throw new ArgumentNullException("trusts");
			}
			ApplicationTrustEnumerator enumerator = trusts.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ApplicationTrust current = enumerator.Current;
				if (current.ApplicationIdentity == null)
				{
					throw new ArgumentException(Locale.GetText("ApplicationTrust.ApplicationIdentity can't be null."), "trust");
				}
				_list.Add(current);
			}
		}

		public void Clear()
		{
			_list.Clear();
		}

		public void CopyTo(ApplicationTrust[] array, int index)
		{
			_list.CopyTo(array, index);
		}

		public ApplicationTrustCollection Find(ApplicationIdentity applicationIdentity, ApplicationVersionMatch versionMatch)
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			string text = applicationIdentity.FullName;
			switch (versionMatch)
			{
			case ApplicationVersionMatch.MatchAllVersions:
			{
				int num = text.IndexOf(", Version=");
				if (num >= 0)
				{
					text = text.Substring(0, num);
				}
				break;
			}
			default:
				throw new ArgumentException("versionMatch");
			case ApplicationVersionMatch.MatchExactVersion:
				break;
			}
			ApplicationTrustCollection applicationTrustCollection = new ApplicationTrustCollection();
			foreach (ApplicationTrust item in _list)
			{
				if (item.ApplicationIdentity.FullName.StartsWith(text))
				{
					applicationTrustCollection.Add(item);
				}
			}
			return applicationTrustCollection;
		}

		public ApplicationTrustEnumerator GetEnumerator()
		{
			return new ApplicationTrustEnumerator(this);
		}

		public void Remove(ApplicationTrust trust)
		{
			if (trust == null)
			{
				throw new ArgumentNullException("trust");
			}
			if (trust.ApplicationIdentity == null)
			{
				throw new ArgumentException(Locale.GetText("ApplicationTrust.ApplicationIdentity can't be null."), "trust");
			}
			RemoveAllInstances(trust);
		}

		public void Remove(ApplicationIdentity applicationIdentity, ApplicationVersionMatch versionMatch)
		{
			ApplicationTrustCollection applicationTrustCollection = Find(applicationIdentity, versionMatch);
			ApplicationTrustEnumerator enumerator = applicationTrustCollection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ApplicationTrust current = enumerator.Current;
				RemoveAllInstances(current);
			}
		}

		public void RemoveRange(ApplicationTrust[] trusts)
		{
			if (trusts == null)
			{
				throw new ArgumentNullException("trusts");
			}
			foreach (ApplicationTrust trust in trusts)
			{
				RemoveAllInstances(trust);
			}
		}

		public void RemoveRange(ApplicationTrustCollection trusts)
		{
			if (trusts == null)
			{
				throw new ArgumentNullException("trusts");
			}
			ApplicationTrustEnumerator enumerator = trusts.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ApplicationTrust current = enumerator.Current;
				RemoveAllInstances(current);
			}
		}

		internal void RemoveAllInstances(ApplicationTrust trust)
		{
			for (int num = _list.Count - 1; num >= 0; num--)
			{
				if (trust.Equals(_list[num]))
				{
					_list.RemoveAt(num);
				}
			}
		}
	}
}
