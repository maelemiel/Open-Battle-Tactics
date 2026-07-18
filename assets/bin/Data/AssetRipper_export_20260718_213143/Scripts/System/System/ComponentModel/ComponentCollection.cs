using System.Collections;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public class ComponentCollection : ReadOnlyCollectionBase
	{
		public virtual IComponent this[int index]
		{
			get
			{
				return (IComponent)base.InnerList[index];
			}
		}

		public virtual IComponent this[string name]
		{
			get
			{
				foreach (IComponent inner in base.InnerList)
				{
					if (inner.Site != null && inner.Site.Name == name)
					{
						return inner;
					}
				}
				return null;
			}
		}

		public ComponentCollection(IComponent[] components)
		{
			base.InnerList.AddRange(components);
		}

		public void CopyTo(IComponent[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}
	}
}
