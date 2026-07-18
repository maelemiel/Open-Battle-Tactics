using UnityEngine;

public class UnitItemController : MonoBehaviour
{
	public UserUnit unit;

	private UnitSelectHUDController unitHUDController;

	[SerializeField]
	private UnitItemView unitItemView;

	[SerializeField]
	private tk2dUIItem promoteButton;

	private bool state;

	public bool State
	{
		get
		{
			return state;
		}
	}

	public UnitItemView UnitItemView
	{
		get
		{
			return unitItemView;
		}
	}

	private void Awake()
	{
		unitItemView.SetState(false);
	}

	public void ToggleState()
	{
		state = !state;
		unitItemView.SetState(state);
	}

	public void Init(UnitSelectHUDController controller, UserUnit unit)
	{
		unitHUDController = controller;
		this.unit = unit;
		if (unit.IsMaxLevel)
		{
			SetPromoteButtonState(false);
		}
	}

	public void AddUnitButtonDown()
	{
		if (unitHUDController != null)
		{
			unitHUDController.UnitSelectedOnHUD(this);
		}
	}

	public void PromoteUnitButtonDown()
	{
		if (unitHUDController != null)
		{
			unitHUDController.UnitPromotedOnHUD(this);
		}
	}

	public void UpdateUnitView()
	{
		if ((bool)unitItemView)
		{
			unitItemView.ConfigureUnitView(unit);
			if (unit.IsMaxLevel)
			{
				SetPromoteButtonState(false);
			}
			else
			{
				SetPromoteButtonState(true);
			}
		}
	}

	private void SetPromoteButtonState(bool state)
	{
		promoteButton.enabled = state;
		unitItemView.SetPromoteButtonStateView(state);
	}
}
