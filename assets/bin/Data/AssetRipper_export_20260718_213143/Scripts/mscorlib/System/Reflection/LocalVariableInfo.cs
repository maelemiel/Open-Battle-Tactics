using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public class LocalVariableInfo
	{
		internal Type type;

		internal bool is_pinned;

		internal ushort position;

		public virtual bool IsPinned
		{
			get
			{
				return is_pinned;
			}
		}

		public virtual int LocalIndex
		{
			get
			{
				return position;
			}
		}

		public virtual Type LocalType
		{
			get
			{
				return type;
			}
		}

		internal LocalVariableInfo()
		{
		}

		public override string ToString()
		{
			if (is_pinned)
			{
				return string.Format("{0} ({1}) (pinned)", type, position);
			}
			return string.Format("{0} ({1})", type, position);
		}
	}
}
