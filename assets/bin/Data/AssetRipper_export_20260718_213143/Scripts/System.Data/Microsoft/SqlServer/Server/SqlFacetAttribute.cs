using System;

namespace Microsoft.SqlServer.Server
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
	public class SqlFacetAttribute : Attribute
	{
		private bool isFixedLength;

		private bool isNullable;

		private int maxSize;

		private int precision;

		private int scale;

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

		public bool IsNullable
		{
			get
			{
				return isNullable;
			}
			set
			{
				isNullable = value;
			}
		}

		public int MaxSize
		{
			get
			{
				return maxSize;
			}
			set
			{
				maxSize = value;
			}
		}

		public int Precision
		{
			get
			{
				return precision;
			}
			set
			{
				precision = value;
			}
		}

		public int Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
			}
		}

		public SqlFacetAttribute()
		{
			isFixedLength = false;
			isNullable = false;
			maxSize = 0;
			precision = 0;
			scale = 0;
		}
	}
}
