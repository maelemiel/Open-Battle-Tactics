namespace Mono.Data.Tds
{
	internal static class TdsCollation
	{
		public static int LCID(byte[] collation)
		{
			if (collation == null)
			{
				return -1;
			}
			return collation[0] | (collation[1] << 8) | ((collation[2] & 0xF) << 16);
		}

		public static int CollationFlags(byte[] collation)
		{
			if (collation == null)
			{
				return -1;
			}
			return (collation[2] & 0xF0) | ((collation[3] & 0xF) << 4);
		}

		public static int Version(byte[] collation)
		{
			if (collation == null)
			{
				return -1;
			}
			return collation[3] & 0xF0;
		}

		public static int SortId(byte[] collation)
		{
			if (collation == null)
			{
				return -1;
			}
			return collation[4];
		}
	}
}
