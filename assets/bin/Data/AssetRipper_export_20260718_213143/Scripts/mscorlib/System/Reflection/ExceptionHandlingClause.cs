using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public sealed class ExceptionHandlingClause
	{
		internal Type catch_type;

		internal int filter_offset;

		internal ExceptionHandlingClauseOptions flags;

		internal int try_offset;

		internal int try_length;

		internal int handler_offset;

		internal int handler_length;

		public Type CatchType
		{
			get
			{
				return catch_type;
			}
		}

		public int FilterOffset
		{
			get
			{
				return filter_offset;
			}
		}

		public ExceptionHandlingClauseOptions Flags
		{
			get
			{
				return flags;
			}
		}

		public int HandlerLength
		{
			get
			{
				return handler_length;
			}
		}

		public int HandlerOffset
		{
			get
			{
				return handler_offset;
			}
		}

		public int TryLength
		{
			get
			{
				return try_length;
			}
		}

		public int TryOffset
		{
			get
			{
				return try_offset;
			}
		}

		internal ExceptionHandlingClause()
		{
		}

		public override string ToString()
		{
			string text = string.Format("Flags={0}, TryOffset={1}, TryLength={2}, HandlerOffset={3}, HandlerLength={4}", flags, try_offset, try_length, handler_offset, handler_length);
			if (catch_type != null)
			{
				text = string.Format("{0}, CatchType={1}", text, catch_type);
			}
			if (flags == ExceptionHandlingClauseOptions.Filter)
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0}, FilterOffset={1}", text, filter_offset);
			}
			return text;
		}
	}
}
