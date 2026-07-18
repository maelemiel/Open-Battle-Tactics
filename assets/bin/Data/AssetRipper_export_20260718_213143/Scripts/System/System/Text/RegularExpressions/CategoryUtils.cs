using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal class CategoryUtils
	{
		public static System.Text.RegularExpressions.Category CategoryFromName(string name)
		{
			try
			{
				if (name.StartsWith("Is"))
				{
					name = name.Substring(2);
				}
				return (System.Text.RegularExpressions.Category)(ushort)Enum.Parse(typeof(System.Text.RegularExpressions.Category), "Unicode" + name, false);
			}
			catch (ArgumentException)
			{
				return System.Text.RegularExpressions.Category.None;
			}
		}

		public static bool IsCategory(System.Text.RegularExpressions.Category cat, char c)
		{
			switch (cat)
			{
			case System.Text.RegularExpressions.Category.None:
				return false;
			case System.Text.RegularExpressions.Category.Any:
				return c != '\n';
			case System.Text.RegularExpressions.Category.AnySingleline:
				return true;
			case System.Text.RegularExpressions.Category.Word:
				return char.IsLetterOrDigit(c) || IsCategory(UnicodeCategory.ConnectorPunctuation, c);
			case System.Text.RegularExpressions.Category.Digit:
				return char.IsDigit(c);
			case System.Text.RegularExpressions.Category.WhiteSpace:
				return char.IsWhiteSpace(c);
			case System.Text.RegularExpressions.Category.EcmaAny:
				return c != '\n';
			case System.Text.RegularExpressions.Category.EcmaAnySingleline:
				return true;
			case System.Text.RegularExpressions.Category.EcmaWord:
				return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || ('0' <= c && c <= '9') || '_' == c;
			case System.Text.RegularExpressions.Category.EcmaDigit:
				return '0' <= c && c <= '9';
			case System.Text.RegularExpressions.Category.EcmaWhiteSpace:
				return c == ' ' || c == '\f' || c == '\n' || c == '\r' || c == '\t' || c == '\v';
			case System.Text.RegularExpressions.Category.UnicodeLu:
				return IsCategory(UnicodeCategory.UppercaseLetter, c);
			case System.Text.RegularExpressions.Category.UnicodeLl:
				return IsCategory(UnicodeCategory.LowercaseLetter, c);
			case System.Text.RegularExpressions.Category.UnicodeLt:
				return IsCategory(UnicodeCategory.TitlecaseLetter, c);
			case System.Text.RegularExpressions.Category.UnicodeLm:
				return IsCategory(UnicodeCategory.ModifierLetter, c);
			case System.Text.RegularExpressions.Category.UnicodeLo:
				return IsCategory(UnicodeCategory.OtherLetter, c);
			case System.Text.RegularExpressions.Category.UnicodeMn:
				return IsCategory(UnicodeCategory.NonSpacingMark, c);
			case System.Text.RegularExpressions.Category.UnicodeMe:
				return IsCategory(UnicodeCategory.EnclosingMark, c);
			case System.Text.RegularExpressions.Category.UnicodeMc:
				return IsCategory(UnicodeCategory.SpacingCombiningMark, c);
			case System.Text.RegularExpressions.Category.UnicodeNd:
				return IsCategory(UnicodeCategory.DecimalDigitNumber, c);
			case System.Text.RegularExpressions.Category.UnicodeNl:
				return IsCategory(UnicodeCategory.LetterNumber, c);
			case System.Text.RegularExpressions.Category.UnicodeNo:
				return IsCategory(UnicodeCategory.OtherNumber, c);
			case System.Text.RegularExpressions.Category.UnicodeZs:
				return IsCategory(UnicodeCategory.SpaceSeparator, c);
			case System.Text.RegularExpressions.Category.UnicodeZl:
				return IsCategory(UnicodeCategory.LineSeparator, c);
			case System.Text.RegularExpressions.Category.UnicodeZp:
				return IsCategory(UnicodeCategory.ParagraphSeparator, c);
			case System.Text.RegularExpressions.Category.UnicodePd:
				return IsCategory(UnicodeCategory.DashPunctuation, c);
			case System.Text.RegularExpressions.Category.UnicodePs:
				return IsCategory(UnicodeCategory.OpenPunctuation, c);
			case System.Text.RegularExpressions.Category.UnicodePi:
				return IsCategory(UnicodeCategory.InitialQuotePunctuation, c);
			case System.Text.RegularExpressions.Category.UnicodePe:
				return IsCategory(UnicodeCategory.ClosePunctuation, c);
			case System.Text.RegularExpressions.Category.UnicodePf:
				return IsCategory(UnicodeCategory.FinalQuotePunctuation, c);
			case System.Text.RegularExpressions.Category.UnicodePc:
				return IsCategory(UnicodeCategory.ConnectorPunctuation, c);
			case System.Text.RegularExpressions.Category.UnicodePo:
				return IsCategory(UnicodeCategory.OtherPunctuation, c);
			case System.Text.RegularExpressions.Category.UnicodeSm:
				return IsCategory(UnicodeCategory.MathSymbol, c);
			case System.Text.RegularExpressions.Category.UnicodeSc:
				return IsCategory(UnicodeCategory.CurrencySymbol, c);
			case System.Text.RegularExpressions.Category.UnicodeSk:
				return IsCategory(UnicodeCategory.ModifierSymbol, c);
			case System.Text.RegularExpressions.Category.UnicodeSo:
				return IsCategory(UnicodeCategory.OtherSymbol, c);
			case System.Text.RegularExpressions.Category.UnicodeCc:
				return IsCategory(UnicodeCategory.Control, c);
			case System.Text.RegularExpressions.Category.UnicodeCf:
				return IsCategory(UnicodeCategory.Format, c);
			case System.Text.RegularExpressions.Category.UnicodeCo:
				return IsCategory(UnicodeCategory.PrivateUse, c);
			case System.Text.RegularExpressions.Category.UnicodeCs:
				return IsCategory(UnicodeCategory.Surrogate, c);
			case System.Text.RegularExpressions.Category.UnicodeCn:
				return IsCategory(UnicodeCategory.OtherNotAssigned, c);
			case System.Text.RegularExpressions.Category.UnicodeL:
				return IsCategory(UnicodeCategory.UppercaseLetter, c) || IsCategory(UnicodeCategory.LowercaseLetter, c) || IsCategory(UnicodeCategory.TitlecaseLetter, c) || IsCategory(UnicodeCategory.ModifierLetter, c) || IsCategory(UnicodeCategory.OtherLetter, c);
			case System.Text.RegularExpressions.Category.UnicodeM:
				return IsCategory(UnicodeCategory.NonSpacingMark, c) || IsCategory(UnicodeCategory.EnclosingMark, c) || IsCategory(UnicodeCategory.SpacingCombiningMark, c);
			case System.Text.RegularExpressions.Category.UnicodeN:
				return IsCategory(UnicodeCategory.DecimalDigitNumber, c) || IsCategory(UnicodeCategory.LetterNumber, c) || IsCategory(UnicodeCategory.OtherNumber, c);
			case System.Text.RegularExpressions.Category.UnicodeZ:
				return IsCategory(UnicodeCategory.SpaceSeparator, c) || IsCategory(UnicodeCategory.LineSeparator, c) || IsCategory(UnicodeCategory.ParagraphSeparator, c);
			case System.Text.RegularExpressions.Category.UnicodeP:
				return IsCategory(UnicodeCategory.DashPunctuation, c) || IsCategory(UnicodeCategory.OpenPunctuation, c) || IsCategory(UnicodeCategory.InitialQuotePunctuation, c) || IsCategory(UnicodeCategory.ClosePunctuation, c) || IsCategory(UnicodeCategory.FinalQuotePunctuation, c) || IsCategory(UnicodeCategory.ConnectorPunctuation, c) || IsCategory(UnicodeCategory.OtherPunctuation, c);
			case System.Text.RegularExpressions.Category.UnicodeS:
				return IsCategory(UnicodeCategory.MathSymbol, c) || IsCategory(UnicodeCategory.CurrencySymbol, c) || IsCategory(UnicodeCategory.ModifierSymbol, c) || IsCategory(UnicodeCategory.OtherSymbol, c);
			case System.Text.RegularExpressions.Category.UnicodeC:
				return IsCategory(UnicodeCategory.Control, c) || IsCategory(UnicodeCategory.Format, c) || IsCategory(UnicodeCategory.PrivateUse, c) || IsCategory(UnicodeCategory.Surrogate, c) || IsCategory(UnicodeCategory.OtherNotAssigned, c);
			case System.Text.RegularExpressions.Category.UnicodeBasicLatin:
				return '\0' <= c && c <= '\u007f';
			case System.Text.RegularExpressions.Category.UnicodeLatin1Supplement:
				return '\u0080' <= c && c <= 'ÿ';
			case System.Text.RegularExpressions.Category.UnicodeLatinExtendedA:
				return 'Ā' <= c && c <= 'ſ';
			case System.Text.RegularExpressions.Category.UnicodeLatinExtendedB:
				return 'ƀ' <= c && c <= 'ɏ';
			case System.Text.RegularExpressions.Category.UnicodeIPAExtensions:
				return 'ɐ' <= c && c <= 'ʯ';
			case System.Text.RegularExpressions.Category.UnicodeSpacingModifierLetters:
				return 'ʰ' <= c && c <= '\u02ff';
			case System.Text.RegularExpressions.Category.UnicodeCombiningDiacriticalMarks:
				return '\u0300' <= c && c <= '\u036f';
			case System.Text.RegularExpressions.Category.UnicodeGreek:
				return 'Ͱ' <= c && c <= 'Ͽ';
			case System.Text.RegularExpressions.Category.UnicodeCyrillic:
				return 'Ѐ' <= c && c <= 'ӿ';
			case System.Text.RegularExpressions.Category.UnicodeArmenian:
				return '\u0530' <= c && c <= '֏';
			case System.Text.RegularExpressions.Category.UnicodeHebrew:
				return '\u0590' <= c && c <= '\u05ff';
			case System.Text.RegularExpressions.Category.UnicodeArabic:
				return '\u0600' <= c && c <= 'ۿ';
			case System.Text.RegularExpressions.Category.UnicodeSyriac:
				return '܀' <= c && c <= 'ݏ';
			case System.Text.RegularExpressions.Category.UnicodeThaana:
				return 'ހ' <= c && c <= '\u07bf';
			case System.Text.RegularExpressions.Category.UnicodeDevanagari:
				return '\u0900' <= c && c <= 'ॿ';
			case System.Text.RegularExpressions.Category.UnicodeBengali:
				return 'ঀ' <= c && c <= '\u09ff';
			case System.Text.RegularExpressions.Category.UnicodeGurmukhi:
				return '\u0a00' <= c && c <= '\u0a7f';
			case System.Text.RegularExpressions.Category.UnicodeGujarati:
				return '\u0a80' <= c && c <= '\u0aff';
			case System.Text.RegularExpressions.Category.UnicodeOriya:
				return '\u0b00' <= c && c <= '\u0b7f';
			case System.Text.RegularExpressions.Category.UnicodeTamil:
				return '\u0b80' <= c && c <= '\u0bff';
			case System.Text.RegularExpressions.Category.UnicodeTelugu:
				return '\u0c00' <= c && c <= '౿';
			case System.Text.RegularExpressions.Category.UnicodeKannada:
				return 'ಀ' <= c && c <= '\u0cff';
			case System.Text.RegularExpressions.Category.UnicodeMalayalam:
				return '\u0d00' <= c && c <= 'ൿ';
			case System.Text.RegularExpressions.Category.UnicodeSinhala:
				return '\u0d80' <= c && c <= '\u0dff';
			case System.Text.RegularExpressions.Category.UnicodeThai:
				return '\u0e00' <= c && c <= '\u0e7f';
			case System.Text.RegularExpressions.Category.UnicodeLao:
				return '\u0e80' <= c && c <= '\u0eff';
			case System.Text.RegularExpressions.Category.UnicodeTibetan:
				return 'ༀ' <= c && c <= '\u0fff';
			case System.Text.RegularExpressions.Category.UnicodeMyanmar:
				return 'က' <= c && c <= '႟';
			case System.Text.RegularExpressions.Category.UnicodeGeorgian:
				return 'Ⴀ' <= c && c <= 'ჿ';
			case System.Text.RegularExpressions.Category.UnicodeHangulJamo:
				return 'ᄀ' <= c && c <= 'ᇿ';
			case System.Text.RegularExpressions.Category.UnicodeEthiopic:
				return 'ሀ' <= c && c <= '\u137f';
			case System.Text.RegularExpressions.Category.UnicodeCherokee:
				return 'Ꭰ' <= c && c <= '\u13ff';
			case System.Text.RegularExpressions.Category.UnicodeUnifiedCanadianAboriginalSyllabics:
				return '᐀' <= c && c <= 'ᙿ';
			case System.Text.RegularExpressions.Category.UnicodeOgham:
				return '\u1680' <= c && c <= '\u169f';
			case System.Text.RegularExpressions.Category.UnicodeRunic:
				return 'ᚠ' <= c && c <= '\u16ff';
			case System.Text.RegularExpressions.Category.UnicodeKhmer:
				return 'ក' <= c && c <= '\u17ff';
			case System.Text.RegularExpressions.Category.UnicodeMongolian:
				return '᠀' <= c && c <= '\u18af';
			case System.Text.RegularExpressions.Category.UnicodeLatinExtendedAdditional:
				return 'Ḁ' <= c && c <= 'ỿ';
			case System.Text.RegularExpressions.Category.UnicodeGreekExtended:
				return 'ἀ' <= c && c <= '\u1fff';
			case System.Text.RegularExpressions.Category.UnicodeGeneralPunctuation:
				return '\u2000' <= c && c <= '\u206f';
			case System.Text.RegularExpressions.Category.UnicodeSuperscriptsandSubscripts:
				return '⁰' <= c && c <= '\u209f';
			case System.Text.RegularExpressions.Category.UnicodeCurrencySymbols:
				return '₠' <= c && c <= '\u20cf';
			case System.Text.RegularExpressions.Category.UnicodeCombiningMarksforSymbols:
				return '\u20d0' <= c && c <= '\u20ff';
			case System.Text.RegularExpressions.Category.UnicodeLetterlikeSymbols:
				return '℀' <= c && c <= '⅏';
			case System.Text.RegularExpressions.Category.UnicodeNumberForms:
				return '⅐' <= c && c <= '\u218f';
			case System.Text.RegularExpressions.Category.UnicodeArrows:
				return '←' <= c && c <= '⇿';
			case System.Text.RegularExpressions.Category.UnicodeMathematicalOperators:
				return '∀' <= c && c <= '⋿';
			case System.Text.RegularExpressions.Category.UnicodeMiscellaneousTechnical:
				return '⌀' <= c && c <= '⏿';
			case System.Text.RegularExpressions.Category.UnicodeControlPictures:
				return '␀' <= c && c <= '\u243f';
			case System.Text.RegularExpressions.Category.UnicodeOpticalCharacterRecognition:
				return '⑀' <= c && c <= '\u245f';
			case System.Text.RegularExpressions.Category.UnicodeEnclosedAlphanumerics:
				return '①' <= c && c <= '⓿';
			case System.Text.RegularExpressions.Category.UnicodeBoxDrawing:
				return '─' <= c && c <= '╿';
			case System.Text.RegularExpressions.Category.UnicodeBlockElements:
				return '▀' <= c && c <= '▟';
			case System.Text.RegularExpressions.Category.UnicodeGeometricShapes:
				return '■' <= c && c <= '◿';
			case System.Text.RegularExpressions.Category.UnicodeMiscellaneousSymbols:
				return '☀' <= c && c <= '⛿';
			case System.Text.RegularExpressions.Category.UnicodeDingbats:
				return '✀' <= c && c <= '➿';
			case System.Text.RegularExpressions.Category.UnicodeBraillePatterns:
				return '⠀' <= c && c <= '⣿';
			case System.Text.RegularExpressions.Category.UnicodeCJKRadicalsSupplement:
				return '⺀' <= c && c <= '\u2eff';
			case System.Text.RegularExpressions.Category.UnicodeKangxiRadicals:
				return '⼀' <= c && c <= '\u2fdf';
			case System.Text.RegularExpressions.Category.UnicodeIdeographicDescriptionCharacters:
				return '⿰' <= c && c <= '⿿';
			case System.Text.RegularExpressions.Category.UnicodeCJKSymbolsandPunctuation:
				return '\u3000' <= c && c <= '〿';
			case System.Text.RegularExpressions.Category.UnicodeHiragana:
				return '\u3040' <= c && c <= 'ゟ';
			case System.Text.RegularExpressions.Category.UnicodeKatakana:
				return '゠' <= c && c <= 'ヿ';
			case System.Text.RegularExpressions.Category.UnicodeBopomofo:
				return '\u3100' <= c && c <= 'ㄯ';
			case System.Text.RegularExpressions.Category.UnicodeHangulCompatibilityJamo:
				return '\u3130' <= c && c <= '\u318f';
			case System.Text.RegularExpressions.Category.UnicodeKanbun:
				return '㆐' <= c && c <= '㆟';
			case System.Text.RegularExpressions.Category.UnicodeBopomofoExtended:
				return 'ㆠ' <= c && c <= 'ㆿ';
			case System.Text.RegularExpressions.Category.UnicodeEnclosedCJKLettersandMonths:
				return '㈀' <= c && c <= '㋿';
			case System.Text.RegularExpressions.Category.UnicodeCJKCompatibility:
				return '㌀' <= c && c <= '㏿';
			case System.Text.RegularExpressions.Category.UnicodeCJKUnifiedIdeographsExtensionA:
				return '㐀' <= c && c <= '䶵';
			case System.Text.RegularExpressions.Category.UnicodeCJKUnifiedIdeographs:
				return '一' <= c && c <= '鿿';
			case System.Text.RegularExpressions.Category.UnicodeYiSyllables:
				return 'ꀀ' <= c && c <= '\ua48f';
			case System.Text.RegularExpressions.Category.UnicodeYiRadicals:
				return '꒐' <= c && c <= '\ua4cf';
			case System.Text.RegularExpressions.Category.UnicodeHangulSyllables:
				return '가' <= c && c <= '힣';
			case System.Text.RegularExpressions.Category.UnicodeHighSurrogates:
				return '\ud800' <= c && c <= '\udb7f';
			case System.Text.RegularExpressions.Category.UnicodeHighPrivateUseSurrogates:
				return '\udb80' <= c && c <= '\udbff';
			case System.Text.RegularExpressions.Category.UnicodeLowSurrogates:
				return '\udc00' <= c && c <= '\udfff';
			case System.Text.RegularExpressions.Category.UnicodePrivateUse:
				return '\ue000' <= c && c <= '\uf8ff';
			case System.Text.RegularExpressions.Category.UnicodeCJKCompatibilityIdeographs:
				return '豈' <= c && c <= '\ufaff';
			case System.Text.RegularExpressions.Category.UnicodeAlphabeticPresentationForms:
				return 'ﬀ' <= c && c <= 'ﭏ';
			case System.Text.RegularExpressions.Category.UnicodeArabicPresentationFormsA:
				return 'ﭐ' <= c && c <= '﷿';
			case System.Text.RegularExpressions.Category.UnicodeCombiningHalfMarks:
				return '\ufe20' <= c && c <= '\ufe2f';
			case System.Text.RegularExpressions.Category.UnicodeCJKCompatibilityForms:
				return '︰' <= c && c <= '\ufe4f';
			case System.Text.RegularExpressions.Category.UnicodeSmallFormVariants:
				return '﹐' <= c && c <= '\ufe6f';
			case System.Text.RegularExpressions.Category.UnicodeArabicPresentationFormsB:
				return 'ﹰ' <= c && c <= '\ufefe';
			case System.Text.RegularExpressions.Category.UnicodeHalfwidthandFullwidthForms:
				return '\uff00' <= c && c <= '\uffef';
			case System.Text.RegularExpressions.Category.UnicodeSpecials:
				return ('\ufeff' <= c && c <= '\ufeff') || ('\ufff0' <= c && c <= '\ufffd');
			case System.Text.RegularExpressions.Category.UnicodeOldItalic:
			case System.Text.RegularExpressions.Category.UnicodeGothic:
			case System.Text.RegularExpressions.Category.UnicodeDeseret:
			case System.Text.RegularExpressions.Category.UnicodeByzantineMusicalSymbols:
			case System.Text.RegularExpressions.Category.UnicodeMusicalSymbols:
			case System.Text.RegularExpressions.Category.UnicodeMathematicalAlphanumericSymbols:
			case System.Text.RegularExpressions.Category.UnicodeCJKUnifiedIdeographsExtensionB:
			case System.Text.RegularExpressions.Category.UnicodeCJKCompatibilityIdeographsSupplement:
			case System.Text.RegularExpressions.Category.UnicodeTags:
				return false;
			default:
				return false;
			}
		}

		private static bool IsCategory(UnicodeCategory uc, char c)
		{
			if (char.GetUnicodeCategory(c) == uc)
			{
				return true;
			}
			return false;
		}
	}
}
