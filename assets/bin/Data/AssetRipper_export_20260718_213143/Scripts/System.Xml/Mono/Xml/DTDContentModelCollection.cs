using System.Collections;

namespace Mono.Xml
{
	internal class DTDContentModelCollection
	{
		private ArrayList contentModel = new ArrayList();

		public IList Items
		{
			get
			{
				return contentModel;
			}
		}

		public DTDContentModel this[int i]
		{
			get
			{
				return contentModel[i] as DTDContentModel;
			}
		}

		public int Count
		{
			get
			{
				return contentModel.Count;
			}
		}

		public void Add(DTDContentModel model)
		{
			contentModel.Add(model);
		}
	}
}
