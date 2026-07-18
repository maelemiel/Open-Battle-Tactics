using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class CubeRotator : MonoBehaviour
{
	private class CubeFaceData
	{
		public float rotation;

		public Transform transform;

		public CubeFaceData(float rotation, Transform transform)
		{
			this.rotation = rotation;
			this.transform = transform;
		}
	}

	[SerializeField]
	private Transform frontFace;

	[SerializeField]
	private Transform backFace;

	[SerializeField]
	private Transform topFace;

	[SerializeField]
	private Transform bottomFace;

	[SerializeField]
	private CubeBarController cubeBar;

	private string targetSprite;

	public float currentXRotation;

	private Dictionary<CubeFace, CubeFaceData> FramePosDictionary = new Dictionary<CubeFace, CubeFaceData>();

	private Vector3 _currentFrame;

	private float _rotationTime;

	public float RotationTime
	{
		set
		{
			_rotationTime = value;
		}
	}

	private void Awake()
	{
		FramePosDictionary.Add(CubeFace.FRONT, new CubeFaceData(-360f, frontFace));
		FramePosDictionary.Add(CubeFace.BACK, new CubeFaceData(-180f, backFace));
		FramePosDictionary.Add(CubeFace.TOP, new CubeFaceData(-90f, topFace));
		FramePosDictionary.Add(CubeFace.BOTTOM, new CubeFaceData(-270f, bottomFace));
	}

	private void Start()
	{
		_currentFrame = base.transform.rotation.eulerAngles;
		currentXRotation = _currentFrame.x;
	}

	private void Update()
	{
		base.transform.rotation = Quaternion.Euler(currentXRotation, 0f, 0f);
	}

	public void DoAllRotations(CubeFace targetFace)
	{
		float num = 0f;
		CubeFaceData cubeFaceData = FramePosDictionary[targetFace];
		num = cubeFaceData.rotation;
		HOTween.To(this, _rotationTime, new TweenParms().NewProp("currentXRotation", num).Ease(EaseType.Linear));
	}
}
