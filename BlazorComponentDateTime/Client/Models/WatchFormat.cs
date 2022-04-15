using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorAppTest.Client.Models;

public enum WatchFormat
{
    None = 0x00,
    WithSeconds = 0x01,
    WithBlinking = 0x02,
    WithSecondsAndBlinking = 0x03,
    Undefined = 0xFF
}
