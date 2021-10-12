using System.Collections.Generic;
using UnityEngine;

namespace BrainWorks.ObjectSense
{
	[RequireComponent(typeof(SphereCollider))]
	public class SphereObjectSense : MonoBehaviour, IObjectSense
	{
		private const int MaxColliderAmount = 128;

		[SerializeField] private float radius;
		[SerializeField] private LayerMask visibleLayerMask;

		private Collider[] _colliders = new Collider[MaxColliderAmount];

		public GameObject[] GetVisibleObjects()
		{
			var visibleObjects = new List<GameObject>();

			var colliderAmount =
				Physics.OverlapSphereNonAlloc(transform.position, radius, _colliders, visibleLayerMask);

			for (var i = 0; i < colliderAmount; i++)
				visibleObjects.Add(_colliders[i].gameObject);

			return visibleObjects.ToArray();
		}

		/// <summary>
		/// Returns the sphere radius amount.
		/// </summary>
		/// <returns></returns>
		public float GetRadius() => radius;

#if UNITY_EDITOR

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, radius);
		}

#endif
	}
}