using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlazorComponentDateTime.Client.Models;
using Microsoft.AspNetCore.Components;

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

        [Parameter]
        public bool Is24h { get; set; } = false;

        private string systemWatch = string.Empty;
        private string systemDate = string.Empty;
        private string separator = ":";
        private bool separatorActive = false;

        private string watchLeftPart = string.Empty;
        private string watchRightPart = string.Empty;

        private string blinkStyleSeparator = string.Empty;

        protected override void OnInitialized()
        {
            watch.SecondChangedEvent += Sw_SecondChangedEvent;
            base.OnInitialized();
        }

        /// <summary>
        /// The second changed event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Sw_SecondChangedEvent(object? sender, DateTime e)
        {
            string timeFormat = "";
            switch (ClockDisplay)
            {
                case WatchDisplayEnum.None:
                    timeFormat = Is24h ?  "HH:mm" : "hh:mm tt";
                    systemWatch = e.ToString(timeFormat, Culture);
                    separator = ":";
                    break;
                case WatchDisplayEnum.WithSeconds:
                    timeFormat = Is24h ? "HH:mm ss": "hh:mm ss tt";
                    systemWatch = e.ToString(timeFormat, Culture);
                    separator = ":";
                    break;
                case WatchDisplayEnum.WithBlinking:
                    timeFormat = Is24h ? "HH:mm": "hh:mm ss" ;
                    systemWatch = e.ToString(timeFormat, Culture);
                    separatorActive = e.Second % 2 == 0;
                    break;
                case WatchDisplayEnum.WithSecondsAndBlinking:
                    timeFormat = Is24h ? "HH:mm ss": "hh:mm ss tt";
                    systemWatch = e.ToString(timeFormat, Culture);
                    separatorActive = e.Second % 2 == 0;
                    break;
                case WatchDisplayEnum.Undefined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var regex = new Regex(Regex.Escape(":"));
            systemWatch = regex.Replace(systemWatch, separator, 1);

            watchRightPart = systemWatch.Substring(systemWatch.IndexOf(":", StringComparison.Ordinal) + 1);
            watchLeftPart = systemWatch.Replace(watchRightPart, "").Replace(":", "");
            blinkStyleSeparator = separatorActive ? "" : "clock-separator";


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

            StateHasChanged();
        }
        protected override void OnParametersSet()
        {
            StateHasChanged();
            base.OnParametersSet();
        }

        public ValueTask DisposeAsync()
        {
            watch.SecondChangedEvent -= Sw_SecondChangedEvent;
            return ValueTask.CompletedTask;
        }
    }
}
