using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public sealed class SqlUserDefinedAggregateAttribute : Attribute
	{
		public const int MaxByteSizeValue = 8000;

		private Format format;

		private bool isInvariantToDuplicates;

		private bool isInvariantToNulls;

		private bool isInvariantToOrder;

		private bool isNullIfEmpty;

		private int maxByteSize;

		public Format Format
		{
			get
			{
				return format;
			}
		}

		public bool IsInvariantToDuplicates
		{
			get
			{
				return isInvariantToDuplicates;
			}
			set
			{
				isInvariantToDuplicates = value;
			}
		}

		public bool IsInvariantToNulls
		{
			get
			{
				return isInvariantToNulls;
			}
			set
			{
				isInvariantToNulls = value;
			}
		}

		public bool IsInvariantToOrder
		{
			get
			{
				return isInvariantToOrder;
			}
			set
			{
				isInvariantToOrder = value;
			}
		}

		public bool IsNullIfEmpty
		{
			get
			{
				return isNullIfEmpty;
			}
			set
			{
				isNullIfEmpty = value;
			}
		}

		public int MaxByteSize
		{
			get
			{
				return maxByteSize;
			}
			set
			{
				maxByteSize = value;
			}
		}

		public SqlUserDefinedAggregateAttribute(Format f)
		{
			format = f;
			IsInvariantToDuplicates = false;
			IsInvariantToNulls = false;
			IsInvariantToOrder = false;
			IsNullIfEmpty = false;
			MaxByteSize = 8000;
		}
	}
}
