using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class SqlUserDefinedTypeAttribute : Attribute
	{
		private const int MaxByteSizeValue = 8000;

		private Format format;

		private bool isByteOrdered;

		private bool isFixedLength;

		private int maxByteSize;

		public Format Format
		{
			get
			{
				return format;
			}
		}

		public bool IsByteOrdered
		{
			get
			{
				return isByteOrdered;
			}
			set
			{
				isByteOrdered = value;
			}
		}

		public bool IsFixedLength
		{
			get
			{
				return isFixedLength;
			}
			set
			{
				isFixedLength = value;
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

		public SqlUserDefinedTypeAttribute(Format f)
		{
			format = f;
			IsByteOrdered = false;
			IsFixedLength = false;
			MaxByteSize = 8000;
		}
	}
}
