using AJWPFAdmin.Core.GlobalEvents;
using AJWPFAdmin.Core.HardwareSDKS.ADSioReader;
using AJWPFAdmin.Core.Utils;
using log4net;
using Masuit.Tools.Reflection;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.HardwareSDKS.Models
{
    public class SioReaderDevice : DeviceInfo
    {

        private SioBase _sioSDK;
        private RcpBase _rcpBase;

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set { SetProperty(ref _statusText, value); }
        }

        public override Task OpenAsync()
        {
            _ctrl.Source = CommonUtil.GetImageFromLocalOrHttp("/images/device-reader.jpg");

            try
            {
                _sioSDK = new SioNet();
                _rcpBase = new RcpBase();

                _sioSDK.onStatus += OnSioSDKStatusChange;
                _sioSDK.onReceived += OnSioSDKReceived;
                _rcpBase.RxRspParsed += OnSioRCPBaseParsed;

                return Task.Run(() =>
                {
                    _sioSDK.Connect(IP, Port);
                });
            }
            catch (Exception e)
            {
                Logger
                .Error($"{DeviceDescription}:高频读头串口打开失败:{e.Message}", e);
            }

            return Task.CompletedTask;

        }

        private void OnSioRCPBaseParsed(object sender, ProtocolEventArgs e)
        {
            var protocolPacket = e.Protocol;

            switch (protocolPacket.Code)
            {
                case RcpBase.RCP_MM_READ_C_UII:
                    if (protocolPacket.Type == 2 || protocolPacket.Type == 5)
                    {
                        int pcepclen = ((protocolPacket.Payload[1] >> 3) + 1) * 2;
                        int datalen = protocolPacket.Length - 2;//去掉天线号去掉rssi
                        var cp = new ADSioTagInfo
                        {
                            TagType = TagType.TYPE_6C,
                            Length = datalen,//去掉天线号去掉RSSI
                            Antenna = protocolPacket.Payload[0],
                            PCData = ADSioTagInfo.GetData(protocolPacket.Payload, 1, 2),
                            EPCData = ADSioTagInfo.GetData(protocolPacket.Payload, 3, pcepclen - 2),
                            Rssi = GetRssi(protocolPacket.Payload[protocolPacket.Length - 1]) + "dBm"
                        };
                        if ((datalen - pcepclen) > 0) cp.DataBytes = ADSioTagInfo.GetData(protocolPacket.Payload, 1 + pcepclen, datalen - pcepclen);

                        if (!CommonRegex.CARNO.IsMatch(cp.EPCString))
                        {
                            return;
                        }

                        _ctrl.Dispatcher.Invoke(() =>
                        {
                            CarIdentificationPlateResult = new CarIdentificationResult
                            {
                                CarNo = cp.EPCString
                            };
                            EventAggregator.GetEvent<DeviceNotifyEvent>().Publish(this);
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnSioSDKReceived(object sender, ReceivedEventArgs e)
        {
            _rcpBase.ReciveBytePkt(e.Data);
        }

        private void OnSioSDKStatusChange(object sender, StatusEventArgs e)
        {
            _ctrl.Dispatcher.Invoke(() =>
            {
                StatusText = ((SioSDKReaderStatus)e.Status).GetDescription();
            });
        }

        private string GetRssi(byte rssi)
        {
            int rssidBm = (sbyte)rssi; // rssidBm is negative && in bytes
            rssidBm -= Convert.ToInt32("-20", 10);
            rssidBm -= Convert.ToInt32("3", 10);
            return rssidBm.ToString();
        }

        public override void Close()
        {
            _sioSDK?.DisConnect();
            _rcpBase?.Dispose();
        }
    }
}
