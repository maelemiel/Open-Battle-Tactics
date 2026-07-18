using System.Collections;

namespace System.Text.RegularExpressions
{
	internal interface ICompiler
	{
		void Reset();

		System.Text.RegularExpressions.IMachineFactory GetMachineFactory();

		void EmitFalse();

		void EmitTrue();

		void EmitCharacter(char c, bool negate, bool ignore, bool reverse);

		void EmitCategory(System.Text.RegularExpressions.Category cat, bool negate, bool reverse);

		void EmitNotCategory(System.Text.RegularExpressions.Category cat, bool negate, bool reverse);

		void EmitRange(char lo, char hi, bool negate, bool ignore, bool reverse);

		void EmitSet(char lo, BitArray set, bool negate, bool ignore, bool reverse);

		void EmitString(string str, bool ignore, bool reverse);

		void EmitPosition(System.Text.RegularExpressions.Position pos);

		void EmitOpen(int gid);

		void EmitClose(int gid);

		void EmitBalanceStart(int gid, int balance, bool capture, System.Text.RegularExpressions.LinkRef tail);

		void EmitBalance();

		void EmitReference(int gid, bool ignore, bool reverse);

		void EmitIfDefined(int gid, System.Text.RegularExpressions.LinkRef tail);

		void EmitSub(System.Text.RegularExpressions.LinkRef tail);

		void EmitTest(System.Text.RegularExpressions.LinkRef yes, System.Text.RegularExpressions.LinkRef tail);

		void EmitBranch(System.Text.RegularExpressions.LinkRef next);

		void EmitJump(System.Text.RegularExpressions.LinkRef target);

		void EmitRepeat(int min, int max, bool lazy, System.Text.RegularExpressions.LinkRef until);

		void EmitUntil(System.Text.RegularExpressions.LinkRef repeat);

		void EmitIn(System.Text.RegularExpressions.LinkRef tail);

		void EmitInfo(int count, int min, int max);

		void EmitFastRepeat(int min, int max, bool lazy, System.Text.RegularExpressions.LinkRef tail);

		void EmitAnchor(bool reverse, int offset, System.Text.RegularExpressions.LinkRef tail);

		void EmitBranchEnd();

		void EmitAlternationEnd();

		System.Text.RegularExpressions.LinkRef NewLink();

		void ResolveLink(System.Text.RegularExpressions.LinkRef link);
	}
}
