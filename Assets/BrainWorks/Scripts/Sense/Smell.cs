using UnityEngine;

namespace BrainWorks.Senses
{
	public class Smell : MonoBehaviour, ISense
	{
		public void Tick(int objectCount)
		{
			throw new System.NotImplementedException();
		}

		public ISense.SenseType GetSenseType()
		{
			return ISense.SenseType.Smell;
		}
	}
}