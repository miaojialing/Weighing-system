using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace AJWPFAdmin.Core
{
    public class NativeHost : HwndHost
    {
        const int WS_CHILD = 0x40000000;
        const int WS_VISIBLE = 0x10000000;
        const int LBS_NOTIFY = 0x001;

        new public IntPtr Handle
        {
            get { return (IntPtr)GetValue(HandleProperty); }
            set { SetValue(HandleProperty, value); }
        }

        public static readonly DependencyProperty HandleProperty
            = DependencyProperty.Register(nameof(Handle), typeof(IntPtr), typeof(NativeHost), new PropertyMetadata(IntPtr.Zero));

        private HwndSource _source;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            _source = new HwndSource(0, WS_CHILD | WS_VISIBLE | LBS_NOTIFY, 0, 0, 0, (int)Width, (int)Height, "naviveHost", hwndParent.Handle);
            Handle = _source.Handle;
            return new HandleRef(this,Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            _source.Dispose();
        }

    }
}
