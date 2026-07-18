using UnityEngine;

[RequireComponent(typeof(tk2dBaseSprite))]
public class EventCoinSpriteController : MonoBehaviour
{
	[SerializeField]
	private bool autoInit = true;

	[SerializeField]
	private tk2dSprite iconSprite;

	[SerializeField]
	private string eventPointSpriteName;

	[SerializeField]
	private float eventPointEventIconScale = 1f;

	[SerializeField]
	private string raidBossEventPointSpriteName;

	[SerializeField]
	private float raidBossEventIconScale = 1f;

	[SerializeField]
	private string pvpEventPointSpriteName;

	[SerializeField]
	private float pvpEventIconScale = 1f;

	private tk2dBaseSprite eventPointCoin;

	private void Start()
	{
		if (!autoInit)
		{
			return;
		}
		UserProfile player = UserProfile.player;
		if (player == null)
		{
			Log.Error("UserProfile not found", base.gameObject);
			return;
		}
		EventDataModel activeOnCooldownEvent = player.GetActiveOnCooldownEvent();
		if (activeOnCooldownEvent != null)
		{
			Init(activeOnCooldownEvent);
		}
	}

	public void Init(EventDataModel activeEvent)
	{
		eventPointCoin = GetComponent<tk2dBaseSprite>();
		string empty = string.Empty;
		Vector3 vector = new Vector3(1f, 1f, 1f);
		switch (activeEvent.EventType)
		{
		case EventDataModel.EventTypes.RAIDBOSS_EVENT:
			empty = raidBossEventPointSpriteName;
			vector = new Vector3(raidBossEventIconScale, raidBossEventIconScale, raidBossEventIconScale);
			break;
		case EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT:
			empty = pvpEventPointSpriteName;
			vector = new Vector3(pvpEventIconScale, pvpEventIconScale, pvpEventIconScale);
			break;
		default:
			empty = eventPointSpriteName;
			vector = new Vector3(eventPointEventIconScale, eventPointEventIconScale, eventPointEventIconScale);
			break;
		}
		if (!string.IsNullOrEmpty(empty))
		{
			eventPointCoin.SetSprite(empty);
		}
		if (iconSprite != null)
		{
			iconSprite.scale = vector;
		}
	}
}
