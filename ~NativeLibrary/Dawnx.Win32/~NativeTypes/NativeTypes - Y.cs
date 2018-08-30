﻿using System.Runtime.InteropServices;

namespace Dawnx.Win32
{
    /// Return Type: UINT->unsigned int
    ///mciId: MCIDEVICEID->UINT->unsigned int
    ///dwYieldData: DWORD->unsigned int
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate uint YIELDPROC(uint mciId, uint dwYieldData);

}
