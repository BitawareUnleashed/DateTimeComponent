using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BlazorAppTest.Client.Models;
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
        [Parameter]
        public WatchFormat WatchFormat { get; set; } = WatchFormat.None;

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        /// <value>
        /// The date format.
        /// </value>
        [Parameter]
        public DateFormat DateFormat { get; set; } = DateFormat.None;

        private string systemWatch = string.Empty;
        private string systemDate = string.Empty;
        private string separator = string.Empty;

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
            var secondsAdded = string.Empty;


            switch (WatchFormat)
            {
                case WatchFormat.None:
                    systemWatch = e.ToShortTimeString();
                    separator = ":";
                    break;
                case WatchFormat.WithSeconds:
                    systemWatch = e.ToLongTimeString();
                    separator = ":";
                    break;
                case WatchFormat.WithBlinking:
                    systemWatch = e.ToShortTimeString();
                    separator = e.Second % 2 == 0 ? "." : ":";
                    watchRightPart = systemWatch.Substring(systemWatch.IndexOf(":", StringComparison.Ordinal)).Replace(":", "");
                    watchLeftPart = systemWatch.Replace(watchRightPart, "").Replace(":", "");
                    break;
                case WatchFormat.WithSecondsAndBlinking:
                    systemWatch = e.ToLongTimeString();
                    separator = e.Second % 2 == 0 ? "." : ":";
                    watchRightPart = systemWatch.Substring(systemWatch.IndexOf(":", StringComparison.Ordinal)+1);
                    watchLeftPart = systemWatch.Replace(watchRightPart, "").Replace(":", "");
                    break;
                case WatchFormat.Undefined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            blinkStyleSeparator = separator == ":" ? "" : "clock-separator";

            var regex = new Regex(Regex.Escape(":"));
            systemWatch = regex.Replace(systemWatch, separator, 1);


            switch (DateFormat)
            {
                case DateFormat.None:
                    break;
                case DateFormat.WithShortDate:
                    systemDate = e.ToShortDateString();
                    break;
                case DateFormat.WithLongDate:
                    systemDate = e.ToLongDateString();
                    break;
                case DateFormat.Undefined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            StateHasChanged();
        }

        public ValueTask DisposeAsync()
        {
            watch.SecondChangedEvent -= Sw_SecondChangedEvent;
            return ValueTask.CompletedTask;
        }
    }
}
