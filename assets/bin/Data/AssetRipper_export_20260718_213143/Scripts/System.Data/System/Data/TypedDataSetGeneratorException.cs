using System.Collections;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class TypedDataSetGeneratorException : DataException
	{
		private readonly ArrayList errorList;

		public ArrayList ErrorList
		{
			get
			{
				return errorList;
			}
		}

		public TypedDataSetGeneratorException()
			: base(global::Locale.GetText("System error."))
		{
		}

		public TypedDataSetGeneratorException(ArrayList list)
			: base(global::Locale.GetText("System error."))
		{
			errorList = list;
		}

		protected TypedDataSetGeneratorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			int @int = info.GetInt32("KEY_ARRAYCOUNT");
			errorList = new ArrayList(@int);
			for (int i = 0; i < @int; i++)
			{
				errorList.Add(info.GetString("KEY_ARRAYVALUES" + i));
			}
		}

		public TypedDataSetGeneratorException(string error)
			: base(error)
		{
		}

		public TypedDataSetGeneratorException(string error, Exception inner)
			: base(error, inner)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			int num = ((errorList != null) ? ErrorList.Count : 0);
			info.AddValue("KEY_ARRAYCOUNT", num);
			for (int i = 0; i < num; i++)
			{
				info.AddValue("KEY_ARRAYVALUES" + i, ErrorList[i]);
			}
		}
	}
}
