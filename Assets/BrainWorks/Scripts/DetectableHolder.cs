using System.Collections.Generic;

namespace BrainWorks.Senses
{
	public static class DetectableHolder
	{
		public static List<Detectable> Detectables = new List<Detectable>();

		/// <summary>
		/// Returns all cached detectables.
		/// </summary>
		/// <returns></returns>
		public static Detectable[] GetDetectables()
		{
			return Detectables.ToArray();
		}

		public static void AddToDetectables(Detectable detectable)
		{
			Detectables.Add(detectable);
		}

		public static void RemoveFromDetectables(Detectable detectable)
		{
			Detectables.Remove(detectable);
		}
	}
}