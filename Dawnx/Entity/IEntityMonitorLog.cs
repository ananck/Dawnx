﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dawnx.Entity
{
    public interface IEntityMonitorLog
    {
        DateTime MonitorTime { get; set; }
        string MonitorEvent { get; set; }
        string ModelClassName { get; set; }
        string ModelKeys { get; set; }
        string ChangeValues { get; set; }
    }

}