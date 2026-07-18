using System;
using System.Collections.Generic;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class HOTweenManager : ABSHOTweenEditorElement
{
	[Serializable]
	public class HOTweenData
	{
		public enum OnCompleteActionType
		{
			None = 0,
			PlayAll = 1,
			PlayTweensById = 2,
			SendMessage = 3
		}

		public enum ParameterType
		{
			None = 0,
			Color = 1,
			Number = 2,
			Object = 3,
			Quaternion = 4,
			Rect = 5,
			String = 6,
			Vector2 = 7,
			Vector3 = 8,
			Vector4 = 9
		}

		public string _targetType;

		public UnityEngine.Object target;

		public GameObject targetRoot;

		public string _targetPath;

		public string targetName;

		public bool foldout = true;

		public bool isActive = true;

		public int creationTime;

		public List<HOPropData> propDatas;

		public float duration = 1f;

		public bool tweenFrom;

		public bool paused;

		public bool autoKill = true;

		public UpdateType updateType = HOTween.defUpdateType;

		public float delay;

		public string id = string.Empty;

		public LoopType loopType = HOTween.defLoopType;

		public int loops = 1;

		public float timeScale = HOTween.defTimeScale;

		public EaseType easeType = HOTween.defEaseType;

		public OnCompleteActionType onCompleteActionType;

		public string onCompletePlayId = string.Empty;

		public GameObject onCompleteTarget;

		public string onCompleteMethodName = string.Empty;

		public ParameterType onCompleteParmType;

		public Color onCompleteParmColor = new Color(0f, 0f, 0f, 1f);

		public float onCompleteParmNumber;

		public UnityEngine.Object onCompleteParmObject;

		public Quaternion onCompleteParmQuaternion;

		public Rect onCompleteParmRect;

		public string onCompleteParmString;

		public Vector2 onCompleteParmVector2;

		public Vector3 onCompleteParmVector3;

		public Vector4 onCompleteParmVector4;

		public Type targetType
		{
			get
			{
				return Type.GetType(_targetType);
			}
		}

		public string targetPath
		{
			get
			{
				_targetPath = ((!(targetRoot == null)) ? (targetRoot.name + ((!(targetRoot == target)) ? ("." + _targetPath.Substring(_targetPath.IndexOf(".") + 1)) : string.Empty)) : _targetPath);
				return _targetPath;
			}
		}

		public string partialTargetPath
		{
			get
			{
				return _targetPath.Substring(_targetPath.IndexOf(".") + 1);
			}
		}

		public HOTweenData(int p_creationTime, GameObject p_targetRoot, UnityEngine.Object p_target, string p_targetPath)
		{
			targetRoot = p_targetRoot;
			target = p_target;
			_targetPath = p_targetPath;
			creationTime = p_creationTime;
			Type type = p_target.GetType();
			_targetType = type.FullName + ", " + type.Assembly.GetName().Name;
			targetName = targetPath.Substring(targetPath.LastIndexOf(".") + 1);
			propDatas = new List<HOPropData>();
		}

		public HOTweenData Clone(int p_creationTime)
		{
			HOTweenData hOTweenData = new HOTweenData(p_creationTime, targetRoot, target, _targetPath);
			hOTweenData.duration = duration;
			hOTweenData.tweenFrom = tweenFrom;
			hOTweenData.paused = paused;
			hOTweenData.updateType = updateType;
			hOTweenData.delay = delay;
			hOTweenData.id = id;
			hOTweenData.loopType = loopType;
			hOTweenData.loops = loops;
			hOTweenData.timeScale = timeScale;
			hOTweenData.easeType = easeType;
			hOTweenData.onCompleteActionType = onCompleteActionType;
			hOTweenData.onCompletePlayId = onCompletePlayId;
			hOTweenData.onCompleteTarget = onCompleteTarget;
			hOTweenData.onCompleteMethodName = onCompleteMethodName;
			hOTweenData.onCompleteParmType = onCompleteParmType;
			hOTweenData.onCompleteParmColor = new Color(onCompleteParmColor.r, onCompleteParmColor.g, onCompleteParmColor.b, onCompleteParmColor.a);
			hOTweenData.onCompleteParmNumber = onCompleteParmNumber;
			hOTweenData.onCompleteParmObject = onCompleteParmObject;
			hOTweenData.onCompleteParmQuaternion = new Quaternion(onCompleteParmQuaternion.x, onCompleteParmQuaternion.y, onCompleteParmQuaternion.z, onCompleteParmQuaternion.w);
			hOTweenData.onCompleteParmRect = new Rect(onCompleteParmRect);
			hOTweenData.onCompleteParmString = onCompleteParmString;
			hOTweenData.onCompleteParmVector2 = new Vector2(onCompleteParmVector2.x, onCompleteParmVector2.y);
			hOTweenData.onCompleteParmVector3 = new Vector3(onCompleteParmVector3.x, onCompleteParmVector3.y, onCompleteParmVector3.z);
			hOTweenData.onCompleteParmVector4 = new Vector4(onCompleteParmVector4.x, onCompleteParmVector4.y, onCompleteParmVector4.z, onCompleteParmVector4.w);
			foreach (HOPropData propData in propDatas)
			{
				hOTweenData.propDatas.Add(propData.Clone());
			}
			return hOTweenData;
		}
	}

	[Serializable]
	public class HOPropData
	{
		[SerializeField]
		private string _propType;

		[SerializeField]
		private string _pluginType = string.Empty;

		[SerializeField]
		private string _valueType = string.Empty;

		public string propName;

		public string shortPropType;

		public bool isRelative;

		public bool isActive = true;

		public Vector2 endValVector2 = Vector2.zero;

		public Vector3 endValVector3 = Vector3.zero;

		public Vector4 endValVector4 = Vector4.zero;

		public float endValFloat;

		public int endValInt;

		public string endValString = string.Empty;

		public Color endValColor = Color.white;

		public Type pluginType
		{
			get
			{
				return (!(_pluginType == string.Empty)) ? Type.GetType(_pluginType) : null;
			}
			set
			{
				_pluginType = ((value != null) ? (value.FullName + ", " + value.Assembly.GetName().Name) : string.Empty);
			}
		}

		public Type valueType
		{
			get
			{
				return (!(_valueType == string.Empty)) ? Type.GetType(_valueType) : null;
			}
			set
			{
				_valueType = ((value != null) ? (value.FullName + ", " + value.Assembly.GetName().Name) : string.Empty);
			}
		}

		public Type propType
		{
			get
			{
				return Type.GetType(_propType);
			}
		}

		public object endVal
		{
			get
			{
				Type type = valueType;
				if (type == typeof(Vector2))
				{
					return endValVector2;
				}
				if (type == typeof(Vector3))
				{
					return endValVector3;
				}
				if (type == typeof(Vector4))
				{
					return endValVector4;
				}
				if (type == typeof(string))
				{
					return endValString;
				}
				if (type == typeof(Color))
				{
					return endValColor;
				}
				if (type == typeof(int))
				{
					return endValInt;
				}
				return endValFloat;
			}
			set
			{
				Type type = valueType;
				if (type == typeof(Vector2))
				{
					endValVector2 = (Vector2)value;
				}
				else if (type == typeof(Vector3))
				{
					endValVector3 = (Vector3)value;
				}
				else if (type == typeof(Vector4))
				{
					endValVector4 = (Vector4)value;
				}
				else if (type == typeof(string))
				{
					endValString = (string)value;
				}
				else if (type == typeof(Color))
				{
					endValColor = (Color)value;
				}
				else if (type == typeof(int))
				{
					endValInt = Convert.ToInt32(value);
				}
				else
				{
					endValFloat = Convert.ToSingle(value);
				}
			}
		}

		public HOPropData(string p_propName, Type p_propType)
		{
			propName = p_propName;
			_propType = p_propType.FullName + ", " + p_propType.Assembly.GetName().Name;
			shortPropType = p_propType.Name;
		}

		public HOPropData Clone()
		{
			HOPropData hOPropData = new HOPropData(propName, propType);
			hOPropData._pluginType = _pluginType;
			hOPropData._valueType = _valueType;
			hOPropData.isRelative = isRelative;
			hOPropData.isActive = isActive;
			hOPropData.endValVector2 = new Vector2(endValVector2.x, endValVector2.y);
			hOPropData.endValVector3 = new Vector3(endValVector3.x, endValVector3.y, endValVector3.z);
			hOPropData.endValVector4 = new Vector4(endValVector4.x, endValVector4.y, endValVector4.z, endValVector4.w);
			hOPropData.endValFloat = endValFloat;
			hOPropData.endValInt = endValInt;
			hOPropData.endValString = endValString;
			hOPropData.endValColor = new Color(endValColor.r, endValColor.g, endValColor.b, endValColor.a);
			return hOPropData;
		}
	}

	private void Awake()
	{
		HOTween.Init(true, true, true);
		foreach (HOTweenData tweenData in tweenDatas)
		{
			CreateTween(tweenData, globalDelay, globalTimeScale);
		}
		DoDestroy();
	}

	protected override void DoDestroy()
	{
		if (!destroyed)
		{
			base.DoDestroy();
			if (base.gameObject != null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	public static Tweener CreateTween(HOTweenData p_twData, float p_globalDelay, float p_globalTimeScale)
	{
		if (p_twData.propDatas.Count == 0 || !p_twData.isActive)
		{
			return null;
		}
		TweenParms tweenParms = new TweenParms().Delay(p_twData.delay + p_globalDelay).Id(p_twData.id).Loops(p_twData.loops, p_twData.loopType)
			.UpdateType(p_twData.updateType)
			.Ease(p_twData.easeType)
			.TimeScale(p_twData.timeScale * p_globalTimeScale)
			.AutoKill(p_twData.autoKill)
			.Pause(p_twData.paused);
		if (p_twData.onCompleteActionType != HOTweenData.OnCompleteActionType.None)
		{
			switch (p_twData.onCompleteActionType)
			{
			case HOTweenData.OnCompleteActionType.PlayAll:
				tweenParms.OnComplete((TweenDelegate.TweenCallback)delegate
				{
					HOTween.Play();
				});
				break;
			case HOTweenData.OnCompleteActionType.PlayTweensById:
				tweenParms.OnComplete((TweenDelegate.TweenCallback)delegate
				{
					HOTween.Play(p_twData.onCompletePlayId);
				});
				break;
			case HOTweenData.OnCompleteActionType.SendMessage:
				if (!(p_twData.onCompleteTarget == null) && !(p_twData.onCompleteMethodName == string.Empty))
				{
					object p_value = null;
					switch (p_twData.onCompleteParmType)
					{
					case HOTweenData.ParameterType.Color:
						p_value = p_twData.onCompleteParmColor;
						break;
					case HOTweenData.ParameterType.Number:
						p_value = p_twData.onCompleteParmNumber;
						break;
					case HOTweenData.ParameterType.Object:
						p_value = p_twData.onCompleteParmObject;
						break;
					case HOTweenData.ParameterType.Quaternion:
						p_value = p_twData.onCompleteParmQuaternion;
						break;
					case HOTweenData.ParameterType.Rect:
						p_value = p_twData.onCompleteParmRect;
						break;
					case HOTweenData.ParameterType.String:
						p_value = p_twData.onCompleteParmString;
						break;
					case HOTweenData.ParameterType.Vector2:
						p_value = p_twData.onCompleteParmVector2;
						break;
					case HOTweenData.ParameterType.Vector3:
						p_value = p_twData.onCompleteParmVector3;
						break;
					case HOTweenData.ParameterType.Vector4:
						p_value = p_twData.onCompleteParmVector4;
						break;
					}
					tweenParms.OnComplete(p_twData.onCompleteTarget, p_twData.onCompleteMethodName, p_value);
				}
				break;
			}
		}
		foreach (HOPropData propData in p_twData.propDatas)
		{
			if (propData.isActive)
			{
				tweenParms.Prop(propData.propName, Activator.CreateInstance(propData.pluginType, propData.endVal, propData.isRelative));
			}
		}
		if (!tweenParms.hasProps)
		{
			return null;
		}
		if (p_twData.tweenFrom)
		{
			return HOTween.From(p_twData.target, p_twData.duration, tweenParms);
		}
		return HOTween.To(p_twData.target, p_twData.duration, tweenParms);
	}
}
