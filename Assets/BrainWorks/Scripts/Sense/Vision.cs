using System.Collections;
using System.Collections.Generic;
using BrainWorks.Extensions;
using UnityEngine;

namespace BrainWorks.Senses
{
	[RequireComponent(typeof(IObjectSense))]
	public class Vision : MonoBehaviour
	{
		[SerializeField] private float visionUpdateTime = 0.2f;

		private readonly List<GameObject> _visibleGameObjects = new List<GameObject>();

		private IObjectSense _objectSense;

		private void Start()
		{
			_objectSense = GetComponent<IObjectSense>();

			StartCoroutine(VisionUpdate());
		}

		private IEnumerator VisionUpdate()
		{
			var wait = new WaitForSeconds(visionUpdateTime);

			while (true)
			{
				yield return wait;
				GatherVisibleObjects();
			}
		}

		/// <summary>
		/// Gathers all visible objects based on the assigned object sense component.
		/// </summary>
		private void GatherVisibleObjects()
		{
			_visibleGameObjects.Clear();

			var visibleObjects = _objectSense.GetVisibleObjects();

			for (var i = 0; i < visibleObjects.Length; i++)
				FieldOfViewCheck(visibleObjects[i]);
		}

		private void FieldOfViewCheck(GameObject target)
		{
			var objectPosition = transform.position;

			var targetPosition = target.transform.position;
			var directionToTarget = (targetPosition - objectPosition).normalized;

			var isVisible = false;

			//Check if no obstacles are in the way.
			if (Physics.Raycast(objectPosition, directionToTarget, out var hitInfo, Mathf.Infinity))
			{
				if (hitInfo.transform.gameObject.Equals(target))
					isVisible = true;
			}

			if (!isVisible && CanSeeBounds(target, objectPosition))
				isVisible = true;

			if (isVisible)
			{
				if (!_visibleGameObjects.Contains(target))
					_visibleGameObjects.Add(target);
			}
			else
			{
				if (_visibleGameObjects.Contains(target))
					_visibleGameObjects.Remove(target);
			}
		}

		/// <summary>
		/// Checks if collider bounds are visible.
		/// </summary>
		/// <param name="targetObject"></param>
		/// <param name="objectPosition"></param>
		/// <returns></returns>
		private static bool CanSeeBounds(GameObject targetObject, Vector3 objectPosition)
		{
			var bounds = targetObject.GetComponent<Collider>().bounds.BoundPositions();

			for (var i = 0; i < bounds.Length; i++)
			{
				var currentBoundPosition = bounds[i];
				var directionToBoundPosition = (currentBoundPosition - objectPosition).normalized;

				//Check if no obstacles are in the way.
				if (!Physics.Raycast(objectPosition, directionToBoundPosition, out var hitInfo)) continue;

				if (!hitInfo.transform.gameObject.Equals(targetObject)) continue;

				return true;
			}

			return false;
		}

#if UNITY_EDITOR

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;

			for (var i = 0; i < _visibleGameObjects.Count; i++)
				Gizmos.DrawLine(transform.position, _visibleGameObjects[i].transform.position);
		}

#endif
	}
}