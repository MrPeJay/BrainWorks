using UnityEngine;

namespace BrainWorks.ObjectSense
{
	public class Detectable : MonoBehaviour
	{
		[Tooltip("Should we add object to detectables on object initialization?")] [SerializeField]
		private bool detectOnInitialization = true;

		private Renderer _renderer;
		private Bounds _bounds;

		protected virtual void Start()
		{
			_renderer = GetComponent<Renderer>();
			_bounds = GetComponent<Collider>().bounds;

			if (detectOnInitialization)
				DetectableHolder.AddToDetectables(this);
		}

		/// <summary>
		/// Returns assigned object renderer component.
		/// </summary>
		/// <returns></returns>
		public Renderer GetRendererComponent()
		{
			return _renderer;
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