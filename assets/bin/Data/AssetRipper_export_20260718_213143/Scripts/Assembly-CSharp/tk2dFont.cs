using UnityEngine;

[AddComponentMenu("2D Toolkit/Backend/tk2dFont")]
public class tk2dFont : MonoBehaviour
{
	public TextAsset bmFont;

	public Material material;

	public Texture texture;

	public Texture2D gradientTexture;

	public bool dupeCaps;

	public bool flipTextureY;

	[HideInInspector]
	public bool proxyFont;

	[SerializeField]
	[HideInInspector]
	private bool useTk2dCamera;

	[SerializeField]
	[HideInInspector]
	private int targetHeight = 640;

	[SerializeField]
	[HideInInspector]
	private float targetOrthoSize = 1f;

	public tk2dSpriteCollectionSize sizeDef = tk2dSpriteCollectionSize.Default();

	public int gradientCount = 1;

	public bool manageMaterial;

	[HideInInspector]
	public bool loadable;

	public int charPadX;

	public tk2dFontData data;

	public static int CURRENT_VERSION = 1;

	public int version;

	public void Upgrade()
	{
		if (version < CURRENT_VERSION)
		{
			Debug.Log("Font '" + base.name + "' - Upgraded from version " + version);
			if (version == 0)
			{
				sizeDef.CopyFromLegacy(useTk2dCamera, targetOrthoSize, targetHeight);
			}
			version = CURRENT_VERSION;
		}
	}
}
