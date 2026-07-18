using System;
using UnityEngine;

[Serializable]
public class AnnouncerConfiguration
{
	public string id;

	public Transform announcerStart;

	public Transform announcerEnd;

	public Transform speechStart;

	public Transform speechEnd;

	public Transform continueTextAnchor;

	public Transform bubbleArrowAnchor;

	public bool flipSpeechBubble;
}
