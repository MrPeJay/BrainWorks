using UnityEngine;

namespace BrainWorks.Chunks
{
	public class VisibilityChunk : MonoBehaviour
	{
		[SerializeField] private Transform[] worldCorners = new Transform[2];
		[SerializeField] private float chunkUpdateTime = 5f;
		[SerializeField] private int divisionCount = 4;

		private Chunk _chunks;
		private bool _chunksCreated = false;

		private struct Chunk
		{
			public readonly Bounds ChunkBounds;
			public Chunk[] ChildChunks;

			public Chunk(Bounds bounds)
			{
				ChunkBounds = bounds;
				ChildChunks = null;
			}
		}

		private void Start()
		{
			CreateChunks();
		}

		private void CreateChunks()
		{
			_chunks = new Chunk(InitialBounds(worldCorners[0].position, worldCorners[1].position));
			_chunks.ChildChunks = DivideChunk(_chunks);

			_chunksCreated = true;
		}

		private static Bounds InitialBounds(Vector3 leftCornerPosition, Vector3 rightCornerPosition)
		{
			var centerPoint = CenterVector(leftCornerPosition, rightCornerPosition);

			return new Bounds(centerPoint,
				new Vector3(leftCornerPosition.x - rightCornerPosition.x,
					leftCornerPosition.y - rightCornerPosition.y,
					leftCornerPosition.z - rightCornerPosition.z));
		}

		private Chunk[] DivideChunk(Chunk parentChunk, int divisionCount = 0)
		{
			if (divisionCount == this.divisionCount)
				return null;

			var parentBounds = parentChunk.ChunkBounds;

			var leftUpBounds =
				new Bounds(CenterVector(parentBounds.min, parentBounds.center), parentBounds.extents);
			var rightUpBounds =
				new Bounds(
					new Vector3(CenterPoint(parentBounds.max.x, parentBounds.center.x),
						CenterPoint(parentBounds.max.y, parentBounds.center.y),
						CenterPoint(parentBounds.min.z, parentBounds.center.z)), parentBounds.extents);
			var leftDownBounds =
				new Bounds(
					new Vector3(CenterPoint(parentBounds.min.x, parentBounds.center.x),
						CenterPoint(parentBounds.min.y, parentBounds.center.y),
						CenterPoint(parentBounds.max.z, parentBounds.center.z)), parentBounds.extents);
			var rightDownBound =
				new Bounds(CenterVector(parentBounds.max, parentBounds.center), parentBounds.extents);

			var leftUpChunk = new Chunk(leftUpBounds);
			var rightUpChunk = new Chunk(rightUpBounds);
			var leftDownChunk = new Chunk(leftDownBounds);
			var rightDownChunk = new Chunk(rightDownBound);

			leftUpChunk.ChildChunks = DivideChunk(leftUpChunk, divisionCount + 1);
			rightUpChunk.ChildChunks = DivideChunk(rightUpChunk, divisionCount + 1);
			leftDownChunk.ChildChunks = DivideChunk(leftDownChunk, divisionCount + 1);
			rightDownChunk.ChildChunks = DivideChunk(rightDownChunk, divisionCount + 1);

			return new Chunk[] {leftUpChunk, rightUpChunk, leftDownChunk, rightDownChunk};
		}

		private static Vector3 CenterVector(Vector3 point1, Vector3 point2)
		{
			return new Vector3(CenterPoint(point1.x, point2.x),
				CenterPoint(point1.y, point2.y),
				CenterPoint(point1.z, point2.z));
		}

		private static float CenterPoint(float point1, float point2)
		{
			return point1 + (point2 - point1) / 2;
		}

		private void UpdateChunks()
		{

		}

#if UNITY_EDITOR

		private void OnDrawGizmosSelected()
		{
			if (!_chunksCreated)
				return;

			DrawChunkOutlines(_chunks);
		}

		private static void DrawChunkOutlines(Chunk chunk)
		{
			if (chunk.ChildChunks == null)
				DrawBoundOutlines(chunk.ChunkBounds);
			else
			{
				for (var i = 0; i < 4; i++)
					DrawChunkOutlines(chunk.ChildChunks[i]);

				DrawBoundOutlines(chunk.ChunkBounds);
			}
		}

		private static void DrawBoundOutlines(Bounds bounds)
		{
			var minPositionLeft = new Vector3(bounds.min.x, 0, bounds.min.z);
			var minPositionRight = new Vector3(bounds.min.x, 0, bounds.max.z);
			var maxPositionRight = new Vector3(bounds.max.x, 0, bounds.max.z);
			var maxPositionLeft = new Vector3(bounds.max.x, 0, bounds.min.z);

			Gizmos.color = Color.blue;

			Gizmos.DrawLine(minPositionLeft, minPositionRight);
			Gizmos.DrawLine(maxPositionRight, maxPositionLeft);
			Gizmos.DrawLine(maxPositionRight, minPositionRight);
			Gizmos.DrawLine(minPositionLeft, maxPositionLeft);
		}

#endif
	}
}