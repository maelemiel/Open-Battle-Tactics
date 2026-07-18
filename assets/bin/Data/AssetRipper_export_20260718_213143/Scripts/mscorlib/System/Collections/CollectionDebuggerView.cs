using System.Diagnostics;

namespace System.Collections
{
	internal sealed class CollectionDebuggerView
	{
		private readonly ICollection c;

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public object[] Items
		{
			get
			{
				object[] array = new object[Math.Min(c.Count, 1024)];
				c.CopyTo(array, 0);
				return array;
			}
		}

		public CollectionDebuggerView(ICollection col)
		{
			c = col;
		}
	}
}
