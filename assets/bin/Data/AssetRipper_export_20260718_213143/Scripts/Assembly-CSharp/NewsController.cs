using System;
using System.Collections;
using UnityEngine;

public class NewsController : MonoBehaviour
{
	public const string NEWS_KEYVALUE_SUFIX = "new_count_";

	[SerializeField]
	protected NewsBackgroundController _backgroundController;

	public Action TvShaker;

	protected NewsDataModel _dataModel;

	private float backgroundSpeed;

	public bool swaping;

	public virtual AnnouncerType AnnouncerType
	{
		get
		{
			if (_dataModel == null)
			{
				return AnnouncerType.NONE;
			}
			return AnnouncerTypeExtensions.GetAnnouncerType(_dataModel.announcerType);
		}
	}

	public int ShowTimes
	{
		get
		{
			if (_dataModel == null)
			{
				return 0;
			}
			if (_dataModel.showNumber == 0)
			{
				return 0;
			}
			if (!UserProfileManager.Kvs.ContainsKey("new_count_" + _dataModel.id))
			{
				return 0;
			}
			return UserProfileManager.Kvs.GetValue<int>("new_count_" + _dataModel.id);
		}
		set
		{
			if (_dataModel != null && _dataModel.showNumber != 0)
			{
				UserProfileManager.Kvs.SetValue("new_count_" + _dataModel.id, value);
			}
		}
	}

	protected virtual void Awake()
	{
		if ((bool)_backgroundController)
		{
			_backgroundController.SetBackgroundSpeed(0);
		}
	}

	public virtual IEnumerator Init(NewsDataModel newsDM = null)
	{
		_dataModel = newsDM;
		swaping = false;
		if ((bool)_backgroundController && _dataModel != null)
		{
			backgroundSpeed = (float)newsDM.backgroundSpeed / 100f;
			_backgroundController.SetBackground(newsDM.background, backgroundSpeed);
		}
		yield return 0;
	}

	public virtual void TvButtonPress()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaScene);
	}

	public virtual void BeforeMovingInAction()
	{
		if ((bool)_backgroundController)
		{
			_backgroundController.SetBackgroundSpeed(0);
		}
	}

	public virtual void BeforeMovingOutAction()
	{
		if ((bool)_backgroundController)
		{
			_backgroundController.SetBackgroundSpeed(0);
		}
	}

	public virtual IEnumerator AfterMovingInAction()
	{
		if ((bool)_backgroundController)
		{
			_backgroundController.SetBackgroundSpeed(backgroundSpeed);
		}
		yield return 1;
	}

	public virtual IEnumerator AfterMovingOutAction()
	{
		if ((bool)_backgroundController)
		{
			_backgroundController.SetBackgroundSpeed(backgroundSpeed);
		}
		yield return 1;
	}
}
