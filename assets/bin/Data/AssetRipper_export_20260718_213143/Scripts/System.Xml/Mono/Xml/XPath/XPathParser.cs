using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.XPath.yyParser;
using Mono.Xml.XPath.yydebug;

namespace Mono.Xml.XPath
{
	internal class XPathParser
	{
		private class YYRules : MarshalByRefObject
		{
			public static string[] yyRule = new string[104]
			{
				"$accept : Expr", "Pattern : LocationPathPattern", "Pattern : Pattern BAR LocationPathPattern", "LocationPathPattern : SLASH", "LocationPathPattern : SLASH RelativePathPattern", "LocationPathPattern : IdKeyPattern", "LocationPathPattern : IdKeyPattern SLASH RelativePathPattern", "LocationPathPattern : IdKeyPattern SLASH2 RelativePathPattern", "LocationPathPattern : SLASH2 RelativePathPattern", "LocationPathPattern : RelativePathPattern",
				"IdKeyPattern : FUNCTION_NAME PAREN_OPEN LITERAL PAREN_CLOSE", "IdKeyPattern : FUNCTION_NAME PAREN_OPEN LITERAL COMMA LITERAL PAREN_CLOSE", "RelativePathPattern : StepPattern", "RelativePathPattern : RelativePathPattern SLASH StepPattern", "RelativePathPattern : RelativePathPattern SLASH2 StepPattern", "StepPattern : ChildOrAttributeAxisSpecifier NodeTest Predicates", "ChildOrAttributeAxisSpecifier : AbbreviatedAxisSpecifier", "ChildOrAttributeAxisSpecifier : CHILD COLON2", "ChildOrAttributeAxisSpecifier : ATTRIBUTE COLON2", "Predicates :",
				"Predicates : Predicates Predicate", "Expr : OrExpr", "OrExpr : AndExpr", "OrExpr : OrExpr OR AndExpr", "AndExpr : EqualityExpr", "AndExpr : AndExpr AND EqualityExpr", "EqualityExpr : RelationalExpr", "EqualityExpr : EqualityExpr EQ RelationalExpr", "EqualityExpr : EqualityExpr NE RelationalExpr", "RelationalExpr : AdditiveExpr",
				"RelationalExpr : RelationalExpr LT AdditiveExpr", "RelationalExpr : RelationalExpr GT AdditiveExpr", "RelationalExpr : RelationalExpr LE AdditiveExpr", "RelationalExpr : RelationalExpr GE AdditiveExpr", "AdditiveExpr : MultiplicativeExpr", "AdditiveExpr : AdditiveExpr PLUS MultiplicativeExpr", "AdditiveExpr : AdditiveExpr MINUS MultiplicativeExpr", "MultiplicativeExpr : UnaryExpr", "MultiplicativeExpr : MultiplicativeExpr MULTIPLY UnaryExpr", "MultiplicativeExpr : MultiplicativeExpr DIV UnaryExpr",
				"MultiplicativeExpr : MultiplicativeExpr MOD UnaryExpr", "UnaryExpr : UnionExpr", "UnaryExpr : MINUS UnaryExpr", "UnionExpr : PathExpr", "UnionExpr : UnionExpr BAR PathExpr", "PathExpr : LocationPath", "PathExpr : FilterExpr", "PathExpr : FilterExpr SLASH RelativeLocationPath", "PathExpr : FilterExpr SLASH2 RelativeLocationPath", "LocationPath : RelativeLocationPath",
				"LocationPath : AbsoluteLocationPath", "AbsoluteLocationPath : SLASH", "AbsoluteLocationPath : SLASH RelativeLocationPath", "AbsoluteLocationPath : SLASH2 RelativeLocationPath", "RelativeLocationPath : Step", "RelativeLocationPath : RelativeLocationPath SLASH Step", "RelativeLocationPath : RelativeLocationPath SLASH2 Step", "Step : AxisSpecifier NodeTest Predicates", "Step : AbbreviatedStep", "NodeTest : NameTest",
				"NodeTest : NodeType PAREN_OPEN PAREN_CLOSE", "NodeTest : PROCESSING_INSTRUCTION PAREN_OPEN OptionalLiteral PAREN_CLOSE", "NameTest : ASTERISK", "NameTest : QName", "AbbreviatedStep : DOT", "AbbreviatedStep : DOT2", "Predicates :", "Predicates : Predicates Predicate", "AxisSpecifier : AxisName COLON2", "AxisSpecifier : AbbreviatedAxisSpecifier",
				"AbbreviatedAxisSpecifier :", "AbbreviatedAxisSpecifier : AT", "NodeType : COMMENT", "NodeType : TEXT", "NodeType : PROCESSING_INSTRUCTION", "NodeType : NODE", "FilterExpr : PrimaryExpr", "FilterExpr : FilterExpr Predicate", "PrimaryExpr : DOLLAR QName", "PrimaryExpr : PAREN_OPEN Expr PAREN_CLOSE",
				"PrimaryExpr : LITERAL", "PrimaryExpr : NUMBER", "PrimaryExpr : FunctionCall", "FunctionCall : FUNCTION_NAME PAREN_OPEN OptionalArgumentList PAREN_CLOSE", "OptionalArgumentList :", "OptionalArgumentList : Expr OptionalArgumentListTail", "OptionalArgumentListTail :", "OptionalArgumentListTail : COMMA Expr OptionalArgumentListTail", "Predicate : BRACKET_OPEN Expr BRACKET_CLOSE", "AxisName : ANCESTOR",
				"AxisName : ANCESTOR_OR_SELF", "AxisName : ATTRIBUTE", "AxisName : CHILD", "AxisName : DESCENDANT", "AxisName : DESCENDANT_OR_SELF", "AxisName : FOLLOWING", "AxisName : FOLLOWING_SIBLING", "AxisName : NAMESPACE", "AxisName : PARENT", "AxisName : PRECEDING",
				"AxisName : PRECEDING_SIBLING", "AxisName : SELF", "OptionalLiteral :", "OptionalLiteral : LITERAL"
			};

			public static string getRule(int index)
			{
				return yyRule[index];
			}
		}

		internal IStaticXsltContext Context;

		private static int yacc_verbose_flag;

		public TextWriter ErrorOutput = Console.Out;

		public int eof_token;

		internal yyDebug debug;

		protected static int yyFinal = 25;

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

		public XPathParser()
			: this(null)
		{
		}

		internal XPathParser(IStaticXsltContext context)
		{
			Context = context;
			ErrorOutput = TextWriter.Null;
		}

		static XPathParser()
		{
			string[] array = new string[334];
			array[0] = "end-of-file";
			array[36] = "'$'";
			array[40] = "'('";
			array[41] = "')'";
			array[42] = "'*'";
			array[43] = "'+'";
			array[44] = "','";
			array[45] = "'-'";
			array[46] = "'.'";
			array[47] = "'/'";
			array[60] = "'<'";
			array[61] = "'='";
			array[62] = "'>'";
			array[64] = "'@'";
			array[91] = "'['";
			array[93] = "']'";
			array[124] = "'|'";
			array[257] = "ERROR";
			array[258] = "EOF";
			array[259] = "SLASH";
			array[260] = "SLASH2";
			array[261] = "\"//\"";
			array[262] = "DOT";
			array[263] = "DOT2";
			array[264] = "\"..\"";
			array[265] = "COLON2";
			array[266] = "\"::\"";
			array[267] = "COMMA";
			array[268] = "AT";
			array[269] = "FUNCTION_NAME";
			array[270] = "BRACKET_OPEN";
			array[271] = "BRACKET_CLOSE";
			array[272] = "PAREN_OPEN";
			array[273] = "PAREN_CLOSE";
			array[274] = "AND";
			array[275] = "\"and\"";
			array[276] = "OR";
			array[277] = "\"or\"";
			array[278] = "DIV";
			array[279] = "\"div\"";
			array[280] = "MOD";
			array[281] = "\"mod\"";
			array[282] = "PLUS";
			array[283] = "MINUS";
			array[284] = "ASTERISK";
			array[285] = "DOLLAR";
			array[286] = "BAR";
			array[287] = "EQ";
			array[288] = "NE";
			array[289] = "\"!=\"";
			array[290] = "LE";
			array[291] = "\"<=\"";
			array[292] = "GE";
			array[293] = "\">=\"";
			array[294] = "LT";
			array[295] = "GT";
			array[296] = "ANCESTOR";
			array[297] = "\"ancestor\"";
			array[298] = "ANCESTOR_OR_SELF";
			array[299] = "\"ancstor-or-self\"";
			array[300] = "ATTRIBUTE";
			array[301] = "\"attribute\"";
			array[302] = "CHILD";
			array[303] = "\"child\"";
			array[304] = "DESCENDANT";
			array[305] = "\"descendant\"";
			array[306] = "DESCENDANT_OR_SELF";
			array[307] = "\"descendant-or-self\"";
			array[308] = "FOLLOWING";
			array[309] = "\"following\"";
			array[310] = "FOLLOWING_SIBLING";
			array[311] = "\"sibling\"";
			array[312] = "NAMESPACE";
			array[313] = "\"NameSpace\"";
			array[314] = "PARENT";
			array[315] = "\"parent\"";
			array[316] = "PRECEDING";
			array[317] = "\"preceding\"";
			array[318] = "PRECEDING_SIBLING";
			array[319] = "\"preceding-sibling\"";
			array[320] = "SELF";
			array[321] = "\"self\"";
			array[322] = "COMMENT";
			array[323] = "\"comment\"";
			array[324] = "TEXT";
			array[325] = "\"text\"";
			array[326] = "PROCESSING_INSTRUCTION";
			array[327] = "\"processing-instruction\"";
			array[328] = "NODE";
			array[329] = "\"node\"";
			array[330] = "MULTIPLY";
			array[331] = "NUMBER";
			array[332] = "LITERAL";
			array[333] = "QName";
			yyNames = array;
			yyLhs = new short[104]
			{
				-1, 1, 1, 2, 2, 2, 2, 2, 2, 2,
				4, 4, 3, 3, 3, 5, 6, 6, 6, 8,
				8, 0, 11, 11, 12, 12, 13, 13, 13, 14,
				14, 14, 14, 14, 15, 15, 15, 16, 16, 16,
				16, 17, 17, 18, 18, 19, 19, 19, 19, 20,
				20, 23, 23, 23, 22, 22, 22, 24, 24, 7,
				7, 7, 27, 27, 26, 26, 8, 8, 25, 25,
				9, 9, 28, 28, 28, 28, 21, 21, 31, 31,
				31, 31, 31, 32, 33, 33, 34, 34, 10, 30,
				30, 30, 30, 30, 30, 30, 30, 30, 30, 30,
				30, 30, 29, 29
			};
			yyLen = new short[104]
			{
				2, 1, 3, 1, 2, 1, 3, 3, 2, 1,
				4, 6, 1, 3, 3, 3, 1, 2, 2, 0,
				2, 1, 1, 3, 1, 3, 1, 3, 3, 1,
				3, 3, 3, 3, 1, 3, 3, 1, 3, 3,
				3, 1, 2, 1, 3, 1, 1, 3, 3, 1,
				1, 1, 2, 2, 1, 3, 3, 3, 1, 1,
				3, 4, 1, 1, 1, 1, 0, 2, 2, 1,
				0, 1, 1, 1, 1, 1, 1, 2, 2, 3,
				1, 1, 1, 4, 0, 2, 0, 3, 3, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 1, 0, 1
			};
			yyDefRed = new short[118]
			{
				0, 0, 0, 64, 65, 71, 0, 0, 0, 0,
				89, 90, 91, 92, 93, 94, 95, 96, 97, 98,
				99, 100, 101, 81, 80, 0, 69, 0, 0, 0,
				0, 0, 0, 37, 0, 43, 45, 0, 0, 50,
				54, 0, 58, 0, 76, 82, 0, 0, 0, 0,
				42, 78, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 77,
				0, 0, 62, 72, 73, 0, 75, 63, 19, 59,
				0, 68, 0, 0, 79, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 39, 40, 38, 44, 0,
				0, 0, 55, 56, 0, 0, 0, 0, 85, 83,
				88, 103, 0, 20, 60, 0, 61, 87
			};
			yyDgoto = new short[35]
			{
				25, 0, 0, 0, 0, 0, 0, 78, 105, 26,
				69, 27, 28, 29, 30, 31, 32, 33, 34, 35,
				36, 37, 38, 39, 40, 41, 42, 79, 80, 112,
				43, 44, 45, 83, 108
			};
			yySindex = new short[118]
			{
				-254, -130, -130, 0, 0, 0, -270, -254, -254, -326,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, -266, -262, -271,
				-256, -201, -267, 0, -258, 0, 0, -238, -169, 0,
				0, -227, 0, -245, 0, 0, -169, -169, -254, -243,
				0, 0, -254, -254, -254, -254, -254, -254, -254, -254,
				-254, -254, -254, -254, -254, -189, -130, -130, -254, 0,
				-130, -130, 0, 0, 0, -237, 0, 0, 0, 0,
				-232, 0, -224, -228, 0, -262, -271, -256, -256, -201,
				-201, -201, -201, -267, -267, 0, 0, 0, 0, -169,
				-169, -222, 0, 0, -285, -219, -220, -254, 0, 0,
				0, 0, -218, 0, 0, -224, 0, 0
			};
			yyRindex = new short[118]
			{
				-176, 1, -176, 0, 0, 0, 0, -176, -176, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 19, 93, 37,
				27, 357, 276, 0, 250, 0, 0, 85, 114, 0,
				0, 0, 0, 0, 0, 0, 140, 169, -198, 0,
				0, 0, -176, -176, -176, -176, -176, -176, -176, -176,
				-176, -176, -176, -176, -176, -176, -176, -176, -176, 0,
				-176, -176, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, -208, 0, 0, 336, 484, 458, 476, 383,
				393, 419, 429, 302, 328, 0, 0, 0, 0, 195,
				224, 0, 0, 0, -206, 59, 0, -176, 0, 0,
				0, 0, 0, 0, 0, -208, 0, 0
			};
			yyGindex = new short[35]
			{
				-7, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				-29, 0, 20, 31, 48, -33, 44, 25, 0, 29,
				0, 0, 2, 0, 66, 0, 0, 0, 0, 0,
				0, 0, 0, 0, -23
			};
			yyTable = new short[765]
			{
				49, 51, 48, 46, 47, 1, 2, 51, 3, 4,
				52, 62, 53, 63, 5, 6, 54, 55, 7, 21,
				81, 66, 67, 89, 90, 91, 92, 26, 65, 8,
				84, 9, 68, 50, 56, 104, 57, 24, 58, 59,
				106, 82, 10, 107, 11, 109, 12, 111, 13, 110,
				14, 68, 15, 114, 16, 116, 17, 72, 18, 57,
				19, 101, 20, 64, 21, 86, 22, 102, 99, 100,
				1, 2, 85, 3, 4, 84, 113, 23, 24, 5,
				6, 60, 61, 7, 86, 46, 70, 95, 96, 97,
				70, 71, 117, 22, 98, 73, 9, 74, 0, 75,
				115, 76, 87, 88, 93, 94, 77, 10, 70, 11,
				0, 12, 0, 13, 49, 14, 0, 15, 0, 16,
				0, 17, 0, 18, 70, 19, 70, 20, 70, 21,
				70, 22, 3, 4, 0, 70, 102, 103, 5, 0,
				52, 0, 23, 24, 0, 0, 70, 0, 70, 0,
				70, 0, 70, 0, 0, 0, 0, 70, 0, 0,
				0, 0, 0, 0, 0, 0, 10, 0, 11, 53,
				12, 0, 13, 0, 14, 0, 15, 0, 16, 0,
				17, 0, 18, 0, 19, 0, 20, 0, 21, 0,
				22, 0, 0, 0, 0, 47, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 48, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				41, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 51, 0,
				0, 0, 51, 0, 51, 51, 34, 51, 0, 51,
				0, 51, 0, 51, 51, 70, 21, 51, 51, 51,
				21, 51, 21, 51, 26, 51, 51, 0, 26, 0,
				26, 26, 35, 26, 24, 0, 0, 0, 24, 0,
				24, 24, 0, 24, 26, 26, 0, 0, 57, 57,
				0, 0, 0, 70, 0, 70, 57, 70, 36, 70,
				57, 51, 57, 57, 70, 57, 23, 57, 0, 57,
				0, 57, 57, 0, 0, 57, 57, 57, 0, 57,
				0, 57, 46, 57, 57, 0, 46, 29, 46, 46,
				22, 46, 0, 46, 22, 46, 22, 46, 46, 22,
				0, 46, 46, 46, 0, 46, 0, 46, 0, 46,
				46, 49, 0, 32, 0, 49, 0, 49, 49, 57,
				49, 0, 49, 33, 49, 0, 49, 49, 0, 0,
				49, 49, 49, 0, 49, 0, 49, 52, 49, 49,
				0, 52, 0, 52, 52, 46, 52, 0, 52, 30,
				52, 0, 52, 52, 0, 0, 52, 52, 52, 31,
				52, 0, 52, 0, 52, 52, 53, 0, 0, 0,
				53, 0, 53, 53, 49, 53, 0, 53, 0, 53,
				0, 53, 53, 0, 0, 53, 53, 53, 27, 53,
				0, 53, 47, 53, 53, 0, 47, 0, 47, 47,
				52, 47, 0, 47, 0, 47, 28, 47, 47, 0,
				0, 47, 47, 47, 25, 47, 0, 47, 0, 47,
				47, 48, 0, 0, 0, 48, 0, 48, 48, 53,
				48, 0, 48, 0, 48, 0, 48, 48, 0, 0,
				48, 48, 48, 0, 48, 0, 48, 41, 48, 48,
				0, 41, 0, 41, 41, 47, 41, 0, 41, 0,
				41, 0, 41, 41, 0, 0, 0, 41, 41, 0,
				41, 0, 41, 34, 41, 41, 0, 34, 0, 34,
				34, 0, 34, 0, 48, 0, 0, 0, 34, 34,
				0, 0, 0, 34, 34, 0, 34, 0, 34, 35,
				34, 34, 0, 35, 0, 35, 35, 0, 35, 0,
				41, 0, 0, 0, 35, 35, 0, 0, 0, 35,
				35, 0, 35, 0, 35, 36, 35, 35, 0, 36,
				0, 36, 36, 23, 36, 0, 0, 23, 0, 23,
				36, 36, 23, 0, 0, 36, 36, 0, 36, 0,
				36, 0, 36, 36, 29, 0, 0, 0, 29, 0,
				29, 29, 0, 29, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 29, 29, 0, 29, 0, 29,
				32, 29, 29, 0, 32, 0, 32, 32, 0, 32,
				33, 0, 0, 0, 33, 0, 33, 33, 0, 33,
				32, 32, 0, 32, 0, 32, 0, 32, 32, 0,
				33, 33, 0, 33, 0, 33, 30, 33, 33, 0,
				30, 0, 30, 30, 0, 30, 31, 0, 0, 0,
				31, 0, 31, 31, 0, 31, 30, 30, 0, 30,
				0, 30, 0, 30, 30, 0, 31, 31, 0, 31,
				0, 31, 0, 31, 31, 27, 0, 0, 0, 27,
				0, 27, 27, 0, 27, 0, 0, 0, 0, 0,
				0, 0, 0, 28, 0, 27, 27, 28, 0, 28,
				28, 25, 28, 0, 0, 25, 0, 25, 25, 0,
				25, 0, 0, 28, 28
			};
			yyCheck = new short[765]
			{
				7, 0, 272, 1, 2, 259, 260, 333, 262, 263,
				276, 278, 274, 280, 268, 269, 287, 288, 272, 0,
				265, 259, 260, 56, 57, 58, 59, 0, 286, 283,
				273, 285, 270, 8, 290, 272, 292, 0, 294, 295,
				272, 48, 296, 267, 298, 273, 300, 332, 302, 271,
				304, 270, 306, 273, 308, 273, 310, 284, 312, 0,
				314, 68, 316, 330, 318, 273, 320, 273, 66, 67,
				259, 260, 52, 262, 263, 273, 105, 331, 332, 268,
				269, 282, 283, 272, 53, 0, 284, 62, 63, 64,
				259, 260, 115, 0, 65, 322, 285, 324, -1, 326,
				107, 328, 54, 55, 60, 61, 333, 296, 284, 298,
				-1, 300, -1, 302, 0, 304, -1, 306, -1, 308,
				-1, 310, -1, 312, 322, 314, 324, 316, 326, 318,
				328, 320, 262, 263, -1, 333, 70, 71, 268, -1,
				0, -1, 331, 332, -1, -1, 322, -1, 324, -1,
				326, -1, 328, -1, -1, -1, -1, 333, -1, -1,
				-1, -1, -1, -1, -1, -1, 296, -1, 298, 0,
				300, -1, 302, -1, 304, -1, 306, -1, 308, -1,
				310, -1, 312, -1, 314, -1, 316, -1, 318, -1,
				320, -1, -1, -1, -1, 0, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, 0, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				0, -1, -1, -1, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, -1, -1, -1, -1, 267, -1,
				-1, -1, 271, -1, 273, 274, 0, 276, -1, 278,
				-1, 280, -1, 282, 283, 284, 267, 286, 287, 288,
				271, 290, 273, 292, 267, 294, 295, -1, 271, -1,
				273, 274, 0, 276, 267, -1, -1, -1, 271, -1,
				273, 274, -1, 276, 287, 288, -1, -1, 259, 260,
				-1, -1, -1, 322, -1, 324, 267, 326, 0, 328,
				271, 330, 273, 274, 333, 276, 0, 278, -1, 280,
				-1, 282, 283, -1, -1, 286, 287, 288, -1, 290,
				-1, 292, 267, 294, 295, -1, 271, 0, 273, 274,
				267, 276, -1, 278, 271, 280, 273, 282, 283, 276,
				-1, 286, 287, 288, -1, 290, -1, 292, -1, 294,
				295, 267, -1, 0, -1, 271, -1, 273, 274, 330,
				276, -1, 278, 0, 280, -1, 282, 283, -1, -1,
				286, 287, 288, -1, 290, -1, 292, 267, 294, 295,
				-1, 271, -1, 273, 274, 330, 276, -1, 278, 0,
				280, -1, 282, 283, -1, -1, 286, 287, 288, 0,
				290, -1, 292, -1, 294, 295, 267, -1, -1, -1,
				271, -1, 273, 274, 330, 276, -1, 278, -1, 280,
				-1, 282, 283, -1, -1, 286, 287, 288, 0, 290,
				-1, 292, 267, 294, 295, -1, 271, -1, 273, 274,
				330, 276, -1, 278, -1, 280, 0, 282, 283, -1,
				-1, 286, 287, 288, 0, 290, -1, 292, -1, 294,
				295, 267, -1, -1, -1, 271, -1, 273, 274, 330,
				276, -1, 278, -1, 280, -1, 282, 283, -1, -1,
				286, 287, 288, -1, 290, -1, 292, 267, 294, 295,
				-1, 271, -1, 273, 274, 330, 276, -1, 278, -1,
				280, -1, 282, 283, -1, -1, -1, 287, 288, -1,
				290, -1, 292, 267, 294, 295, -1, 271, -1, 273,
				274, -1, 276, -1, 330, -1, -1, -1, 282, 283,
				-1, -1, -1, 287, 288, -1, 290, -1, 292, 267,
				294, 295, -1, 271, -1, 273, 274, -1, 276, -1,
				330, -1, -1, -1, 282, 283, -1, -1, -1, 287,
				288, -1, 290, -1, 292, 267, 294, 295, -1, 271,
				-1, 273, 274, 267, 276, -1, -1, 271, -1, 273,
				282, 283, 276, -1, -1, 287, 288, -1, 290, -1,
				292, -1, 294, 295, 267, -1, -1, -1, 271, -1,
				273, 274, -1, 276, -1, -1, -1, -1, -1, -1,
				-1, -1, -1, -1, 287, 288, -1, 290, -1, 292,
				267, 294, 295, -1, 271, -1, 273, 274, -1, 276,
				267, -1, -1, -1, 271, -1, 273, 274, -1, 276,
				287, 288, -1, 290, -1, 292, -1, 294, 295, -1,
				287, 288, -1, 290, -1, 292, 267, 294, 295, -1,
				271, -1, 273, 274, -1, 276, 267, -1, -1, -1,
				271, -1, 273, 274, -1, 276, 287, 288, -1, 290,
				-1, 292, -1, 294, 295, -1, 287, 288, -1, 290,
				-1, 292, -1, 294, 295, 267, -1, -1, -1, 271,
				-1, 273, 274, -1, 276, -1, -1, -1, -1, -1,
				-1, -1, -1, 267, -1, 287, 288, 271, -1, 273,
				274, 267, 276, -1, -1, 271, -1, 273, 274, -1,
				276, -1, -1, 287, 288
			};
		}

		internal Expression Compile(string xpath)
		{
			try
			{
				Tokenizer yyLex = new Tokenizer(xpath);
				return (Expression)yyparse(yyLex);
			}
			catch (XPathException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new XPathException("Error during parse of " + xpath, innerException);
			}
		}

		private NodeSet CreateNodeTest(Axes axis, object nodeTest, ArrayList plist)
		{
			NodeSet nodeSet = CreateNodeTest(axis, nodeTest);
			if (plist != null)
			{
				for (int i = 0; i < plist.Count; i++)
				{
					nodeSet = new ExprFilter(nodeSet, (Expression)plist[i]);
				}
			}
			return nodeSet;
		}

		private NodeTest CreateNodeTest(Axes axis, object test)
		{
			if (test is XPathNodeType)
			{
				return new NodeTypeTest(axis, (XPathNodeType)(int)test, null);
			}
			if (test is string || test == null)
			{
				return new NodeTypeTest(axis, XPathNodeType.ProcessingInstruction, (string)test);
			}
			XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)test;
			if (xmlQualifiedName == XmlQualifiedName.Empty)
			{
				return new NodeTypeTest(axis);
			}
			return new NodeNameTest(axis, xmlQualifiedName, Context);
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
					case 2:
						obj = new ExprUNION((NodeSet)array2[-2 + num4], (NodeSet)array2[0 + num4]);
						break;
					case 3:
						obj = new ExprRoot();
						break;
					case 4:
						obj = new ExprSLASH(new ExprRoot(), (NodeSet)array2[0 + num4]);
						break;
					case 6:
						obj = new ExprSLASH((Expression)array2[-2 + num4], (NodeSet)array2[0 + num4]);
						break;
					case 7:
						obj = new ExprSLASH2((Expression)array2[-2 + num4], (NodeSet)array2[0 + num4]);
						break;
					case 8:
						obj = new ExprSLASH2(new ExprRoot(), (NodeSet)array2[0 + num4]);
						break;
					case 10:
					{
						XmlQualifiedName xmlQualifiedName2 = (XmlQualifiedName)array2[-3 + num4];
						if (xmlQualifiedName2.Name != "id" || xmlQualifiedName2.Namespace != string.Empty)
						{
							throw new XPathException(string.Format("Expected 'id' but got '{0}'", xmlQualifiedName2));
						}
						obj = ExprFunctionCall.Factory(xmlQualifiedName2, new FunctionArguments(new ExprLiteral((string)array2[-1 + num4]), null), Context);
						break;
					}
					case 11:
					{
						XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)array2[-5 + num4];
						if (xmlQualifiedName.Name != "key" || xmlQualifiedName.Namespace != string.Empty)
						{
							throw new XPathException(string.Format("Expected 'key' but got '{0}'", xmlQualifiedName));
						}
						obj = Context.TryGetFunction(xmlQualifiedName, new FunctionArguments(new ExprLiteral((string)array2[-3 + num4]), new FunctionArguments(new ExprLiteral((string)array2[-1 + num4]), null)));
						break;
					}
					case 13:
						obj = new ExprSLASH((Expression)array2[-2 + num4], (NodeSet)array2[0 + num4]);
						break;
					case 14:
						obj = new ExprSLASH2((Expression)array2[-2 + num4], (NodeSet)array2[0 + num4]);
						break;
					case 15:
						obj = CreateNodeTest((Axes)(int)array2[-2 + num4], array2[-1 + num4], (ArrayList)array2[0 + num4]);
						break;
					case 17:
						obj = Axes.Child;
						break;
					case 18:
						obj = Axes.Attribute;
						break;
					case 19:
						obj = null;
						break;
					case 20:
					{
						ArrayList arrayList2 = (ArrayList)array2[-1 + num4];
						if (arrayList2 == null)
						{
							arrayList2 = new ArrayList();
						}
						arrayList2.Add((Expression)array2[0 + num4]);
						obj = arrayList2;
						break;
					}
					case 23:
						obj = new ExprOR((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 25:
						obj = new ExprAND((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 27:
						obj = new ExprEQ((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 28:
						obj = new ExprNE((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 30:
						obj = new ExprLT((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 31:
						obj = new ExprGT((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 32:
						obj = new ExprLE((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 33:
						obj = new ExprGE((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 35:
						obj = new ExprPLUS((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 36:
						obj = new ExprMINUS((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 38:
						obj = new ExprMULT((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 39:
						obj = new ExprDIV((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 40:
						obj = new ExprMOD((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 42:
						obj = new ExprNEG((Expression)array2[0 + num4]);
						break;
					case 44:
						obj = new ExprUNION((Expression)array2[-2 + num4], (Expression)array2[0 + num4]);
						break;
					case 47:
						obj = new ExprSLASH((Expression)array2[-2 + num4], (NodeSet)array2[0 + num4]);
						break;
					case 48:
						obj = new ExprSLASH2((Expression)array2[-2 + num4], (NodeSet)array2[0 + num4]);
						break;
					case 51:
						obj = new ExprRoot();
						break;
					case 52:
						obj = new ExprSLASH(new ExprRoot(), (NodeSet)array2[0 + num4]);
						break;
					case 53:
						obj = new ExprSLASH2(new ExprRoot(), (NodeSet)array2[0 + num4]);
						break;
					case 55:
						obj = new ExprSLASH((NodeSet)array2[-2 + num4], (NodeSet)array2[0 + num4]);
						break;
					case 56:
						obj = new ExprSLASH2((NodeSet)array2[-2 + num4], (NodeSet)array2[0 + num4]);
						break;
					case 57:
						obj = CreateNodeTest((Axes)(int)array2[-2 + num4], array2[-1 + num4], (ArrayList)array2[0 + num4]);
						break;
					case 60:
						obj = (XPathNodeType)(int)array2[-2 + num4];
						break;
					case 61:
						obj = (string)array2[-1 + num4];
						break;
					case 62:
						obj = XmlQualifiedName.Empty;
						break;
					case 64:
						obj = new NodeTypeTest(Axes.Self, XPathNodeType.All);
						break;
					case 65:
						obj = new NodeTypeTest(Axes.Parent, XPathNodeType.All);
						break;
					case 66:
						obj = null;
						break;
					case 67:
					{
						ArrayList arrayList = (ArrayList)array2[-1 + num4];
						if (arrayList == null)
						{
							arrayList = new ArrayList();
						}
						arrayList.Add(array2[0 + num4]);
						obj = arrayList;
						break;
					}
					case 68:
						obj = array2[-1 + num4];
						break;
					case 70:
						obj = Axes.Child;
						break;
					case 71:
						obj = Axes.Attribute;
						break;
					case 72:
						obj = XPathNodeType.Comment;
						break;
					case 73:
						obj = XPathNodeType.Text;
						break;
					case 74:
						obj = XPathNodeType.ProcessingInstruction;
						break;
					case 75:
						obj = XPathNodeType.All;
						break;
					case 77:
						obj = new ExprFilter((Expression)array2[-1 + num4], (Expression)array2[0 + num4]);
						break;
					case 78:
					{
						Expression expression2 = null;
						if (Context != null)
						{
							expression2 = Context.TryGetVariable(((XmlQualifiedName)array2[0 + num4]).ToString());
						}
						if (expression2 == null)
						{
							expression2 = new ExprVariable((XmlQualifiedName)array2[0 + num4], Context);
						}
						obj = expression2;
						break;
					}
					case 79:
						obj = new ExprParens((Expression)array2[-1 + num4]);
						break;
					case 80:
						obj = new ExprLiteral((string)array2[0 + num4]);
						break;
					case 81:
						obj = new ExprNumber((double)array2[0 + num4]);
						break;
					case 83:
					{
						Expression expression = null;
						if (Context != null)
						{
							expression = Context.TryGetFunction((XmlQualifiedName)array2[-3 + num4], (FunctionArguments)array2[-1 + num4]);
						}
						if (expression == null)
						{
							expression = ExprFunctionCall.Factory((XmlQualifiedName)array2[-3 + num4], (FunctionArguments)array2[-1 + num4], Context);
						}
						obj = expression;
						break;
					}
					case 85:
						obj = new FunctionArguments((Expression)array2[-1 + num4], (FunctionArguments)array2[0 + num4]);
						break;
					case 87:
						obj = new FunctionArguments((Expression)array2[-1 + num4], (FunctionArguments)array2[0 + num4]);
						break;
					case 88:
						obj = array2[-1 + num4];
						break;
					case 89:
						obj = Axes.Ancestor;
						break;
					case 90:
						obj = Axes.AncestorOrSelf;
						break;
					case 91:
						obj = Axes.Attribute;
						break;
					case 92:
						obj = Axes.Child;
						break;
					case 93:
						obj = Axes.Descendant;
						break;
					case 94:
						obj = Axes.DescendantOrSelf;
						break;
					case 95:
						obj = Axes.Following;
						break;
					case 96:
						obj = Axes.FollowingSibling;
						break;
					case 97:
						obj = Axes.Namespace;
						break;
					case 98:
						obj = Axes.Parent;
						break;
					case 99:
						obj = Axes.Preceding;
						break;
					case 100:
						obj = Axes.PrecedingSibling;
						break;
					case 101:
						obj = Axes.Self;
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
