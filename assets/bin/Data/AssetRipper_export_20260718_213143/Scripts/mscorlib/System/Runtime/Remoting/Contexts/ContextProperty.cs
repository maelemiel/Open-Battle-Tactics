using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	public class ContextProperty
	{
		private string name;

		private object prop;

		public virtual string Name
		{
			get
			{
				return name;
			}
		}

		public virtual object Property
		{
			get
			{
				return prop;
			}
		}

		private ContextProperty(string name, object prop)
		{
			this.name = name;
			this.prop = prop;
		}
	}
}
