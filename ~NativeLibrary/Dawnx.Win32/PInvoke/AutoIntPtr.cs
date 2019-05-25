﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Dawnx.Win32.PInvoke
{
    public sealed class AutoIntPtr : IDisposable
    {
        public int Length { get; private set; }
        public IntPtr Ptr { get; private set; }
        public bool HasUnmanagedMemoryBeenAllocated { get; private set; }
        private byte[] ManagedValue;

        public AutoIntPtr(int length)
        {
            Length = length;
        }

        public byte[] Value
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                if (HasUnmanagedMemoryBeenAllocated)
                {
                    ManagedValue = (byte[])Marshal.PtrToStructure(Ptr, typeof(byte[]));
                    Marshal.FreeHGlobal(Ptr);
                    HasUnmanagedMemoryBeenAllocated = false;
                    return ManagedValue;
                }
                else return ManagedValue;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static implicit operator IntPtr(AutoIntPtr @this)
        {
            if (!@this.HasUnmanagedMemoryBeenAllocated)
            {
                @this.Ptr = Marshal.AllocHGlobal(@this.Length);
                @this.HasUnmanagedMemoryBeenAllocated = true;
            }
            return @this.Ptr;
        }

        public void Dispose()
        {
            if (HasUnmanagedMemoryBeenAllocated) Marshal.FreeHGlobal(Ptr);
        }
    }

}
