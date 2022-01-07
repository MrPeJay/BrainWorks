namespace BrainWorks.Senses
{
	public interface IObjectSense
	{
		/// <summary>
		/// Returns all visible objects by the specific sense.
		/// </summary>
		/// <param name="objectCount">Max number of objects that should be contained</param>
		/// <returns></returns>
		Detectable[] GetVisibleObjects(int objectCount);
	}
}