using UnityEngine;
using BrainWorks.Chunks;

namespace BrainWorks.Senses
{
	[RequireComponent(typeof(Camera))]
	public class CameraObjectSense : MonoBehaviour, IObjectSense
	{
		[SerializeField] private LayerMask visibleLayerMask;
		[SerializeField] private VisibilityChunk visibilityChunk;

		private Camera _camera;
		private Transform _transform;
		private VisibilityChunk.Chunk _currentChunk;

		private void Awake()
		{
			_transform = transform;
			_camera = GetComponent<Camera>();
		}

		private void Start()
		{
			_camera.enabled = false;
		}

		public Detectable[] GetVisibleObjects(int objectCount)
		{
			var detectableDatas = new DetectableData[objectCount];
			var currentPosition = _transform.position;

			var detectables = VisibilityChunk.Instance.Chunks.GetDetectables(currentPosition);

			if (detectables == null)
				return null;

			var maxDistance = 0f;
			var length = 0;

			var detectableCount = detectables.Count;
			for (var i = 0; i < detectableCount; i++)
			{
				var currentDetectable = detectables[i];

				//Check if correct layer mask.
				if (visibleLayerMask != (visibleLayerMask | 1 << currentDetectable.gameObject.layer))
					continue;

				//More performance friendly approach to see if the object is visible in a certain camera view.
				var currentDetectablePosition = currentDetectable.transform.position;
				if (!IsPositionInView(currentDetectablePosition)) continue;

				var currentObjectDistance = (currentDetectablePosition - currentPosition).sqrMagnitude;

				//Add objects till it is full.
				if (length < objectCount)
				{
					detectableDatas[length] = new DetectableData(i, currentObjectDistance);

					if (maxDistance < currentObjectDistance)
						maxDistance = currentObjectDistance; 

					length++;

					continue;
				}

				//If value is higher than the max of the first items, skip it.
				if (currentObjectDistance > maxDistance)
					continue;

				var previousDetectableIndex = -1;

				for (var j = 0; j < length; j++)
				{
					var previousDetectableData = detectableDatas[j];

					if (!(previousDetectableData.DistanceToDetectable > currentObjectDistance)) continue;

					previousDetectableIndex = j;
					break;
				}

				if (previousDetectableIndex == -1)
					continue;

				detectableDatas[previousDetectableIndex] =
					new DetectableData(i, currentObjectDistance);
			}

			var detectableArray = new Detectable[length];
			for (var i = 0; i < length; i++)
				detectableArray[i] = detectables[detectableDatas[i].DetectableIndex];

			return detectableArray;
		}

		private readonly struct DetectableData
		{
			public readonly int DetectableIndex;
			public readonly float DistanceToDetectable;

			public DetectableData(int index, float distance)
			{
				DetectableIndex = index;
				DistanceToDetectable = distance;
			}
		}

		/// <summary>
		/// Returns whether the target position is inside the camera frustum.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		private bool IsPositionInView(Vector3 position)
		{
			var screenPoint = _camera.WorldToViewportPoint(position);

			return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 &&
			       screenPoint.y > 0 && screenPoint.y < 1;
		}

		public bool IsChunkVisible(VisibilityChunk.Chunk chunk)
		{
			var bounds = chunk.BoundArray();

			for (var i = 0; i < 4; i++)
			{
				var currentBounds = bounds[i];
				var boundPosition = new Vector3(currentBounds.x, _transform.position.y, currentBounds.z);

				if (IsPositionInView(boundPosition))
					return true;
			}

			return false;
		}

#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;

			if (visibilityChunk == null)
				return;

			_currentChunk = visibilityChunk.GetChunkByPosition(transform.position);

			if (_currentChunk != null)
				VisibilityChunk.DrawBoundOutlines(_currentChunk.ChunkBounds, Color.green);
		}

#endif
	}
}