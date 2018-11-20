﻿using System;
using System.Threading;

namespace Dawnx.Patterns
{
    public static class UseSpinLock
    {
        /// <summary>
        /// Do a task with SpinLock pattern:
        ///     do { @task; } until(@until)
        /// </summary>
        /// <param name="until"></param>
        /// <param name="task"></param>
        public static void Do(Action task, Func<bool> until)
        {
            do { task(); }
            while (!until());
        }

        /// <summary>
        /// Do a task with SpinLock pattern:
        ///     do { @task; sleep(@frequency); } until(@until)
        /// </summary>
        /// <param name="until"></param>
        /// <param name="task"></param>
        public static void Do(Action task, Func<bool> until, TimeSpan frequency)
        {
            do
            {
                task();
                Thread.Sleep(frequency);
            }
            while (!until());
        }

    }
}
