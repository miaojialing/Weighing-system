using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.HardwareSDKS.VzClient;
using AJWPFAdmin.Core.Logger;
using AJWPFAdmin.Core.Utils;
using Emgu.CV;
using Emgu.Util;
using log4net;
using Masuit.Tools;
using Masuit.Tools.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using SixLabors.ImageSharp.Drawing.Processing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AJWPFAdmin.Core.HardwareSDKS.Models
{
    public class DeviceInfo : BindableBase
    {
        /// <summary>
        /// 设备初始化成功后由 DeviceScreenViewModel 注入的
        /// </summary>
        public IEventAggregator EventAggregator { get; set; }

        /// <summary>
        /// 设备初始化成功后由 DeviceScreenViewModel 注入的, 写一些错误日志,好排查问题
        /// </summary>
        public AJLog4NetLogger Logger { get; set; }

        public long Id { get; set; }

        private string _code;
        /// <summary>
        /// 设备编号
        /// </summary>
        public string Code
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }

        private DeviceType _type;
        /// <summary>
        /// 设备编号
        /// </summary>
        public DeviceType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        private SerialPortType _serialPortType;
        /// <summary>
        /// 设备串口号
        /// </summary>
        public SerialPortType SerialPort
        {
            get { return _serialPortType; }
            set { SetProperty(ref _serialPortType, value); }
        }

        private string _ip;
        /// <summary>
        /// 设备IP地址
        /// </summary>
        public string IP
        {
            get { return _ip; }
            set { SetProperty(ref _ip, value); }
        }


        /// <summary>
        /// 设备端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 登录账户名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPassword { get; set; }

        /// <summary>
        /// 进出方向类型,动态赋值
        /// </summary>
        public PassagewayDirection Direction { get; set; }

        protected string DeviceDescription
        {
            get
            {
                return $"{WatchhouseName}>{PassagewayName}>{Code}";
            }
        }

        private bool _mouseHover;
        public bool MouseHover
        {
            get { return _mouseHover; }
            set { SetProperty(ref _mouseHover, value); }
        }

        private CarIdentificationResult _carIdentificationResult;
        /// <summary>
        /// 当前识别的车牌信息
        /// </summary>
        public CarIdentificationResult CarIdentificationPlateResult
        {
            get { return _carIdentificationResult; }
            set { SetProperty(ref _carIdentificationResult, value); }
        }

        private double _renderHeight;
        /// <summary>
        /// 渲染高度,动态计算
        /// </summary>
        public double RenderHeight
        {
            get { return _renderHeight; }
            set { SetProperty(ref _renderHeight, value); }
        }

        private double _renderWidth;
        /// <summary>
        /// 渲染宽度,动态计算
        /// </summary>
        public double RenderWidth
        {
            get { return _renderWidth; }
            set { SetProperty(ref _renderWidth, value); }
        }

        /// <summary>
        /// 关联岗亭id,动态赋值
        /// </summary>
        public long WatchhouseId { get; set; }

        /// <summary>
        /// 关联岗亭名称,动态赋值
        /// </summary>
        public string WatchhouseName { get; set; }

        /// <summary>
        /// 关联通道id, 动态赋值
        /// </summary>
        public long PassagewayId { get; set; }

        /// <summary>
        /// 关联通道名称,动态赋值
        /// </summary>
        public string PassagewayName { get; set; }

        /// <summary>
        /// 是否仅作为监控
        /// </summary>
        public bool OnlyMonitor { get; set; }

        /// <summary>
        /// 设备控制句柄,一般是dllimport 之类的硬件SDK赋值使用
        /// </summary>
        protected int _deviceHandle;

        /// <summary>
        /// 串口控制句柄
        /// </summary>
        protected int _serialPortHandle;

        protected bool _deviceBusyOrError;

        private DelegateCommand _onMouseEnterCmd;
        public DelegateCommand OnMouseEnterCmd =>
            _onMouseEnterCmd ?? (_onMouseEnterCmd = new DelegateCommand(ExecuteOnMouseEnterCmd));

        void ExecuteOnMouseEnterCmd()
        {
            MouseHover = true;
        }

        private DelegateCommand _onMouseLeaveCmd;
        public DelegateCommand OnMouseLeaveCmd =>
            _onMouseLeaveCmd ?? (_onMouseLeaveCmd = new DelegateCommand(ExecuteOnMouseLeaveCmd));

        void ExecuteOnMouseLeaveCmd()
        {
            MouseHover = false;
        }

        private DelegateCommand<Image> _deviceControlLoadCmd;
        public DelegateCommand<Image> DeviceControlLoadCmd =>
            _deviceControlLoadCmd ?? (_deviceControlLoadCmd = new DelegateCommand<Image>(ExecuteDeviceControlLoadCmd));

        async void ExecuteDeviceControlLoadCmd(Image ctrl)
        {
            Init(ctrl);
            await OpenAsync();
        }

        protected Image _ctrl;

        private DeviceReceiveCmdEvent _deviceReceiveCmdEvent;

        protected void Init(Image ctrl)
        {
            _ctrl = ctrl;

            _deviceReceiveCmdEvent = EventAggregator.GetEvent<DeviceReceiveCmdEvent>();

            _deviceReceiveCmdEvent.Subscribe(OnDeviceReceiveCmdEvent);
        }

        public virtual Task OpenAsync()
        {
            return Task.CompletedTask;
        }

        protected virtual void OnDeviceReceiveCmdEvent(DeviceReceiveCmdEventArgs e)
        {

        }

        public virtual void Close()
        {

        }

        public virtual string GetCapture()
        {
            return string.Empty;
        }
    }
}
