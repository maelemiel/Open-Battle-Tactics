namespace System.Text.RegularExpressions
{
	internal class Disassembler
	{
		public static void DisassemblePattern(ushort[] image)
		{
			DisassembleBlock(image, 0, 0);
		}

		public static void DisassembleBlock(ushort[] image, int pc, int depth)
		{
			while (pc < image.Length)
			{
				System.Text.RegularExpressions.OpCode op;
				System.Text.RegularExpressions.OpFlags flags;
				System.Text.RegularExpressions.PatternCompiler.DecodeOp(image[pc], out op, out flags);
				Console.Write(FormatAddress(pc) + ": ");
				Console.Write(new string(' ', depth * 2));
				Console.Write(DisassembleOp(image, pc));
				Console.WriteLine();
				int num;
				switch (op)
				{
				case System.Text.RegularExpressions.OpCode.False:
				case System.Text.RegularExpressions.OpCode.True:
				case System.Text.RegularExpressions.OpCode.Until:
					num = 1;
					break;
				case System.Text.RegularExpressions.OpCode.Position:
				case System.Text.RegularExpressions.OpCode.Reference:
				case System.Text.RegularExpressions.OpCode.Character:
				case System.Text.RegularExpressions.OpCode.Category:
				case System.Text.RegularExpressions.OpCode.NotCategory:
				case System.Text.RegularExpressions.OpCode.In:
				case System.Text.RegularExpressions.OpCode.Open:
				case System.Text.RegularExpressions.OpCode.Close:
				case System.Text.RegularExpressions.OpCode.Sub:
				case System.Text.RegularExpressions.OpCode.Branch:
				case System.Text.RegularExpressions.OpCode.Jump:
					num = 2;
					break;
				case System.Text.RegularExpressions.OpCode.Range:
				case System.Text.RegularExpressions.OpCode.Balance:
				case System.Text.RegularExpressions.OpCode.IfDefined:
				case System.Text.RegularExpressions.OpCode.Test:
				case System.Text.RegularExpressions.OpCode.Anchor:
					num = 3;
					break;
				case System.Text.RegularExpressions.OpCode.Repeat:
				case System.Text.RegularExpressions.OpCode.FastRepeat:
				case System.Text.RegularExpressions.OpCode.Info:
					num = 4;
					break;
				case System.Text.RegularExpressions.OpCode.String:
					num = image[pc + 1] + 2;
					break;
				case System.Text.RegularExpressions.OpCode.Set:
					num = image[pc + 2] + 3;
					break;
				default:
					num = 1;
					break;
				}
				pc += num;
			}
		}

		public static string DisassembleOp(ushort[] image, int pc)
		{
			System.Text.RegularExpressions.OpCode op;
			System.Text.RegularExpressions.OpFlags flags;
			System.Text.RegularExpressions.PatternCompiler.DecodeOp(image[pc], out op, out flags);
			string text = op.ToString();
			if (flags != System.Text.RegularExpressions.OpFlags.None)
			{
				text = text + "[" + flags.ToString("f") + "]";
			}
			switch (op)
			{
			case System.Text.RegularExpressions.OpCode.Info:
			{
				text = text + " " + image[pc + 1];
				string text2 = text;
				text = text2 + " (" + image[pc + 2] + ", " + image[pc + 3] + ")";
				break;
			}
			case System.Text.RegularExpressions.OpCode.Character:
				text = text + " '" + FormatChar((char)image[pc + 1]) + "'";
				break;
			case System.Text.RegularExpressions.OpCode.Category:
			case System.Text.RegularExpressions.OpCode.NotCategory:
				text = text + " /" + (System.Text.RegularExpressions.Category)image[pc + 1];
				break;
			case System.Text.RegularExpressions.OpCode.Range:
				text = text + " '" + FormatChar((char)image[pc + 1]) + "', ";
				text = text + " '" + FormatChar((char)image[pc + 2]) + "'";
				break;
			case System.Text.RegularExpressions.OpCode.Set:
				text = text + " " + FormatSet(image, pc + 1);
				break;
			case System.Text.RegularExpressions.OpCode.String:
				text = text + " '" + ReadString(image, pc + 1) + "'";
				break;
			case System.Text.RegularExpressions.OpCode.Position:
				text = text + " /" + (System.Text.RegularExpressions.Position)image[pc + 1];
				break;
			case System.Text.RegularExpressions.OpCode.Reference:
			case System.Text.RegularExpressions.OpCode.Open:
			case System.Text.RegularExpressions.OpCode.Close:
				text = text + " " + image[pc + 1];
				break;
			case System.Text.RegularExpressions.OpCode.Balance:
			{
				string text2 = text;
				text = text2 + " " + image[pc + 1] + " " + image[pc + 2];
				break;
			}
			case System.Text.RegularExpressions.OpCode.IfDefined:
			case System.Text.RegularExpressions.OpCode.Anchor:
				text = text + " :" + FormatAddress(pc + image[pc + 1]);
				text = text + " " + image[pc + 2];
				break;
			case System.Text.RegularExpressions.OpCode.In:
			case System.Text.RegularExpressions.OpCode.Sub:
			case System.Text.RegularExpressions.OpCode.Branch:
			case System.Text.RegularExpressions.OpCode.Jump:
				text = text + " :" + FormatAddress(pc + image[pc + 1]);
				break;
			case System.Text.RegularExpressions.OpCode.Test:
				text = text + " :" + FormatAddress(pc + image[pc + 1]);
				text = text + ", :" + FormatAddress(pc + image[pc + 2]);
				break;
			case System.Text.RegularExpressions.OpCode.Repeat:
			case System.Text.RegularExpressions.OpCode.FastRepeat:
			{
				text = text + " :" + FormatAddress(pc + image[pc + 1]);
				string text2 = text;
				text = text2 + " (" + image[pc + 2] + ", ";
				text = ((image[pc + 3] != ushort.MaxValue) ? (text + image[pc + 3]) : (text + "Inf"));
				text += ")";
				break;
			}
			}
			return text;
		}

		private static string ReadString(ushort[] image, int pc)
		{
			int num = image[pc];
			char[] array = new char[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (char)image[pc + i + 1];
			}
			return new string(array);
		}

		private static string FormatAddress(int pc)
		{
			return pc.ToString("x4");
		}

		private static string FormatSet(ushort[] image, int pc)
		{
			int num = image[pc++];
			int num2 = (image[pc++] << 4) - 1;
			string text = "[";
			bool flag = false;
			char c = '\0';
			for (int i = 0; i <= num2; i++)
			{
				bool flag2 = (image[pc + (i >> 4)] & (1 << (i & 0xF))) != 0;
				if (flag2 && !flag)
				{
					c = (char)(num + i);
					flag = true;
				}
				else if (flag && (!flag2 || i == num2))
				{
					char c2 = (char)(num + i - 1);
					text += FormatChar(c);
					if (c2 != c)
					{
						text = text + "-" + FormatChar(c2);
					}
					flag = false;
				}
			}
			return text + "]";
		}

		private static string FormatChar(char c)
		{
			if (c == '-' || c == ']')
			{
				return "\\" + c;
			}
			if (char.IsLetterOrDigit(c) || char.IsSymbol(c))
			{
				return c.ToString();
			}
			if (char.IsControl(c))
			{
				return "^" + (char)(64 + c);
			}
			int num = c;
			return "\\u" + num.ToString("x4");
		}
	}
}
