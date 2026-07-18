using System.Text;

namespace System.Reflection.Emit
{
	internal class ArrayType : DerivedType
	{
		private int rank;

		public override Type BaseType
		{
			get
			{
				return typeof(Array);
			}
		}

		internal ArrayType(Type elementType, int rank)
			: base(elementType)
		{
			this.rank = rank;
		}

		protected override bool IsArrayImpl()
		{
			return true;
		}

		public override int GetArrayRank()
		{
			return (rank == 0) ? 1 : rank;
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			if (((ModuleBuilder)elementType.Module).assemblyb.IsCompilerContext)
			{
				return (elementType.Attributes & TypeAttributes.VisibilityMask) | TypeAttributes.Sealed | TypeAttributes.Serializable;
			}
			return elementType.Attributes;
		}

		internal override string FormatName(string elementName)
		{
			if (elementName == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(elementName);
			stringBuilder.Append("[");
			for (int i = 1; i < rank; i++)
			{
				stringBuilder.Append(",");
			}
			if (rank == 1)
			{
				stringBuilder.Append("*");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}
}
