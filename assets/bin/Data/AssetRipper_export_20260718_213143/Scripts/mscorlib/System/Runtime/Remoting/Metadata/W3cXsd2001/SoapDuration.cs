using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	public sealed class SoapDuration
	{
		public static string XsdType
		{
			get
			{
				return "duration";
			}
		}

		public static TimeSpan Parse(string value)
		{
			if (value.Length == 0)
			{
				throw new ArgumentException("Invalid format string for duration schema datatype.");
			}
			int num = 0;
			if (value[0] == '-')
			{
				num = 1;
			}
			bool flag = num == 1;
			if (value[num] != 'P')
			{
				throw new ArgumentException("Invalid format string for duration schema datatype.");
			}
			num++;
			int num2 = 0;
			int num3 = 0;
			bool flag2 = false;
			int hours = 0;
			int minutes = 0;
			double value2 = 0.0;
			bool flag3 = false;
			int i = num;
			while (i < value.Length)
			{
				if (value[i] == 'T')
				{
					flag2 = true;
					num2 = 4;
					i++;
					num = i;
					continue;
				}
				bool flag4 = true;
				int num4 = 0;
				for (; i < value.Length; i++)
				{
					if (!char.IsDigit(value[i]))
					{
						if (value[i] != '.')
						{
							break;
						}
						flag4 = false;
						num4++;
						if (num4 > 1)
						{
							flag3 = true;
							break;
						}
					}
				}
				int num5 = -1;
				double num6 = -1.0;
				if (flag4)
				{
					num5 = int.Parse(value.Substring(num, i - num));
				}
				else
				{
					num6 = double.Parse(value.Substring(num, i - num), CultureInfo.InvariantCulture);
				}
				switch (value[i])
				{
				case 'Y':
					num3 += num5 * 365;
					if (num2 > 0 || !flag4)
					{
						flag3 = true;
					}
					else
					{
						num2 = 1;
					}
					break;
				case 'M':
					if (num2 < 2 && flag4)
					{
						num3 += 365 * (num5 / 12) + 30 * (num5 % 12);
						num2 = 2;
					}
					else if (flag2 && num2 < 6 && flag4)
					{
						minutes = num5;
						num2 = 6;
					}
					else
					{
						flag3 = true;
					}
					break;
				case 'D':
					num3 += num5;
					if (num2 > 2 || !flag4)
					{
						flag3 = true;
					}
					else
					{
						num2 = 3;
					}
					break;
				case 'H':
					hours = num5;
					if (!flag2 || num2 > 4 || !flag4)
					{
						flag3 = true;
					}
					else
					{
						num2 = 5;
					}
					break;
				case 'S':
					value2 = ((!flag4) ? num6 : ((double)num5));
					if (!flag2 || num2 > 6)
					{
						flag3 = true;
					}
					else
					{
						num2 = 7;
					}
					break;
				default:
					flag3 = true;
					break;
				}
				if (flag3)
				{
					break;
				}
				i++;
				num = i;
			}
			if (flag3)
			{
				throw new ArgumentException("Invalid format string for duration schema datatype.");
			}
			TimeSpan timeSpan = new TimeSpan(num3, hours, minutes, 0) + TimeSpan.FromSeconds(value2);
			return (!flag) ? timeSpan : (-timeSpan);
		}

		public static string ToString(TimeSpan timeSpan)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (timeSpan.Ticks < 0)
			{
				stringBuilder.Append('-');
				timeSpan = timeSpan.Negate();
			}
			stringBuilder.Append('P');
			if (timeSpan.Days > 0)
			{
				stringBuilder.Append(timeSpan.Days).Append('D');
			}
			if (timeSpan.Days > 0 || timeSpan.Minutes > 0 || timeSpan.Seconds > 0 || timeSpan.Milliseconds > 0)
			{
				stringBuilder.Append('T');
				if (timeSpan.Hours > 0)
				{
					stringBuilder.Append(timeSpan.Hours).Append('H');
				}
				if (timeSpan.Minutes > 0)
				{
					stringBuilder.Append(timeSpan.Minutes).Append('M');
				}
				if (timeSpan.Seconds > 0 || timeSpan.Milliseconds > 0)
				{
					double num = timeSpan.Seconds;
					if (timeSpan.Milliseconds > 0)
					{
						num += (double)timeSpan.Milliseconds / 1000.0;
					}
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0:0.0000000}", num));
					stringBuilder.Append('S');
				}
			}
			return stringBuilder.ToString();
		}
	}
}
