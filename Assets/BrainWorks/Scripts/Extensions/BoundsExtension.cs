using UnityEngine;

namespace BrainWorks.Extensions
{
	public static class BoundsExtension
	{
		public static Vector3[] BoundPositions(this Bounds bounds)
		{
			var boundsMax = bounds.max;
			var boundsMin = bounds.min;

			return new Vector3[]
			{
				boundsMin, boundsMax, new Vector3(boundsMin.x, boundsMin.y, boundsMax.z),
				new Vector3(boundsMin.x, boundsMax.y, boundsMin.z),
				new Vector3(boundsMax.x, boundsMin.y, boundsMin.z),
				new Vector3(boundsMin.x, boundsMax.y, boundsMax.z),
				new Vector3(boundsMax.x, boundsMin.y, boundsMax.z),
				new Vector3(boundsMax.x, boundsMax.y, boundsMin.z)
			};
		}
	}
}