using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FetcherConsole
{
    public class TaskInfo
    {
        public TimeSpan StartTime { get; set; } = TimeSpan.FromSeconds(1);
        public TimeSpan Interval { get; set; }

    }
}
