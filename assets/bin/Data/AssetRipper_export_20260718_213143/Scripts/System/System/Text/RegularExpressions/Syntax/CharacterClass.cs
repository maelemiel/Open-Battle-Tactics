using System.Collections;

namespace System.Text.RegularExpressions.Syntax
{
	internal class CharacterClass : System.Text.RegularExpressions.Syntax.Expression
	{
		private const int distance_between_upper_and_lower_case = 32;

		private static System.Text.RegularExpressions.Interval upper_case_characters = new System.Text.RegularExpressions.Interval(65, 90);

		private bool negate;

		private bool ignore;

		private BitArray pos_cats;

		private BitArray neg_cats;

		private System.Text.RegularExpressions.IntervalCollection intervals;

		public bool Negate
		{
			get
			{
				return negate;
			}
			set
			{
				negate = value;
			}
		}

		public bool IgnoreCase
		{
			get
			{
				return ignore;
			}
			set
			{
				ignore = value;
			}
		}

		public CharacterClass(bool negate, bool ignore)
		{
			this.negate = negate;
			this.ignore = ignore;
			intervals = new System.Text.RegularExpressions.IntervalCollection();
			int length = 144;
			pos_cats = new BitArray(length);
			neg_cats = new BitArray(length);
		}

		public CharacterClass(System.Text.RegularExpressions.Category cat, bool negate)
			: this(false, false)
		{
			AddCategory(cat, negate);
		}

		public void AddCategory(System.Text.RegularExpressions.Category cat, bool negate)
		{
			if (negate)
			{
				neg_cats[(int)cat] = true;
			}
			else
			{
				pos_cats[(int)cat] = true;
			}
		}

		public void AddCharacter(char c)
		{
			AddRange(c, c);
		}

		public void AddRange(char lo, char hi)
		{
			System.Text.RegularExpressions.Interval i = new System.Text.RegularExpressions.Interval(lo, hi);
			if (ignore)
			{
				if (upper_case_characters.Intersects(i))
				{
					System.Text.RegularExpressions.Interval i2;
					if (i.low < upper_case_characters.low)
					{
						i2 = new System.Text.RegularExpressions.Interval(upper_case_characters.low + 32, i.high + 32);
						i.high = upper_case_characters.low - 1;
					}
					else
					{
						i2 = new System.Text.RegularExpressions.Interval(i.low + 32, upper_case_characters.high + 32);
						i.low = upper_case_characters.high + 1;
					}
					intervals.Add(i2);
				}
				else if (upper_case_characters.Contains(i))
				{
					i.high += 32;
					i.low += 32;
				}
			}
			intervals.Add(i);
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			System.Text.RegularExpressions.IntervalCollection metaCollection = intervals.GetMetaCollection(GetIntervalCost);
			int num = metaCollection.Count;
			for (int i = 0; i < pos_cats.Length; i++)
			{
				if (pos_cats[i] || neg_cats[i])
				{
					num++;
				}
			}
			if (num == 0)
			{
				return;
			}
			System.Text.RegularExpressions.LinkRef linkRef = cmp.NewLink();
			if (num > 1)
			{
				cmp.EmitIn(linkRef);
			}
			foreach (System.Text.RegularExpressions.Interval item in metaCollection)
			{
				if (item.IsDiscontiguous)
				{
					BitArray bitArray = new BitArray(item.Size);
					foreach (System.Text.RegularExpressions.Interval interval in intervals)
					{
						if (item.Contains(interval))
						{
							for (int j = interval.low; j <= interval.high; j++)
							{
								bitArray[j - item.low] = true;
							}
						}
					}
					cmp.EmitSet((char)item.low, bitArray, negate, ignore, reverse);
				}
				else if (item.IsSingleton)
				{
					cmp.EmitCharacter((char)item.low, negate, ignore, reverse);
				}
				else
				{
					cmp.EmitRange((char)item.low, (char)item.high, negate, ignore, reverse);
				}
			}
			for (int k = 0; k < pos_cats.Length; k++)
			{
				if (pos_cats[k])
				{
					if (neg_cats[k])
					{
						cmp.EmitCategory(System.Text.RegularExpressions.Category.AnySingleline, negate, reverse);
					}
					else
					{
						cmp.EmitCategory((System.Text.RegularExpressions.Category)k, negate, reverse);
					}
				}
				else if (neg_cats[k])
				{
					cmp.EmitNotCategory((System.Text.RegularExpressions.Category)k, negate, reverse);
				}
			}
			if (num > 1)
			{
				if (negate)
				{
					cmp.EmitTrue();
				}
				else
				{
					cmp.EmitFalse();
				}
				cmp.ResolveLink(linkRef);
			}
		}

		public override void GetWidth(out int min, out int max)
		{
			min = (max = 1);
		}

		public override bool IsComplex()
		{
			return false;
		}

		private static double GetIntervalCost(System.Text.RegularExpressions.Interval i)
		{
			if (i.IsDiscontiguous)
			{
				return 3 + (i.Size + 15 >> 4);
			}
			if (i.IsSingleton)
			{
				return 2.0;
			}
			return 3.0;
		}
	}
}
