using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorAppTest.Client.Models
{
    public enum DateFormat
    {
        None = 0x00,
        WithShortDate = 0x01,
        WithLongDate = 0x02,
        Undefined = 0xFF
    }
}
