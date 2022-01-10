using UnityEngine;

namespace BrainWorks.Senses
{
	public class Detectable : MonoBehaviour
	{
		[Tooltip("Should we add object to detectables on object initialization?")] [SerializeField]
		private bool detectOnInitialization = true;

		private Bounds _bounds;
		private Transform _transform;

		protected virtual void Awake()
		{
			_bounds = GetComponent<Collider>().bounds;
			_transform = transform;
		}

		protected virtual void Start()
		{
			if (detectOnInitialization)
				DetectableHolder.AddToDetectables(this);
		}

		protected void OnDestroy()
		{
			DetectableHolder.RemoveFromDetectables(this);
		}

		public Bounds GetBounds()
		{
			return _bounds;
		}

		public Vector3 GetPosition()
		{
			return _transform.position;
		}
	}
}