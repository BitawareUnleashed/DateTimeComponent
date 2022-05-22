using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlazorComponentDateTime.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorComponentDateTime.Client.Shared
{
    public partial class SystemWatchComponent
    {
        /// <summary>
        /// Gets or sets the watch format.
        /// </summary>
        /// <value>
        /// The watch format.
        /// </value>
        [Parameter] public WatchDisplayEnum ClockDisplay { get; set; } = WatchDisplayEnum.None;

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        [Parameter] public DateDisplayEnum DateDisplay { get; set; } = DateDisplayEnum.None;

        /// <summary>
        /// Gets or sets the culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        [Parameter] public CultureInfo Culture { get; set; }= CultureInfo.CurrentCulture;

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SystemWatchComponent"/> is is24h.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is24h; otherwise, <c>false</c>.
        /// </value>
        [Parameter] public bool Is24H { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether enable js time clock.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable js time]; otherwise, <c>false</c>.
        /// </value>
        [Parameter] public bool EnableJsTime { get; set; }

        private string systemWatch = string.Empty;
        private string systemDate = string.Empty;
        private string separator = ":";
        private bool separatorActive = false;
        private string watchLeftPart = string.Empty;
        private string watchRightPart = string.Empty;

        private string blinkStyleSeparator = string.Empty;

        private CancellationTokenSource ct = new CancellationTokenSource();


        protected override void OnInitialized()
        {
            if (EnableJsTime)
            {
                var dotNetReference = DotNetObjectReference.Create(this);
                JsRuntime.InvokeVoidAsync("SystemWatchCaller.NewWatch", dotNetReference);
                base.OnInitialized();
            }
            else
            {
                watch.SecondChangedEvent += Sw_SecondChangedEvent;// Watch_SecondChangedEvent;
            }
        }

        private void Watch_SecondChangedEvent(object? sender, string e)
        {
            UpdateWatch(e);
        }

        [JSInvokable("UpdateWatch")]
        public void UpdateWatch(string time)
        {
            var timePieces=time.Split(':');
            if (timePieces.Length <3)
            {
                throw new ArgumentException("Invalid datetime");
            }

            DateTime e = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(timePieces[0]),
                int.Parse(timePieces[1]), int.Parse(timePieces[2]));
            ClockDisplayMethod(e);

            FormatClock();

            DateDisplayMethod(e);

            StateHasChanged();
        }

        /// <summary>
        /// The second changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Sw_SecondChangedEvent(object? sender, DateTime e)
        {
            ClockDisplayMethod(e);

            FormatClock();

            DateDisplayMethod(e);

            StateHasChanged();
        }

        /// <summary>
        /// Method able to display properly the time according to the user's choices
        /// </summary>
        /// <param name="e">The e.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        private void ClockDisplayMethod(DateTime e)
        {
            string timeFormat;
            switch (ClockDisplay)
            {
                case WatchDisplayEnum.None:
                    timeFormat = Is24H ? "HH:mm" : "hh:mm tt";
                    systemWatch = e.ToString(timeFormat, Culture);
                    separator = ":";
                    break;
                case WatchDisplayEnum.WithSeconds:
                    timeFormat = Is24H ? "HH:mm ss" : "hh:mm ss tt";
                    systemWatch = e.ToString(timeFormat, Culture);
                    separator = ":";
                    break;
                case WatchDisplayEnum.WithBlinking:
                    timeFormat = Is24H ? "HH:mm" : "hh:mm";
                    systemWatch = e.ToString(timeFormat, Culture);
                    separatorActive = e.Second % 2 == 0;
                    break;
                case WatchDisplayEnum.WithSecondsAndBlinking:
                    timeFormat = Is24H ? "HH:mm ss" : "hh:mm ss tt";
                    systemWatch = e.ToString(timeFormat, Culture);
                    separatorActive = e.Second % 2 == 0;
                    break;
                case WatchDisplayEnum.Undefined:
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        
        /// <summary>
        /// Method able to display properly the date according to the user's choices
        /// </summary>
        /// <param name="e">The e.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        private void DateDisplayMethod(DateTime e)
        {
            switch (DateDisplay)
            {
                case DateDisplayEnum.None:
                    break;
                case DateDisplayEnum.WithShortDate:
                    systemDate = e.ToShortDateString();
                    break;
                case DateDisplayEnum.WithLongDate:
                    systemDate = e.ToLongDateString();
                    break;
                case DateDisplayEnum.Undefined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// Formats the clock.
        /// </summary>
        private void FormatClock()
        {
            var regex = new Regex(Regex.Escape(":"));
            systemWatch = regex.Replace(systemWatch, separator, 1);

            watchRightPart = systemWatch.Substring(systemWatch.IndexOf(":", StringComparison.Ordinal) + 1);
            watchLeftPart = systemWatch.Substring(0, systemWatch.IndexOf(":", StringComparison.CurrentCulture));
            blinkStyleSeparator = separatorActive ? "" : "clock-separator";
        }

        protected override void OnParametersSet()
        {
            StateHasChanged();
            base.OnParametersSet();
        }

        public ValueTask DisposeAsync()
        {
            ct.Cancel();
            watch.SecondChangedEvent -= Sw_SecondChangedEvent;
            //watch.SecondChangedEvent -= Watch_SecondChangedEvent;
            return ValueTask.CompletedTask;
        }
    }
}
