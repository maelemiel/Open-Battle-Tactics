using System.Collections.Generic;

namespace Mono.Xml
{
	internal class DictionaryBase : List<KeyValuePair<string, DTDNode>>
	{
		public IEnumerable<DTDNode> Values
		{
			get
			{
				using (Enumerator enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						yield return enumerator.Current.Value;
					}
				}
			}
		}
	}
}
