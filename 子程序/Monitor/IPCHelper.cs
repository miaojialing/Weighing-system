using System;
using System.Runtime.InteropServices;

namespace Monitor
{
    public static class IPCHelper
    {
        public static readonly int CLOSE_APP = 0x0801;
        public static readonly int MONITOR_FIRST_WEIGHING = 0x0402;
        public static readonly int MONITOR_SECOND_WEIGHING = 0x0403;
        public static readonly int MONITOR_ONCE_WEIGHING = 0x0404;
        public static readonly int WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
    }
}
