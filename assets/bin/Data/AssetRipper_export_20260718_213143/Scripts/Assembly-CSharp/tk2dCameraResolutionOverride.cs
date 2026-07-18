using System;
using UnityEngine;

[Serializable]
public class tk2dCameraResolutionOverride
{
	public enum MatchByType
	{
		Resolution = 0,
		AspectRatio = 1,
		Wildcard = 2
	}

	public enum AutoScaleMode
	{
		None = 0,
		FitWidth = 1,
		FitHeight = 2,
		FitVisible = 3,
		StretchToFit = 4,
		ClosestMultipleOfTwo = 5,
		PixelPerfect = 6,
		Fill = 7
	}

	public enum FitMode
	{
		Constant = 0,
		Center = 1
	}

	public string name;

	public MatchByType matchBy;

	public int width;

	public int height;

	public float aspectRatioNumerator = 4f;

	public float aspectRatioDenominator = 3f;

	public float scale = 1f;

	public Vector2 offsetPixels = new Vector2(0f, 0f);

	public AutoScaleMode autoScaleMode;

	public FitMode fitMode;

	public static tk2dCameraResolutionOverride DefaultOverride
	{
		get
		{
			tk2dCameraResolutionOverride tk2dCameraResolutionOverride2 = new tk2dCameraResolutionOverride();
			tk2dCameraResolutionOverride2.name = "Override";
			tk2dCameraResolutionOverride2.matchBy = MatchByType.Wildcard;
			tk2dCameraResolutionOverride2.autoScaleMode = AutoScaleMode.FitVisible;
			tk2dCameraResolutionOverride2.fitMode = FitMode.Center;
			return tk2dCameraResolutionOverride2;
		}
	}

	public bool Match(int pixelWidth, int pixelHeight)
	{
		switch (matchBy)
		{
		case MatchByType.Wildcard:
			return true;
		case MatchByType.Resolution:
			return pixelWidth == width && pixelHeight == height;
		case MatchByType.AspectRatio:
		{
			float a = (float)pixelWidth * aspectRatioDenominator / aspectRatioNumerator;
			return Mathf.Approximately(a, pixelHeight);
		}
		default:
			return false;
		}
	}

	public void Upgrade(int version)
	{
		if (version == 0)
		{
			matchBy = (((width == -1 && height == -1) || (width == 0 && height == 0)) ? MatchByType.Wildcard : MatchByType.Resolution);
		}
	}
}
