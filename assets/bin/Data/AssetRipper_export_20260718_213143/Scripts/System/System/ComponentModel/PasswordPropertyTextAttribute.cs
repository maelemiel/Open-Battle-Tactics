namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class PasswordPropertyTextAttribute : Attribute
	{
		public static readonly PasswordPropertyTextAttribute Default;

		public static readonly PasswordPropertyTextAttribute No;

		public static readonly PasswordPropertyTextAttribute Yes;

		private bool _password;

		public bool Password
		{
			get
			{
				return _password;
			}
		}

		public PasswordPropertyTextAttribute()
			: this(false)
		{
		}

		public PasswordPropertyTextAttribute(bool password)
		{
			_password = password;
		}

		static PasswordPropertyTextAttribute()
		{
			No = new PasswordPropertyTextAttribute(false);
			Yes = new PasswordPropertyTextAttribute(true);
			Default = No;
		}

		public override bool Equals(object o)
		{
			if (!(o is PasswordPropertyTextAttribute))
			{
				return false;
			}
			return ((PasswordPropertyTextAttribute)o).Password == Password;
		}

		public override int GetHashCode()
		{
			return Password.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return Default.Equals(this);
		}
	}
}
