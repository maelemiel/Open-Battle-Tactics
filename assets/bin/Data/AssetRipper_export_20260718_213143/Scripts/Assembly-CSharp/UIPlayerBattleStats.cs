using UnityEngine;

public class UIPlayerBattleStats : MonoBehaviour
{
	[SerializeField]
	private GameObject _winCounter01;

	[SerializeField]
	private GameObject _winCounter02;

	[SerializeField]
	private StreamingThumbnail _playerPicture;

	[SerializeField]
	private tk2dTextMesh _playerName;

	private void Awake()
	{
		SetWins(0);
	}

	public void SetWins(int ammount)
	{
		switch (ammount)
		{
		case 0:
			_winCounter01.SetActive(false);
			_winCounter02.SetActive(false);
			break;
		case 1:
			_winCounter01.SetActive(true);
			_winCounter02.SetActive(false);
			break;
		default:
			_winCounter01.SetActive(true);
			_winCounter02.SetActive(true);
			break;
		}
	}

	public void SetPlayerName(string name)
	{
		_playerName.text = name;
		_playerName.Commit();
	}

	public void SetPlayerImage(string thumbnailUrl)
	{
		StartCoroutine(_playerPicture.ChangeThumbnailCoroutine(thumbnailUrl));
	}
}
