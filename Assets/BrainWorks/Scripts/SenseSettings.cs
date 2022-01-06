using System;
using UnityEngine;

namespace BrainWorks.Senses.Settings
{
	[CreateAssetMenu(fileName = "Sense Settings", menuName = "BrainWorks/Sense Settings")]
	public class SenseSettings : ScriptableObject
	{
		[Header("Sense Tick Time")] [Tooltip("Time it takes to update vision sense")]
		[SerializeField] private float visionTickTime = 0.25f;

		[Tooltip("Times it takes to update hearing sense")]
		[SerializeField] private float hearingTickTime = 0.5f;

		[Tooltip("Times it takes to update hearing sense")]
		[SerializeField] private float smellTickTime = 1f;

		[Header("Sense Object Count")] public int maxVisibleObjects = 25;
		[SerializeField] private int maxHearingObjects = 10;
		[SerializeField] private int maxSmellableObjects = 5;

		public float GetTickTime(ISense.SenseType senseType)
		{
			switch (senseType)
			{
				case ISense.SenseType.Vision:
					return visionTickTime;
				case ISense.SenseType.Hearing:
					return hearingTickTime;
				case ISense.SenseType.Smell:
					return smellTickTime;
				default:
					throw new ArgumentOutOfRangeException(nameof(senseType), senseType, null);
			}
		}

		public int GetObjectCount(ISense.SenseType senseType)
		{
			switch (senseType)
			{
				case ISense.SenseType.Vision:
					return maxVisibleObjects;
				case ISense.SenseType.Hearing:
					return maxHearingObjects;
				case ISense.SenseType.Smell:
					return maxSmellableObjects;
				default:
					throw new ArgumentOutOfRangeException(nameof(senseType), senseType, null);
			}
		}
	}
}