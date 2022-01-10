using System.Collections.Generic;
using BrainWorks.Chunks;
using UnityEngine;

namespace BrainWorks.Senses
{
	public static class DetectableHolder
	{
		public static List<Detectable> Detectables = new List<Detectable>();

		public static Detectable[] GetDetectables()
		{
			return Detectables.ToArray();
		}

		public static Detectable[] GetDetectablesByPosition(Vector3 position)
		{
			return VisibilityChunk.Instance.Chunks.GetDetectables(position);
		}

		public static void AddToDetectables(Detectable detectable)
		{
			Detectables.Add(detectable);
			VisibilityChunk.Instance.Chunks.AssignDetectable(detectable);
		}

		public static void RemoveFromDetectables(Detectable detectable)
		{
			if (!Detectables.Contains(detectable))
				return;

			Detectables.Remove(detectable);
		}
	}
}