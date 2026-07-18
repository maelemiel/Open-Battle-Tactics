using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[Serializable]
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	public sealed class Evidence : IEnumerable, ICollection
	{
		private class EvidenceEnumerator : IEnumerator
		{
			private IEnumerator currentEnum;

			private IEnumerator hostEnum;

			private IEnumerator assemblyEnum;

			public object Current
			{
				get
				{
					return currentEnum.Current;
				}
			}

			public EvidenceEnumerator(IEnumerator hostenum, IEnumerator assemblyenum)
			{
				hostEnum = hostenum;
				assemblyEnum = assemblyenum;
				currentEnum = hostEnum;
			}

			public bool MoveNext()
			{
				if (currentEnum == null)
				{
					return false;
				}
				bool flag = currentEnum.MoveNext();
				if (!flag && hostEnum == currentEnum && assemblyEnum != null)
				{
					currentEnum = assemblyEnum;
					flag = assemblyEnum.MoveNext();
				}
				return flag;
			}

			public void Reset()
			{
				if (hostEnum != null)
				{
					hostEnum.Reset();
					currentEnum = hostEnum;
				}
				else
				{
					currentEnum = assemblyEnum;
				}
				if (assemblyEnum != null)
				{
					assemblyEnum.Reset();
				}
			}
		}

		private bool _locked;

		private ArrayList hostEvidenceList;

		private ArrayList assemblyEvidenceList;

		private int _hashCode;

		public int Count
		{
			get
			{
				int num = 0;
				if (hostEvidenceList != null)
				{
					num += hostEvidenceList.Count;
				}
				if (assemblyEvidenceList != null)
				{
					num += assemblyEvidenceList.Count;
				}
				return num;
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

		public bool Locked
		{
			get
			{
				return _locked;
			}
			set
			{
				_locked = value;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		internal ArrayList HostEvidenceList
		{
			get
			{
				if (hostEvidenceList == null)
				{
					hostEvidenceList = ArrayList.Synchronized(new ArrayList());
				}
				return hostEvidenceList;
			}
		}

		internal ArrayList AssemblyEvidenceList
		{
			get
			{
				if (assemblyEvidenceList == null)
				{
					assemblyEvidenceList = ArrayList.Synchronized(new ArrayList());
				}
				return assemblyEvidenceList;
			}
		}

		public Evidence()
		{
		}

		public Evidence(Evidence evidence)
		{
			if (evidence != null)
			{
				Merge(evidence);
			}
		}

		public Evidence(object[] hostEvidence, object[] assemblyEvidence)
		{
			if (hostEvidence != null)
			{
				HostEvidenceList.AddRange(hostEvidence);
			}
			if (assemblyEvidence != null)
			{
				AssemblyEvidenceList.AddRange(assemblyEvidence);
			}
		}

		public void AddAssembly(object id)
		{
			AssemblyEvidenceList.Add(id);
			_hashCode = 0;
		}

		public void AddHost(object id)
		{
			if (_locked && SecurityManager.SecurityEnabled)
			{
				new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Demand();
			}
			HostEvidenceList.Add(id);
			_hashCode = 0;
		}

		[ComVisible(false)]
		public void Clear()
		{
			if (hostEvidenceList != null)
			{
				hostEvidenceList.Clear();
			}
			if (assemblyEvidenceList != null)
			{
				assemblyEvidenceList.Clear();
			}
			_hashCode = 0;
		}

		public void CopyTo(Array array, int index)
		{
			int num = 0;
			if (hostEvidenceList != null)
			{
				num = hostEvidenceList.Count;
				if (num > 0)
				{
					hostEvidenceList.CopyTo(array, index);
				}
			}
			if (assemblyEvidenceList != null && assemblyEvidenceList.Count > 0)
			{
				assemblyEvidenceList.CopyTo(array, index + num);
			}
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			Evidence evidence = obj as Evidence;
			if (evidence == null)
			{
				return false;
			}
			if (HostEvidenceList.Count != evidence.HostEvidenceList.Count)
			{
				return false;
			}
			if (AssemblyEvidenceList.Count != evidence.AssemblyEvidenceList.Count)
			{
				return false;
			}
			for (int i = 0; i < hostEvidenceList.Count; i++)
			{
				bool flag = false;
				int num = 0;
				while (num < evidence.hostEvidenceList.Count)
				{
					if (hostEvidenceList[i].Equals(evidence.hostEvidenceList[num]))
					{
						flag = true;
						break;
					}
					i++;
				}
				if (!flag)
				{
					return false;
				}
			}
			for (int j = 0; j < assemblyEvidenceList.Count; j++)
			{
				bool flag2 = false;
				int num2 = 0;
				while (num2 < evidence.assemblyEvidenceList.Count)
				{
					if (assemblyEvidenceList[j].Equals(evidence.assemblyEvidenceList[num2]))
					{
						flag2 = true;
						break;
					}
					j++;
				}
				if (!flag2)
				{
					return false;
				}
			}
			return true;
		}

		public IEnumerator GetEnumerator()
		{
			IEnumerator hostenum = null;
			if (hostEvidenceList != null)
			{
				hostenum = hostEvidenceList.GetEnumerator();
			}
			IEnumerator assemblyenum = null;
			if (assemblyEvidenceList != null)
			{
				assemblyenum = assemblyEvidenceList.GetEnumerator();
			}
			return new EvidenceEnumerator(hostenum, assemblyenum);
		}

		public IEnumerator GetAssemblyEnumerator()
		{
			return AssemblyEvidenceList.GetEnumerator();
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			if (_hashCode == 0)
			{
				if (hostEvidenceList != null)
				{
					for (int i = 0; i < hostEvidenceList.Count; i++)
					{
						_hashCode ^= hostEvidenceList[i].GetHashCode();
					}
				}
				if (assemblyEvidenceList != null)
				{
					for (int j = 0; j < assemblyEvidenceList.Count; j++)
					{
						_hashCode ^= assemblyEvidenceList[j].GetHashCode();
					}
				}
			}
			return _hashCode;
		}

		public IEnumerator GetHostEnumerator()
		{
			return HostEvidenceList.GetEnumerator();
		}

		public void Merge(Evidence evidence)
		{
			if (evidence == null || evidence.Count <= 0)
			{
				return;
			}
			if (evidence.hostEvidenceList != null)
			{
				foreach (object hostEvidence in evidence.hostEvidenceList)
				{
					AddHost(hostEvidence);
				}
			}
			if (evidence.assemblyEvidenceList != null)
			{
				foreach (object assemblyEvidence in evidence.assemblyEvidenceList)
				{
					AddAssembly(assemblyEvidence);
				}
			}
			_hashCode = 0;
		}

		[ComVisible(false)]
		public void RemoveType(Type t)
		{
			for (int num = hostEvidenceList.Count; num >= 0; num--)
			{
				if (hostEvidenceList.GetType() == t)
				{
					hostEvidenceList.RemoveAt(num);
					_hashCode = 0;
				}
			}
			for (int num2 = assemblyEvidenceList.Count; num2 >= 0; num2--)
			{
				if (assemblyEvidenceList.GetType() == t)
				{
					assemblyEvidenceList.RemoveAt(num2);
					_hashCode = 0;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAuthenticodePresent(Assembly a);

		internal static Evidence GetDefaultHostEvidence(Assembly a)
		{
			return new Evidence();
		}
	}
}
