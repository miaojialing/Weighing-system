using AJWPFAdmin.Core.HardwareSDKS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.Enums
{
    public enum DeviceType : short
    {
        [DeviceTCPSocketPort(Port = 8131)]
        车牌识别相机_臻识,
        高频读头,
        监控相机_海康
    }
}
