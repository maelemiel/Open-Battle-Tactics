using System.Globalization;

namespace System.Text.RegularExpressions
{
	internal sealed class RxInterpreter : System.Text.RegularExpressions.BaseMachine
	{
		internal struct IntStack
		{
			private int[] values;

			private int count;

			public int Top
			{
				get
				{
					return values[count - 1];
				}
			}

			public int Count
			{
				get
				{
					return count;
				}
				set
				{
					if (value > count)
					{
						throw new SystemException("can only truncate the stack");
					}
					count = value;
				}
			}

			public int Pop()
			{
				return values[--count];
			}

			public void Push(int value)
			{
				if (values == null)
				{
					values = new int[8];
				}
				else if (count == values.Length)
				{
					int num = values.Length;
					num += num >> 1;
					int[] array = new int[num];
					for (int i = 0; i < count; i++)
					{
						array[i] = values[i];
					}
					values = array;
				}
				values[count++] = value;
			}
		}

		private class RepeatContext
		{
			private int start;

			private int min;

			private int max;

			private bool lazy;

			private int expr_pc;

			private RepeatContext previous;

			private int count;

			public int Count
			{
				get
				{
					return count;
				}
				set
				{
					count = value;
				}
			}

			public int Start
			{
				get
				{
					return start;
				}
				set
				{
					start = value;
				}
			}

			public bool IsMinimum
			{
				get
				{
					return min <= count;
				}
			}

			public bool IsMaximum
			{
				get
				{
					return max <= count;
				}
			}

			public bool IsLazy
			{
				get
				{
					return lazy;
				}
			}

			public int Expression
			{
				get
				{
					return expr_pc;
				}
			}

			public RepeatContext Previous
			{
				get
				{
					return previous;
				}
			}

			public RepeatContext(RepeatContext previous, int min, int max, bool lazy, int expr_pc)
			{
				this.previous = previous;
				this.min = min;
				this.max = max;
				this.lazy = lazy;
				this.expr_pc = expr_pc;
				start = -1;
				count = 0;
			}
		}

		private byte[] program;

		private string str;

		private int string_start;

		private int string_end;

		private int group_count;

		private int[] groups;

		private System.Text.RegularExpressions.EvalDelegate eval_del;

		private System.Text.RegularExpressions.Mark[] marks;

		private int mark_start;

		private int mark_end;

		private IntStack stack;

		private RepeatContext repeat;

		private RepeatContext deep;

		public static readonly bool trace_rx;

		public RxInterpreter(byte[] program, System.Text.RegularExpressions.EvalDelegate eval_del)
		{
			this.program = program;
			this.eval_del = eval_del;
			group_count = 1 + (program[1] | (program[2] << 8));
			groups = new int[group_count];
			stack = default(IntStack);
			ResetGroups();
		}

		private static int ReadInt(byte[] code, int pc)
		{
			int num = code[pc];
			num |= code[pc + 1] << 8;
			num |= code[pc + 2] << 16;
			return num | (code[pc + 3] << 24);
		}

		public override Match Scan(Regex regex, string text, int start, int end)
		{
			str = text;
			string_start = start;
			string_end = end;
			int strpos_result = 0;
			bool flag = ((eval_del == null) ? EvalByteCode(11, start, ref strpos_result) : eval_del(this, start, ref strpos_result));
			marks[groups[0]].End = strpos_result;
			if (flag)
			{
				return GenerateMatch(regex);
			}
			return Match.Empty;
		}

		private void Open(int gid, int ptr)
		{
			int num = groups[gid];
			if (num < mark_start || marks[num].IsDefined)
			{
				num = CreateMark(num);
				groups[gid] = num;
			}
			marks[num].Start = ptr;
		}

		private void Close(int gid, int ptr)
		{
			marks[groups[gid]].End = ptr;
		}

		private bool Balance(int gid, int balance_gid, bool capture, int ptr)
		{
			int num = groups[balance_gid];
			if (num == -1 || marks[num].Index < 0)
			{
				return false;
			}
			if (gid > 0 && capture)
			{
				Open(gid, marks[num].Index + marks[num].Length);
				Close(gid, ptr);
			}
			groups[balance_gid] = marks[num].Previous;
			return true;
		}

		private int Checkpoint()
		{
			mark_start = mark_end;
			return mark_start;
		}

		private void Backtrack(int cp)
		{
			for (int i = 0; i < groups.Length; i++)
			{
				int num = groups[i];
				while (cp <= num)
				{
					num = marks[num].Previous;
				}
				groups[i] = num;
			}
		}

		private void ResetGroups()
		{
			int num = groups.Length;
			if (marks == null)
			{
				marks = new System.Text.RegularExpressions.Mark[num];
			}
			for (int i = 0; i < num; i++)
			{
				groups[i] = i;
				marks[i].Start = -1;
				marks[i].End = -1;
				marks[i].Previous = -1;
			}
			mark_start = 0;
			mark_end = num;
		}

		private int GetLastDefined(int gid)
		{
			int num = groups[gid];
			while (num >= 0 && !marks[num].IsDefined)
			{
				num = marks[num].Previous;
			}
			return num;
		}

		private int CreateMark(int previous)
		{
			if (mark_end == marks.Length)
			{
				System.Text.RegularExpressions.Mark[] array = new System.Text.RegularExpressions.Mark[marks.Length * 2];
				marks.CopyTo(array, 0);
				marks = array;
			}
			int num = mark_end++;
			marks[num].Start = (marks[num].End = -1);
			marks[num].Previous = previous;
			return num;
		}

		private void GetGroupInfo(int gid, out int first_mark_index, out int n_caps)
		{
			first_mark_index = -1;
			n_caps = 0;
			for (int num = groups[gid]; num >= 0; num = marks[num].Previous)
			{
				if (marks[num].IsDefined)
				{
					if (first_mark_index < 0)
					{
						first_mark_index = num;
					}
					n_caps++;
				}
			}
		}

		private void PopulateGroup(Group g, int first_mark_index, int n_caps)
		{
			int num = 1;
			for (int previous = marks[first_mark_index].Previous; previous >= 0; previous = marks[previous].Previous)
			{
				if (marks[previous].IsDefined)
				{
					Capture cap = new Capture(str, marks[previous].Index, marks[previous].Length);
					g.Captures.SetValue(cap, n_caps - 1 - num);
					num++;
				}
			}
		}

		private Match GenerateMatch(Regex regex)
		{
			int first_mark_index;
			int n_caps;
			GetGroupInfo(0, out first_mark_index, out n_caps);
			if (!needs_groups_or_captures)
			{
				return new Match(regex, this, str, string_end, 0, marks[first_mark_index].Index, marks[first_mark_index].Length);
			}
			Match match = new Match(regex, this, str, string_end, groups.Length, marks[first_mark_index].Index, marks[first_mark_index].Length, n_caps);
			PopulateGroup(match, first_mark_index, n_caps);
			for (int i = 1; i < groups.Length; i++)
			{
				GetGroupInfo(i, out first_mark_index, out n_caps);
				Group g;
				if (first_mark_index < 0)
				{
					g = Group.Fail;
				}
				else
				{
					g = new Group(str, marks[first_mark_index].Index, marks[first_mark_index].Length, n_caps);
					PopulateGroup(g, first_mark_index, n_caps);
				}
				match.Groups.SetValue(g, i);
			}
			return match;
		}

		internal void SetStartOfMatch(int pos)
		{
			marks[groups[0]].Start = pos;
		}

		private static bool IsWordChar(char c)
		{
			return char.IsLetterOrDigit(c) || char.GetUnicodeCategory(c) == UnicodeCategory.ConnectorPunctuation;
		}

		private bool EvalByteCode(int pc, int strpos, ref int strpos_result)
		{
			int num = 0;
			while (true)
			{
				if (trace_rx)
				{
					Console.WriteLine("evaluating: {0} at pc: {1}, strpos: {2}, cge: {3}", (System.Text.RegularExpressions.RxOp)program[pc], pc, strpos, num);
				}
				RepeatContext repeatContext;
				int strpos_result3;
				int count2;
				int count3;
				int num3;
				switch ((System.Text.RegularExpressions.RxOp)program[pc])
				{
				case System.Text.RegularExpressions.RxOp.True:
					if (num != 0)
					{
						pc = num;
						num = 0;
						continue;
					}
					strpos_result = strpos;
					return true;
				case System.Text.RegularExpressions.RxOp.False:
					return false;
				case System.Text.RegularExpressions.RxOp.AnyPosition:
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.StartOfString:
					if (strpos != 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.StartOfLine:
					if (strpos == 0 || str[strpos - 1] == '\n')
					{
						pc++;
						continue;
					}
					return false;
				case System.Text.RegularExpressions.RxOp.StartOfScan:
					if (strpos != string_start)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.End:
					if (strpos == string_end || (strpos == string_end - 1 && str[strpos] == '\n'))
					{
						pc++;
						continue;
					}
					return false;
				case System.Text.RegularExpressions.RxOp.EndOfString:
					if (strpos != string_end)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.EndOfLine:
					if (strpos == string_end || str[strpos] == '\n')
					{
						pc++;
						continue;
					}
					return false;
				case System.Text.RegularExpressions.RxOp.WordBoundary:
					if (string_end == 0)
					{
						return false;
					}
					if (strpos == 0)
					{
						if (IsWordChar(str[strpos]))
						{
							pc++;
							continue;
						}
					}
					else if (strpos == string_end)
					{
						if (IsWordChar(str[strpos - 1]))
						{
							pc++;
							continue;
						}
					}
					else if (IsWordChar(str[strpos]) != IsWordChar(str[strpos - 1]))
					{
						pc++;
						continue;
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoWordBoundary:
					if (string_end == 0)
					{
						return false;
					}
					if (strpos == 0)
					{
						if (!IsWordChar(str[strpos]))
						{
							pc++;
							continue;
						}
					}
					else if (strpos == string_end)
					{
						if (!IsWordChar(str[strpos - 1]))
						{
							pc++;
							continue;
						}
					}
					else if (IsWordChar(str[strpos]) == IsWordChar(str[strpos - 1]))
					{
						pc++;
						continue;
					}
					return false;
				case System.Text.RegularExpressions.RxOp.Anchor:
				{
					int num15 = program[pc + 1] | (program[pc + 2] << 8);
					int num16 = program[pc + 3] | (program[pc + 4] << 8);
					int num17 = pc + 5;
					System.Text.RegularExpressions.RxOp rxOp = (System.Text.RegularExpressions.RxOp)(program[num17] & 0xFF);
					bool flag2 = false;
					if ((rxOp == System.Text.RegularExpressions.RxOp.String || rxOp == System.Text.RegularExpressions.RxOp.StringIgnoreCase) && pc + num15 == num17 + 2 + program[num17 + 1] + 1)
					{
						flag2 = true;
						if (trace_rx)
						{
							Console.WriteLine("  string anchor at {0}, offset {1}", num17, num16);
						}
					}
					pc += num15;
					if (program[pc] == 4)
					{
						if (strpos == 0)
						{
							int strpos_result8 = strpos;
							if (groups.Length > 1)
							{
								ResetGroups();
								marks[groups[0]].Start = strpos;
							}
							if (EvalByteCode(pc + 1, strpos, ref strpos_result8))
							{
								marks[groups[0]].Start = strpos;
								if (groups.Length > 1)
								{
									marks[groups[0]].End = strpos_result8;
								}
								strpos_result = strpos_result8;
								return true;
							}
						}
						return false;
					}
					int num4 = string_end + 1;
					while (strpos < num4)
					{
						if (flag2 && (rxOp == System.Text.RegularExpressions.RxOp.String || rxOp == System.Text.RegularExpressions.RxOp.StringIgnoreCase))
						{
							int strpos_result9 = strpos;
							if (!EvalByteCode(num17, strpos + num16, ref strpos_result9))
							{
								strpos++;
								continue;
							}
						}
						int strpos_result10 = strpos;
						if (groups.Length > 1)
						{
							ResetGroups();
							marks[groups[0]].Start = strpos;
						}
						if (EvalByteCode(pc, strpos, ref strpos_result10))
						{
							marks[groups[0]].Start = strpos;
							if (groups.Length > 1)
							{
								marks[groups[0]].End = strpos_result10;
							}
							strpos_result = strpos_result10;
							return true;
						}
						strpos++;
					}
					return false;
				}
				case System.Text.RegularExpressions.RxOp.AnchorReverse:
				{
					int i = program[pc + 3] | (program[pc + 4] << 8);
					pc += program[pc + 1] | (program[pc + 2] << 8);
					int num4 = 0;
					while (strpos >= 0)
					{
						int strpos_result12 = strpos;
						if (groups.Length > 1)
						{
							ResetGroups();
							marks[groups[0]].Start = strpos;
						}
						if (EvalByteCode(pc, strpos, ref strpos_result12))
						{
							marks[groups[0]].Start = strpos;
							if (groups.Length > 1)
							{
								marks[groups[0]].End = strpos_result12;
							}
							strpos_result = strpos_result12;
							return true;
						}
						strpos--;
					}
					return false;
				}
				case System.Text.RegularExpressions.RxOp.Reference:
				{
					int i = GetLastDefined(program[pc + 1] | (program[pc + 2] << 8));
					if (i < 0)
					{
						return false;
					}
					num3 = marks[i].Index;
					i = marks[i].Length;
					if (strpos + i > string_end)
					{
						return false;
					}
					for (int num4 = num3 + i; num3 < num4; num3++)
					{
						if (str[strpos] != str[num3])
						{
							return false;
						}
						strpos++;
					}
					pc += 3;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.ReferenceIgnoreCase:
				{
					int i = GetLastDefined(program[pc + 1] | (program[pc + 2] << 8));
					if (i < 0)
					{
						return false;
					}
					num3 = marks[i].Index;
					i = marks[i].Length;
					if (strpos + i > string_end)
					{
						return false;
					}
					for (int num4 = num3 + i; num3 < num4; num3++)
					{
						if (str[strpos] != str[num3] && char.ToLower(str[strpos]) != char.ToLower(str[num3]))
						{
							return false;
						}
						strpos++;
					}
					pc += 3;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.ReferenceReverse:
				{
					int i = GetLastDefined(program[pc + 1] | (program[pc + 2] << 8));
					if (i < 0)
					{
						return false;
					}
					num3 = marks[i].Index;
					i = marks[i].Length;
					if (strpos - i < 0)
					{
						return false;
					}
					int num24 = strpos - i;
					int num4 = num3 + i;
					while (num3 < num4)
					{
						if (str[num24] != str[num3])
						{
							return false;
						}
						num3++;
						num24++;
					}
					strpos -= i;
					pc += 3;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.IfDefined:
					pc = ((GetLastDefined(program[pc + 3] | (program[pc + 4] << 8)) < 0) ? (pc + (program[pc + 1] | (program[pc + 2] << 8))) : (pc + 5));
					continue;
				case System.Text.RegularExpressions.RxOp.SubExpression:
				{
					int strpos_result7 = 0;
					if (EvalByteCode(pc + 3, strpos, ref strpos_result7))
					{
						pc += program[pc + 1] | (program[pc + 2] << 8);
						strpos = strpos_result7;
						continue;
					}
					return false;
				}
				case System.Text.RegularExpressions.RxOp.Test:
				{
					int strpos_result4 = 0;
					pc = ((!EvalByteCode(pc + 5, strpos, ref strpos_result4)) ? (pc + (program[pc + 3] | (program[pc + 4] << 8))) : (pc + (program[pc + 1] | (program[pc + 2] << 8))));
					continue;
				}
				case System.Text.RegularExpressions.RxOp.OpenGroup:
					Open(program[pc + 1] | (program[pc + 2] << 8), strpos);
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.CloseGroup:
					Close(program[pc + 1] | (program[pc + 2] << 8), strpos);
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.BalanceStart:
				{
					int strpos_result11 = 0;
					if (EvalByteCode(pc + 8, strpos, ref strpos_result11))
					{
						int gid = program[pc + 1] | (program[pc + 2] << 8);
						int balance_gid = program[pc + 3] | (program[pc + 4] << 8);
						bool capture = program[pc + 5] > 0;
						if (Balance(gid, balance_gid, capture, strpos))
						{
							strpos = strpos_result11;
							pc += program[pc + 6] | (program[pc + 7] << 8);
							continue;
						}
					}
					goto IL_3e19;
				}
				case System.Text.RegularExpressions.RxOp.Jump:
					pc += program[pc + 1] | (program[pc + 2] << 8);
					continue;
				case System.Text.RegularExpressions.RxOp.TestCharGroup:
					num = pc + (program[pc + 1] | (program[pc + 2] << 8));
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.String:
				{
					num3 = pc + 2;
					int i = program[pc + 1];
					if (strpos + i > string_end)
					{
						return false;
					}
					int num4;
					for (num4 = num3 + i; num3 < num4; num3++)
					{
						if (str[strpos] != program[num3])
						{
							return false;
						}
						strpos++;
					}
					pc = num4;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.StringIgnoreCase:
				{
					num3 = pc + 2;
					int i = program[pc + 1];
					if (strpos + i > string_end)
					{
						return false;
					}
					int num4;
					for (num4 = num3 + i; num3 < num4; num3++)
					{
						if (str[strpos] != program[num3] && char.ToLower(str[strpos]) != program[num3])
						{
							return false;
						}
						strpos++;
					}
					pc = num4;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.StringReverse:
				{
					num3 = pc + 2;
					int i = program[pc + 1];
					if (strpos < i)
					{
						return false;
					}
					int num13 = strpos - i;
					int num4 = num3 + i;
					while (num3 < num4)
					{
						if (str[num13] != program[num3])
						{
							return false;
						}
						num3++;
						num13++;
					}
					strpos -= i;
					pc = num4;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.StringIgnoreCaseReverse:
				{
					num3 = pc + 2;
					int i = program[pc + 1];
					if (strpos < i)
					{
						return false;
					}
					int num18 = strpos - i;
					int num4 = num3 + i;
					while (num3 < num4)
					{
						if (str[num18] != program[num3] && char.ToLower(str[num18]) != program[num3])
						{
							return false;
						}
						num3++;
						num18++;
					}
					strpos -= i;
					pc = num4;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.UnicodeString:
				{
					num3 = pc + 3;
					int i = program[pc + 1] | (program[pc + 2] << 8);
					if (strpos + i > string_end)
					{
						return false;
					}
					int num4;
					for (num4 = num3 + i * 2; num3 < num4; num3 += 2)
					{
						int num6 = program[num3] | (program[num3 + 1] << 8);
						if (str[strpos] != num6)
						{
							return false;
						}
						strpos++;
					}
					pc = num4;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.UnicodeStringIgnoreCase:
				{
					num3 = pc + 3;
					int i = program[pc + 1] | (program[pc + 2] << 8);
					if (strpos + i > string_end)
					{
						return false;
					}
					int num4;
					for (num4 = num3 + i * 2; num3 < num4; num3 += 2)
					{
						int num32 = program[num3] | (program[num3 + 1] << 8);
						if (str[strpos] != num32 && char.ToLower(str[strpos]) != num32)
						{
							return false;
						}
						strpos++;
					}
					pc = num4;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.UnicodeStringReverse:
				{
					num3 = pc + 3;
					int i = program[pc + 1] | (program[pc + 2] << 8);
					if (strpos < i)
					{
						return false;
					}
					int num7 = strpos - i;
					int num4 = num3 + i * 2;
					while (num3 < num4)
					{
						int num8 = program[num3] | (program[num3 + 1] << 8);
						if (str[num7] != num8)
						{
							return false;
						}
						num3 += 2;
						num7 += 2;
					}
					strpos -= i;
					pc = num4;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.UnicodeStringIgnoreCaseReverse:
				{
					num3 = pc + 3;
					int i = program[pc + 1] | (program[pc + 2] << 8);
					if (strpos < i)
					{
						return false;
					}
					int num22 = strpos - i;
					int num4 = num3 + i * 2;
					while (num3 < num4)
					{
						int num23 = program[num3] | (program[num3 + 1] << 8);
						if (str[num22] != num23 && char.ToLower(str[num22]) != num23)
						{
							return false;
						}
						num3 += 2;
						num22 += 2;
					}
					strpos -= i;
					pc = num4;
					continue;
				}
				case System.Text.RegularExpressions.RxOp.Char:
					if (strpos < string_end)
					{
						char c44 = str[strpos];
						if (c44 == program[pc + 1])
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 2;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 2;
					continue;
				case System.Text.RegularExpressions.RxOp.Range:
					if (strpos < string_end)
					{
						char c11 = str[strpos];
						if (c11 >= program[pc + 1] && c11 <= program[pc + 2])
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 3;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeRange:
					if (strpos < string_end)
					{
						char c20 = str[strpos];
						if (c20 >= (program[pc + 1] | (program[pc + 2] << 8)) && c20 <= (program[pc + 3] | (program[pc + 4] << 8)))
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 5;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 5;
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeChar:
					if (strpos < string_end)
					{
						char c66 = str[strpos];
						if (c66 == (program[pc + 1] | (program[pc + 2] << 8)))
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 3;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryAny:
					if (strpos < string_end)
					{
						char c63 = str[strpos];
						if (c63 != '\n')
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryAnySingleline:
					if (strpos < string_end)
					{
						char c57 = str[strpos];
						strpos++;
						if (num != 0)
						{
							break;
						}
						pc++;
						continue;
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryWord:
					if (strpos < string_end)
					{
						char c19 = str[strpos];
						if (char.IsLetterOrDigit(c19) || char.GetUnicodeCategory(c19) == UnicodeCategory.ConnectorPunctuation)
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryDigit:
					if (strpos < string_end)
					{
						char c4 = str[strpos];
						if (char.IsDigit(c4))
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryWhiteSpace:
					if (strpos < string_end)
					{
						char c76 = str[strpos];
						if (char.IsWhiteSpace(c76))
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryEcmaWord:
					if (strpos < string_end)
					{
						char c54 = str[strpos];
						if (('a' <= c54 && c54 <= 'z') || ('A' <= c54 && c54 <= 'Z') || ('0' <= c54 && c54 <= '9') || c54 == '_')
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryEcmaWhiteSpace:
					if (strpos < string_end)
					{
						char c38 = str[strpos];
						if (c38 == ' ' || c38 == '\t' || c38 == '\n' || c38 == '\r' || c38 == '\f' || c38 == '\v')
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryUnicodeSpecials:
					if (strpos < string_end)
					{
						char c18 = str[strpos];
						if (('\ufeff' <= c18 && c18 <= '\ufeff') || ('\ufff0' <= c18 && c18 <= '\ufffd'))
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryUnicode:
					if (strpos < string_end)
					{
						char c27 = str[strpos];
						if (char.GetUnicodeCategory(c27) == (UnicodeCategory)program[pc + 1])
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 2;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 2;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryGeneral:
					if (strpos < string_end)
					{
						char c82 = str[strpos];
						if (System.Text.RegularExpressions.CategoryUtils.IsCategory((System.Text.RegularExpressions.Category)program[pc + 1], c82))
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 2;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 2;
					continue;
				case System.Text.RegularExpressions.RxOp.Bitmap:
					if (strpos < string_end)
					{
						char c62 = str[strpos];
						int num29 = c62;
						num29 -= program[pc + 1];
						int i = program[pc + 2];
						if (num29 >= 0 && num29 < i << 3 && (program[pc + 3 + (num29 >> 3)] & (1 << (num29 & 7))) != 0)
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 3 + program[pc + 2];
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3 + program[pc + 2];
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeBitmap:
					if (strpos < string_end)
					{
						char c49 = str[strpos];
						int num19 = c49;
						num19 -= program[pc + 1] | (program[pc + 2] << 8);
						int i = program[pc + 3] | (program[pc + 4] << 8);
						if (num19 >= 0 && num19 < i << 3 && (program[pc + 5 + (num19 >> 3)] & (1 << (num19 & 7))) != 0)
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
					continue;
				case System.Text.RegularExpressions.RxOp.CharIgnoreCase:
					if (strpos < string_end)
					{
						char c41 = char.ToLower(str[strpos]);
						if (c41 == program[pc + 1])
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 2;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 2;
					continue;
				case System.Text.RegularExpressions.RxOp.RangeIgnoreCase:
					if (strpos < string_end)
					{
						char c42 = char.ToLower(str[strpos]);
						if (c42 >= program[pc + 1] && c42 <= program[pc + 2])
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 3;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeRangeIgnoreCase:
					if (strpos < string_end)
					{
						char c3 = char.ToLower(str[strpos]);
						if (c3 >= (program[pc + 1] | (program[pc + 2] << 8)) && c3 <= (program[pc + 3] | (program[pc + 4] << 8)))
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 5;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 5;
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeCharIgnoreCase:
					if (strpos < string_end)
					{
						char c83 = char.ToLower(str[strpos]);
						if (c83 == (program[pc + 1] | (program[pc + 2] << 8)))
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 3;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.BitmapIgnoreCase:
					if (strpos < string_end)
					{
						char c70 = char.ToLower(str[strpos]);
						int num30 = c70;
						num30 -= program[pc + 1];
						int i = program[pc + 2];
						if (num30 >= 0 && num30 < i << 3 && (program[pc + 3 + (num30 >> 3)] & (1 << (num30 & 7))) != 0)
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 3 + program[pc + 2];
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3 + program[pc + 2];
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeBitmapIgnoreCase:
					if (strpos < string_end)
					{
						char c72 = char.ToLower(str[strpos]);
						int num31 = c72;
						num31 -= program[pc + 1] | (program[pc + 2] << 8);
						int i = program[pc + 3] | (program[pc + 4] << 8);
						if (num31 >= 0 && num31 < i << 3 && (program[pc + 5 + (num31 >> 3)] & (1 << (num31 & 7))) != 0)
						{
							strpos++;
							if (num != 0)
							{
								break;
							}
							pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
					continue;
				case System.Text.RegularExpressions.RxOp.NoChar:
					if (strpos < string_end)
					{
						char c64 = str[strpos];
						if (c64 != program[pc + 1])
						{
							pc += 2;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoRange:
					if (strpos < string_end)
					{
						char c58 = str[strpos];
						if (c58 < program[pc + 1] || c58 > program[pc + 2])
						{
							pc += 3;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeRange:
					if (strpos < string_end)
					{
						char c28 = str[strpos];
						if (c28 < (program[pc + 1] | (program[pc + 2] << 8)) || c28 > (program[pc + 3] | (program[pc + 4] << 8)))
						{
							pc += 5;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeChar:
					if (strpos < string_end)
					{
						char c36 = str[strpos];
						if (c36 != (program[pc + 1] | (program[pc + 2] << 8)))
						{
							pc += 3;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryAny:
					if (strpos < string_end)
					{
						char c13 = str[strpos];
						if (c13 == '\n')
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryAnySingleline:
					if (strpos < string_end)
					{
						char c14 = str[strpos];
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryWord:
					if (strpos < string_end)
					{
						char c9 = str[strpos];
						if (!char.IsLetterOrDigit(c9) && char.GetUnicodeCategory(c9) != UnicodeCategory.ConnectorPunctuation)
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryDigit:
					if (strpos < string_end)
					{
						char c = str[strpos];
						if (!char.IsDigit(c))
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryWhiteSpace:
					if (strpos < string_end)
					{
						char c81 = str[strpos];
						if (!char.IsWhiteSpace(c81))
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryEcmaWord:
					if (strpos < string_end)
					{
						char c55 = str[strpos];
						if (('a' > c55 || c55 > 'z') && ('A' > c55 || c55 > 'Z') && ('0' > c55 || c55 > '9') && c55 != '_')
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryEcmaWhiteSpace:
					if (strpos < string_end)
					{
						char c45 = str[strpos];
						if (c45 != ' ' && c45 != '\t' && c45 != '\n' && c45 != '\r' && c45 != '\f' && c45 != '\v')
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryUnicodeSpecials:
					if (strpos < string_end)
					{
						char c35 = str[strpos];
						if (('\ufeff' > c35 || c35 > '\ufeff') && ('\ufff0' > c35 || c35 > '\ufffd'))
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryUnicode:
					if (strpos < string_end)
					{
						char c31 = str[strpos];
						if (char.GetUnicodeCategory(c31) != (UnicodeCategory)program[pc + 1])
						{
							pc += 2;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryGeneral:
					if (strpos < string_end)
					{
						char c17 = str[strpos];
						if (!System.Text.RegularExpressions.CategoryUtils.IsCategory((System.Text.RegularExpressions.Category)program[pc + 1], c17))
						{
							pc += 2;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoBitmap:
					if (strpos < string_end)
					{
						char c12 = str[strpos];
						int num11 = c12;
						num11 -= program[pc + 1];
						int i = program[pc + 2];
						if (num11 < 0 || num11 >= i << 3 || (program[pc + 3 + (num11 >> 3)] & (1 << (num11 & 7))) == 0)
						{
							pc += 3 + program[pc + 2];
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeBitmap:
					if (strpos < string_end)
					{
						char c87 = str[strpos];
						int num33 = c87;
						num33 -= program[pc + 1] | (program[pc + 2] << 8);
						int i = program[pc + 3] | (program[pc + 4] << 8);
						if (num33 < 0 || num33 >= i << 3 || (program[pc + 5 + (num33 >> 3)] & (1 << (num33 & 7))) == 0)
						{
							pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCharIgnoreCase:
					if (strpos < string_end)
					{
						char c80 = char.ToLower(str[strpos]);
						if (c80 != program[pc + 1])
						{
							pc += 2;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoRangeIgnoreCase:
					if (strpos < string_end)
					{
						char c75 = char.ToLower(str[strpos]);
						if (c75 < program[pc + 1] || c75 > program[pc + 2])
						{
							pc += 3;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeRangeIgnoreCase:
					if (strpos < string_end)
					{
						char c67 = char.ToLower(str[strpos]);
						if (c67 < (program[pc + 1] | (program[pc + 2] << 8)) || c67 > (program[pc + 3] | (program[pc + 4] << 8)))
						{
							pc += 5;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeCharIgnoreCase:
					if (strpos < string_end)
					{
						char c61 = char.ToLower(str[strpos]);
						if (c61 != (program[pc + 1] | (program[pc + 2] << 8)))
						{
							pc += 3;
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoBitmapIgnoreCase:
					if (strpos < string_end)
					{
						char c53 = char.ToLower(str[strpos]);
						int num26 = c53;
						num26 -= program[pc + 1];
						int i = program[pc + 2];
						if (num26 < 0 || num26 >= i << 3 || (program[pc + 3 + (num26 >> 3)] & (1 << (num26 & 7))) == 0)
						{
							pc += 3 + program[pc + 2];
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeBitmapIgnoreCase:
					if (strpos < string_end)
					{
						char c50 = char.ToLower(str[strpos]);
						int num20 = c50;
						num20 -= program[pc + 1] | (program[pc + 2] << 8);
						int i = program[pc + 3] | (program[pc + 4] << 8);
						if (num20 < 0 || num20 >= i << 3 || (program[pc + 5 + (num20 >> 3)] & (1 << (num20 & 7))) == 0)
						{
							pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
							if (num == 0 || pc + 1 == num)
							{
								strpos++;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.CharReverse:
					if (strpos > 0)
					{
						char c40 = str[strpos - 1];
						if (c40 == program[pc + 1])
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 2;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 2;
					continue;
				case System.Text.RegularExpressions.RxOp.RangeReverse:
					if (strpos > 0)
					{
						char c32 = str[strpos - 1];
						if (c32 >= program[pc + 1] && c32 <= program[pc + 2])
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 3;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeRangeReverse:
					if (strpos > 0)
					{
						char c24 = str[strpos - 1];
						if (c24 >= (program[pc + 1] | (program[pc + 2] << 8)) && c24 <= (program[pc + 3] | (program[pc + 4] << 8)))
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 5;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 5;
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeCharReverse:
					if (strpos > 0)
					{
						char c7 = str[strpos - 1];
						if (c7 == (program[pc + 1] | (program[pc + 2] << 8)))
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 3;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryAnyReverse:
					if (strpos > 0)
					{
						char c2 = str[strpos - 1];
						if (c2 != '\n')
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryAnySinglelineReverse:
					if (strpos > 0)
					{
						char c5 = str[strpos - 1];
						strpos--;
						if (num != 0)
						{
							break;
						}
						pc++;
						continue;
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryWordReverse:
					if (strpos > 0)
					{
						char c79 = str[strpos - 1];
						if (char.IsLetterOrDigit(c79) || char.GetUnicodeCategory(c79) == UnicodeCategory.ConnectorPunctuation)
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryDigitReverse:
					if (strpos > 0)
					{
						char c74 = str[strpos - 1];
						if (char.IsDigit(c74))
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryWhiteSpaceReverse:
					if (strpos > 0)
					{
						char c71 = str[strpos - 1];
						if (char.IsWhiteSpace(c71))
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryEcmaWordReverse:
					if (strpos > 0)
					{
						char c56 = str[strpos - 1];
						if (('a' <= c56 && c56 <= 'z') || ('A' <= c56 && c56 <= 'Z') || ('0' <= c56 && c56 <= '9') || c56 == '_')
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryEcmaWhiteSpaceReverse:
					if (strpos > 0)
					{
						char c47 = str[strpos - 1];
						if (c47 == ' ' || c47 == '\t' || c47 == '\n' || c47 == '\r' || c47 == '\f' || c47 == '\v')
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryUnicodeSpecialsReverse:
					if (strpos > 0)
					{
						char c30 = str[strpos - 1];
						if (('\ufeff' <= c30 && c30 <= '\ufeff') || ('\ufff0' <= c30 && c30 <= '\ufffd'))
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc++;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc++;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryUnicodeReverse:
					if (strpos > 0)
					{
						char c23 = str[strpos - 1];
						if (char.GetUnicodeCategory(c23) == (UnicodeCategory)program[pc + 1])
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 2;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 2;
					continue;
				case System.Text.RegularExpressions.RxOp.CategoryGeneralReverse:
					if (strpos > 0)
					{
						char c16 = str[strpos - 1];
						if (System.Text.RegularExpressions.CategoryUtils.IsCategory((System.Text.RegularExpressions.Category)program[pc + 1], c16))
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 2;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 2;
					continue;
				case System.Text.RegularExpressions.RxOp.BitmapReverse:
					if (strpos > 0)
					{
						char c8 = str[strpos - 1];
						int num10 = c8;
						num10 -= program[pc + 1];
						int i = program[pc + 2];
						if (num10 >= 0 && num10 < i << 3 && (program[pc + 3 + (num10 >> 3)] & (1 << (num10 & 7))) != 0)
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 3 + program[pc + 2];
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3 + program[pc + 2];
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeBitmapReverse:
					if (strpos > 0)
					{
						char c88 = str[strpos - 1];
						int num34 = c88;
						num34 -= program[pc + 1] | (program[pc + 2] << 8);
						int i = program[pc + 3] | (program[pc + 4] << 8);
						if (num34 >= 0 && num34 < i << 3 && (program[pc + 5 + (num34 >> 3)] & (1 << (num34 & 7))) != 0)
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
					continue;
				case System.Text.RegularExpressions.RxOp.CharIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c84 = char.ToLower(str[strpos - 1]);
						if (c84 == program[pc + 1])
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 2;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 2;
					continue;
				case System.Text.RegularExpressions.RxOp.RangeIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c85 = char.ToLower(str[strpos - 1]);
						if (c85 >= program[pc + 1] && c85 <= program[pc + 2])
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 3;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeRangeIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c77 = char.ToLower(str[strpos - 1]);
						if (c77 >= (program[pc + 1] | (program[pc + 2] << 8)) && c77 <= (program[pc + 3] | (program[pc + 4] << 8)))
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 5;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 5;
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeCharIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c68 = char.ToLower(str[strpos - 1]);
						if (c68 == (program[pc + 1] | (program[pc + 2] << 8)))
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 3;
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3;
					continue;
				case System.Text.RegularExpressions.RxOp.BitmapIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c60 = char.ToLower(str[strpos - 1]);
						int num28 = c60;
						num28 -= program[pc + 1];
						int i = program[pc + 2];
						if (num28 >= 0 && num28 < i << 3 && (program[pc + 3 + (num28 >> 3)] & (1 << (num28 & 7))) != 0)
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 3 + program[pc + 2];
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 3 + program[pc + 2];
					continue;
				case System.Text.RegularExpressions.RxOp.UnicodeBitmapIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c51 = char.ToLower(str[strpos - 1]);
						int num21 = c51;
						num21 -= program[pc + 1] | (program[pc + 2] << 8);
						int i = program[pc + 3] | (program[pc + 4] << 8);
						if (num21 >= 0 && num21 < i << 3 && (program[pc + 5 + (num21 >> 3)] & (1 << (num21 & 7))) != 0)
						{
							strpos--;
							if (num != 0)
							{
								break;
							}
							pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
							continue;
						}
					}
					if (num == 0)
					{
						return false;
					}
					pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
					continue;
				case System.Text.RegularExpressions.RxOp.NoCharReverse:
					if (strpos > 0)
					{
						char c46 = str[strpos - 1];
						if (c46 != program[pc + 1])
						{
							pc += 2;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoRangeReverse:
					if (strpos > 0)
					{
						char c39 = str[strpos - 1];
						if (c39 < program[pc + 1] || c39 > program[pc + 2])
						{
							pc += 3;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeRangeReverse:
					if (strpos > 0)
					{
						char c34 = str[strpos - 1];
						if (c34 < (program[pc + 1] | (program[pc + 2] << 8)) || c34 > (program[pc + 3] | (program[pc + 4] << 8)))
						{
							pc += 5;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeCharReverse:
					if (strpos > 0)
					{
						char c29 = str[strpos - 1];
						if (c29 != (program[pc + 1] | (program[pc + 2] << 8)))
						{
							pc += 3;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryAnyReverse:
					if (strpos > 0)
					{
						char c22 = str[strpos - 1];
						if (c22 == '\n')
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryAnySinglelineReverse:
					if (strpos > 0)
					{
						char c25 = str[strpos - 1];
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryWordReverse:
					if (strpos > 0)
					{
						char c15 = str[strpos - 1];
						if (!char.IsLetterOrDigit(c15) && char.GetUnicodeCategory(c15) != UnicodeCategory.ConnectorPunctuation)
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryDigitReverse:
					if (strpos > 0)
					{
						char c10 = str[strpos - 1];
						if (!char.IsDigit(c10))
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryWhiteSpaceReverse:
					if (strpos > 0)
					{
						char c6 = str[strpos - 1];
						if (!char.IsWhiteSpace(c6))
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryEcmaWordReverse:
					if (strpos > 0)
					{
						char c86 = str[strpos - 1];
						if (('a' > c86 || c86 > 'z') && ('A' > c86 || c86 > 'Z') && ('0' > c86 || c86 > '9') && c86 != '_')
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryEcmaWhiteSpaceReverse:
					if (strpos > 0)
					{
						char c78 = str[strpos - 1];
						if (c78 != ' ' && c78 != '\t' && c78 != '\n' && c78 != '\r' && c78 != '\f' && c78 != '\v')
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryUnicodeSpecialsReverse:
					if (strpos > 0)
					{
						char c73 = str[strpos - 1];
						if (('\ufeff' > c73 || c73 > '\ufeff') && ('\ufff0' > c73 || c73 > '\ufffd'))
						{
							pc++;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryUnicodeReverse:
					if (strpos > 0)
					{
						char c69 = str[strpos - 1];
						if (char.GetUnicodeCategory(c69) != (UnicodeCategory)program[pc + 1])
						{
							pc += 2;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCategoryGeneralReverse:
					if (strpos > 0)
					{
						char c65 = str[strpos - 1];
						if (!System.Text.RegularExpressions.CategoryUtils.IsCategory((System.Text.RegularExpressions.Category)program[pc + 1], c65))
						{
							pc += 2;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoBitmapReverse:
					if (strpos > 0)
					{
						char c59 = str[strpos - 1];
						int num27 = c59;
						num27 -= program[pc + 1];
						int i = program[pc + 2];
						if (num27 < 0 || num27 >= i << 3 || (program[pc + 3 + (num27 >> 3)] & (1 << (num27 & 7))) == 0)
						{
							pc += 3 + program[pc + 2];
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeBitmapReverse:
					if (strpos > 0)
					{
						char c52 = str[strpos - 1];
						int num25 = c52;
						num25 -= program[pc + 1] | (program[pc + 2] << 8);
						int i = program[pc + 3] | (program[pc + 4] << 8);
						if (num25 < 0 || num25 >= i << 3 || (program[pc + 5 + (num25 >> 3)] & (1 << (num25 & 7))) == 0)
						{
							pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoCharIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c48 = char.ToLower(str[strpos - 1]);
						if (c48 != program[pc + 1])
						{
							pc += 2;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoRangeIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c43 = char.ToLower(str[strpos - 1]);
						if (c43 < program[pc + 1] || c43 > program[pc + 2])
						{
							pc += 3;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeRangeIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c37 = char.ToLower(str[strpos - 1]);
						if (c37 < (program[pc + 1] | (program[pc + 2] << 8)) || c37 > (program[pc + 3] | (program[pc + 4] << 8)))
						{
							pc += 5;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeCharIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c33 = char.ToLower(str[strpos - 1]);
						if (c33 != (program[pc + 1] | (program[pc + 2] << 8)))
						{
							pc += 3;
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoBitmapIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c26 = char.ToLower(str[strpos - 1]);
						int num14 = c26;
						num14 -= program[pc + 1];
						int i = program[pc + 2];
						if (num14 < 0 || num14 >= i << 3 || (program[pc + 3 + (num14 >> 3)] & (1 << (num14 & 7))) == 0)
						{
							pc += 3 + program[pc + 2];
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.NoUnicodeBitmapIgnoreCaseReverse:
					if (strpos > 0)
					{
						char c21 = char.ToLower(str[strpos - 1]);
						int num12 = c21;
						num12 -= program[pc + 1] | (program[pc + 2] << 8);
						int i = program[pc + 3] | (program[pc + 4] << 8);
						if (num12 < 0 || num12 >= i << 3 || (program[pc + 5 + (num12 >> 3)] & (1 << (num12 & 7))) == 0)
						{
							pc += 5 + (program[pc + 3] | (program[pc + 4] << 8));
							if (num == 0 || pc + 1 == num)
							{
								strpos--;
								if (pc + 1 == num)
								{
									break;
								}
								continue;
							}
							continue;
						}
					}
					return false;
				case System.Text.RegularExpressions.RxOp.Branch:
				{
					int strpos_result6 = 0;
					if (EvalByteCode(pc + 3, strpos, ref strpos_result6))
					{
						strpos_result = strpos_result6;
						return true;
					}
					pc += program[pc + 1] | (program[pc + 2] << 8);
					continue;
				}
				case System.Text.RegularExpressions.RxOp.Repeat:
				case System.Text.RegularExpressions.RxOp.RepeatLazy:
				{
					int strpos_result5 = 0;
					repeat = new RepeatContext(repeat, ReadInt(program, pc + 3), ReadInt(program, pc + 7), program[pc] == 153, pc + 11);
					int pc2 = pc + (program[pc + 1] | (program[pc + 2] << 8));
					if (!EvalByteCode(pc2, strpos, ref strpos_result5))
					{
						repeat = repeat.Previous;
						return false;
					}
					strpos = strpos_result5;
					strpos_result = strpos;
					return true;
				}
				case System.Text.RegularExpressions.RxOp.Until:
					repeatContext = repeat;
					strpos_result3 = 0;
					if (deep != repeatContext)
					{
						num3 = repeatContext.Start;
						count2 = repeatContext.Count;
						while (!repeatContext.IsMinimum)
						{
							repeatContext.Count++;
							repeatContext.Start = strpos;
							deep = repeatContext;
							if (!EvalByteCode(repeatContext.Expression, strpos, ref strpos_result3))
							{
								goto IL_38eb;
							}
							strpos = strpos_result3;
							if (deep == repeatContext)
							{
								continue;
							}
							goto case System.Text.RegularExpressions.RxOp.Balance;
						}
						if (strpos == repeatContext.Start)
						{
							repeat = repeatContext.Previous;
							deep = null;
							if (!EvalByteCode(pc + 1, strpos, ref strpos_result3))
							{
								repeat = repeatContext;
								goto IL_3e19;
							}
							strpos = strpos_result3;
						}
						else if (repeatContext.IsLazy)
						{
							while (true)
							{
								repeat = repeatContext.Previous;
								deep = null;
								int cp2 = Checkpoint();
								if (EvalByteCode(pc + 1, strpos, ref strpos_result3))
								{
									strpos = strpos_result3;
									break;
								}
								Backtrack(cp2);
								repeat = repeatContext;
								if (!repeatContext.IsMaximum)
								{
									repeatContext.Count++;
									repeatContext.Start = strpos;
									deep = repeatContext;
									if (!EvalByteCode(repeatContext.Expression, strpos, ref strpos_result3))
									{
										repeatContext.Start = num3;
										repeatContext.Count = count2;
									}
									else
									{
										strpos = strpos_result3;
										if (deep != repeatContext)
										{
											break;
										}
										if (strpos != repeatContext.Start)
										{
											continue;
										}
									}
								}
								goto IL_3e19;
							}
						}
						else
						{
							count3 = stack.Count;
							while (true)
							{
								if (!repeatContext.IsMaximum)
								{
									int num9 = Checkpoint();
									int value = strpos;
									int start = repeatContext.Start;
									repeatContext.Count++;
									if (trace_rx)
									{
										Console.WriteLine("recurse with count {0}.", repeatContext.Count);
									}
									repeatContext.Start = strpos;
									deep = repeatContext;
									if (!EvalByteCode(repeatContext.Expression, strpos, ref strpos_result3))
									{
										repeatContext.Count--;
										repeatContext.Start = start;
										Backtrack(num9);
									}
									else
									{
										strpos = strpos_result3;
										if (deep != repeatContext)
										{
											break;
										}
										stack.Push(num9);
										stack.Push(value);
										if (strpos != repeatContext.Start)
										{
											continue;
										}
									}
								}
								if (trace_rx)
								{
									Console.WriteLine("matching tail: {0} pc={1}", strpos, pc + 1);
								}
								repeat = repeatContext.Previous;
								goto IL_3b78;
							}
							stack.Count = count3;
						}
					}
					goto case System.Text.RegularExpressions.RxOp.Balance;
				case System.Text.RegularExpressions.RxOp.FastRepeat:
				case System.Text.RegularExpressions.RxOp.FastRepeatLazy:
				{
					bool flag = program[pc] == 156;
					int strpos_result2 = 0;
					int num2 = pc + (program[pc + 1] | (program[pc + 2] << 8));
					num3 = ReadInt(program, pc + 3);
					int num4 = ReadInt(program, pc + 7);
					int i = 0;
					deep = null;
					for (; i < num3; i++)
					{
						if (!EvalByteCode(pc + 11, strpos, ref strpos_result2))
						{
							return false;
						}
						strpos = strpos_result2;
					}
					if (flag)
					{
						while (true)
						{
							int cp = Checkpoint();
							if (EvalByteCode(num2, strpos, ref strpos_result2))
							{
								break;
							}
							Backtrack(cp);
							if (i >= num4)
							{
								return false;
							}
							if (!EvalByteCode(pc + 11, strpos, ref strpos_result2))
							{
								return false;
							}
							strpos = strpos_result2;
							i++;
						}
						strpos = strpos_result2;
					}
					else
					{
						int count = stack.Count;
						for (; i < num4; i++)
						{
							int num5 = Checkpoint();
							if (!EvalByteCode(pc + 11, strpos, ref strpos_result2))
							{
								Backtrack(num5);
								break;
							}
							stack.Push(num5);
							stack.Push(strpos);
							strpos = strpos_result2;
						}
						if (num2 <= pc)
						{
							throw new Exception();
						}
						while (!EvalByteCode(num2, strpos, ref strpos_result2))
						{
							if (stack.Count == count)
							{
								return false;
							}
							strpos = stack.Pop();
							Backtrack(stack.Pop());
							if (trace_rx)
							{
								Console.WriteLine("backtracking to: {0}", strpos);
							}
						}
						strpos = strpos_result2;
						stack.Count = count;
					}
					goto case System.Text.RegularExpressions.RxOp.Balance;
				}
				default:
					Console.WriteLine("evaluating: {0} at pc: {1}, strpos: {2}", (System.Text.RegularExpressions.RxOp)program[pc], pc, strpos);
					throw new NotSupportedException();
				case System.Text.RegularExpressions.RxOp.Balance:
					{
						strpos_result = strpos;
						return true;
					}
					IL_3b78:
					while (true)
					{
						deep = null;
						if (EvalByteCode(pc + 1, strpos, ref strpos_result3))
						{
							break;
						}
						if (stack.Count != count3)
						{
							repeatContext.Count--;
							strpos = stack.Pop();
							Backtrack(stack.Pop());
							if (trace_rx)
							{
								Console.WriteLine("backtracking to {0} expr={1} pc={2}", strpos, repeatContext.Expression, pc);
							}
							continue;
						}
						goto IL_3bb8;
					}
					strpos = strpos_result3;
					stack.Count = count3;
					goto case System.Text.RegularExpressions.RxOp.Balance;
					IL_3bb8:
					repeat = repeatContext;
					goto IL_3e19;
					IL_38eb:
					repeatContext.Start = num3;
					repeatContext.Count = count2;
					goto IL_3e19;
					IL_3e19:
					return false;
				}
				pc = num;
				num = 0;
			}
		}
	}
}
