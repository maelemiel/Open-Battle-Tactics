using System.Reflection;

namespace System.Xml.Serialization
{
	internal class XmlTypeMapMember
	{
		private const int OPTIONAL = 1;

		private const int RETURN_VALUE = 2;

		private const int IGNORE = 4;

		private string _name;

		private int _index;

		private int _globalIndex;

		private TypeData _typeData;

		private MemberInfo _member;

		private MemberInfo _specifiedMember;

		private object _defaultValue = DBNull.Value;

		private string documentation;

		private int _flags;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public object DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				_defaultValue = value;
			}
		}

		public string Documentation
		{
			get
			{
				return documentation;
			}
			set
			{
				documentation = value;
			}
		}

		public TypeData TypeData
		{
			get
			{
				return _typeData;
			}
			set
			{
				_typeData = value;
			}
		}

		public int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = value;
			}
		}

		public int GlobalIndex
		{
			get
			{
				return _globalIndex;
			}
			set
			{
				_globalIndex = value;
			}
		}

		public bool IsOptionalValueType
		{
			get
			{
				return (_flags & 1) != 0;
			}
			set
			{
				_flags = ((!value) ? (_flags & -2) : (_flags | 1));
			}
		}

		public bool IsReturnValue
		{
			get
			{
				return (_flags & 2) != 0;
			}
			set
			{
				_flags = ((!value) ? (_flags & -3) : (_flags | 2));
			}
		}

		public bool Ignore
		{
			get
			{
				return (_flags & 4) != 0;
			}
			set
			{
				_flags = ((!value) ? (_flags & -5) : (_flags | 4));
			}
		}

		public virtual bool RequiresNullable
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly(Type type)
		{
			if (_member == null)
			{
				InitMember(type);
			}
			return _member is PropertyInfo && !((PropertyInfo)_member).CanWrite;
		}

		public static object GetValue(object ob, string name)
		{
			MemberInfo[] member = ob.GetType().GetMember(name, BindingFlags.Instance | BindingFlags.Public);
			if (member[0] is PropertyInfo)
			{
				return ((PropertyInfo)member[0]).GetValue(ob, null);
			}
			return ((FieldInfo)member[0]).GetValue(ob);
		}

		public object GetValue(object ob)
		{
			if (_member == null)
			{
				InitMember(ob.GetType());
			}
			if (_member is PropertyInfo)
			{
				return ((PropertyInfo)_member).GetValue(ob, null);
			}
			return ((FieldInfo)_member).GetValue(ob);
		}

		public void SetValue(object ob, object value)
		{
			if (_member == null)
			{
				InitMember(ob.GetType());
			}
			if (_member is PropertyInfo)
			{
				((PropertyInfo)_member).SetValue(ob, value, null);
			}
			else
			{
				((FieldInfo)_member).SetValue(ob, value);
			}
		}

		public static void SetValue(object ob, string name, object value)
		{
			MemberInfo[] member = ob.GetType().GetMember(name, BindingFlags.Instance | BindingFlags.Public);
			if (member[0] is PropertyInfo)
			{
				((PropertyInfo)member[0]).SetValue(ob, value, null);
			}
			else
			{
				((FieldInfo)member[0]).SetValue(ob, value);
			}
		}

		private void InitMember(Type type)
		{
			MemberInfo[] member = type.GetMember(_name, BindingFlags.Instance | BindingFlags.Public);
			_member = member[0];
			member = type.GetMember(_name + "Specified", BindingFlags.Instance | BindingFlags.Public);
			if (member.Length > 0)
			{
				_specifiedMember = member[0];
			}
			if (_specifiedMember is PropertyInfo && !((PropertyInfo)_specifiedMember).CanWrite)
			{
				_specifiedMember = null;
			}
		}

		public void CheckOptionalValueType(Type type)
		{
			if (_member == null)
			{
				InitMember(type);
			}
			IsOptionalValueType = _specifiedMember != null;
		}

		public bool GetValueSpecified(object ob)
		{
			if (_specifiedMember is PropertyInfo)
			{
				return (bool)((PropertyInfo)_specifiedMember).GetValue(ob, null);
			}
			return (bool)((FieldInfo)_specifiedMember).GetValue(ob);
		}

		public void SetValueSpecified(object ob, bool value)
		{
			if (_specifiedMember is PropertyInfo)
			{
				((PropertyInfo)_specifiedMember).SetValue(ob, value, null);
			}
			else
			{
				((FieldInfo)_specifiedMember).SetValue(ob, value);
			}
		}
	}
}
