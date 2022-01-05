using UnityEngine;

namespace BrainWorks.ObjectSense
{
	public interface IObjectSense
	{
		GameObject[] GetVisibleObjects(int objectCount);
	}
}