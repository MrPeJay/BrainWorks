using System.Collections.Generic;
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

		public GameObject[] GetVisibleObjects()
		{
			_camera.enabled = true;

			var visibleGameObjects = new List<GameObject>();

			var planes = GeometryUtility.CalculateFrustumPlanes(_camera);

			//Get all available renderers.
			var renderers = FindObjectsOfType<Renderer>();

			for (var i = 0; i < renderers.Length; i++)
			{
				var currentTarget = renderers[i];

				//Check if correct layer mask.
				if (visibleLayerMask != (visibleLayerMask | 1 << currentTarget.gameObject.layer))
					continue;

				if (GeometryUtility.TestPlanesAABB(planes, currentTarget.bounds))
					visibleGameObjects.Add(currentTarget.gameObject);
			}

			_camera.enabled = false;

			return visibleGameObjects.ToArray();
		}
	}
}