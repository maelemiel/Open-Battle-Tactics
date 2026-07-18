using System.Collections.Generic;
using UnityEngine;

public class UIStarsBar : MonoBehaviour
{
	private const string _starAnim = "StarsBar_star";

	private const int _delta = 10;

	private const int _min = 20;

	[SerializeField]
	private int _score = 100;

	[SerializeField]
	private int _totalScore = 5000;

	[SerializeField]
	private List<int> _scoreThresholds;

	[SerializeField]
	private bool _manualInit;

	[SerializeField]
	private GameObject _star;

	[SerializeField]
	private GameObject _starSlot;

	[SerializeField]
	private tk2dSlicedSprite _bar;

	[SerializeField]
	private tk2dSlicedSprite _barContainer;

	private float _totalWidth;

	private List<bool> _starsEnabled;

	private List<GameObject> _stars;

	private List<GameObject> _starSlots;

	private bool _init;

	private int _lastScore = -1;

	private void Start()
	{
		if (!_manualInit)
		{
			Init();
		}
	}

	private void Init()
	{
		_bar.gameObject.SetActive(true);
		_bar.dimensions = new Vector2(_barContainer.dimensions.x - 10f, _bar.dimensions.y);
		_bar.transform.localPosition += new Vector3((0f - (_barContainer.dimensions.x - 10f)) / 2f, 0f, 0f);
		_totalWidth = _barContainer.dimensions.x - 10f - 20f;
		_starsEnabled = new List<bool>();
		_stars = new List<GameObject>();
		_starSlots = new List<GameObject>();
		for (int i = 0; i < _scoreThresholds.Count; i++)
		{
			Vector3 vector = new Vector3(10f + (float)_scoreThresholds[i] * (_barContainer.dimensions.x - 10f - 20f) / (float)_totalScore - (_barContainer.dimensions.x - 10f) / 2f, 0f, 0f);
			GameObject gameObject = (GameObject)Object.Instantiate(_starSlot);
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.localPosition = _starSlot.transform.localPosition + vector;
			gameObject.SetActive(true);
			_starSlots.Add(gameObject);
			gameObject = (GameObject)Object.Instantiate(_star);
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.transform.localPosition = _star.transform.localPosition + vector;
			gameObject.SetActive(false);
			_stars.Add(gameObject);
			_starsEnabled.Add(false);
		}
		_init = true;
	}

	public void Init(int initScore, int totalScore, List<int> scoreThresholds)
	{
		_score = initScore;
		_totalScore = totalScore;
		_scoreThresholds = scoreThresholds;
		Init();
	}

	public void SetScore(int score)
	{
		if (!_init || score == _lastScore)
		{
			return;
		}
		if (score >= 0 && score > _totalScore)
		{
			score = _totalScore;
		}
		_score = score;
		_lastScore = _score;
		float x = (float)score * _totalWidth / (float)_totalScore + 20f;
		_bar.dimensions = new Vector2(x, _bar.dimensions.y);
		for (int i = 0; i < _scoreThresholds.Count; i++)
		{
			if (_score >= _scoreThresholds[i] && !_starsEnabled[i])
			{
				_stars[i].SetActive(true);
				_starsEnabled[i] = true;
				_stars[i].animation["StarsBar_star"].time = 0f;
				_stars[i].animation["StarsBar_star"].speed = 1f;
				_stars[i].animation.Play("StarsBar_star");
			}
			else if (score < _scoreThresholds[i] && _starsEnabled[i])
			{
				_starsEnabled[i] = false;
				_stars[i].animation["StarsBar_star"].time = _stars[i].animation["StarsBar_star"].length;
				_stars[i].animation["StarsBar_star"].speed = -1f;
				_stars[i].animation.Play("StarsBar_star");
			}
		}
	}
}
