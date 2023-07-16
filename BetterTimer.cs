namespace CityExtras
{
	public class BetterTimer
	{
		public BetterTimer(double triggerTime, System.Action callback)
		{
			_triggerTime = triggerTime;
			_randomResets = false;
			_time = 0;
			_hasStopped = true;
			_callback = callback;
		}

		public BetterTimer(double minTriggerTime, double maxTriggerTime, System.Action callback)
		{
			MinTriggerTime = minTriggerTime;
			MaxTriggerTime = maxTriggerTime;
			_triggerTime = Main.Rand.Next((int)minTriggerTime, (int)maxTriggerTime + 1);
			_time = 0;
			_hasStopped = true;
			_callback = callback;
		}

		private double _time { get; set; }
		private double _triggerTime { get; set; }
		private bool _randomResets { get; set; }
		public double MinTriggerTime { get; set; }
		public double MaxTriggerTime { get; set; }
		private bool _hasStopped { get; set; }
		private System.Action _callback { get; set; }

		public void IncrementTime(double deltaTime)
		{
			if (_hasStopped) return;

			_time += deltaTime;

			if (_time > _triggerTime)
			{
				_callback();
				_Reset();
			}
		}

		public void Start()
		{
			_hasStopped = false;
		}

		public void Stop()
		{
			_hasStopped = true;
		}

		private void _Reset()
		{
			if (_randomResets)
			{
				_triggerTime = Main.Rand.Next((int)MinTriggerTime, (int)MaxTriggerTime + 1);
			}

			_time = 0;
		}
	}
}
