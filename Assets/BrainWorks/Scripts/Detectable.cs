using System;
using System.Collections;
using BrainWorks.Chunks;
using UnityEngine;

namespace BrainWorks.Senses
{
	public class Detectable : MonoBehaviour
	{
		[Tooltip("Should we add object to detectables on object initialization?")] [SerializeField]
		private bool detectOnInitialization = true;

		private Bounds _bounds;
		private Transform _transform;

		public delegate void PositionChanged(Detectable detectable, Vector3 newPosition);

		public PositionChanged OnPositionChanged;

		private Vector3 _previousPosition;

		protected virtual void Awake()
		{
			_bounds = GetComponent<Collider>().bounds;
			_transform = transform;

			_previousPosition = _transform.position;
		}

		protected virtual void Start()
		{
			if (detectOnInitialization)
				VisibilityChunk.Instance.SubscribeToVisibilityChunk(this, _transform.position);
		}

		private void Update()
		{
			if (!_previousPosition.Equals(_transform.position))
				OnPositionChanged(this, _transform.position);
		}

		public Bounds GetBounds()
		{
			return _bounds;
		}
	}
}