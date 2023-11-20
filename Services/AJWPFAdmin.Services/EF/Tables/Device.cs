using AJWPFAdmin.Core.CommonEntity;
using AJWPFAdmin.Core.Enums;
using AJWPFAdmin.Core.HardwareSDKS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJWPFAdmin.Services.EF.Tables
{
    /// <summary>
    /// 设备表
    /// </summary>
    [Comment("设备表")]
    public partial class Device : CommonTableEntity
    {

        /// <summary>
        /// 关联岗亭Id
        /// </summary>
        public long WatchhouseId { get; set; }


        /// <summary>
        /// 关联通道id
        /// </summary>
        public long PassagewayId { get; set; }

        /// <summary>
        /// 导航属性,关联通道数据
        /// </summary>
        [ForeignKey("PassagewayId")]
        public virtual Passageway Passageway { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public DeviceType Type { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        [Comment("编号")]
        [MaxLength(50)]
        public string Code { get; set; }

        /// <summary>
        /// 设备IP地址
        /// </summary>
        [Comment("设备IP地址")]
        [MaxLength(50)]
        public string IP { get; set; }

        /// <summary>
        /// 设备串口号
        /// </summary>
        [Comment("设备串口号")]
        [MaxLength(10)]
        public SerialPortType SerialPort { get; set; }

        /// <summary>
        /// 设备端口号
        /// </summary>
        [Comment("设备端口号")]
        public int Port { get; set; }

        /// <summary>
        /// 登录账户名
        /// </summary>
        [Comment("登录账户名")]
        [MaxLength(50)]
        public string LoginName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Comment("登录密码")]
        [MaxLength(100)]
        public string LoginPassword { get; set; }

        /// <summary>
        /// 仅作为监控
        /// </summary>
        [Comment("是否仅作为监控")]
        public bool OnlyMonitor { get; set; }

        /// <summary>
        /// 关联岗亭名称,动态赋值
        /// </summary>
        [NotMapped]
        public string WatchhouseName { get; set; }

        /// <summary>
        /// 关联通道名称,动态赋值
        /// </summary>
        [NotMapped]
        public string PassagewayName { get; set; }

        /// <summary>
        /// 渲染高度,动态计算
        /// </summary>
        [NotMapped]
        public double RenderHeight { get; set; }

        /// <summary>
        /// 渲染宽度,动态计算
        /// </summary>
        [NotMapped]
        public double RenderWidth { get; set; }


        public static explicit operator DeviceInfo(Device data)
        {
            switch (data.Type)
            {
                case DeviceType.车牌识别相机_臻识:
                    return new VzCarIdentificationDevice
                    {
                        Id = data.Id,
                        Type = data.Type,
                        Code = data.Code,
                        Port = data.Port,
                        IP = data.IP,
                        LoginName = data.LoginName,
                        LoginPassword = data.LoginPassword,
                        RenderWidth = data.RenderWidth,
                        RenderHeight = data.RenderHeight,
                        OnlyMonitor = data.OnlyMonitor
                    };
                case DeviceType.高频读头:
                    return new SioReaderDevice
                    {
                        Id = data.Id,
                        Type = data.Type,
                        Code = data.Code,
                        Port = data.Port,
                        IP = data.IP,
                        LoginName = data.LoginName,
                        LoginPassword = data.LoginPassword,
                        RenderWidth = data.RenderWidth,
                        RenderHeight = data.RenderHeight,
                        SerialPort = data.SerialPort
                    };
                case DeviceType.监控相机_海康:
                    return new HIKVisionDevice
                    {
                        Id = data.Id,
                        Type = data.Type,
                        Code = data.Code,
                        Port = data.Port,
                        IP = data.IP,
                        LoginName = data.LoginName,
                        LoginPassword = data.LoginPassword,
                        RenderWidth = data.RenderWidth,
                        RenderHeight = data.RenderHeight,
                        OnlyMonitor = data.OnlyMonitor
                    };
                default:
                    return new DeviceInfo
                    {
                        Id = data.Id,
                        Type = data.Type,
                        Code = data.Code,
                        Port = data.Port,
                        IP = data.IP,
                        LoginName = data.LoginName,
                        LoginPassword = data.LoginPassword,
                        RenderWidth = data.RenderWidth,
                        RenderHeight = data.RenderHeight,
                        OnlyMonitor = data.OnlyMonitor,
                        SerialPort = data.SerialPort
                    };
            }

        }
    }
}
