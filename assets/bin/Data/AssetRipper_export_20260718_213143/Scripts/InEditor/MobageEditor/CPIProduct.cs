using System;
using System.Collections.Generic;
using System.Net;

namespace MobageEditor
{
	public class CPIProduct : ISerializableItem
	{
		public static void GetCPIProducts(Action<SimpleAPIStatus, Error, List<CPIProduct>> completeCb)
		{
			Action<Error, List<CPIProduct>> finalCallback = delegate(Error error, List<CPIProduct> products)
			{
				completeCb((error != null) ? SimpleAPIStatus.Error : SimpleAPIStatus.Success, error, products);
			};
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = "cpi/products";
			request.HTTPMethod = "GET";
			request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (error != null)
				{
					finalCallback(error, null);
				}
				else
				{
					List<CPIProduct> list = new List<CPIProduct>();
					if (data != null)
					{
						for (int i = 0; i < data.Count; i++)
						{
							list.Add(JsonMapper.ToObject<CPIProduct>(data[i].ToJson()));
						}
					}
					finalCallback(null, list);
				}
			});
		}

		public Dictionary<string, object> PackForEnvironment(ModelSerializationEnvironment env)
		{
			return new Dictionary<string, object>();
		}
	}
}
