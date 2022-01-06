using BrainWorks.Senses;
using BrainWorks.Senses.Settings;
using UnityEngine;

namespace BrainWorks.Brain
{
	public class Brain : MonoBehaviour
	{
		[SerializeField] private SenseSettings settings;

		private ISense[] _senses;
		private float[] _tickTimers;

		private void Start()
		{
			_senses = GetComponents<ISense>();

			SetTickTimers();
		}

		private void SetTickTimers()
		{
			var senseCount = _senses.Length;
			_tickTimers = new float[senseCount];

			for (var i = 0; i < senseCount; i++)
				_tickTimers[i] = settings.GetTickTime(_senses[i].GetSenseType());
		}

		private void Update()
		{
			UpdateSenseTickTimers();
		}

		private void UpdateSenseTickTimers()
		{
			var senseCount = _senses.Length;
			for (var i = 0; i < senseCount; i++)
			{
				if (_tickTimers[i] < 0)
				{
					var sense = _senses[i];
					var senseType = sense.GetSenseType();

					sense.Tick(settings.GetObjectCount(senseType));
					_tickTimers[i] = settings.GetTickTime(senseType);
				}

				_tickTimers[i] -= Time.unscaledDeltaTime;
			}
		}
	}
}