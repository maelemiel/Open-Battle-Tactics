using System.Collections;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class DesignerVerbCollection : CollectionBase
	{
		public DesignerVerb this[int index]
		{
			get
			{
				return (DesignerVerb)base.InnerList[index];
			}
			set
			{
				base.InnerList[index] = value;
			}
		}

		public DesignerVerbCollection()
		{
		}

		public DesignerVerbCollection(DesignerVerb[] value)
		{
			base.InnerList.AddRange(value);
		}

		public int Add(DesignerVerb value)
		{
			return base.InnerList.Add(value);
		}

		public void AddRange(DesignerVerb[] value)
		{
			base.InnerList.AddRange(value);
		}

		public void AddRange(DesignerVerbCollection value)
		{
			base.InnerList.AddRange(value);
		}

		public bool Contains(DesignerVerb value)
		{
			return base.InnerList.Contains(value);
		}

		public void CopyTo(DesignerVerb[] array, int index)
		{
			base.InnerList.CopyTo(array, index);
		}

		public int IndexOf(DesignerVerb value)
		{
			return base.InnerList.IndexOf(value);
		}

		public void Insert(int index, DesignerVerb value)
		{
			base.InnerList.Insert(index, value);
		}

		protected override void OnClear()
		{
		}

		protected override void OnInsert(int index, object value)
		{
		}

		protected override void OnRemove(int index, object value)
		{
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
		}

		protected override void OnValidate(object value)
		{
		}

		public void Remove(DesignerVerb value)
		{
			base.InnerList.Remove(value);
		}
	}
}
