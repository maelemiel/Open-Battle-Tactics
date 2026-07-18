using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public struct TimeSpan : IComparable, IComparable<TimeSpan>, IEquatable<TimeSpan>
	{
		private class Parser
		{
			private string _src;

			private int _cur;

			private int _length;

			private bool formatError;

			public bool AtEnd
			{
				get
				{
					return _cur >= _length;
				}
			}

			public Parser(string src)
			{
				_src = src;
				_length = _src.Length;
			}

			private void ParseWhiteSpace()
			{
				while (!AtEnd && char.IsWhiteSpace(_src, _cur))
				{
					_cur++;
				}
			}

			private bool ParseSign()
			{
				bool result = false;
				if (!AtEnd && _src[_cur] == '-')
				{
					result = true;
					_cur++;
				}
				return result;
			}

			private int ParseInt(bool optional)
			{
				if (optional && AtEnd)
				{
					return 0;
				}
				int num = 0;
				int num2 = 0;
				while (!AtEnd && char.IsDigit(_src, _cur))
				{
					num = checked(num * 10 + _src[_cur] - 48);
					_cur++;
					num2++;
				}
				if (!optional && num2 == 0)
				{
					formatError = true;
				}
				return num;
			}

			private bool ParseOptDot()
			{
				if (AtEnd)
				{
					return false;
				}
				if (_src[_cur] == '.')
				{
					_cur++;
					return true;
				}
				return false;
			}

			private void ParseOptColon()
			{
				if (!AtEnd)
				{
					if (_src[_cur] == ':')
					{
						_cur++;
					}
					else
					{
						formatError = true;
					}
				}
			}

			private long ParseTicks()
			{
				long num = 1000000L;
				long num2 = 0L;
				bool flag = false;
				while (num > 0 && !AtEnd && char.IsDigit(_src, _cur))
				{
					num2 += (_src[_cur] - 48) * num;
					_cur++;
					num /= 10;
					flag = true;
				}
				if (!flag)
				{
					formatError = true;
				}
				return num2;
			}

			public TimeSpan Execute()
			{
				int num = 0;
				ParseWhiteSpace();
				bool flag = ParseSign();
				int num2 = ParseInt(false);
				if (ParseOptDot())
				{
					num = ParseInt(true);
				}
				else if (!AtEnd)
				{
					num = num2;
					num2 = 0;
				}
				ParseOptColon();
				int num3 = ParseInt(true);
				ParseOptColon();
				int num4 = ParseInt(true);
				long num5 = ((!ParseOptDot()) ? 0 : ParseTicks());
				ParseWhiteSpace();
				if (!AtEnd)
				{
					formatError = true;
				}
				if (num > 23 || num3 > 59 || num4 > 59)
				{
					throw new OverflowException(Locale.GetText("Invalid time data."));
				}
				if (formatError)
				{
					throw new FormatException(Locale.GetText("Invalid format for TimeSpan.Parse."));
				}
				long num6 = CalculateTicks(num2, num, num3, num4, 0);
				num6 = checked((!flag) ? (num6 + num5) : (-num6 - num5));
				return new TimeSpan(num6);
			}
		}

		public const long TicksPerDay = 864000000000L;

		public const long TicksPerHour = 36000000000L;

		public const long TicksPerMillisecond = 10000L;

		public const long TicksPerMinute = 600000000L;

		public const long TicksPerSecond = 10000000L;

		public static readonly TimeSpan MaxValue;

		public static readonly TimeSpan MinValue;

		public static readonly TimeSpan Zero;

		private long _ticks;

		public int Days
		{
			get
			{
				return (int)(_ticks / 864000000000L);
			}
		}

		public int Hours
		{
			get
			{
				return (int)(_ticks % 864000000000L / 36000000000L);
			}
		}

		public int Milliseconds
		{
			get
			{
				return (int)(_ticks % 10000000 / 10000);
			}
		}

		public int Minutes
		{
			get
			{
				return (int)(_ticks % 36000000000L / 600000000);
			}
		}

		public int Seconds
		{
			get
			{
				return (int)(_ticks % 600000000 / 10000000);
			}
		}

		public long Ticks
		{
			get
			{
				return _ticks;
			}
		}

		public double TotalDays
		{
			get
			{
				return (double)_ticks / 864000000000.0;
			}
		}

		public double TotalHours
		{
			get
			{
				return (double)_ticks / 36000000000.0;
			}
		}

		public double TotalMilliseconds
		{
			get
			{
				return (double)_ticks / 10000.0;
			}
		}

		public double TotalMinutes
		{
			get
			{
				return (double)_ticks / 600000000.0;
			}
		}

		public double TotalSeconds
		{
			get
			{
				return (double)_ticks / 10000000.0;
			}
		}

		public TimeSpan(long ticks)
		{
			_ticks = ticks;
		}

		public TimeSpan(int hours, int minutes, int seconds)
		{
			_ticks = CalculateTicks(0, hours, minutes, seconds, 0);
		}

		public TimeSpan(int days, int hours, int minutes, int seconds)
		{
			_ticks = CalculateTicks(days, hours, minutes, seconds, 0);
		}

		public TimeSpan(int days, int hours, int minutes, int seconds, int milliseconds)
		{
			_ticks = CalculateTicks(days, hours, minutes, seconds, milliseconds);
		}

		static TimeSpan()
		{
			MaxValue = new TimeSpan(long.MaxValue);
			MinValue = new TimeSpan(long.MinValue);
			Zero = new TimeSpan(0L);
			if (MonoTouchAOTHelper.FalseFlag)
			{
				GenericComparer<TimeSpan> genericComparer = new GenericComparer<TimeSpan>();
				GenericEqualityComparer<TimeSpan> genericEqualityComparer = new GenericEqualityComparer<TimeSpan>();
			}
		}

		internal static long CalculateTicks(int days, int hours, int minutes, int seconds, int milliseconds)
		{
			int num = hours * 3600;
			int num2 = minutes * 60;
			long num3 = (long)(num + num2 + seconds) * 1000L + milliseconds;
			num3 *= 10000;
			bool flag = false;
			if (days > 0)
			{
				long num4 = 864000000000L * days;
				if (num3 < 0)
				{
					long num5 = num3;
					num3 += num4;
					flag = num5 > num3;
				}
				else
				{
					num3 += num4;
					flag = num3 < 0;
				}
			}
			else if (days < 0)
			{
				long num6 = 864000000000L * days;
				if (num3 <= 0)
				{
					num3 += num6;
					flag = num3 > 0;
				}
				else
				{
					long num7 = num3;
					num3 += num6;
					flag = num3 > num7;
				}
			}
			if (flag)
			{
				throw new ArgumentOutOfRangeException(Locale.GetText("The timespan is too big or too small."));
			}
			return num3;
		}

		public TimeSpan Add(TimeSpan ts)
		{
			try
			{
				return new TimeSpan(checked(_ticks + ts.Ticks));
			}
			catch (OverflowException)
			{
				throw new OverflowException(Locale.GetText("Resulting timespan is too big."));
			}
		}

		public static int Compare(TimeSpan t1, TimeSpan t2)
		{
			if (t1._ticks < t2._ticks)
			{
				return -1;
			}
			if (t1._ticks > t2._ticks)
			{
				return 1;
			}
			return 0;
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is TimeSpan))
			{
				throw new ArgumentException(Locale.GetText("Argument has to be a TimeSpan."), "value");
			}
			return Compare(this, (TimeSpan)value);
		}

		public int CompareTo(TimeSpan value)
		{
			return Compare(this, value);
		}

		public bool Equals(TimeSpan obj)
		{
			return obj._ticks == _ticks;
		}

		public TimeSpan Duration()
		{
			try
			{
				return new TimeSpan(Math.Abs(_ticks));
			}
			catch (OverflowException)
			{
				throw new OverflowException(Locale.GetText("This TimeSpan value is MinValue so you cannot get the duration."));
			}
		}

		public override bool Equals(object value)
		{
			if (!(value is TimeSpan))
			{
				return false;
			}
			return _ticks == ((TimeSpan)value)._ticks;
		}

		public static bool Equals(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks == t2._ticks;
		}

		public static TimeSpan FromDays(double value)
		{
			return From(value, 864000000000L);
		}

		public static TimeSpan FromHours(double value)
		{
			return From(value, 36000000000L);
		}

		public static TimeSpan FromMinutes(double value)
		{
			return From(value, 600000000L);
		}

		public static TimeSpan FromSeconds(double value)
		{
			return From(value, 10000000L);
		}

		public static TimeSpan FromMilliseconds(double value)
		{
			return From(value, 10000L);
		}

		private static TimeSpan From(double value, long tickMultiplicator)
		{
			if (double.IsNaN(value))
			{
				throw new ArgumentException(Locale.GetText("Value cannot be NaN."), "value");
			}
			if (double.IsNegativeInfinity(value) || double.IsPositiveInfinity(value) || value < (double)MinValue.Ticks || value > (double)MaxValue.Ticks)
			{
				throw new OverflowException(Locale.GetText("Outside range [MinValue,MaxValue]"));
			}
			try
			{
				value *= (double)(tickMultiplicator / 10000);
				checked
				{
					long num = (long)Math.Round(value);
					return new TimeSpan(num * 10000);
				}
			}
			catch (OverflowException)
			{
				throw new OverflowException(Locale.GetText("Resulting timespan is too big."));
			}
		}

		public static TimeSpan FromTicks(long value)
		{
			return new TimeSpan(value);
		}

		public override int GetHashCode()
		{
			return _ticks.GetHashCode();
		}

		public TimeSpan Negate()
		{
			long ticks = _ticks;
			TimeSpan minValue = MinValue;
			if (ticks == minValue._ticks)
			{
				throw new OverflowException(Locale.GetText("This TimeSpan value is MinValue and cannot be negated."));
			}
			return new TimeSpan(-_ticks);
		}

		public static TimeSpan Parse(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			Parser parser = new Parser(s);
			return parser.Execute();
		}

		public static bool TryParse(string s, out TimeSpan result)
		{
			if (s == null)
			{
				result = Zero;
				return false;
			}
			try
			{
				result = Parse(s);
				return true;
			}
			catch
			{
				result = Zero;
				return false;
			}
		}

		public TimeSpan Subtract(TimeSpan ts)
		{
			try
			{
				return new TimeSpan(checked(_ticks - ts.Ticks));
			}
			catch (OverflowException)
			{
				throw new OverflowException(Locale.GetText("Resulting timespan is too big."));
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(14);
			if (_ticks < 0)
			{
				stringBuilder.Append('-');
			}
			if (Days != 0)
			{
				stringBuilder.Append(Math.Abs(Days));
				stringBuilder.Append('.');
			}
			stringBuilder.Append(Math.Abs(Hours).ToString("D2"));
			stringBuilder.Append(':');
			stringBuilder.Append(Math.Abs(Minutes).ToString("D2"));
			stringBuilder.Append(':');
			stringBuilder.Append(Math.Abs(Seconds).ToString("D2"));
			int num = (int)Math.Abs(_ticks % 10000000);
			if (num != 0)
			{
				stringBuilder.Append('.');
				stringBuilder.Append(num.ToString("D7"));
			}
			return stringBuilder.ToString();
		}

		public static TimeSpan operator +(TimeSpan t1, TimeSpan t2)
		{
			return t1.Add(t2);
		}

		public static bool operator ==(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks == t2._ticks;
		}

		public static bool operator >(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks > t2._ticks;
		}

		public static bool operator >=(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks >= t2._ticks;
		}

		public static bool operator !=(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks != t2._ticks;
		}

		public static bool operator <(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks < t2._ticks;
		}

		public static bool operator <=(TimeSpan t1, TimeSpan t2)
		{
			return t1._ticks <= t2._ticks;
		}

		public static TimeSpan operator -(TimeSpan t1, TimeSpan t2)
		{
			return t1.Subtract(t2);
		}

		public static TimeSpan operator -(TimeSpan t)
		{
			return t.Negate();
		}

		public static TimeSpan operator +(TimeSpan t)
		{
			return t;
		}
	}
}
