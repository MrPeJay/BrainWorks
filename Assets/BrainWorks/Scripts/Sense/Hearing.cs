using UnityEngine;

namespace BrainWorks.Senses
{
	public class Hearing : MonoBehaviour, ISense
	{
		public void Tick(int objectCount)
		{
			throw new System.NotImplementedException();
		}

		public ISense.SenseType GetSenseType()
		{
			return ISense.SenseType.Hearing;
		}
	}
}