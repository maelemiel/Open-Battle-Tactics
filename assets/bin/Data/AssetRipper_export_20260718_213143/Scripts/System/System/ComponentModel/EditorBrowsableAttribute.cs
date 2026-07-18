namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate)]
	public sealed class EditorBrowsableAttribute : Attribute
	{
		private EditorBrowsableState state;

		public EditorBrowsableState State
		{
			get
			{
				return state;
			}
		}

		public EditorBrowsableAttribute()
		{
			state = EditorBrowsableState.Always;
		}

		public EditorBrowsableAttribute(EditorBrowsableState state)
		{
			this.state = state;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is EditorBrowsableAttribute))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((EditorBrowsableAttribute)obj).State == state;
		}

		public override int GetHashCode()
		{
			return state.GetHashCode();
		}
	}
}
