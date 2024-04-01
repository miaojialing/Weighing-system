using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using log4net;
using Stylet;
using static System.Windows.Forms.AxHost;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Monitor
{
    public class ShellViewModel : Screen
    {
        public double _Width = 310;
        public double Width
        {
            get
            {
                return _Width;
            }
            set
            {
                if (value > 310)
                    _Width = value;
            }
        }

        public double _Heigh = 620;
        public double Heigh
        {
            get
            {
                return _Heigh;
            }
            set
            {
                if (value > 620)
                    _Heigh = value;
            }
        }

        public bool WindowTopmost { get; set; } = false;

        private string NormalPic = "/Resources/connected.png";
        private string AbnormalPic = "/Resources/ununited.png";
        private string _FixedPic = "/Resources/fixed.png";
        private string _UnfixedPic = "/Resources/unfixed.png";

        public bool ShowCamera01 { get; set; } = true;
        public bool ShowCamera02 { get; set; } = true;
        public bool ShowCamera03 { get; set; } = true;
        public bool ShowCamera04 { get; set; } = true;
        public string StatusBar { get; set; }
        public bool Camera1Enable { get; set; }
        public bool Camera2Enable { get; set; }
        public bool Camera3Enable { get; set; }
        public bool Camera4Enable { get; set; }

        private int _Camera1Connected;
        private int _Camera2Connected;
        private int _Camera3Connected;
        private int _Camera4Connected;

        public GridLength Row1High { get; set; } = new GridLength(1.0, GridUnitType.Star);
        public GridLength Row2High { get; set; } = new GridLength(1.0, GridUnitType.Star);
        public GridLength Row3High { get; set; } = new GridLength(1.0, GridUnitType.Star);
        public GridLength Row4High { get; set; } = new GridLength(1.0, GridUnitType.Star);

        public int Camera1Connected
        {
            get
            {
                return _Camera1Connected;
            }
            set
            {
                _Camera1Connected = value;
                if (value == 99999)
                {
                    Camera1Pic = NormalPic;
                    Camera1Tips = string.Empty;
                }
                else
                {
                    Camera1Pic = AbnormalPic;
                    Camera1Tips = value.ToString();
                }
            }

        }

        public int Camera2Connected
        {
            get
            {
                return _Camera2Connected;
            }
            set
            {
                _Camera2Connected = value;
                if (value == 99999)
                {
                    Camera2Pic = NormalPic;
                    Camera2Tips = string.Empty;
                }
                else
                {
                    Camera2Pic = AbnormalPic;
                    Camera2Tips = value.ToString();
                }
            }

        }

        public int Camera3Connected
        {
            get
            {
                return _Camera3Connected;
            }
            set
            {
                _Camera3Connected = value;
                if (value == 99999)
                {
                    Camera3Pic = NormalPic;
                    Camera3Tips = string.Empty;
                }
                else
                {
                    Camera3Pic = AbnormalPic;
                    Camera3Tips = value.ToString();
                }
            }

        }

        public int Camera4Connected
        {
            get
            {
                return _Camera4Connected;
            }
            set
            {
                _Camera4Connected = value;
                if (value == 99999)
                {
                    Camera4Pic = NormalPic;
                    Camera4Tips = string.Empty;
                }
                else
                {
                    Camera4Pic = AbnormalPic;
                    Camera4Tips = value.ToString();
                }
            }

        }
        public string FixedPic { get; set; }
        public string Camera1Pic { get; set; }
        public string Camera2Pic { get; set; }
        public string Camera3Pic { get; set; }
        public string Camera4Pic { get; set; }

        public string Camera1Tips { get; set; }
        public string Camera2Tips { get; set; }
        public string Camera3Tips { get; set; }
        public string Camera4Tips { get; set; }

        private static readonly ILog log = LogManager.GetLogger("Monitor");

        public ShellViewModel()
        {
            FixedPic = _UnfixedPic;

            _Heigh = double.Parse(ConfigurationManager.AppSettings["Heigh"]);
            _Width = double.Parse(ConfigurationManager.AppSettings["Width"]);

            if (ConfigurationManager.AppSettings["Monitor1Enable"] == "1") Camera1Enable = true;
            if (ConfigurationManager.AppSettings["Monitor2Enable"] == "1") Camera2Enable = true;
            if (ConfigurationManager.AppSettings["Monitor3Enable"] == "1") Camera3Enable = true;
            if (ConfigurationManager.AppSettings["Monitor4Enable"] == "1") Camera4Enable = true;

            CHCNetSDK.NET_DVR_Init();

            if (Camera1Enable)
            {
                var uname = ConfigurationManager.AppSettings["Monitor1Username"];
                var pwd = ConfigurationManager.AppSettings["Monitor1Password"];
                Camera1Connected = MonitorLogin(ref Globalspace._UserID1, ConfigurationManager.AppSettings["Monitor1IP"], uname, pwd);

            }
            else
            {
                Row1High = new GridLength(1.0, GridUnitType.Auto);
                ShowCamera01 = false;
            }
            if (Camera2Enable)
            {
                var uname = ConfigurationManager.AppSettings["Monitor2Username"];
                var pwd = ConfigurationManager.AppSettings["Monitor2Password"];
                Camera2Connected = MonitorLogin(ref Globalspace._UserID2, ConfigurationManager.AppSettings["Monitor2IP"], uname, pwd);

            }
            else
            {
                Row2High = new GridLength(1.0, GridUnitType.Auto);
                ShowCamera02 = false;
            }

            if (Camera3Enable)
            {
                var uname = ConfigurationManager.AppSettings["Monitor3Username"];
                var pwd = ConfigurationManager.AppSettings["Monitor3Password"];
                Camera3Connected = MonitorLogin(ref Globalspace._UserID3, ConfigurationManager.AppSettings["Monitor3IP"], uname, pwd);

            }
            else
            {
                Row3High = new GridLength(1.0, GridUnitType.Auto);
                ShowCamera03 = false;
            }
            if (Camera4Enable)
            {
                var uname = ConfigurationManager.AppSettings["Monitor4Username"];
                var pwd = ConfigurationManager.AppSettings["Monitor4Password"];
                Camera4Connected = MonitorLogin(ref Globalspace._UserID4, ConfigurationManager.AppSettings["Monitor4IP"], uname, pwd);

            }
            else
            {
                Row4High = new GridLength(1.0, GridUnitType.Auto);
                ShowCamera04 = false;
            }

            Task.Run(async delegate
            {
                await Task.Delay(600);
                //if (Camera1Enable) SetChanNameOSD(Globalspace._UserID1);
                //if (Camera2Enable) SetChanNameOSD(Globalspace._UserID2);
                //if (Camera3Enable) SetChanNameOSD(Globalspace._UserID3);
                //if (Camera4Enable) SetChanNameOSD(Globalspace._UserID4);
                if (Camera1Enable) MonitorPreview(Globalspace._UserID1, Globalspace._Wnd1Handle);
                if (Camera2Enable) MonitorPreview(Globalspace._UserID2, Globalspace._Wnd2Handle);
                if (Camera3Enable) MonitorPreview(Globalspace._UserID3, Globalspace._Wnd3Handle);
                if (Camera4Enable) MonitorPreview(Globalspace._UserID4, Globalspace._Wnd4Handle);
            });

            var delayRunTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(2500) };
            delayRunTimer.Start();
            delayRunTimer.Tick += (sender, args) =>
            {
                delayRunTimer.Stop();
                Preview();
            };
        }

        private int MonitorLogin(ref int userID, string IP, string uname, string pwd)
        {
            if (userID < 0)
            {
                string DVRIPAddress = IP;//设备IP地址或者域名
                short DVRPortNumber = 8000;//设备服务端口号
                                           //string DVRUserName = "admin";//设备登录用户名
                                           //string DVRPassword = "Abc123456";//设备登录密码
                string DVRUserName = uname;//设备登录用户名
                string DVRPassword = pwd;//设备登录密码
                log.Info($"{IP}--{uname}--{pwd}--CHANGSHI登录成功");
                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                userID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress,
                    DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);

                if (userID < 0)
                {
                    var state = CHCNetSDK.NET_DVR_GetLastError();
                    StatusBar += "摄像头登录失败：" + DVRIPAddress + " 错误代码：" + state;
                    return (int)state;
                }
                else
                {
                    log.Info(DVRIPAddress + "登录成功");
                }
            }
            return 99999;
        }

        private CHCNetSDK.NET_DVR_PICCFG_V40 m_struPicCfgV40;

        private void SetChanNameOSD(int userID)
        {
            uint dwReturn = 0;
            int nSize = Marshal.SizeOf(m_struPicCfgV40);
            IntPtr ptrPicCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(m_struPicCfgV40, ptrPicCfg, false);
            if (CHCNetSDK.NET_DVR_GetDVRConfig(userID, CHCNetSDK.NET_DVR_GET_PICCFG_V40, 1, ptrPicCfg, (uint)nSize, ref dwReturn))
            {
                m_struPicCfgV40 = (CHCNetSDK.NET_DVR_PICCFG_V40)Marshal.PtrToStructure(ptrPicCfg, typeof(CHCNetSDK.NET_DVR_PICCFG_V40));

                byte[] byName = System.Text.Encoding.Default.GetBytes("Camera0" + (userID + 1).ToString());
                m_struPicCfgV40.sChanName = new byte[32];
                byName.CopyTo(m_struPicCfgV40.sChanName, 0);

                m_struPicCfgV40.wShowNameTopLeftX = ushort.Parse("620");
                m_struPicCfgV40.wShowNameTopLeftY = ushort.Parse("540");

                m_struPicCfgV40.wOSDTopLeftX = ushort.Parse("5");
                m_struPicCfgV40.wOSDTopLeftY = ushort.Parse("32");

                nSize = Marshal.SizeOf(m_struPicCfgV40);
                ptrPicCfg = Marshal.AllocHGlobal(nSize);
                Marshal.StructureToPtr(m_struPicCfgV40, ptrPicCfg, false);

                bool ret = CHCNetSDK.NET_DVR_SetDVRConfig(userID, CHCNetSDK.NET_DVR_SET_PICCFG_V40, 1, ptrPicCfg, (uint)nSize);

                log.Info("OSD设置为" + System.Text.Encoding.UTF8.GetString(byName) + ",返回值：" + ret);
            }
            else
            {
                log.Info(CHCNetSDK.NET_DVR_GetLastError());
            }

            Marshal.FreeHGlobal(ptrPicCfg);

        }

        private void MonitorPreview(int userID, IntPtr handle)
        {
            if (userID > -1)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO
                {
                    hPlayWnd = handle,//预览窗口
                    lChannel = 1,//预te览的设备通道
                    dwStreamType = 1,//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    dwLinkMode = 0,//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    bBlocked = true, //0- 非阻塞取流，1- 阻塞取流
                    dwDisplayBufNum = 1, //播放库播放缓冲区最大缓冲帧数
                    byProtoType = 0,
                    byPreviewMode = 0
                };

                IntPtr pUser = new IntPtr();//用户数据

                int ret = CHCNetSDK.NET_DVR_RealPlay_V40(userID, ref lpPreviewInfo, null/*RealData*/, pUser);

                log.Info(userID + "预览画面返回值：" + ret);
            }
        }

        public void Preview()
        {
            if (Globalspace._UserID1 > -1)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = Globalspace._Wnd1Handle;//预览窗口
                lpPreviewInfo.lChannel = 1;//预te览的设备通道
                lpPreviewInfo.dwStreamType = 1;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;


                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                var ret = CHCNetSDK.NET_DVR_RealPlay_V40(Globalspace._UserID1, ref lpPreviewInfo, null/*RealData*/, pUser);
                log.Info(Globalspace._UserID1 + "预览画面返回值：" + ret);
            }

            if (Globalspace._UserID2 > -1)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = Globalspace._Wnd2Handle;//预览窗口
                lpPreviewInfo.lChannel = 1;//预te览的设备通道
                lpPreviewInfo.dwStreamType = 1;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;


                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                var ret = CHCNetSDK.NET_DVR_RealPlay_V40(Globalspace._UserID2, ref lpPreviewInfo, null/*RealData*/, pUser);
                log.Info(Globalspace._UserID2 + "预览画面返回值：" + ret);
            }

            if (Globalspace._UserID3 > -1)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = Globalspace._Wnd3Handle;//预览窗口
                lpPreviewInfo.lChannel = 1;//预te览的设备通道
                lpPreviewInfo.dwStreamType = 1;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;


                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                var ret = CHCNetSDK.NET_DVR_RealPlay_V40(Globalspace._UserID3, ref lpPreviewInfo, null/*RealData*/, pUser);
                log.Info(Globalspace._UserID3 + "预览画面返回值：" + ret);
            }

            if (Globalspace._UserID4 > -1)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = Globalspace._Wnd4Handle;//预览窗口
                lpPreviewInfo.lChannel = 1;//预te览的设备通道
                lpPreviewInfo.dwStreamType = 1;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;


                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                var ret = CHCNetSDK.NET_DVR_RealPlay_V40(Globalspace._UserID4, ref lpPreviewInfo, null/*RealData*/, pUser);
                log.Info(Globalspace._UserID4 + "预览画面返回值：" + ret);
            }
        }

        public void SetTop()
        {
            WindowTopmost = !WindowTopmost;
            FixedPic = WindowTopmost ? _FixedPic : _UnfixedPic;
        }

        //protected override void OnClose()
        //{
        //    try
        //    {
        //        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        //        var ChargeSection = config.AppSettings;
        //        ChargeSection.Settings["Heigh"].Value = Heigh.ToString();
        //        ChargeSection.Settings["Width"].Value = Width.ToString();
        //        config.Save(ConfigurationSaveMode.Modified);
        //        ConfigurationManager.RefreshSection("appSettings");
        //    }
        //    catch (Exception e)
        //    {
        //        log.Info($"OnClose Error:{e.Message}");
        //    }
        //}
    }
}
