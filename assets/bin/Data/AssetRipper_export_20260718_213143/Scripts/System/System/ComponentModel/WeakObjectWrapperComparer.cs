using System.Collections.Generic;

namespace System.ComponentModel
{
	internal sealed class WeakObjectWrapperComparer : EqualityComparer<System.ComponentModel.WeakObjectWrapper>
	{
		public override bool Equals(System.ComponentModel.WeakObjectWrapper x, System.ComponentModel.WeakObjectWrapper y)
		{
			if (x == null && y == null)
			{
				return false;
			}
			if (x == null || y == null)
			{
				return false;
			}
			WeakReference weak = x.Weak;
			WeakReference weak2 = y.Weak;
			if (!weak.IsAlive && !weak2.IsAlive)
			{
				return false;
			}
			return weak.Target == weak2.Target;
		}

		public override int GetHashCode(System.ComponentModel.WeakObjectWrapper obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.TargetHashCode;
		}
	}
}
