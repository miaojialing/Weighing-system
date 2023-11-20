using AJWPFAdmin.Core.HardwareSDKS.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.GlobalEvents
{
    /// <summary>
    /// 设备主动通知事件,比如车牌识别了,要通知其他组件
    /// </summary>
    public class DeviceNotifyEvent : PubSubEvent<DeviceInfo>
    {

    }
}
