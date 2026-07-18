using System;
using System.Text;

namespace Microsoft.Win32
{
	internal class ExpandString
	{
		private string value;

		public ExpandString(string s)
		{
			value = s;
		}

		public override string ToString()
		{
			return value;
		}

		public string Expand()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < value.Length; i++)
			{
				if (value[i] == '%')
				{
					int j;
					for (j = i + 1; j < value.Length; j++)
					{
						if (value[j] == '%')
						{
							string variable = value.Substring(i + 1, j - i - 1);
							stringBuilder.Append(Environment.GetEnvironmentVariable(variable));
							i += j;
							break;
						}
					}
					if (j == value.Length)
					{
						stringBuilder.Append('%');
					}
				}
				else
				{
					stringBuilder.Append(value[i]);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
