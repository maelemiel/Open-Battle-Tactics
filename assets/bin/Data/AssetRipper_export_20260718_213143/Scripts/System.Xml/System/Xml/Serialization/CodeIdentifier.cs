using System.Globalization;

namespace System.Xml.Serialization
{
	public class CodeIdentifier
	{
		[Obsolete("Design mistake. It only contains static methods.")]
		public CodeIdentifier()
		{
		}

		public static string MakeCamel(string identifier)
		{
			string text = MakeValid(identifier);
			return char.ToLower(text[0], CultureInfo.InvariantCulture) + text.Substring(1);
		}

		public static string MakePascal(string identifier)
		{
			string text = MakeValid(identifier);
			return char.ToUpper(text[0], CultureInfo.InvariantCulture) + text.Substring(1);
		}

		public static string MakeValid(string identifier)
		{
			if (identifier == null)
			{
				throw new NullReferenceException();
			}
			if (identifier.Length == 0)
			{
				return "Item";
			}
			string text = string.Empty;
			if (!char.IsLetter(identifier[0]) && identifier[0] != '_')
			{
				text = "Item";
			}
			foreach (char c in identifier)
			{
				if (char.IsLetterOrDigit(c) || c == '_')
				{
					text += c;
				}
			}
			if (text.Length > 400)
			{
				text = text.Substring(0, 400);
			}
			return text;
		}
	}
}
