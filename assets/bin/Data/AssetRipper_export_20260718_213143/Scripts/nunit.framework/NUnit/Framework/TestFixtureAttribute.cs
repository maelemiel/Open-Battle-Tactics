using System;
using System.Collections;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public class TestFixtureAttribute : Attribute
	{
		private string description;

		private object[] arguments;

		private bool isIgnored;

		private string ignoreReason;

		private string category;

		private Type[] typeArgs;

		private bool argsSeparated;

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public string Category
		{
			get
			{
				return category;
			}
			set
			{
				category = value;
			}
		}

		public IList Categories
		{
			get
			{
				return (category == null) ? null : category.Split(',');
			}
		}

		public object[] Arguments
		{
			get
			{
				if (!argsSeparated)
				{
					SeparateArgs();
				}
				return arguments;
			}
		}

		public bool Ignore
		{
			get
			{
				return isIgnored;
			}
			set
			{
				isIgnored = value;
			}
		}

		public string IgnoreReason
		{
			get
			{
				return ignoreReason;
			}
			set
			{
				ignoreReason = value;
				isIgnored = ignoreReason != null && ignoreReason != string.Empty;
			}
		}

		public Type[] TypeArgs
		{
			get
			{
				if (!argsSeparated)
				{
					SeparateArgs();
				}
				return typeArgs;
			}
			set
			{
				typeArgs = value;
				argsSeparated = true;
			}
		}

		public TestFixtureAttribute()
			: this(null)
		{
		}

		public TestFixtureAttribute(params object[] arguments)
		{
			this.arguments = ((arguments == null) ? new object[0] : arguments);
			for (int i = 0; i < this.arguments.Length; i++)
			{
				if (arguments[i] is SpecialValue && (SpecialValue)arguments[i] == SpecialValue.Null)
				{
					arguments[i] = null;
				}
			}
		}

		private void SeparateArgs()
		{
			int num = 0;
			if (arguments != null)
			{
				object[] array = arguments;
				foreach (object obj in array)
				{
					if (obj is Type)
					{
						num++;
						continue;
					}
					break;
				}
				typeArgs = new Type[num];
				for (int j = 0; j < num; j++)
				{
					typeArgs[j] = (Type)arguments[j];
				}
				if (num > 0)
				{
					object[] array2 = new object[arguments.Length - num];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = arguments[num + j];
					}
					arguments = array2;
				}
			}
			else
			{
				typeArgs = new Type[0];
			}
			argsSeparated = true;
		}
	}
}
