using System;

[Serializable]
public class tk2dSpriteCollectionPlatform
{
	public string name = string.Empty;

	public tk2dSpriteCollection spriteCollection;

	public bool Valid
	{
		get
		{
			return name.Length > 0 && spriteCollection != null;
		}
	}

	public void CopyFrom(tk2dSpriteCollectionPlatform source)
	{
		name = source.name;
		spriteCollection = source.spriteCollection;
	}
}
