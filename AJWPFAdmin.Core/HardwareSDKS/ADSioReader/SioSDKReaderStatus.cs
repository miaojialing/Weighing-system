using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Core.HardwareSDKS.ADSioReader
{
    public enum SioSDKReaderStatus
    {
        [Description("操作成功")]
        SUCCEED = 0,
        [Description("操作失败")]
        FAIL = 1,
        [Description("连接成功")]
        CONNECT_OK = 2,
        [Description("连接成功")]
        CONNECT_FAIL = 3,
        [Description("断开连接成功")]
        DISCONNECT_OK = 4,
        [Description("断开连接异常")]
        DISCONNECT_EXCEPT = 5,
        [Description("连接_调试")]
        CONNECTING_DEV = 6,
        [Description("连接_调试成功")]
        CONNECT_DEV_OK = 7,
        [Description("SUCCEED_NON")]
        SUCCEED_NON = 10,
        [Description("卡错误")]
        ERROR_CARD = 100,
        [Description("操作错误")]
        ERROR_ACTION = 101,
        [Description("连接错误")]
        ERROR_CONNECT = 201,
        [Description("获取错误")]
        ERROR_GET = 202,
        [Description("设置错误")]
        ERROR_SET = 203,
        [Description("超时")]
        TIMEOUT_SET = 204,
        [Description("发送错误")]
        ERROR_SEND = 205,
        [Description("接收错误")]
        ERROR_RECIVE = 206,
        [Description("发送超时")]
        TIMEOUT_SEND = 208,
        [Description("接收超时")]
        TIMEOUT_RECIVE = 209,
        [Description("其他")]
        OTHER = 254
    }
}
