using System;
using UnityEngine;

public class PasswordPopUpController : PopupControllerCustomLabels
{
	[SerializeField]
	private tk2dUITextInput passwordInput;

	private Action<string> passwordCallback;

	protected override void Start()
	{
		base.Start();
		passwordCallback = (Action<string>)model.payload;
	}

	public override void OnRightButton()
	{
		base.OnRightButton();
		if (passwordCallback != null)
		{
			passwordCallback(passwordInput.Text);
		}
	}
}
