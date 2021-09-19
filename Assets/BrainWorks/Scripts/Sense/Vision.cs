using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BrainWorks.Extensions;
using BrainWorks.ObjectSense;
using UnityEngine;

namespace BrainWorks.Senses
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(IObjectSense))]
	public class Vision : MonoBehaviour
	{
		[SerializeField] private bool inheritFromObjectSense = true;
		[Range(0f, 360f)] [SerializeField] private float angle = 90f;
		[SerializeField] private float visionUpdateTime = 0.2f;

		private List<GameObject> _visibleGameObjects = new List<GameObject>();

#if UNITY_EDITOR
		private IObjectSense _objectSense;
#endif

		private void Start()
		{
			if (Application.isPlaying)
				StartCoroutine(VisionUpdate());

#if UNITY_EDITOR
			_objectSense = GetComponent<IObjectSense>();
#endif
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
			var visibleObjects = GetComponent<IObjectSense>().GetVisibleObjects();

			for (var i = 0; i < visibleObjects.Length; i++)
				FieldOfViewCheck(visibleObjects[i]);

			RemoveUnseen(visibleObjects);
		}

		/// <summary>
		/// Removes unseen objects that are still available in the visible object list.
		/// </summary>
		/// <param name="visibleObjects"></param>
		private void RemoveUnseen(GameObject[] visibleObjects)
		{
			var objectsToRemove = new List<GameObject>();

			for (var i = 0; i < _visibleGameObjects.Count; i++)
			{
				var currentObject = _visibleGameObjects[i];

				if (!visibleObjects.Contains(currentObject))
					objectsToRemove.Add(currentObject);
			}

			foreach (var objectToRemove in objectsToRemove)
				_visibleGameObjects.Remove(objectToRemove);
		}

		private void FieldOfViewCheck(GameObject target)
		{
			var targetTransform = target.transform;
			var directionToTarget = (targetTransform.position - transform.position).normalized;

			if (!inheritFromObjectSense)
			{
				if (Vector3.Angle(transform.forward, directionToTarget) > angle / 2)
					return;
			}

			//Check if center of mass position is within the correct angle.
			var distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

			//Check if no obstacles are in the way.
			if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget))
			{
				if (!_visibleGameObjects.Contains(target))
					_visibleGameObjects.Add(target);

				return;
			}

			//Can't use center of mass, check for bounds.
			var bounds = target.GetComponent<Collider>().bounds.BoundPositions();

			for (var i = 0; i < bounds.Length; i++)
			{
				var currentBoundPosition = bounds[i];
				var directionToBoundPosition = (currentBoundPosition - transform.position).normalized;

				if (!inheritFromObjectSense)
				{
					if (Vector3.Angle(transform.forward, directionToBoundPosition) > angle / 2)
						return;
				}

				var distanceToBoundPosition = Vector3.Distance(transform.position, currentBoundPosition);

				//Check if no obstacles are in the way.
				if (!Physics.Raycast(transform.position, directionToBoundPosition, distanceToBoundPosition)) continue;

				//Save position.
				if (!_visibleGameObjects.Contains(target))
					_visibleGameObjects.Add(target);

				return;
			}
		}

#if UNITY_EDITOR

		private void OnDrawGizmosSelected()
		{
			if (!inheritFromObjectSense)
			{
				var sphereObjectSense = _objectSense as SphereObjectSense;

				if (sphereObjectSense != null)
				{
					var radius = sphereObjectSense.GetRadius();

					Vector3 viewAngle01 = DirectionFromAngle(transform.eulerAngles.y, -angle / 2);
					Vector3 viewAngle02 = DirectionFromAngle(transform.eulerAngles.y, angle / 2);

					Gizmos.color = Color.yellow;
					Gizmos.DrawLine(transform.position, transform.position + viewAngle01 * radius);
					Gizmos.DrawLine(transform.position, transform.position + viewAngle02 * radius);
				}
			}

			Gizmos.color = Color.green;

			for (var i = 0; i < _visibleGameObjects.Count; i++)
				Gizmos.DrawLine(transform.position, _visibleGameObjects[i].transform.position);
		}

		private static Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
		{
			angleInDegrees += eulerY;

			return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
		}

#endif
	}
}