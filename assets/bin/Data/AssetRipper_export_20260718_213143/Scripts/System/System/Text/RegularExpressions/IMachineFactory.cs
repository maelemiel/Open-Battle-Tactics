using System.Collections;

namespace System.Text.RegularExpressions
{
	internal interface IMachineFactory
	{
		IDictionary Mapping { get; set; }

		int GroupCount { get; }

		int Gap { get; set; }

		string[] NamesMapping { get; set; }

		System.Text.RegularExpressions.IMachine NewInstance();
	}
}
