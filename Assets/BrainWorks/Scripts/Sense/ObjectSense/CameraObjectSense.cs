using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BrainWorks.ObjectSense
{
	[RequireComponent(typeof(Camera))]
	public class CameraObjectSense : MonoBehaviour, IObjectSense
	{
		[SerializeField] private LayerMask visibleLayerMask;

		private Camera _camera;

		private void Start()
		{
			_camera = GetComponent<Camera>();
			_camera.enabled = false;
		}

		public GameObject[] GetVisibleObjects(int objectCount)
		{
			var visibleGameObjects = new Dictionary<GameObject, float>(objectCount);
			var planes = GeometryUtility.CalculateFrustumPlanes(_camera);

			//Get all available renderers.
			var detectables = DetectableHolder.GetDetectables();

			for (var i = 0; i < detectables.Length; i++)
			{
				var currentTarget = detectables[i].GetRendererComponent();

				//Check if correct layer mask.
				if (visibleLayerMask != (visibleLayerMask | 1 << currentTarget.gameObject.layer))
					continue;

				if (GeometryUtility.TestPlanesAABB(planes, currentTarget.bounds))
				{
					var currentObjectDistance = (currentTarget.transform.position - transform.position).sqrMagnitude;

					//Add objects till it is full.
					if (visibleGameObjects.Count < objectCount)
					{
						visibleGameObjects.Add(currentTarget.gameObject, currentObjectDistance);
						continue;
					}

					GameObject previousObject = null, currentObject = currentTarget.gameObject;

					foreach (var currentVisibleGameObject in visibleGameObjects.Keys)
					{
						var previousObjectDistance = visibleGameObjects[currentVisibleGameObject];

						if (previousObjectDistance > currentObjectDistance)
						{
							previousObject = currentVisibleGameObject;
							break;
						}
					}

					if (previousObject != null)
					{
						visibleGameObjects.Remove(previousObject);
						visibleGameObjects.Add(currentObject, currentObjectDistance);
					}
				}
			}

			return visibleGameObjects.Keys.ToArray();
		}
	}
}