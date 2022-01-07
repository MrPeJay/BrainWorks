using UnityEngine;

namespace BrainWorks.Senses
{
	[RequireComponent(typeof(SphereCollider))]
	public class SphereObjectSense : MonoBehaviour, IObjectSense
	{
		private const int MaxColliderAmount = 128;

		[SerializeField] private float radius;
		[SerializeField] private LayerMask visibleLayerMask;

		private Collider[] _colliders = new Collider[MaxColliderAmount];

		public Detectable[] GetVisibleObjects(int objectCount)
		{
			var colliderAmount =
				Physics.OverlapSphereNonAlloc(transform.position, radius, _colliders, visibleLayerMask);

			var visibleDetectables = new Detectable[colliderAmount];

			for (var i = 0; i < colliderAmount; i++)
				visibleDetectables[i] = _colliders[i].GetComponent<Detectable>();

			return visibleDetectables;
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