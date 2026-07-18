using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace System.Net.Mime
{
	public class ContentDisposition
	{
		private const string rfc822 = "dd MMM yyyy HH':'mm':'ss zz00";

		private string dispositionType;

		private StringDictionary parameters = new StringDictionary();

		public DateTime CreationDate
		{
			get
			{
				if (parameters.ContainsKey("creation-date"))
				{
					return DateTime.ParseExact(parameters["creation-date"], "dd MMM yyyy HH':'mm':'ss zz00", null);
				}
				return DateTime.MinValue;
			}
			set
			{
				if (value > DateTime.MinValue)
				{
					parameters["creation-date"] = value.ToString("dd MMM yyyy HH':'mm':'ss zz00");
				}
				else
				{
					parameters.Remove("modification-date");
				}
			}
		}

		public string DispositionType
		{
			get
			{
				return dispositionType;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (value.Length < 1)
				{
					throw new ArgumentException();
				}
				dispositionType = value;
			}
		}

		public string FileName
		{
			get
			{
				return parameters["filename"];
			}
			set
			{
				parameters["filename"] = value;
			}
		}

		public bool Inline
		{
			get
			{
				return string.Compare(dispositionType, "inline", true, CultureInfo.InvariantCulture) == 0;
			}
			set
			{
				if (value)
				{
					dispositionType = "inline";
				}
				else
				{
					dispositionType = "attachment";
				}
			}
		}

		public DateTime ModificationDate
		{
			get
			{
				if (parameters.ContainsKey("modification-date"))
				{
					return DateTime.ParseExact(parameters["modification-date"], "dd MMM yyyy HH':'mm':'ss zz00", null);
				}
				return DateTime.MinValue;
			}
			set
			{
				if (value > DateTime.MinValue)
				{
					parameters["modification-date"] = value.ToString("dd MMM yyyy HH':'mm':'ss zz00");
				}
				else
				{
					parameters.Remove("modification-date");
				}
			}
		}

		public StringDictionary Parameters
		{
			get
			{
				return parameters;
			}
		}

		public DateTime ReadDate
		{
			get
			{
				if (parameters.ContainsKey("read-date"))
				{
					return DateTime.ParseExact(parameters["read-date"], "dd MMM yyyy HH':'mm':'ss zz00", null);
				}
				return DateTime.MinValue;
			}
			set
			{
				if (value > DateTime.MinValue)
				{
					parameters["read-date"] = value.ToString("dd MMM yyyy HH':'mm':'ss zz00");
				}
				else
				{
					parameters.Remove("read-date");
				}
			}
		}

		public long Size
		{
			get
			{
				if (parameters.ContainsKey("size"))
				{
					return long.Parse(parameters["size"]);
				}
				return -1L;
			}
			set
			{
				if (value > -1)
				{
					parameters["size"] = value.ToString();
				}
				else
				{
					parameters.Remove("size");
				}
			}
		}

		public ContentDisposition()
			: this("attachment")
		{
		}

		public ContentDisposition(string disposition)
		{
			if (disposition == null)
			{
				throw new ArgumentNullException();
			}
			if (disposition.Length < 1)
			{
				throw new FormatException();
			}
			Size = -1L;
			try
			{
				int num = disposition.IndexOf(';');
				if (num < 0)
				{
					dispositionType = disposition.Trim();
					return;
				}
				string[] array = disposition.Split(';');
				dispositionType = array[0].Trim();
				for (int i = 1; i < array.Length; i++)
				{
					Parse(array[i]);
				}
			}
			catch
			{
				throw new FormatException();
			}
		}

		private void Parse(string pair)
		{
			if (pair != null && pair.Length >= 0)
			{
				string[] array = pair.Split('=');
				if (array.Length != 2)
				{
					throw new FormatException();
				}
				parameters.Add(array[0].Trim(), array[1].Trim());
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ContentDisposition);
		}

		private bool Equals(ContentDisposition other)
		{
			return other != null && ToString() == other.ToString();
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(DispositionType.ToLower());
			if (Parameters != null && Parameters.Count > 0)
			{
				bool flag = false;
				foreach (DictionaryEntry parameter in Parameters)
				{
					if (parameter.Value != null && parameter.Value.ToString().Length > 0)
					{
						stringBuilder.Append("; ");
						stringBuilder.Append(parameter.Key);
						stringBuilder.Append("=");
						string text = parameter.Key.ToString();
						string text2 = parameter.Value.ToString();
						flag = (((text == "filename" && text2.IndexOf(' ') != -1) || text.EndsWith("date")) ? true : false);
						if (flag)
						{
							stringBuilder.Append("\"");
						}
						stringBuilder.Append(text2);
						if (flag)
						{
							stringBuilder.Append("\"");
						}
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
