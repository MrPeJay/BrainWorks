using UnityEngine;

namespace BrainWorks.Senses.Settings
{
	[CreateAssetMenu(fileName = "Sense Settings", menuName = "BrainWorks/Sense Settings")]
	public class SenseSettings : ScriptableObject
	{
		[Header("Sense Tick Time")] [Tooltip("Time it takes to update vision sense")]
		public float VisionTickTime = 0.25f;

		[Tooltip("Times it takes to update hearing sense")]
		public float HearingTickTime = 0.5f;

		[Tooltip("Times it takes to update hearing sense")]
		public float SmellTickTime = 1f;

		[Header("Sense Object Count")] public int MaxVisibleObjects = 25;
		public int MaxHearingObjects = 10;
		public int MaxSmellableObjects = 5;
	}
}