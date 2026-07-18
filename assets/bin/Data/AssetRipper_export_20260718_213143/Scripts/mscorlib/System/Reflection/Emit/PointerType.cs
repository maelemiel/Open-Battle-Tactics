namespace System.Reflection.Emit
{
	internal class PointerType : DerivedType
	{
		public override Type BaseType
		{
			get
			{
				return typeof(Array);
			}
		}

		internal PointerType(Type elementType)
			: base(elementType)
		{
		}

		protected override bool IsPointerImpl()
		{
			return true;
		}

		internal override string FormatName(string elementName)
		{
			if (elementName == null)
			{
				return null;
			}
			return elementName + "*";
		}
	}
}
