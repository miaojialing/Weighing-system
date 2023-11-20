using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.HardwareSDKS.VzClient;
using AJWPFAdmin.Core.Utils;
using Emgu.CV;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AJWPFAdmin.Core.HardwareSDKS.Models
{
    public class VzCarIdentificationDevice : DeviceInfo
    {
        /// <summary>
        /// 默认LED文本配置,设备初始化的时候由DeviceScreenViewModel赋值
        /// </summary>
        public string[] DefaultLEDTextArray { get; set; }

        private VzSocketClientSDK _sdk;

        private VideoCapture _videoCapture;

        private static readonly string[] _ledTextMap
            = new string[]
                    {
                        "00 64 FF FF 6E DL 00 04 ",
                        "00 01 01 05 00 R G 00 00 DL DATA",
                        "01 01 01 05 00 R G 00 00 DL DATA",
                        "02 01 01 05 00 R G 00 00 DL DATA",
                        "03 01 01 05 00 R G 00 00 DL DATA",
                        "0A VTL DATA"
                    };

        private static readonly string[] _ledColorReplaceArray = new string[] { "00", "FF" };

        private DelegateCommand<string> _toggleSwitchCmd;
        public DelegateCommand<string> ToggleSwitchCmd =>
            _toggleSwitchCmd ?? (_toggleSwitchCmd = new DelegateCommand<string>(ExecuteToggleSwitchCmd, CanExecuteToggleSwitchCmd));

        async void ExecuteToggleSwitchCmd(string isOpen)
        {
            await _sdk.ExecuteIOCtlCmdAsync(1);
            await Task.Delay(500);
            await _sdk.ExecuteIOCtlCmdAsync(0);

            await Task.Delay(3000);

            PlayDefaultTextToLED();
        }

        bool CanExecuteToggleSwitchCmd(string isOpen)
        {
            return !_deviceBusyOrError;
        }

        public override async Task OpenAsync()
        {

            var tcpPortAttr = typeof(DeviceType)
                .GetField(Type.ToString())!.GetCustomAttribute<DeviceTCPSocketPortAttribute>(false);

            _sdk = new VzSocketClientSDK(IP, tcpPortAttr?.Port ?? Port);

            _sdk.ErrorEvent += OnSDKErrorEvent;

            _sdk.RtspUriResponseEvent += OnSDKRTSPEvent;

            if (!OnlyMonitor)
            {
                _sdk.IVSCarPlateResultEvent += OnIVSCarPlateResultEvent;
            }

            var success = await _sdk.ConnectAsync();

            if (success)
            {
                await _sdk.ExecuteRTSPUriCmdAsync();
            }
            else
            {
                _deviceBusyOrError = true;
                ToggleSwitchCmd.RaiseCanExecuteChanged();
            }

            try
            {
                _deviceHandle = VzClientSDK.VzLPRClient_Open(IP, (ushort)Port, LoginName, LoginPassword);

                _serialPortHandle = VzClientSDK.VzLPRClient_SerialStart(_deviceHandle, 0, null, IntPtr.Zero);

                PlayDefaultTextToLED();
            }
            catch (Exception e)
            {
                Logger.Error($"{DeviceDescription}:LED串口控制失败:{e.Message}");
            }

        }

        public override void Close()
        {
            
            _sdk.ErrorEvent -= OnSDKErrorEvent;
            _sdk.RtspUriResponseEvent -= OnSDKRTSPEvent;
            _sdk.IVSCarPlateResultEvent -= OnIVSCarPlateResultEvent;
            _sdk.Shutdown();

            if (_videoCapture != null)
            {
                _videoCapture.ImageGrabbed -= OnVideoCaptureGrabbed;
            }
            
            _videoCapture?.Stop();
            _videoCapture?.Dispose();


            try
            {
                VzClientSDK.VzLPRClient_Close(_deviceHandle);
            }
            catch
            {
            }

        }

        private void OnSDKErrorEvent(object sender, VzSDKErrorEventArgs e)
        {
            if (e.NotifyToUser)
            {
                _ctrl.Dispatcher.Invoke(async () =>
                {
                    await CommonUtil.ShowAlertDialogAsync(new MaterialDesignExtensions.Controls.AlertDialogArguments
                    {
                        Title = $"设备:{Code}错误",
                        Message = $"{DeviceDescription} : {e.ErrMessage}"
                    });
                });
                return;
            }

            Logger.Error($"{DeviceDescription}:发生错误:{e.ErrMessage}", e.ErrorData);

        }

        private void OnIVSCarPlateResultEvent(object sender, VzCarPlateResultModel e)
        {
            _ctrl.Dispatcher.Invoke(() =>
            {
                CarIdentificationPlateResult = new CarIdentificationResult
                {
                    CarNo =  e.PlateResult.license,
                    FullImgFile = e.FullImgFile,
                    ClipImgFile =  e.ClipImgFile
                }; 
                EventAggregator.GetEvent<DeviceNotifyEvent>().Publish(this);
            });
        }

        private async void OnSDKRTSPEvent(object sender, string uri)
        {
            _videoCapture = new VideoCapture(uri);
            _videoCapture.ImageGrabbed += OnVideoCaptureGrabbed;
            _videoCapture.Start();

            await _sdk.ExecuteIVSResultCmdAsync();
        }

        protected override void OnDeviceReceiveCmdEvent(DeviceReceiveCmdEventArgs e)
        {
            if (e.DeviceId != Id)
            {
                return;
            }

            switch (e.Type)
            {
                case DeviceReceiveCmdType.开闸:
                    ExecuteOpenGateCmd((DeviceOpenGateCmdParameter)e.Data);
                    break;
                default:
                    break;
            }
        }

        private void OnVideoCaptureGrabbed(object sender, EventArgs e)
        {
            try
            {
                // 原图
                var frame = new Mat();
                // 改变后的图片
                var frame2 = new Mat();
                // 设定长度 这里 width 240 , high 160 可根据需要去设定
                var s = new System.Drawing.Size((int)RenderWidth, (int)RenderHeight);
                lock (_lock)
                {
                    if (!_videoCapture.Retrieve(frame))
                    {
                        frame.Dispose();
                        return;
                    }
                    if (frame.IsEmpty)
                        return;

                    /*
                        void resize(InputArray src, OutputArray dst, Size dsize, double fx=0, double fy=0, int interpolation=INTER_LINEAR )
                        各个参数的意义比较直观，但是需要注意的是dsize与fx和fy必须不能同时为零，也就是说要么dsize不为零而fx与fy同时可以为0，要么dsize为0而fx与fy不同时为0；resize函数的目标大小可以是任意的大小，可以不保持长宽比率，删除的像素或者新增的像素值通过interpolation控制；
                    */
                    CvInvoke.Resize(frame, frame2, s, 0, 0);
                    frame2.ToBitmap();
                    // 也可以使用 pyrDown 缩小1/4  pyrUp 放大1/4
                    // CvInvoke.PyrDown(frame, frame);//缩小
                    _ctrl.Dispatcher.Invoke(new Action(() =>
                    {
                        _ctrl.Source = BitmapSourceConvert.ToBitmapSource(frame2.ToBitmap(true));
                    }));

                    //显示图片 可以使用Emgu CV 提供的 ImageBox显示视频, 也可以转成 BitmapSource显示。
                    frame2.Dispose();
                    //略
                    //  _capture.Dispose();停止关闭
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{DeviceDescription}:实时视频发生错误:{ex.Message}", ex);
            }
        }

        private void ExecuteOpenGateCmd(DeviceOpenGateCmdParameter data)
        {
            if (data != null)
            {
                data.PreProcessLEDTextLines();

                PlayTextToLED(data.TTSText, "g", data.LEDTextLines);
            }

            if ((data?.Open).GetValueOrDefault())
            {
                ExecuteToggleSwitchCmd(true.ToString());
            }
        }

        private void PlayDefaultTextToLED()
        {
            var @params = new DeviceOpenGateCmdParameter { LEDTextLines = DefaultLEDTextArray };

            @params.PreProcessLEDTextLines();

            PlayTextToLED(string.Empty, "g", @params.LEDTextLines);
        }

        private void PlayTextToLED(string tts, string rgb, params string[] lines)
        {
            var line1 = LEDTextToHEXString(CleanAndPrepareLEDText(lines.ElementAtOrDefault(0)));
            var line2 = LEDTextToHEXString(CleanAndPrepareLEDText(lines.ElementAtOrDefault(1)));
            var line3 = LEDTextToHEXString(CleanAndPrepareLEDText(lines.ElementAtOrDefault(2)));
            var line4 = LEDTextToHEXString(CleanAndPrepareLEDText(lines.ElementAtOrDefault(3)));

            tts = LEDTextToHEXString(tts);

            var line1Len = ComputeLEDHEXTextLength(line1);
            var line2Len = ComputeLEDHEXTextLength(line2);
            var line3Len = ComputeLEDHEXTextLength(line3);
            var line4Len = ComputeLEDHEXTextLength(line4);
            var ttsLen = ComputeLEDHEXTextLength(tts);

            var formatHead = _ledTextMap.ElementAtOrDefault(0);
            var formatLine1 = _ledTextMap.ElementAtOrDefault(1);
            var formatLine2 = _ledTextMap.ElementAtOrDefault(2);
            var formatLine3 = _ledTextMap.ElementAtOrDefault(3);
            var formatLine4 = _ledTextMap.ElementAtOrDefault(4);
            var formatTTS = _ledTextMap.ElementAtOrDefault(5);

            var replaceStr = rgb == "g" ? _ledColorReplaceArray : _ledColorReplaceArray.Reverse().ToArray();

            formatLine1 = formatLine1.Replace("R", replaceStr[0]).Replace("G", replaceStr[1]);
            formatLine2 = formatLine2.Replace("R", replaceStr[0]).Replace("G", replaceStr[1]);
            formatLine3 = formatLine3.Replace("R", replaceStr[0]).Replace("G", replaceStr[1]);
            formatLine4 = formatLine4.Replace("R", replaceStr[0]).Replace("G", replaceStr[1]);


            formatLine1 = formatLine1.Replace("DL", line1Len).Replace("DATA", line1);
            formatLine2 = formatLine2.Replace("DL", line2Len).Replace("DATA", line2);
            formatLine3 = formatLine3.Replace("DL", line3Len).Replace("DATA", line3);
            formatLine4 = formatLine4.Replace("DL", line4Len).Replace("DATA", line4);
            formatTTS = formatTTS.Replace("VTL", ttsLen).Replace("DATA", tts);

            var data = formatLine1 + "0D " + formatLine2 + "0D " + formatLine3 + "0D " + formatLine4 + "00 " + formatTTS + "00 ";
            var dataLen = ComputeLEDHEXTextLength(data, 2);

            formatHead = formatHead.Replace("DL", dataLen);
            data = formatHead + data;
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
                    Logger.Info($"{DeviceDescription}:LED发送消息失败: 16进制数据输入格式不正确 :{new string(txt_buf)}");
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
                        Logger.Info($"{DeviceDescription}:LED发送消息失败: 16进制数据输入格式不正确 :{new string(strHex)}");
                        return;
                    }
                }

                string hex_value = new string(strHex);
                uc = byte.Parse(hex_value, System.Globalization.NumberStyles.HexNumber);
                send_buf[index] = uc;
                index++;
            }

            try
            {
                var hObject = GCHandle.Alloc(send_buf, GCHandleType.Pinned);
                var pObject = hObject.AddrOfPinnedObject();

                var a = VzClientSDK.VzLPRClient_SerialSend(_serialPortHandle, pObject, index);

                if (hObject.IsAllocated)
                    hObject.Free();
            }
            catch (Exception e)
            {
                Logger.Error($"{DeviceDescription}:LED发送消息失败: {e.Message}");
            }

        }

        /// <summary>
        /// 清除文字换行和空格
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string CleanAndPrepareLEDText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return " ";
            }

            return text.Replace(" ", string.Empty)
                .Replace("\0", string.Empty)
                .Replace("\r\n", string.Empty).Replace("\n", string.Empty).Trim();

        }

        /// <summary>
        /// 将LED文字转换成HEX String
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string LEDTextToHEXString(string text)
        {
            var result = string.Empty;
            var arrByte = Encoding.GetEncoding("GB2312").GetBytes(text);
            for (int i = 0; i < arrByte.Length; i++)
            {
                result += Convert.ToString(arrByte[i], 16) + " ";
            }

            return result.ToUpper();
        }

        private string ComputeLEDHEXTextLength(string text, int extra = 0)
        {
            return Convert.ToString(text.Length / 3 + extra, 16).ToUpper().PadLeft(2, '0');
        }
    }
}
