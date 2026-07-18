using System.Collections;

namespace System.Text.RegularExpressions
{
	internal class PatternCompiler : System.Text.RegularExpressions.ICompiler
	{
		private class PatternLinkStack : System.Text.RegularExpressions.LinkStack
		{
			private struct Link
			{
				public int base_addr;

				public int offset_addr;
			}

			private Link link;

			public int BaseAddress
			{
				set
				{
					link.base_addr = value;
				}
			}

			public int OffsetAddress
			{
				get
				{
					return link.offset_addr;
				}
				set
				{
					link.offset_addr = value;
				}
			}

			public int GetOffset(int target_addr)
			{
				return target_addr - link.base_addr;
			}

			protected override object GetCurrent()
			{
				return link;
			}

			protected override void SetCurrent(object l)
			{
				link = (Link)l;
			}
		}

		private ArrayList pgm;

		private int CurrentAddress
		{
			get
			{
				return pgm.Count;
			}
		}

		public PatternCompiler()
		{
			pgm = new ArrayList();
		}

		public static ushort EncodeOp(System.Text.RegularExpressions.OpCode op, System.Text.RegularExpressions.OpFlags flags)
		{
			return (ushort)((uint)op | (uint)(flags & (System.Text.RegularExpressions.OpFlags)65280));
		}

		public static void DecodeOp(ushort word, out System.Text.RegularExpressions.OpCode op, out System.Text.RegularExpressions.OpFlags flags)
		{
			op = (System.Text.RegularExpressions.OpCode)(word & 0xFF);
			flags = (System.Text.RegularExpressions.OpFlags)(word & 0xFF00);
		}

		public void Reset()
		{
			pgm.Clear();
		}

		public System.Text.RegularExpressions.IMachineFactory GetMachineFactory()
		{
			ushort[] array = new ushort[pgm.Count];
			pgm.CopyTo(array);
			return new System.Text.RegularExpressions.InterpreterFactory(array);
		}

		public void EmitFalse()
		{
			Emit(System.Text.RegularExpressions.OpCode.False);
		}

		public void EmitTrue()
		{
			Emit(System.Text.RegularExpressions.OpCode.True);
		}

		private void EmitCount(int count)
		{
			Emit((ushort)(count & 0xFFFF));
			Emit((ushort)((uint)count >> 16));
		}

		public void EmitCharacter(char c, bool negate, bool ignore, bool reverse)
		{
			Emit(System.Text.RegularExpressions.OpCode.Character, MakeFlags(negate, ignore, reverse, false));
			if (ignore)
			{
				c = char.ToLower(c);
			}
			Emit(c);
		}

		public void EmitCategory(System.Text.RegularExpressions.Category cat, bool negate, bool reverse)
		{
			Emit(System.Text.RegularExpressions.OpCode.Category, MakeFlags(negate, false, reverse, false));
			Emit((ushort)cat);
		}

		public void EmitNotCategory(System.Text.RegularExpressions.Category cat, bool negate, bool reverse)
		{
			Emit(System.Text.RegularExpressions.OpCode.NotCategory, MakeFlags(negate, false, reverse, false));
			Emit((ushort)cat);
		}

		public void EmitRange(char lo, char hi, bool negate, bool ignore, bool reverse)
		{
			Emit(System.Text.RegularExpressions.OpCode.Range, MakeFlags(negate, ignore, reverse, false));
			Emit(lo);
			Emit(hi);
		}

		public void EmitSet(char lo, BitArray set, bool negate, bool ignore, bool reverse)
		{
			Emit(System.Text.RegularExpressions.OpCode.Set, MakeFlags(negate, ignore, reverse, false));
			Emit(lo);
			int num = set.Length + 15 >> 4;
			Emit((ushort)num);
			int num2 = 0;
			while (num-- != 0)
			{
				ushort num3 = 0;
				for (int i = 0; i < 16; i++)
				{
					if (num2 >= set.Length)
					{
						break;
					}
					if (set[num2++])
					{
						num3 |= (ushort)(1 << i);
					}
				}
				Emit(num3);
			}
		}

		public void EmitString(string str, bool ignore, bool reverse)
		{
			Emit(System.Text.RegularExpressions.OpCode.String, MakeFlags(false, ignore, reverse, false));
			int length = str.Length;
			Emit((ushort)length);
			if (ignore)
			{
				str = str.ToLower();
			}
			for (int i = 0; i < length; i++)
			{
				Emit(str[i]);
			}
		}

		public void EmitPosition(System.Text.RegularExpressions.Position pos)
		{
			Emit(System.Text.RegularExpressions.OpCode.Position, System.Text.RegularExpressions.OpFlags.None);
			Emit((ushort)pos);
		}

		public void EmitOpen(int gid)
		{
			Emit(System.Text.RegularExpressions.OpCode.Open);
			Emit((ushort)gid);
		}

		public void EmitClose(int gid)
		{
			Emit(System.Text.RegularExpressions.OpCode.Close);
			Emit((ushort)gid);
		}

		public void EmitBalanceStart(int gid, int balance, bool capture, System.Text.RegularExpressions.LinkRef tail)
		{
			BeginLink(tail);
			Emit(System.Text.RegularExpressions.OpCode.BalanceStart);
			Emit((ushort)gid);
			Emit((ushort)balance);
			Emit((ushort)(capture ? 1u : 0u));
			EmitLink(tail);
		}

		public void EmitBalance()
		{
			Emit(System.Text.RegularExpressions.OpCode.Balance);
		}

		public void EmitReference(int gid, bool ignore, bool reverse)
		{
			Emit(System.Text.RegularExpressions.OpCode.Reference, MakeFlags(false, ignore, reverse, false));
			Emit((ushort)gid);
		}

		public void EmitIfDefined(int gid, System.Text.RegularExpressions.LinkRef tail)
		{
			BeginLink(tail);
			Emit(System.Text.RegularExpressions.OpCode.IfDefined);
			EmitLink(tail);
			Emit((ushort)gid);
		}

		public void EmitSub(System.Text.RegularExpressions.LinkRef tail)
		{
			BeginLink(tail);
			Emit(System.Text.RegularExpressions.OpCode.Sub);
			EmitLink(tail);
		}

		public void EmitTest(System.Text.RegularExpressions.LinkRef yes, System.Text.RegularExpressions.LinkRef tail)
		{
			BeginLink(yes);
			BeginLink(tail);
			Emit(System.Text.RegularExpressions.OpCode.Test);
			EmitLink(yes);
			EmitLink(tail);
		}

		public void EmitBranch(System.Text.RegularExpressions.LinkRef next)
		{
			BeginLink(next);
			Emit(System.Text.RegularExpressions.OpCode.Branch, System.Text.RegularExpressions.OpFlags.None);
			EmitLink(next);
		}

		public void EmitJump(System.Text.RegularExpressions.LinkRef target)
		{
			BeginLink(target);
			Emit(System.Text.RegularExpressions.OpCode.Jump, System.Text.RegularExpressions.OpFlags.None);
			EmitLink(target);
		}

		public void EmitRepeat(int min, int max, bool lazy, System.Text.RegularExpressions.LinkRef until)
		{
			BeginLink(until);
			Emit(System.Text.RegularExpressions.OpCode.Repeat, MakeFlags(false, false, false, lazy));
			EmitLink(until);
			EmitCount(min);
			EmitCount(max);
		}

		public void EmitUntil(System.Text.RegularExpressions.LinkRef repeat)
		{
			ResolveLink(repeat);
			Emit(System.Text.RegularExpressions.OpCode.Until);
		}

		public void EmitFastRepeat(int min, int max, bool lazy, System.Text.RegularExpressions.LinkRef tail)
		{
			BeginLink(tail);
			Emit(System.Text.RegularExpressions.OpCode.FastRepeat, MakeFlags(false, false, false, lazy));
			EmitLink(tail);
			EmitCount(min);
			EmitCount(max);
		}

		public void EmitIn(System.Text.RegularExpressions.LinkRef tail)
		{
			BeginLink(tail);
			Emit(System.Text.RegularExpressions.OpCode.In);
			EmitLink(tail);
		}

		public void EmitAnchor(bool reverse, int offset, System.Text.RegularExpressions.LinkRef tail)
		{
			BeginLink(tail);
			Emit(System.Text.RegularExpressions.OpCode.Anchor, MakeFlags(false, false, reverse, false));
			EmitLink(tail);
			Emit((ushort)offset);
		}

		public void EmitInfo(int count, int min, int max)
		{
			Emit(System.Text.RegularExpressions.OpCode.Info);
			EmitCount(count);
			EmitCount(min);
			EmitCount(max);
		}

		public System.Text.RegularExpressions.LinkRef NewLink()
		{
			return new PatternLinkStack();
		}

		public void ResolveLink(System.Text.RegularExpressions.LinkRef lref)
		{
			PatternLinkStack patternLinkStack = (PatternLinkStack)lref;
			while (patternLinkStack.Pop())
			{
				pgm[patternLinkStack.OffsetAddress] = (ushort)patternLinkStack.GetOffset(CurrentAddress);
			}
		}

		public void EmitBranchEnd()
		{
		}

		public void EmitAlternationEnd()
		{
		}

		private static System.Text.RegularExpressions.OpFlags MakeFlags(bool negate, bool ignore, bool reverse, bool lazy)
		{
			System.Text.RegularExpressions.OpFlags opFlags = System.Text.RegularExpressions.OpFlags.None;
			if (negate)
			{
				opFlags |= System.Text.RegularExpressions.OpFlags.Negate;
			}
			if (ignore)
			{
				opFlags |= System.Text.RegularExpressions.OpFlags.IgnoreCase;
			}
			if (reverse)
			{
				opFlags |= System.Text.RegularExpressions.OpFlags.RightToLeft;
			}
			if (lazy)
			{
				opFlags |= System.Text.RegularExpressions.OpFlags.Lazy;
			}
			return opFlags;
		}

		private void Emit(System.Text.RegularExpressions.OpCode op)
		{
			Emit(op, System.Text.RegularExpressions.OpFlags.None);
		}

		private void Emit(System.Text.RegularExpressions.OpCode op, System.Text.RegularExpressions.OpFlags flags)
		{
			Emit(EncodeOp(op, flags));
		}

		private void Emit(ushort word)
		{
			pgm.Add(word);
		}

		private void BeginLink(System.Text.RegularExpressions.LinkRef lref)
		{
			PatternLinkStack patternLinkStack = (PatternLinkStack)lref;
			patternLinkStack.BaseAddress = CurrentAddress;
		}

		private void EmitLink(System.Text.RegularExpressions.LinkRef lref)
		{
			PatternLinkStack patternLinkStack = (PatternLinkStack)lref;
			patternLinkStack.OffsetAddress = CurrentAddress;
			Emit((ushort)0);
			patternLinkStack.Push();
		}
	}
}
