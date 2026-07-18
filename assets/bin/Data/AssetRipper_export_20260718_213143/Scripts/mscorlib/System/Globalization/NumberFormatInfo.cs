using System.Runtime.InteropServices;
using System.Threading;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public sealed class NumberFormatInfo : ICloneable, IFormatProvider
	{
		private bool isReadOnly;

		private string decimalFormats;

		private string currencyFormats;

		private string percentFormats;

		private string digitPattern = "#";

		private string zeroPattern = "0";

		private int currencyDecimalDigits;

		private string currencyDecimalSeparator;

		private string currencyGroupSeparator;

		private int[] currencyGroupSizes;

		private int currencyNegativePattern;

		private int currencyPositivePattern;

		private string currencySymbol;

		private string nanSymbol;

		private string negativeInfinitySymbol;

		private string negativeSign;

		private int numberDecimalDigits;

		private string numberDecimalSeparator;

		private string numberGroupSeparator;

		private int[] numberGroupSizes;

		private int numberNegativePattern;

		private int percentDecimalDigits;

		private string percentDecimalSeparator;

		private string percentGroupSeparator;

		private int[] percentGroupSizes;

		private int percentNegativePattern;

		private int percentPositivePattern;

		private string percentSymbol;

		private string perMilleSymbol;

		private string positiveInfinitySymbol;

		private string positiveSign;

		private string ansiCurrencySymbol;

		private int m_dataItem;

		private bool m_useUserOverride;

		private bool validForParseAsNumber;

		private bool validForParseAsCurrency;

		private string[] nativeDigits = invariantNativeDigits;

		private int digitSubstitution = 1;

		private static readonly string[] invariantNativeDigits = new string[10] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

		public int CurrencyDecimalDigits
		{
			get
			{
				return currencyDecimalDigits;
			}
			set
			{
				if (value < 0 || value > 99)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 99");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				currencyDecimalDigits = value;
			}
		}

		public string CurrencyDecimalSeparator
		{
			get
			{
				return currencyDecimalSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				currencyDecimalSeparator = value;
			}
		}

		public string CurrencyGroupSeparator
		{
			get
			{
				return currencyGroupSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				currencyGroupSeparator = value;
			}
		}

		public int[] CurrencyGroupSizes
		{
			get
			{
				return (int[])RawCurrencyGroupSizes.Clone();
			}
			set
			{
				RawCurrencyGroupSizes = value;
			}
		}

		internal int[] RawCurrencyGroupSizes
		{
			get
			{
				return currencyGroupSizes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				if (value.Length == 0)
				{
					currencyGroupSizes = new int[0];
					return;
				}
				int num = value.Length - 1;
				for (int i = 0; i < num; i++)
				{
					if (value[i] < 1 || value[i] > 9)
					{
						throw new ArgumentOutOfRangeException("One of the elements in the array specified is not between 1 and 9");
					}
				}
				if (value[num] < 0 || value[num] > 9)
				{
					throw new ArgumentOutOfRangeException("Last element in the array specified is not between 0 and 9");
				}
				currencyGroupSizes = (int[])value.Clone();
			}
		}

		public int CurrencyNegativePattern
		{
			get
			{
				return currencyNegativePattern;
			}
			set
			{
				if (value < 0 || value > 15)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 15");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				currencyNegativePattern = value;
			}
		}

		public int CurrencyPositivePattern
		{
			get
			{
				return currencyPositivePattern;
			}
			set
			{
				if (value < 0 || value > 3)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 3");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				currencyPositivePattern = value;
			}
		}

		public string CurrencySymbol
		{
			get
			{
				return currencySymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				currencySymbol = value;
			}
		}

		public static NumberFormatInfo CurrentInfo
		{
			get
			{
				NumberFormatInfo numberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat;
				numberFormat.isReadOnly = true;
				return numberFormat;
			}
		}

		public static NumberFormatInfo InvariantInfo
		{
			get
			{
				NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
				numberFormatInfo.NumberNegativePattern = 1;
				numberFormatInfo.isReadOnly = true;
				return numberFormatInfo;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return isReadOnly;
			}
		}

		public string NaNSymbol
		{
			get
			{
				return nanSymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				nanSymbol = value;
			}
		}

		public string NegativeInfinitySymbol
		{
			get
			{
				return negativeInfinitySymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				negativeInfinitySymbol = value;
			}
		}

		public string NegativeSign
		{
			get
			{
				return negativeSign;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				negativeSign = value;
			}
		}

		public int NumberDecimalDigits
		{
			get
			{
				return numberDecimalDigits;
			}
			set
			{
				if (value < 0 || value > 99)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 99");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				numberDecimalDigits = value;
			}
		}

		public string NumberDecimalSeparator
		{
			get
			{
				return numberDecimalSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				numberDecimalSeparator = value;
			}
		}

		public string NumberGroupSeparator
		{
			get
			{
				return numberGroupSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				numberGroupSeparator = value;
			}
		}

		public int[] NumberGroupSizes
		{
			get
			{
				return (int[])RawNumberGroupSizes.Clone();
			}
			set
			{
				RawNumberGroupSizes = value;
			}
		}

		internal int[] RawNumberGroupSizes
		{
			get
			{
				return numberGroupSizes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				if (value.Length == 0)
				{
					numberGroupSizes = new int[0];
					return;
				}
				int num = value.Length - 1;
				for (int i = 0; i < num; i++)
				{
					if (value[i] < 1 || value[i] > 9)
					{
						throw new ArgumentOutOfRangeException("One of the elements in the array specified is not between 1 and 9");
					}
				}
				if (value[num] < 0 || value[num] > 9)
				{
					throw new ArgumentOutOfRangeException("Last element in the array specified is not between 0 and 9");
				}
				numberGroupSizes = (int[])value.Clone();
			}
		}

		public int NumberNegativePattern
		{
			get
			{
				return numberNegativePattern;
			}
			set
			{
				if (value < 0 || value > 4)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 15");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				numberNegativePattern = value;
			}
		}

		public int PercentDecimalDigits
		{
			get
			{
				return percentDecimalDigits;
			}
			set
			{
				if (value < 0 || value > 99)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 99");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				percentDecimalDigits = value;
			}
		}

		public string PercentDecimalSeparator
		{
			get
			{
				return percentDecimalSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				percentDecimalSeparator = value;
			}
		}

		public string PercentGroupSeparator
		{
			get
			{
				return percentGroupSeparator;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				percentGroupSeparator = value;
			}
		}

		public int[] PercentGroupSizes
		{
			get
			{
				return (int[])RawPercentGroupSizes.Clone();
			}
			set
			{
				RawPercentGroupSizes = value;
			}
		}

		internal int[] RawPercentGroupSizes
		{
			get
			{
				return percentGroupSizes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				if (this == CultureInfo.CurrentCulture.NumberFormat)
				{
					throw new Exception("HERE the value was modified");
				}
				if (value.Length == 0)
				{
					percentGroupSizes = new int[0];
					return;
				}
				int num = value.Length - 1;
				for (int i = 0; i < num; i++)
				{
					if (value[i] < 1 || value[i] > 9)
					{
						throw new ArgumentOutOfRangeException("One of the elements in the array specified is not between 1 and 9");
					}
				}
				if (value[num] < 0 || value[num] > 9)
				{
					throw new ArgumentOutOfRangeException("Last element in the array specified is not between 0 and 9");
				}
				percentGroupSizes = (int[])value.Clone();
			}
		}

		public int PercentNegativePattern
		{
			get
			{
				return percentNegativePattern;
			}
			set
			{
				if (value < 0 || value > 2)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 15");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				percentNegativePattern = value;
			}
		}

		public int PercentPositivePattern
		{
			get
			{
				return percentPositivePattern;
			}
			set
			{
				if (value < 0 || value > 2)
				{
					throw new ArgumentOutOfRangeException("The value specified for the property is less than 0 or greater than 3");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				percentPositivePattern = value;
			}
		}

		public string PercentSymbol
		{
			get
			{
				return percentSymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				percentSymbol = value;
			}
		}

		public string PerMilleSymbol
		{
			get
			{
				return perMilleSymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				perMilleSymbol = value;
			}
		}

		public string PositiveInfinitySymbol
		{
			get
			{
				return positiveInfinitySymbol;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				positiveInfinitySymbol = value;
			}
		}

		public string PositiveSign
		{
			get
			{
				return positiveSign;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("The value specified for the property is a null reference");
				}
				if (isReadOnly)
				{
					throw new InvalidOperationException("The current instance is read-only and a set operation was attempted");
				}
				positiveSign = value;
			}
		}

		internal NumberFormatInfo(int lcid, bool read_only)
		{
			isReadOnly = read_only;
			if (lcid != 127)
			{
				lcid = 127;
			}
			int num = lcid;
			if (num == 127)
			{
				isReadOnly = false;
				currencyDecimalDigits = 2;
				currencyDecimalSeparator = ".";
				currencyGroupSeparator = ",";
				currencyGroupSizes = new int[1] { 3 };
				currencyNegativePattern = 0;
				currencyPositivePattern = 0;
				currencySymbol = "$";
				nanSymbol = "NaN";
				negativeInfinitySymbol = "-Infinity";
				negativeSign = "-";
				numberDecimalDigits = 2;
				numberDecimalSeparator = ".";
				numberGroupSeparator = ",";
				numberGroupSizes = new int[1] { 3 };
				numberNegativePattern = 1;
				percentDecimalDigits = 2;
				percentDecimalSeparator = ".";
				percentGroupSeparator = ",";
				percentGroupSizes = new int[1] { 3 };
				percentNegativePattern = 0;
				percentPositivePattern = 0;
				percentSymbol = "%";
				perMilleSymbol = "‰";
				positiveInfinitySymbol = "Infinity";
				positiveSign = "+";
			}
		}

		internal NumberFormatInfo(bool read_only)
			: this(127, read_only)
		{
		}

		public NumberFormatInfo()
			: this(false)
		{
		}

		private void InitPatterns()
		{
			string[] array = decimalFormats.Split(new char[1] { ';' }, 2);
			string[] array2;
			string[] array3;
			if (array.Length == 2)
			{
				array2 = array[0].Split(new char[1] { '.' }, 2);
				if (array2.Length == 2)
				{
					numberDecimalDigits = 0;
					for (int i = 0; i < array2[1].Length && array2[1][i] == digitPattern[0]; i++)
					{
						numberDecimalDigits++;
					}
					array3 = array2[0].Split(',');
					if (array3.Length > 1)
					{
						numberGroupSizes = new int[array3.Length - 1];
						for (int j = 0; j < numberGroupSizes.Length; j++)
						{
							string text = array3[j + 1];
							numberGroupSizes[j] = text.Length;
						}
					}
					else
					{
						numberGroupSizes = new int[1];
					}
					if (array[1].StartsWith("(") && array[1].EndsWith(")"))
					{
						numberNegativePattern = 0;
					}
					else if (array[1].StartsWith("- "))
					{
						numberNegativePattern = 2;
					}
					else if (array[1].StartsWith("-"))
					{
						numberNegativePattern = 1;
					}
					else if (array[1].EndsWith(" -"))
					{
						numberNegativePattern = 4;
					}
					else if (array[1].EndsWith("-"))
					{
						numberNegativePattern = 3;
					}
					else
					{
						numberNegativePattern = 1;
					}
				}
			}
			array = currencyFormats.Split(new char[1] { ';' }, 2);
			if (array.Length == 2)
			{
				array2 = array[0].Split(new char[1] { '.' }, 2);
				if (array2.Length == 2)
				{
					currencyDecimalDigits = 0;
					for (int k = 0; k < array2[1].Length && array2[1][k] == zeroPattern[0]; k++)
					{
						currencyDecimalDigits++;
					}
					array3 = array2[0].Split(',');
					if (array3.Length > 1)
					{
						currencyGroupSizes = new int[array3.Length - 1];
						for (int l = 0; l < currencyGroupSizes.Length; l++)
						{
							string text2 = array3[l + 1];
							currencyGroupSizes[l] = text2.Length;
						}
					}
					else
					{
						currencyGroupSizes = new int[1];
					}
					if (array[1].StartsWith("(¤ ") && array[1].EndsWith(")"))
					{
						currencyNegativePattern = 14;
					}
					else if (array[1].StartsWith("(¤") && array[1].EndsWith(")"))
					{
						currencyNegativePattern = 0;
					}
					else if (array[1].StartsWith("¤ ") && array[1].EndsWith("-"))
					{
						currencyNegativePattern = 11;
					}
					else if (array[1].StartsWith("¤") && array[1].EndsWith("-"))
					{
						currencyNegativePattern = 3;
					}
					else if (array[1].StartsWith("(") && array[1].EndsWith(" ¤"))
					{
						currencyNegativePattern = 15;
					}
					else if (array[1].StartsWith("(") && array[1].EndsWith("¤"))
					{
						currencyNegativePattern = 4;
					}
					else if (array[1].StartsWith("-") && array[1].EndsWith(" ¤"))
					{
						currencyNegativePattern = 8;
					}
					else if (array[1].StartsWith("-") && array[1].EndsWith("¤"))
					{
						currencyNegativePattern = 5;
					}
					else if (array[1].StartsWith("-¤ "))
					{
						currencyNegativePattern = 9;
					}
					else if (array[1].StartsWith("-¤"))
					{
						currencyNegativePattern = 1;
					}
					else if (array[1].StartsWith("¤ -"))
					{
						currencyNegativePattern = 12;
					}
					else if (array[1].StartsWith("¤-"))
					{
						currencyNegativePattern = 2;
					}
					else if (array[1].EndsWith(" ¤-"))
					{
						currencyNegativePattern = 10;
					}
					else if (array[1].EndsWith("¤-"))
					{
						currencyNegativePattern = 7;
					}
					else if (array[1].EndsWith("- ¤"))
					{
						currencyNegativePattern = 13;
					}
					else if (array[1].EndsWith("-¤"))
					{
						currencyNegativePattern = 6;
					}
					else
					{
						currencyNegativePattern = 0;
					}
					if (array[0].StartsWith("¤ "))
					{
						currencyPositivePattern = 2;
					}
					else if (array[0].StartsWith("¤"))
					{
						currencyPositivePattern = 0;
					}
					else if (array[0].EndsWith(" ¤"))
					{
						currencyPositivePattern = 3;
					}
					else if (array[0].EndsWith("¤"))
					{
						currencyPositivePattern = 1;
					}
					else
					{
						currencyPositivePattern = 0;
					}
				}
			}
			if (percentFormats.StartsWith("%"))
			{
				percentPositivePattern = 2;
				percentNegativePattern = 2;
			}
			else if (percentFormats.EndsWith(" %"))
			{
				percentPositivePattern = 0;
				percentNegativePattern = 0;
			}
			else if (percentFormats.EndsWith("%"))
			{
				percentPositivePattern = 1;
				percentNegativePattern = 1;
			}
			else
			{
				percentPositivePattern = 0;
				percentNegativePattern = 0;
			}
			array2 = percentFormats.Split(new char[1] { '.' }, 2);
			if (array2.Length != 2)
			{
				return;
			}
			percentDecimalDigits = 0;
			for (int m = 0; m < array2[1].Length && array2[1][m] == digitPattern[0]; m++)
			{
				percentDecimalDigits++;
			}
			array3 = array2[0].Split(',');
			if (array3.Length > 1)
			{
				percentGroupSizes = new int[array3.Length - 1];
				for (int n = 0; n < percentGroupSizes.Length; n++)
				{
					string text3 = array3[n + 1];
					percentGroupSizes[n] = text3.Length;
				}
			}
			else
			{
				percentGroupSizes = new int[1];
			}
		}

		public object GetFormat(Type formatType)
		{
			return (formatType != typeof(NumberFormatInfo)) ? null : this;
		}

		public object Clone()
		{
			NumberFormatInfo numberFormatInfo = (NumberFormatInfo)MemberwiseClone();
			numberFormatInfo.isReadOnly = false;
			return numberFormatInfo;
		}

		public static NumberFormatInfo ReadOnly(NumberFormatInfo nfi)
		{
			NumberFormatInfo numberFormatInfo = (NumberFormatInfo)nfi.Clone();
			numberFormatInfo.isReadOnly = true;
			return numberFormatInfo;
		}

		public static NumberFormatInfo GetInstance(IFormatProvider formatProvider)
		{
			if (formatProvider != null)
			{
				NumberFormatInfo numberFormatInfo = (NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo));
				if (numberFormatInfo != null)
				{
					return numberFormatInfo;
				}
			}
			return CurrentInfo;
		}
	}
}
