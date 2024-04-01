using log4net;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using static Monitor.IPCHelper;

namespace Monitor
{
    /// <summary>
    /// ShellView.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView : Window
    {
        private static readonly ILog log = LogManager.GetLogger("Monitor");

        public string MonitorSavePath
        {
            get
            {
                return ConfigurationManager.AppSettings["MonitorSavePath"] + "\\";
            }
        }

        public string PictureSavePath
        {
            get
            {
                return ((AppSettingsSection)ConfigurationManager.OpenExeConfiguration("车牌识别.exe").GetSection("appSettings")).Settings["LPRSavePath"].Value + "\\";
            }
        }

        public ShellView()
        {
            InitializeComponent();

            var _Heigh = double.Parse(ConfigurationManager.AppSettings["Heigh"]);
            var _Width = double.Parse(ConfigurationManager.AppSettings["Width"]);
            this.Width = _Width;
            this.Height = _Heigh;
            setGridSize();

            Globalspace._Wnd1Handle = RealPlayWnd1.Handle;
            Globalspace._Wnd2Handle = RealPlayWnd2.Handle;
            Globalspace._Wnd3Handle = RealPlayWnd3.Handle;
            Globalspace._Wnd4Handle = RealPlayWnd4.Handle;
        }

        //由于没有标题栏，需要在整个窗口里都可以拖动窗口位置
        private void Window_DragMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch { }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
        }

        private void Window_Topmost(object sender, RoutedEventArgs e)
        {
            Topmost = (bool)((CheckBox)sender).IsChecked;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }

        //通过此窗口句柄传递过来的消息
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            try
            {

                if (msg == WM_COPYDATA)
                {
                    COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT)); // 接收封装的消息
                    string[] recv = cds.lpData.Split(':'); // 获取消息内容

                    var picPath = PictureSavePath + "/" + recv[1] + "/" + recv[2];
                    if (!System.IO.Directory.Exists(picPath))
                    {
                        System.IO.Directory.CreateDirectory(picPath);
                    }

                    // 自定义行为       
                    if (recv[0] == "FirstWeighing")
                    {
                        //if (Globalspace._UserID1 != -1) ScreenShot(Globalspace._UserID1, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W1_M1.jpg");
                        //if (Globalspace._UserID2 != -1) ScreenShot(Globalspace._UserID2, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W1_M2.jpg");
                        //if (Globalspace._UserID3 != -1) ScreenShot(Globalspace._UserID3, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W1_M3.jpg");
                        //if (Globalspace._UserID4 != -1) ScreenShot(Globalspace._UserID4, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W1_M4.jpg");
                        if (Globalspace._UserID1 != -1) ScreenShot(Globalspace._UserID1, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W1_M1.jpg");
                        if (Globalspace._UserID2 != -1) ScreenShot(Globalspace._UserID2, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W1_M2.jpg");
                        if (Globalspace._UserID3 != -1) ScreenShot(Globalspace._UserID3, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W1_M3.jpg");
                        if (Globalspace._UserID4 != -1) ScreenShot(Globalspace._UserID4, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W1_M4.jpg");


                    }
                    if (recv[0] == "SecondWeighing")
                    {
                        //if (Globalspace._UserID1 != -1) ScreenShot(Globalspace._UserID1, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W2_M1.jpg");
                        //if (Globalspace._UserID2 != -1) ScreenShot(Globalspace._UserID2, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W2_M2.jpg");
                        //if (Globalspace._UserID3 != -1) ScreenShot(Globalspace._UserID3, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W2_M3.jpg");
                        //if (Globalspace._UserID4 != -1) ScreenShot(Globalspace._UserID4, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W2_M4.jpg");
                        if (Globalspace._UserID1 != -1) ScreenShot(Globalspace._UserID1, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W2_M1.jpg");
                        if (Globalspace._UserID2 != -1) ScreenShot(Globalspace._UserID2, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W2_M2.jpg");
                        if (Globalspace._UserID3 != -1) ScreenShot(Globalspace._UserID3, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W2_M3.jpg");
                        if (Globalspace._UserID4 != -1) ScreenShot(Globalspace._UserID4, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W2_M4.jpg");

                    }
                    if (recv[0] == "OnceWeighing")
                    {
                        //if (Globalspace._UserID1 != -1) ScreenShot(Globalspace._UserID1, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W_M1.jpg");
                        //if (Globalspace._UserID2 != -1) ScreenShot(Globalspace._UserID2, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W_M2.jpg");
                        //if (Globalspace._UserID3 != -1) ScreenShot(Globalspace._UserID3, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W_M3.jpg");
                        //if (Globalspace._UserID4 != -1) ScreenShot(Globalspace._UserID4, recv[1] + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_W_M4.jpg");
                        if (Globalspace._UserID1 != -1) ScreenShot(Globalspace._UserID1, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W_M1.jpg");
                        if (Globalspace._UserID2 != -1) ScreenShot(Globalspace._UserID2, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W_M2.jpg");
                        if (Globalspace._UserID3 != -1) ScreenShot(Globalspace._UserID3, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W_M3.jpg");
                        if (Globalspace._UserID4 != -1) ScreenShot(Globalspace._UserID4, recv[1] + "/" + recv[2] + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "_W_M4.jpg");
                    }

                    if (recv[0] == "StartWeighing")
                    {
                        try
                        {
                            StartCapture(Globalspace._UserID1, 1, recv[1], recv[2]);
                            StartCapture(Globalspace._UserID2, 2, recv[1], recv[2]);
                        }
                        catch (Exception ex)
                        {
                            log.Info($"StartWeighing error:{ex.Message}");
                        }

                        //if (Globalspace._UserID1 != -1)
                        //{
                        //    //2022-12-15 注释，因为这里占用资源大，所以暂时不启用

                        //    CHCNetSDK.NET_DVR_MakeKeyFrame(Globalspace._UserID1, 1);
                        //    //开始录像 Start recording
                        //    //var sVideoFileName = "Snap/Temp/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
                        //    //var sVideoFileName = MonitorSavePath+DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";

                        //    var videoPath = MonitorSavePath + "/" + recv[1] + "/" + recv[2];
                        //    if (!System.IO.Directory.Exists(videoPath))
                        //    {
                        //        System.IO.Directory.CreateDirectory(videoPath);
                        //    }

                        //    var sVideoFileName = videoPath + "/" + recv[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + ".mp4";
                        //    sVideoFileName = sVideoFileName.Replace("/", "\\");

                        //    if (!CHCNetSDK.NET_DVR_SaveRealData(Globalspace._UserID1, sVideoFileName))
                        //    {
                        //        var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        //        string str = "NET_DVR_SaveRealData failed, error code= " + iLastErr;
                        //        log.Error(str);
                        //    }
                        //    else
                        //    {
                        //        var delayStopTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
                        //        delayStopTimer.Start();
                        //        delayStopTimer.Tick += (sender, args) =>
                        //        {
                        //            delayStopTimer.Stop();

                        //            //停止录像 Stop recording
                        //            if (!CHCNetSDK.NET_DVR_StopSaveRealData(Globalspace._UserID1))
                        //            {
                        //                var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                        //                string str = "NET_DVR_StopSaveRealData failed, error code= " + iLastErr;
                        //                log.Error(str);
                        //            }
                        //            else
                        //            {
                        //                string str = "saved file is " + sVideoFileName;
                        //                log.Info(str);
                        //            }
                        //        };
                        //    }
                        //}
                        //else
                        //{
                        //    log.Info("录像失败，相机未打开");
                        //}
                    }

                    if (recv[0] == "EndWeighing")
                    {
                        try
                        {
                            EndCapture(Globalspace._UserID1, recv[1], recv[2]);
                            EndCapture(Globalspace._UserID2, recv[1], recv[2]);
                        }
                        catch (Exception ex)
                        {
                            log.Info($"StartWeighing error:{ex.Message}");
                        }
                    }
                }

            }
            catch (Exception e)
            {
                log.Info($"WndProc error:{e.Message}");

            }
            return hwnd;
        }

        /// <summary>
        /// 开始录像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recv1"></param>
        /// <param name="recv2"></param>
        private void StartCapture(int userId, int index, string recv1, string recv2)
        {
            if (userId != -1)
            {
                CHCNetSDK.NET_DVR_MakeKeyFrame(userId, 1);
                //开始录像 Start recording

                var videoPath = MonitorSavePath + "/" + recv1 + "/" + recv2;
                if (!System.IO.Directory.Exists(videoPath))
                {
                    System.IO.Directory.CreateDirectory(videoPath);
                }

                var sVideoFileName = videoPath + "/" + recv1 + "-" + DateTime.Now.ToString("yyyyMMddHHmm") + "-Capture_" + index + ".mp4";
                sVideoFileName = sVideoFileName.Replace("/", "\\");

                if (!CHCNetSDK.NET_DVR_SaveRealData(userId, sVideoFileName))
                {
                    var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string str = "准备保存录像失败, 错误代码= " + iLastErr;
                    log.Error(str);
                }
                //else
                //{
                //    var delayStopTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
                //    delayStopTimer.Start();
                //    delayStopTimer.Tick += (sender, args) =>
                //    {
                //        delayStopTimer.Stop();

                //        //停止录像 Stop recording
                //        if (!CHCNetSDK.NET_DVR_StopSaveRealData(userId))
                //        {
                //            var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                //            string str = "停止录像失败, 错误代码= " + iLastErr;
                //            log.Error(str);
                //        }
                //        else
                //        {
                //            string str = "成功保存录像： " + sVideoFileName;
                //            log.Info(str);
                //        }
                //    };
                //}
            }
            else
            {
                log.Info("录像失败，相机未打开");
            }
        }

        /// <summary>
        /// 停止录像
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="recv1"></param>
        /// <param name="recv2"></param>
        private void EndCapture(int userId, string recv1, string recv2)
        {
            if (userId != -1)
            {
                //CHCNetSDK.NET_DVR_MakeKeyFrame(userId, 1);
                //开始录像 Start recording

                //停止录像 Stop recording
                if (!CHCNetSDK.NET_DVR_StopSaveRealData(userId))
                {
                    var iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    string str = "停止录像失败, 错误代码= " + iLastErr;
                    log.Error(str);
                }
                else
                {
                    string str = "成功保存录像 ";
                    log.Info(str);
                }
            }
            else
            {
                log.Info("录像失败，相机未打开");
            }
        }


        private void ScreenShot(int userID, string sJpegPicFileName)
        {
            //sJpegPicFileName = "Snap/" + sJpegPicFileName;
            //sJpegPicFileName = MonitorSavePath+ sJpegPicFileName;
            sJpegPicFileName = sJpegPicFileName.Replace("/", "\\");
            sJpegPicFileName = PictureSavePath + sJpegPicFileName;

            CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA
            {
                wPicQuality = 0,
                wPicSize = 0xff
            };

            CHCNetSDK.NET_DVR_CaptureJPEGPicture(userID, 1, ref lpJpegPara, sJpegPicFileName);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            setGridSize();
        }

        private void setGridSize()
        {
            this.topPanel.Width = this.Width - 20;
            this.topPanel.Height = this.Height - 18;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var ChargeSection = config.AppSettings;
                ChargeSection.Settings["Heigh"].Value = this.topPanel.Height.ToString();
                ChargeSection.Settings["Width"].Value = this.topPanel.Width.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                log.Info($"OnClose Error:{ex.Message}");
            }
        }

        int i = 0;
        private void DockPanel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            i += 1;

            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 300);

            timer.Tick += (s, e1) => { timer.IsEnabled = false; i = 0; };

            timer.IsEnabled = true;

            if (i % 2 == 0)

            {

                timer.IsEnabled = false;

                i = 0;

                //this.WindowState = this.WindowState == WindowState.Maximized ?

                //              WindowState.Normal : WindowState.Maximized;
                this.WindowState = WindowState.Normal;

            }
        }
    }
}
