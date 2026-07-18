using System.Collections;
using UnityEngine;

public class GachaPlinkoBaseState : MonoBehaviour
{
	public ItemsMixerSceneController ItemsMixer { get; set; }

	public virtual IEnumerator StartStateSequence()
	{
		yield break;
	}
}
