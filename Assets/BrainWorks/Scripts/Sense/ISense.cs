namespace BrainWorks.Senses
{
	public interface ISense
	{
		void Tick(int objectCount);

		SenseType GetSenseType();

		public enum SenseType
		{
			Vision,
			Hearing,
			Smell
		}
	}
}