using System;
using System.Text;
using System.Windows.Threading;

namespace Storm
{
    public class CountdownDispatcherTimer
    {
        #region Fields
        private DateTime created = DateTime.Now;
        private Action tick = null;
        private DispatcherTimer timer = new DispatcherTimer();
        #endregion

        #region Properties
        public bool IsActive
        {
            get
            {
                // when timer is NOT null, IsActive == true; when timer IS null, isActive == false
                return (timer != null);
            }
        }

        public TimeSpan TimeLeft
        {
            get
            {
                if (timer == null)
                {
                    return TimeSpan.Zero;
                }
                else
                {
                    // created + timer.Interval is the time in the future when it fires, so minus DateTime.Now is how much time is left between now and then
                    return ((created + timer.Interval) - DateTime.Now);
                }
            }
        }
        #endregion

        public CountdownDispatcherTimer(DateTime time, Action tick)
        {
            if (time < DateTime.Now) throw new ArgumentException(string.Format("{0} is in the past, it must be in the future", time.ToString()));
            if (tick == null) throw new ArgumentNullException("tick event is null");

            this.tick = tick;

            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan((time - DateTime.Now).Ticks);

            timer.Start();
        }

        public CountdownDispatcherTimer(TimeSpan span, Action tick)
        {
            if (span.Ticks < 10000000L) throw new ArgumentException("span cannot be less than 1 second"); // 10,000 ticks in 1 ms => 10,000 * 1000 ticks in 1 s == 10,000,000 ticks
            if (tick == null) throw new ArgumentNullException("tick event is null");

            this.tick = tick;

            timer.Tick += timer_Tick;
            timer.Interval = span;

            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            tick();

            Stop();
        }

        public void Stop()
        {
            if (IsActive)
            {
                timer.Stop();
                timer.Tick -= timer_Tick;

                timer = null;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(this.GetType().ToString());
            sb.AppendLine(string.Format("Created at: {0}", created.ToString()));
            sb.AppendLine(string.Format("Active: {0}", IsActive));
            sb.AppendLine(string.Format("Time left: {0}", TimeLeft.ToString()));

            return sb.ToString();
        }
    }
}