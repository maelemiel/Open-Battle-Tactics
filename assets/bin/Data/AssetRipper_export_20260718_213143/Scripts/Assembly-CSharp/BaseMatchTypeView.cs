using UnityEngine;

public class BaseMatchTypeView : MonoBehaviour
{
	protected object dataObject;

	public virtual object DataObject
	{
		get
		{
			return dataObject;
		}
		set
		{
			dataObject = value;
			SetupMatchTypeView();
		}
	}

	public virtual void SetEnabled(bool state)
	{
		base.gameObject.SetActive(state);
	}

	protected virtual void SetupMatchTypeView()
	{
	}
}
