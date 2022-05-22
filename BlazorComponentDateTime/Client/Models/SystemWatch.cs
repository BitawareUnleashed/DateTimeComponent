using System.Globalization;
using System.Timers;

namespace BlazorComponentDateTime.Client.Models
{
    public class SystemWatch : IDisposable
    {
        public event EventHandler<DateTime>? SecondChangedEvent;
        public event EventHandler<string>? Separator;

        #region FILEDS
        private DateTime StartingDateSeconds = DateTime.Now;
        #endregion

        private static System.Timers.Timer? aTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemWatch"/> class.
        /// </summary>
        /// <param name="interval">The interval in milliseconds.</param>
        public SystemWatch(int interval = 10)
        {
            // Create a timer and set a two second interval.
            if (aTimer == null)
            {
                aTimer = new System.Timers.Timer();
                aTimer.Interval = interval;

                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed -= ATimer_Elapsed!;
                aTimer.Elapsed += ATimer_Elapsed!;

                // Have the timer fire repeated events (true is the default)
                aTimer.AutoReset = true;

                // Start the timer
                aTimer.Enabled = true;
            }
        }

        private void ATimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeCheck(e.SignalTime);
        }

        #region Methods

        private void TimeCheck(DateTime e)
        {
            // Useful for separator event
            Separator?.Invoke(this, e.Second % 2 == 0 ? " " : ":");


            // Seconds
            if ((e - StartingDateSeconds).Seconds > 0)
            {
                StartingDateSeconds = e;
                SecondChangedEvent?.Invoke(this, e);
                Console.WriteLine(".NET Timer: "+e.Hour.ToString("00") + ":" + e.Minute.ToString("00") + ":" + e.Second.ToString("00"));
            }
        }
        #endregion

        #region IDisposable
        // To detect redundant calls
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            // If already disposed then exit
            if (_disposed) { return; }
            // Dispose managed state (managed objects).
            if (disposing) { aTimer!.Dispose(); }
            _disposed = true;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
