using UnityEngine;

[RequireComponent(typeof(PrefabProxy))]
public class AssetLinkageLoader : MonoBehaviour
{
	public int assetLinkageID;

	private PrefabProxy prefabProxy;

	private void Start()
	{
		prefabProxy = GetComponent<PrefabProxy>();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			LoadAssetLinkage();
		});
	}

	private void LoadAssetLinkage()
	{
		AssetLinkageDataModel single = AssetLinkageDataModel.GetSingle(assetLinkageID);
		if (single != null)
		{
			prefabProxy.ChangeAsset(single);
		}
	}
}
