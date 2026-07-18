using System;
using System.Collections;
using System.Data;
using System.IO;
using Mono.Data.SqlExpressions.yyParser;
using Mono.Data.SqlExpressions.yydebug;

namespace Mono.Data.SqlExpressions
{
	internal class Parser
	{
		private class YYRules : MarshalByRefObject
		{
			public static string[] yyRule = new string[84]
			{
				"$accept : Expr", "Expr : BoolExpr", "Expr : ArithExpr", "BoolExpr : PAROPEN BoolExpr PARCLOSE", "BoolExpr : BoolExpr AND BoolExpr", "BoolExpr : BoolExpr OR BoolExpr", "BoolExpr : NOT BoolExpr", "BoolExpr : Predicate", "Predicate : CompPredicate", "Predicate : IsPredicate",
				"Predicate : LikePredicate", "Predicate : InPredicate", "CompPredicate : ArithExpr CompOp ArithExpr", "CompOp : EQ", "CompOp : NE", "CompOp : LT", "CompOp : GT", "CompOp : LE", "CompOp : GE", "LE : LT EQ",
				"NE : LT GT", "GE : GT EQ", "ArithExpr : PAROPEN ArithExpr PARCLOSE", "ArithExpr : ArithExpr MUL ArithExpr", "ArithExpr : ArithExpr DIV ArithExpr", "ArithExpr : ArithExpr MOD ArithExpr", "ArithExpr : ArithExpr PLUS ArithExpr", "ArithExpr : ArithExpr MINUS ArithExpr", "ArithExpr : MINUS ArithExpr", "ArithExpr : Function",
				"ArithExpr : Value", "Value : LiteralValue", "Value : SingleColumnValue", "LiteralValue : StringLiteral", "LiteralValue : NumberLiteral", "LiteralValue : DateLiteral", "LiteralValue : BoolLiteral", "BoolLiteral : TRUE", "BoolLiteral : FALSE", "SingleColumnValue : LocalColumnValue",
				"SingleColumnValue : ParentColumnValue", "MultiColumnValue : LocalColumnValue", "MultiColumnValue : ChildColumnValue", "LocalColumnValue : ColumnName", "ParentColumnValue : PARENT DOT ColumnName", "ParentColumnValue : PARENT PAROPEN RelationName PARCLOSE DOT ColumnName", "ChildColumnValue : CHILD DOT ColumnName", "ChildColumnValue : CHILD PAROPEN RelationName PARCLOSE DOT ColumnName", "ColumnName : Identifier", "ColumnName : ColumnName DOT Identifier",
				"RelationName : Identifier", "Function : CalcFunction", "Function : AggFunction", "Function : StringFunction", "AggFunction : AggFunctionName PAROPEN MultiColumnValue PARCLOSE", "AggFunctionName : COUNT", "AggFunctionName : SUM", "AggFunctionName : AVG", "AggFunctionName : MAX", "AggFunctionName : MIN",
				"AggFunctionName : STDEV", "AggFunctionName : VAR", "StringExpr : PAROPEN StringExpr PARCLOSE", "StringExpr : SingleColumnValue", "StringExpr : StringLiteral", "StringExpr : StringFunction", "StringFunction : TRIM PAROPEN StringExpr PARCLOSE", "StringFunction : SUBSTRING PAROPEN StringExpr COMMA ArithExpr COMMA ArithExpr PARCLOSE", "StringFunction : StringExpr PLUS StringExpr", "CalcFunction : IIF PAROPEN Expr COMMA Expr COMMA Expr PARCLOSE",
				"CalcFunction : ISNULL PAROPEN Expr COMMA Expr PARCLOSE", "CalcFunction : LEN PAROPEN Expr PARCLOSE", "CalcFunction : CONVERT PAROPEN Expr COMMA TypeSpecifier PARCLOSE", "TypeSpecifier : StringLiteral", "TypeSpecifier : Identifier", "IsPredicate : ArithExpr IS NULL", "IsPredicate : ArithExpr IS NOT NULL", "LikePredicate : StringExpr LIKE StringExpr", "LikePredicate : StringExpr NOT_LIKE StringExpr", "InPredicate : ArithExpr IN InPredicateValue",
				"InPredicate : ArithExpr NOT_IN InPredicateValue", "InPredicateValue : PAROPEN InValueList PARCLOSE", "InValueList : LiteralValue", "InValueList : InValueList COMMA LiteralValue"
			};

			public static string getRule(int index)
			{
				return yyRule[index];
			}
		}

		private bool cacheAggregationResults;

		private DataRow[] aggregationRows;

		private static int yacc_verbose_flag;

		public TextWriter ErrorOutput = Console.Out;

		public int eof_token;

		internal yyDebug debug;

		protected static int yyFinal;

		protected static string[] yyNames;

		private int yyExpectingState;

		protected int yyMax;

		private static short[] yyLhs;

		private static short[] yyLen;

		private static short[] yyDefRed;

		protected static short[] yyDgoto;

		protected static short[] yySindex;

		protected static short[] yyRindex;

		protected static short[] yyGindex;

		protected static short[] yyTable;

		protected static short[] yyCheck;

		public Parser()
		{
			ErrorOutput = TextWriter.Null;
			cacheAggregationResults = true;
		}

		public Parser(DataRow[] aggregationRows)
		{
			ErrorOutput = TextWriter.Null;
			this.aggregationRows = aggregationRows;
		}

		static Parser()
		{
			yyFinal = 24;
			string[] array = new string[301];
			array[0] = "end-of-file";
			array[257] = "PAROPEN";
			array[258] = "PARCLOSE";
			array[259] = "AND";
			array[260] = "OR";
			array[261] = "NOT";
			array[262] = "TRUE";
			array[263] = "FALSE";
			array[264] = "NULL";
			array[265] = "PARENT";
			array[266] = "CHILD";
			array[267] = "EQ";
			array[268] = "LT";
			array[269] = "GT";
			array[270] = "PLUS";
			array[271] = "MINUS";
			array[272] = "MUL";
			array[273] = "DIV";
			array[274] = "MOD";
			array[275] = "DOT";
			array[276] = "COMMA";
			array[277] = "IS";
			array[278] = "IN";
			array[279] = "NOT_IN";
			array[280] = "LIKE";
			array[281] = "NOT_LIKE";
			array[282] = "COUNT";
			array[283] = "SUM";
			array[284] = "AVG";
			array[285] = "MAX";
			array[286] = "MIN";
			array[287] = "STDEV";
			array[288] = "VAR";
			array[289] = "IIF";
			array[290] = "SUBSTRING";
			array[291] = "ISNULL";
			array[292] = "LEN";
			array[293] = "TRIM";
			array[294] = "CONVERT";
			array[295] = "StringLiteral";
			array[296] = "NumberLiteral";
			array[297] = "DateLiteral";
			array[298] = "Identifier";
			array[299] = "FunctionName";
			array[300] = "UMINUS";
			yyNames = array;
			yyLhs = new short[84]
			{
				-1, 0, 0, 1, 1, 1, 1, 1, 3, 3,
				3, 3, 4, 8, 8, 8, 8, 8, 8, 10,
				9, 11, 2, 2, 2, 2, 2, 2, 2, 2,
				2, 13, 13, 14, 14, 14, 14, 16, 16, 15,
				15, 19, 19, 17, 18, 18, 20, 20, 21, 21,
				22, 12, 12, 12, 24, 26, 26, 26, 26, 26,
				26, 26, 27, 27, 27, 27, 25, 25, 25, 23,
				23, 23, 23, 28, 28, 5, 5, 6, 6, 7,
				7, 29, 30, 30
			};
			yyLen = new short[84]
			{
				2, 1, 1, 3, 3, 3, 2, 1, 1, 1,
				1, 1, 3, 1, 1, 1, 1, 1, 1, 2,
				2, 2, 3, 3, 3, 3, 3, 3, 2, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 1, 1, 3, 6, 3, 6, 1, 3,
				1, 1, 1, 1, 4, 1, 1, 1, 1, 1,
				1, 1, 3, 1, 1, 1, 4, 8, 3, 8,
				6, 4, 6, 1, 1, 3, 4, 3, 3, 3,
				3, 3, 1, 3
			};
			yyDefRed = new short[163]
			{
				0, 0, 0, 37, 38, 0, 0, 55, 56, 57,
				58, 59, 60, 61, 0, 0, 0, 0, 0, 0,
				0, 34, 35, 48, 0, 0, 0, 7, 8, 9,
				10, 11, 29, 30, 31, 0, 36, 39, 40, 0,
				51, 52, 0, 0, 0, 0, 0, 0, 6, 0,
				0, 0, 0, 28, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 13, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 14, 17, 18, 0, 0,
				0, 0, 0, 3, 22, 62, 50, 0, 0, 0,
				0, 0, 0, 64, 63, 65, 0, 0, 0, 0,
				0, 4, 0, 19, 20, 21, 0, 0, 23, 24,
				25, 0, 75, 0, 79, 80, 0, 49, 0, 41,
				0, 42, 68, 0, 0, 0, 0, 0, 0, 71,
				66, 0, 76, 33, 82, 0, 0, 0, 54, 0,
				0, 0, 0, 73, 74, 0, 81, 0, 0, 0,
				0, 0, 0, 70, 72, 83, 0, 0, 0, 0,
				69, 67, 0
			};
			yyDgoto = new short[31]
			{
				24, 25, 26, 27, 28, 29, 30, 31, 74, 75,
				76, 77, 32, 33, 34, 35, 36, 37, 38, 120,
				121, 39, 87, 40, 41, 42, 43, 44, 145, 114,
				135
			};
			yySindex = new short[163]
			{
				-94, -94, -94, 0, 0, -247, -52, 0, 0, 0,
				0, 0, 0, 0, -248, -237, -235, -224, -214, -204,
				0, 0, 0, 0, 0, -200, 262, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, -190,
				0, 0, 0, -166, -257, -112, -136, -243, 0, 262,
				-206, -199, -52, 0, -172, -94, -251, -94, -94, -251,
				-94, -94, -94, 0, -162, -149, -52, -52, -52, -52,
				-52, -187, -137, -137, -52, 0, 0, 0, -170, -266,
				-251, -251, -251, 0, 0, 0, 0, -128, -190, -147,
				-195, -132, -251, 0, 0, 0, -259, -123, -119, -177,
				-121, 0, -103, 0, 0, 0, -122, -122, 0, 0,
				0, -124, 0, -217, 0, 0, -56, 0, -245, 0,
				-101, 0, 0, -172, -172, -117, -94, -52, -94, 0,
				0, -189, 0, 0, 0, -210, -206, -199, 0, -199,
				-116, -157, -96, 0, 0, -93, 0, -217, -92, -190,
				-190, -94, -52, 0, 0, 0, -111, -88, -50, -199,
				0, 0, -190
			};
			short[] array2 = new short[163];
			array2[20] = 1;
			array2[25] = 176;
			array2[26] = 185;
			array2[35] = 25;
			array2[39] = 49;
			array2[42] = 73;
			array2[64] = 188;
			array2[65] = 230;
			array2[88] = 97;
			array2[102] = 184;
			array2[106] = 145;
			array2[107] = 159;
			array2[116] = 149;
			array2[123] = 173;
			array2[124] = 181;
			array2[149] = -86;
			array2[150] = 121;
			array2[162] = -84;
			yyRindex = array2;
			yyGindex = new short[31]
			{
				-39, 3, 34, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, -106, -30, 0, 96, 0, 0,
				0, -49, 42, 0, 0, -25, 0, 2, 0, 106,
				0
			};
			yyTable = new short[542]
			{
				118, 33, 88, 47, 45, 48, 92, 134, 54, 55,
				50, 80, 136, 80, 5, 85, 91, 127, 97, 98,
				56, 100, 57, 81, 82, 32, 94, 80, 51, 94,
				137, 95, 23, 58, 95, 46, 49, 81, 82, 15,
				53, 155, 18, 59, 93, 3, 4, 23, 146, 43,
				94, 94, 94, 60, 90, 95, 95, 95, 96, 61,
				62, 99, 94, 85, 101, 102, 147, 95, 54, 54,
				54, 54, 54, 53, 111, 80, 54, 112, 133, 21,
				22, 130, 122, 123, 124, 78, 89, 140, 149, 142,
				150, 79, 86, 80, 90, 49, 49, 44, 80, 23,
				106, 107, 108, 109, 110, 103, 143, 104, 116, 144,
				162, 84, 157, 66, 67, 68, 69, 70, 105, 152,
				113, 45, 84, 66, 67, 68, 69, 70, 117, 54,
				125, 63, 64, 65, 66, 67, 68, 69, 70, 129,
				132, 71, 72, 73, 126, 26, 83, 61, 62, 12,
				68, 69, 70, 128, 54, 131, 61, 138, 139, 27,
				151, 141, 153, 1, 159, 154, 156, 2, 3, 4,
				160, 5, 46, 77, 47, 119, 1, 6, 148, 115,
				0, 78, 0, 0, 5, 2, 158, 0, 7, 8,
				9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
				19, 20, 21, 22, 23, 52, 0, 0, 161, 0,
				3, 4, 0, 5, 66, 67, 68, 69, 70, 6,
				66, 67, 68, 69, 70, 0, 0, 0, 0, 0,
				7, 8, 9, 10, 11, 12, 13, 14, 15, 16,
				17, 18, 19, 20, 21, 22, 23, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 33,
				33, 33, 0, 0, 0, 0, 0, 0, 33, 33,
				33, 33, 33, 33, 33, 33, 0, 33, 33, 33,
				33, 64, 64, 32, 32, 32, 0, 0, 0, 0,
				0, 0, 32, 32, 32, 32, 32, 32, 32, 32,
				0, 32, 32, 32, 32, 63, 63, 43, 43, 43,
				0, 0, 0, 0, 0, 0, 43, 43, 43, 43,
				43, 43, 43, 43, 0, 43, 43, 43, 43, 43,
				43, 53, 53, 53, 0, 0, 0, 0, 0, 0,
				53, 53, 53, 53, 53, 53, 53, 53, 0, 53,
				53, 53, 53, 65, 65, 44, 44, 44, 0, 0,
				0, 0, 0, 0, 44, 44, 44, 44, 44, 44,
				44, 44, 0, 44, 44, 44, 44, 44, 44, 45,
				45, 45, 0, 0, 0, 0, 0, 0, 45, 45,
				45, 45, 45, 45, 45, 45, 0, 45, 45, 45,
				45, 45, 45, 26, 26, 26, 0, 12, 12, 12,
				0, 0, 26, 26, 26, 26, 26, 27, 27, 27,
				0, 26, 26, 26, 26, 12, 27, 27, 27, 27,
				27, 77, 77, 77, 1, 27, 27, 27, 27, 78,
				78, 78, 5, 2, 5, 15, 0, 0, 0, 77,
				15, 15, 1, 15, 0, 0, 0, 78, 0, 15,
				5, 2, 0, 0, 0, 0, 0, 0, 0, 0,
				15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
				15, 15, 15, 15, 15, 15, 15, 16, 0, 0,
				0, 0, 16, 16, 0, 16, 0, 0, 0, 0,
				0, 16, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 16, 16, 16, 16, 16, 16, 16, 16,
				16, 16, 16, 16, 16, 16, 16, 16, 16, 63,
				64, 65, 66, 67, 68, 69, 70, 0, 0, 71,
				72, 73
			};
			yyCheck = new short[542]
			{
				266, 0, 51, 1, 1, 2, 257, 113, 6, 257,
				257, 270, 257, 270, 265, 258, 55, 276, 57, 58,
				257, 60, 257, 280, 281, 0, 56, 270, 275, 59,
				275, 56, 298, 257, 59, 1, 2, 280, 281, 290,
				6, 147, 293, 257, 295, 262, 263, 298, 258, 0,
				80, 81, 82, 257, 52, 80, 81, 82, 56, 259,
				260, 59, 92, 258, 61, 62, 276, 92, 66, 67,
				68, 69, 70, 0, 261, 270, 74, 264, 295, 296,
				297, 258, 80, 81, 82, 275, 52, 126, 137, 128,
				139, 257, 298, 270, 92, 61, 62, 0, 270, 298,
				66, 67, 68, 69, 70, 267, 295, 269, 74, 298,
				159, 258, 151, 270, 271, 272, 273, 274, 267, 276,
				257, 0, 258, 270, 271, 272, 273, 274, 298, 127,
				258, 267, 268, 269, 270, 271, 272, 273, 274, 258,
				264, 277, 278, 279, 276, 0, 258, 259, 260, 0,
				272, 273, 274, 276, 152, 276, 259, 258, 275, 0,
				276, 127, 258, 257, 275, 258, 258, 261, 262, 263,
				258, 265, 258, 0, 258, 79, 0, 271, 136, 73,
				-1, 0, -1, -1, 0, 0, 152, -1, 282, 283,
				284, 285, 286, 287, 288, 289, 290, 291, 292, 293,
				294, 295, 296, 297, 298, 257, -1, -1, 258, -1,
				262, 263, -1, 265, 270, 271, 272, 273, 274, 271,
				270, 271, 272, 273, 274, -1, -1, -1, -1, -1,
				282, 283, 284, 285, 286, 287, 288, 289, 290, 291,
				292, 293, 294, 295, 296, 297, 298, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, 258,
				259, 260, -1, -1, -1, -1, -1, -1, 267, 268,
				269, 270, 271, 272, 273, 274, -1, 276, 277, 278,
				279, 280, 281, 258, 259, 260, -1, -1, -1, -1,
				-1, -1, 267, 268, 269, 270, 271, 272, 273, 274,
				-1, 276, 277, 278, 279, 280, 281, 258, 259, 260,
				-1, -1, -1, -1, -1, -1, 267, 268, 269, 270,
				271, 272, 273, 274, -1, 276, 277, 278, 279, 280,
				281, 258, 259, 260, -1, -1, -1, -1, -1, -1,
				267, 268, 269, 270, 271, 272, 273, 274, -1, 276,
				277, 278, 279, 280, 281, 258, 259, 260, -1, -1,
				-1, -1, -1, -1, 267, 268, 269, 270, 271, 272,
				273, 274, -1, 276, 277, 278, 279, 280, 281, 258,
				259, 260, -1, -1, -1, -1, -1, -1, 267, 268,
				269, 270, 271, 272, 273, 274, -1, 276, 277, 278,
				279, 280, 281, 258, 259, 260, -1, 258, 259, 260,
				-1, -1, 267, 268, 269, 270, 271, 258, 259, 260,
				-1, 276, 277, 278, 279, 276, 267, 268, 269, 270,
				271, 258, 259, 260, 258, 276, 277, 278, 279, 258,
				259, 260, 258, 258, 260, 257, -1, -1, -1, 276,
				262, 263, 276, 265, -1, -1, -1, 276, -1, 271,
				276, 276, -1, -1, -1, -1, -1, -1, -1, -1,
				282, 283, 284, 285, 286, 287, 288, 289, 290, 291,
				292, 293, 294, 295, 296, 297, 298, 257, -1, -1,
				-1, -1, 262, 263, -1, 265, -1, -1, -1, -1,
				-1, 271, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, 282, 283, 284, 285, 286, 287, 288, 289,
				290, 291, 292, 293, 294, 295, 296, 297, 298, 267,
				268, 269, 270, 271, 272, 273, 274, -1, -1, 277,
				278, 279
			};
			if (Environment.GetEnvironmentVariable("MONO_DEBUG_SQLEXPRESSIONS") != null)
			{
				yacc_verbose_flag = 2;
			}
		}

		public IExpression Compile(string sqlExpr)
		{
			try
			{
				Tokenizer yyLex = new Tokenizer(sqlExpr);
				if (yacc_verbose_flag > 1)
				{
					return (IExpression)yyparse(yyLex, new yyDebugSimple());
				}
				return (IExpression)yyparse(yyLex);
			}
			catch (yyException)
			{
				throw new SyntaxErrorException(string.Format("Expression '{0}' is invalid.", sqlExpr));
			}
		}

		public void yyerror(string message)
		{
			yyerror(message, null);
		}

		public void yyerror(string message, string[] expected)
		{
			if (yacc_verbose_flag > 0 && expected != null && expected.Length > 0)
			{
				ErrorOutput.Write(message + ", expecting");
				for (int i = 0; i < expected.Length; i++)
				{
					ErrorOutput.Write(" " + expected[i]);
				}
				ErrorOutput.WriteLine();
			}
			else
			{
				ErrorOutput.WriteLine(message);
			}
		}

		public static string yyname(int token)
		{
			if (token < 0 || token > yyNames.Length)
			{
				return "[illegal]";
			}
			string result;
			if ((result = yyNames[token]) != null)
			{
				return result;
			}
			return "[unknown]";
		}

		protected int[] yyExpectingTokens(int state)
		{
			int num = 0;
			bool[] array = new bool[yyNames.Length];
			int num2;
			int i;
			if ((num2 = yySindex[state]) != 0)
			{
				for (i = ((num2 < 0) ? (-num2) : 0); i < yyNames.Length && num2 + i < yyTable.Length; i++)
				{
					if (yyCheck[num2 + i] == i && !array[i] && yyNames[i] != null)
					{
						num++;
						array[i] = true;
					}
				}
			}
			if ((num2 = yyRindex[state]) != 0)
			{
				for (i = ((num2 < 0) ? (-num2) : 0); i < yyNames.Length && num2 + i < yyTable.Length; i++)
				{
					if (yyCheck[num2 + i] == i && !array[i] && yyNames[i] != null)
					{
						num++;
						array[i] = true;
					}
				}
			}
			int[] array2 = new int[num];
			num2 = (i = 0);
			while (num2 < num)
			{
				if (array[i])
				{
					array2[num2++] = i;
				}
				i++;
			}
			return array2;
		}

		protected string[] yyExpecting(int state)
		{
			int[] array = yyExpectingTokens(state);
			string[] array2 = new string[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i++] = yyNames[array[i]];
			}
			return array2;
		}

		internal object yyparse(yyInput yyLex, object yyd)
		{
			debug = (yyDebug)yyd;
			return yyparse(yyLex);
		}

		protected object yyDefault(object first)
		{
			return first;
		}

		internal object yyparse(yyInput yyLex)
		{
			if (yyMax <= 0)
			{
				yyMax = 256;
			}
			int num = 0;
			int[] array = new int[yyMax];
			object obj = null;
			object[] array2 = new object[yyMax];
			int num2 = -1;
			int num3 = 0;
			int num4 = 0;
			while (true)
			{
				if (num4 >= array.Length)
				{
					int[] array3 = new int[array.Length + yyMax];
					array.CopyTo(array3, 0);
					array = array3;
					object[] array4 = new object[array2.Length + yyMax];
					array2.CopyTo(array4, 0);
					array2 = array4;
				}
				array[num4] = num;
				array2[num4] = obj;
				if (debug != null)
				{
					debug.push(num, obj);
				}
				while (true)
				{
					int num5;
					if ((num5 = yyDefRed[num]) == 0)
					{
						if (num2 < 0)
						{
							num2 = (yyLex.advance() ? yyLex.token() : 0);
							if (debug != null)
							{
								debug.lex(num, num2, yyname(num2), yyLex.value());
							}
						}
						if ((num5 = yySindex[num]) != 0 && (num5 += num2) >= 0 && num5 < yyTable.Length && yyCheck[num5] == num2)
						{
							if (debug != null)
							{
								debug.shift(num, yyTable[num5], num3 - 1);
							}
							num = yyTable[num5];
							obj = yyLex.value();
							num2 = -1;
							if (num3 > 0)
							{
								num3--;
							}
							break;
						}
						if ((num5 = yyRindex[num]) == 0 || (num5 += num2) < 0 || num5 >= yyTable.Length || yyCheck[num5] != num2)
						{
							switch (num3)
							{
							case 0:
								yyExpectingState = num;
								if (debug != null)
								{
									debug.error("syntax error");
								}
								if (num2 == 0 || num2 == eof_token)
								{
									throw new yyUnexpectedEof();
								}
								break;
							case 1:
							case 2:
								break;
							case 3:
								goto IL_02f5;
							default:
								goto IL_034b;
							}
							num3 = 3;
							while ((num5 = yySindex[array[num4]]) == 0 || (num5 += 256) < 0 || num5 >= yyTable.Length || yyCheck[num5] != 256)
							{
								if (debug != null)
								{
									debug.pop(array[num4]);
								}
								if (--num4 < 0)
								{
									if (debug != null)
									{
										debug.reject();
									}
									throw new yyException("irrecoverable syntax error");
								}
							}
							if (debug != null)
							{
								debug.shift(array[num4], yyTable[num5], 3);
							}
							num = yyTable[num5];
							obj = yyLex.value();
							break;
						}
						num5 = yyTable[num5];
					}
					goto IL_034b;
					IL_02f5:
					if (num2 == 0)
					{
						if (debug != null)
						{
							debug.reject();
						}
						throw new yyException("irrecoverable syntax error at end-of-file");
					}
					if (debug != null)
					{
						debug.discard(num, num2, yyname(num2), yyLex.value());
					}
					num2 = -1;
					continue;
					IL_034b:
					int num6 = num4 + 1 - yyLen[num5];
					if (debug != null)
					{
						debug.reduce(num, array[num6 - 1], num5, YYRules.getRule(num5), yyLen[num5]);
					}
					obj = yyDefault((num6 <= num4) ? array2[num6] : null);
					switch (num5)
					{
					case 3:
						obj = (IExpression)array2[-1 + num4];
						break;
					case 4:
						obj = new BoolOperation(Operation.AND, (IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 5:
						obj = new BoolOperation(Operation.OR, (IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 6:
						obj = new Negation((IExpression)array2[0 + num4]);
						break;
					case 12:
						obj = new Comparison((Operation)(int)array2[-1 + num4], (IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 13:
						obj = Operation.EQ;
						break;
					case 14:
						obj = Operation.NE;
						break;
					case 15:
						obj = Operation.LT;
						break;
					case 16:
						obj = Operation.GT;
						break;
					case 17:
						obj = Operation.LE;
						break;
					case 18:
						obj = Operation.GE;
						break;
					case 22:
						obj = (IExpression)array2[-1 + num4];
						break;
					case 23:
						obj = new ArithmeticOperation(Operation.MUL, (IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 24:
						obj = new ArithmeticOperation(Operation.DIV, (IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 25:
						obj = new ArithmeticOperation(Operation.MOD, (IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 26:
						obj = new ArithmeticOperation(Operation.ADD, (IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 27:
						obj = new ArithmeticOperation(Operation.SUB, (IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 28:
						obj = new Negative((IExpression)array2[0 + num4]);
						break;
					case 33:
						obj = new Literal(array2[0 + num4]);
						break;
					case 34:
						obj = new Literal(array2[0 + num4]);
						break;
					case 35:
						obj = new Literal(array2[0 + num4]);
						break;
					case 37:
						obj = new Literal(true);
						break;
					case 38:
						obj = new Literal(false);
						break;
					case 43:
						obj = new ColumnReference((string)array2[0 + num4]);
						break;
					case 44:
						obj = new ColumnReference(ReferencedTable.Parent, null, (string)array2[0 + num4]);
						break;
					case 45:
						obj = new ColumnReference(ReferencedTable.Parent, (string)array2[-3 + num4], (string)array2[0 + num4]);
						break;
					case 46:
						obj = new ColumnReference(ReferencedTable.Child, null, (string)array2[0 + num4]);
						break;
					case 47:
						obj = new ColumnReference(ReferencedTable.Child, (string)array2[-3 + num4], (string)array2[0 + num4]);
						break;
					case 49:
						obj = (string)array2[-2 + num4] + "." + (string)array2[0 + num4];
						break;
					case 54:
						obj = new Aggregation(cacheAggregationResults, aggregationRows, (AggregationFunction)(int)array2[-3 + num4], (ColumnReference)array2[-1 + num4]);
						break;
					case 55:
						obj = AggregationFunction.Count;
						break;
					case 56:
						obj = AggregationFunction.Sum;
						break;
					case 57:
						obj = AggregationFunction.Avg;
						break;
					case 58:
						obj = AggregationFunction.Max;
						break;
					case 59:
						obj = AggregationFunction.Min;
						break;
					case 60:
						obj = AggregationFunction.StDev;
						break;
					case 61:
						obj = AggregationFunction.Var;
						break;
					case 62:
						obj = (IExpression)array2[-1 + num4];
						break;
					case 64:
						obj = new Literal(array2[0 + num4]);
						break;
					case 66:
						obj = new TrimFunction((IExpression)array2[-1 + num4]);
						break;
					case 67:
						obj = new SubstringFunction((IExpression)array2[-5 + num4], (IExpression)array2[-3 + num4], (IExpression)array2[-1 + num4]);
						break;
					case 68:
						obj = new ConcatFunction((IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 69:
						obj = new IifFunction((IExpression)array2[-5 + num4], (IExpression)array2[-3 + num4], (IExpression)array2[-1 + num4]);
						break;
					case 70:
						obj = new IsNullFunction((IExpression)array2[-3 + num4], (IExpression)array2[-1 + num4]);
						break;
					case 71:
						obj = new LenFunction((IExpression)array2[-1 + num4]);
						break;
					case 72:
						obj = new ConvertFunction((IExpression)array2[-3 + num4], (string)array2[-1 + num4]);
						break;
					case 75:
						obj = new Comparison(Operation.EQ, (IExpression)array2[-2 + num4], new Literal(null));
						break;
					case 76:
						obj = new Comparison(Operation.NE, (IExpression)array2[-3 + num4], new Literal(null));
						break;
					case 77:
						obj = new Like((IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]);
						break;
					case 78:
						obj = new Negation(new Like((IExpression)array2[-2 + num4], (IExpression)array2[0 + num4]));
						break;
					case 79:
						obj = new In((IExpression)array2[-2 + num4], (IList)array2[0 + num4]);
						break;
					case 80:
						obj = new Negation(new In((IExpression)array2[-2 + num4], (IList)array2[0 + num4]));
						break;
					case 81:
						obj = array2[-1 + num4];
						break;
					case 82:
						obj = new ArrayList();
						((IList)obj).Add(array2[0 + num4]);
						break;
					case 83:
						((IList)(obj = array2[-2 + num4])).Add(array2[0 + num4]);
						break;
					}
					num4 -= yyLen[num5];
					num = array[num4];
					int num7 = yyLhs[num5];
					if (num == 0 && num7 == 0)
					{
						if (debug != null)
						{
							debug.shift(0, yyFinal);
						}
						num = yyFinal;
						if (num2 < 0)
						{
							num2 = (yyLex.advance() ? yyLex.token() : 0);
							if (debug != null)
							{
								debug.lex(num, num2, yyname(num2), yyLex.value());
							}
						}
						if (num2 == 0)
						{
							if (debug != null)
							{
								debug.accept(obj);
							}
							return obj;
						}
						break;
					}
					num = (((num5 = yyGindex[num7]) == 0 || (num5 += num) < 0 || num5 >= yyTable.Length || yyCheck[num5] != num) ? yyDgoto[num7] : yyTable[num5]);
					if (debug != null)
					{
						debug.shift(array[num4], num);
					}
					break;
				}
				num4++;
			}
		}
	}
}
