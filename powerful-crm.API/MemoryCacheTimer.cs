using System.Timers;

namespace powerful_crm.API
{
    public class MemoryCacheTimer
    {
        private Timer _timer;
        public MemoryCacheTimer(int interval)
        {
            _timer = new Timer
            {
                Interval = interval,
                AutoReset = true,
                Enabled = true
            };
        }

        public void SubscribeToTimer(ElapsedEventHandler method) => _timer.Elapsed += method;
        public void UnsubscribeFromTimer(ElapsedEventHandler method) => _timer.Elapsed -= method;
    }
}

