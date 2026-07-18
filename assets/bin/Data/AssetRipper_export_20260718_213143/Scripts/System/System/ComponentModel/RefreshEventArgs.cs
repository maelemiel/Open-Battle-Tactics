namespace System.ComponentModel
{
	public class RefreshEventArgs : EventArgs
	{
		private object component;

		private Type type;

		public object ComponentChanged
		{
			get
			{
				return component;
			}
		}

		public Type TypeChanged
		{
			get
			{
				return type;
			}
		}

		public RefreshEventArgs(object componentChanged)
		{
			if (componentChanged == null)
			{
				throw new ArgumentNullException("componentChanged");
			}
			component = componentChanged;
			type = component.GetType();
		}

		public RefreshEventArgs(Type typeChanged)
		{
			type = typeChanged;
		}
	}
}
