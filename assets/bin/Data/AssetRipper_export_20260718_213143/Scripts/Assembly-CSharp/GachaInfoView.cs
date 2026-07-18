using UnityEngine;

public class GachaInfoView : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh title;

	[SerializeField]
	private tk2dTextMesh description;

	[SerializeField]
	private PrefabProxy gachaInfoImagePrefabProxy;

	public void ConfigureView(string title, string description, AssetLinkageDataModel assetLinkage)
	{
		if ((bool)this.title)
		{
			this.title.text = title;
		}
		if ((bool)this.description)
		{
			this.description.text = description;
		}
		if (gachaInfoImagePrefabProxy != null && assetLinkage != null)
		{
			StartCoroutine(gachaInfoImagePrefabProxy.ChangeAssetCoroutine(assetLinkage));
		}
	}
}
