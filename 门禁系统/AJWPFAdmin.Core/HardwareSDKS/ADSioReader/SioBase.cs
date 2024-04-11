using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.HardwareSDKS.ADSioReader
{
    [ComImport]
    [Guid("CB5BDC81-93C1-11CF-8F20-00805F2CD064")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IObjectSafety
    {
        [PreserveSig]
        int GetInterfaceSafetyOptions(ref Guid riid, [MarshalAs(UnmanagedType.U4)] ref int pdwSupportedOptions, [MarshalAs(UnmanagedType.U4)] ref int pdwEnabledOptions);

        [PreserveSig]
        int SetInterfaceSafetyOptions(ref Guid riid, [MarshalAs(UnmanagedType.U4)] int dwOptionSetMask, [MarshalAs(UnmanagedType.U4)] int dwEnabledOptions);
    }

    public abstract class ActiveXControl : IObjectSafety
    {
        private const string _IID_IDispatch = "{00020400-0000-0000-C000-000000000046}";

        private const string _IID_IDispatchEx = "{a6ef9860-c720-11d0-9337-00a0c90dcaa9}";

        private const string _IID_IPersistStorage = "{0000010A-0000-0000-C000-000000000046}";

        private const string _IID_IPersistStream = "{00000109-0000-0000-C000-000000000046}";

        private const string _IID_IPersistPropertyBag = "{37D84F60-42CB-11CE-8135-00AA004BB851}";

        private const int INTERFACESAFE_FOR_UNTRUSTED_CALLER = 1;

        private const int INTERFACESAFE_FOR_UNTRUSTED_DATA = 2;

        private const int S_OK = 0;

        private const int E_FAIL = -2147467259;

        private const int E_NOINTERFACE = -2147467262;

        private bool _fSafeForScripting = true;

        private bool _fSafeForInitializing = true;

        public int GetInterfaceSafetyOptions(ref Guid riid, ref int pdwSupportedOptions, ref int pdwEnabledOptions)
        {
            int num = -2147467259;
            string text = riid.ToString("B");
            pdwSupportedOptions = 3;
            switch (text)
            {
                case "{00020400-0000-0000-C000-000000000046}":
                case "{a6ef9860-c720-11d0-9337-00a0c90dcaa9}":
                    num = 0;
                    pdwEnabledOptions = 0;
                    if (_fSafeForScripting)
                    {
                        pdwEnabledOptions = 1;
                    }

                    break;
                case "{0000010A-0000-0000-C000-000000000046}":
                case "{00000109-0000-0000-C000-000000000046}":
                case "{37D84F60-42CB-11CE-8135-00AA004BB851}":
                    num = 0;
                    pdwEnabledOptions = 0;
                    if (_fSafeForInitializing)
                    {
                        pdwEnabledOptions = 2;
                    }

                    break;
                default:
                    num = -2147467262;
                    break;
            }

            return num;
        }

        public int SetInterfaceSafetyOptions(ref Guid riid, int dwOptionSetMask, int dwEnabledOptions)
        {
            int result = -2147467259;
            switch (riid.ToString("B"))
            {
                case "{00020400-0000-0000-C000-000000000046}":
                case "{a6ef9860-c720-11d0-9337-00a0c90dcaa9}":
                    if ((dwEnabledOptions & dwOptionSetMask) == 1 && _fSafeForScripting)
                    {
                        result = 0;
                    }

                    break;
                case "{0000010A-0000-0000-C000-000000000046}":
                case "{00000109-0000-0000-C000-000000000046}":
                case "{37D84F60-42CB-11CE-8135-00AA004BB851}":
                    if ((dwEnabledOptions & dwOptionSetMask) == 2 && _fSafeForInitializing)
                    {
                        result = 0;
                    }

                    break;
                default:
                    result = -2147467262;
                    break;
            }

            return result;
        }
    }


    public sealed class ReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; set; }

        public ReceivedEventArgs(byte[] value)
        {
            Data = value;
        }
    }

    public class StatusEventArgs : EventArgs
    {
        public object Obj { get; }

        public int Status { get; }

        public string Msg { get; }

        public StatusEventArgs(int status, string msg)
        {
            Status = status;
            Msg = msg;
        }

        public StatusEventArgs(int status, object obj)
        {
            Status = status;
            Obj = obj;
        }
    }

    public delegate void ReceivedEventHandler(object sender, ReceivedEventArgs e);
    public delegate void StatusEventHandler(object sender, StatusEventArgs e);

    public abstract class SioBase : ActiveXControl, IDisposable
    {
        public bool bConnected = false;

        public int USBVerNum = 113;

        private bool disposedValue = false;

        public event ReceivedEventHandler onReceived;

        public event StatusEventHandler onStatus;

        public abstract bool Connect(string name, int port);

        public abstract bool Send(byte[] byData);

        public abstract bool Send(string strData);

        public abstract void DisConnect();

        protected virtual void RaiseEventStatus(StatusEventArgs e)
        {
            this.onStatus?.Invoke(this, e);
        }

        protected virtual void RaiseEventReceived(byte[] iData)
        {
            this.onReceived?.Invoke(this, new ReceivedEventArgs(iData));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
        }
    }
}
