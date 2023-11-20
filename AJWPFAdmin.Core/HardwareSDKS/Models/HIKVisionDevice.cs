using AJWPFAdmin.Core.Utils;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.HardwareSDKS.Models
{
    public class HIKVisionDevice : DeviceInfo
    {
        private VideoCapture _videoCapture;

        public override async Task OpenAsync()
        {
            _videoCapture = new VideoCapture($"rtsp://{LoginName}:{LoginPassword}@{IP}:{Port}/h264/ch1/main/av_stream");
            _videoCapture.ImageGrabbed += OnVideoCaptureGrabbed;

            await Task.Run(() =>
            {
                _videoCapture.Start();
            });
        }

        public override void Close()
        {
            _videoCapture?.Stop();
            _videoCapture?.Dispose();
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
    }
}
