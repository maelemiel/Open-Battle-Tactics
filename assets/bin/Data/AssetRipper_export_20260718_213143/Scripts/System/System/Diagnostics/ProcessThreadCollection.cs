namespace System.Diagnostics
{
	public class ProcessThreadCollection : ProcessThreadCollectionBase
	{
		public new ProcessThread this[int index]
		{
			get
			{
				return base.InnerList[index];
			}
		}

		protected ProcessThreadCollection()
		{
		}

		public ProcessThreadCollection(ProcessThread[] processThreads)
		{
			base.InnerList.AddRange(processThreads);
		}

		internal static ProcessThreadCollection GetEmpty()
		{
			return new ProcessThreadCollection();
		}

		public new int Add(ProcessThread thread)
		{
			return base.InnerList.Add(thread);
		}

		public new bool Contains(ProcessThread thread)
		{
			return base.InnerList.Contains(thread);
		}

		public new void CopyTo(ProcessThread[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}

		public new int IndexOf(ProcessThread thread)
		{
			return base.InnerList.IndexOf(thread);
		}

		public new void Insert(int index, ProcessThread thread)
		{
			base.InnerList.Insert(index, thread);
		}

		public new void Remove(ProcessThread thread)
		{
			base.InnerList.Remove(thread);
		}
	}
}
