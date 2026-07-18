using System;
using System.Collections.Generic;
using UnityEngine;

public class TicketBoostPopupController : PopupController
{
	protected enum ToggleStates
	{
		StandardBoost = 0,
		BiggerBoost = 1
	}

	private const string TOGGLE_SELECTED_KEY = "last_toggle_selected_in_event_boost_popUp";

	[SerializeField]
	protected tk2dUIToggleButtonGroup _boostToggleGroup;

	[SerializeField]
	protected GameObject _standartBoostsGO;

	[SerializeField]
	protected GameObject _biggerBoostsGO;

	[SerializeField]
	protected PointBoostController _standartPointBoost;

	[SerializeField]
	protected PointBoostController _twoTicketsPointBoost;

	[SerializeField]
	protected PointBoostController _treeTicketsPointBoost;

	[SerializeField]
	protected PointBoostController _biggerPointBoost1;

	[SerializeField]
	protected PointBoostController _biggerPointBoost2;

	[SerializeField]
	protected PointBoostController _biggerPointBoost3;

	[SerializeField]
	protected ThrobGameObjectController _throbBiggerBoostToggle;

	protected TicketBoostPopupModel popupModel;

	protected List<BoostDataModel> _boostList;

	protected ToggleStates _currentDBToggle;

	protected ToggleStates _currentSelectedToggle;

	protected override void Start()
	{
		base.Start();
		_boostList = BoostDataModel.GetAll();
		popupModel = (TicketBoostPopupModel)model.payload;
		if (UserProfileManager.Kvs.ContainsKey("last_toggle_selected_in_event_boost_popUp"))
		{
			_currentDBToggle = (ToggleStates)(int)Enum.Parse(typeof(ToggleStates), UserProfileManager.Kvs.GetValue<string>("last_toggle_selected_in_event_boost_popUp"));
			_currentSelectedToggle = _currentDBToggle;
		}
		_standartPointBoost.Init(null, OnBattleButton);
		ConfigureBiggerBoost();
	}

	public override void OnCloseButton()
	{
		if (model.closeButtonAction != null)
		{
			model.closeButtonAction();
		}
		Close();
	}

	protected void ConfigureBoostToggle()
	{
		_boostToggleGroup.SelectedIndex = (int)_currentSelectedToggle;
		_standartBoostsGO.SetActive(_currentSelectedToggle == ToggleStates.StandardBoost);
		_biggerBoostsGO.SetActive(_currentSelectedToggle != ToggleStates.StandardBoost);
	}

	private void ChangeSelectedToggleState(tk2dUIToggleButtonGroup toggleButton)
	{
		_currentSelectedToggle = (ToggleStates)_boostToggleGroup.SelectedIndex;
		ConfigureBoostToggle();
		if (_currentSelectedToggle == ToggleStates.StandardBoost)
		{
			StartCoroutine(_throbBiggerBoostToggle.InitThrob());
		}
	}

	protected void OnBattleButton(BoostType boostType, int energy_count)
	{
		if (UserProfile.player.energy >= energy_count)
		{
			GoToBattle(boostType);
			return;
		}
		if (energy_count == 1)
		{
			TopBarController.instance.BuyEnergyThenDoAction("ui_tickets_depleted_title", "ui_buy_tickets_message", delegate
			{
				GoToBattle(boostType);
			});
			return;
		}
		int energy = UserProfile.player.energy;
		TopBarController.instance.BuyBulkEnergyThenDoAction(energy_count - energy, energy_count, "ui_tickets_depleted_title", "ui_buy_bulk_tickets_message", delegate
		{
			GoToBattle(boostType);
		});
	}

	private void GoToBattle(BoostType boostType)
	{
		if (_currentSelectedToggle != _currentDBToggle)
		{
			UserProfileManager.Kvs.SetValue("last_toggle_selected_in_event_boost_popUp", _currentSelectedToggle.ToString());
		}
		Close();
		if (popupModel != null && popupModel.onBattleCallback != null)
		{
			popupModel.onBattleCallback(boostType);
		}
	}

	protected virtual void ConfigureBiggerBoost()
	{
	}
}
