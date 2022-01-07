using System.Collections.Generic;
using UnityEngine;

namespace BrainWorks.Senses
{
	[RequireComponent(typeof(IObjectSense))]
	public class Vision : MonoBehaviour, ISense
	{
		private readonly List<Detectable> _visibleDetectables = new List<Detectable>();

		private IObjectSense _objectSense;

		private void Start()
		{
			_objectSense = GetComponent<IObjectSense>();
		}

		public void Tick(int objectCount)
		{
			GatherVisibleObjects(objectCount);
		}

		public ISense.SenseType GetSenseType()
		{
			return ISense.SenseType.Vision;
		}

		/// <summary>
		/// Gathers all visible objects based on the assigned object sense component.
		/// </summary>
		private void GatherVisibleObjects(int objectCount)
		{
			_visibleDetectables.Clear();

			var visibleObjects = _objectSense.GetVisibleObjects(objectCount);

			var visibleObjectCount = visibleObjects.Length;
			for (var i = 0; i < visibleObjectCount; i++)
				VisibilityCheck(visibleObjects[i]);
		}

		private void VisibilityCheck(Detectable target)
		{
			var objectPosition = transform.position;

			var targetPosition = target.transform.position;
			var directionToTarget = (targetPosition - objectPosition).normalized;

			var isVisible = false;

			//Check if no obstacles are in the way.
			if (Physics.Raycast(objectPosition, directionToTarget, out var hitInfo, Mathf.Infinity))
			{
				if (hitInfo.transform.gameObject.Equals(target.gameObject))
					isVisible = true;
			}

			if (!isVisible && CanSeeBounds(target, objectPosition))
				isVisible = true;

			if (isVisible)
			{
				if (!_visibleDetectables.Contains(target))
					_visibleDetectables.Add(target);
			}
			else
			{
				if (_visibleDetectables.Contains(target))
					_visibleDetectables.Remove(target);
			}
		}

		/// <summary>
		/// Checks if collider bounds are visible.
		/// </summary>
		/// <param name="targetObject"></param>
		/// <param name="objectPosition"></param>
		/// <returns></returns>
		private static bool CanSeeBounds(Detectable targetObject, Vector3 objectPosition)
		{
			var bounds = BoundPositions(targetObject.GetBounds());

			var boundCount = bounds.Length;
			for (var i = 0; i < boundCount; i++)
			{
				var currentBoundPosition = bounds[i];
				var directionToBoundPosition = (currentBoundPosition - objectPosition).normalized;

				//Check if no obstacles are in the way.
				if (!Physics.Raycast(objectPosition, directionToBoundPosition, out var hitInfo)) continue;

				if (!hitInfo.transform.gameObject.Equals(targetObject.gameObject)) continue;

				return true;
			}

			return false;
		}

		private static Vector3[] BoundPositions(Bounds bounds)
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

#if UNITY_EDITOR

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;

			for (var i = 0; i < _visibleDetectables.Count; i++)
				Gizmos.DrawLine(transform.position, _visibleDetectables[i].transform.position);
		}

#endif
	}
}