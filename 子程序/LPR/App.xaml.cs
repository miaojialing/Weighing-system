using ADRcpLib;
using ADSioLib;
using Common;
using Common.Model;
using Common.Platform;
using Common.Platform.Citys.Reef;
using Common.Platform.Citys.Shandong_Boxing;
using Common.Platform.Citys.WuXiWeightAPI;
using Common.Utility;
using Common.Utility.AJ;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


//using ReaderB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using static Common.Platform.Citys.WuXiWeightAPI.WuXiWeightAPI;
//using WG3000_COMM.Common;
//using WG3000_COMM.Core;
using static LPR.IPCHelper;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace LPR
{

    public class ShareMem
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(int hFile, IntPtr lpAttributes, uint flProtect, uint dwMaxSizeHi, uint dwMaxSizeLow, string lpName);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr OpenFileMapping(int dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMapping, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool UnmapViewOfFile(IntPtr pvBaseAddress);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32", EntryPoint = "GetLastError")]
        public static extern int GetLastError();
        const int ERROR_ALREADY_EXISTS = 183;
        const int FILE_MAP_COPY = 0x0001;
        const int FILE_MAP_WRITE = 0x0002;
        const int FILE_MAP_READ = 0x0004;
        const int FILE_MAP_ALL_ACCESS = 0x0002 | 0x0004;
        const int PAGE_READONLY = 0x02;
        const int PAGE_READWRITE = 0x04;
        const int PAGE_WRITECOPY = 0x08;
        const int PAGE_EXECUTE = 0x10;
        const int PAGE_EXECUTE_READ = 0x20;
        const int PAGE_EXECUTE_READWRITE = 0x40;
        const int SEC_COMMIT = 0x8000000;
        const int SEC_IMAGE = 0x1000000;
        const int SEC_NOCACHE = 0x10000000;
        const int SEC_RESERVE = 0x4000000;
        const int INVALID_HANDLE_VALUE = -1;
        IntPtr m_hSharedMemoryFile = IntPtr.Zero;
        IntPtr m_pwData = IntPtr.Zero;
        bool m_bAlreadyExist = false;
        bool m_bInit = false;
        long m_MemSize = 0;
        public struct MAIN_LPR_OVER_WEIGHT_DATA
        {
            public string mz;
            public string overWeightCount;
            public string axleNum;
        }

        public struct MAIN_LPR_MESS
        {
            public string mess1;
            public string mess2;
            public string mess3;
            public string mess4;
            public string mess5;
            public string mess6;
        }
        //[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]

        public byte[] StructToBytes<T>(T obj)
        {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr bufferPtr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(obj, bufferPtr, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(bufferPtr, bytes, 0, size);
                return bytes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in StructToBytes ! " + ex.Message);
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPtr);
            }
        }
        public object BytesToStuct(byte[] bytes)
        {
            int size = Marshal.SizeOf(typeof(MAIN_LPR_OVER_WEIGHT_DATA));
            if (size > bytes.Length)
            {
                return null;
            }
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, structPtr, size);
            object obj = Marshal.PtrToStructure(structPtr, typeof(MAIN_LPR_OVER_WEIGHT_DATA));
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }
        public ShareMem()
        {
        }
        ~ShareMem()
        {
            Close();
        }
        public int Init(string strName, long lngSize)
        {
            if (lngSize <= 0 || lngSize > 104857600) lngSize = 104857600;
            //0x00800000
            m_MemSize = lngSize;
            if (strName.Length > 0)
            {
                //创建内存共享体(INVALID_HANDLE_VALUE)
                m_hSharedMemoryFile = CreateFileMapping(INVALID_HANDLE_VALUE, IntPtr.Zero, (uint)PAGE_READWRITE, 0, (uint)lngSize, strName);
                if (m_hSharedMemoryFile == IntPtr.Zero)
                {
                    m_bAlreadyExist = false;
                    m_bInit = false;
                    return 2;
                    //创建共享体失败
                }
                else
                {
                    if (GetLastError() == ERROR_ALREADY_EXISTS)  //已经创建 
                    {
                        m_bAlreadyExist = true;
                    }
                    else                                         //新创建 
                    {
                        m_bAlreadyExist = false;
                    }
                }
                //---------------------------------------
                //创建内存映射
                m_pwData = MapViewOfFile(m_hSharedMemoryFile, FILE_MAP_WRITE, 0, 0, (uint)lngSize);
                if (m_pwData == IntPtr.Zero)
                {
                    m_bInit = false;
                    CloseHandle(m_hSharedMemoryFile);
                    return 3;
                    //创建内存映射失败
                }
                else
                {
                    m_bInit = true;
                    if (m_bAlreadyExist == false)
                    {
                        //初始化
                    }
                }
                //----------------------------------------
            }
            else
            {
                return 1;
                //参数错误
            }
            return 0;
            //创建成功
        }
        /// <summary>
        /// 关闭共享内存
        /// </summary>
        public void Close()
        {
            if (m_bInit)
            {
                UnmapViewOfFile(m_pwData);
                CloseHandle(m_hSharedMemoryFile);
            }
        }
        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="bytData">数据</param>
        /// <param name="lngAddr">起始地址</param>
        /// <param name="lngSize">个数</param>
        /// <returns></returns>
        public int Read(ref byte[] bytData, int lngAddr, int lngSize)
        {
            if (lngAddr + lngSize > m_MemSize) return 2;
            //超出数据区
            if (m_bInit)
            {
                Marshal.Copy(m_pwData, bytData, lngAddr, lngSize);
            }
            else
            {
                return 1;
                //共享内存未初始化
            }
            return 0;
            //读成功
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="bytData">数据</param>
        /// <param name="lngAddr">起始地址</param>
        /// <param name="lngSize">个数</param>
        /// <returns></returns>
        public int Write(byte[] bytData, int lngAddr, int lngSize)
        {
            if (lngAddr + lngSize > m_MemSize) return 2;
            //超出数据区
            if (m_bInit)
            {
                Marshal.Copy(bytData, lngAddr, m_pwData, lngSize);
            }
            else
            {
                return 1;
                //共享内存未初始化
            }
            return 0;
            //写成功
        }
    }


    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger("车牌识别");
        private static readonly int REFRESH_DI_STATE = 0x0800;
        private static readonly int CLOSE_APP = 0x0801;

        //当识别到车牌并成功发送给主程序后，主程序发送消息正在称重，称重完成后发送消息继续车牌识别
        private static readonly int READY_TO_WEIGH = 0x0804;
        private static readonly int GET_PLATE = 0x0806;
        private static readonly int WEIGHING_COMPLETE = 0x0807;
        private static readonly int IS_WEIGHING = 0x0808;
        private static readonly int IS_STABLE = 0x080A;
        public static readonly int VIOLATE_WHITE_LIST = 0x080B;
        public static readonly int OVER_WEIGHT = 0x080C;


        //继电器控制命令0x08F0~0x08FF,最后一位的二进制0000表示全关,1111表示全开
        private static readonly int RELAY_MIN = 0x08F0;
        private static readonly int RELAY_MAX = 0x08FF;

        //单独红绿灯控制参数
        private static readonly int GREENON = 0x0F0A;
        private static readonly int REDON = 0x0F0B;

        //更新Vz设备 --阿吉 2023年12月1日11点51分
        private static readonly int UPDATEDEVICE = 100;

        private VzClientSDK.VZLPRC_PLATE_INFO_CALLBACK m_PlateResultCB1 = null;
        private VzClientSDK.VZLPRC_PLATE_INFO_CALLBACK m_PlateResultCB2 = null;

        private string m_LprMode;   //车牌识别模式 1：远程射频卡 2：车牌识别相机
        private string m_PlateNo = string.Empty;
        private string m_DevNo = string.Empty;

        //车牌识别相机句柄
        int handle1 = 0;
        int handle2 = 0;
        int serial_handle1 = 0;
        int serial_handle2 = 0;
        int iSignoState = 0;
        byte stat1 = 0;
        byte stat2 = 0;

        public bool InWeighing { get; private set; }

        //控制继电器的命令
        private byte[] m_DICmd = new byte[8] { 0x01, 0x02, 0x00, 0x00, 0x00, 0x04, 0x79, 0xC9 };
        private byte[] m_DOCmd = new byte[10] { 0x01, 0x0F, 0x00, 0x00, 0x00, 0x04, 0x01, 0x00, 0x00, 0x00 };
        private byte[] m_OpenRelay = new byte[9] { 0x7C, 0xFF, 0xFF, 0xD7, 0x00, 0x02, 0x01, 0x01, 0xAB };
        private byte[] m_CloseRelay = new byte[9] { 0x7C, 0xFF, 0xFF, 0xD7, 0x00, 0x02, 0x01, 0x00, 0xAC };
        private string m_DIORet = string.Empty;   //继电器接收到命令的返回值
        private int m_TimerTickCount = 0;   //持续给继电器发送命令的次数，大于10则超时
        private SerialPort m_RelayPort = new SerialPort();  //继电器串口
        private DispatcherTimer m_DOTimer;   //持续给继电器发送命令的定时器
        private DispatcherTimer m_DITimer;   //持续给继电器发送命令的定时器
        private string signoState;
        public string SignoState
        {
            get { return signoState; }
            set
            {
                if (value != signoState)
                {
                    SendMessageToApp("称重软件3.0", "Signo," + value);
                    //log.Info("发送DI状态给衡七管家：" + value);
                }
                signoState = value;
            }
        }

        private string diState;
        public string DIState
        {
            get { return diState; }
            set
            {
                if (value != diState)
                {
                    SendMessageToApp("称重软件3.0", "DIState," + value);
                }
                diState = value;
            }
        }
        /// <summary>
        /// LPR图片保存位置
        /// </summary>
        private string LPRSavePath
        {
            get
            {
                return ConfigurationManager.AppSettings["LPRSavePath"] + "\\";
            }
        }

        //新的网口继电器
        byte[] buffer = new byte[64 * 4];
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Thread tcpThread;
        byte[] checkRelay = new byte[] { 0x6A, 0xA6, 0x03, 0xFE, 0x06, 0x07 };
        byte[] checkGPIO = new byte[] { 0x6A, 0xA6, 0x03, 0xFE, 0x07, 0x08 };
        byte[] controlRelay = new byte[] { 0x6A, 0xA6, 0x05, 0x01, 0x00, 0x00, 0x00, 0x00 };
        private DispatcherTimer delay2timer = new DispatcherTimer(); //定义定时器，进行周期的查询继电器和数字量状态
        private bool ioSwitch = true;
        //private bool sendCMDToRealy2Flag = false;

        private byte realyStatus;
        public byte RealyStatus
        {
            get { return realyStatus; }
            set
            {
                if (value != realyStatus)
                {
                    //ShowRelayStatus(value);

                    log.Info("DO收到继电器反馈：" + Convert.ToString(Convert.ToInt32(value) / 4, 2).PadLeft(4, '0'));
                }
                realyStatus = value;
            }
        }

        private byte gpioStatus;
        public byte GPIOStatus
        {
            get { return gpioStatus; }
            set
            {
                if (value != gpioStatus)
                {
                    int i = (Convert.ToInt32(value) - 3) / 4;
                    SendMessageToApp("称重软件3.0", "Signo," + i.ToString());
                    //log.Info("发送DI状态给衡七管家：" + i);
                }
                gpioStatus = value;
            }
        }


        private byte gpio2Status;
        public byte GPIO2Status
        {
            get { return gpio2Status; }
            set
            {
                if (value != gpio2Status)
                {
                    SendMessageToApp("称重软件3.0", "Gpio," + value);

                }
                gpio2Status = value;
            }
        }


        //远程读卡器串口 旧版
        private bool m_IsActiveScan;
        private string m_RF1PortName;
        private string m_RF2PortName;
        private int m_RF1OpenRet = -1;
        private int m_RF1ComIndex;
        private byte m_RF1ComAdr = 0xFF;
        private int m_RF2OpenRet = -1;
        private int m_RF2ComIndex;
        private byte m_RF2ComAdr = 0xFF;
        private DispatcherTimer m_RFTimer = new DispatcherTimer();
        //远程读卡器串口 新版
        private SerialPort m_RF1Port = new SerialPort();
        private SerialPort m_RF2Port = new SerialPort();
        private string m_EpcStrAppend = string.Empty;

        public static SioBase SioBase = new SioNet();
        public static RcpBase RcpBase = new RcpBase();
        public static SioBase SioBase2 = new SioNet();
        public static RcpBase RcpBase2 = new RcpBase();
        public bool IsConnectedSio
        {
            get
            {
                if (SioBase == null) return false;
                if (!SioBase.bConnected) return false;
                return true;
            }
        }
        public bool IsConnectedSio2
        {
            get
            {
                if (SioBase2 == null) return false;
                if (!SioBase2.bConnected) return false;
                return true;
            }
        }
        //public string camera1GPIO = string.Empty;
        //public string Camera1GPIO
        //{

        //    get
        //    {
        //        if (string.IsNullOrEmpty(camera1GPIO))
        //        {
        //            camera1GPIO = ConfigurationManager.AppSettings["Camera1GPIO"];
        //        }
        //        return camera1GPIO;
        //    }
        //}
        private AppSettingsSection _mainSetting;
        private bool _isVZSDKSetup;

        private int[] _camer1GPIOStatus = new int[] { 1, 1 };
        private int[] _camer2GPIOStatus = new int[] { 1, 1 };
        private static readonly int[] _refTempGPIOStatus = new int[1];

        private static readonly GCHandle _tempGPIOHandleObject = GCHandle.Alloc(_refTempGPIOStatus, GCHandleType.Pinned);
        private static readonly IntPtr _tempGPIOHandlePtr = _tempGPIOHandleObject.AddrOfPinnedObject();
        private VzClientSDK.VZLPRC_FIND_DEVICE_CALLBACK_EX _vzFindDeviceCallback;

        protected override void OnExit(ExitEventArgs e)
        {

            try
            {

                if (_tempGPIOHandleObject.IsAllocated)
                    _tempGPIOHandleObject.Free();

                if (handle1 != 0)
                {
                    VzClientSDK.VzLPRClient_Close(handle1);
                }
                if (handle2 != 0)
                {
                    VzClientSDK.VzLPRClient_Close(handle2);
                }

                VzClientSDK.VZLPRClient_StopFindDevice();

                VzClientSDK.VzLPRClient_Cleanup();

                PlatformManager.Instance.Current.Dispose();
            }
            catch
            {
            }

            base.OnExit(e);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _mainSetting = SettingsHelper.AWSV2Settings;

            var platformName = _mainSetting.Settings["PlatformName"]?.Value ?? string.Empty;
            Common.Platform.PlatformManager.LOGGER = log;
            PlatformManager.Instance.Init(platformName);

            //根据启动时传递的参数，决定初始化远程刷卡器或者车牌识别相机
            if (e.Args.Count() != 0) m_LprMode = e.Args[0];

            if (m_LprMode == "1")
            {
                log.Info("车牌识别方式：远程刷卡器");
                //旧版                
                //OpenRFPort(); //打开读卡器串口
                ////打开定时器轮询
                //m_RFTimer.Interval = TimeSpan.FromMilliseconds(500);
                //m_RFTimer.Tick += (sender, args) => RFTimer_Tick();
                //m_RFTimer.Start();

                //第二版
                OpenRFPort();

                SioBase.onStatus += SioBase_onStatus;
                SioBase.onReceived += SioBase_onReceived;
                RcpBase.RxRspParsed += RcpBase_RxRspParsed;
                if (ConfigurationManager.AppSettings["RF3Enable"] == "1")
                {
                    SioBase.Connect(ConfigurationManager.AppSettings["RF3IP"], 49152);
                }
                SioBase2.onStatus += SioBase2_onStatus;
                SioBase2.onReceived += SioBase2_onReceived;
                RcpBase2.RxRspParsed += RcpBase2_RxRspParsed;
                if (ConfigurationManager.AppSettings["RF4Enable"] == "1")
                {
                    SioBase2.Connect(ConfigurationManager.AppSettings["RF4IP"], 49152);
                }
            }


            if (m_LprMode == "2")
            {
                log.Info("车牌识别方式：车牌识别相机");
                VzClientSDK.VzLPRClient_Setup();
                _isVZSDKSetup = true;
                Login();

                if (PlatformManager.Instance.Current is WuXiWeightAPI wuxiApi)
                {
                    wuxiApi.StartTimer();
                }

                FindVzClientDevices();

                var connStatTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
                connStatTimer.Start();
                connStatTimer.Tick += (sender, args) =>
                {
                    if (ConfigurationManager.AppSettings["Camera1Enable"] == "1" && handle1 != 0) VzClientSDK.VzLPRClient_IsConnected(handle1, ref stat1);
                    if (ConfigurationManager.AppSettings["Camera2Enable"] == "1" && handle2 != 0) VzClientSDK.VzLPRClient_IsConnected(handle2, ref stat2);

                    if (stat1 == 0 && ConfigurationManager.AppSettings["Camera1Enable"] == "1")
                    {
                        log.Info("检测到车牌相机1掉线");
                        if (PlatformManager.Instance.Current is WuXiWeightAPI)
                        {
                            WuXiWeightAPI.Device.status = WuXiDeviceStatus.离线;
                        }
                        Login();
                    }
                    if (stat2 == 0 && ConfigurationManager.AppSettings["Camera2Enable"] == "1")
                    {
                        log.Info("检测到车牌相机2掉线");
                        if (PlatformManager.Instance.Current is WuXiWeightAPI)
                        {
                            WuXiWeightAPI.Device.status = WuXiDeviceStatus.离线;
                        }
                        
                        Login();
                    }
                };
                OpenRFPort();
                SioBase.onStatus += SioBase_onStatus;
                SioBase.onReceived += SioBase_onReceived;
                RcpBase.RxRspParsed += RcpBase_RxRspParsed;
                if (ConfigurationManager.AppSettings["RF3Enable"] == "1")
                {
                    SioBase.Connect(ConfigurationManager.AppSettings["RF3IP"], 49152);
                }
                SioBase2.onStatus += SioBase2_onStatus;
                SioBase2.onReceived += SioBase2_onReceived;
                RcpBase2.RxRspParsed += RcpBase2_RxRspParsed;
                if (ConfigurationManager.AppSettings["RF4Enable"] == "1")
                {
                    SioBase2.Connect(ConfigurationManager.AppSettings["RF4IP"], 49152);
                }
            }

            //若有接入继电器，打开继电器串口，初始化继电器定时器
            if (ConfigurationManager.AppSettings["RelayEnable"] == "1")
            {
                m_RelayPort.PortName = ConfigurationManager.AppSettings["RelayUart"];
                m_RelayPort.BaudRate = 9600;
                var openResult = false;
                try
                {
                    m_RelayPort.Open();

                    if (m_RelayPort.IsOpen)
                    {
                        Thread recvRelayPortDataThread = new Thread(new ThreadStart(RecvRelayPortData)) { IsBackground = true };
                        recvRelayPortDataThread.Start();

                        SendMsgToApp("车牌识别", 0x08F0);
                        log.Info("打开继电器串口，并初始化继电器：" + m_RelayPort.PortName);

                        openResult = true;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }

                var jsonObj = JObject.FromObject(new
                {
                    Key = Common.Model.DeviceNoticeIcon.DeviceKey.控制器,
                    Online = openResult
                }).ToString(Formatting.None);
                SendMessageToApp("称重软件3.0", $"AJDeviceNotice,{jsonObj.Replace(",", "|")}");

                m_DOTimer = new DispatcherTimer();
                m_DOTimer.Interval = TimeSpan.FromMilliseconds(100);
                m_DOTimer.Tick += (sender, args) => RelayTimer_Tick(m_DOCmd);

                m_DITimer = new DispatcherTimer();
                m_DITimer.Interval = TimeSpan.FromMilliseconds(500);
                m_DITimer.Tick += (sender, args) => SignoStateTimer_Tick();
                m_DITimer.Start();
            }

            // 重构代码, 使用主程序的 CheckGrating 判断是否读取GPIO状态 --阿吉 2023年10月11日08点45分
            if ((_mainSetting.Settings["CheckGrating"]?.Value ?? "0").Equals("1"))
            {
                if (!_isVZSDKSetup)
                {
                    VzClientSDK.VzLPRClient_Setup();
                    _isVZSDKSetup = true;
                }
                Login();
                m_DITimer = new DispatcherTimer();
                m_DITimer.Interval = TimeSpan.FromMilliseconds(500);
                m_DITimer.Tick += (sender, args) => SignoStateTimer_Tick();
                m_DITimer.Start();
            }

            if (ConfigurationManager.AppSettings["Relay2Enable"] == "1")
            {
                var openResult = false;
                try
                {
                    if (!socket.Connected)
                        socket.Connect(new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["Relay2IP"]), 6000));

                    if (socket.Connected)
                    {
                        tcpThread = new Thread(TCPReceived);
                        tcpThread.IsBackground = true;
                        tcpThread.Start(socket);
                        log.Info("控制器连接成功，断开全部开关。");

                        openResult = true;

                        delay2timer.Tick += new EventHandler(Delay2_Timer_Tick);
                        delay2timer.Interval = TimeSpan.FromSeconds(1);
                        //delay2timer.Interval = TimeSpan.FromSeconds(2.00);
                        delay2timer.Start();
                        log.Info("打开DI DO查询定时器");

                        SendMsgToApp("车牌识别", 0x08F0);
                    }
                }
                catch (Exception ex)
                {
                    log.Info("控制器连接失败:" + ex.Message);
                }

                var jsonObj = JObject.FromObject(new 
                {
                    Key = Common.Model.DeviceNoticeIcon.DeviceKey.控制器,
                    Online = openResult
                }).ToString(Formatting.None);
                SendMessageToApp("称重软件3.0", $"AJDeviceNotice,{jsonObj.Replace(",", "|")}");
            }

            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
        }

        #region 串口继电器
        //继电器定时器，发送命令直到收到反馈或超时
        private void RelayTimer_Tick(byte[] relayCmd)
        {
            if (m_RelayPort.IsOpen)
            {
                if (m_DIORet == "010F000000045408")
                {
                    m_DOTimer.Stop();
                    m_DIORet = string.Empty;
                    m_TimerTickCount = 0;
                    log.Info("DO收到继电器反馈");
                    return;
                }

                m_RelayPort.Write(relayCmd, 0, relayCmd.Length);

                log.Info("发送命令给继电器：" + Convert.ToString(relayCmd[7], 2).PadLeft(4, '0'));
                //log.Info("发送命令给继电器：" + ByteArrayToHexString(relayCmd));
            }
            else
            {
                log.Warn("继电器串口未打开");
            }

            if (m_TimerTickCount++ > 10)
            {
                m_TimerTickCount = 0;
                m_DOTimer.Stop();
                log.Warn("继电器超时未响应");
            }
        }
        //道闸状态定时器，一直查询道闸状态，并发消息给主APP
        private void SignoStateTimer_Tick()
        {
            if (m_RelayPort.IsOpen)
            {
                if (m_DIORet.StartsWith("010201"))
                {
                    SignoState = m_DIORet.Substring(7, 1);
                }

                m_RelayPort.Write(m_DICmd, 0, m_DICmd.Length);
            }




            if (handle1 != 0)
            {
                VzClientSDK.VzLPRClient_GetGPIOValue(handle1, 0, _tempGPIOHandlePtr);

                // 赋值第一个通道的开关值
                _camer1GPIOStatus[0] = _refTempGPIOStatus[0];

                VzClientSDK.VzLPRClient_GetGPIOValue(handle1, 1, _tempGPIOHandlePtr);

                // 赋值第二个通道的开关值;
                _camer1GPIOStatus[1] = _refTempGPIOStatus[0];


                //if (test[0] == 0)
                //{
                //    //gpio2Status = 0x30;
                //    SendMessageToApp("称重软件3.0", "Gpio," + "0");
                //}

                //if (test[0] == 1) i += 1;

                //VzClientSDK.VzLPRClient_GetGPIOValue(handle1, 1, pObject);

                //if (test[0] == 1) i += 2;

                //if (hObject.IsAllocated)
                //    hObject.Free();

                //SignoState = i.ToString();
            }

            if (handle2 != 0)
            {
                VzClientSDK.VzLPRClient_GetGPIOValue(handle2, 0, _tempGPIOHandlePtr);

                // 赋值第一个通道的开关值
                _camer2GPIOStatus[0] = _refTempGPIOStatus[0];

                VzClientSDK.VzLPRClient_GetGPIOValue(handle2, 1, _tempGPIOHandlePtr);

                // 赋值第二个通道的开关值;
                _camer2GPIOStatus[1] = _refTempGPIOStatus[0];
            }

            // 直接发送,--阿吉 2023年10月10日08点56分
            SendMessageToApp("称重软件3.0", $"AJGpio,{_camer1GPIOStatus[0]}_{_camer1GPIOStatus[1]}|{_camer2GPIOStatus[0]}_{_camer2GPIOStatus[1]}");
        }
        //继电器串口接收数据线程
        private void RecvRelayPortData()
        {
            m_RelayPort.DataReceived += (sender, e) =>
            {
                try
                {
                    byte[] RecvDatas = new byte[m_RelayPort.BytesToRead];
                    m_RelayPort.Read(RecvDatas, 0, RecvDatas.Length);
                    m_DIORet = ByteArrayToHexString(RecvDatas);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            };
        }
        #endregion

        #region 车牌识别相机

        /// <summary>
        /// 查找设备
        /// </summary>
        private void FindVzClientDevices()
        {
            if (_vzFindDeviceCallback == null)
            {
                _vzFindDeviceCallback = new VzClientSDK.VZLPRC_FIND_DEVICE_CALLBACK_EX((string pStrDevName, string pStrIPAddr, ushort usPort1, ushort usPort2, uint SL, uint SH, string netmask, string gateway, IntPtr pUserData) =>
                {
                    // 字段对应模型 Common\Utility\AJ\MobileConfiguration\DeviceInfo.cs --阿吉 2023年11月30日14点16分
                    // 但是要处理逗号， 那边主程序接受是按逗号分隔的， 所以这里的json字符串不能有逗号， 用 | 代替了逗号
                    var jsonObj = JsonConvert.SerializeObject(new
                    {
                        label = pStrDevName,
                        ip = pStrIPAddr,
                        prot = usPort1,
                        extraData = new
                        {
                            SL,
                            SH,
                            netmask,
                            gateway
                        }
                    });

                    SendMessageToApp("称重软件3.0", $"AJVZDevice,{jsonObj.Replace(",", "|")}");
                });
            }
            VzClientSDK.VZLPRClient_StartFindDeviceEx(_vzFindDeviceCallback, IntPtr.Zero);
        }

        private int[] Login()
        {

            var openResult = false;

            if (ConfigurationManager.AppSettings["Camera1Enable"] == "1" && handle1 == 0)
            {
                var uname = ConfigurationManager.AppSettings["Camera1Username"];
                var pwd = ConfigurationManager.AppSettings["Camera1Password"];
                handle1 = VzClientSDK.VzLPRClient_Open(ConfigurationManager.AppSettings["Camera1IP"], 80, uname, pwd);
                if (handle1 != 0)
                {
                    if (PlatformManager.Instance.Current is WuXiWeightAPI)
                    {
                        WuXiWeightAPI.Device.status = WuXiDeviceStatus.在线;
                    }
                    log.Info("Camera1 Login Success");
                    openResult = true;
                    

                    stat1 = 1;
                    //获取车牌信息
                    m_PlateResultCB1 = new VzClientSDK.VZLPRC_PLATE_INFO_CALLBACK(OnPlateResult);
                    VzClientSDK.VzLPRClient_SetPlateInfoCallBack(handle1, m_PlateResultCB1, IntPtr.Zero, 1);

                    //相机扩展显示屏
                    if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1")
                    {
                        //打开485串口
                        VzClientSDK.VZDEV_SERIAL_RECV_DATA_CALLBACK serial_recv_ = null;
                        serial_handle1 = VzClientSDK.VzLPRClient_SerialStart(handle1, 0, serial_recv_, IntPtr.Zero);
                        if (serial_handle1 == 0)
                        {
                            log.Info("相机1的485串口1打开失败!");
                        }
                        //发送初始化数据
                        //SendMsgToCameraLED(serial_handle1, "车牌识别", "自动称重", DateTime.Now.ToString(), "衡七管家", "", "g");
                        SendMsgToCameraLED(serial_handle1, "称重软件3.0", "全程抓拍", "自动称重", "缓慢上磅", "", "g");
                    }
                }
                else
                {
                    log.Info("Camera1 Login Failed");
                }
            }

            if (ConfigurationManager.AppSettings["Camera2Enable"] == "1" && handle2 == 0)
            {
                var uname = ConfigurationManager.AppSettings["Camera2Username"];
                var pwd = ConfigurationManager.AppSettings["Camera2Password"];
                handle2 = VzClientSDK.VzLPRClient_Open(ConfigurationManager.AppSettings["Camera2IP"], 80, uname, pwd);
                if (handle2 != 0)
                {
                    if (PlatformManager.Instance.Current is WuXiWeightAPI)
                    {
                        WuXiWeightAPI.Device.status = WuXiDeviceStatus.在线;
                    }
                    log.Info("Camera2 Login Success");

                    openResult = true;

                    stat2 = 1;
                    //获取车牌信息 
                    m_PlateResultCB2 = new VzClientSDK.VZLPRC_PLATE_INFO_CALLBACK(OnPlateResult);
                    VzClientSDK.VzLPRClient_SetPlateInfoCallBack(handle2, m_PlateResultCB2, IntPtr.Zero, 1);

                    //相机扩展显示屏
                    if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1")
                    {
                        //打开485串口
                        VzClientSDK.VZDEV_SERIAL_RECV_DATA_CALLBACK serial_recv_ = null;
                        serial_handle2 = VzClientSDK.VzLPRClient_SerialStart(handle2, 0, serial_recv_, IntPtr.Zero);
                        if (serial_handle2 == 0)
                        {
                            log.Info("相机2的485串口1打开失败!");
                        }
                        //发送初始化数据
                        //SendMsgToCameraLED(serial_handle2, "车牌识别", "自动称重", DateTime.Now.ToString(), "衡七管家", "", "g");
                        SendMsgToCameraLED(serial_handle2, "称重软件3.0", "全程抓拍", "自动称重", "缓慢上磅", "", "g");
                    }
                }
                else
                {
                    log.Info("Camera2 Login Failed");
                }
            }

            var jsonObj = JObject.FromObject(new
            {
                Key = Common.Model.DeviceNoticeIcon.DeviceKey.车牌识别,
                Online = openResult
            }).ToString(Formatting.None);
            SendMessageToApp("称重软件3.0", $"AJDeviceNotice,{jsonObj.Replace(",", "|")}");

            return new int[] { handle1, handle2, serial_handle1, serial_handle2 };
        }


        private int OnPlateResult(int handle, IntPtr pUserData, IntPtr pResult, uint uNumPlates,
            VzClientSDK.VZ_LPRC_RESULT_TYPE eResultType, IntPtr pImgFull, IntPtr pImgPlateClip)
        {
            try
            {
                // log.Info($"eResultType:{eResultType} VZ_LPRC_RESULT_REALTIME:{VzClientSDK.VZ_LPRC_RESULT_TYPE.VZ_LPRC_RESULT_REALTIME}");
                if (eResultType != VzClientSDK.VZ_LPRC_RESULT_TYPE.VZ_LPRC_RESULT_REALTIME)
                {
                    VzClientSDK.TH_PlateResult result = (VzClientSDK.TH_PlateResult)Marshal.PtrToStructure(pResult, typeof(VzClientSDK.TH_PlateResult));
                    string strLicense = new string(result.license);
                    strLicense = strLicense.Replace("\0", "");

                    log.Info($"strLicense:{strLicense} InWeighing:{InWeighing}");

                    if (strLicense == "_无_") return 0;
                    if (strLicense == "无牌车") return 0;
                    if (InWeighing) return 0;

                    //模糊匹配
                    ConfigurationManager.RefreshSection("appSettings");
                    if ("1" == ConfigurationManager.AppSettings["FuzzyMatching"])
                    {
                        string[] plateList = ConfigurationManager.AppSettings["FuzzyPlateNo"].Split(',');
                        bool isMatch = false;
                        foreach (var plate in plateList)
                        {
                            string regexStr;
                            regexStr = plate.Replace("0", "[0,D,Q]");
                            isMatch = Regex.IsMatch(strLicense, regexStr);
                            if (isMatch) { strLicense = plate; break; }
                            regexStr = plate.Replace("D", "[0,D]");
                            isMatch = Regex.IsMatch(strLicense, regexStr);
                            if (isMatch) { strLicense = plate; break; }
                            regexStr = plate.Replace("Q", "[0,Q]");
                            isMatch = Regex.IsMatch(strLicense, regexStr);
                            if (isMatch) { strLicense = plate; break; }
                            regexStr = plate.Replace("0", "[0,D,Q]");
                            isMatch = Regex.IsMatch(strLicense, regexStr);
                            if (isMatch) { strLicense = plate; break; }
                            regexStr = plate.Replace("8", "[8,B]");
                            isMatch = Regex.IsMatch(strLicense, regexStr);
                            if (isMatch) { strLicense = plate; break; }
                            regexStr = plate.Replace("B", "[8,B]");
                            isMatch = Regex.IsMatch(strLicense, regexStr);
                            if (isMatch) { strLicense = plate; break; }
                            regexStr = plate.Replace("E", "[E,F]");
                            isMatch = Regex.IsMatch(strLicense, regexStr);
                            if (isMatch) { strLicense = plate; break; }
                            regexStr = plate.Replace("F", "[E,F]");
                            isMatch = Regex.IsMatch(strLicense, regexStr);
                            if (isMatch) { strLicense = plate; break; }
                        }
                    }
                    string tempPicPath = string.Empty;
                    string strIP = string.Empty;

                    try
                    {
                        byte[] strDecIP = new byte[32];
                        VzClientSDK.VzLPRClient_GetDeviceIP(handle, ref strDecIP[0], 32);
                        strIP = Encoding.Default.GetString(strDecIP).TrimEnd('\0');

                        var directoryPath = System.IO.Path.Combine(LPRSavePath, strLicense);

                        log.Info($"路径地址:{directoryPath}");
                        //string tempPicPath = Environment.CurrentDirectory + "\\Snap\\Temp\\" + strLicense + ".jpg";
                         
                        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

                        m_PlateNo = strLicense;
                        if (strIP == ConfigurationManager.AppSettings["Camera1IP"])
                        {

                            m_DevNo = "A";
                        }
                        else if (strIP == ConfigurationManager.AppSettings["Camera2IP"])
                        {
                            m_DevNo = "B";
                        }

                        var direction = m_DevNo == "A" ? "in" : "out";

                        tempPicPath = $"{directoryPath}/{strLicense}_{DateTime.Now.ToString("yyyyMMddHHmm")}_LPR_{direction}.jpg";
                        // string tempPicPath = LPRSavePath + strLicense + ".jpg";
                        VzClientSDK.VzLPRClient_ImageSaveToJpeg(pImgFull, tempPicPath, 100);

                        // 新增ReefAPI , 判断车牌是不是能称重 --阿吉 2023年12月8日16点42分
                        if (PlatformManager.Instance.Current is ReefPlatform _reefAPI)
                        {
                            var reefApiRet = _reefAPI.API.CheckCarNo(strLicense, direction);
                            if (!reefApiRet.Success)
                            {
                                var jsonObj = JsonConvert.SerializeObject(new ReefAPI.APIRessult
                                {
                                    data = strLicense,
                                    message = reefApiRet.Message
                                });
                                log.Info($"reef 接口返回失败:{reefApiRet.Message}");
                                SendMessageToApp("称重软件3.0", $"AJCarBlock,{jsonObj.Replace(",", "|")}");
                                return 0;
                            }
                        }

                        // 新增WuXiAPI , 车辆信息注册 --阿吉 2023年12月22日15点49分
                        if (PlatformManager.Instance.Current is WuXiWeightAPI wuXiWeightAPI)
                        {
                            var wuXiWeightAPIRet = wuXiWeightAPI.CarRegister(strLicense,
                                result.nColor,
                                direction == "in" ? "0" : "1",
                                tempPicPath);
                            if (!wuXiWeightAPIRet.Success)
                            {
                                log.Error($"wuXiWeightAPI 车辆注册接口返回失败:{wuXiWeightAPIRet.Message}", wuXiWeightAPIRet.Data as Exception);
                            }
                        }

                        // 山东博兴 第三方接口（上磅请求）
                        if (PlatformManager.Instance.Current is Shandong_Boxing_Platform sdPlatform)
                        {
                            var apiRet = sdPlatform.Start(strLicense, m_DevNo, tempPicPath);
                            if (apiRet.code != 0)
                            {
                                log.Info($"山东博兴接口：车牌 {strLicense} 平台检测异常：{apiRet.message}");
                                var jsonObj = JsonConvert.SerializeObject(new ReefAPI.APIRessult
                                {
                                    data = strLicense,
                                    message = apiRet.message
                                });
                                SendMessageToApp("称重软件3.0", $"AJCarBlock,{jsonObj.Replace(",", "|")}");
                                return 0;
                            }
                        }
                        var msgBody = new string[] { "Plate",
                            strLicense,
                            m_DevNo,
                            tempPicPath,
                            result.nColor.ToString() // 车牌颜色 
                        };
                        SendMessageToApp("称重软件3.0", string.Join(",", msgBody));

                        log.Info(strIP + " 检测到车牌 " + strLicense);

                    }
                    catch (Exception ex)
                    {
                        log.Info($"OnPlateResult -Create Image Error:{ex.Message}");

                    }


                }
            }
            catch (Exception e)
            {

                log.Info($"OnPlateResult Error:{e.Message}");
            }
            return 0;
        }

        private void SendMsgToCameraLED(int handle, string line1, string line2, string line3, string line4, string tts, string rgb)
        {
            var selectedController = ConfigurationManager.AppSettings["SelectedController"];
            switch (selectedController)
            {
                case "道通物联":
                    Dtwl_Led(handle, line1, line2, line3, line4, tts, rgb);
                    break;
                case "方控":
                    //Fk_Led(handle, 1, line1, tts, rgb);
                    //Fk_Led(handle, 2, line2, tts, rgb);
                    //Fk_Led(handle, 3, line3, tts, rgb);
                    //Fk_Led(handle, 4, line4, tts, rgb);
                    Fk_Led(handle, 1, line1, tts, rgb);
                    Fk_Led(handle, 2, line2, tts, rgb);
                    Fk_Led(handle, 3, line3, tts, rgb);
                    Fk_Led(handle, 4, line4, tts, rgb);
                    break;
                default:
                    Dtwl_Led(handle, line1, line2, line3, line4, tts, rgb);
                    break;
            }
        }

        /// <summary>
        /// 方控LED发送
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="line3"></param>
        /// <param name="line4"></param>
        /// <param name="tts"></param>
        /// <param name="rgb"></param>
        private void Fk_Led(int handle, int rowIndex, string line, string tts, string rgb)
        {
            //因为屏幕不支持 切换颜色，所以统一为黄色。
            byte[] FK_Head = { (byte)rowIndex, 0x3C, 0x03, 0x00 };//显示格式
            byte[] FK_Body = System.Text.Encoding.Default.GetBytes(line);//屏显内容
            byte[] Data = FK_Head.Concat(FK_Body).ToArray();//拼接两数组

            int len = Data.Length + 11;//总长度
            byte[] sendData = new byte[len];
            sendData[0] = 0xAA;//包头，2字节
            sendData[1] = 0x55;
            sendData[2] = 0x1E;//流水号
            sendData[3] = 0x64;//地址
            sendData[4] = 0x00;//业务类型
            sendData[5] = 0x27;//命令
            sendData[6] = (byte)(Data.Length >> 8);//长度高位
            sendData[7] = (byte)Data.Length;//长度低位
            int i = 8;
            for (; i < 8 + Data.Length; i++)
            {
                sendData[i] = Data[i - 8];
            }
            //CRC16校验
            byte[] crcByte = CRC.Run(sendData, 2, len - 1);
            sendData[i++] = crcByte[0];
            sendData[i++] = crcByte[1];
            sendData[i++] = 0xAF;//结尾

            GCHandle hObject = GCHandle.Alloc(sendData, GCHandleType.Pinned);
            IntPtr pObject = hObject.AddrOfPinnedObject();
            //var tt = Int32ToHex(sendData);
            VzClientSDK.VzLPRClient_SerialSend(handle, pObject, sendData.Length);

            if (hObject.IsAllocated)
                hObject.Free();

            Thread.Sleep(70);
        }

        public static string Int32ToHex(byte[] data, string split = " ")
        {
            if (data == null) return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                if (split == "" || split == null)
                {
                    sb.AppendFormat("{0:x2}", data[i]);
                }
                else
                {
                    sb.AppendFormat("{0:x2}" + split, data[i]);
                }
            }
            string result = sb.ToString().Trim().ToUpper();
            if (result.EndsWith(split))
            {
                int index = result.LastIndexOf(split);
                result = result.Remove(index, split.Length);
            }
            return result;
        }

        /// <summary>
        /// 将16进制的字符串转为byte[]
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] StrToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 道通物联LED发送
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="line3"></param>
        /// <param name="line4"></param>
        /// <param name="tts"></param>
        /// <param name="rgb"></param>
        private void Dtwl_Led(int handle, string line1, string line2, string line3, string line4, string tts, string rgb)
        {
            //去空格
            line1 = line1.Replace(" ", string.Empty).Replace("\0", string.Empty).Trim();
            line2 = line2.Replace(" ", string.Empty).Replace("\0", string.Empty).Trim();
            line3 = line3.Replace(" ", string.Empty).Replace("\0", string.Empty).Trim();
            line4 = line4.Replace(" ", string.Empty).Replace("\0", string.Empty).Trim();

            //为空处理
            line1 = string.IsNullOrWhiteSpace(line1) ? " " : line1;
            line1 = string.IsNullOrWhiteSpace(line1) ? " " : line1;
            line1 = string.IsNullOrWhiteSpace(line1) ? " " : line1;
            line1 = string.IsNullOrWhiteSpace(line1) ? " " : line1;

            line1 = Str2Hex(line1);
            line2 = Str2Hex(line2);
            line3 = Str2Hex(line3);
            line4 = Str2Hex(line4);
            tts = Str2Hex(tts);
            string line1Len = Convert.ToString(line1.Length / 3, 16).ToUpper().PadLeft(2, '0');
            string line2Len = Convert.ToString(line2.Length / 3, 16).ToUpper().PadLeft(2, '0');
            string line3Len = Convert.ToString(line3.Length / 3, 16).ToUpper().PadLeft(2, '0');
            string line4Len = Convert.ToString(line4.Length / 3, 16).ToUpper().PadLeft(2, '0');
            string ttsLen = Convert.ToString(tts.Length / 3, 16).ToUpper().PadLeft(2, '0');

            string strHead = "00 64 FF FF 6E DL 00 04 ";
            string strLine1 = "00 01 01 05 00 R G 00 00 DL DATA";
            string strLine2 = "01 01 01 05 00 R G 00 00 DL DATA";
            string strLine3 = "02 01 01 05 00 R G 00 00 DL DATA";
            string strLine4 = "03 01 01 05 00 R G 00 00 DL DATA";
            string strTTS = "0A VTL DATA";

            if (rgb == "r")
            {
                strLine1 = strLine1.Replace("R", "FF").Replace("G", "00");
                strLine2 = strLine2.Replace("R", "FF").Replace("G", "00");
                strLine3 = strLine3.Replace("R", "FF").Replace("G", "00");
                strLine4 = strLine4.Replace("R", "FF").Replace("G", "00");
            }
            else if (rgb == "g")
            {
                strLine1 = strLine1.Replace("R", "00").Replace("G", "FF");
                strLine2 = strLine2.Replace("R", "00").Replace("G", "FF");
                strLine3 = strLine3.Replace("R", "00").Replace("G", "FF");
                strLine4 = strLine4.Replace("R", "00").Replace("G", "FF");
            }
            strLine1 = strLine1.Replace("DL", line1Len).Replace("DATA", line1);
            strLine2 = strLine2.Replace("DL", line2Len).Replace("DATA", line2);
            strLine3 = strLine3.Replace("DL", line3Len).Replace("DATA", line3);
            strLine4 = strLine4.Replace("DL", line4Len).Replace("DATA", line4);
            strTTS = strTTS.Replace("VTL", ttsLen).Replace("DATA", tts);

            string data = strLine1 + "0D " + strLine2 + "0D " + strLine3 + "0D " + strLine4 + "00 " + strTTS + "00 ";
            string dataLen = Convert.ToString(data.Length / 3 + 2, 16).ToUpper().PadLeft(2, '0');

            strHead = strHead.Replace("DL", dataLen);
            data = strHead + data;
            string strcrc = CRC.ToModbusCRC16(data, true);
            strcrc = strcrc.Insert(2, " ");
            data = data + strcrc;

            byte[] send_buf = new byte[1024];
            string new_content = data.Insert(data.Length, " ");
            int txt_len = new_content.Length;
            char[] txt_buf = new_content.ToCharArray();

            int index = 0;

            char[] strHex = new char[3];
            byte uc;
            for (int i = 0; i < txt_len - 2; i += 3)
            {
                if (txt_buf[i + 2] != ' ')
                {
                    MessageBox.Show("16 进制数据输入格式不正确");
                    log.Info($" 16 进制数据输入格式不正确_1 txt_buf:{new string(txt_buf)}");
                    return;
                }

                strHex[0] = txt_buf[i];
                strHex[1] = txt_buf[i + 1];
                strHex[2] = (char)0;

                for (int j = 0; j < 2; j++)
                {
                    if (strHex[j] < '0' || (strHex[j] > '9' && strHex[j] < 'A') || (strHex[j] > 'F' &&
                        strHex[j] < 'a') || strHex[j] > 'f')
                    {
                        MessageBox.Show("16 进制数据输入格式不正确");
                        log.Info($" 16 进制数据输入格式不正确_2  strHex:{new string(strHex)} ");
                        return;
                    }
                }

                string hex_value = new string(strHex);
                uc = byte.Parse(hex_value, System.Globalization.NumberStyles.HexNumber);
                send_buf[index] = uc;
                index++;
            }

            GCHandle hObject = GCHandle.Alloc(send_buf, GCHandleType.Pinned);
            IntPtr pObject = hObject.AddrOfPinnedObject();

            var FLAG = VzClientSDK.VzLPRClient_SerialSend(handle, pObject, index);

            if (hObject.IsAllocated)
                hObject.Free();
        }

        private void SwitchLight(int handle, bool isOpen = true)
        {

            var selectedController = ConfigurationManager.AppSettings["SelectedController"];
            switch (selectedController)
            {
                case "道通物联":
                    Dtwl_SwitchLight(handle, isOpen);
                    break;
                case "方控":
                    Fk_SwitchLight(handle, isOpen);
                    break;
                default:
                    Dtwl_SwitchLight(handle, isOpen);
                    break;
            }
        }

        private void Fk_SwitchLight(int handle, bool isOpen = true)
        {
            //string strHead = "00 64 FF FF 0F DL ";
            string strHead = "00 64 FF FF 0F 06 ";
            var data = "00 01 00 00 00 FF ";
            //data = "00 01 00 00 00 0A ";
            if (!isOpen)
            {
                data = "02 01 00 00 00 02 ";
            }
            string dataLen = Convert.ToString(data.Length / 3 + 2, 16).ToUpper().PadLeft(2, '0');

            // strHead = strHead.Replace("DL", dataLen);
            data = strHead + data;
            string strcrc = CRC.ToModbusCRC16(data, true);
            strcrc = strcrc.Insert(2, " ");
            data = data + strcrc;

            byte[] send_buf = new byte[1024];
            string new_content = data.Insert(data.Length, " ");
            int txt_len = new_content.Length;
            char[] txt_buf = new_content.ToCharArray();

            int index = 0;

            char[] strHex = new char[3];
            byte uc;
            for (int i = 0; i < txt_len - 2; i += 3)
            {
                if (txt_buf[i + 2] != ' ')
                {
                    MessageBox.Show("16 进制数据输入格式不正确");
                    log.Info($" 16 进制数据输入格式不正确_1 txt_buf:{new string(txt_buf)}");
                    return;
                }

                strHex[0] = txt_buf[i];
                strHex[1] = txt_buf[i + 1];
                strHex[2] = (char)0;

                for (int j = 0; j < 2; j++)
                {
                    if (strHex[j] < '0' || (strHex[j] > '9' && strHex[j] < 'A') || (strHex[j] > 'F' &&
                        strHex[j] < 'a') || strHex[j] > 'f')
                    {
                        MessageBox.Show("16 进制数据输入格式不正确");
                        log.Info($" 16 进制数据输入格式不正确_2  strHex:{new string(strHex)} ");
                        return;
                    }
                }

                string hex_value = new string(strHex);
                uc = byte.Parse(hex_value, System.Globalization.NumberStyles.HexNumber);
                send_buf[index] = uc;
                index++;
            }

            GCHandle hObject = GCHandle.Alloc(send_buf, GCHandleType.Pinned);
            IntPtr pObject = hObject.AddrOfPinnedObject();

            var f = VzClientSDK.VzLPRClient_SerialSend(handle, pObject, index);

            if (hObject.IsAllocated)
                hObject.Free();
        }

        private void Dtwl_SwitchLight(int handle, bool isOpen = true)
        {
            //string strHead = "00 64 FF FF 0F DL ";
            string strHead = "00 64 FF FF 0F 06 ";
            var data = "00 01 00 00 00 FF ";
            //data = "00 01 00 00 00 0A ";
            if (!isOpen)
            {
                data = "02 01 00 00 00 02 ";
            }
            string dataLen = Convert.ToString(data.Length / 3 + 2, 16).ToUpper().PadLeft(2, '0');

            // strHead = strHead.Replace("DL", dataLen);
            data = strHead + data;
            string strcrc = CRC.ToModbusCRC16(data, true);
            strcrc = strcrc.Insert(2, " ");
            data = data + strcrc;

            byte[] send_buf = new byte[1024];
            string new_content = data.Insert(data.Length, " ");
            int txt_len = new_content.Length;
            char[] txt_buf = new_content.ToCharArray();

            int index = 0;

            char[] strHex = new char[3];
            byte uc;
            for (int i = 0; i < txt_len - 2; i += 3)
            {
                if (txt_buf[i + 2] != ' ')
                {
                    MessageBox.Show("16 进制数据输入格式不正确");
                    log.Info($" 16 进制数据输入格式不正确_1 txt_buf:{new string(txt_buf)}");
                    return;
                }

                strHex[0] = txt_buf[i];
                strHex[1] = txt_buf[i + 1];
                strHex[2] = (char)0;

                for (int j = 0; j < 2; j++)
                {
                    if (strHex[j] < '0' || (strHex[j] > '9' && strHex[j] < 'A') || (strHex[j] > 'F' &&
                        strHex[j] < 'a') || strHex[j] > 'f')
                    {
                        MessageBox.Show("16 进制数据输入格式不正确");
                        log.Info($" 16 进制数据输入格式不正确_2  strHex:{new string(strHex)} ");
                        return;
                    }
                }

                string hex_value = new string(strHex);
                uc = byte.Parse(hex_value, System.Globalization.NumberStyles.HexNumber);
                send_buf[index] = uc;
                index++;
            }

            GCHandle hObject = GCHandle.Alloc(send_buf, GCHandleType.Pinned);
            IntPtr pObject = hObject.AddrOfPinnedObject();

            var f = VzClientSDK.VzLPRClient_SerialSend(handle, pObject, index);

            if (hObject.IsAllocated)
                hObject.Free();
        }

        private string Str2Hex(string s)
        {
            string result = string.Empty;

            byte[] arrByte = Encoding.GetEncoding("GB2312").GetBytes(s);
            for (int i = 0; i < arrByte.Length; i++)
            {
                result += Convert.ToString(arrByte[i], 16) + " ";        //Convert.ToString(byte, 16)把byte转化成十六进制string 
            }

            return result.ToUpper();
        }
        #endregion

        #region 远程读卡器 旧版

        //private void OpenRFPort()
        //{
        //    if (ConfigurationManager.AppSettings["RF1Enable"] == "1")
        //    {
        //        m_RF1PortName = ConfigurationManager.AppSettings["RF1Uart"];
        //        int port = Convert.ToInt32(m_RF1PortName.Substring(3, 1));
        //        /*
        //         1.工作模式 0：应答，1：主动 -> 实时切换
        //         2.协议选择【bit0】0：6C，1：6B 【bit1】0：韦根，1：RS232  -> 设置为10，也就是2
        //         3.查询区域，6C卡有效，固定设置为5：单张查询
        //         4.读取起始地址，固定设置为0：从第一个字开始读
        //         5.要读取的字数，RS232有效，固定设置为1：读取一个字
        //         6.主动模式+单张查询间隔时间，设置为5，表示5秒的间隔
        //         */
        //        byte[] Parameter = new byte[6] { 1, 2, 5, 0, 1, 5 };
        //        m_RF1OpenRet = StaticClassReaderB.OpenComPort(port, ref m_RF1ComAdr, 5, ref m_RF1ComIndex);
        //        m_IsActiveScan = true;

        //        if (m_RF1OpenRet == 0)
        //        {
        //            StaticClassReaderB.BuzzerAndLEDControl(ref m_RF1ComAdr, 1, 1, 2, m_RF1ComIndex);
        //            StaticClassReaderB.SetWorkMode(ref m_RF1ComAdr, Parameter, m_RF1ComIndex);
        //            byte powerDbm = Convert.ToByte(ConfigurationManager.AppSettings["RF1Power"]);
        //            StaticClassReaderB.SetPowerDbm(ref m_RF1ComAdr, powerDbm, m_RF1ComIndex);
        //            log.Info("车辆标签串口1打开成功");
        //        }
        //        else
        //        {
        //            log.Error("车辆标签串口1打开失败！错误代码：" + m_RF1OpenRet.ToString());
        //        }
        //    }

        //    if (ConfigurationManager.AppSettings["RF2Enable"] == "1")
        //    {
        //        m_RF2PortName = ConfigurationManager.AppSettings["RF2Uart"];
        //        int port = Convert.ToInt32(m_RF2PortName.Substring(3, 1));
        //        /*
        //         1.工作模式 0：应答，1：主动 -> 实时切换
        //         2.协议选择【bit0】0：6C，1：6B 【bit1】0：韦根，1：RS232  -> 设置为10，也就是2
        //         3.查询区域，6C卡有效，固定设置为5：单张查询
        //         4.读取起始地址，固定设置为0：从第一个字开始读
        //         5.要读取的字数，RS232有效，固定设置为1：读取一个字
        //         6.主动模式+单张查询间隔时间，设置为5，表示5秒的间隔
        //         */
        //        byte[] Parameter = new byte[6] { 1, 2, 5, 0, 1, 5 };
        //        m_RF2OpenRet = StaticClassReaderB.OpenComPort(port, ref m_RF2ComAdr, 5, ref m_RF2ComIndex);

        //        if (m_RF2OpenRet == 0)
        //        {
        //            StaticClassReaderB.BuzzerAndLEDControl(ref m_RF2ComAdr, 1, 1, 2, m_RF2ComIndex);
        //            StaticClassReaderB.SetWorkMode(ref m_RF2ComAdr, Parameter, m_RF2ComIndex);
        //            byte powerDbm = Convert.ToByte(ConfigurationManager.AppSettings["RF2Power"]);
        //            StaticClassReaderB.SetPowerDbm(ref m_RF2ComAdr, powerDbm, m_RF2ComIndex);
        //            log.Info("车辆标签串口2打开成功");
        //        }
        //        else
        //        {
        //            log.Error("车辆标签串口2打开失败！错误代码：" + m_RF2OpenRet.ToString());
        //        }
        //    }
        //}

        //private void RFTimer_Tick()
        //{
        //    if (m_RF1OpenRet == 0)
        //    {
        //        byte[] ScanModeData = new byte[100];
        //        int Totallen = 0;
        //        int fCmdRet = StaticClassReaderB.ReadActiveModeData(ScanModeData, ref Totallen, m_RF1ComIndex);

        //        if (fCmdRet == 0)
        //        {
        //            //读取到卡，设置为应答模式
        //            PassiveScan();

        //            byte[] daw = new byte[12]; //epc长度12
        //            Array.Copy(ScanModeData, 4, daw, 0, 12);
        //            string strEPC = ByteArrayToHexString(daw);
        //            CarLabelModel carLabel = SQLDataAccess.LoadCarLabel(strEPC);

        //            if (carLabel != null)
        //            {
        //                log.Info("读卡器1," + strEPC + "," + carLabel.PlateNo);
        //                SendMessageToApp("称重软件3.0", "Plate," + carLabel.PlateNo + ",A");
        //            }
        //            else
        //            {
        //                log.Info("读卡器1," + strEPC + ",未查询到车号");
        //                ActiveScan();
        //            }
        //        }
        //    }

        //    if (m_RF2OpenRet == 0)
        //    {
        //        byte[] ScanModeData = new byte[100];
        //        int Totallen = 0;
        //        int fCmdRet = StaticClassReaderB.ReadActiveModeData(ScanModeData, ref Totallen, m_RF2ComIndex);

        //        if (fCmdRet == 0)
        //        {
        //            //读取到卡，设置为应答模式
        //            PassiveScan();

        //            byte[] daw = new byte[12]; //epc长度12
        //            Array.Copy(ScanModeData, 4, daw, 0, 12);
        //            string strEPC = ByteArrayToHexString(daw);
        //            CarLabelModel carLabel = SQLDataAccess.LoadCarLabel(strEPC);

        //            if (carLabel != null)
        //            {
        //                log.Info("读卡器2," + strEPC + "," + carLabel.PlateNo);
        //                SendMessageToApp("称重软件3.0", "Plate," + carLabel.PlateNo + ",B");
        //            }
        //            else
        //            {
        //                log.Info("读卡器2," + strEPC + ",未查询到车号");
        //                ActiveScan();
        //            }
        //        }
        //    }
        //}

        //private void ActiveScan()
        //{
        //    byte[] Parameter = new byte[6] { 1, 2, 5, 0, 1, 5 };
        //    StaticClassReaderB.SetWorkMode(ref m_RF1ComAdr, Parameter, m_RF1ComIndex);
        //    StaticClassReaderB.SetWorkMode(ref m_RF2ComAdr, Parameter, m_RF2ComIndex);
        //    m_IsActiveScan = true;
        //    log.Info("RF主动模式");
        //}

        //private void PassiveScan()
        //{
        //    byte[] Parameter = new byte[6] { 0, 2, 5, 0, 1, 5 };
        //    StaticClassReaderB.SetWorkMode(ref m_RF1ComAdr, Parameter, m_RF1ComIndex);
        //    StaticClassReaderB.SetWorkMode(ref m_RF2ComAdr, Parameter, m_RF2ComIndex);
        //    m_IsActiveScan = false;
        //    log.Info("RF被动模式");
        //}

        #endregion

        #region 远程读卡器 第二版
        private void OpenRFPort()
        {
            if (ConfigurationManager.AppSettings["RF1Enable"] == "1")
            {
                m_RF1Port.PortName = ConfigurationManager.AppSettings["RF1Uart"];
                m_RF1Port.BaudRate = 57600;
                var openResult = false;
                try
                {
                    m_RF1Port.Open();

                    if (m_RF1Port.IsOpen)
                    {
                        Thread recvRF1PortDataThread = new Thread(new ThreadStart(RecvRF1PortData)) { IsBackground = true };
                        recvRF1PortDataThread.Start();

                        log.Info("打开RF1串口：" + m_RF1Port.PortName);

                        openResult = true;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }

                var jsonObj = JObject.FromObject(new 
                {
                    Key = Common.Model.DeviceNoticeIcon.DeviceKey.读头,
                    Online = openResult
                }).ToString(Formatting.None);
                SendMessageToApp("称重软件3.0", $"AJDeviceNotice,{jsonObj.Replace(",", "|")}");
            }
            if (ConfigurationManager.AppSettings["RF2Enable"] == "1")
            {
                m_RF2Port.PortName = ConfigurationManager.AppSettings["RF2Uart"];
                m_RF2Port.BaudRate = 57600;
                var openResult = false;
                try
                {
                    m_RF2Port.Open();

                    if (m_RF2Port.IsOpen)
                    {
                        Thread recvRF2PortDataThread = new Thread(new ThreadStart(RecvRF2PortData)) { IsBackground = true };
                        recvRF2PortDataThread.Start();

                        log.Info("打开RF2串口：" + m_RF2Port.PortName);

                        openResult = true;

                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                    
                }

                var jsonObj = JObject.FromObject(new
                {
                    Key = Common.Model.DeviceNoticeIcon.DeviceKey.读头,
                    Online = openResult
                }).ToString(Formatting.None);
                SendMessageToApp("称重软件3.0", $"AJDeviceNotice,{jsonObj.Replace(",", "|")}");
            }
        }

        //RF1串口接收数据线程
        private void RecvRF1PortData()
        {
            m_RF1Port.DataReceived += (sender, e) =>
            {
                try
                {
                    byte[] RecvDatas = new byte[m_RF1Port.BytesToRead];
                    m_RF1Port.Read(RecvDatas, 0, RecvDatas.Length);
                    string strEPC = ByteArrayToHexString(RecvDatas);

                    m_EpcStrAppend += strEPC;
                    if (!m_EpcStrAppend.StartsWith("CCFF"))
                    {
                        m_EpcStrAppend = m_EpcStrAppend.Substring(m_EpcStrAppend.IndexOf("CCFF"));
                    }
                    if (m_EpcStrAppend.Length < 46)
                    {
                        log.Info("append: " + m_EpcStrAppend);
                        return;
                    }
                    if (!m_EpcStrAppend.StartsWith("CCFF"))
                    {
                        log.Info("NOT FIND CCFF");
                        return;
                    }
                    strEPC = m_EpcStrAppend.Substring(18, 24);
                    m_EpcStrAppend = string.Empty;

                    CarLabelModel carLabel = SQLDataAccess.LoadCarLabel(strEPC);
                    if (carLabel != null)
                    {
                        if (!InWeighing && carLabel.PlateNo != m_PlateNo)
                        {
                            log.Info("读卡器1," + strEPC + "," + carLabel.PlateNo);
                            m_PlateNo = carLabel.PlateNo;
                            SendMessageToApp("称重软件3.0", "Plate," + carLabel.PlateNo + ",A");
                        }
                    }
                    else
                    {
                        log.Info("读卡器1," + strEPC + ",未查询到车号");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            };
        }
        private void RecvRF2PortData()
        {
            m_RF2Port.DataReceived += (sender, e) =>
            {
                try
                {
                    byte[] RecvDatas = new byte[m_RF2Port.BytesToRead];
                    m_RF2Port.Read(RecvDatas, 0, RecvDatas.Length);
                    string strEPC = ByteArrayToHexString(RecvDatas);

                    m_EpcStrAppend += strEPC;
                    if (m_EpcStrAppend.Length < 42)
                    {
                        //m_EpcStrAppend += strEPC;
                        //log.Info(m_EpcStrAppend);
                        return;
                    }

                    strEPC = m_EpcStrAppend.Substring(18, 24);
                    m_EpcStrAppend = string.Empty;

                    //if (strEPC.Length < 24)
                    //{
                    //    log.Info(strEPC);
                    //    return;
                    //}                    
                    //strEPC = strEPC.Substring(18, 24);

                    CarLabelModel carLabel = SQLDataAccess.LoadCarLabel(strEPC);
                    if (carLabel != null)
                    {
                        if (!InWeighing && carLabel.PlateNo != m_PlateNo)
                        {
                            log.Info("读卡器2," + strEPC + "," + carLabel.PlateNo);
                            m_PlateNo = carLabel.PlateNo;
                            SendMessageToApp("称重软件3.0", "Plate," + carLabel.PlateNo + ",B");
                        }
                    }
                    else
                    {
                        log.Info("读卡器2," + strEPC + ",未查询到车号");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            };
        }
        #endregion

        #region 远程读卡器 网络版

        public void SendSio(ProtocolPacket protocolPacket)
        {
            if (IsConnectedSio)
            {
                RcpBase.ShowBytePack(protocolPacket);
                SioBase.Send(protocolPacket.ToArray());
            }
        }

        private void SioBase_onStatus(object sender, StatusEventArgs e)
        {
            try
            {
                switch ((StatusType)e.Status)
                {
                    case StatusType.CONNECT_OK:
                        try
                        {
                            int intVer = Convert.ToInt32(e.Msg);
                        }
                        catch { }
                        log.Info("CONNECTED OK> " + e.Msg + "(" + SioBase.ToString() + ")");
                        break;
                    case StatusType.CONNECT_FAIL:
                        log.Info("ERROR> " + e.Msg + "(" + SioBase.ToString() + ")");
                        break;
                    case StatusType.DISCONNECT_OK:
                        log.Info("DISCONNECT OK> " + e.Msg + "(" + SioBase.ToString() + ")");
                        break;
                    case StatusType.DISCONNECT_EXCEPT:
                        log.Info("ERROR> " + e.Msg + "(" + SioBase.ToString() + ")");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
            }
        }

        private void SioBase_onReceived(object sender, ReceivedEventArgs e)
        {
            RcpBase.ReciveBytePkt(e.Data);
        }

        private void RcpBase_RxRspParsed(object sender, ProtocolEventArgs e)
        {
            try
            {
                log.Info("Rx> " + TagInfo.ByteArrayToHexString(e.Data));
                __ParseRsp(e.Protocol);
            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
            }
        }

        private void SioBase2_onStatus(object sender, StatusEventArgs e)
        {
            try
            {
                switch ((StatusType)e.Status)
                {
                    case StatusType.CONNECT_OK:
                        try
                        {
                            int intVer = Convert.ToInt32(e.Msg);
                        }
                        catch { }
                        log.Info("CONNECTED OK> " + e.Msg + "(" + SioBase2.ToString() + ")");
                        break;
                    case StatusType.CONNECT_FAIL:
                        log.Info("ERROR> " + e.Msg + "(" + SioBase2.ToString() + ")");
                        break;
                    case StatusType.DISCONNECT_OK:
                        log.Info("DISCONNECT OK> " + e.Msg + "(" + SioBase2.ToString() + ")");
                        break;
                    case StatusType.DISCONNECT_EXCEPT:
                        log.Info("ERROR> " + e.Msg + "(" + SioBase2.ToString() + ")");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
            }
        }

        private void SioBase2_onReceived(object sender, ReceivedEventArgs e)
        {
            RcpBase2.ReciveBytePkt(e.Data);
        }

        private void RcpBase2_RxRspParsed(object sender, ProtocolEventArgs e)
        {
            try
            {
                log.Info("Rx> " + TagInfo.ByteArrayToHexString(e.Data));
                __ParseRsp2(e.Protocol);
            }
            catch (Exception ex)
            {
                log.Info(ex.ToString());
            }
        }

        private int GetCodelen(byte iData)
        {
            return (((iData >> 3) + 1) * 2);
        }

        private string GetRssi(byte rssi)
        {
            int rssidBm = (sbyte)rssi; // rssidBm is negative && in bytes
            rssidBm -= Convert.ToInt32("-20", 10);
            rssidBm -= Convert.ToInt32("3", 10);
            return rssidBm.ToString();
        }

        private void __ParseRsp(ProtocolPacket protocolPacket)
        {
            switch (protocolPacket.Code)
            {
                case RcpBase.RCP_CMD_INFO:
                    if (protocolPacket.Length > 30 && (protocolPacket.Type & 0x7f) == 0)
                    {
                        #region ---Parameter---
                        string strInfo = Encoding.ASCII.GetString(protocolPacket.Payload, 0, protocolPacket.Length);

                        log.Info("Type:" + RcpBase.Mode + RcpBase.Type + " - Version:" + RcpBase.Version + " - Address: " + RcpBase.Address);
                        #endregion
                    }
                    break;

                case RcpBase.RCP_MM_READ_C_UII:
                    if (protocolPacket.Type == 2 || protocolPacket.Type == 5)
                    {
                        int pcepclen = GetCodelen(protocolPacket.Payload[1]);
                        int datalen = protocolPacket.Length - 2;//去掉天线号去掉rssi
                        TagInfo cp = new TagInfo
                        {
                            TagType = TagType.TYPE_6C,
                            Length = datalen,//去掉天线号去掉RSSI
                            Antenna = protocolPacket.Payload[0],
                            PCData = TagInfo.GetData(protocolPacket.Payload, 1, 2),
                            EPCData = TagInfo.GetData(protocolPacket.Payload, 3, pcepclen - 2),
                            Rssi = GetRssi(protocolPacket.Payload[protocolPacket.Length - 1]) + "dBm"
                        };
                        if ((datalen - pcepclen) > 0) cp.DataBytes = TagInfo.GetData(protocolPacket.Payload, 1 + pcepclen, datalen - pcepclen);
                        //txtCard.Text = cp.EPCString;
                        //log.Info(cp.EPCString);
                        CarLabelModel carLabel = SQLDataAccess.LoadCarLabel(cp.EPCString);
                        if (carLabel != null)
                        {
                            //log.Info($"carLabel.PlateNo:{carLabel.PlateNo},InWeighing:{InWeighing},m_PlateNo:{m_PlateNo}");
                            if (!InWeighing && carLabel.PlateNo != m_PlateNo)
                            {
                                m_PlateNo = carLabel.PlateNo;
                                //log.Info($"protocolPacket.Address:{ByteArrayToHexString(protocolPacket.Address)},RF3IP:{ConfigurationManager.AppSettings["RF3IP"]},RF4IP:{ConfigurationManager.AppSettings["RF4IP"]}");
                                //if (ByteArrayToHexString(protocolPacket.Address) == ConfigurationManager.AppSettings["RF3IP"])
                                //{
                                log.Info("读卡器1," + cp.EPCString + "," + carLabel.PlateNo);
                                SendMessageToApp("称重软件3.0", "Plate," + carLabel.PlateNo + ",A");
                                //}
                                //else if (ByteArrayToHexString(protocolPacket.Address) == ConfigurationManager.AppSettings["RF4IP"])
                                //{
                                //    log.Info("读卡器2," + cp.EPCString + "," + carLabel.PlateNo);
                                //    SendMessageToApp("称重软件3.0", "Plate," + carLabel.PlateNo + ",B");
                                //}
                            }
                        }
                        else
                        {
                            log.Info("ao: " + cp.EPCString + " 未查询到车号");
                        }
                    }

                    break;
                case RcpBase.RCP_MM_READ_C_DT:
                    if (protocolPacket.Type == 0)
                    {
                        int pcepclen = GetCodelen(protocolPacket.Payload[1]);
                        int datalen = protocolPacket.Length - pcepclen - 1;//去掉天线号去掉PC+EPc
                        TagInfo cp = new TagInfo
                        {
                            TagType = TagType.TYPE_6C,
                            Length = datalen,
                            Antenna = protocolPacket.Payload[0],
                            PCData = TagInfo.GetData(protocolPacket.Payload, 1, 2),
                            EPCData = TagInfo.GetData(protocolPacket.Payload, 3, pcepclen - 2),
                            DataBytes = TagInfo.GetData(protocolPacket.Payload, 1 + pcepclen, datalen)
                        };
                    }
                    break;
                case RcpBase.RCP_MM_WRITE_C_DT:
                    if (protocolPacket.Type == 0)
                    {
                        int pcepclen = GetCodelen(protocolPacket.Payload[1]);
                        int datalen = protocolPacket.Length - pcepclen - 1;//去掉天线号去掉PC+EPc
                        TagInfo cp = new TagInfo
                        {
                            TagType = TagType.TYPE_6C,
                            Length = datalen,
                            Antenna = protocolPacket.Payload[0],
                            PCData = TagInfo.GetData(protocolPacket.Payload, 1, 2),
                            EPCData = TagInfo.GetData(protocolPacket.Payload, 3, pcepclen - 2)
                        };
                    }
                    break;
                case RcpBase.RCP_MM_GET_ACCESS_EPC_MATCH:
                    break;
                case RcpBase.RCP_MM_SET_ACCESS_EPC_MATCH:
                    break;
                default:
                    break;
            }
        }
        private void __ParseRsp2(ProtocolPacket protocolPacket)
        {
            switch (protocolPacket.Code)
            {
                case RcpBase.RCP_CMD_INFO:
                    if (protocolPacket.Length > 30 && (protocolPacket.Type & 0x7f) == 0)
                    {
                        #region ---Parameter---
                        string strInfo = Encoding.ASCII.GetString(protocolPacket.Payload, 0, protocolPacket.Length);

                        log.Info("Type:" + RcpBase.Mode + RcpBase.Type + " - Version:" + RcpBase.Version + " - Address: " + RcpBase.Address);
                        #endregion
                    }
                    break;

                case RcpBase.RCP_MM_READ_C_UII:
                    if (protocolPacket.Type == 2 || protocolPacket.Type == 5)
                    {
                        int pcepclen = GetCodelen(protocolPacket.Payload[1]);
                        int datalen = protocolPacket.Length - 2;//去掉天线号去掉rssi
                        TagInfo cp = new TagInfo
                        {
                            TagType = TagType.TYPE_6C,
                            Length = datalen,//去掉天线号去掉RSSI
                            Antenna = protocolPacket.Payload[0],
                            PCData = TagInfo.GetData(protocolPacket.Payload, 1, 2),
                            EPCData = TagInfo.GetData(protocolPacket.Payload, 3, pcepclen - 2),
                            Rssi = GetRssi(protocolPacket.Payload[protocolPacket.Length - 1]) + "dBm"
                        };
                        if ((datalen - pcepclen) > 0) cp.DataBytes = TagInfo.GetData(protocolPacket.Payload, 1 + pcepclen, datalen - pcepclen);
                        //txtCard.Text = cp.EPCString;
                        //log.Info(cp.EPCString);
                        CarLabelModel carLabel = SQLDataAccess.LoadCarLabel(cp.EPCString);
                        if (carLabel != null)
                        {
                            //log.Info($"carLabel.PlateNo:{carLabel.PlateNo},InWeighing:{InWeighing},m_PlateNo:{m_PlateNo}");
                            if (!InWeighing && carLabel.PlateNo != m_PlateNo)
                            {
                                m_PlateNo = carLabel.PlateNo;
                                //log.Info($"protocolPacket.Address:{ByteArrayToHexString(protocolPacket.Address)},RF3IP:{ConfigurationManager.AppSettings["RF3IP"]},RF4IP:{ConfigurationManager.AppSettings["RF4IP"]}");
                                //if (ByteArrayToHexString(protocolPacket.Address) == ConfigurationManager.AppSettings["RF3IP"])
                                //{
                                //    log.Info("读卡器1," + cp.EPCString + "," + carLabel.PlateNo);
                                //    SendMessageToApp("衡七管家", "Plate," + carLabel.PlateNo + ",A");
                                //}
                                //else if (ByteArrayToHexString(protocolPacket.Address) == ConfigurationManager.AppSettings["RF4IP"])
                                //{
                                log.Info("读卡器2," + cp.EPCString + "," + carLabel.PlateNo);
                                SendMessageToApp("称重软件3.0", "Plate," + carLabel.PlateNo + ",B");
                                //}
                            }
                        }
                        else
                        {
                            log.Info("卡号: " + cp.EPCString + " 未查询到车号");
                        }
                    }

                    break;
                case RcpBase.RCP_MM_READ_C_DT:
                    if (protocolPacket.Type == 0)
                    {
                        int pcepclen = GetCodelen(protocolPacket.Payload[1]);
                        int datalen = protocolPacket.Length - pcepclen - 1;//去掉天线号去掉PC+EPc
                        TagInfo cp = new TagInfo
                        {
                            TagType = TagType.TYPE_6C,
                            Length = datalen,
                            Antenna = protocolPacket.Payload[0],
                            PCData = TagInfo.GetData(protocolPacket.Payload, 1, 2),
                            EPCData = TagInfo.GetData(protocolPacket.Payload, 3, pcepclen - 2),
                            DataBytes = TagInfo.GetData(protocolPacket.Payload, 1 + pcepclen, datalen)
                        };
                    }
                    break;
                case RcpBase.RCP_MM_WRITE_C_DT:
                    if (protocolPacket.Type == 0)
                    {
                        int pcepclen = GetCodelen(protocolPacket.Payload[1]);
                        int datalen = protocolPacket.Length - pcepclen - 1;//去掉天线号去掉PC+EPc
                        TagInfo cp = new TagInfo
                        {
                            TagType = TagType.TYPE_6C,
                            Length = datalen,
                            Antenna = protocolPacket.Payload[0],
                            PCData = TagInfo.GetData(protocolPacket.Payload, 1, 2),
                            EPCData = TagInfo.GetData(protocolPacket.Payload, 3, pcepclen - 2)
                        };
                    }
                    break;
                case RcpBase.RCP_MM_GET_ACCESS_EPC_MATCH:
                    break;
                case RcpBase.RCP_MM_SET_ACCESS_EPC_MATCH:
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 继电器网络版
        private void Delay2_Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!socket.Connected)
                {
                    socket.Close();
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["Relay2IP"]), 6000));
                    log.Info("控制器重新连接:" + socket.Connected);
                }


                byte[] buf = new byte[] { };
                ////buf = System.Text.Encoding.Default.GetBytes("AT+OCCH1=?\r\nAT+OCCH2=?\r\n");
                //buf = System.Text.Encoding.Default.GetBytes("AT+OCCH1=?\r\n");
                //int state1 = socket.Send(buf);
                ////Thread.Sleep(10);
                //buf = System.Text.Encoding.Default.GetBytes("AT+OCCH2=?\r\n");
                //int state2 = socket.Send(buf);
                ////Thread.Sleep(50);

                ////循环发送1-8的端口状态查询命令！其中：1-2 为光栅端口状态。3-8 为 其他按钮端口状态
                //for (int i = 1; i <= 8; i++)
                //{
                //    buf = System.Text.Encoding.Default.GetBytes($"AT+OCCH{i}=?\r\n");
                //    int state = socket.Send(buf);
                //}


                //buf = System.Text.Encoding.Default.GetBytes($"AT+OCCH0=?\r\n");
                //int state = socket.Send(buf);
                //log.Info($"buf:{buf} , state:{state}");

            }
            catch (Exception ex)
            {
                log.Info("控制器连接失败:" + ex.Message);
            }
        }

        private void TCPReceived(object obj)
        {
            string str;
            string stateStr = "0";
            while (true)
            {
                //Socket receiveSocket = obj as Socket;
                Socket receiveSocket = socket;
                try
                {
                    if (!receiveSocket.Connected) return;

                    if (receiveSocket.Available > 0)
                    {
                        int result = receiveSocket.Receive(buffer);
                        if (result == 0)
                        {
                            break;
                        }
                        else
                        {
                            str = System.Text.Encoding.Default.GetString(buffer);

                            //log.Info($"str:{str}");
                            if (string.IsNullOrWhiteSpace(str))
                            {
                                stateStr = "0";
                            }

                            ////"+OCCH1:0\r\n+OCCH2:0\r\n\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0"
                            ////判断2个通道的状态是否都在线，如果其中有一个不在线，那么则认为整体都存在问题。故返回0，否则为3，正常。


                            //if (str.StartsWith("+OCCH"))
                            //{
                            //    if (str.StartsWith("+OCCH1:0"))
                            //    {
                            //        stateStr = "1";
                            //    }
                            //    else if (str.StartsWith("+OCCH2:0"))
                            //    {
                            //        stateStr = "2";
                            //    }
                            //    else
                            //    {
                            //        stateStr = "3";
                            //    }
                            //    //log.Info("SignoState：" + str);

                            //    //防止重复发送消息，影响性能
                            //    if (SignoState != stateStr)
                            //    {
                            //        SignoState = stateStr;
                            //    }

                            //    if (SignoState != stateStr)
                            //    {
                            //        SignoState = stateStr;
                            //    }

                            //}

                            System.Diagnostics.Trace.WriteLine($"raw:{str}");

                            //光栅防作弊状态返回结果处理逻辑
                            if (str.Contains("+OCCH"))
                            {
                                if (str.Contains("+OCCH1:0"))
                                {
                                    stateStr = "1";
                                }
                                else if (str.Contains("+OCCH2:0"))
                                {
                                    stateStr = "2";
                                }
                                else
                                {
                                    stateStr = "";
                                }

                                //防止重复发送消息，影响性能
                                if (SignoState != stateStr)
                                {
                                    SignoState = stateStr;
                                }

                                //DI按钮绑定物资处理逻辑 
                                if (
                                    str.Contains("+OCCH3:") ||
                                     str.Contains("+OCCH4:") ||
                                      str.Contains("+OCCH5:") ||
                                       str.Contains("+OCCH6:") ||
                                        str.Contains("+OCCH7:") ||
                                         str.Contains("+OCCH8:")
                                    )
                                {
                                    stateStr = string.Empty;

                                    if (str.Contains("+OCCH3:1"))
                                    {
                                        stateStr = "3";
                                    }
                                    else if (str.Contains("+OCCH4:1"))
                                    {
                                        stateStr = "4";
                                    }
                                    else if (str.Contains("+OCCH5:1"))
                                    {
                                        stateStr = "5";
                                    }
                                    else if (str.Contains("+OCCH6:1"))
                                    {
                                        stateStr = "6";
                                    }
                                    else if (str.Contains("+OCCH7:1"))
                                    {
                                        stateStr = "7";
                                    }
                                    else if (str.Contains("+OCCH8:1"))
                                    {
                                        stateStr = "8";
                                    }
                                    else
                                    {
                                        stateStr = "0";
                                    }

                                    System.Diagnostics.Trace.WriteLine($"DIState:{DIState}, stateStr:{stateStr}");

                                    if (DIState != stateStr)
                                    {
                                        DIState = stateStr;
                                        log.Info($"DIState:{DIState} , stateStr:{stateStr}");
                                    }

                                }
                            }

                        }
                        Array.Clear(buffer, 0, buffer.Length);
                    }

                }
                catch (Exception ex)
                {
                    log.Info("服务器异常:" + ex.Message);
                }
            }

        }

        private int checksum(byte[] src)
        {
            int sum = 0;
            for (int i = 0; i < src.Length; i++)
            {
                sum += src[i];
            }
            return sum;
        }
        #endregion


        /*
        public object getSHM_LPR() {
            ShareMem MemDB = new ShareMem();
            ShareMem MemDB2 = new ShareMem();
            MemDB.Init("shared", 1024 * 8);
            MemDB2.Init("shared_len", 5 * 8);
            byte[] a = new byte[4];
            MemDB2.Read(ref a, 0, 4);
            int len = System.BitConverter.ToInt32(a, 0);
            byte[] b = new byte[len];
            MemDB.Read(ref b, 0, len);
            object c = MemDB.BytesToStuct(b);
            return c;
        }
        */
        public ShareMem.MAIN_LPR_OVER_WEIGHT_DATA getOverLpr()
        {
            ShareMem MemDB = new ShareMem();
            ShareMem MemDB2 = new ShareMem();
            ShareMem MemDB3 = new ShareMem();
            MemDB.Init("shared_over_mz", 10 * 8);
            MemDB2.Init("shared_over_count", 10 * 8);
            MemDB3.Init("shared_over_axle_num", 10 * 8);
            byte[] mz = new byte[10];
            byte[] overWeight = new byte[10];
            byte[] axleNum = new byte[10];
            MemDB.Read(ref mz, 0, 10);
            MemDB2.Read(ref overWeight, 0, 10);
            MemDB3.Read(ref axleNum, 0, 10);

            ShareMem.MAIN_LPR_OVER_WEIGHT_DATA obj;
            obj.mz = System.Text.Encoding.Default.GetString(mz).TrimEnd();
            obj.axleNum = System.Text.Encoding.Default.GetString(axleNum).TrimEnd();
            obj.overWeightCount = System.Text.Encoding.Default.GetString(overWeight).TrimEnd();
            return obj;
        }

        public ShareMem.MAIN_LPR_MESS getLprMessage()
        {

            byte[] mess1 = new byte[20];
            byte[] mess2 = new byte[20];
            byte[] mess3 = new byte[20];
            byte[] mess4 = new byte[20];
            byte[] mess5 = new byte[20];
            byte[] mess6 = new byte[20];

            try
            {
                ShareMem MemDB1 = new ShareMem();
                ShareMem MemDB2 = new ShareMem();
                ShareMem MemDB3 = new ShareMem();
                ShareMem MemDB4 = new ShareMem();
                ShareMem MemDB5 = new ShareMem();
                ShareMem MemDB6 = new ShareMem();

                MemDB1.Init("mess1", 20 * 8);
                MemDB2.Init("mess2", 20 * 8);
                MemDB3.Init("mess3", 20 * 8);
                MemDB4.Init("mess4", 20 * 8);
                MemDB5.Init("mess5", 20 * 8);
                MemDB6.Init("mess6", 20 * 8);

                MemDB1.Read(ref mess1, 0, 20);
                MemDB2.Read(ref mess2, 0, 20);
                MemDB3.Read(ref mess3, 0, 20);
                MemDB4.Read(ref mess4, 0, 20);
                MemDB5.Read(ref mess5, 0, 20);
                MemDB6.Read(ref mess6, 0, 20);
            }
            catch (Exception)
            {

            }


            ShareMem.MAIN_LPR_MESS obj;
            obj.mess1 = System.Text.Encoding.Default.GetString(mess1).Replace("\0", string.Empty).TrimEnd();
            obj.mess2 = System.Text.Encoding.Default.GetString(mess2).Replace("\0", string.Empty).TrimEnd();
            obj.mess3 = System.Text.Encoding.Default.GetString(mess3).Replace("\0", string.Empty).TrimEnd();
            obj.mess4 = System.Text.Encoding.Default.GetString(mess4).Replace("\0", string.Empty).TrimEnd();
            obj.mess5 = System.Text.Encoding.Default.GetString(mess5).Replace("\0", string.Empty).TrimEnd();
            obj.mess6 = System.Text.Encoding.Default.GetString(mess6).Replace("\0", string.Empty).TrimEnd();

            return obj;
        }


        public string getWeighingComplete()
        {
            ShareMem MemDB = new ShareMem();
            MemDB.Init("shared_weighing_complete", 10 * 8);
            byte[] mzBytes = new byte[10];
            MemDB.Read(ref mzBytes, 0, 10);
            string mz = System.Text.Encoding.Default.GetString(mzBytes).TrimEnd();
            return mz;

        }
        //接收其他APP传来的消息
        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            try
            {
                if (msg.message == UPDATEDEVICE)
                {
                    var sm = new ShareMem();
                    var len = (int)msg.wParam;
                    sm.Init("deviceInfo", len);
                    var bytes = new byte[len];
                    sm.Read(ref bytes, 0, len);
                    var jsonObj = JsonConvert.DeserializeObject<JObject>(Encoding.Default.GetString(bytes));

                    VzClientSDK.VzLPRClient_UpdateNetworkParam(
                        jsonObj["SL"].ToObject<uint>(),
                        jsonObj["SH"].ToObject<uint>(),
                        jsonObj["ip"].ToString(),
                        jsonObj["gateway"].ToString(),
                        jsonObj["netmask"].ToString());
                    return;
                }

                var mainSetting = (AppSettingsSection)ConfigurationManager.OpenExeConfiguration("称重软件3.0.exe")
                            .GetSection("appSettings");

                var keepOpen = (mainSetting.Settings["KeepOpen"]?.Value ?? "false").ToLower().Equals("true");

                if (msg.message == REFRESH_DI_STATE)
                {
                    DIState = "0";
                }

                if (msg.message == CLOSE_APP)
                {
                    if (m_LprMode == "2")
                    {
                        VzClientSDK.VzLPRClient_Cleanup();
                    }
                    Thread.Sleep(100);
                    Current.Shutdown();
                }
                if (msg.message == OVER_WEIGHT)
                {
                    if (!_isVZSDKSetup)
                    {
                        VzClientSDK.VzLPRClient_Setup();
                        _isVZSDKSetup = true;
                    }
                    int[] tmp_handle = Login();
                    ShareMem.MAIN_LPR_OVER_WEIGHT_DATA obj = getOverLpr();
                    Configuration AWSConfig = ConfigurationManager.OpenExeConfiguration("TruckScale.exe");
                    AppSettingsSection AWSSection = (AppSettingsSection)AWSConfig.GetSection("appSettings");
                    string unit = AWSSection.Settings["WeighingUnit"].Value.ToUpper();

                    log.Info($"obj.overWeightCount:{obj.overWeightCount}");

                    if (ConfigurationManager.AppSettings["CameraLEDMode"] == "0")
                    {
                        string mz = string.IsNullOrWhiteSpace(obj.mz) ? "" : obj.mz;
                        string overWeightCount = string.IsNullOrWhiteSpace(obj.overWeightCount) ? "" : obj.overWeightCount;
                        string axleNum = string.IsNullOrWhiteSpace(obj.axleNum) ? "" : obj.axleNum;

                        if (double.Parse(obj.overWeightCount) > 0)
                        {
                            if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[2], "车号:" + m_PlateNo, "轴数:" + axleNum, "重量:" + mz + unit, "超载:" + overWeightCount + unit, "", "r");
                            if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[3], "车号:" + m_PlateNo, "轴数:" + axleNum, "重量:" + mz + unit, "超载" + overWeightCount + unit, "", "r");
                        }
                        else
                        {
                            if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[2], "车号:" + m_PlateNo, "轴数:" + axleNum, "重量:" + mz + "T", "超载:" + overWeightCount + "T", "", "g");
                            if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[3], "车号:" + m_PlateNo, "轴数:" + axleNum, "重量:" + mz + "T", "超载" + overWeightCount + "T", "", "g");
                        }
                    }
                }
                if (msg.message == VIOLATE_WHITE_LIST)
                {
                    if (!_isVZSDKSetup)
                    {
                        VzClientSDK.VzLPRClient_Setup();
                        _isVZSDKSetup = true;
                    }
                    int[] tmp_handle = Login();
                    ShareMem.MAIN_LPR_MESS obj = getLprMessage();
                    if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1")
                    {
                        SendMsgToCameraLED(tmp_handle[2], obj.mess1, obj.mess2, obj.mess3, obj.mess4, "", "g");
                        SwitchLight(tmp_handle[2], false);
                    }
                    if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1")
                    {
                        SendMsgToCameraLED(tmp_handle[3], obj.mess1, obj.mess2, obj.mess3, obj.mess4, "", "g");
                        SwitchLight(tmp_handle[3], false);
                    }
                }
                if (msg.message == WEIGHING_COMPLETE)
                {
                    /*
                    VzClientSDK.VzLPRClient_Setup();
                    int[] tmp_handle = Login();
                    string mz = System.Text.Encoding.Default.GetString(mzBytes).TrimEnd();
                    // 朝内模式
                    if (ConfigurationManager.AppSettings["CameraLEDMode"] == "0") {
                        if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[2], m_PlateNo, "重量:" + mz, "缓慢下磅", "注意安全", "", "r");
                        if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[3], m_PlateNo, "重量:" + mz, "缓慢下磅", "注意安全", "", "g");
                    }
                    */
                    if (!_isVZSDKSetup)
                    {
                        VzClientSDK.VzLPRClient_Setup();
                        _isVZSDKSetup = true;
                    }
                    int[] tmp_handle = Login();
                    ShareMem.MAIN_LPR_MESS obj = getLprMessage();
                    if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1")
                    {
                        SendMsgToCameraLED(tmp_handle[2], obj.mess1, obj.mess2, obj.mess3, obj.mess4, "", "g");
                        SwitchLight(tmp_handle[2], false);
                    }
                    if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1")
                    {
                        SendMsgToCameraLED(tmp_handle[3], obj.mess1, obj.mess2, obj.mess3, obj.mess4, "", "g");
                        SwitchLight(tmp_handle[3], false);
                    }
                }
                if (msg.message == READY_TO_WEIGH)
                {
                    if (!_isVZSDKSetup)
                    {
                        VzClientSDK.VzLPRClient_Setup();
                        _isVZSDKSetup = true;
                    }
                    int[] tmp_handle = Login();
                    //主程序称重完成后，发过来继续进行车牌识别的命令
                    log.Info("收到称重软件3.0命令：准备好称重，继续识别车牌");
                    InWeighing = false;
                    log.Info($"READY_TO_WEIGH - InWeighing：{InWeighing}");

                    m_PlateNo = "";
                    //朝外模式-双向
                    if (ConfigurationManager.AppSettings["CameraLEDMode"] == "1")
                    {
                        //ShareMem.MAIN_LPR_MESS obj = getLprMessage();
                        if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1")
                        {
                            SendMsgToCameraLED(tmp_handle[2], "称重软件3.0", "车牌识别", "减速慢行", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "r");
                            //2023-01-03注释，此句更换成上面那一句
                            //SendMsgToCameraLED(tmp_handle[2], "准备就绪", "自动识别", "缓慢行驶", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "r");

                            //if (!string.IsNullOrWhiteSpace(obj.mess5))
                            {
                                SwitchLight(tmp_handle[2], false);
                            }
                        }
                        if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1")
                        {
                            SendMsgToCameraLED(tmp_handle[3], "称重软件3.0", "车牌识别", "减速慢行", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "g");
                            //2023-01-03注释，此句更换成上面那一句
                            //SendMsgToCameraLED(tmp_handle[3], "准备就绪", "自动识别", "缓慢行驶", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "g");

                            //if (!string.IsNullOrWhiteSpace(obj.mess5))
                            {
                                SwitchLight(tmp_handle[3], false);
                            }
                        }
                        if (handle1 != 0 && m_DevNo == "A") VzClientSDK.VzLPRClient_ForceTrigger(handle1);
                        if (handle2 != 0 && m_DevNo == "B") VzClientSDK.VzLPRClient_ForceTrigger(handle2);
                    }
                    else
                    {
                        ShareMem.MAIN_LPR_MESS obj = getLprMessage();
                        if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1")
                        {
                            SendMsgToCameraLED(tmp_handle[2], "称重软件3.0", "全程抓拍", "自动称重", "缓慢上磅", "", "r");
                            if (!string.IsNullOrWhiteSpace(obj.mess5))
                            {
                                SwitchLight(tmp_handle[2]);
                            }
                        }

                        if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1")
                        {
                            SendMsgToCameraLED(tmp_handle[3], "称重软件3.0", "全程抓拍", "自动称重", "缓慢上磅", "", "g");
                            if (!string.IsNullOrWhiteSpace(obj.mess5))
                            {
                                SwitchLight(tmp_handle[3]);
                            }
                        }
                        if (handle1 != 0 && m_DevNo == "A") VzClientSDK.VzLPRClient_ForceTrigger(handle1);
                        if (handle2 != 0 && m_DevNo == "B") VzClientSDK.VzLPRClient_ForceTrigger(handle2);
                    }
                    //SendMsgToApp("车牌识别", 0x08F0);
                }
                if (msg.message == GET_PLATE)
                {
                    if (!_isVZSDKSetup)
                    {
                        VzClientSDK.VzLPRClient_Setup();
                        _isVZSDKSetup = true;
                    }
                    int[] tmp_handle = Login();
                    //朝外模式-双向
                    if (ConfigurationManager.AppSettings["CameraLEDMode"] == "1")
                    {
                        ShareMem.MAIN_LPR_MESS obj = getLprMessage();
                        var plantNo = m_PlateNo == "" ? "无牌车" : m_PlateNo;
                        string rgb = string.IsNullOrWhiteSpace(obj.mess5) ? "g" : obj.mess5;
                        if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[2], plantNo, "缓慢上磅", "秩序过磅", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "g");
                        if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[3], plantNo, "缓慢上磅", "秩序过磅", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "g");
                    }
                    else
                    {
                        ShareMem.MAIN_LPR_MESS obj = getLprMessage();
                        string rgb = string.IsNullOrWhiteSpace(obj.mess5) ? "g" : obj.mess5;
                        if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[2], obj.mess1, obj.mess2, obj.mess3, obj.mess4, "", "r");
                        if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1") SendMsgToCameraLED(tmp_handle[3], obj.mess1, obj.mess2, obj.mess3, obj.mess4, "", "r");
                    }
                }
                if (msg.message == IS_WEIGHING)
                {
                    if (!_isVZSDKSetup)
                    {
                        VzClientSDK.VzLPRClient_Setup();
                        _isVZSDKSetup = true;
                    }
                    int[] tmp_handle = Login();
                    //主程序接收到车牌信息后，发过来不再进行车牌识别的命令
                    log.Info("收到称重软件3.0命令：正在称重，停止识别车牌");

                    //朝外模式-双向
                    if (ConfigurationManager.AppSettings["CameraLEDMode"] == "1")
                    {
                        InWeighing = true;//双向模式下，大于最小重量了就不能再识别车牌了
                        var plantNo = m_PlateNo == "" ? "无牌车" : m_PlateNo;
                        if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1")
                        {
                            SendMsgToCameraLED(tmp_handle[2], plantNo, "正在称重", "后车等待", "秩序上磅", "", "r");
                        }
                        if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1")
                        {
                            SendMsgToCameraLED(tmp_handle[3], plantNo, "正在称重", "后车等待", "秩序上磅", "", "r");
                        }
                    }
                    else
                    {
                        InWeighing = false;//单向模式下 不管重量如何都可以随时识别车牌，所以为flase
                        var plantNo = m_PlateNo == "" ? "无牌车" : m_PlateNo;
                        if (string.IsNullOrWhiteSpace(m_PlateNo))
                        {
                            ShareMem.MAIN_LPR_MESS obj = getLprMessage();
                            string rgb = string.IsNullOrWhiteSpace(obj.mess5) ? "g" : obj.mess5;
                            if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1")
                            {
                                SendMsgToCameraLED(tmp_handle[2], obj.mess1, obj.mess2, obj.mess3, obj.mess4, "", "g");
                                if (!string.IsNullOrWhiteSpace(obj.mess5))
                                {
                                    SwitchLight(tmp_handle[2]);
                                }
                            }
                            if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1")
                            {
                                SendMsgToCameraLED(tmp_handle[3], obj.mess1, obj.mess2, obj.mess3, obj.mess4, "", "r");
                                if (!string.IsNullOrWhiteSpace(obj.mess5))
                                {
                                    SwitchLight(tmp_handle[3]);
                                }
                            }
                        }
                    }
                    log.Info($"IS_WEIGHING - InWeighing：{InWeighing}");
                }
                if (msg.message >= RELAY_MIN && msg.message <= RELAY_MAX)
                {
                    if (m_RelayPort.IsOpen)
                    {
                        var bit = msg.message & 0x0F;

                        if (keepOpen)
                        {
                            m_DOCmd[7] = ((byte)(3));//全部抬起
                        }
                        else
                        {
                            m_DOCmd[7] = ((byte)(bit == 13 ? 1 : bit == 14 ? 2 : bit));
                        }


                        CRCCalc(m_DOCmd.Take(8).ToArray());
                        m_DOTimer.Start();
                    }


                    if (ConfigurationManager.AppSettings["Relay2Enable"] == "1")
                    {
                        int i = msg.message & 0x0F;
                        byte[] buf = new byte[] { };

                        if (keepOpen)
                        {
                            i = 3;//全部抬起
                        }

                        if (!socket.Connected)
                        {
                            socket.Close();
                            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            socket.Connect(new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["Relay2IP"]), 6000));
                            log.Info("控制器重新连接:" + socket.Connected);
                        }


                        switch (i)
                        {
                            case 0:
                                //F4 全部关闭道闸
                                try
                                {
                                    if (keepOpen)
                                    {
                                        return;
                                    }
                                    //道闸全落
                                    buf = System.Text.Encoding.Default.GetBytes("AT+STACH1=0\r\nAT+STACH2=0\r\nAT+STACH3=0\r\nAT+STACH4=0\r\n");
                                    socket.Send(buf);

                                    ////道闸全落
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH1=0\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=0\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);

                                    ////红绿灯全部反转，绿变红或者红变绿
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH3=0\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH4=0\r\n");
                                    //socket.Send(buf);

                                }
                                catch (Exception ex)
                                {
                                    log.Info("控制器连接失败:" + ex.Message);
                                }
                                break;
                            case 1://道闸A开 ，道闸B关
                                try
                                {
                                    buf = System.Text.Encoding.Default.GetBytes("AT+STACH1=1\r\n");
                                    socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=0\r\n");
                                    //socket.Send(buf);
                                }
                                catch (Exception ex)
                                {
                                    log.Info("控制器连接失败:" + ex.Message);
                                }
                                break;
                            case 2://道闸B开 ，道闸A关
                                try
                                {
                                    buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=1\r\n");
                                    socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH1=0\r\n");
                                    //socket.Send(buf);
                                }
                                catch (Exception ex)
                                {
                                    log.Info("控制器连接失败:" + ex.Message);
                                }
                                break;
                            case 3://F3 全部开启道闸
                                try
                                {

                                    //LightType= 0：称重完成亮绿灯 ，1：车下磅后绿灯。 发送红绿灯指令给LPR
                                    var lightType = Common.Utility.SettingsHelper.AWSV2Settings.Settings["LightType"].Value;

                                    if (lightType == "0")
                                    {
                                        //红绿灯全部反转，绿变红或者红变绿
                                        buf = System.Text.Encoding.Default.GetBytes("AT+STACH3=0\r\nAT+STACH4=0\r\nAT+STACH1=1\r\nAT+STACH2=1\r\n");
                                        socket.Send(buf);
                                    }
                                    else
                                    {
                                        //当LightType!=0时 不执行灯的颜色改变命令，由最小重量那边处理
                                        buf = System.Text.Encoding.Default.GetBytes("AT+STACH1=1\r\nAT+STACH2=1\r\n");
                                        socket.Send(buf);
                                    }


                                    ////红绿灯全部反转，绿变红或者红变绿
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH3=0\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH4=0\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH1=1\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=1\r\n");
                                    //socket.Send(buf);
                                }
                                catch (Exception ex)
                                {
                                    log.Info("控制器连接失败:" + ex.Message);
                                }
                                break;
                            case 12:
                                try
                                {
                                    //红绿灯全部反转，绿变红或者红变绿
                                    buf = System.Text.Encoding.Default.GetBytes("AT+STACH3=1\r\nAT+STACH4=1\r\nAT+STACH1=0\r\nAT+STACH2=0\r\n");
                                    socket.Send(buf);

                                    ////红绿灯全部反转，绿变红或者红变绿
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH3=1\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH4=1\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH1=0\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=0\r\n");
                                    //socket.Send(buf);

                                }
                                catch (Exception ex)
                                {
                                    log.Info("控制器连接失败:" + ex.Message);
                                }
                                break;
                            case 13:
                                try
                                {
                                    var lightType = Common.Utility.SettingsHelper.AWSV2Settings.Settings["LightType"].Value;
                                    buf = System.Text.Encoding.Default.GetBytes($"AT+STACH1=1\r\nAT+STACH3={lightType}\r\nAT+STACH4={lightType}\r\n");
                                    socket.Send(buf);

                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH1=1\r\n");
                                    //socket.Send(buf);
                                    ////红绿灯全部反转，绿变红或者红变绿
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH3=0\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH4=0\r\n");
                                }
                                catch (Exception ex)
                                {
                                    log.Info("控制器连接失败:" + ex.Message);
                                }
                                break;
                            case 14:
                                try
                                {
                                    var lightType = Common.Utility.SettingsHelper.AWSV2Settings.Settings["LightType"].Value;
                                    buf = System.Text.Encoding.Default.GetBytes($"AT+STACH2=1\r\nAT+STACH3={lightType}\r\nAT+STACH4={lightType}\r\n");
                                    socket.Send(buf);

                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH2=1\r\n");
                                    //socket.Send(buf);
                                    ////红绿灯全部反转，绿变红或者红变绿
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH3=0\r\n");
                                    //socket.Send(buf);
                                    ////Thread.Sleep(20);
                                    //buf = System.Text.Encoding.Default.GetBytes("AT+STACH4=0\r\n");
                                }
                                catch (Exception ex)
                                {
                                    log.Info("控制器连接失败:" + ex.Message);
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    if (ConfigurationManager.AppSettings["Plate2Enable"] == "1")
                    {
                        //
                        var msgHex = msg.message.ToString("X");
                        int i = msg.message & 0x0F;
                        if (!_isVZSDKSetup)
                        {
                            VzClientSDK.VzLPRClient_Setup();
                            _isVZSDKSetup = true;
                        }
                        int[] tmp_handle = Login();
                        switch (i)
                        {
                            // 全落
                            case 0:
                            case 12:
                                log.Info($"{msgHex} 第一个 入参:1,1,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 1, 1));
                                Thread.Sleep(500);
                                log.Info($"{msgHex} 第二个 入参:1,1,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 1, 1));
                                Thread.Sleep(500);
                                log.Info($"{msgHex} 第一个 入参:1,0,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 1, 0));
                                Thread.Sleep(500);
                                log.Info($"{msgHex} 第二个 入参:1,0,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 1, 0));
                                // 告知主程序道闸全落了 --阿吉 2024年03月29日 16:05:17
                                var msg12 = JObject.FromObject(new AJGateStatusNotifyModel
                                {
                                    Id = null,
                                    Open = false
                                }).ToString(Formatting.None);
                                SendMessageToApp("称重软件3.0", $"AJGateStatus,{msg12.Replace(",", "|")}");
                                break;
                            // 抬一
                            case 1:
                            case 13:
                                log.Info($"{msgHex} 第一个 入参:0,1,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 0, 1));
                                Thread.Sleep(500);
                                log.Info($"{msgHex}  第一个 入参:0,0,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 0, 0));
                                //VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 0, 0);
                                // 告知主程序道闸A开了 --阿吉 2024年03月29日 16:05:17
                                var msg1Or13 = JObject.FromObject(new AJGateStatusNotifyModel
                                {
                                    Id = 1,
                                    Open = true
                                }).ToString(Formatting.None);
                                SendMessageToApp("称重软件3.0", $"AJGateStatus,{msg1Or13.Replace(",", "|")}");
                                break;
                            // 抬二
                            case 2:
                            case 14:
                                log.Info($"{msgHex} 第二个 入参:0,1,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 0, 1));
                                //VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 0, 1);
                                Thread.Sleep(500);
                                log.Info($"{msgHex} 第二个 入参:0,0,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 0, 0));
                                // 告知主程序道闸B开了 --阿吉 2024年03月29日 16:05:17
                                var msg2Or14 = JObject.FromObject(new AJGateStatusNotifyModel
                                {
                                    Id = 2,
                                    Open = true
                                }).ToString(Formatting.None);
                                SendMessageToApp("称重软件3.0", $"AJGateStatus,{msg2Or14.Replace(",", "|")}");
                                break;
                            // 关一
                            case 4:
                                log.Info($"{msgHex} 第一个 入参：1,1,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 1, 1));
                                Thread.Sleep(500);
                                log.Info($"{msgHex}  第一个 入参:1,0,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 1, 0));
                                //VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 0, 0);
                                break;
                            // 关二
                            case 5:
                                log.Info($"{msgHex} 第二个 入参：1,1,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 1, 1));
                                //VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 0, 1);
                                Thread.Sleep(500);
                                log.Info($"{msgHex} 第二个 入参:1,0,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 1, 0));
                                break;
                            // 全抬
                            case 3:
                                log.Info($"{msgHex} 第一个 入参:0,1,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 0, 1));
                                Thread.Sleep(500);
                                log.Info($"{msgHex} 第二个 入参:0,1,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 0, 1));
                                Thread.Sleep(500);
                                log.Info($"{msgHex} 第一个 入参:0,0,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[0], 0, 0));
                                Thread.Sleep(500);
                                log.Info($"{msgHex} 第二个 入参:0,0,出参:" + VzClientSDK.VzLPRClient_SetIOOutput(tmp_handle[1], 0, 0));
                                // 告知主程序道闸全开了 --阿吉 2024年03月29日 16:05:17
                                var msg3 = JObject.FromObject(new AJGateStatusNotifyModel
                                {
                                    Id = null,
                                    Open = true
                                }).ToString(Formatting.None);
                                SendMessageToApp("称重软件3.0", $"AJGateStatus,{msg3.Replace(",", "|")}");
                                break;
                            case 10:
                                // 朝外模式
                                //if(ConfigurationManager.AppSettings["CameraLEDMode"] == "1")
                                //{
                                //    if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1") SendMsgToCameraLED(serial_handle1, m_PlateNo, "等待称重", "缓慢上磅", "注意安全", "", "g");
                                //    if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1") SendMsgToCameraLED(serial_handle2, m_PlateNo, "等待称重", "缓慢上磅", "注意安全", "", "r");
                                //}
                                // 朝内模式
                                //else
                                // {
                                //     if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1") SendMsgToCameraLED(serial_handle1, m_PlateNo, "等待称重", "缓慢上磅", "注意安全", "", "g");
                                //     if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1") SendMsgToCameraLED(serial_handle2, m_PlateNo, "等待称重", "缓慢上磅", "注意安全", "", "r");
                                //}
                                break;
                            default:
                                break;
                        }
                    }

                    if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1")
                    {

                        if (m_PlateNo != "")
                        {
                            if (m_DevNo == "A")
                            {
                                // 朝外模式
                                //if(ConfigurationManager.AppSettings["CameraLEDMode"] == "1")
                                //{
                                //    SendMsgToCameraLED(serial_handle1, m_PlateNo, "等待称重", "缓慢上磅", "注意安全", "", "g");
                                //     if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1") SendMsgToCameraLED(serial_handle2, m_PlateNo, "等待称重", "缓慢上磅", "注意安全", "", "r");
                                // }
                                // 朝内模式
                                //else
                                // {
                                //    SendMsgToCameraLED(serial_handle1, m_PlateNo, "缓慢移动", "居中停稳", "正在称重", "", "g");
                                //    if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1") SendMsgToCameraLED(serial_handle2, m_PlateNo, "缓慢移动", "居中停稳", "正在称重", "", "r");
                                //}
                            }
                        }
                    }

                    if (ConfigurationManager.AppSettings["Camera2LEDEnable"] == "1")
                    {

                        if (m_PlateNo != "")
                        {
                            if (m_DevNo == "B")
                            {

                                // 朝外模式
                                //if(ConfigurationManager.AppSettings["CameraLEDMode"] == "1")
                                //{
                                //    SendMsgToCameraLED(serial_handle2, m_PlateNo, "等待称重", "缓慢上磅", "注意安全", "", "g");
                                //    if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1") SendMsgToCameraLED(serial_handle1, m_PlateNo, "等待称重", "缓慢上磅", "注意安全", "", "r");
                                //}
                                // 朝内模式
                                //else
                                //{
                                //    SendMsgToCameraLED(serial_handle2, m_PlateNo, "缓慢移动", "居中停稳", "正在称重", "", "g");
                                //    if (ConfigurationManager.AppSettings["Camera1LEDEnable"] == "1") SendMsgToCameraLED(serial_handle1, m_PlateNo, "缓慢移动", "居中停稳", "正在称重", "", "r");
                                //}
                            }
                        }
                    }
                }
                if (msg.message == GREENON)
                {
                    if (m_RelayPort.IsOpen)
                    {
                        //m_DOCmd[7] = (byte)(0x08F0 & 0x0F);
                        m_DOCmd[7] = (byte)(1);
                        CRCCalc(m_DOCmd.Take(8).ToArray());
                        //m_RelayPort.Write(m_DOCmd, 0, m_DOCmd.Length);
                        m_DOTimer.Start();
                    }
                }
                if (msg.message == REDON)
                {
                    if (m_RelayPort.IsOpen)
                    {
                        //m_DOCmd[7] = (byte)(0x08FC & 0x0F);
                        CRCCalc(m_DOCmd.Take(8).ToArray());
                        m_RelayPort.Write(m_DOCmd, 0, m_DOCmd.Length);
                        //m_DOTimer.Start();
                    }
                }


            }
            catch (Exception e)
            {
                log.Info($"ComponentDispatcher_ThreadPreprocessMessage Error:{e.Message}");
            }
            // 不能标记ture ，会影响到道闸继电器的定时器运行
            //handled = true;
        }

        //给其他APP发送消息
        private void SendMessageToApp(string appName, string msg)
        {
            Process[] procs = Process.GetProcesses();

            foreach (Process p in procs)
            {
                if (p.ProcessName.Equals(appName))
                {
                    // 获取目标进程句柄
                    IntPtr hWnd = p.MainWindowHandle;

                    // 封装消息
                    byte[] sarr = Encoding.Default.GetBytes(msg);
                    int len = sarr.Length;
                    IPCHelper.COPYDATASTRUCT cds;
                    cds.dwData = (IntPtr)0;
                    cds.cbData = len + 1;
                    cds.lpData = msg;

                    // 发送消息
                    IPCHelper.SendMessage(hWnd, IPCHelper.WM_COPYDATA, IntPtr.Zero, ref cds);
                }
            }
        }

        private void SendMsgToApp(string appName, int msg)
        {
            Process[] proc = Process.GetProcessesByName(appName);

            if (proc.Length > 0)
            {
                int threadID = Process.GetProcessById(proc[0].Id).Threads[0].Id;
                IPCHelper.PostThreadMessage(threadID, msg, IntPtr.Zero, IntPtr.Zero);
            }
        }



        //CRC校验
        private void CRCCalc(byte[] data)
        {
            byte[] crcbuf = data;
            //计算并填写CRC校验码
            int crc = 0xffff;
            int len = crcbuf.Length;
            for (int n = 0; n < len; n++)
            {
                byte i;
                crc = crc ^ crcbuf[n];
                for (i = 0; i < 8; i++)
                {
                    int TT;
                    TT = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (TT == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }

            }
            m_DOCmd[8] = (byte)(crc & 0xff);
            m_DOCmd[9] = (byte)((crc >> 8) & 0xff);
        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 4字节转成整型数(低位前, 高位后)
        /// </summary>
        /// <param name="buff">字节数组</param>
        /// <param name="start">起始索引位(从0开始计)</param>
        /// <param name="len">长度</param>
        /// <returns>整型数</returns>
        private long ByteToLong(byte[] buff, int start, int len)
        {
            long val = 0;
            for (int i = 0; i < len && i < 4; i++)
            {
                long lng = buff[i + start];
                val += (lng << (8 * i));
            }
            return val;
        }
    }
}

