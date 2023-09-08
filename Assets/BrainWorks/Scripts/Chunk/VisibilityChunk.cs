using System.Collections.Generic;
using BrainWorks.Senses;
using UnityEngine;

namespace BrainWorks.Chunks
{
	public class VisibilityChunk : MonoBehaviour
	{
		public static VisibilityChunk Instance;
		public Chunk Chunks;

		private const int ChildChunkCount = 4;
		private const float ChunkHeight = 256f;

		[SerializeField] private Transform[] worldCorners = new Transform[2];
		[SerializeField] private int divisionCount = 4;
		[SerializeField] private bool dynamicChunks = false;

		public class Chunk
		{
			public Bounds ChunkBounds;
			public int Depth;
			public bool HasChildChunks, HasParentChunk;

			public Chunk[] ChildChunks
			{
				get => _childChunks;
				set
				{
					_childChunks = value;
					HasChildChunks = value != null;
				}
			}

			public Chunk ParentChunk
			{
				get => _parentChunk;
				set
				{
					_parentChunk = value;
					HasParentChunk = value != null;
				}
			}

			private readonly Dictionary<Detectable, Vector3> _detectables = new Dictionary<Detectable, Vector3>();
			private readonly List<Detectable> _detectablesList = new List<Detectable>();

			private Chunk _parentChunk;
			private Chunk[] _childChunks;

			public Chunk(Bounds bounds, int depth = 0)
			{
				ChunkBounds = bounds;
				Depth = depth;
				ChildChunks = null;
			}

			public bool ContainsPosition(Vector3 position)
			{
				return ChunkBounds.Contains(position);
			}

			private void OnDetectablePositionChanged(Detectable detectable, Vector3 currentPosition)
			{
				AssignDetectable(detectable, currentPosition);
			}

			public void AssignDetectable(Detectable detectable, Vector3 currentPosition, bool parentAssign = false)
			{
				if (!ContainsPosition(currentPosition))
				{
					//If position is not in bounds and has parent chunk, assign it there.
					if (!parentAssign && HasParentChunk)
						ParentChunk.AssignDetectable(detectable, currentPosition);

					if (_detectables.ContainsKey(detectable))
					{
						_detectables.Remove(detectable);
						_detectablesList.Remove(detectable);
						detectable.OnPositionChanged -= OnDetectablePositionChanged;
					}

					return;
				}

				if (!HasChildChunks)
				{
					if (!_detectables.ContainsKey(detectable))
					{
						_detectables.Add(detectable, currentPosition);
						_detectablesList.Add(detectable);
						detectable.OnPositionChanged += OnDetectablePositionChanged;
					}

					return;
				}

				for (var i = 0; i < ChildChunkCount; i++)
					ChildChunks[i].AssignDetectable(detectable, currentPosition, true);
			}

			public List<Detectable> GetDetectables(Vector3 position)
			{
				if (!ContainsPosition(position))
					return null;

				if (!HasChildChunks)
					return _detectablesList;

				for (var i = 0; i < ChildChunkCount; i++)
				{
					var detectables = ChildChunks[i].GetDetectables(position);

					if (detectables != null)
						return detectables;
				}

				return null;
			}

			public Vector3[] BoundArray()
			{
				return new Vector3[]
				{
					new Vector3(ChunkBounds.min.x, 0, ChunkBounds.min.z),
					new Vector3(ChunkBounds.min.x, 0, ChunkBounds.max.z),
					new Vector3(ChunkBounds.max.x, 0, ChunkBounds.max.z),
					new Vector3(ChunkBounds.max.x, 0, ChunkBounds.min.z)
				};
			}
		}

		public void SubscribeToVisibilityChunk(Detectable detectable, Vector3 currentPosition)
		{
			Chunks.AssignDetectable(detectable, currentPosition);
		}

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			CreateChunks();
		}

		public Chunk GetChunkByPosition(Vector3 position)
		{
			if (Chunks == null)
				return null;

			//Is outside of visibility chunks.
			if (!Chunks.ContainsPosition(position))
				return null;

			return GetChunk(Chunks, position);
		}

		private static Chunk GetChunk(Chunk parentChunk, Vector3 position)
		{
			if (!parentChunk.HasChildChunks)
				return parentChunk;

			for (var i = 0; i < ChildChunkCount; i++)
			{
				var childChunk = parentChunk.ChildChunks[i];

				if (!childChunk.ContainsPosition(position))
					continue;

				return GetChunk(childChunk, position);
			}

			return parentChunk;
		}

		private void CreateChunks()
		{
			Chunks = new Chunk(InitialBounds(worldCorners[0].position, worldCorners[1].position));
			Chunks.ChildChunks = DivideChunk(Chunks);
		}

		private static Bounds InitialBounds(Vector3 leftCornerPosition, Vector3 rightCornerPosition)
		{
			var centerPoint = CenterVector(leftCornerPosition, rightCornerPosition);

			return new Bounds(centerPoint,
				new Vector3(Mathf.Abs(leftCornerPosition.x - rightCornerPosition.x),
					ChunkHeight,
					Mathf.Abs(leftCornerPosition.z - rightCornerPosition.z)));
		}

		private Chunk[] DivideChunk(Chunk parentChunk, int divisionCount = 0)
		{
			if (divisionCount == this.divisionCount)
				return null;

			var parentBounds = parentChunk.ChunkBounds;

			var parentExtents = new Vector3(parentBounds.extents.x, ChunkHeight, parentBounds.extents.z);

			var leftUpBounds =
				new Bounds(CenterVector(parentBounds.min, parentBounds.center), parentExtents);
			var rightUpBounds =
				new Bounds(
					new Vector3(CenterPoint(parentBounds.max.x, parentBounds.center.x),
						0f,
						CenterPoint(parentBounds.min.z, parentBounds.center.z)), parentExtents);
			var leftDownBounds =
				new Bounds(
					new Vector3(CenterPoint(parentBounds.min.x, parentBounds.center.x),
						0f,
						CenterPoint(parentBounds.max.z, parentBounds.center.z)), parentExtents);
			var rightDownBound =
				new Bounds(CenterVector(parentBounds.max, parentBounds.center), parentExtents);

			var leftUpChunk = new Chunk(leftUpBounds, divisionCount + 1);
			var rightUpChunk = new Chunk(rightUpBounds, divisionCount + 1);
			var leftDownChunk = new Chunk(leftDownBounds, divisionCount + 1);
			var rightDownChunk = new Chunk(rightDownBound, divisionCount + 1);

			leftUpChunk.ChildChunks = DivideChunk(leftUpChunk, divisionCount + 1);
			rightUpChunk.ChildChunks = DivideChunk(rightUpChunk, divisionCount + 1);
			leftDownChunk.ChildChunks = DivideChunk(leftDownChunk, divisionCount + 1);
			rightDownChunk.ChildChunks = DivideChunk(rightDownChunk, divisionCount + 1);

			leftUpChunk.ParentChunk = parentChunk;
			rightUpChunk.ParentChunk = parentChunk;
			leftDownChunk.ParentChunk = parentChunk;
			rightDownChunk.ParentChunk = parentChunk;

			//parentChunk.HasChildChunks = true;

			return new Chunk[] {leftUpChunk, rightUpChunk, leftDownChunk, rightDownChunk};
		}

		private static Vector3 CenterVector(Vector3 point1, Vector3 point2)
		{
			return new Vector3(CenterPoint(point1.x, point2.x),
				0f,
				CenterPoint(point1.z, point2.z));
		}

		private static float CenterPoint(float point1, float point2)
		{
			return point1 + (point2 - point1) / 2;
		}

#if UNITY_EDITOR

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;

			if (Chunks == null)
				return;

			DrawChunkOutlines(Chunks);
		}

		private static void DrawChunkOutlines(Chunk chunk)
		{
			if (chunk.ChildChunks == null)
				DrawBoundOutlines(chunk.ChunkBounds, Color.blue);
			else
			{
				for (var i = 0; i < ChildChunkCount; i++)
					DrawChunkOutlines(chunk.ChildChunks[i]);
			}
		}

		public static void DrawBoundOutlines(Bounds bounds, Color color)
		{
			var minPositionLeft = new Vector3(bounds.min.x, 0, bounds.min.z);
			var minPositionRight = new Vector3(bounds.min.x, 0, bounds.max.z);
			var maxPositionRight = new Vector3(bounds.max.x, 0, bounds.max.z);
			var maxPositionLeft = new Vector3(bounds.max.x, 0, bounds.min.z);

			Gizmos.color = color;

			Gizmos.DrawLine(minPositionLeft, minPositionRight);
			Gizmos.DrawLine(maxPositionRight, maxPositionLeft);
			Gizmos.DrawLine(maxPositionRight, minPositionRight);
			Gizmos.DrawLine(minPositionLeft, maxPositionLeft);
		}

#endif
	}
}