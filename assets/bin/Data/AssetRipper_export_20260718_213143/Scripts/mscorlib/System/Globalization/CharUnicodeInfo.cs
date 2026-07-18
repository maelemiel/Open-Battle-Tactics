namespace System.Globalization
{
	public sealed class CharUnicodeInfo
	{
		private CharUnicodeInfo()
		{
		}

		public static int GetDecimalDigitValue(char ch)
		{
			switch ((int)ch)
			{
			case 178:
				return 2;
			case 179:
				return 3;
			case 185:
				return 1;
			case 8304:
				return 0;
			default:
				if ('⁴' <= ch && ch < '⁺')
				{
					return ch - 8304;
				}
				if ('₀' <= ch && ch < '₊')
				{
					return ch - 8320;
				}
				if (!char.IsDigit(ch))
				{
					return -1;
				}
				if (ch < ':')
				{
					return ch - 48;
				}
				if (ch < '٪')
				{
					return ch - 1632;
				}
				if (ch < 'ۺ')
				{
					return ch - 1776;
				}
				if (ch < '॰')
				{
					return ch - 2406;
				}
				if (ch < 'ৰ')
				{
					return ch - 2534;
				}
				if (ch < '\u0a70')
				{
					return ch - 2662;
				}
				if (ch < '૰')
				{
					return ch - 2790;
				}
				if (ch < '୰')
				{
					return ch - 2918;
				}
				if (ch < '௰')
				{
					return ch - 3046;
				}
				if (ch < '\u0c70')
				{
					return ch - 3174;
				}
				if (ch < '\u0cf0')
				{
					return ch - 3302;
				}
				if (ch < '൰')
				{
					return ch - 3430;
				}
				if (ch < '๚')
				{
					return ch - 3664;
				}
				if (ch < '\u0eda')
				{
					return ch - 3792;
				}
				if (ch < '༪')
				{
					return ch - 3872;
				}
				if (ch < '၊')
				{
					return ch - 4160;
				}
				if (ch < '፲')
				{
					return ch - 4968;
				}
				if (ch < '\u17ea')
				{
					return ch - 6112;
				}
				if (ch < '\u181a')
				{
					return ch - 6160;
				}
				if (ch < '⁺')
				{
					return ch - 8304;
				}
				if (ch < '₊')
				{
					return ch - 8320;
				}
				if (ch < '０')
				{
					return -1;
				}
				if (ch < '：')
				{
					return ch - 65296;
				}
				return -1;
			}
		}

		public static int GetDecimalDigitValue(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return GetDecimalDigitValue(s[index]);
		}

		public static int GetDigitValue(char ch)
		{
			int decimalDigitValue = GetDecimalDigitValue(ch);
			if (decimalDigitValue >= 0)
			{
				return decimalDigitValue;
			}
			decimalDigitValue = ch;
			switch (decimalDigitValue)
			{
			case 9450:
				return 0;
			case 9312:
			case 9313:
			case 9314:
			case 9315:
			case 9316:
			case 9317:
			case 9318:
			case 9319:
			case 9320:
				return decimalDigitValue - 9311;
			default:
				if (decimalDigitValue >= 9332 && decimalDigitValue < 9341)
				{
					return decimalDigitValue - 9331;
				}
				if (decimalDigitValue >= 9352 && decimalDigitValue < 9361)
				{
					return decimalDigitValue - 9351;
				}
				if (decimalDigitValue >= 9461 && decimalDigitValue < 9470)
				{
					return decimalDigitValue - 9460;
				}
				if (decimalDigitValue >= 10102 && decimalDigitValue < 10111)
				{
					return decimalDigitValue - 10101;
				}
				if (decimalDigitValue >= 10112 && decimalDigitValue < 10121)
				{
					return decimalDigitValue - 10111;
				}
				if (decimalDigitValue >= 10122 && decimalDigitValue < 10131)
				{
					return decimalDigitValue - 10121;
				}
				return -1;
			}
		}

		public static int GetDigitValue(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return GetDigitValue(s[index]);
		}

		public static double GetNumericValue(char ch)
		{
			int digitValue = GetDigitValue(ch);
			if (digitValue >= 0)
			{
				return digitValue;
			}
			digitValue = ch;
			switch (digitValue)
			{
			case 188:
				return 0.25;
			case 189:
				return 0.5;
			case 190:
				return 0.75;
			case 2548:
				return 1.0;
			case 2549:
				return 2.0;
			case 2550:
				return 3.0;
			case 2551:
				return 4.0;
			case 2553:
				return 16.0;
			case 3056:
				return 10.0;
			case 3057:
				return 100.0;
			case 3058:
				return 1000.0;
			case 4988:
				return 10000.0;
			case 5870:
				return 17.0;
			case 5871:
				return 18.0;
			case 5872:
				return 19.0;
			case 8531:
				return 1.0 / 3.0;
			case 8532:
				return 2.0 / 3.0;
			case 8537:
				return 1.0 / 6.0;
			case 8538:
				return 5.0 / 6.0;
			case 8539:
				return 0.125;
			case 8540:
				return 0.375;
			case 8541:
				return 0.625;
			case 8542:
				return 0.875;
			case 8543:
				return 1.0;
			case 8556:
				return 50.0;
			case 8557:
				return 100.0;
			case 8558:
				return 500.0;
			case 8559:
				return 1000.0;
			case 8572:
				return 50.0;
			case 8573:
				return 100.0;
			case 8574:
				return 500.0;
			case 8575:
				return 1000.0;
			case 8576:
				return 1000.0;
			case 8577:
				return 5000.0;
			case 8578:
				return 10000.0;
			case 9470:
			case 10111:
			case 10121:
			case 10131:
				return 10.0;
			case 12295:
				return 0.0;
			case 12344:
				return 10.0;
			case 12345:
				return 20.0;
			case 12346:
				return 30.0;
			default:
				if (9451 <= digitValue && digitValue < 9461)
				{
					return digitValue - 9440;
				}
				if (12321 <= digitValue && digitValue < 12330)
				{
					return digitValue - 12320;
				}
				if (12881 <= digitValue && digitValue < 12896)
				{
					return digitValue - 12860;
				}
				if (12977 <= digitValue && digitValue < 12992)
				{
					return digitValue - 12941;
				}
				if (!char.IsNumber(ch))
				{
					return -1.0;
				}
				if (digitValue < 3891)
				{
					return 0.5 + (double)digitValue - 3882.0;
				}
				if (digitValue < 4988)
				{
					return (digitValue - 4977) * 10;
				}
				if (digitValue < 8537)
				{
					return 0.2 * (double)(digitValue - 8532);
				}
				if (digitValue < 8556)
				{
					return digitValue - 8543;
				}
				if (digitValue < 8572)
				{
					return digitValue - 8559;
				}
				if (digitValue < 9332)
				{
					return digitValue - 9311;
				}
				if (digitValue < 9352)
				{
					return digitValue - 9331;
				}
				if (digitValue < 9372)
				{
					return digitValue - 9351;
				}
				if (digitValue < 12694)
				{
					return digitValue - 12689;
				}
				if (digitValue < 12842)
				{
					return digitValue - 12831;
				}
				if (digitValue < 12938)
				{
					return digitValue - 12927;
				}
				return -1.0;
			}
		}

		public static double GetNumericValue(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return GetNumericValue(s[index]);
		}

		public static UnicodeCategory GetUnicodeCategory(char ch)
		{
			return char.GetUnicodeCategory(ch);
		}

		public static UnicodeCategory GetUnicodeCategory(string s, int index)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return char.GetUnicodeCategory(s, index);
		}
	}
}
