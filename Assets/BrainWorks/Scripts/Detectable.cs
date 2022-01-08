using UnityEngine;

namespace BrainWorks.Senses
{
	public class Detectable : MonoBehaviour
	{
		[Tooltip("Should we add object to detectables on object initialization?")] [SerializeField]
		private bool detectOnInitialization = true;

		private Bounds _bounds;

		protected virtual void Awake()
		{
			_bounds = GetComponent<Collider>().bounds;
		}

		protected virtual void Start()
		{
			if (detectOnInitialization)
				DetectableHolder.AddToDetectables(this);
		}

		/// <summary>
		/// Returns collider component bounds.
		/// </summary>
		/// <returns></returns>
		public Bounds GetBounds()
		{
			return _bounds;
		}
	}
}