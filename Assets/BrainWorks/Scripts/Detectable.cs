using UnityEngine;

namespace BrainWorks.ObjectSense
{
	public class Detectable : MonoBehaviour
	{
		[Tooltip("Should we add object to detectables on object initialization?")] [SerializeField]
		private bool detectOnInitialization = true;

		private Renderer _renderer;

		protected virtual void Start()
		{
			_renderer = GetComponent<Renderer>();

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
	}
}