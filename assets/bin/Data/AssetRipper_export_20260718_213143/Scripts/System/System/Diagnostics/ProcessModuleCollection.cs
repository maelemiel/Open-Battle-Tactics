namespace System.Diagnostics
{
	public class ProcessModuleCollection : ProcessModuleCollectionBase
	{
		public new ProcessModule this[int index]
		{
			get
			{
				return base.InnerList[index];
			}
		}

		protected ProcessModuleCollection()
		{
		}

		public ProcessModuleCollection(ProcessModule[] processModules)
		{
			base.InnerList.AddRange(processModules);
		}

		public new bool Contains(ProcessModule module)
		{
			return base.InnerList.Contains(module);
		}

		public new void CopyTo(ProcessModule[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}

		public new int IndexOf(ProcessModule module)
		{
			return base.InnerList.IndexOf(module);
		}
	}
}
