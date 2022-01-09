using BrainWorks.Senses;
using UnityEngine;

namespace BrainWorks.Chunks
{
	public class VisibilityChunk : MonoBehaviour
	{
		private const int ChildChunkCount = 4;
		private const float ChunkHeight = 256f;

		[SerializeField] private Transform[] worldCorners = new Transform[2];
		[SerializeField] private float chunkUpdateTime = 5f;
		[SerializeField] private int divisionCount = 4;

		private Chunk _chunks;

		public class Chunk
		{
			public Bounds ChunkBounds;
			public Detectable[] Detectables;
			public Chunk[] ChildChunks;
			public int Depth;

			public Chunk(Bounds bounds, int depth = 0)
			{
				ChunkBounds = bounds;
				Depth = depth;
			}

			public bool ContainsChildChunks()
			{
				return ChildChunks != null;
			}

			public bool ContainsPosition(Vector3 position)
			{
				return ChunkBounds.Contains(position);
			}
		}

		private void Start()
		{
			CreateChunks();
		}

		public Chunk GetChunkByPosition(Vector3 position)
		{
			if (_chunks == null)
				return null;

			//Is outside of visibility chunks.
			if (!_chunks.ContainsPosition(position))
				return null;

			return GetChunk(_chunks, position);
		}

		private static Chunk GetChunk(Chunk parentChunk, Vector3 position)
		{
			if (!parentChunk.ContainsChildChunks())
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
			_chunks = new Chunk(InitialBounds(worldCorners[0].position, worldCorners[1].position));
			_chunks.ChildChunks = DivideChunk(_chunks);
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

			if (_chunks == null)
				return;

			DrawChunkOutlines(_chunks);
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