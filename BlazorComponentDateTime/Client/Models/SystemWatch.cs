using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BlazorAppTest.Client.Models
{
    public class SystemWatch : IDisposable
    {
        //public event EventHandler<DateTime>? MillisecondChangedEvent;
        public event EventHandler<DateTime>? SecondChangedEvent;
        public event EventHandler<DateTime>? MinuteChangedEvent;
        public event EventHandler<DateTime>? HourChangedEvent;
        public event EventHandler<DateTime>? DayChangedEvent;
        public event EventHandler<DateTime>? WeekChangedEvent;
        public event EventHandler<DateTime>? MonthChangedEvent;
        public event EventHandler<DateTime>? YearChangedEvent;
        public event EventHandler<string>? Separator;

        #region FILEDS
        private DateTime StartingDateSeconds = DateTime.Now;
        private DateTime StartingDateMinutes = DateTime.Now;
        private DateTime StartingDateHours = DateTime.Now;
        private DateTime StartingDateDay = DateTime.Now;
        private int StartingDateWeek = GetIso8601WeekOfYear(DateTime.Now);
        private DateTime StartingDateMonth = DateTime.Now;
        private DateTime StartingDateYear = DateTime.Now;
        #endregion

        private static System.Timers.Timer? aTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemWatch"/> class.
        /// </summary>
        /// <param name="interval">The interval in milliseconds [minimum 500].</param>
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
            //// Milliseconds
            //MillisecondChangedEvent?.Invoke(this, DateTime.Now);

            // Useful for separator event
            Separator?.Invoke(this, e.Second % 2 == 0 ? " " : ":");


            // Seconds
            if ((e - StartingDateSeconds).Seconds > 0)
            {
                StartingDateSeconds = e;
                SecondChangedEvent?.Invoke(this, e);
            }

            // Minutes
            if ((e - StartingDateMinutes).Minutes > 0)
            {
                StartingDateMinutes = DateTime.Now;
                MinuteChangedEvent?.Invoke(this, e);
            }

            // Hours
            if ((e - StartingDateHours).Hours > 0)
            {
                StartingDateHours = DateTime.Now;
                HourChangedEvent?.Invoke(this, e);
            }

            // Day
            if ((e - StartingDateDay).Days > 0)
            {
                StartingDateDay = DateTime.Now;
                DayChangedEvent?.Invoke(this, e);
            }

            // Week
            if (GetIso8601WeekOfYear(e) > StartingDateWeek)
            {
                StartingDateWeek = GetIso8601WeekOfYear(e);
                WeekChangedEvent?.Invoke(this, e);
            }

            // Month
            if ((e.Month - StartingDateMonth.Month) != 0)
            {
                StartingDateMonth = DateTime.Now;
                MonthChangedEvent?.Invoke(this, e);
            }

            // Year
            if ((e.Year - StartingDateYear.Year) != 0)
            {
                StartingDateYear = DateTime.Now;
                YearChangedEvent?.Invoke(this, e);
            }

        }

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        private static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
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
