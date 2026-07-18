using System.Collections;
using System.Text;

namespace Mono.Data.Tds
{
	internal static class TdsCharset
	{
		private static Hashtable lcidCodes;

		private static Hashtable sortCodes;

		static TdsCharset()
		{
			lcidCodes = new Hashtable();
			sortCodes = new Hashtable();
			lcidCodes[1078] = 1252;
			lcidCodes[1052] = 1250;
			lcidCodes[1025] = 1256;
			lcidCodes[2049] = 1256;
			lcidCodes[3073] = 1256;
			lcidCodes[4097] = 1256;
			lcidCodes[5121] = 1256;
			lcidCodes[6145] = 1256;
			lcidCodes[7169] = 1256;
			lcidCodes[8193] = 1256;
			lcidCodes[9217] = 1256;
			lcidCodes[10241] = 1256;
			lcidCodes[11265] = 1256;
			lcidCodes[12289] = 1256;
			lcidCodes[13313] = 1256;
			lcidCodes[14337] = 1256;
			lcidCodes[15361] = 1256;
			lcidCodes[16385] = 1256;
			lcidCodes[1069] = 1252;
			lcidCodes[1059] = 1251;
			lcidCodes[1026] = 1251;
			lcidCodes[1027] = 1252;
			lcidCodes[197636] = 950;
			lcidCodes[1028] = 950;
			lcidCodes[2052] = 936;
			lcidCodes[133124] = 936;
			lcidCodes[4100] = 936;
			lcidCodes[1050] = 1250;
			lcidCodes[1029] = 1250;
			lcidCodes[1030] = 1252;
			lcidCodes[1043] = 1252;
			lcidCodes[2067] = 1252;
			lcidCodes[1033] = 1252;
			lcidCodes[2057] = 1252;
			lcidCodes[4105] = 1252;
			lcidCodes[5129] = 1252;
			lcidCodes[3081] = 1252;
			lcidCodes[6153] = 1252;
			lcidCodes[7177] = 1252;
			lcidCodes[9225] = 1252;
			lcidCodes[8201] = 1252;
			lcidCodes[1061] = 1257;
			lcidCodes[1080] = 1252;
			lcidCodes[1065] = 1256;
			lcidCodes[1035] = 1252;
			lcidCodes[1036] = 1252;
			lcidCodes[2060] = 1252;
			lcidCodes[4108] = 1252;
			lcidCodes[3084] = 1252;
			lcidCodes[5132] = 1252;
			lcidCodes[66615] = 1252;
			lcidCodes[66567] = 1252;
			lcidCodes[1031] = 1252;
			lcidCodes[2055] = 1252;
			lcidCodes[3079] = 1252;
			lcidCodes[4103] = 1252;
			lcidCodes[5127] = 1252;
			lcidCodes[1032] = 1253;
			lcidCodes[1037] = 1255;
			lcidCodes[1081] = 65001;
			lcidCodes[1038] = 1250;
			lcidCodes[4174] = 1250;
			lcidCodes[1039] = 1252;
			lcidCodes[1057] = 1252;
			lcidCodes[1040] = 1252;
			lcidCodes[2064] = 1252;
			lcidCodes[1041] = 932;
			lcidCodes[66577] = 932;
			lcidCodes[1042] = 949;
			lcidCodes[1042] = 949;
			lcidCodes[1062] = 1257;
			lcidCodes[1063] = 1257;
			lcidCodes[2087] = 1257;
			lcidCodes[1052] = 1251;
			lcidCodes[1044] = 1252;
			lcidCodes[2068] = 1252;
			lcidCodes[1045] = 1250;
			lcidCodes[2070] = 1252;
			lcidCodes[1046] = 1252;
			lcidCodes[1048] = 1250;
			lcidCodes[1049] = 1251;
			lcidCodes[2074] = 1251;
			lcidCodes[3098] = 1251;
			lcidCodes[1051] = 1250;
			lcidCodes[1060] = 1250;
			lcidCodes[2058] = 1252;
			lcidCodes[1034] = 1252;
			lcidCodes[3082] = 1252;
			lcidCodes[4106] = 1252;
			lcidCodes[5130] = 1252;
			lcidCodes[6154] = 1252;
			lcidCodes[7178] = 1252;
			lcidCodes[8202] = 1252;
			lcidCodes[9226] = 1252;
			lcidCodes[10250] = 1252;
			lcidCodes[11274] = 1252;
			lcidCodes[12298] = 1252;
			lcidCodes[13322] = 1252;
			lcidCodes[14346] = 1252;
			lcidCodes[15370] = 1252;
			lcidCodes[16394] = 1252;
			lcidCodes[1053] = 1252;
			lcidCodes[1054] = 874;
			lcidCodes[1055] = 1254;
			lcidCodes[1058] = 1251;
			lcidCodes[1056] = 1256;
			lcidCodes[1066] = 1258;
			sortCodes[30] = 437;
			sortCodes[31] = 437;
			sortCodes[32] = 437;
			sortCodes[33] = 437;
			sortCodes[34] = 437;
			sortCodes[40] = 850;
			sortCodes[41] = 850;
			sortCodes[42] = 850;
			sortCodes[43] = 850;
			sortCodes[44] = 850;
			sortCodes[49] = 850;
			sortCodes[50] = 1252;
			sortCodes[51] = 1252;
			sortCodes[52] = 1252;
			sortCodes[53] = 1252;
			sortCodes[54] = 1252;
			sortCodes[55] = 850;
			sortCodes[56] = 850;
			sortCodes[57] = 850;
			sortCodes[58] = 850;
			sortCodes[59] = 850;
			sortCodes[60] = 850;
			sortCodes[61] = 850;
			sortCodes[71] = 1252;
			sortCodes[72] = 1252;
			sortCodes[73] = 1252;
			sortCodes[74] = 1252;
			sortCodes[75] = 1252;
			sortCodes[80] = 1250;
			sortCodes[81] = 1250;
			sortCodes[82] = 1250;
			sortCodes[83] = 1250;
			sortCodes[84] = 1250;
			sortCodes[85] = 1250;
			sortCodes[86] = 1250;
			sortCodes[87] = 1250;
			sortCodes[88] = 1250;
			sortCodes[89] = 1250;
			sortCodes[90] = 1250;
			sortCodes[91] = 1250;
			sortCodes[92] = 1250;
			sortCodes[93] = 1250;
			sortCodes[94] = 1250;
			sortCodes[95] = 1250;
			sortCodes[96] = 1250;
			sortCodes[97] = 1250;
			sortCodes[98] = 1250;
			sortCodes[104] = 1251;
			sortCodes[105] = 1251;
			sortCodes[106] = 1251;
			sortCodes[107] = 1251;
			sortCodes[108] = 1251;
			sortCodes[112] = 1253;
			sortCodes[113] = 1253;
			sortCodes[114] = 1253;
			sortCodes[120] = 1253;
			sortCodes[121] = 1253;
			sortCodes[124] = 1253;
			sortCodes[128] = 1254;
			sortCodes[129] = 1254;
			sortCodes[130] = 1254;
			sortCodes[136] = 1255;
			sortCodes[137] = 1255;
			sortCodes[138] = 1255;
			sortCodes[144] = 1256;
			sortCodes[145] = 1256;
			sortCodes[146] = 1256;
			sortCodes[152] = 1257;
			sortCodes[153] = 1257;
			sortCodes[154] = 1257;
			sortCodes[155] = 1257;
			sortCodes[156] = 1257;
			sortCodes[157] = 1257;
			sortCodes[158] = 1257;
			sortCodes[159] = 1257;
			sortCodes[160] = 1257;
			sortCodes[183] = 1252;
			sortCodes[184] = 1252;
			sortCodes[185] = 1252;
			sortCodes[186] = 1252;
			sortCodes[192] = 932;
			sortCodes[193] = 932;
			sortCodes[194] = 949;
			sortCodes[195] = 949;
			sortCodes[196] = 950;
			sortCodes[197] = 950;
			sortCodes[198] = 936;
			sortCodes[199] = 936;
			sortCodes[200] = 932;
			sortCodes[201] = 949;
			sortCodes[202] = 950;
			sortCodes[203] = 936;
			sortCodes[204] = 874;
			sortCodes[205] = 874;
			sortCodes[206] = 874;
		}

		public static Encoding GetEncoding(byte[] collation)
		{
			if (TdsCollation.SortId(collation) != 0)
			{
				return GetEncodingFromSortOrder(collation);
			}
			return GetEncodingFromLCID(collation);
		}

		public static Encoding GetEncodingFromLCID(byte[] collation)
		{
			int lcid = TdsCollation.LCID(collation);
			return GetEncodingFromLCID(lcid);
		}

		public static Encoding GetEncodingFromLCID(int lcid)
		{
			if (lcidCodes[lcid] != null)
			{
				return Encoding.GetEncoding((int)lcidCodes[lcid]);
			}
			return null;
		}

		public static Encoding GetEncodingFromSortOrder(byte[] collation)
		{
			int sortId = TdsCollation.SortId(collation);
			return GetEncodingFromSortOrder(sortId);
		}

		public static Encoding GetEncodingFromSortOrder(int sortId)
		{
			if (sortCodes[sortId] != null)
			{
				return Encoding.GetEncoding((int)sortCodes[sortId]);
			}
			return null;
		}
	}
}
