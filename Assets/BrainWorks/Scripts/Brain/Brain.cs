using System;
using System.Collections;
using BrainWorks.Senses;
using BrainWorks.Senses.Settings;
using UnityEngine;

public class Brain : MonoBehaviour
{
	[SerializeField] private SenseSettings settings;

	private ISense[] senses;

	private void Start()
	{
		senses = GetComponents<ISense>();

		for (var i = 0; i < senses.Length; i++)
		{
			var currentSense = senses[i];
			StartCoroutine(TickCoroutine(currentSense, GetTickTime(currentSense.GetSenseType())));
		}
	}

	private IEnumerator TickCoroutine(ISense sense, float timeToWait)
	{
		yield return new WaitForSeconds(timeToWait);

		var senseType = sense.GetSenseType();

		sense.Tick(GetObjectCount(senseType));
		StartCoroutine(TickCoroutine(sense, GetTickTime(senseType)));
	}

	private float GetTickTime(ISense.SenseType senseType)
	{
		switch (senseType)
		{
			case ISense.SenseType.Vision:
				return settings.VisionTickTime;
			case ISense.SenseType.Hearing:
				return settings.HearingTickTime;
			case ISense.SenseType.Smell:
				return settings.SmellTickTime;
			default:
				throw new ArgumentOutOfRangeException(nameof(senseType), senseType, null);
		}
	}

	private int GetObjectCount(ISense.SenseType senseType)
	{
		switch (senseType)
		{
			case ISense.SenseType.Vision:
				return settings.MaxVisibleObjects;
			case ISense.SenseType.Hearing:
				return settings.MaxHearingObjects;
			case ISense.SenseType.Smell:
				return settings.MaxSmellableObjects;
			default:
				throw new ArgumentOutOfRangeException(nameof(senseType), senseType, null);
		}
	}
}
