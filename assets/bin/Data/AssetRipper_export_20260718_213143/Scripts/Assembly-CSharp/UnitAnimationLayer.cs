using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationLayer : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite _sprite;

	[SerializeField]
	private string layerPrefix = "a";

	[SerializeField]
	private float updateFrequency = 0.09f;

	private Transform _selfTransform;

	private List<int> layerSpriteIDs = new List<int>();

	public bool shouldBounce;

	public Transform bounceTarget;

	public bool hideInsideBattle;

	public bool hideOutsideBattle;

	public Color color
	{
		get
		{
			return _sprite.color;
		}
		set
		{
			_sprite.color = value;
		}
	}

	public tk2dSprite Sprite
	{
		get
		{
			return _sprite;
		}
	}

	private void Awake()
	{
		_selfTransform = base.transform;
	}

	private void Update()
	{
		int index = Mathf.FloorToInt(Time.time / updateFrequency) % layerSpriteIDs.Count;
		_sprite.spriteId = layerSpriteIDs[index];
	}

	private void LateUpdate()
	{
		if (shouldBounce && (bool)bounceTarget)
		{
			Vector3 localPosition = bounceTarget.localPosition;
			localPosition.z = _selfTransform.localPosition.z;
			_selfTransform.localPosition = localPosition;
		}
	}

	public void Activate(tk2dSpriteCollectionData spriteCollection, string baseSpriteName)
	{
		layerSpriteIDs = new List<int>();
		int num = 0;
		while (true)
		{
			string text = baseSpriteName + "_" + layerPrefix + num;
			tk2dSpriteDefinition spriteDefinition = spriteCollection.GetSpriteDefinition(text);
			if (spriteDefinition != null)
			{
				int spriteIdByName = spriteCollection.GetSpriteIdByName(text);
				AddFrameLayer(spriteIdByName, spriteDefinition);
			}
			else if (num >= 2)
			{
				break;
			}
			num++;
		}
		_sprite.Collection = spriteCollection;
		if (layerSpriteIDs.Count > 0)
		{
			base.gameObject.SetActive(true);
		}
	}

	private void AddFrameLayer(int spriteId, tk2dSpriteDefinition spriteDef)
	{
		int result = 1;
		tk2dSpriteDefinition.AttachPoint[] attachPoints = spriteDef.attachPoints;
		foreach (tk2dSpriteDefinition.AttachPoint attachPoint in attachPoints)
		{
			if (attachPoint.name == "bounce")
			{
				shouldBounce = true;
			}
			else if (attachPoint.name.StartsWith("fps_"))
			{
				int result2 = 0;
				if (int.TryParse(attachPoint.name.Split('_')[1], out result2))
				{
					updateFrequency = 1f / (float)result2;
				}
			}
			else if (attachPoint.name.StartsWith("repeat_"))
			{
				int.TryParse(attachPoint.name.Split('_')[1], out result);
			}
			else if (attachPoint.name.StartsWith("hide_inside_battle"))
			{
				hideInsideBattle = true;
			}
			else if (attachPoint.name.StartsWith("hide_outside_battle"))
			{
				hideOutsideBattle = true;
			}
		}
		for (int j = 0; j < result; j++)
		{
			layerSpriteIDs.Add(spriteId);
		}
	}

	public void SetSortingOrder(int sortingOrder)
	{
		if ((bool)_sprite)
		{
			_sprite.SortingOrder = sortingOrder;
		}
	}
}
