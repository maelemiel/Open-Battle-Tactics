namespace System.Xml.Serialization
{
	internal sealed class TypeMember
	{
		private Type type;

		private string member;

		internal TypeMember(Type type, string member)
		{
			this.type = type;
			this.member = member;
		}

		public override int GetHashCode()
		{
			return type.GetHashCode() + member.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is TypeMember)
			{
				return Equals(this, (TypeMember)obj);
			}
			return false;
		}

		public static bool Equals(TypeMember tm1, TypeMember tm2)
		{
			if (object.ReferenceEquals(tm1, tm2))
			{
				return true;
			}
			if (object.ReferenceEquals(tm1, null) || object.ReferenceEquals(tm2, null))
			{
				return false;
			}
			if (tm1.type == tm2.type && tm1.member == tm2.member)
			{
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return type.ToString() + " " + member;
		}

		public static bool operator ==(TypeMember tm1, TypeMember tm2)
		{
			return Equals(tm1, tm2);
		}

		public static bool operator !=(TypeMember tm1, TypeMember tm2)
		{
			return !Equals(tm1, tm2);
		}
	}
}
