using System.Collections;
using UnityEngine;

[RequireComponent(typeof(tk2dSpriteFromTexture))]
public class StreamingThumbnail : MonoBehaviour
{
	private const string DEFAULT_THUMBNAIL_NAME = "mobage";

	private tk2dSpriteFromTexture _image;

	private tk2dSprite _sprite;

	public string url = string.Empty;

	public int sortingOrder;

	public Vector2 size = new Vector2(128f, 128f);

	[SerializeField]
	private GameObject loadingIcon;

	[SerializeField]
	private Vector3 loadingIconScale = Vector3.one;

	private GameObject loadingIndicatorInstance;

	private void Awake()
	{
		_image = GetComponent<tk2dSpriteFromTexture>();
		_image.spriteCollectionSize.pixelsPerMeter = 1f;
		_image.texture = Resources.Load("mobage") as Texture;
		_image.ForceBuild();
		_sprite = _image.GetComponent<tk2dSprite>();
		_sprite.SortingOrder = sortingOrder;
		AdjustScale();
	}

	public void ChangeThumbnail(string url)
	{
		if (!string.IsNullOrEmpty(url))
		{
			StopAllCoroutines();
			StartCoroutine(ChangeThumbnailCoroutine(url));
		}
	}

	public IEnumerator ChangeThumbnailCoroutine(string url)
	{
		this.url = url;
		_sprite.renderer.enabled = false;
		_image.texture = null;
		_image.ForceBuild();
		if (!loadingIndicatorInstance && (bool)loadingIcon)
		{
			loadingIndicatorInstance = Object.Instantiate(loadingIcon) as GameObject;
			loadingIndicatorInstance.transform.parent = base.transform;
			loadingIndicatorInstance.SetSortingOrder(sortingOrder);
			loadingIndicatorInstance.transform.localPosition = Vector3.zero;
			loadingIndicatorInstance.transform.localScale = loadingIconScale;
		}
		WWW www = new WWW(url);
		yield return www;
		_image.texture = new Texture2D(4, 4, TextureFormat.DXT1, false);
		www.LoadImageIntoTexture(_image.texture as Texture2D);
		_image.ForceBuild();
		_sprite.renderer.enabled = true;
		AdjustScale();
		if ((bool)loadingIndicatorInstance)
		{
			Object.Destroy(loadingIndicatorInstance);
		}
	}

	private void AdjustScale()
	{
		float x = size.x / (float)_image.texture.width;
		float y = size.y / (float)_image.texture.height;
		_sprite.scale = new Vector3(x, y, 1f);
	}

	private void OnDisable()
	{
		if ((bool)loadingIndicatorInstance)
		{
			Object.Destroy(loadingIndicatorInstance);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(base.transform.position, new Vector3(size.x, size.y, 1f));
	}
}
