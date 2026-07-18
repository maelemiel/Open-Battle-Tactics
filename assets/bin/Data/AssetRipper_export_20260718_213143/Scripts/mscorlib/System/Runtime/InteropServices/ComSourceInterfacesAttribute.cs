namespace System.Runtime.InteropServices
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	[ComVisible(true)]
	public sealed class ComSourceInterfacesAttribute : Attribute
	{
		private string internalValue;

		public string Value
		{
			get
			{
				return internalValue;
			}
		}

		public ComSourceInterfacesAttribute(string sourceInterfaces)
		{
			internalValue = sourceInterfaces;
		}

		public ComSourceInterfacesAttribute(Type sourceInterface)
		{
			internalValue = sourceInterface.ToString();
		}

		public ComSourceInterfacesAttribute(Type sourceInterface1, Type sourceInterface2)
		{
			internalValue = sourceInterface1.ToString() + sourceInterface2.ToString();
		}

		public ComSourceInterfacesAttribute(Type sourceInterface1, Type sourceInterface2, Type sourceInterface3)
		{
			internalValue = sourceInterface1.ToString() + sourceInterface2.ToString() + sourceInterface3.ToString();
		}

		public ComSourceInterfacesAttribute(Type sourceInterface1, Type sourceInterface2, Type sourceInterface3, Type sourceInterface4)
		{
			internalValue = sourceInterface1.ToString() + sourceInterface2.ToString() + sourceInterface3.ToString() + sourceInterface4.ToString();
		}
	}
}
