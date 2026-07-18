using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Text.RegularExpressions.Syntax;

namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Regex : ISerializable
	{
		private class Adapter
		{
			private MatchEvaluator ev;

			public Adapter(MatchEvaluator ev)
			{
				this.ev = ev;
			}

			public void Evaluate(Match m, StringBuilder sb)
			{
				sb.Append(ev(m));
			}
		}

		private static System.Text.RegularExpressions.FactoryCache cache = new System.Text.RegularExpressions.FactoryCache(15);

		private System.Text.RegularExpressions.IMachineFactory machineFactory;

		private IDictionary mapping;

		private int group_count;

		private int gap;

		private bool refsInitialized;

		private string[] group_names;

		private int[] group_numbers;

		protected internal string pattern;

		protected internal RegexOptions roptions;

		[System.MonoTODO]
		internal Dictionary<string, int> capnames;

		[System.MonoTODO]
		internal Dictionary<int, int> caps;

		[System.MonoTODO]
		protected internal int capsize;

		[System.MonoTODO]
		protected internal string[] capslist;

		public static int CacheSize
		{
			get
			{
				return cache.Capacity;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("CacheSize");
				}
				cache.Capacity = value;
			}
		}

		public RegexOptions Options
		{
			get
			{
				return roptions;
			}
		}

		public bool RightToLeft
		{
			get
			{
				return (roptions & RegexOptions.RightToLeft) != 0;
			}
		}

		internal int GroupCount
		{
			get
			{
				return group_count;
			}
		}

		internal int Gap
		{
			get
			{
				return gap;
			}
		}

		private int[] GroupNumbers
		{
			get
			{
				if (group_numbers == null)
				{
					group_numbers = new int[1 + group_count];
					for (int i = 0; i < gap; i++)
					{
						group_numbers[i] = i;
					}
					for (int j = gap; j <= group_count; j++)
					{
						group_numbers[j] = int.Parse(group_names[j]);
					}
					return group_numbers;
				}
				return group_numbers;
			}
		}

		protected Regex()
		{
		}

		public Regex(string pattern)
			: this(pattern, RegexOptions.None)
		{
		}

		public Regex(string pattern, RegexOptions options)
		{
			if (pattern == null)
			{
				throw new ArgumentNullException("pattern");
			}
			validate_options(options);
			this.pattern = pattern;
			roptions = options;
			Init();
		}

		protected Regex(SerializationInfo info, StreamingContext context)
			: this(info.GetString("pattern"), (RegexOptions)(int)info.GetValue("options", typeof(RegexOptions)))
		{
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("pattern", ToString(), typeof(string));
			info.AddValue("options", Options, typeof(RegexOptions));
		}

		[System.MonoTODO]
		public static void CompileToAssembly(RegexCompilationInfo[] regexes, AssemblyName aname)
		{
			CompileToAssembly(regexes, aname, new CustomAttributeBuilder[0], null);
		}

		[System.MonoTODO]
		public static void CompileToAssembly(RegexCompilationInfo[] regexes, AssemblyName aname, CustomAttributeBuilder[] attribs)
		{
			CompileToAssembly(regexes, aname, attribs, null);
		}

		[System.MonoTODO]
		public static void CompileToAssembly(RegexCompilationInfo[] regexes, AssemblyName aname, CustomAttributeBuilder[] attribs, string resourceFile)
		{
			throw new NotImplementedException();
		}

		public static string Escape(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return System.Text.RegularExpressions.Syntax.Parser.Escape(str);
		}

		public static string Unescape(string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			return System.Text.RegularExpressions.Syntax.Parser.Unescape(str);
		}

		public static bool IsMatch(string input, string pattern)
		{
			return IsMatch(input, pattern, RegexOptions.None);
		}

		public static bool IsMatch(string input, string pattern, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.IsMatch(input);
		}

		public static Match Match(string input, string pattern)
		{
			return Match(input, pattern, RegexOptions.None);
		}

		public static Match Match(string input, string pattern, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Match(input);
		}

		public static MatchCollection Matches(string input, string pattern)
		{
			return Matches(input, pattern, RegexOptions.None);
		}

		public static MatchCollection Matches(string input, string pattern, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Matches(input);
		}

		public static string Replace(string input, string pattern, MatchEvaluator evaluator)
		{
			return Replace(input, pattern, evaluator, RegexOptions.None);
		}

		public static string Replace(string input, string pattern, MatchEvaluator evaluator, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Replace(input, evaluator);
		}

		public static string Replace(string input, string pattern, string replacement)
		{
			return Replace(input, pattern, replacement, RegexOptions.None);
		}

		public static string Replace(string input, string pattern, string replacement, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Replace(input, replacement);
		}

		public static string[] Split(string input, string pattern)
		{
			return Split(input, pattern, RegexOptions.None);
		}

		public static string[] Split(string input, string pattern, RegexOptions options)
		{
			Regex regex = new Regex(pattern, options);
			return regex.Split(input);
		}

		private static void validate_options(RegexOptions options)
		{
			if ((options & ~(RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.RightToLeft | RegexOptions.ECMAScript | RegexOptions.CultureInvariant)) != RegexOptions.None)
			{
				throw new ArgumentOutOfRangeException("options");
			}
			if ((options & RegexOptions.ECMAScript) != RegexOptions.None && (options & ~(RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ECMAScript)) != RegexOptions.None)
			{
				throw new ArgumentOutOfRangeException("options");
			}
		}

		private void Init()
		{
			machineFactory = cache.Lookup(pattern, roptions);
			if (machineFactory == null)
			{
				InitNewRegex();
				return;
			}
			group_count = machineFactory.GroupCount;
			gap = machineFactory.Gap;
			mapping = machineFactory.Mapping;
			group_names = machineFactory.NamesMapping;
		}

		private void InitNewRegex()
		{
			machineFactory = CreateMachineFactory(pattern, roptions);
			cache.Add(pattern, roptions, machineFactory);
			group_count = machineFactory.GroupCount;
			gap = machineFactory.Gap;
			mapping = machineFactory.Mapping;
			group_names = machineFactory.NamesMapping;
		}

		private static System.Text.RegularExpressions.IMachineFactory CreateMachineFactory(string pattern, RegexOptions options)
		{
			System.Text.RegularExpressions.Syntax.Parser parser = new System.Text.RegularExpressions.Syntax.Parser();
			System.Text.RegularExpressions.Syntax.RegularExpression regularExpression = parser.ParseRegularExpression(pattern, options);
			System.Text.RegularExpressions.ICompiler compiler = new System.Text.RegularExpressions.PatternCompiler();
			regularExpression.Compile(compiler, (options & RegexOptions.RightToLeft) != 0);
			System.Text.RegularExpressions.IMachineFactory machineFactory = compiler.GetMachineFactory();
			Hashtable hashtable = new Hashtable();
			machineFactory.Gap = parser.GetMapping(hashtable);
			machineFactory.Mapping = hashtable;
			machineFactory.NamesMapping = GetGroupNamesArray(machineFactory.GroupCount, machineFactory.Mapping);
			return machineFactory;
		}

		public string[] GetGroupNames()
		{
			string[] array = new string[1 + group_count];
			Array.Copy(group_names, array, 1 + group_count);
			return array;
		}

		public int[] GetGroupNumbers()
		{
			int[] array = new int[1 + group_count];
			Array.Copy(GroupNumbers, array, 1 + group_count);
			return array;
		}

		public string GroupNameFromNumber(int i)
		{
			i = GetGroupIndex(i);
			if (i < 0)
			{
				return string.Empty;
			}
			return group_names[i];
		}

		public int GroupNumberFromName(string name)
		{
			if (!mapping.Contains(name))
			{
				return -1;
			}
			int num = (int)mapping[name];
			if (num >= gap)
			{
				num = int.Parse(name);
			}
			return num;
		}

		internal int GetGroupIndex(int number)
		{
			if (number < gap)
			{
				return number;
			}
			if (gap > group_count)
			{
				return -1;
			}
			return Array.BinarySearch(GroupNumbers, gap, group_count - gap + 1, number);
		}

		private int default_startat(string input)
		{
			return (RightToLeft && input != null) ? input.Length : 0;
		}

		public bool IsMatch(string input)
		{
			return IsMatch(input, default_startat(input));
		}

		public bool IsMatch(string input, int startat)
		{
			return Match(input, startat).Success;
		}

		public Match Match(string input)
		{
			return Match(input, default_startat(input));
		}

		public Match Match(string input, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			return CreateMachine().Scan(this, input, startat, input.Length);
		}

		public Match Match(string input, int startat, int length)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			if (length < 0 || length > input.Length - startat)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			return CreateMachine().Scan(this, input, startat, startat + length);
		}

		public MatchCollection Matches(string input)
		{
			return Matches(input, default_startat(input));
		}

		public MatchCollection Matches(string input, int startat)
		{
			Match start = Match(input, startat);
			return new MatchCollection(start);
		}

		public string Replace(string input, MatchEvaluator evaluator)
		{
			return Replace(input, evaluator, int.MaxValue, default_startat(input));
		}

		public string Replace(string input, MatchEvaluator evaluator, int count)
		{
			return Replace(input, evaluator, count, default_startat(input));
		}

		public string Replace(string input, MatchEvaluator evaluator, int count, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (evaluator == null)
			{
				throw new ArgumentNullException("evaluator");
			}
			if (count < -1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			System.Text.RegularExpressions.BaseMachine baseMachine = (System.Text.RegularExpressions.BaseMachine)CreateMachine();
			if (RightToLeft)
			{
				return baseMachine.RTLReplace(this, input, evaluator, count, startat);
			}
			Adapter adapter = new Adapter(evaluator);
			return baseMachine.LTRReplace(this, input, adapter.Evaluate, count, startat);
		}

		public string Replace(string input, string replacement)
		{
			return Replace(input, replacement, int.MaxValue, default_startat(input));
		}

		public string Replace(string input, string replacement, int count)
		{
			return Replace(input, replacement, count, default_startat(input));
		}

		public string Replace(string input, string replacement, int count, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (replacement == null)
			{
				throw new ArgumentNullException("replacement");
			}
			if (count < -1)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			return CreateMachine().Replace(this, input, replacement, count, startat);
		}

		public string[] Split(string input)
		{
			return Split(input, int.MaxValue, default_startat(input));
		}

		public string[] Split(string input, int count)
		{
			return Split(input, count, default_startat(input));
		}

		public string[] Split(string input, int count, int startat)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (startat < 0 || startat > input.Length)
			{
				throw new ArgumentOutOfRangeException("startat");
			}
			return CreateMachine().Split(this, input, count, startat);
		}

		protected void InitializeReferences()
		{
			if (refsInitialized)
			{
				throw new NotSupportedException("This operation is only allowed once per object.");
			}
			refsInitialized = true;
			Init();
		}

		protected bool UseOptionR()
		{
			return (roptions & RegexOptions.RightToLeft) != 0;
		}

		public override string ToString()
		{
			return pattern;
		}

		private System.Text.RegularExpressions.IMachine CreateMachine()
		{
			return machineFactory.NewInstance();
		}

		private static string[] GetGroupNamesArray(int groupCount, IDictionary mapping)
		{
			string[] array = new string[groupCount + 1];
			IDictionaryEnumerator enumerator = mapping.GetEnumerator();
			while (enumerator.MoveNext())
			{
				array[(int)enumerator.Value] = (string)enumerator.Key;
			}
			return array;
		}
	}
}
