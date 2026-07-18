namespace System
{
	[Serializable]
	public struct ConsoleKeyInfo
	{
		internal static ConsoleKeyInfo Empty = new ConsoleKeyInfo('\0', (ConsoleKey)0, false, false, false);

		private ConsoleKey key;

		private char keychar;

		private ConsoleModifiers modifiers;

		public ConsoleKey Key
		{
			get
			{
				return key;
			}
		}

		public char KeyChar
		{
			get
			{
				return keychar;
			}
		}

		public ConsoleModifiers Modifiers
		{
			get
			{
				return modifiers;
			}
		}

		public ConsoleKeyInfo(char keyChar, ConsoleKey key, bool shift, bool alt, bool control)
		{
			this.key = key;
			keychar = keyChar;
			modifiers = (ConsoleModifiers)0;
			SetModifiers(shift, alt, control);
		}

		internal ConsoleKeyInfo(ConsoleKeyInfo other)
		{
			key = other.key;
			keychar = other.keychar;
			modifiers = other.modifiers;
		}

		internal void SetKey(ConsoleKey key)
		{
			this.key = key;
		}

		internal void SetKeyChar(char keyChar)
		{
			keychar = keyChar;
		}

		internal void SetModifiers(bool shift, bool alt, bool control)
		{
			modifiers = (shift ? ConsoleModifiers.Shift : ((ConsoleModifiers)0));
			modifiers |= (ConsoleModifiers)(alt ? 1 : 0);
			modifiers |= (ConsoleModifiers)(control ? 4 : 0);
		}

		public override bool Equals(object value)
		{
			if (!(value is ConsoleKeyInfo))
			{
				return false;
			}
			return Equals((ConsoleKeyInfo)value);
		}

		public bool Equals(ConsoleKeyInfo obj)
		{
			return key == obj.key && obj.keychar == keychar && obj.modifiers == modifiers;
		}

		public override int GetHashCode()
		{
			return key.GetHashCode() ^ keychar.GetHashCode() ^ modifiers.GetHashCode();
		}

		public static bool operator ==(ConsoleKeyInfo a, ConsoleKeyInfo b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(ConsoleKeyInfo a, ConsoleKeyInfo b)
		{
			return !a.Equals(b);
		}
	}
}
