using System;
using System.Collections.Generic;

public class DataModelQueue : AsyncQueue<DataModelQueue, DataModelQueue.Request, DataModelQueue.Response>
{
	public class Request : BaseRequest
	{
		public enum Type
		{
			Connect = 0,
			Multi = 1,
			SingleAssetBundle = 2,
			AllAssetBundles = 3
		}

		public delegate AccessBase.Error MultiDelegate(DMAccessManager db, object inputs, out object outputs);

		public Type type;

		public string connectPath;

		public object multiInputs;

		public MultiDelegate multiDelegate;

		public object id;

		public static Request Connect(string path, Callback callback)
		{
			Request request = new Request();
			request.callback = callback;
			request.type = Type.Connect;
			request.connectPath = path;
			return request;
		}

		public static Request Multi(object multiInputs, MultiDelegate multiDelegate, Callback callback)
		{
			Request request = new Request();
			request.callback = callback;
			request.type = Type.Multi;
			request.multiInputs = multiInputs;
			request.multiDelegate = multiDelegate;
			return request;
		}

		public static Request SingleAssetBundle(int id, Callback callback)
		{
			Request request = new Request();
			request.callback = callback;
			request.type = Type.SingleAssetBundle;
			request.id = id;
			return request;
		}

		public static Request AllAssetBundles(Callback callback)
		{
			Request request = new Request();
			request.callback = callback;
			request.type = Type.AllAssetBundles;
			return request;
		}
	}

	public class Response : BaseResponse
	{
		public object dataModel;

		public AccessBase.Error error;
	}

	private DMAccessManager dataModelAccess;

	public override void Start()
	{
		NonUnitySingleton<DMAccessManager>.Instantiate();
		dataModelAccess = NonUnitySingleton<DMAccessManager>.instance;
		base.Start();
	}

	protected override void ProcessRequest(Request request, Response response)
	{
		switch (request.type)
		{
		case Request.Type.Connect:
			ProcessConnectRequest(request, response);
			break;
		case Request.Type.Multi:
			ProcessMultiRequest(request, response);
			break;
		case Request.Type.SingleAssetBundle:
			ProcessAssetBundleRequest(request, response);
			break;
		case Request.Type.AllAssetBundles:
			ProcessAssetBundleRequest(request, response);
			break;
		}
	}

	private void ProcessConnectRequest(Request request, Response response)
	{
		ConnectDataModel m;
		response.error = dataModelAccess.Connect(request.connectPath, out m);
		if (response.error == null)
		{
			response.dataModel = m;
		}
	}

	private void ProcessMultiRequest(Request request, Response response)
	{
		try
		{
			response.error = request.multiDelegate(dataModelAccess, request.multiInputs, out response.dataModel);
		}
		catch (Exception e)
		{
			response.error = AccessBase.Error.Exception(e);
		}
	}

	private void ProcessAssetBundleRequest(Request request, Response response)
	{
		if (request.type == Request.Type.AllAssetBundles)
		{
			List<AssetBundleDataModel> assetBundles;
			response.error = dataModelAccess.DataModelAccess.GetAllAssetBundles(out assetBundles);
			if (response.error == null)
			{
				response.dataModel = assetBundles;
			}
		}
		else
		{
			AssetBundleDataModel m;
			response.error = dataModelAccess.DataModelAccess._GetSingleAssetBundle((int)request.id, out m);
			if (response.error == null)
			{
				response.dataModel = m;
			}
		}
	}

	public static void GetSingleOld<T>(string id, Action<T> cb) where T : BaseDataModel, new()
	{
		Request request = Request.Multi(null, delegate(DMAccessManager db, object inputs, out object outputs)
		{
			outputs = null;
			T q;
			AccessBase.Error singleOld = db.GetSingleOld<T>(id, out q);
			if (singleOld != null)
			{
				return singleOld;
			}
			outputs = q;
			return (AccessBase.Error)null;
		}, delegate(Response response)
		{
			if (response.error != null)
			{
				Log.Error("Error while setting {0}. Error: " + response.error.description, typeof(T).ToString());
			}
			else
			{
				cb(response.dataModel as T);
			}
		});
		Singleton<DataModelQueue>.instance.Enqueue(request);
	}
}
